using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace IO.Modules.Security
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _config;

		public EmailService(IConfiguration config)
		{
			_config = config;
		}

		public void SendEmail(EmailDto request)
		{
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("skph@interia.pl"));
			email.To.Add(MailboxAddress.Parse(request.To));
			email.Subject = request.Subject;
			email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

			using var smtp = new SmtpClient();
			smtp.Connect("poczta.interia.pl", 587, SecureSocketOptions.StartTls);
			smtp.Authenticate("skph@interia.pl", "ioponiedzialek10151234");
			smtp.Send(email);
			smtp.Disconnect(true);
		}
	}
}