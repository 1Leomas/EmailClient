using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;

namespace EmailClient.EmailService;

internal class EmailReceiver: EmailService
{
    public EmailReceiver(string email, string password, string emailServer, int emailPort) : base(email, password, emailServer, emailPort)
    {
    }

    public async Task<IList<UniqueId>> ReceiveEmailAsync(SearchQuery searchQuery = null!)
    {
        using var client = new ImapClient();
        try
        {
            await client.ConnectAsync(_emailServer, _emailPort, SecureSocketOptions.SslOnConnect);
        }
        catch (Exception)
        {
            ConsoleEx.WriteLine($"Can not connect to {_emailServer}:{_emailPort}", ConsoleColor.Red);
            throw;
        }
        try
        {
            await client.AuthenticateAsync(_email, _password);
        }
        catch (Exception)
        {
            ConsoleEx.WriteLine("Can not authenticate with given credentials", ConsoleColor.Red);
            throw;
        }

        var inbox = client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadOnly);

        searchQuery = searchQuery ?? SearchQuery.All;
        return await inbox.SearchAsync(searchQuery);
    }
}