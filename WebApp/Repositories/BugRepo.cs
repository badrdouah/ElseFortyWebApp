using System;
using ElseForty.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace ElseForty.Repositories
{
    public interface IBugRepo
    {
        Task<List<BugModel>> GetAll();
        Task<BugModel> Get(string id);
        Task Add(BugModel model);
        Task Update(BugModel model);
        Task Delete(string id);
    }
    public class BugRepo : IBugRepo
    {
        public BugRepo(IConfiguration configuration)
        {
            Configuration = configuration;
            connectionString = configuration["CosmosDB:connectionString"];
            databaseId = configuration["CosmosDB:databaseId"];
            containerId = configuration["CosmosDB:blogContainerId"];
        }

        private IConfiguration Configuration { get; }

        private string? connectionString { get; }
        private string? databaseId { get; }
        private string? containerId { get; }

        public async Task Add(BugModel model)
        {
            using (var client = new CosmosClient(connectionString))
            {
                var container = client.GetContainer(databaseId, containerId);
                await container.CreateItemAsync(model);
            }
        }

        public async Task Delete(string id)
        {
            using (var client = new CosmosClient(connectionString))
            {
                var container = client.GetContainer(databaseId, containerId);
                await container.DeleteItemAsync<BugModel>(id.ToString(), new PartitionKey(id.ToString()));
            }
        }

        public async Task<BugModel> Get(string id)
        {
            BugModel result = null;
            using (var client = new CosmosClient(connectionString))
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

        public async Task<List<BugModel>> GetAll()
        {
            List<BugModel> result = new List<BugModel>();
            using (var client = new CosmosClient(connectionString))
            {
                var container = client.GetContainer(databaseId, containerId);

                IOrderedQueryable<BugModel> queryable = container.GetItemLinqQueryable<BugModel>();
                // Convert to feed iterator
                using FeedIterator<BugModel> linqFeed = queryable.ToFeedIterator();

                // Iterate query result pages
                while (linqFeed.HasMoreResults)
                {
                    FeedResponse<BugModel> response = await linqFeed.ReadNextAsync();

                    // Iterate query results
                    foreach (BugModel post in response)
                    {
                        result.Add(post);
                    }
                }
            }
            return result;
        }

        public async Task Update(BugModel model)
        {
            using (var client = new CosmosClient(connectionString))
            {
                var container = client.GetContainer(databaseId, containerId);
                await container.ReplaceItemAsync(model, model.id.ToString());
            }
        }
    }
}

