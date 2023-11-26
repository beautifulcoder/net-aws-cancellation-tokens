using Aws.CancellationTokens.AspNet.Binders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Aws.CancellationTokens.AspNet.Extensions;

public static class GracefulTimeoutExtensions
{
  public static MvcOptions ConfigureGracefulTimeout(this MvcOptions options)
  {
    options.ModelBinderProviders.RemoveType<CancellationTokenModelBinderProvider>();
    options.ModelBinderProviders.Insert(0, new TimeoutModelBinderProvider());

    return options;
  }
}
