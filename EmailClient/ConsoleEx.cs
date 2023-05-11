namespace EmailClient;

internal static class ConsoleEx
{
    public static void Write(string write, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.Write(write);
        Console.ForegroundColor = ConsoleColor.White;
    }
    public static void WriteLine(string write, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(write);
        Console.ForegroundColor = ConsoleColor.White;
    }
}