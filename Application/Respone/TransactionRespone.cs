﻿namespace Application.Respone
{
    public class TransactionRespone
    {
        public int Result { get; set; }
        public long? TotalAmount { get; set; }
        public long? TotalDiscount { get; set; }
        public long? FinalAmount { get; set; }
        public string ResultMessage { get; set; }
    }
}
