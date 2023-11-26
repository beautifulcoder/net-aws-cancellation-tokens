using Amazon.Lambda.AspNetCoreServer;

namespace Aws.CancellationTokens.AspNet;

public class LambdaEntryPoint : APIGatewayProxyFunction
{
  protected override void Init(IWebHostBuilder builder)
  {
    builder.UseStartup<Startup>();
  }
}
