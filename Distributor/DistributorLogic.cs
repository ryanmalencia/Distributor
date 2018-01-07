using System;
using System.Net;
using System.Threading;
using DataTypes;
using WebAPIClient.APICalls;
using Newtonsoft.Json;

namespace Distributor
{
    public class DistributorLogic
    {
        public static void SendJob(Agent agent, Job job)
        {
            AgentAPI.SetQueued(agent.Name);
            Thread thread = new Thread(SendTheJob);
            string jobinfo = JsonConvert.SerializeObject(agent) + "?" + JsonConvert.SerializeObject(job);
            Console.WriteLine("Sending Job " + job.JobName + " to Agent " + agent.Name + "...");
            thread.Start(jobinfo);
        }

        private static void SendTheJob(object jobinfo)
        {
            string info = (string)jobinfo;
            Agent agent = JsonConvert.DeserializeObject<Agent>(info.Split('?')[0]);
            Job job = JsonConvert.DeserializeObject<Job>(info.Split('?')[1]);
            using (var client = new WebClient())
            {
                try
                {
                    client.Headers.Add("content-type", "application/json");
                    client.UploadString("http://" + agent.IP + "/api/machine/give/" + job.JobID, "PUT", "");
                    JobAPI.SetJobDist(job);
                }
                catch (Exception)
                {
                    AgentAPI.SetDead(agent.Name);
                    JobAPI.ResetJob(job);
                    Console.WriteLine("Agent " + agent.Name + " may not be running");
                }
            }
        }
        
        public static void CheckAgent(Agent agent)
        {
            Thread thread = new Thread(CheckTheAgent);
            thread.Start(agent);
        }

        private static void CheckTheAgent(object agent)
        {
            Agent theagent = (Agent)agent;
            using (var client = new WebClient())
            {
                try
                {
                    client.Headers.Add("content-type", "application/json");
                    client.DownloadData("http://" + theagent.IP + "/api/machine/getstatus");
                }
                catch (Exception)
                {
                    AgentAPI.SetDead(theagent.Name);
                    if (theagent.fk_job != 0)
                    {
                        JobAPI.ResetJob(JobAPI.GetById(theagent.fk_job));
                    }
                    Console.WriteLine("Agent " + theagent.Name + " may not be running");
                }
            }
        }

        /// <summary>
        /// Get All Agents
        /// </summary>
        /// <returns>Agent Collection of Idle Agents</returns>
        public static AgentCollection GetAgents()
        {
            return AgentAPI.GetAllAgents();
        }

        /// <summary>
        /// Get all jobs
        /// </summary>
        /// <returns>Sorted Job Collection</returns>
        public static JobCollection GetJobs()
        {
            JobCollection jobs = JobAPI.GetJobsToRun();
            jobs.Jobs.Sort();
            return jobs;
        }
    }
}
