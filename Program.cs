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
                Console.WriteLine("\nMod Manager Ready!");
                Console.WriteLine("Type 'help' for commands...\n");
                
                // Simple command loop
                while (true)
                {
                    Console.Write("> ");
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input)) continue;
                    
                    switch (input.ToLower())
                    {
                        case "help":
                            Console.WriteLine("Commands: list, exit");
                            break;
                        case "list":
                            Console.WriteLine("Feature list coming soon...");
                            break;
                        case "exit":
                            return;
                        default:
                            Console.WriteLine("Unknown command. Type 'help'");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                Console.WriteLine($"Details: {ex.StackTrace}");
            }
        }
    }
}
