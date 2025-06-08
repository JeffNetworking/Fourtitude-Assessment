using log4net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Application.Helper
{
    public static class HelperModule
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        public static string ComputeSignature(string timestamp, string partnerKey, string partnerRefNo, long totalAmount, string partnerPasswordBase64)
        {
            DateTimeOffset dto = DateTimeOffset.Parse(timestamp);

            string timestampUtc = dto.UtcDateTime.ToString("yyyyMMddHHmmss");  

            string raw = $"{timestampUtc}{partnerKey}{partnerRefNo}{totalAmount}{partnerPasswordBase64}";

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));

            string hex = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(hex));

            return base64;
        }

        public static (long totalDiscount, long finalAmount) CalculateDiscount(long totalAmount)
        {

            _logger.Info("Access CalculateDiscount");

            if (totalAmount <= 0)
                return (0, totalAmount);

            double baseDiscountPercent = 0;
            double conditionalDiscountPercent = 0;

            if (totalAmount >= 20000 && totalAmount <= 50000)          
                baseDiscountPercent = 0.05;
            else if (totalAmount >= 50001 && totalAmount <= 80000)    
                baseDiscountPercent = 0.07;
            else if (totalAmount >= 80001 && totalAmount <= 120000)    
                baseDiscountPercent = 0.10;
            else if (totalAmount > 120000)                             
                baseDiscountPercent = 0.15;

            if (totalAmount > 50000 && IsPrime(totalAmount))           
                conditionalDiscountPercent += 0.08;

            if (totalAmount > 90000 && ((totalAmount / 100) % 10 == 5))          
                conditionalDiscountPercent += 0.10;

            double totalDiscountPercent = baseDiscountPercent + conditionalDiscountPercent;
            if (totalDiscountPercent > 0.20)
                totalDiscountPercent = 0.20;

            long totalDiscount = (long)(totalAmount * totalDiscountPercent);
            long finalAmount = totalAmount - totalDiscount;

            _logger.Info($"CalculateDiscount-totalDiscount:{totalDiscount}, finalAmount:{finalAmount}");

            return (totalDiscount, finalAmount);
        }

        private static bool IsPrime(long number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (long)Math.Sqrt(number);
            for (long i = 3; i <= boundary; i += 2)
                if (number % i == 0) return false;

            return true;
        }
    }
}
