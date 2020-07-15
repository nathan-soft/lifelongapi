using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Data;
using LifeLongApi.Data.Repositories;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using LifeLongApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace LifeLongApi {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddControllers ();
            services.AddAutoMapper (typeof (Startup));
            services.AddScoped<IAuthService, AuthService> ();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWorkExperienceService, WorkExperienceService>();
            services.AddScoped(typeof(ApiOkResponseDto));
            services.AddScoped(typeof(ApiErrorResponseDto));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IFollowRepository, FollowRepository>();
            services.AddScoped<IQualificationRepository, QualificationRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<IUserFieldOfInterestRepository, UserFieldOfInterestRepository>();
            services.AddScoped<IWorkExperienceRepository, WorkExperienceRepository>();
            //services.AddScoped<typeof(ServiceResponse<>)>();

            services.AddIdentity<AppUser, AppRole> (options => {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<IdentityAppContext> ();

            services.AddDbContext<IdentityAppContext> (options => {
                options.UseSqlServer (Configuration.GetConnectionString ("DefaultConnec"));
            });

            services.Configure<IdentityOptions> (options => {
                // Password settings.
                // options.Password.RequireDigit = true;
                // options.Password.RequireLowercase = true;
                // options.Password.RequireNonAlphanumeric = true;
                // options.Password.RequireUppercase = true;
                // options.Password.RequiredLength = 6;
                // options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                // options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = false;

                // User settings.
                // options.User.AllowedUserNameCharacters =
                // "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.AddAuthentication (x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer ("Bearer", options => {
                    options.Events = new JwtBearerEvents () {
                        OnChallenge = context => {
                            context.HandleResponse ();

                            var response = new { error = "Unauthorized access" };
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.HttpContext.Response.Headers.Append ("www-authenticate", "Bearer");
                            return context.Response.WriteAsync (JsonConvert.SerializeObject (response));
                        }
                    };
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = Configuration.GetSection ("Jwt").GetSection ("Audience").Value,
                        ValidIssuer = Configuration.GetSection ("Jwt").GetSection ("Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey (Encoding.ASCII
                        .GetBytes (Configuration.GetSection ("Jwt").GetSection ("Key").Value)),
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseHttpsRedirection ();

            app.UseRouting ();

            // global cors policy
            // app.UseCors (x => x
            //     .AllowAnyOrigin ()
            //     .AllowAnyMethod ()
            //     .AllowAnyHeader ());

            app.UseAuthentication ();
            app.UseAuthorization ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
            });
        }
    }
}