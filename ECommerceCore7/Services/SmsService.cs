using ECommerceCore7.Models;
using Microsoft.Extensions.Configuration;
using PARSGREEN.CORE.RESTful.SMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceCore7.Services
{
    public interface ISmsService
    {
        Task SendSms(string SmsText, List<string> ReceipentPhones);
    }

    public class SmsService : ISmsService
    {
        private ParsGreenConfig parsGreenConfig = new ParsGreenConfig();
        private IConfiguration Configuration { get; }

        public SmsService(IConfiguration configuration)
        {
            Configuration = configuration;
            parsGreenConfig.SendFromNumber = Configuration.GetValue<string>("ParsGreen:SendFromNumber");
            parsGreenConfig.ApiToken = Configuration.GetValue<string>("ParsGreen:ApiToken");
        }
        public async Task SendSms(string SmsText, List<string> ReceipentPhones)
        {
            await Task.Run(() =>
            {
                Message msg = new Message(parsGreenConfig.ApiToken);
                msg.SendSms(SmsText, ReceipentPhones.ToArray(), parsGreenConfig.SendFromNumber);
            }).ConfigureAwait(false);

        }
    }
}
