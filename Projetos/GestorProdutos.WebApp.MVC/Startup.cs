using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorProdutos.WebApp.MVC.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GestorProdutos.Catalogo.Data;
using GestorProdutos.WebApp.MVC.Setup;
using GestorProdutos.Negocio.AutoMapper;
using NDD.GestorProdutos.Migracoes;
using GestorProdutos.Infra.Configuracoes;

namespace GestorProdutos.API
{
    public class Startup
    {
        private readonly GestorProdutosConfiguracoes _configuracoes;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _configuracoes = new GestorProdutosConfiguracoes();
            configuration.Bind(_configuracoes);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_configuracoes.AppSettings.ConnectionString));

            services.AddDbContext<CatalogoContext>(options => options.UseSqlServer(_configuracoes.AppSettings.ConnectionString));

            services.AddDefaultIdentity<IdentityUser>().AddDefaultUI(UIFramework.Bootstrap4).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAutoMapper(typeof(DomainToViewModelMappingProfile), typeof(ViewModelToDomainMappingProfile));

            services.AddMediatR(typeof(Startup));

            services.AdicionarMigracoes(_configuracoes.AppSettings.ConnectionString, Base.Enums.DbProviderEnum.SqlServer);

            services.RegisterServices();

            //services.AddTransient(c => new GestorProdutosConfiguracoes());
            services.AddTransient(c => _configuracoes);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.InicializarMigracoes();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=AdminProdutos}/{action=Index}/{id?}");
            });
        }
    }
}
