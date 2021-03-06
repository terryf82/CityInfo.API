﻿using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace CityInfo.API
{
    public class Startup
    {
        public static IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter()
                ));
                // .AddJsonOptions(o => {
                //     if (o.SerializerSettings.ContractResolver != null)
                //     {
                //         var castedResolver = o.SerializerSettings.ContractResolver
                //             as DefaultContractResolver;
                //         castedResolver.NamingStrategy = null;
                //     }
                // });
            
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif

            // var config = Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();

            services.AddDbContext<CityInfoContext>(o => 
                o.UseMySQL(Configuration.GetConnectionString("CityDB")));
            
            services.AddScoped<ICityInfoRepository, CityInfoRepository>();

            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CityInfoContext cityInfoContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            cityInfoContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                // cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestForCreationDto>()
                    // .ForMember(x => x.Id, opt => opt.Ignore());
                // cfg.CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
                // cfg.CreateMap<Entities.City, Models.CityDto>();
                cfg.CreateMap<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>();
            });

            app.UseMvc();
        }
    }
}
