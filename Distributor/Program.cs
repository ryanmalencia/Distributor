using System;
using System.Diagnostics;
using System.Threading;
using DataTypes;

namespace Distributor
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Console.WriteLine("Distributing jobs...");
                AgentCollection agents = DistributorLogic.GetAgents();
                JobCollection jobs = DistributorLogic.GetJobs();
                foreach (Agent agent in agents.Agents)
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
                watch.Stop();
                Console.WriteLine("Done Distributing. Redistributing in 10 seconds.");
                Console.WriteLine(watch.Elapsed.TotalSeconds + " Seconds to Distribute");
                Thread.Sleep(10000);
            }
        }
    }
}
