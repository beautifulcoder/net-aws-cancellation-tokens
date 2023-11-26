using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;

namespace Aws.CancellationTokens.Cdk;

public class CancellationTokensStack : Stack
{
  internal CancellationTokensStack(
    Construct scope, string id, IStackProps props = null)
    : base(scope, id, props)
  {
    var repositoryAspNet = Repository.FromRepositoryName(
      this, "ecr-repository-aspnet", "net-aws-cancellation-tokens-aspnet");
    var handlerAspNet = new DockerImageFunction(this, "lambda-aspnet", new DockerImageFunctionProps
    {
      Architecture = Architecture.ARM_64,
      Timeout = Duration.Seconds(15),
      MemorySize = 128,
      LogRetention = RetentionDays.THREE_DAYS,
      Code = DockerImageCode.FromEcr(repositoryAspNet)
    });

    var apiAspNet = new RestApi(this, "rest-api-aspnet", new RestApiProps
    {
      RestApiName = "net-aws-cancellation-tokens-aspnet",
      Description = "Support cancellation tokens with ASP.NET"
    });
    var restIntegrationAspNet = new LambdaIntegration(handlerAspNet);

    apiAspNet.Root.AddProxy(new ProxyResourceOptions
    {
      AnyMethod = true,
      DefaultIntegration = restIntegrationAspNet
    });

    var repositoryFunction = Repository.FromRepositoryName(
      this, "ecr-repository-function", "net-aws-cancellation-tokens-function");
    var handlerFunction = new DockerImageFunction(this, "lambda-function", new DockerImageFunctionProps
    {
      Architecture = Architecture.ARM_64,
      Timeout = Duration.Seconds(15),
      MemorySize = 128,
      LogRetention = RetentionDays.THREE_DAYS,
      Code = DockerImageCode.FromEcr(repositoryFunction)
    });

    var apiFunction = new RestApi(this, "rest-api-function", new RestApiProps
    {
      RestApiName = "net-aws-cancellation-tokens-function",
      Description = "Support cancellation tokens with a Lambda function"
    });
    var restIntegrationFunction = new LambdaIntegration(handlerFunction);

    apiFunction.Root.AddMethod("POST", restIntegrationFunction);
  }
}
