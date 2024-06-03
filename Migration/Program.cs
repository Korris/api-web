using Mcsg.Lib.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<McsgDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("McsgConnectionString")));

var app = builder.Build();

app.Run();