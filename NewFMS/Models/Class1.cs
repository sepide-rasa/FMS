using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Filters;
using System.Web.Mvc;
using NewFMS;

namespace NewFMS.Models
{
    /// </summary>
    /// <remarks>
    /// Uses the current System.Web.Caching.Cache to store each client request to the decorated route.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ThrottleAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        /// <summary>
        /// A unique name for this Throttle.
        /// </summary>
        /// <remarks>
        /// We'll be inserting a Cache record based on this name and client IP, e.g. "Name-192.168.0.1"
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// The number of seconds clients must wait before executing this decorated route again.
        /// </summary>
        //public int Seconds { get; set; }

        /// <summary>
        /// A text message that will be sent to the client upon throttling.  You can include the token {n} to
        /// show this.Seconds in the message, e.g. "Wait {n} seconds before trying again".
        /// </summary>
        //public string Message { get; set; }

        public override void OnActionExecuting(ActionExecutingContext c)
        {
            var key = string.Concat(Name, "-", c.HttpContext.Request.UserHostAddress);

            if (HttpRuntime.Cache[key] == null)
            {
                HttpContext.Current.Session["CountUploaded"] = 1;
                HttpRuntime.Cache.Add(key,
                    true, // is this the smallest data we can have?
                    null, // no dependencies
                    DateTime.Now.AddSeconds(3600), // absolute expiration
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null); // no callback
            }
            else
            {
                HttpContext.Current.Session["CountUploaded"] = (int)HttpContext.Current.Session["CountUploaded"] + 1;
            }

            if ((int)HttpContext.Current.Session["CountUploaded"] > 5)
            {
                var result = new JsonResult();
                result.Data = new
                {
                    Title = "خطا",
                    Message = "محدودیت آپلود فایل"
                };
                c.Result = result;
                return;
            }
        }
    }
}