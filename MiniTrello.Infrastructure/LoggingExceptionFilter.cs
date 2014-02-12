using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using log4net;

namespace MiniTrello.Infrastructure
{
    public class LoggingExceptionFilter : ExceptionFilterAttribute
    {
        readonly List<Func<Exception, bool>> _excludingConditions = new List<Func<Exception, bool>>();

        public LoggingExceptionFilter Excluding(Func<Exception, bool> condition)
        {
            _excludingConditions.Add(condition);
            return this;
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (IsExceptionThatShouldBeExcluded(actionExecutedContext)) return;

            string userName = "User: " + HttpContext.Current.User.Identity.Name;
            string remoteIp = "IP Address: " + GetIP();
            string pageUrl = "Page url: " + HttpContext.Current.Request.Url;
            string userAgent = "Browser: " + HttpContext.Current.Request.UserAgent;

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("##### User Details #####");
            sb.AppendLine();
            sb.AppendLine(userName);
            sb.AppendLine(remoteIp);
            sb.AppendLine(pageUrl);
            sb.AppendLine(userAgent);
            sb.AppendLine();

            sb.AppendLine("##### Exception Details #####");                
            ILog Log = LogManager.GetLogger(actionExecutedContext.ActionContext.ControllerContext.Controller.GetType());

            Log.Error(sb.ToString(), actionExecutedContext.Exception);
        }

        bool IsExceptionThatShouldBeExcluded(HttpActionExecutedContext actionExecutedContext)
        {
            var stop = false;
            _excludingConditions.ForEach(x =>
                                             {
                                                 if (x(actionExecutedContext.Exception))
                                                     stop = true;
                                             });
            if (stop) return true;
            return false;
        }

        public static string GetIP()
        {
            string ip =
                HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }
    }
}