using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public ItemsController(IRepository<Item> itemsRepository){
            this._itemsRepository = itemsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync(){
            var items = (await _itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());
            return items;
        }

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

            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id){
            var item = _itemsRepository.GetAsync(id);
            if(item == null) return NotFound();
            await _itemsRepository.RemoveAsync(id);

            return NoContent();
        }
    }
}