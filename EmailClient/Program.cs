using EmailClient;
using EmailClient.EmailService;
using MailKit.Net.Smtp;
using MailKit.Search;

var author = string.Empty;
var email = string.Empty;
var password = string.Empty;

await Authentication();

var receiver = new EmailReceiver(
    email,
    password,
    "imap.gmail.com",
    993);
var sender = new EmailSender(
    email,
    password,
    "smtp.gmail.com",
    587);
var reader = new EmailReader(
    email,
    password,
    "imap.gmail.com",
    993);


while (true)
{
    Console.Clear();
    Console.WriteLine("Options");
    Console.WriteLine("1. Send email");
    Console.WriteLine("2. Get not seen emails");
    Console.WriteLine("3. Get all emails");
    Console.WriteLine("0. Exit");

    var selectedOption = ReadFromConsole();
    switch (selectedOption)
    {
        case 0: return;
        case 1: await SendEmail(); break;
        case 2: await ReceiveEmails(SearchQuery.NotSeen); break;
        case 3: await ReceiveEmails(); break;
        default: ConsoleEx.WriteLine("Bad option", ConsoleColor.DarkRed); break;
    }

    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}

async Task SendEmail()
{
    var creator = new EmailCreator();
    var destinationEmail = creator.CreateEmail(author, email);
    if (destinationEmail != null)
        await sender.SendEmailAsync(destinationEmail);   
}

async Task ReceiveEmails(SearchQuery searchQuery = null!)
{
    var messagesId = await receiver.ReceiveEmailAsync(searchQuery);

    await reader.ReadEmailAsync(messagesId);
}

byte ReadFromConsole()
{
    byte result;
    while (true)
    {
        Console.Write("Select: ");
        var input = Console.ReadLine() ?? "";

        if (byte.TryParse(input, out result))
            break;

        ConsoleEx.WriteLine("Bad input", ConsoleColor.Red);
    }
    return result;
}

async Task Authentication()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("Authentication process");

        if (string.IsNullOrEmpty(author))
        {
            Console.Write("\nYour name: ");
            author = Console.ReadLine() ?? "";
        }
        else
        {
            Console.WriteLine($"\nName: {author}");
        }

        Console.Write("Your gmail address: ");
        email = Console.ReadLine() ?? "";

        Console.Write("Your gmail service password: ");
        password = Console.ReadLine() ?? "";

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync("smtp.gmail.com", 587, false);
        }
        catch (Exception)
        {
            ConsoleEx.WriteLine("Can not connect to gmail server", ConsoleColor.Red);
            Exit();
        }
        try
        {
            await client.AuthenticateAsync(email, password);
            break;
        }
        catch (Exception)
        {
            ConsoleEx.WriteLine("Can not authenticate with given credentials", ConsoleColor.Red);
            email = string.Empty;
            password = string.Empty;
            Exit();
        }

        void Exit()
        {
            Console.Write("Try again(ENTER -> yes, any other key -> exit): ");
            var response = Console.ReadKey();
            if (response.Key != ConsoleKey.Enter)
                Environment.Exit(1);
        }
    }
}