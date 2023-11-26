using Amazon.CDK;
using Aws.CancellationTokens.Cdk;

var app = new App();
_ = new CancellationTokensStack(app, "aws-cancellationtokens-stack", new StackProps
{
  Env = new Environment
  {
    Account = "ACCOUNT_ID",
    Region = "us-east-1"
  }
});
app.Synth();
