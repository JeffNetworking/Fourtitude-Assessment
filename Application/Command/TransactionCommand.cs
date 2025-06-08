using Application.Model;
using Application.Respone;
using MediatR;
using System.Globalization;

namespace Application.Command
{
    public class TransactionCommand : IRequest<TransactionRespone>
    {
        public string? PartnerKey { get; set; }
        public string? PartnerrefNo { get; set; }
        public string? PartnerPassword { get; set; }
        public long TotalAmount { get; set; }
        public IEnumerable<TransactionItem>? Items { get; set; }
        public string? TimeStamp { get; set; }
        public string? Sig { get; set; }


        public void Validate()
        {
            ValidateRequiredField();
            ValidateTotalAmount();
            ValidateItems();
            ValidateItemsRequiredField();
            ValidateDuplicateItemName();
            ValidateItemTotalAmount();
            ValidateTimestamp();
        }



        private void ValidateRequiredField()
        {
            if (string.IsNullOrWhiteSpace(PartnerKey))
                throw new Exception("partnerkey is required.");

            if (string.IsNullOrWhiteSpace(PartnerrefNo))
                throw new Exception("partnerrefno is required.");

            if (string.IsNullOrWhiteSpace(PartnerPassword))
                throw new Exception("partnerpassword is required.");

            if (string.IsNullOrWhiteSpace(TimeStamp))
                throw new Exception("timestamp is required.");

            if (string.IsNullOrWhiteSpace(Sig))
                throw new Exception("sig is required.");
        }

        private void ValidateTotalAmount()
        {
            if (TotalAmount <= 0)
                throw new Exception("totalamount must be greater than zero.");
        }

        private void ValidateItems()
        {
            if(Items == null && !Items.Any())
            {
                throw new Exception("Items is required.");
            }
        }

        private void ValidateItemsRequiredField()
        {
            if (Items != null && Items.Any())
            {
                foreach (var item in Items)
                {
                    if (string.IsNullOrWhiteSpace(item.PartnerItemRef))
                        throw new Exception("partneritemref is required.");

                    if (string.IsNullOrWhiteSpace(item.Name))
                        throw new Exception("name is required.");

                    if (item.Qty <= 0 || item.Qty > 5)
                        throw new Exception("Item.qty must be between 1 and 5.");

                    if (item.UnitPrice <= 0)
                        throw new Exception("Unitprice must be greater than zero.");
                }
               
            }
        }

        private void ValidateItemTotalAmount()
        {
            var itemSum = Items.Sum(x => x.Qty * x.UnitPrice);

            if (itemSum != TotalAmount)
            {
                throw new Exception("The total value in item details does not match the totalamount.");
            }
        }

        private void ValidateTimestamp()
        {
            if (!DateTime.TryParseExact(
              TimeStamp,
              "yyyy-MM-ddTHH:mm:ssZ",                 
              CultureInfo.InvariantCulture,
              DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
              out DateTime partnerTime))
            {
                throw new Exception("Invalid timestamp format.");
            }

            DateTime serverTime = DateTime.UtcNow;
            TimeSpan timeDiff = serverTime - partnerTime;

            if (Math.Abs(timeDiff.TotalMinutes) > 5)
                throw new Exception("Provided timestamp exceeds the allowed ±5 minute window.");
        }

        private void ValidateDuplicateItemName()
        {
            var duplicateNames = Items
                .GroupBy(x => x.Name?.Trim().ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Any())
                throw new Exception($"Duplicate item name(s) found: {string.Join(", ", duplicateNames)}");
        }
    }
}
