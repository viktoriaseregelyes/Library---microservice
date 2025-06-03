using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Book;
using Service.Book.IntegrationEventHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<BookContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostGreSQL")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<BorrowEventHandler>();

builder.Services.AddMassTransit(x =>
{
    // Register your consumers and services
    x.AddConsumer<BorrowEventHandler>();
    x.AddConsumer<DeleteUserHandler>();

    // Configure RabbitMQ (or other broker)
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(ctx.GetRequiredService<IConfiguration>()
            .GetConnectionString("library-messaging"));
        cfg.ConfigureEndpoints(ctx);
    });
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
