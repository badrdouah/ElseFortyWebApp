using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using BugReportArchive.Model;
using System.Net.Http;
using System.Text;
using BugReportArchive.Repo;

namespace BugReportArchive.Activities
{
    public static class Notify
    {
        //send email to admin with a URL to set bug as fixed, fill report and archive the bug with the resoltion
        //send email to bug reported to let him know that bug has been successfully received
        //this gets called from orchestrator
        [FunctionName("Notify")]
        public static async Task Run(
            [ActivityTrigger] string bugReportId,
            ILogger log)
        {
            using (var client = new HttpClient())
               {
                   var endPoint = Environment.GetEnvironmentVariable("emailAdminClientEndPoint");

                 var bugReport = await MyRepo.GetBugReport(bugReportId);
                 var json = JsonConvert.SerializeObject(bugReport);
                   var content = new StringContent(json, Encoding.UTF8, "application/json");
                   var response = await client.PostAsync(endPoint, content);
                    response.EnsureSuccessStatusCode();
               }
          
        }
    }
}

