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
    public static class TimeOutFunc
    {
        //send email  to admin with link to fill bug report archive form
        //this gets called from orchestrator
        [FunctionName("TimeOut")]
        public static async Task Run([ActivityTrigger] string bugReportId, ILogger log)
        {
            log.LogInformation($"Bug with ID = {bugReportId} was not resolved in the legal time!");

            var bugReport = await MyRepo.GetBugReport(bugReportId);
            bugReport.status = "Timeout";
            await MyRepo.UpdateBugReport(bugReport);
        }

    }
}

