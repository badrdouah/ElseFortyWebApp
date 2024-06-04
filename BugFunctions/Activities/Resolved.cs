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

namespace BugReportArchive.Activities
{
    public static class ResolvedFunc
    {
        //send email  to admin with link to fill bug report archive form
        //this gets called from orchestrator
        [FunctionName("Resolved")]
        public static async Task Run([ActivityTrigger] string bugReportId, ILogger log)
        {
            log.LogInformation($"Bug with ID = {bugReportId} has been resolved!");

            var bugReport = await MyRepo.GetBugReport(bugReportId);
            bugReport.status = "Resolved";
            await MyRepo.UpdateBugReport(bugReport);
        }

    }
}

