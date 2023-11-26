using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Aws.CancellationTokens.Function;

public class Function
{
  private static readonly TimeSpan GracefulStopTimeLimit = TimeSpan.FromSeconds(3);

  public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
  {
    var cts = new CancellationTokenSource(
      context.RemainingTime > GracefulStopTimeLimit
        ? context.RemainingTime.Subtract(GracefulStopTimeLimit)
        : GracefulStopTimeLimit);

    try
    {
      await Task.Delay(TimeSpan.FromSeconds(15), cts.Token);

      return new APIGatewayProxyResponse
      {
        Body = JsonSerializer.Serialize(new Casing(request.Body.ToLower(), request.Body.ToUpper())),
        StatusCode = 200
      };
    }
    catch (TaskCanceledException e)
    {
      return new APIGatewayProxyResponse
      {
        Body = e.Message,
        StatusCode = 504
      };
    }
  }
}

public record Casing(string Lower, string Upper);
