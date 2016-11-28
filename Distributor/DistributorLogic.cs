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

        public static void SendTheJob(object jobinfo)
        {
            string info = (string)jobinfo;
            Agent agent = JsonConvert.DeserializeObject<Agent>(info.Split('?')[0]);
            Job job = JsonConvert.DeserializeObject<Job>(info.Split('?')[1]);
            using (var client = new WebClient())
            {
                try
                {
                    client.Headers.Add("content-type", "application/json");
                    client.UploadString("http://" + agent.IP + "/api/machine/give", "PUT",info.Split('?')[1]);
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
    }
}
