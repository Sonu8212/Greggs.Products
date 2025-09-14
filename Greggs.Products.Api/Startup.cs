using Greggs.Products.Api.Configuration;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Helpers;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Greggs.Products.Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddSwaggerGen();
        // Data access layer
        services.AddSingleton<IDataAccess<Product>, ProductAccess>();

        // Business/service layer
        services.AddScoped<IProductService, ProductService>();

        // Bind CurrencyOptions from config
        services.Configure<CurrencyOptions>(Configuration.GetSection("Currency"));

        // Add after registering CurrencyOptions
        services.AddSingleton<ICurrencyConverter>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CurrencyOptions>>().Value;
            return new CurrencyConverter(options);
        });
    }

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Greggs Products API V1"); });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
