
using System.Net.Mail;
using Microsoft.AspNetCore.Http.HttpResults;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Org.BouncyCastle.Utilities;

public class EmailService : IEmailService
{
private readonly IConfiguration _configuration;
    
    public EmailService (IConfiguration configuracion){
    _configuration = configuracion ;
    }
   public string SendEmailAsync(string email, string confirmacionLink){
    string token =  GenerateRandomToken(4);
   MailMessage mailMessage = new MailMessage("yaselbarrioscarrillo@gmail.com" , email ,"Yasel" ,confirmacionLink+token);
   mailMessage.IsBodyHtml = true;
   System.Net.Mail.SmtpClient smtpClient  = new System.Net.Mail.SmtpClient("smtp.gmail.com");
   smtpClient.EnableSsl = true;
   smtpClient.UseDefaultCredentials = false;
   
   smtpClient.Port = Convert.ToInt32(_configuration.GetSection("Email:Port").Value);
   smtpClient.Credentials = new System.Net.NetworkCredential(_configuration.GetSection("Email:Username").Value, _configuration.GetSection("Email:Password").Value);
   smtpClient.Send(mailMessage);
   return token; 
   }

    public string GenerateRandomToken(int length)
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var random = new Random();
    var token = new char[length];
    
    for (int i = 0; i < length; i++)
    {
        token[i] = chars[random.Next(chars.Length)];
    }
    
    return new string(token);
}
}
   
