using System.Diagnostics;

namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private string _mailTo = Startup.Configuration["MailSettings:MailToAddress"];
        private string _mailFrom = Startup.Configuration["MailSettings:MailFromAddress"];

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"LocalMailService: Mail sent to {_mailTo} from {_mailFrom} using LocalMailService");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}