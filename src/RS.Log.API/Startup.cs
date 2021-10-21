using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RS.Log.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.Log.API
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
			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "RS.Log.API", Version = "v1" });
			});

			var strConn = "Server = localhost; Port = 3306; Database = RS.Log; Uid = root; Pwd = MyDB@123;";
			services.AddDbContext<ApplicationContext>(p => p
				.UseMySql(strConn, ServerVersion.AutoDetect(strConn))
				.LogTo(Console.WriteLine)
				.EnableSensitiveDataLogging()
			);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RS.Log.API v1"));
			}

			DatabaseInitialize(app);

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private void DatabaseInitialize(IApplicationBuilder app)
		{
			using var db = app.ApplicationServices
				.CreateScope()
				.ServiceProvider
				.GetRequiredService<ApplicationContext>();

			db.Database.EnsureDeleted();
			db.Database.EnsureCreated();

			for (var i = 1; i <= 5; i++)
			{
				db.Logs.Add(new Domain.Log {
					Project = $"Projetct {i}",
					Message = $"Message {i}",
					StackTrace = $"StackTrace {i}"
				});
			}

			db.SaveChanges();
		}
	}
}
