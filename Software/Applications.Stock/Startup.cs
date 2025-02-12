using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;
using Stock.Infrastructure.Context;
using Stock.Infrastructure.Repositories;
using System;


namespace Stock.Application
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder
                                      .AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .Build();
                                  });
            });

            services.AddControllers().AddControllersAsServices().AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddDbContext<EfContext>(options => options.UseSqlServer(Configuration.GetConnectionString("principal")), ServiceLifetime.Scoped);
            services.AddScoped<IRepository<BranchOffice>, BranchOfficeRepository>();
            services.AddScoped<IRepository<Invoice>, InvoiceRepository>();
            services.AddScoped<IRepository<Product>, ProductRepository>();
            services.AddScoped<IRepository<Location>, LocationRepository>();
            services.AddScoped<IRepository<Provider>, ProviderRepository>();
            services.AddScoped<IRepository<Warehouse>, WareHouseRepository>();
            services.AddScoped<IRepository<ProductManagement>, ProductManagementRepository>();
            services.AddScoped<IRepository<ProductTransition>, ProductTransitionRepository>();
            services.AddScoped<IRepository<ProductProvider>, ProductProviderRepository>();

            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Controle de estoque e produtos",
                    Description = "Documentação do projeto controle de estoque e produtos",
                    TermsOfService = new Uri("https://github.com/gabriel2mm/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Gabriel Maia",
                        Email = "gabriel_more@hotmail.com",
                        Url = new Uri("https://github.com/gabriel2mm/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use MTI License",
                        Url = new Uri("https://github.com/gabriel2mm/"),
                    }
                });
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API STOCK v1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
