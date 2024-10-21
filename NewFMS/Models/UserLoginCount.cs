using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class UserLoginCount
    {
        public static List<LogOnUsers> userObj = new List<LogOnUsers>();
        public static void AddOnlineUser(string strUserId, string UserName, string Ip, string NameShakhs, string AkharinOn, string strSessionId)
        {
            LogOnUsers user = new LogOnUsers();
            if (UserLoginCount.userObj.Find(k => k.userId == strUserId) != null)
            {
                var userRemove = (LogOnUsers)userObj.Where(item => item.userId == strUserId).FirstOrDefault();
                userObj.Remove(userRemove);
            }
            user.userId = strUserId;
            user.UserName = UserName;
            user.Name = NameShakhs;
            user.AkharinOn = AkharinOn;
            user.IPAdress = Ip;
            user.sessionId = strSessionId;
            user.newStatus = true;
            userObj.Add(user);
        }

        public static void RemoveOnlineUser(string strUserId)
        {
            var userRemove = (LogOnUsers)userObj.Where(item => item.userId == strUserId).FirstOrDefault();
            userObj.Remove(userRemove);
        }
    }
}