namespace QuickNavSim;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Quick Nav Sim");
        Console.WriteLine("---");

        try
        {
            Manager.Run();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ERROR: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}