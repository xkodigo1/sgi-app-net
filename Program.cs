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
            // Show welcome screen
            UIHelper.ShowWelcomeScreen();
            
            // Verify database connection
            var connectionTest = MySqlSingletonConnection.Instance.TestConnection();
            if (!connectionTest)
            {
                UIHelper.ShowError("Could not connect to the database. Exiting the application.");
                return; // Exit if connection fails
            }

            var context = new YourDbContext(); // Initialize database context

            while (true)
            {
                // Show menu with improved format
                UIHelper.ShowTitle("Integrated Management System");
                
                // Menu options with descriptions
                var options = new Dictionary<string, string>
                {
                    { "1", "Client Panel        - Client information management" },
                    { "2", "Purchases Panel     - Registration and administration of purchases" },
                    { "3", "Products Panel      - Inventory and product management" },
                    { "4", "Suppliers Panel     - Supplier administration" },
                    { "5", "Third Parties Panel - Management of people and/or entities" },
                    { "6", "Sales Panel         - Sales registration and tracking" },
                    { "7", "Cash Flow Panel     - Cash movement control" },
                    { "8", "Sales Details       - Management of sales lines" },
                    { "9", "Purchase Details    - Management of purchase lines" },
                    { "10", "Promotional Plans   - Management of promotions and discounts" },
                    { "11", "Employees Panel     - Employee and payroll management" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        var clientePanel = new ClientePanel(context);
                        clientePanel.ShowMenu();
                        break;
                    case "2":
                        var compraPanel = new CompraPanel(context);
                        compraPanel.ShowMenu().Wait();
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
                        ventaPanel.ShowMenu().Wait();
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
                    case "11":
                        var empleadoPanel = new EmpleadoPanel(context);
                        empleadoPanel.ShowMenu().Wait();
                        break;
                    case "0":
                        UIHelper.ShowGoodbyeScreen();
                        return;
                    default:
                        UIHelper.ShowWarning("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}

