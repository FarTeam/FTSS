﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace FTSS.API.Extensions
{
    public static class Services
    {
        /// <summary>
        /// Add default mapper to the service pool
        /// </summary>
        /// <param name="services"></param>
        public static AutoMapper.IMapper AddMapper(this IServiceCollection services)
        {
            //Create database logger
            var mapConfig = new AutoMapper.MapperConfiguration(mc =>
            {
                mc.AddProfile(new Logic.CommonOperations.Mapper());
            });

            AutoMapper.IMapper mapper = mapConfig.CreateMapper();
            services.AddSingleton(mapper);
            return mapper;
        }


        /// <summary>
        /// Add DatabaseContext as a service to the service pool
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">Database connection string</param>
        /// <remarks>
        /// With this service we could easily access to the database objects like Stored-Procedure
        /// </remarks>
        public static Logic.Database.IDatabaseContext AddORM(this IServiceCollection services, string connectionString)
        {
            //Create Dapper ORM
            var ctx = new Logic.Database.DatabaseContextDapper(connectionString);

            //Add DatabaseContext as a service to the service pool
            services.AddSingleton<Logic.Database.IDatabaseContext>(ctx);
            
            return ctx;
        }


        /// <summary>
        /// Add Logger service to services pool
        /// </summary>
        /// <param name="services"></param>
        /// <param name="defaultORM">The default ORM</param>
        public static void AddLogger(this IServiceCollection services, Logic.Database.IDatabaseContext defaultORM)
        {
            //Create database logger
            var dbLogger = new Logic.Log.LogAtDatabase(defaultORM);

            //Add logger as a service to the service pool
            services.AddSingleton<Logic.Log.ILog>(dbLogger);
        }

        /// <summary>
        /// Add API logger service to service pool
        /// </summary>
        /// <param name="services"></param>
        /// <param name="defaultORM"></param>
        public static void AddAPILogger(this IServiceCollection services, 
                    Logic.Database.IDatabaseContext defaultORM,
                    AutoMapper.IMapper defaultMapper)
        {
            var APILoggerDatabase = new Logic.Log.APILoggerDatabase(defaultORM, defaultMapper);

            //Add logger as a service to the service pool
            services.AddSingleton<Logic.Log.IAPILogger>(APILoggerDatabase);
        }


        /// <summary>
        /// Setup JWT validator
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddJWT(this IServiceCollection services, IConfiguration configuration)
        {
            //Fetch JWT key and issuer from the appsettings.json
            string key = configuration.GetValue<string>("JWT:Key");
            string issuer = configuration.GetValue<string>("JWT:Issuer");

            //When a request receive, this operations check the JWT and set User object
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
