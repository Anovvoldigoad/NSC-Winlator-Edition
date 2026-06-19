using System;
using System.Threading.Tasks;
using NSC.Winlator.Infrastructure;
using NSC.Winlator.Services;

namespace NSC.Winlator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("NSC Winlator Edition v1.0.0");
            Console.WriteLine("Initializing...");
            
            try
            {
                AppBootstrap.Initialize();
                Console.WriteLine("✓ Bootstrap complete");
                Console.WriteLine("✓ Storage initialized");
                Console.WriteLine($"✓ App folder: {AppBootstrap.ApplicationFolder}");
                
                // Start HTTP server in background
                var httpServer = new HttpServerService(5000);
                _ = Task.Run(() => httpServer.Start());
                
                Console.WriteLine("\nMod Manager Ready!");
                Console.WriteLine("HTTP API: http://localhost:5000");
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
