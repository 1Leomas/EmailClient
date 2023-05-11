namespace EmailClient.EmailService;

internal abstract class EmailService
{
    protected readonly string _email;
    protected readonly string _password;

    protected readonly string _emailServer;
    protected readonly int _emailPort;

    public EmailService(string email, string password, string emailServer, int emailPort)
    {
        _email = email;
        _password = password;
        _emailServer = emailServer;
        _emailPort = emailPort;
    }
}