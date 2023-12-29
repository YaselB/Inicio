public interface IEmailService
{
    string SendEmailAsync(string email, string confirmationLink);
}