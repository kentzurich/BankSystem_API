using API.Swagger;
using Application.Account;
using Application.Core;
using Application.Interfaces;
using DataAccess;
using FluentValidation;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services, 
            IConfiguration config)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultSQLConnection")
            ));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddValidatorsFromAssembly(typeof(Deposit).Assembly);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BalanceInquiry.Query).Assembly));
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddHttpContextAccessor();
            
            services.AddScoped<IUserAccessor, UserAccessor>();
            

            return services;
        }
    }
}
