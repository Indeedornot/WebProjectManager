using shared.Api;

using Tailwind;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServerSideBlazor();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IProjectApi, ProjectApi>(client => {
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.RunTailwind("release", "./");
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else {
    app.RunTailwind("build", "./");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapBlazorHub();

app.Run();