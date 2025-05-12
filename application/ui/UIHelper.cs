using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.application.ui
{
    public static class UIHelper
    {
        // Standard colors for the application
        public static void ShowTitle(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║ {title.PadRight(63)} ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void ShowMenuOptions(Dictionary<string, string> options, string exitMessage = "Exit")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var option in options)
            {
                Console.WriteLine($"{option.Key}. {option.Value}");
            }
            Console.WriteLine($"0. {exitMessage}");
            Console.ResetColor();
            Console.Write("\nSelect an option: ");
        }

        public static void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void ShowError(string message, Exception ex = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            if (ex != null)
            {
                Console.WriteLine($"  Detail: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  Inner error: {ex.InnerException.Message}");
                }
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void ShowWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"! {message}");
            Console.ResetColor();
        }

        public static string RequestInput(string label, string defaultValue = "")
        {
            Console.Write($"{label}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            string input = Console.ReadLine();
            Console.ResetColor();
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }

        public static bool Confirm(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{message} (y/n): ");
            Console.ResetColor();
            var response = Console.ReadLine()?.ToLower();
            return response == "y";
        }

        // Method to draw tables
        public static void DrawTable<T>(IEnumerable<T> data, Dictionary<string, Func<T, object>> columns, 
                                        string title = "Results")
        {
            if (data == null || !data.Any())
            {
                ShowWarning("No data to display.");
                return;
            }

            // Determine the width of each column
            var widths = new Dictionary<string, int>();
            foreach (var col in columns)
            {
                // Initialize with header length
                widths[col.Key] = col.Key.Length;
                
                // Find the maximum value considering all data
                foreach (var item in data)
                {
                    var value = col.Value(item)?.ToString() ?? "NULL";
                    widths[col.Key] = Math.Max(widths[col.Key], value.Length);
                }
                
                // Add some padding
                widths[col.Key] += 2;
            }

            // Calculate total table width
            int totalWidth = columns.Sum(c => widths[c.Key]) + columns.Count + 1;

            // Draw table header
            Console.ForegroundColor = ConsoleColor.Cyan;
            
            // Top line
            Console.Write("╔");
            foreach (var col in columns)
            {
                Console.Write(new string('═', widths[col.Key]));
                if (col.Key != columns.Keys.Last())
                    Console.Write("╦");
            }
            Console.WriteLine("╗");
            
            // Column headers
            Console.Write("║");
            foreach (var col in columns)
            {
                Console.Write($" {col.Key.PadRight(widths[col.Key] - 2)} ");
                if (col.Key != columns.Keys.Last())
                    Console.Write("║");
            }
            Console.WriteLine("║");
            
            // Separator line
            Console.Write("╠");
            foreach (var col in columns)
            {
                Console.Write(new string('═', widths[col.Key]));
                if (col.Key != columns.Keys.Last())
                    Console.Write("╬");
            }
            Console.WriteLine("╣");
            Console.ResetColor();

            // Data rows
            foreach (var item in data)
            {
                Console.Write("║");
                foreach (var col in columns)
                {
                    var value = col.Value(item)?.ToString() ?? "NULL";
                    Console.Write($" {value.PadRight(widths[col.Key] - 2)} ");
                    if (col.Key != columns.Keys.Last())
                        Console.Write("║");
                }
                Console.WriteLine("║");
            }

            // Bottom line
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("╚");
            foreach (var col in columns)
            {
                Console.Write(new string('═', widths[col.Key]));
                if (col.Key != columns.Keys.Last())
                    Console.Write("╩");
            }
            Console.WriteLine("╝");
            Console.ResetColor();
            
            Console.WriteLine($"Total: {data.Count()} records");
        }

        public static void ShowWelcomeScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════╗
║                                                                          ║
║   ███████╗ ██████╗ ██╗      █████╗ ██████╗ ██████╗                      ║
║   ██╔════╝██╔════╝ ██║     ██╔══██╗██╔══██╗██╔══██╗                     ║
║   ███████╗██║  ███╗██║     ███████║██████╔╝██████╔╝                     ║
║   ╚════██║██║   ██║██║     ██╔══██║██╔═══╝ ██╔═══╝                      ║
║   ███████║╚██████╔╝███████╗██║  ██║██║     ██║                          ║
║   ╚══════╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝                          ║
║                                                                          ║
║            Integrated Management System - Administration                 ║
║                         Version 1.0.0                                    ║
║                                                                          ║
╚══════════════════════════════════════════════════════════════════════════╝
");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        public static void ShowGoodbyeScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════╗
║                                                                          ║
║         Thank you for using the Integrated Management System             ║
║                                                                          ║
║                    See you soon!                                         ║
║                                                                          ║
╚══════════════════════════════════════════════════════════════════════════╝
");
            Console.ResetColor();
        }

        // Maintain backward compatibility with old method names
        public static void MostrarTitulo(string titulo) => ShowTitle(titulo);
        public static void MostrarMenuOpciones(Dictionary<string, string> opciones, string mensajeSalida = "Salir") 
            => ShowMenuOptions(opciones, mensajeSalida);
        public static void MostrarExito(string mensaje) => ShowSuccess(mensaje);
        public static void MostrarError(string mensaje, Exception ex = null) => ShowError(mensaje, ex);
        public static void MostrarAdvertencia(string mensaje) => ShowWarning(mensaje);
        public static string SolicitarEntrada(string etiqueta, string valorPredeterminado = "") 
            => RequestInput(etiqueta, valorPredeterminado);
        public static bool Confirmar(string mensaje) => Confirm(mensaje);
        public static void DibujarTabla<T>(IEnumerable<T> datos, Dictionary<string, Func<T, object>> columnas, string titulo = "Resultados")
            => DrawTable(datos, columnas, titulo);
        public static void MostrarPantallaBienvenida() => ShowWelcomeScreen();
        public static void MostrarPantallaDespedida() => ShowGoodbyeScreen();
    }
} 