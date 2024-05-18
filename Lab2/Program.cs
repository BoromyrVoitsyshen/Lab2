using Lab2.Data;
using Lab2.Interfaces;
using Lab2.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IWareInterface, WareRepository>();
builder.Services.AddScoped<ICategoryInterface,  CategoryRepository>();
builder.Services.AddScoped<ICountryInterface, CountryRepository>();
builder.Services.AddScoped<IOwnerInterface, OwnerRepository>();
builder.Services.AddScoped<IReviewInterface, ReviewRepository>();
builder.Services.AddScoped<IReviewerInterface, ReviewerRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
