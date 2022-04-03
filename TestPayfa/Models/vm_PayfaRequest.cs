
namespace TestPayfa.Models
{
    public class vm_PayfaRequest
    {
        public string amount { get; set; }
        public string callbackUrl { get; set; }
        public string invoiceId { get; set; }
        public string mobileNumber { get; set; }
    }
}
