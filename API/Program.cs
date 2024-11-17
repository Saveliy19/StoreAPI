using BLL.Infrasructure;
using BLL.Services;
using BLL.Mappers;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Sync;
using DAL.Repositories.Async;
using DAL.Managers.Interfaces;
using DAL.Managers;
using DAL.DataBase;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dataSourceType = builder.Configuration["DataStoreOptions:DataSourceType"];

if (dataSourceType == "File")
{
    var storePath = builder.Configuration["DataStoreOptions:StoreFilePath"];
    var productPath = builder.Configuration["DataStoreOptions:ProductFilePath"];
    var storeProductsPath = builder.Configuration["DataStoreOptions:StoreProductsFilePath"];

    // Регистрация синхронных репозиториев для работы с файлами
    builder.Services.AddScoped<ISyncProductRepository>(provider =>
        new FileProductRepository(productPath, storeProductsPath));

    builder.Services.AddScoped<ISyncStoreRepository>(provider =>
    {
        var productRepository = provider.GetRequiredService<ISyncProductRepository>();
        return new FileStoreRepository(storePath, storeProductsPath, productRepository);
    });

    // Регистрация IStoreRepoManager
    builder.Services.AddScoped<IStoreRepoManager>(provider =>
    {
        var productRepository = provider.GetRequiredService<ISyncProductRepository>();
        var storeRepository = provider.GetRequiredService<ISyncStoreRepository>();
        return new StoreRepoManager(productRepository, storeRepository);
    });


}


else if (dataSourceType == "Database")
{
    var storeDbContext = new StoreDbContext(builder.Configuration["DataStoreOptions:ConnectionString"]);

    builder.Services.AddScoped<IAsyncProductRepository>(provider =>
        new DbProductRepository(storeDbContext));

    builder.Services.AddScoped<IAsyncStoreRepository>(provider =>
        new DbStoreRepository(storeDbContext, provider.GetRequiredService<IAsyncProductRepository>()));

    builder.Services.AddScoped<IStoreRepoManager>(provider =>
    {
        var productRepository = provider.GetRequiredService<IAsyncProductRepository>();
        var storeRepository = provider.GetRequiredService<IAsyncStoreRepository>();
        return new StoreRepoManager(productRepository, storeRepository);
    });
}

else throw new InvalidOperationException("Invalid DataStoreType configuration value.");



builder.Services.AddScoped<IStoreMapper, StoreMapper>();
builder.Services.AddScoped<IStoreService, StoreService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
