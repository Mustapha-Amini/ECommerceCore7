namespace ECommerceCore7.Models.ViewModels
{
    public class MessageViewModel
    {
        public string MainTitle { get; set; }
        public string SubTitle { get; set; }
        public string Message { get; set; }
        public string UrlToGo { get; set; }
        public string UrlToGoTitle { get; set; }
        public MessageType MessageType { get; set; }
    }

    public enum MessageType
    {
        Success,
        Warning,
        Danger,
        Info
    }
}
