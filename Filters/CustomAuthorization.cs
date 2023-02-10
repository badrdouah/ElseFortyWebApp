using System;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Serialization;


namespace ElseForty.Filters
{
	public class CustomAuthorization : IActionFilter
    {
        public string myKey { get; set; }
        public CustomAuthorization(IConfiguration configuration)
        {
            Configuration = configuration;
            myKey = configuration["key"];
        }

        public IConfiguration Configuration { get; }

  
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var key = context.HttpContext.Request.Query["key"].ToString();

            if (key != myKey)
            {
                context.Result = new UnauthorizedObjectResult("Sorry, you're not authrized to access this section!");
               // await context.Result.e(context);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
         }
    }
}

