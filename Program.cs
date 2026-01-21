var builder = WebApplication.CreateBuilder(args);

// NO AddControllers, NO AddAuthorization, NO AddAuthentication

var app = builder.Build();

// 1. Enables serving files from the 'wwwroot' folder
app.UseStaticFiles();

// 2. Redirects the base URL (http://localhost:5167/) to the login file.
// This is the cleanest way to set your landing page.
app.MapGet("/", (HttpContext context) => context.Response.Redirect("/login.html"));

app.Run();