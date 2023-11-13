using API.Controllers.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using API.Services;

namespace BankSystemAPI.Tests
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseInMemoryDatabase("DataContextTestDatabase");
            });

            services.AddIdentity<UserAccount, IdentityRole>()
                     .AddDefaultTokenProviders()
                     .AddEntityFrameworkStores<DataContext>();

            services.AddControllers();
            services.AddScoped<UserAccountController>();
            services.AddScoped<TokenService>();
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
