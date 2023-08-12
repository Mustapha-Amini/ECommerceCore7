namespace ECommerceCore7.Classes.Payment
{
    public class PaymentData
    {
        public int OnlinePaymentID;

        public int PaymentAmount;
        public DateTime OnlinePaymentDate { get; set; }
        public string PaymentTerminalID { get; set; }
        public string PaymentReturnUrl { get; set; }
    }
}