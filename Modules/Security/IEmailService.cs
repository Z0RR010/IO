namespace IO.Modules.Security
{
	public interface IEmailService
	{
		void SendEmail(EmailDto request);
	}
}