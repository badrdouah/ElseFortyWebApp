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
using BugReportArchive.Repo;
using BugReportArchive.Model;

namespace BugReportArchive
{
    public static class RaiseResolvedEvent
    {
        [FunctionName("RaiseResolvedEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client, ILogger log)
        {


            string json = new StreamReader(req.Body).ReadToEnd();
            var newBugReport = JsonConvert.DeserializeObject<BugModel>(json);
            await MyRepo.UpdateBugReport(newBugReport);
            string instanceId = newBugReport.id; 

            log.LogInformation("Resolved event raised for "+ instanceId);


            bool isResolved = true;
            await client.RaiseEventAsync(instanceId, "ResolvedEvent", isResolved);
            return new OkObjectResult("OK");
        }
    }
}

