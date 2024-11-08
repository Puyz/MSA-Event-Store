using Product.Event.Handler.Service.Services;
using Shared.Services.Abstractions;
using Shared.Services.Concretes;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IEventStoreService, EventStoreService>();
builder.Services.AddSingleton<IMongoDBService, MongoDBService>();

builder.Services.AddHostedService<EventStoreHandlerService>();

var host = builder.Build();
host.Run();
