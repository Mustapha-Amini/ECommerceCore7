namespace ECommerceCore7.Models.ViewModels
{
    public class BankParsianPaymentRequestViewModel
    {
        public string MerchantId { get; set; }
        public int PaymentId { get; }
        public int Amount { get; set; }
    }

    public class BankParsianPaymentResponseViewModel
    {
        public bool verified { get; set; }
        public string MerchantId { get; set; }

    }
}
