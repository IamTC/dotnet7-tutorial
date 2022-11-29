using Microsoft.AspNetCore.Mvc;
using Repositories;
using Entities;
using Interfaces;
using Dtos;
using Extensions;

namespace Controllers
{

    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository respository;

        public ItemsController(IItemsRepository repository)
        {
            this.respository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await respository.GetItemsAsync()).Select(item => item.AsDto());
            return items;
        }

        // GET items/id
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await respository.GetItemAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await respository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto item)
        {
            var existingItem = await respository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            Item updateItem = existingItem with
            {
                Name = item.Name,
                Price = item.Price
            };

            await respository.UpdateItemAsync(updateItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = await respository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            await respository.DeleteItemAsync(existingItem.Id);

            return NoContent();
        }
    }
}