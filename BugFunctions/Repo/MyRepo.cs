using System;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Threading.Tasks;
using BugReportArchive.Model;
using Microsoft.Azure.Cosmos.Linq;

namespace BugReportArchive.Repo
{
	public  static class MyRepo
	{
        public static  async Task AddNewBugReport(BugModel model)
        {
            var cosmosDBConnectionString = Environment.GetEnvironmentVariable("cosmosDBConnectionString");
            var databaseId = Environment.GetEnvironmentVariable("databaseId");
            var containerId = Environment.GetEnvironmentVariable("bugsContainerId");
            using (var client = new CosmosClient(cosmosDBConnectionString))
            {
                var container = client.GetContainer(databaseId, containerId);
                await container.CreateItemAsync(model);
            }
        }


        public static async Task<BugModel> GetBugReport(string id)
        {
            var cosmosDBConnectionString = Environment.GetEnvironmentVariable("cosmosDBConnectionString");
            var databaseId = Environment.GetEnvironmentVariable("databaseId");
            var containerId = Environment.GetEnvironmentVariable("bugsContainerId");

            BugModel result = null;
            using (var client = new CosmosClient(cosmosDBConnectionString))
            {
                var container = client.GetContainer(databaseId, containerId);

                IOrderedQueryable<BugModel> queryable = container.GetItemLinqQueryable<BugModel>();

                var matches = queryable
                    .Where(p => p.id == id);


                // Convert to feed iterator
                using FeedIterator<BugModel> linqFeed = matches.ToFeedIterator();

                FeedResponse<BugModel> response = await linqFeed.ReadNextAsync();
                if (response.Resource.Count() != 0) result = response.First();

            }

            return result;
        }


        public static async Task UpdateBugReport(BugModel model)
        {

            var cosmosDBConnectionString = Environment.GetEnvironmentVariable("cosmosDBConnectionString");
            var databaseId = Environment.GetEnvironmentVariable("databaseId");
            var containerId = Environment.GetEnvironmentVariable("bugsContainerId");

            using (var client = new CosmosClient(cosmosDBConnectionString))
            {
                model.resolutionTime = DateTime.Now;
                var container = client.GetContainer(databaseId, containerId);
                await container.ReplaceItemAsync(model, model.id.ToString());
            }
        }
    }

    
    public class BugReportData
    {
        public string id { get; set; }
    }

}

