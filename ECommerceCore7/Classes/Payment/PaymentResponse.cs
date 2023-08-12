namespace ECommerceCore7.Classes.Payment
{
    public class PaymentResponse
    {
        public int ResponseCode;
        public string ResponseMessage;
        public bool Successful;
        public string TransactionID;
        public string PaymentTerminalID { get; set; }
    }
}