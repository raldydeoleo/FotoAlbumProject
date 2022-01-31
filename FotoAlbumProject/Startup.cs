using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using PhotoGallery.Infrastructure;
//using PhotoGallery.Infrastructure.Mappings;
using PhotoGallery.Infrastructure.Repositories;
using PhotoGallery.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Hosting;

namespace FotoAlbumProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            try
            { 
                //Configurando la clase para manejar las config. en el archivo appSettings.json
                var appSettingsSection = Configuration.GetSection("AppSettings");
                //services.Configure<AppSettings>(appSettingsSection);
                //var appSettings = appSettingsSection.Get<AppSettings>();
                //Configurando el contexto EF para los Datos Maestros
               // services.AddDbContext<DbContext>(options =>
               // options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"), SqlServerOptions=>SqlServerOptions.CommandTimeout(appSettings.CommandTimeout)));
                //Configurando el contexto EF para Box Track Label
                
               
               
                //Permitiendo conexiones desde otros origenes
                services.AddCors(action => {
                    action.AddPolicy("MyCorsPolicy", builder =>
                    {
                        builder //.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetIsOriginAllowed(x => true)
                        .Build();
                    });
                });

            }catch (Exception ex)
            {
                throw ex;
                //Log.Error(ex,ex.Message);
            }

            string sqlConnectionString = Configuration["ConnectionStrings:DefaultConnection"];
            bool useInMemoryProvider = bool.Parse(Configuration["Data:PhotoGalleryConnection:InMemoryProvider"]);

            services.AddDbContext<PhotoGalleryContext>(options =>
            {
                switch (useInMemoryProvider)
                {
                    case true:
                       // options.UseInMemoryDatabase();
                        break;
                    default:
                        options.UseSqlServer(sqlConnectionString);
                        break;
                }
            });

            // Repositorios
            services.AddScoped<PhotoRepository, PhotoRepository>();
            services.AddScoped<IAlbumRepository, AlbumRepository>();
           

            // Servicios
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IEncryptionService, EncryptionService>();


            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            try 
            { 
                //Mantiene la estructura de la base de datos actualizada y se asegura de que esté creada.
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<PhotoGalleryContext>();
                    //context.Database.Migrate();
                    //SeedBoxtrackDataBase(context);
                }
            } 
            catch (Exception ex)
            {

                throw ex;
                //Log.Error(ex.ToString());
            }


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
