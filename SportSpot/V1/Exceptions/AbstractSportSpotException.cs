using System.Text.Json;

namespace SportSpot.V1.Exceptions
{
    public abstract class AbstractSportSpotException : Exception
    {

        public string Code { get; private set; }
        public int StatusCode { get; private set; }


        protected AbstractSportSpotException(string code, string message, int statusCode) : base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }

        protected AbstractSportSpotException(string code, string message, int statusCode, Exception ex) : base(message, ex)
        {
            Code = code;
            StatusCode = statusCode;
        }

        public async virtual Task WriteToResponse(HttpResponse response)
        {
            response.StatusCode = StatusCode;
            response.ContentType = "applicatzion/json";
            await response.WriteAsync(JsonSerializer.Serialize(GetErrors()));
        }

        public virtual List<ErrorResult> GetErrors()
        {
            return [new ErrorResult { Code = this.Code, Message = this.Message }];
        }
    }
}
