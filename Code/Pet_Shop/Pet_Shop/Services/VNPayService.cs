using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace Pet_Shop.Services
{
    public class VNPayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VNPayService> _logger;

        public VNPayService(IConfiguration configuration, ILogger<VNPayService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string CreatePaymentUrl(string orderId, decimal amount, string returnUrl)
        {
            try
            {
                // Get VNPay configuration from appsettings.json
                var vnpUrl = _configuration["VNPaySettings:Url"];
                var vnpTmnCode = _configuration["VNPaySettings:TmnCode"];
                var vnpHashSecret = _configuration["VNPaySettings:HashSecret"];
                var vnpVersion = _configuration["VNPaySettings:Version"];
                var vnpCommand = _configuration["VNPaySettings:Command"];
                var vnpCurrCode = _configuration["VNPaySettings:CurrCode"];
                var vnpLocale = _configuration["VNPaySettings:Locale"];
                var vnpOrderType = _configuration["VNPaySettings:OrderType"];
                var vnpReturnUrl = returnUrl;

                var vnpParams = new SortedList<string, string>();
                vnpParams.Add("vnp_Version", vnpVersion);
                vnpParams.Add("vnp_Command", vnpCommand);
                vnpParams.Add("vnp_TmnCode", vnpTmnCode);
                vnpParams.Add("vnp_Amount", ((long)(amount * 100)).ToString()); // Convert to cents
                vnpParams.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpParams.Add("vnp_CurrCode", vnpCurrCode);
                vnpParams.Add("vnp_IpAddr", "127.0.0.1");
                vnpParams.Add("vnp_Locale", vnpLocale);
                vnpParams.Add("vnp_OrderInfo", $"Thanh toan don hang {orderId}");
                vnpParams.Add("vnp_OrderType", vnpOrderType);
                vnpParams.Add("vnp_ReturnUrl", vnpReturnUrl);
                vnpParams.Add("vnp_TxnRef", orderId);

                // Create query string
                var queryString = string.Join("&", vnpParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
                
                // Create secure hash
                var secureHash = CreateSecureHash(queryString, vnpHashSecret);
                queryString += $"&vnp_SecureHash={secureHash}";

                var paymentUrl = $"{vnpUrl}?{queryString}";
                
                _logger.LogInformation($"Created VNPay URL for order {orderId}: {paymentUrl}");
                return paymentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating VNPay URL for order {orderId}: {ex.Message}");
                throw;
            }
        }

        public bool ValidatePaymentResponse(Dictionary<string, string> vnpParams, string vnpSecureHash)
        {
            try
            {
                var vnpHashSecret = _configuration["VNPaySettings:HashSecret"];
                
                // Remove vnp_SecureHash from parameters
                vnpParams.Remove("vnp_SecureHash");
                vnpParams.Remove("vnp_SecureHashType");
                
                // Sort parameters
                var sortedParams = new SortedList<string, string>();
                foreach (var kvp in vnpParams)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        sortedParams.Add(kvp.Key, kvp.Value);
                    }
                }
                
                // Create query string
                var queryString = string.Join("&", sortedParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
                
                // Create secure hash
                var secureHash = CreateSecureHash(queryString, vnpHashSecret);
                
                return secureHash.Equals(vnpSecureHash, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating VNPay response: {ex.Message}");
                return false;
            }
        }

        public VNPayPaymentResult ProcessPaymentResponse(Dictionary<string, string> vnpParams)
        {
            try
            {
                var result = new VNPayPaymentResult
                {
                    OrderId = vnpParams.GetValueOrDefault("vnp_TxnRef", ""),
                    TransactionId = vnpParams.GetValueOrDefault("vnp_TransactionNo", ""),
                    Amount = decimal.Parse(vnpParams.GetValueOrDefault("vnp_Amount", "0")) / 100, // Convert from cents
                    ResponseCode = vnpParams.GetValueOrDefault("vnp_ResponseCode", ""),
                    TransactionStatus = vnpParams.GetValueOrDefault("vnp_TransactionStatus", ""),
                    BankCode = vnpParams.GetValueOrDefault("vnp_BankCode", ""),
                    PayDate = vnpParams.GetValueOrDefault("vnp_PayDate", ""),
                    SecureHash = vnpParams.GetValueOrDefault("vnp_SecureHash", "")
                };

                // Validate response
                result.IsValid = ValidatePaymentResponse(vnpParams, result.SecureHash);
                result.IsSuccess = result.IsValid && result.ResponseCode == "00";

                _logger.LogInformation($"Processed VNPay response for order {result.OrderId}: Success={result.IsSuccess}, ResponseCode={result.ResponseCode}");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing VNPay response: {ex.Message}");
                return new VNPayPaymentResult { IsValid = false, IsSuccess = false };
            }
        }

        private string CreateSecureHash(string queryString, string hashSecret)
        {
            using (var hmacsha512 = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret)))
            {
                var hashBytes = hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(queryString));
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }
    }

    public class VNPayPaymentResult
    {
        public string OrderId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ResponseCode { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string PayDate { get; set; } = string.Empty;
        public string SecureHash { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public bool IsSuccess { get; set; }
    }
}
