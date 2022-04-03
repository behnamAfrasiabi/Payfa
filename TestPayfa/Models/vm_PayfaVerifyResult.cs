
namespace TestPayfa.Models
{
    public class vm_PayfaVerifyResult
    {
        public string cardNo { get; set; }
        public string transactionId { get; set; }
        public long amount { get; set; }
        public string invoiceId { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }
    }
}
