using Microsoft.EntityFrameworkCore;
using sgi_app.domain.entities;
using sgi_app.infrastructure.sql;
using sgi_app.application.ui;
using sgi_app.infrastructure.repositories;
using System;
using System.Collections.Generic;

namespace sgi_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Mostrar pantalla de bienvenida
            UIHelper.MostrarPantallaBienvenida();
            
            // Verificar la conexión a la base de datos
            var connectionTest = MySqlSingletonConnection.Instance.TestConnection();
            if (!connectionTest)
            {
                UIHelper.MostrarError("No se pudo conectar a la base de datos. Saliendo de la aplicación.");
                return; // Salir si la conexión falla
            }

            var context = new YourDbContext(); // Inicializa tu contexto de base de datos

            while (true)
            {
                // Mostrar menú con formato mejorado
                UIHelper.MostrarTitulo("Sistema de Gestión Integral");
                
                // Opciones del menú con descripciones
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Panel de Clientes       - Gestión de información de clientes" },
                    { "2", "Panel de Compras        - Registro y administración de compras" },
                    { "3", "Panel de Productos      - Inventario y gestión de productos" },
                    { "4", "Panel de Proveedores    - Administración de proveedores" },
                    { "5", "Panel de Terceros       - Gestión de personas y/o entidades" },
                    { "6", "Panel de Ventas         - Registro y seguimiento de ventas" },
                    { "7", "Panel de Movimientos    - Control de movimientos en caja" },
                    { "8", "Detalles de Ventas      - Gestión de líneas de venta" },
                    { "9", "Detalles de Compras     - Gestión de líneas de compra" },
                    { "10", "Planes Promocionales    - Gestión de promociones y descuentos" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

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
                        var proveedorRepository = new ProveedorRepository(context);
                        var proveedorPanel = new ProveedorPanel(proveedorRepository, context);
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
                    case "10":
                        var planPromocionalPanel = new PlanPromocionalPanel(context);
                        planPromocionalPanel.ShowMenu();
                        break;
                    case "0":
                        UIHelper.MostrarPantallaDespedida();
                        return;
                    default:
                        UIHelper.MostrarAdvertencia("Opción no válida. Intente de nuevo.");
                        Console.ReadKey();
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
        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
        public DbSet<TipoTercero> TipoTerceros { get; set; }
        public DbSet<Ciudad> Ciudades { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<PlanProducto> PlanProductos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Usamos la cadena de conexión directamente
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3306;Database=sgi-db;User=sgiapp;Password=kodigo777;SslMode=none;",
                new MySqlServerVersion(new Version(8, 0, 21)),
                options => options.EnableRetryOnFailure()
            );
        }
    }
}
