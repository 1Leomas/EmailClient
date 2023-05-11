using MailKit.Net.Smtp;
using MimeKit;

namespace EmailClient.EmailService;

internal class EmailSender: EmailService
{
    public EmailSender(string email, string password, string emailServer, int emailPort) : base(email, password, emailServer, emailPort)
    {
    }   

    public async Task SendEmailAsync(MimeMessage email)
    {
        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_emailServer, _emailPort, false);
        }
        catch (Exception)
        {
            ConsoleEx.WriteLine($"Can not connect to {_emailServer}:{_emailPort}", ConsoleColor.Red);
            return;
        }
        try
        {
            await client.AuthenticateAsync(_email, _password);
        }
        catch (Exception)
        {
            ConsoleEx.WriteLine("Can not authenticate with given credentials", ConsoleColor.Red);
            return;
        }

        var response = await client.SendAsync(email);

        Console.WriteLine("\nMessage sent!");
        Console.WriteLine(response);

        await client.DisconnectAsync(true);
    }

    
}