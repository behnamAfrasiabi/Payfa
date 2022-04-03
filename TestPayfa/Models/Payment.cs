using System;

namespace TestPayfa.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string PayfaToken { get; set; }
        public long Amount { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime PayDate { get; set; }
        //...
    }
}
