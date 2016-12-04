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
                AgentCollection agents = DistributorLogic.GetAgents();
                JobCollection jobs = DistributorLogic.GetJobs();
                foreach (Agent agent in agents.machines)
                {
                    if (agent.IsIdle())
                    {
                        foreach (Job job in jobs.Jobs)
                        {
                            if (job.Distributed == 0)
                            {
                                DistributorLogic.SendJob(agent, job);
                                job.Distributed = 1;
                                break;
                            }
                            else
                            {
                                if (job.Started == 0)
                                {
                                    DistributorLogic.SendJob(agent, job);
                                    job.Distributed = 1;
                                    break;
                                }
                            }
                        }
                    }
                    else if(!agent.IsDead())
                    {
                        DistributorLogic.CheckAgent(agent);
                    }
                }
                Console.WriteLine("Done Distributing. Redistributing in 1 seconds.");
                Thread.Sleep(1000);
            }
        }
    }
}
