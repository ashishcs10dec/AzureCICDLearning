using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyTutor.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GlobalExceptionFilter));
        public GlobalExceptionFilter()
        { 
        
        }
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            Log.Error(filterContext.Exception);
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Error/Error.cshtml"
            };
        }
    }
}