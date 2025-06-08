using Application.Model;

namespace Application.Request
{
    public class TransactionRequest
    {
        public string PartnerKey { get; set; }
        public string PartnerrefNo { get; set; }
        public string PartnerPassword { get; set; }
        public long TotalAmount { get; set; }
        public IEnumerable<TransactionItem> Items { get; set; }
        public string TimeStamp { get; set; }
        public string Sig { get; set; }
    }
}
