using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
 using System.Net.Http;
using BugReportArchive.Repo;
using BugReportArchive.Model;

namespace BugReportArchive.Triggers
{
    public static class StarterFunc
    {
        // client (entry point)
        [FunctionName("Starter")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function,
            "post", Route = null)] HttpRequest req
            , [DurableClient] IDurableOrchestrationClient durableClient, ILogger log)
        {            
             string json = new StreamReader(req.Body).ReadToEnd();
             var newBugReport = JsonConvert.DeserializeObject<BugModel>(json);
             await MyRepo.AddNewBugReport(newBugReport);

            var bugReportData = new BugReportData() { id = newBugReport.id };
            var instanceId = await durableClient.StartNewAsync("Orchestrator", newBugReport.id, bugReportData);

            return durableClient.CreateCheckStatusResponse(req.HttpContext.GetHttpRequestMessage(), instanceId);
        }

    }
}

