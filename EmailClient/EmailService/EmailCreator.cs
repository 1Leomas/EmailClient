using MimeKit;

namespace EmailClient.EmailService;

internal class EmailCreator
{
    private string _emailRecipient = string.Empty;
    private string _nameRecipient = string.Empty;
    private string _emailCC = string.Empty;
    private string _nameCC = string.Empty;
    private string _subject = string.Empty;
    private string _messageText = string.Empty;
    private string _attachment = string.Empty;
        
    public MimeMessage? CreateEmail(string author, string email)   
    {
        var mimeMail = new MimeMessage();
        var builder = new BodyBuilder();

        mimeMail.From.Add(new MailboxAddress(author, email));

        int low = 0, high = 5;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Email creator\n");

            MenuOptions(_emailRecipient,  "1. Email");
            MenuOptions(_emailCC,  "2. CC address", ConsoleColor.DarkYellow);
            MenuOptions(_subject,  "3. Subject", ConsoleColor.DarkYellow);
            MenuOptions(_messageText,  "4. Message text", ConsoleColor.DarkYellow);
            MenuOptions(_attachment,  "5. Attachment", ConsoleColor.DarkYellow);

            if (_emailRecipient != string.Empty)
            {
                high = 6;
                Console.WriteLine("6. SendEmail");
            }

            Console.WriteLine("0: Exit");

            var select = ReadFromConsole("Select: ", low, high);

            switch (select)
            {
                case 1:
                    Console.Write("Enter recipient email name: ");
                    _nameRecipient = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter recipient email address: ");
                    _emailRecipient = Console.ReadLine() ?? string.Empty;
                    if (_emailRecipient != string.Empty && _nameRecipient != string.Empty)
                        mimeMail.To.Add(new MailboxAddress(_nameRecipient, _emailRecipient));
                    break;
                case 2:
                    Console.Write("Enter emailCC name: ");
                    _nameCC = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter emailCC address: ");
                    _emailCC = Console.ReadLine() ?? string.Empty;
                    if (_emailCC != string.Empty && _nameCC != string.Empty)
                        mimeMail.Cc.Add(new MailboxAddress(_nameCC, _emailCC));
                    break;
                case 3:
                    Console.Write("Enter email subject: ");
                    _subject = Console.ReadLine() ?? string.Empty;
                    if (_subject != string.Empty)
                        mimeMail.Subject = _subject;
                    break;
                case 4:
                    Console.Write("Enter email message text: ");
                    _messageText = Console.ReadLine() ?? string.Empty;
                    if (_messageText != string.Empty)
                        builder.TextBody = _messageText;
                    break;
                case 5:
                    Console.Write("Drag an drop file in the console: ");
                    _attachment = Console.ReadLine() ?? string.Empty;
                    if (_attachment != string.Empty)
                    {
                        _attachment = _attachment.Replace("\"", "");
                        builder.Attachments.Add(_attachment);
                        _attachment = Path.GetFileName(_attachment);
                    }
                    break;
                case 6:
                    mimeMail.Body = builder.ToMessageBody();
                    return mimeMail;
                case 0:
                    return null;
            }
        }
    }
        
    void MenuOptions(string option, string titleOption, ConsoleColor color = ConsoleColor.DarkRed)
    {
        if (option != string.Empty)
            ConsoleEx.WriteLine($"{titleOption}: {option}", ConsoleColor.DarkGreen);
        else
            ConsoleEx.WriteLine(titleOption, color);
    }

    int ReadFromConsole(string beforeInput, int low, int high)
    {
        int result;
        while (true)
        {
            Console.Write(beforeInput);
            var input = Console.ReadLine() ?? "";

            if (int.TryParse(input, out result))
            {
                if (result >= low && result <= high)
                    break;
            }
            ConsoleEx.WriteLine("Bad input", ConsoleColor.Red);
        }
        return result;
    }
}       