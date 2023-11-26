using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Aws.CancellationTokens.AspNet.Binders;

public class TimeoutModelBinderProvider : IModelBinderProvider
{
  public IModelBinder? GetBinder(ModelBinderProviderContext context)
  {
    if (context.Metadata.ModelType != typeof(CancellationToken))
    {
      return null;
    }

    var config = context.Services.GetRequiredService<IHttpContextAccessor>();

    return new TimeoutModelBinder(config);
  }

  private class TimeoutModelBinder : CancellationTokenModelBinder, IModelBinder
  {
    private static readonly TimeSpan GracefulStopTimeLimit = TimeSpan.FromSeconds(3);

    private readonly IHttpContextAccessor _httpContextAccessor;

    public TimeoutModelBinder(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public new async Task BindModelAsync(ModelBindingContext bindingContext)
    {
      var httpContext = _httpContextAccessor.HttpContext!;
      var lambdaContext = httpContext.Items["LambdaContext"] as ILambdaContext;

      await base.BindModelAsync(bindingContext);
      if (bindingContext.Result.Model is CancellationToken cancellationToken
        && lambdaContext is not null
        && lambdaContext.RemainingTime > GracefulStopTimeLimit)
      {
        var remainingTimeSource = new CancellationTokenSource();
        remainingTimeSource.CancelAfter(lambdaContext.RemainingTime.Subtract(GracefulStopTimeLimit));

        var newTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
          remainingTimeSource.Token, cancellationToken);

        var model = (object) newTokenSource.Token;

        bindingContext.ValidationState.Clear();
        bindingContext.ValidationState.Add(model, new ValidationStateEntry
        {
          SuppressValidation = true
        });
        bindingContext.Result = ModelBindingResult.Success(model);
      }
    }
  }
}
