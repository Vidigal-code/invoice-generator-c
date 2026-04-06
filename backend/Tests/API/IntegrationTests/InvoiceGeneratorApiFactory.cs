using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;
using InvoiceGenerator.Api.Application.Constants;

namespace InvoiceGenerator.Api.Tests.API.IntegrationTests
{
    public class InvoiceGeneratorApiFactory : WebApplicationFactory<AssemblyMarker>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(HostingEnvironmentNames.IntegrationTests);

            // WebApplicationFactory may infer content root as .../ProjectName/; this repo keeps the .csproj at backend root.
            builder.UseContentRoot(ResolveContentRootWithAppSettings());

            builder.ConfigureTestServices(services =>
            {
                foreach (var d in services.Where(s => s.ServiceType == typeof(IDistributedCache)).ToList())
                    services.Remove(d);
                services.AddDistributedMemoryCache();
            });
        }

        private static string ResolveContentRootWithAppSettings()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "appsettings.json")))
                    return dir.FullName;
                dir = dir.Parent;
            }

            throw new InvalidOperationException(
                "Could not locate appsettings.json walking up from test base directory; integration tests need the API project root.");
        }
    }
}
