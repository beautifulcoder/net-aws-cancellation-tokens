using Microsoft.AspNetCore.Mvc;

namespace Aws.CancellationTokens.AspNet.Controllers;

[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> Get(CancellationToken cancellationToken)
  {
    try
    {
      await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
      return Ok(new[] {"value1", "value2"});
    }
    catch (TaskCanceledException e)
    {
      return StatusCode(504, e.Message);
    }
  }
}
