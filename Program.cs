using System;
using NSC.Winlator.Infrastructure;
using NSC.Winlator.Services;

namespace NSC.Winlator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("NSC Winlator Edition v1.0.0");
            Console.WriteLine("Initializing...");
            
            try
            {
                AppBootstrap.Initialize();
                Console.WriteLine("✓ Bootstrap complete");
                Console.WriteLine("✓ Storage initialized");
                Console.WriteLine($"✓ App folder: {AppBootstrap.ApplicationFolder}");
                Console.WriteLine("\nMod Manager Ready!");
                Console.WriteLine("Type 'help' for commands...\n");
                
                CommandHandler handler = new CommandHandler();
                handler.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                Console.WriteLine($"Details: {ex.StackTrace}");
            }
        }
    }
}
