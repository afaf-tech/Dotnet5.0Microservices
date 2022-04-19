using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Services.Controllers {

    // https://localhost:5001/items
    [ApiController]
    [Route("items")]
    public class ItemsController : Controller {

        private readonly IRepository<Item> _itemsRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint){
            this._itemsRepository = itemsRepository;
            this._publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync(){
            var items = (await _itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());

            return Ok(items);
        }
        // Code Example for Syncronous Integration to provide error result
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync(){

        //     requestCounter++;
        //     Console.WriteLine($"Request {requestCounter}: starting");
        //     if(requestCounter <= 2){
        //         Console.WriteLine($"Request {requestCounter}: Delaying...");
        //         await Task.Delay(TimeSpan.FromSeconds(10));
        //     }
        //     if(requestCounter <= 4){
        //         Console.WriteLine($"Request {requestCounter}:  500 (Internal Server Error)...");
        //         await Task.Delay(TimeSpan.FromSeconds(10));
        //         return StatusCode(500);
        //     }
            
        //     var items = (await _itemsRepository.GetAllAsync())
        //                 .Select(item => item.AsDto());
        //     Console.WriteLine($"Request {requestCounter}:  200 (Ok)...");

        //     return Ok(items);
        // }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id) {
            var item = await _itemsRepository.GetAsync(id);

            if(item == null) return NotFound();
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto){
            var item = new Item{
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow,
            };

            await _itemsRepository.CreateAsync(item);

            await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new {id = item.Id}, item);
        }


        // IActionResult when no need to use Type <>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto){
            var existingItem = await _itemsRepository.GetAsync(id);
            if(existingItem == null) return NotFound();

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await _itemsRepository.UpdateAsync(existingItem);

            await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id){
            var item = _itemsRepository.GetAsync(id);
            if(item == null) return NotFound();
            await _itemsRepository.RemoveAsync(id);
            
            await _publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}