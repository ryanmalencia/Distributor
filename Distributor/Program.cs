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

                foreach (Agent agent in agents.machines)
                {
                    JobCollection jobs = JobAPI.GetAllJobs();
                    foreach (Job job in jobs.Jobs)
                    {
                        if (agent.IsIdle() && job.Distributed == 0)
                        {
                            DistributorLogic.SendJob(agent, job);
                            break;
                        }
                    }
                }
                Thread.Sleep(10000);
            }
        }
    }
}
