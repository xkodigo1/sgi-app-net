using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class VentaPanel
    {
        private readonly YourDbContext _context;

        public VentaPanel(YourDbContext context)
        {
            _context = context;
        }

        public async Task ShowMenu()
        {
            while (true)
            {
                UIHelper.ShowTitle("Sales Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Sales" },
                    { "2", "Create New Sale" },
                    { "3", "Edit Sale" },
                    { "4", "Delete Sale" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListSales();
                        break;
                    case "2":
                        await CreateSaleAsync();
                        break;
                    case "3":
                        await UpdateSaleAsync();
                        break;
                    case "4":
                        await DeleteSaleAsync();
                        break;
                    case "0":
                        return;
                    default:
                        UIHelper.ShowWarning("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ListSales()
        {
            UIHelper.ShowTitle("Sales List");
            
            try
            {
                var ventas = _context.Ventas.ToList();
                
                // Define columns and values to display
                var columns = new Dictionary<string, Func<Venta, object>>
                {
                    { "ID", v => v.Id },
                    { "Employee", v => GetEmployeeName(v.TerceroEnId) },
                    { "Client", v => v.TerceroCliId },
                    { "Invoice", v => v.FactId },
                    { "Date", v => v.Fecha.ToShortDateString() },
                    { "Total", v => GetSaleTotal(v.Id) }
                };
                
                // Use DrawTable method to show formatted data
                UIHelper.DrawTable(ventas, columns, "Sales Records");
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing sales", ex);
            }
        }
        
        // Helper method to get employee name
        private string GetEmployeeName(string terceroId)
        {
            try
            {
                var tercero = _context.Terceros.Find(terceroId);
                if (tercero != null)
                {
                    return $"{tercero.Nombre} {tercero.Apellidos}";
                }
                return terceroId;
            }
            catch
            {
                return terceroId;
            }
        }
        
        // Helper method to calculate sale total
        private string GetSaleTotal(int ventaId)
        {
            try
            {
                var detalles = _context.DetalleVentas.Where(d => d.VentaId == ventaId).ToList();
                decimal total = detalles.Sum(d => d.Cantidad * d.Valor);
                return $"{total:C}";
            }
            catch
            {
                return "Not calculated";
            }
        }

        private async Task CreateSaleAsync()
        {
            UIHelper.ShowTitle("Create New Sale");
            
            try
            {
                // Mostrar empleados disponibles
                await ShowAvailableEmployees();
                
                var empleadoIdStr = UIHelper.SolicitarEntrada("Enter the ID of the employee");
                if (string.IsNullOrWhiteSpace(empleadoIdStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. The employee ID is required.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                    return;
                }
                
                var empleadoId = int.Parse(empleadoIdStr);
                
                // Verificar que el empleado exista
                var empleado = await _context.Empleados.FindAsync(empleadoId);
                if (empleado == null)
                {
                    UIHelper.ShowError($"Employee with ID {empleadoId} does not exist.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                    return;
                }
                
                // Obtener el tercero asociado al empleado
                var terceroEmpleadoId = empleado.TerceroId;
                var terceroEmpleado = await _context.Terceros.FindAsync(terceroEmpleadoId);
                if (terceroEmpleado == null)
                {
                    UIHelper.ShowError($"The third party associated with the employee does not exist.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                    return;
                }
                
                // Mostrar clientes disponibles
                await ShowAvailableClients();
                
                var clienteId = UIHelper.SolicitarEntrada("Enter the ID of the client");
                if (string.IsNullOrWhiteSpace(clienteId))
                {
                    UIHelper.ShowWarning("Operation cancelled. The client ID is required.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                    return;
                }
                
                // Verificar que el cliente exista
                var cliente = await _context.Terceros.FindAsync(clienteId);
                if (cliente == null)
                {
                    UIHelper.ShowError($"Client with ID {clienteId} does not exist. You must create the client first.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                    return;
                }
                
                var factIdStr = UIHelper.SolicitarEntrada("Enter the ID of the invoice");
                if (string.IsNullOrWhiteSpace(factIdStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. The invoice ID is required.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                    return;
                }
                
                var factId = int.Parse(factIdStr);
                
                var fechaStr = UIHelper.SolicitarEntrada("Enter the date (YYYY-MM-DD)", DateTime.Now.ToString("yyyy-MM-dd"));
                var fecha = DateTime.Parse(fechaStr);

                var venta = new Venta { 
                    TerceroEnId = terceroEmpleadoId, 
                    TerceroCliId = clienteId, 
                    FactId = factId,
                    Fecha = fecha 
                };
                
                // Mostrar resumen antes de confirmar
                UIHelper.ShowTitle("Sale Summary");
                Console.WriteLine($"Employee: {GetEmployeeName(venta.TerceroEnId)} (ID: {empleadoId})");
                Console.WriteLine($"Client: {cliente.Nombre} {cliente.Apellidos}");
                Console.WriteLine($"Invoice: {venta.FactId}");
                Console.WriteLine($"Date: {venta.Fecha.ToShortDateString()}");
                
                if (UIHelper.Confirmar("Do you want to save this sale?"))
                {
                    _context.Ventas.Add(venta);
                    await _context.SaveChangesAsync();
                    UIHelper.ShowSuccess($"Sale created successfully with ID: {venta.Id}");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating sale", ex);
                Console.WriteLine("\nPress any key to return to the sales menu...");
                Console.ReadKey();
            }
        }

        private async Task UpdateSaleAsync()
        {
            UIHelper.ShowTitle("Edit Sale");
            
            try
            {
                // Mostrar lista de ventas disponibles
                ListSales();
                
                var idStr = UIHelper.SolicitarEntrada("Enter the ID of the sale to edit");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. The ID is required.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                    return;
                }
                
                var id = int.Parse(idStr);
                var venta = await _context.Ventas.FindAsync(id);

                if (venta != null)
                {
                    // Mostrar información actual
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {venta.Id}");
                    Console.WriteLine($"Employee: {GetEmployeeName(venta.TerceroEnId)}");
                    Console.WriteLine($"Client: {venta.TerceroCliId}");
                    Console.WriteLine($"Invoice: {venta.FactId}");
                    Console.WriteLine($"Date: {venta.Fecha.ToShortDateString()}");
                    Console.WriteLine("\nEnter new values or leave blank to keep current:");
                    
                    // Preguntar si se desea cambiar el empleado
                    if (UIHelper.Confirmar("Do you want to change the assigned employee for this sale?"))
                    {
                        // Mostrar empleados disponibles
                        await ShowAvailableEmployees();
                        
                        var empleadoIdStr = UIHelper.SolicitarEntrada("Enter the new ID of the employee");
                        if (!string.IsNullOrWhiteSpace(empleadoIdStr))
                        {
                            var empleadoId = int.Parse(empleadoIdStr);
                            
                            // Verificar que el empleado exista
                            var empleado = await _context.Empleados.FindAsync(empleadoId);
                            if (empleado == null)
                            {
                                UIHelper.ShowError($"Employee with ID {empleadoId} does not exist.");
                                Console.WriteLine("\nPress any key to return to the sales menu...");
                                Console.ReadKey();
                                return;
                            }
                            
                            // Obtener el tercero asociado al empleado
                            venta.TerceroEnId = empleado.TerceroId;
                        }
                    }
                    
                    // Preguntar si se desea cambiar el cliente
                    if (UIHelper.Confirmar("Do you want to change the assigned client for this sale?"))
                    {
                        // Mostrar clientes disponibles
                        await ShowAvailableClients();
                        
                        var clienteId = UIHelper.SolicitarEntrada("New ID of the client", venta.TerceroCliId);
                        
                        // Verificar que el cliente exista
                        var cliente = await _context.Terceros.FindAsync(clienteId);
                        if (cliente == null)
                        {
                            UIHelper.ShowError($"Client with ID {clienteId} does not exist. You must create the client first.");
                            Console.WriteLine("\nPress any key to return to the sales menu...");
                            Console.ReadKey();
                            return;
                        }
                        
                        venta.TerceroCliId = clienteId;
                    }
                    
                    var factIdStr = UIHelper.SolicitarEntrada("New invoice ID", venta.FactId.ToString());
                    var factId = int.Parse(factIdStr);
                    
                    var fechaStr = UIHelper.SolicitarEntrada("New date (YYYY-MM-DD)", venta.Fecha.ToString("yyyy-MM-dd"));
                    
                    venta.FactId = factId;
                    venta.Fecha = DateTime.Parse(fechaStr);

                    if (UIHelper.Confirmar("Are you sure you want to apply these changes?"))
                    {
                        _context.Update(venta);
                        await _context.SaveChangesAsync();
                        UIHelper.ShowSuccess("Sale updated successfully.");
                        Console.WriteLine("\nPress any key to return to the sales menu...");
                        Console.ReadKey();
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                        Console.WriteLine("\nPress any key to return to the sales menu...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    UIHelper.ShowError("Sale not found.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating sale", ex);
                Console.WriteLine("\nPress any key to return to the sales menu...");
                Console.ReadKey();
            }
        }

        private async Task DeleteSaleAsync()
        {
            UIHelper.ShowTitle("Delete Sale");
            
            try
            {
                // Mostrar lista de ventas disponibles
                ListSales();
                
                var idStr = UIHelper.SolicitarEntrada("Enter the ID of the sale to delete");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. The ID is required.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                    return;
                }
                
                var id = int.Parse(idStr);
                var venta = await _context.Ventas.FindAsync(id);

                if (venta != null)
                {
                    // Verificar si existen detalles asociados
                    var detalles = await _context.DetalleVentas.Where(d => d.VentaId == id).ToListAsync();
                    if (detalles.Any())
                    {
                        UIHelper.ShowWarning($"The sale has {detalles.Count} associated details. These will also be deleted.");
                        
                        // Mostrar los detalles que se eliminarán
                        var columnasDetalles = new Dictionary<string, Func<DetalleVenta, object>>
                        {
                            { "ID", d => d.Id },
                            { "Product", d => d.ProductosId },
                            { "Quantity", d => d.Cantidad },
                            { "Unit Price", d => $"{d.Valor:C}" },
                            { "Total", d => $"{(d.Cantidad * d.Valor):C}" }
                        };
                        UIHelper.DrawTable(detalles, columnasDetalles, "Details to be deleted");
                    }
                    
                    // Mostrar información de la venta a eliminar
                    UIHelper.ShowTitle("Sale Information to Delete");
                    Console.WriteLine($"ID: {venta.Id}");
                    Console.WriteLine($"Employee: {GetEmployeeName(venta.TerceroEnId)}");
                    Console.WriteLine($"Client: {venta.TerceroCliId}");
                    Console.WriteLine($"Invoice: {venta.FactId}");
                    Console.WriteLine($"Date: {venta.Fecha.ToShortDateString()}");
                    Console.WriteLine($"Total: {GetSaleTotal(venta.Id)}");
                    
                    if (UIHelper.Confirmar("Are you ABSOLUTELY sure you want to delete this sale and all its details?"))
                    {
                        var strategy = _context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using (var transaction = await _context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    // Primero eliminamos los detalles
                                    if (detalles.Any())
                                    {
                                        _context.DetalleVentas.RemoveRange(detalles);
                                        await _context.SaveChangesAsync(); // Guardamos primero los cambios de los detalles
                                    }
                                    
                                    // Luego eliminamos la venta
                                    _context.Ventas.Remove(venta);
                                    await _context.SaveChangesAsync(); // Guardamos los cambios de la venta
                                    
                                    await transaction.CommitAsync(); // Confirmamos la transacción
                                    UIHelper.ShowSuccess("Sale and its details deleted successfully.");
                                    Console.WriteLine("\nPress any key to return to the sales menu...");
                                    Console.ReadKey();
                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync();
                                    throw new Exception("Error deleting sale and its details. Changes have been reverted.", ex);
                                }
                            }
                        });
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                        Console.WriteLine("\nPress any key to return to the sales menu...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    UIHelper.ShowError("Sale not found.");
                    Console.WriteLine("\nPress any key to return to the sales menu...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting sale", ex);
                Console.WriteLine("\nPress any key to return to the sales menu...");
                Console.ReadKey();
            }
        }
        
        // Helper methods for displaying related entities
        
        private async Task ShowAvailableEmployees()
        {
            UIHelper.ShowTitle("Available Employees");
            var empleados = await _context.Empleados.ToListAsync();
            
            if (!empleados.Any())
            {
                UIHelper.ShowWarning("No employees are registered.");
                Console.ReadKey();
                return;
            }
            
            var empleadosData = new List<(int Id, string Nombre, string TerceroId)>();
            
            foreach (var empleado in empleados)
            {
                var tercero = await _context.Terceros.FindAsync(empleado.TerceroId);
                if (tercero != null)
                {
                    empleadosData.Add((empleado.Id, $"{tercero.Nombre} {tercero.Apellidos}", empleado.TerceroId));
                }
            }
            
            Console.WriteLine("┌───────┬────────────────────────────────┬──────────────┐");
            Console.WriteLine("│   ID  │ Name                           │ ID Tercero   │");
            Console.WriteLine("├───────┼────────────────────────────────┼──────────────┤");
            
            foreach (var emp in empleadosData)
            {
                Console.WriteLine($"│ {emp.Id.ToString().PadRight(5)} │ {emp.Nombre.PadRight(30)} │ {emp.TerceroId.PadRight(12)} │");
            }
            
            Console.WriteLine("└───────┴────────────────────────────────┴──────────────┘");
            Console.WriteLine();
        }
        
        private async Task ShowAvailableClients()
        {
            UIHelper.ShowTitle("Available Clients");
            var clientes = await _context.Clientes.ToListAsync();
            
            if (!clientes.Any())
            {
                UIHelper.ShowWarning("No clients are registered.");
                
                // Mostrar todos los terceros como alternativa
                var terceros = await _context.Terceros.ToListAsync();
                
                var columnasTerceros = new Dictionary<string, Func<Terceros, object>>
                {
                    { "ID", t => t.Id },
                    { "Name", t => t.Nombre },
                    { "Last Name", t => t.Apellidos }
                };
                
                UIHelper.DrawTable(terceros, columnasTerceros, "List of Available Third Parties");
                return;
            }
            
            var clientesData = new List<(int Id, string Nombre, string TerceroId)>();
            
            foreach (var cliente in clientes)
            {
                var tercero = await _context.Terceros.FindAsync(cliente.TerceroId);
                if (tercero != null)
                {
                    clientesData.Add((cliente.Id, $"{tercero.Nombre} {tercero.Apellidos}", cliente.TerceroId));
                }
            }
            
            Console.WriteLine("┌───────┬────────────────────────────────┬──────────────┐");
            Console.WriteLine("│   ID  │ Name                           │ ID Tercero   │");
            Console.WriteLine("├───────┼────────────────────────────────┼──────────────┤");
            
            foreach (var cli in clientesData)
            {
                Console.WriteLine($"│ {cli.Id.ToString().PadRight(5)} │ {cli.Nombre.PadRight(30)} │ {cli.TerceroId.PadRight(12)} │");
            }
            
            Console.WriteLine("└───────┴────────────────────────────────┴──────────────┘");
            Console.WriteLine();
        }
    }
}