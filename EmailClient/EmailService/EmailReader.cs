using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;

namespace EmailClient.EmailService;

internal class EmailReader: EmailService
{
    private Dictionary<int, UniqueId> _emailIdsIndexes;
    private List<MimeMessage> _emails;

    public EmailReader(string email, string password, string emailServer, int emailPort) 
        : base(email, password, emailServer, emailPort)
    {
    }

    public async Task ReadEmailAsync(IList<UniqueId> emailIds)
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

        _emailIdsIndexes = new();
        _emails = new();

        var inbox = client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite);

        for (var i = 0; i < emailIds.Count; i++)
        {
            var id = emailIds[i];
            var email = await inbox.GetMessageAsync(id);
            _emails.Add(email);

            _emailIdsIndexes.Add(i, id);
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nEmails in {0}: {1}\n", inbox.Name, _emails.Count);

            if (_emails.Count == 0)
                return;

            for (var i = 0; i < _emails.Count; i++)
            {
                var message = _emails[i];

                Console.Write("[{0:D2}] Form: ", i);
                ConsoleEx.Write(
                    $"{message.From.Mailboxes.First().Address,27} ", 
                    ConsoleColor.DarkCyan);
                Console.Write("Subject: ");
                ConsoleEx.WriteLine(message.Subject, ConsoleColor.DarkGreen);
            }

            Console.WriteLine("\nEnter number of email to read or -1 to go back");
            var emailIndex = ReadFromConsole("Email number: ", -1, _emails.Count-1);

            if (emailIndex == -1) return;

            Console.Clear();
            Console.WriteLine("EMAIL [{0:D2}]", emailIndex);
            Console.WriteLine("Author:  {0}", _emails[emailIndex].From.Mailboxes.First().Name);
            Console.WriteLine("Address: {0}", _emails[emailIndex].From.Mailboxes.First().Address);
            Console.WriteLine("Subject: {0}", _emails[emailIndex].Subject);
            Console.WriteLine("Message: {0}", _emails[emailIndex].TextBody);

            var attachments = _emails[emailIndex].Attachments;
            if (attachments.Any())
            {
                Console.WriteLine("Attachments:");
                foreach (var at in attachments)
                {
                    Console.WriteLine(at.ContentType.Name);
                }
                Console.WriteLine();
            }

            Console.WriteLine("\nOptions");
            Console.WriteLine(" 1. Mark message as seen");
            if (attachments.Any()) 
                Console.WriteLine(" 2. Download attachments");
            Console.WriteLine("-1. Back");
            Console.WriteLine(" 0. Exit");
            var select = ReadFromConsole("Select: ", -1, 2);

            switch (select)
            {
                case 1:
                    inbox.Store(_emailIdsIndexes[emailIndex], new StoreFlagsRequest(StoreAction.Add, MessageFlags.Seen) { Silent = true });

                    _emailIdsIndexes.Remove(emailIndex);
                    emailIds.RemoveAt(emailIndex);
                    _emails.RemoveAt(emailIndex);
                    break;
                case 2:
                    await DownloadAttachments(attachments);
                    break;
                case -1:
                    continue;
                case 0:
                    return; 
            } 
        }

        async Task DownloadAttachments(IEnumerable<MimeEntity> attachments)
        {
            const string folderPath = @"C:\Users\crutc\Downloads";

            foreach (var at in attachments)
            {
                var fileName = at.ContentType.Name;
                var filePath = Path.Combine(folderPath, fileName);

                await using var stream = File.Create(filePath);

                HandleMimeEntity(at, stream);
            }
        }

        static void HandleMimeEntity(MimeEntity entity, FileStream fileStream)
        {
            switch (entity)
            {
                case Multipart multipart:
                {
                    foreach (var t in multipart)
                        HandleMimeEntity(t, fileStream);
                    return;
                }
                case MessagePart rfc822:
                {
                    var message = rfc822.Message;

                    HandleMimeEntity(message.Body, fileStream);
                    return;
                }
                default:
                {
                    var part = (MimePart)entity;

                    part.Content.DecodeTo(fileStream);
                    break;
                }
            }
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
                    if(result >= low && result <= high)
                        break;
                }
                ConsoleEx.WriteLine("Bad input", ConsoleColor.Red);
            }
            return result;
        }
    }
}