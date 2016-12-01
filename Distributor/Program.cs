using System;
using System.Threading;
using WebAPIClient.APICalls;
using DataTypes;

namespace Distributor
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Console.WriteLine("Distributing jobs...");
                AgentCollection agents = AgentAPI.GetIdleAgents();
                JobCollection jobs = JobAPI.GetAllJobs();
                jobs.Jobs.Sort();
                foreach (Agent agent in agents.machines)
                {
                    foreach (Job job in jobs.Jobs)
                    {
                        if (agent.IsIdle() && job.Distributed == 0)
                        {
                            if (job.Last_Finished >= job.Last_Distributed)
                            {
                                DistributorLogic.SendJob(agent, job);
                                job.Distributed = 1;
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine("Done Distributing. Redistributing in 10 seconds.");
                Thread.Sleep(10000);
            }
        }
    }
}
