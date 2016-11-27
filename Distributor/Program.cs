using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
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
                AgentCollection agents = AgentAPI.GetAllAgents();
                foreach(Agent agent in agents.machines)
                {
                    if (agent.IsIdle())
                    {
                        DistributorLogic.SendJob(agent);
                    }
                }
                Thread.Sleep(10000);
            }
        }
    }
}
