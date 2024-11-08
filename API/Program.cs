using BLL.Infrasructure;
using BLL.Services;
using BLL.Mappers;
using DAL.Infrastructure;
using DAL.Repositories;

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

    builder.Services.AddScoped<IProductRepository>(provider =>
        new FileProductRepository(productPath, storeProductsPath));

    builder.Services.AddScoped<IStoreRepository>(provider =>
    {
        var productRepository = provider.GetRequiredService<IProductRepository>();
        return new FileStoreRepository(storePath, storeProductsPath, productRepository);
    });
}

else if (dataSourceType == "Database")
{

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
