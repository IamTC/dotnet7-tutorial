using Entities;
using Interfaces;

namespace Repositories
{

    public class InMemItemsRespository : IItemsRepository
    {
        private readonly List<Item> items = new() {
            new Item{Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreatedDate = DateTimeOffset.UtcNow },
            new Item{Id = Guid.NewGuid(), Name = "Iron Sword", Price = 29, CreatedDate = DateTimeOffset.UtcNow },
            new Item{Id = Guid.NewGuid(), Name = "Magic Potion", Price = 19, CreatedDate = DateTimeOffset.UtcNow },
            new Item{Id = Guid.NewGuid(), Name = "Shield", Price = 10, CreatedDate = DateTimeOffset.UtcNow }
        };


        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await Task.FromResult(items);
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            return await Task.FromResult(items.Where(item => item.Id == id).SingleOrDefault());
        }

        public async Task CreateItemAsync(Item item)
        {
            items.Add(item);
            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(Item item)
        {
            var index = items.FindIndex(i => i.Id == item.Id);
            items[index] = item;
            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var index = items.FindIndex(i => i.Id == id);
            items.RemoveAt(index);
            await Task.CompletedTask;
        }
    }
}