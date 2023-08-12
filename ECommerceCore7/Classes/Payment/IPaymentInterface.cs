namespace ECommerceCore7.Classes.Payment
{
    public interface IPaymentInterface
    {
        string PluginTitle { get; }

        string ProcessPayment(
            string ReturnPageUrl,
            string PaymentDescription,
            PaymentData paymentData,
            string serializedSettings);

        PaymentResponse ProcessResponse(
            PaymentData paymentData,
            string serializedSettings);
    }
}
