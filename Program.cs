using Microsoft.EntityFrameworkCore;
using sgi_app.domain.entities;
using sgi_app.infrastructure.sql;
using sgi_app.application.ui;
using System;

namespace sgi_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Verificar la conexión a la base de datos
            var connectionTest = MySqlSingletonConnection.Instance.TestConnection();
            if (!connectionTest)
            {
                Console.WriteLine("❌ No se pudo conectar a la base de datos. Saliendo de la aplicación.");
                return; // Salir si la conexión falla
            }

            var context = new YourDbContext(); // Inicializa tu contexto de base de datos

            while (true)
            {
                Console.WriteLine("=== Menú Principal ===");
                Console.WriteLine("1. Panel de Clientes");
                Console.WriteLine("2. Panel de Compras");
                Console.WriteLine("3. Panel de Productos");
                Console.WriteLine("4. Panel de Proveedores");
                Console.WriteLine("5. Panel de Terceros");
                Console.WriteLine("6. Panel de Ventas");
                Console.WriteLine("7. Panel de Movimiento de Caja");
                Console.WriteLine("8. Panel de Detalle de Ventas");
                Console.WriteLine("9. Panel de Detalle de Compras");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        var clientePanel = new ClientePanel(context);
                        clientePanel.ShowMenu();
                        break;
                    case "2":
                        var compraPanel = new CompraPanel(context);
                        compraPanel.ShowMenu();
                        break;
                    case "3":
                        var productoPanel = new ProductoPanel(context);
                        productoPanel.ShowMenu();
                        break;
                    case "4":
                        var proveedorPanel = new ProveedorPanel(context);
                        proveedorPanel.ShowMenu();
                        break;
                    case "5":
                        var tercerosPanel = new TercerosPanel(context);
                        tercerosPanel.ShowMenu();
                        break;
                    case "6":
                        var ventaPanel = new VentaPanel(context);
                        ventaPanel.ShowMenu();
                        break;
                    case "7":
                        var movimientoCajaPanel = new MovimientoCajaPanel(context);
                        movimientoCajaPanel.ShowMenu();
                        break;
                    case "8":
                        var detalleVentaPanel = new DetalleVentaPanel(context);
                        detalleVentaPanel.ShowMenu();
                        break;
                    case "9":
                        var detalleCompraPanel = new DetalleCompraPanel(context);
                        detalleCompraPanel.ShowMenu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }
    }
}

namespace sgi_app.infrastructure.sql
{
    public class YourDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Terceros> Terceros { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<MovCaja> MovCaja { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Usar la conexión del singleton
            var connection = MySqlSingletonConnection.Instance.GetConnection();
            optionsBuilder.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 21)));
        }
    }
}
