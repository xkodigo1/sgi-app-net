using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.application.ui
{
    public static class UIHelper
    {
        // Colores estándar para la aplicación
        public static void MostrarTitulo(string titulo)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║ {titulo.PadRight(63)} ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void MostrarMenuOpciones(Dictionary<string, string> opciones, string mensajeSalida = "Salir")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var opcion in opciones)
            {
                Console.WriteLine($"{opcion.Key}. {opcion.Value}");
            }
            Console.WriteLine($"0. {mensajeSalida}");
            Console.ResetColor();
            Console.Write("\nSeleccione una opción: ");
        }

        public static void MostrarExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {mensaje}");
            Console.ResetColor();
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public static void MostrarError(string mensaje, Exception ex = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {mensaje}");
            if (ex != null)
            {
                Console.WriteLine($"  Detalle: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  Error interno: {ex.InnerException.Message}");
                }
            }
            Console.ResetColor();
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public static void MostrarAdvertencia(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"! {mensaje}");
            Console.ResetColor();
        }

        public static string SolicitarEntrada(string etiqueta, string valorPredeterminado = "")
        {
            Console.Write($"{etiqueta}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            string entrada = Console.ReadLine();
            Console.ResetColor();
            return string.IsNullOrWhiteSpace(entrada) ? valorPredeterminado : entrada;
        }

        public static bool Confirmar(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{mensaje} (s/n): ");
            Console.ResetColor();
            var respuesta = Console.ReadLine()?.ToLower();
            return respuesta == "s";
        }

        // Método para dibujar tablas
        public static void DibujarTabla<T>(IEnumerable<T> datos, Dictionary<string, Func<T, object>> columnas, 
                                        string titulo = "Resultados")
        {
            if (datos == null || !datos.Any())
            {
                MostrarAdvertencia("No hay datos para mostrar.");
                return;
            }

            // Determinar el ancho de cada columna
            var anchos = new Dictionary<string, int>();
            foreach (var col in columnas)
            {
                // Inicializar con el largo del encabezado
                anchos[col.Key] = col.Key.Length;
                
                // Encontrar el valor máximo considerando todos los datos
                foreach (var item in datos)
                {
                    var valor = col.Value(item)?.ToString() ?? "NULL";
                    anchos[col.Key] = Math.Max(anchos[col.Key], valor.Length);
                }
                
                // Añadir un poco de padding
                anchos[col.Key] += 2;
            }

            // Calcular ancho total de la tabla
            int anchoTotal = columnas.Sum(c => anchos[c.Key]) + columnas.Count + 1;

            // Dibujar encabezado de la tabla
            Console.ForegroundColor = ConsoleColor.Cyan;
            
            // Línea superior
            Console.Write("╔");
            foreach (var col in columnas)
            {
                Console.Write(new string('═', anchos[col.Key]));
                if (col.Key != columnas.Keys.Last())
                    Console.Write("╦");
            }
            Console.WriteLine("╗");
            
            // Encabezados de columna
            Console.Write("║");
            foreach (var col in columnas)
            {
                Console.Write($" {col.Key.PadRight(anchos[col.Key] - 2)} ");
                if (col.Key != columnas.Keys.Last())
                    Console.Write("║");
            }
            Console.WriteLine("║");
            
            // Línea separadora
            Console.Write("╠");
            foreach (var col in columnas)
            {
                Console.Write(new string('═', anchos[col.Key]));
                if (col.Key != columnas.Keys.Last())
                    Console.Write("╬");
            }
            Console.WriteLine("╣");
            Console.ResetColor();

            // Filas de datos
            foreach (var item in datos)
            {
                Console.Write("║");
                foreach (var col in columnas)
                {
                    var valor = col.Value(item)?.ToString() ?? "NULL";
                    Console.Write($" {valor.PadRight(anchos[col.Key] - 2)} ");
                    if (col.Key != columnas.Keys.Last())
                        Console.Write("║");
                }
                Console.WriteLine("║");
            }

            // Línea inferior
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("╚");
            foreach (var col in columnas)
            {
                Console.Write(new string('═', anchos[col.Key]));
                if (col.Key != columnas.Keys.Last())
                    Console.Write("╩");
            }
            Console.WriteLine("╝");
            Console.ResetColor();
            
            Console.WriteLine($"Total: {datos.Count()} registros");
        }

        public static void MostrarPantallaBienvenida()
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
║            Sistema de Gestión Integral - Administración                  ║
║                         Versión 1.0.0                                    ║
║                                                                          ║
╚══════════════════════════════════════════════════════════════════════════╝
");
            Console.ResetColor();
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
        
        public static void MostrarPantallaDespedida()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════╗
║                                                                          ║
║         Gracias por utilizar el Sistema de Gestión Integral              ║
║                                                                          ║
║                    ¡Hasta pronto!                                        ║
║                                                                          ║
╚══════════════════════════════════════════════════════════════════════════╝
");
            Console.ResetColor();
        }
    }
} 