using SportSpot.V1.Exceptions;

namespace SportSpot.ExceptionHandling
{
    public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> _logger, RequestDelegate _next)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AbstractSportSpotException ex)
            {
                _logger.LogError(ex, ex.Message);
                await ex.WriteToResponse(context.Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await new InternalServerErrorException().WriteToResponse(context.Response);
            }
        }
    }
}
