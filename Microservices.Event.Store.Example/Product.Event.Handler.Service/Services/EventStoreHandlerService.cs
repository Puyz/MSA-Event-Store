using MongoDB.Driver;
using Shared.Events;
using Shared.Services.Abstractions;
using System.Reflection;
using System.Text.Json;

namespace Product.Event.Handler.Service.Services
{
    public class EventStoreHandlerService : BackgroundService
    {
        private readonly IEventStoreService _eventStoreService;
        private readonly IMongoDBService _mongoDBService;

        public EventStoreHandlerService(IEventStoreService eventStoreService, IMongoDBService mongoDBService)
        {
            _eventStoreService = eventStoreService;
            _mongoDBService = mongoDBService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStoreService.SubscribeToStreamAsync("products-stream", async (streamSubscription, resolvedEvent, cancellationToken) =>
            {
                string eventType = resolvedEvent.Event.EventType;
                object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(),
                    Assembly.Load("Shared").GetTypes().FirstOrDefault(t => t.Name == eventType)!)!;

                var productCollection = _mongoDBService.GetCollection<Shared.Models.Product>("products");
                Shared.Models.Product? product = null;
                switch (@event)
                {
                    case NewProductAddedEvent e:
                        var hasProduct = await (await productCollection.FindAsync(p => p.Id == e.ProductId, cancellationToken: cancellationToken)).AnyAsync(cancellationToken: cancellationToken);
                        if (!hasProduct)
                        {
                            await productCollection.InsertOneAsync(new()
                            {
                                Id = e.ProductId,
                                ProductName = e.ProductName,
                                Count = e.InitialCount,
                                IsAvailable = e.IsAvailable,
                                Price = e.InitialPrice
                            }, cancellationToken: cancellationToken);
                        }
                        break;
                    case CountDecreasedEvent e:
                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                        if (product != null)
                        {
                            product.Count -= e.DecrementAmount;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product, cancellationToken: cancellationToken);
                        }
                        break;
                    case CountIncreasedEvent e:
                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                        if (product != null)
                        {
                            product.Count += e.IncrementAmount;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product, cancellationToken: cancellationToken);
                        }
                        break;
                    case PriceDecreasedEvent e:
                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                        if (product != null)
                        {
                            product.Price -= e.DecrementAmount;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product, cancellationToken: cancellationToken);
                        }
                        break;
                    case PriceIncreasedEvent e:
                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                        if (product != null)
                        {
                            product.Price += e.IncrementAmount;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product, cancellationToken: cancellationToken);
                        }
                        break;
                    case AvailabilityChangedEvent e:
                        product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                        if (product != null)
                        {
                            product.IsAvailable = e.IsAvailable;
                            await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product, cancellationToken: cancellationToken);
                        }
                        break;
                }
            });
        }
    }
}
