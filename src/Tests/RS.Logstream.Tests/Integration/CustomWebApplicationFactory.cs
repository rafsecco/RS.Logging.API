using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Infra.Providers;

namespace RS.Logstream.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
	public const string TestAuthSecret = "rs-logstream-integration-tests-secret-32chars!";
	public const string TestAuthIssuer = "rs-logstream-tests";
	public const string TestAuthAudience = "rs-logstream-tests-audience";

	private readonly string _dbName = Guid.NewGuid().ToString();

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");

		builder.UseSetting("DatabaseProvider", "SqlServer");
		builder.UseSetting("Auth:Authority", "");
		builder.UseSetting("Auth:Secret", TestAuthSecret);
		builder.UseSetting("Auth:Issuer", TestAuthIssuer);
		builder.UseSetting("Auth:Audience", TestAuthAudience);
		builder.UseSetting("Auth:RequireHttpsMetadata", "false");

		builder.ConfigureTestServices(services =>
		{
			services.RemoveAll<DbContextOptions<RSLogstreamDbContext>>();
			services.RemoveAll<IDbContextOptionsConfiguration<RSLogstreamDbContext>>();
			services.RemoveAll<IDbColumnTypes>();
			services.RemoveAll<IFullTextSearchProvider>();

			services.AddSingleton<IDbColumnTypes, MariaDbColumnTypes>();
			services.AddSingleton<IFullTextSearchProvider, LikeFullTextProvider>();
			services.AddDbContext<RSLogstreamDbContext>(opt => opt.UseInMemoryDatabase(_dbName));
		});
	}
}
