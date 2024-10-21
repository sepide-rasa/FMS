using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR;

namespace NewFMS
{
    [HubName("S_hub")]
    public class SignalrHub : Hub
    {
        public void SendWight(string Weight)
        {
            if (Clients != null)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<SignalrHub>();

                context.Clients.All.LoadVazn_Baskool(Weight);
            }
        }
    }
}