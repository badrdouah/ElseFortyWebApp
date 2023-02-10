using System;
using ElseForty.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace ElseForty.Repositories
{
	public interface IBlogRepo
	{
        Task<List<BlogPostModel>> GetAll();
         Task<BlogPostModel> Get(string id);
        Task  Add(BlogPostModel model);
        Task Update(BlogPostModel model);
        Task Delete(string id);
    }

	public class BlogRepo : IBlogRepo
    {
		public BlogRepo(IConfiguration configuration)
		{
            Configuration = configuration;
            connectionString = configuration["CosmosDB:connectionString"];
            databaseId = configuration["CosmosDB:databaseId"];
            containerId = configuration["CosmosDB:blogContainerId"];
        }

        private IConfiguration Configuration { get; }

        private string connectionString { get; }
        private string databaseId { get; }
        private string containerId { get; }

        public async Task Add(BlogPostModel model)
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
                await container.DeleteItemAsync<BlogPostModel>(id.ToString(), new PartitionKey(id.ToString()));
            }
        }

        public async Task<BlogPostModel> Get(string id)
		{
            BlogPostModel result = null;
            using (var client = new CosmosClient(connectionString))
            {
                var container = client.GetContainer(databaseId, containerId);

                IOrderedQueryable<BlogPostModel> queryable = container.GetItemLinqQueryable<BlogPostModel>();

                var matches = queryable
                    .Where(p => p.id == id);
                using FeedIterator<BlogPostModel> linqFeed = matches.ToFeedIterator();
                    FeedResponse<BlogPostModel> response = await linqFeed.ReadNextAsync();
                    if (response.Resource.Count() != 0) result = response.First();
            }

            return result;
        }

        public async Task<List<BlogPostModel>> GetAll()
        {
            List<BlogPostModel> result = new List<BlogPostModel>();
            using (var client = new CosmosClient(connectionString))
            {
                var container = client.GetContainer(databaseId, containerId);

                IOrderedQueryable<BlogPostModel> queryable = container.GetItemLinqQueryable<BlogPostModel>();
                // Convert to feed iterator
                using FeedIterator<BlogPostModel> linqFeed = queryable.ToFeedIterator();
                while (linqFeed.HasMoreResults)
                {
                    FeedResponse<BlogPostModel> response = await linqFeed.ReadNextAsync();
                    foreach (BlogPostModel post in response)
                    {
                        result.Add(post);
                    }
                }
            }
            return result;
        }

        public async  Task Update(BlogPostModel model)
        {
            model.modificationDate = DateTime.Now;

            using (var client = new CosmosClient(connectionString))
            {
                var container = client.GetContainer(databaseId, containerId);
                await container.ReplaceItemAsync(model, model.id.ToString());
            }
        }
    }
}

