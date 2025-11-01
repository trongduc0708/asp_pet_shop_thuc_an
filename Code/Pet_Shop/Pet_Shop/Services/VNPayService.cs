using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Pet_Shop.Services
{
    public class VNPayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VNPayService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VNPayService(IConfiguration configuration, ILogger<VNPayService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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

                // Get client IP address
                var clientIp = GetClientIPAddress();

                var vnpParams = new SortedList<string, string>(new VnPayCompare());
                vnpParams.Add("vnp_Version", vnpVersion ?? "");
                vnpParams.Add("vnp_Command", vnpCommand ?? "");
                vnpParams.Add("vnp_TmnCode", vnpTmnCode ?? "");
                vnpParams.Add("vnp_Amount", ((long)(amount * 100)).ToString()); // Convert to cents
                vnpParams.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpParams.Add("vnp_CurrCode", vnpCurrCode ?? "");
                vnpParams.Add("vnp_IpAddr", clientIp);
                vnpParams.Add("vnp_Locale", vnpLocale ?? "");
                vnpParams.Add("vnp_OrderInfo", $"Thanh toan don hang {orderId}");
                vnpParams.Add("vnp_OrderType", vnpOrderType ?? "");
                vnpParams.Add("vnp_ReturnUrl", returnUrl);
                vnpParams.Add("vnp_TxnRef", orderId);

                // Create query string with URL encoding for both key and value
                var data = new StringBuilder();
                foreach (var kvp in vnpParams)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        data.Append(WebUtility.UrlEncode(kvp.Key) + "=" + WebUtility.UrlEncode(kvp.Value) + "&");
                    }
                }
                
                string queryString = data.ToString();
                if (queryString.Length > 0)
                {
                    queryString = queryString.Remove(queryString.Length - 1); // Remove last '&'
                }
                
                // Create secure hash
                var secureHash = CreateSecureHash(queryString, vnpHashSecret ?? "");
                
                // Create final payment URL
                var paymentUrl = $"{vnpUrl}?{queryString}&vnp_SecureHash={secureHash}";
                
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
                
                // Remove vnp_SecureHash and vnp_SecureHashType from parameters
                vnpParams.Remove("vnp_SecureHash");
                vnpParams.Remove("vnp_SecureHashType");
                
                // Sort parameters using VnPayCompare
                var sortedParams = new SortedList<string, string>(new VnPayCompare());
                foreach (var kvp in vnpParams)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        sortedParams.Add(kvp.Key, kvp.Value);
                    }
                }
                
                // Create query string with URL encoding for both key and value
                var data = new StringBuilder();
                foreach (var kvp in sortedParams)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        data.Append(WebUtility.UrlEncode(kvp.Key) + "=" + WebUtility.UrlEncode(kvp.Value) + "&");
                    }
                }
                
                string queryString = data.ToString();
                if (queryString.Length > 0)
                {
                    queryString = queryString.Remove(queryString.Length - 1); // Remove last '&'
                }
                
                // Create secure hash
                var secureHash = CreateSecureHash(queryString, vnpHashSecret ?? "");
                
                _logger.LogInformation($"Validating VNPay response - Expected: {vnpSecureHash}, Calculated: {secureHash}");
                
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

        // Debug method to test signature generation
        public string DebugSignatureGeneration(string orderId, decimal amount, string returnUrl)
        {
            try
            {
                var vnpTmnCode = _configuration["VNPaySettings:TmnCode"];
                var vnpHashSecret = _configuration["VNPaySettings:HashSecret"];
                var vnpVersion = _configuration["VNPaySettings:Version"];
                var vnpCommand = _configuration["VNPaySettings:Command"];
                var vnpCurrCode = _configuration["VNPaySettings:CurrCode"];
                var vnpLocale = _configuration["VNPaySettings:Locale"];
                var vnpOrderType = _configuration["VNPaySettings:OrderType"];

                var clientIp = GetClientIPAddress();

                var vnpParams = new SortedList<string, string>(new VnPayCompare());
                vnpParams.Add("vnp_Version", vnpVersion ?? "");
                vnpParams.Add("vnp_Command", vnpCommand ?? "");
                vnpParams.Add("vnp_TmnCode", vnpTmnCode ?? "");
                vnpParams.Add("vnp_Amount", ((long)(amount * 100)).ToString());
                vnpParams.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpParams.Add("vnp_CurrCode", vnpCurrCode ?? "");
                vnpParams.Add("vnp_IpAddr", clientIp);
                vnpParams.Add("vnp_Locale", vnpLocale ?? "");
                vnpParams.Add("vnp_OrderInfo", $"Thanh toan don hang {orderId}");
                vnpParams.Add("vnp_OrderType", vnpOrderType ?? "");
                vnpParams.Add("vnp_ReturnUrl", returnUrl);
                vnpParams.Add("vnp_TxnRef", orderId);

                var data = new StringBuilder();
                foreach (var kvp in vnpParams)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        data.Append(WebUtility.UrlEncode(kvp.Key) + "=" + WebUtility.UrlEncode(kvp.Value) + "&");
                    }
                }
                
                string queryString = data.ToString();
                if (queryString.Length > 0)
                {
                    queryString = queryString.Remove(queryString.Length - 1);
                }
                
                var secureHash = CreateSecureHash(queryString, vnpHashSecret ?? "");

                var debugInfo = $@"
VNPay Debug Information:
=======================
TmnCode: {vnpTmnCode}
HashSecret: {vnpHashSecret}
Query String: {queryString}
Secure Hash: {secureHash}
Client IP: {clientIp}
=======================";

                _logger.LogInformation(debugInfo);
                return debugInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in debug signature generation: {ex.Message}");
                return $"Error: {ex.Message}";
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

        private string GetClientIPAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return "127.0.0.1";
                }

                // Try to get IP from X-Forwarded-For header (for load balancers/proxies)
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    var ips = forwardedFor.Split(',');
                    if (ips.Length > 0)
                    {
                        return ips[0].Trim();
                    }
                }

                // Try to get IP from X-Real-IP header
                var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                {
                    return realIp.Trim();
                }

                // Try to get IP from X-Forwarded-Proto header
                var forwardedProto = httpContext.Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedProto))
                {
                    var connection = httpContext.Connection;
                    if (connection?.RemoteIpAddress != null)
                    {
                        return connection.RemoteIpAddress.ToString();
                    }
                }

                // Fallback to connection remote IP
                var remoteAddr = httpContext.Connection?.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(remoteAddr) && remoteAddr != "::1")
                {
                    return remoteAddr;
                }

                // Default fallback
                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
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

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
