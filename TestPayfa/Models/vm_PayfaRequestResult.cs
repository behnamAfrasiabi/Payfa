
namespace TestPayfa.Models
{
    public class vm_PayfaRequestResult
    {
        public string paymentUrl { get; set; }
        public string approvalUrl { get; set; }
        public string paymentId { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
        public string InvoiceId { get; set; }
    }
}
