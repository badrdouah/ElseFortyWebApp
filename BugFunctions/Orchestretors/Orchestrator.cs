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
using System.Threading;
using BugReportArchive.Repo;

namespace BugReportArchive.Orchestretors
{
    public static class OrchestratorFunc
    {

        //orchestrator
        [FunctionName("Orchestrator")]
        public static async Task Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var bugReportId = (context.GetInput<BugReportData>()).id;
            //Send email to admin and client
            await context.CallActivityAsync("Notify", bugReportId);

            using (var timeoutCts = new CancellationTokenSource())
            {
                DateTime dueTime = context.CurrentUtcDateTime.AddDays(7);
                Task durableTimeout = context.CreateTimer(dueTime, timeoutCts.Token);

                Task<bool> bugResolvedEvent = context.WaitForExternalEvent<bool>("ResolvedEvent");
                if (bugResolvedEvent == await Task.WhenAny(bugResolvedEvent, durableTimeout))
                {
                    timeoutCts.Cancel();
                    await context.CallActivityAsync("Resolved", bugReportId);
                }
                else
                {
                    await context.CallActivityAsync("TimeOut", bugReportId);
                }
            }
        }
    }
}

