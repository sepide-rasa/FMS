using Microsoft.AspNet.SignalR;
using NewFMS.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS
{
    public class Functions
    {
        public static void SendProgress(string progressMessage, int progressCount, int totalItems)
        { 
            //IN ORDER TO INVOKE SIGNALR FUNCTIONALITY DIRECTLY FROM SERVER SIDE WE MUST USE THIS
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            //CALCULATING PERCENTAGE BASED ON THE PARAMETERS SENT
            var percentage = (progressCount * 100) / totalItems;

            //PUSHING DATA TO ALL CLIENTS
            hubContext.Clients.All.AddProgress(progressMessage, percentage);
        }

        //public static void SendWight(int Weight)
        //{
        //    var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
        //    hubContext.Clients.All.AddProgress(Weight);
        //}
    }
}