using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Setup DataContext
builder.Services.AddDbContext<DataContext>(options =>
{
  // Tell the app what database technology we're using (in this case Sqlite)
  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

/***********************************************************/
/***** All App code below is referred to as Middleware *****/
/***********************************************************/
// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();
