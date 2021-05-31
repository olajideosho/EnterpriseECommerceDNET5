using System;
namespace Packt.Ecommerce.Common.Models
{
    public class ExceptionResponse
    {
        public string ErrorMessage { get; set; }
        public string CorrelationIdentifier { get; set; }
        public string InnerException { get; set; }
    }
}
