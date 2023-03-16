using blazor;
using blazor.Api;

using Refit;

using Microsoft.IdentityModel.Logging;

using shared.Common;

using Tailwind;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddConfiguredIdentityServer();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<HeaderHandler>();
builder.Services.AddRefitClient<IWebApi>()
    .ConfigureHttpClient(c => {
        c.BaseAddress = new Uri(IPs.Api);
    }).AddHttpMessageHandler<HeaderHandler>();


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.RunTailwind("build", "./");
    IdentityModelEventSource.ShowPII = true;
}
else {
    app.RunTailwind("release", "./");
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.None, Secure = CookieSecurePolicy.Always });

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseBff(); //Identity Server
app.UseAuthorization();

app.MapBffManagementEndpoints();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();