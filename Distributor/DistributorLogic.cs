using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using DataTypes;
using WebAPIClient.APICalls;

namespace Distributor
{
    public class DistributorLogic
    {
        public static void SendJob(Agent agent)
        {
            AgentAPI.SetQueued(agent.Name);
            Thread thread = new Thread(SendTheJob);
            thread.Start(agent);
        }

        public static void SendTheJob(object theagent)
        {
            Agent agent = (Agent)theagent;
            using (var client = new WebClient())
            {
                try
                {
                    client.UploadString("http://" + agent.IP + "/api/machine/give", "");
                    Console.WriteLine("Sent Job to Agent " + agent.Name);
                }
                catch (Exception e)
                {
                    AgentAPI.SetDead(agent.Name);
                    Console.WriteLine("Agent " + agent.Name + " may not be running");
                }
            }
        }
    }
}
