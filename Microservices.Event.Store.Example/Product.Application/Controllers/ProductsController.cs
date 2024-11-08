using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Product.Application.Models;
using Shared.Events;
using Shared.Services.Abstractions;

namespace Product.Application.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IEventStoreService _eventStoreService;
        private readonly IMongoDBService _mongoDBService;

        public ProductsController(IEventStoreService eventStoreService, IMongoDBService mongoDBService)
        {
            _eventStoreService = eventStoreService;
            _mongoDBService = mongoDBService;
        }

        public async Task<IActionResult> Index()
        {
            var productCollection = _mongoDBService.GetCollection<Shared.Models.Product>("products");
            var products = await (await productCollection.FindAsync(_ => true)).ToListAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProduct model)
        {
            NewProductAddedEvent newProductAddedEvent = new()
            {
                ProductId = Guid.NewGuid().ToString(),
                InitialCount = model.Count,
                InitialPrice = model.Price,
                IsAvailable = model.IsAvailable,
                ProductName = model.ProductName,
            };
            await _eventStoreService.AppendToStreamAsync("products-stream", new[] { _eventStoreService.GenerateEventData(newProductAddedEvent) });
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(string productId)
        {
            var productCollection = _mongoDBService.GetCollection<Shared.Models.Product>("products");
            var product = await (await productCollection.FindAsync(p => p.Id == productId)).FirstOrDefaultAsync();
            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> CountUpdate(Shared.Models.Product model)
        {
            var productCollection = _mongoDBService.GetCollection<Shared.Models.Product>("products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();

            if (model.Count > product.Count)
            {
                CountIncreasedEvent countIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    IncrementAmount = model.Count - product.Count
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[] { _eventStoreService.GenerateEventData(countIncreasedEvent) });
            }
            else
            {
                CountDecreasedEvent countDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementAmount = product.Count - model.Count
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[] { _eventStoreService.GenerateEventData(countDecreasedEvent) });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PriceUpdate(Shared.Models.Product model)
        {
            var productCollection = _mongoDBService.GetCollection<Shared.Models.Product>("products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();

            if (model.Price > product.Price)
            {
                PriceIncreasedEvent priceIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    IncrementAmount = model.Price - product.Price
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[] { _eventStoreService.GenerateEventData(priceIncreasedEvent) });
            }
            else
            {
                PriceDecreasedEvent priceDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementAmount = product.Price - model.Price
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[] { _eventStoreService.GenerateEventData(priceDecreasedEvent) });
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AvailableUpdate(Shared.Models.Product model)
        {
            var productCollection = _mongoDBService.GetCollection<Shared.Models.Product>("products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();

            if (model.IsAvailable != product.IsAvailable)
            {
                AvailabilityChangedEvent availabilityChangedEvent = new()
                {
                    ProductId = model.Id,
                    IsAvailable = model.IsAvailable
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[] { _eventStoreService.GenerateEventData(availabilityChangedEvent) });
            }

            return RedirectToAction("Index");
        }
    }
}
