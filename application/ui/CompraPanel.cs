using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class CompraPanel
    {
        private readonly YourDbContext _context;

        public CompraPanel(YourDbContext context)
        {
            _context = context;
        }

        public async Task ShowMenu()
        {
            while (true)
            {
                UIHelper.ShowTitle("Purchases Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Purchases" },
                    { "2", "Create New Purchase" },
                    { "3", "Edit Purchase" },
                    { "4", "Delete Purchase" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListPurchases();
                        break;
                    case "2":
                        CreatePurchase();
                        break;
                    case "3":
                        UpdatePurchase();
                        break;
                    case "4":
                        await DeletePurchaseAsync();
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

        private void ListPurchases()
        {
            UIHelper.ShowTitle("Purchases List");
            
            try
            {
                var compras = _context.Compras.ToList();
                
                // Define columns and values to display
                var columns = new Dictionary<string, Func<Compra, object>>
                {
                    { "ID", c => c.Id },
                    { "Supplier", c => c.TerceroProvId },
                    { "Employee", c => c.TerceroEmpId },
                    { "Document", c => c.DocCompra },
                    { "Date", c => c.Fecha.ToShortDateString() },
                    { "Total", c => GetPurchaseTotal(c.Id) }
                };
                
                // Use DrawTable method to show formatted data
                UIHelper.DrawTable(compras, columns, "Purchase Records");
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing purchases", ex);
            }
        }
        
        // Helper method to calculate purchase total
        private string GetPurchaseTotal(int compraId)
        {
            try
            {
                var detalles = _context.DetalleCompras.Where(d => d.CompraId == compraId).ToList();
                decimal total = detalles.Sum(d => d.Cantidad * d.Valor);
                return $"{total:C}";
            }
            catch
            {
                return "Not calculated";
            }
        }

        private void CreatePurchase()
        {
            UIHelper.ShowTitle("Create New Purchase");
            
            try
            {
                var proveedorId = UIHelper.RequestInput("Enter supplier ID");
                if (string.IsNullOrWhiteSpace(proveedorId))
                {
                    UIHelper.ShowWarning("Operation cancelled. Supplier ID is required.");
                    return;
                }
                
                // Verify supplier exists
                var proveedor = _context.Terceros.Find(proveedorId);
                if (proveedor == null)
                {
                    UIHelper.ShowError($"Third party with ID {proveedorId} does not exist. You must create the third party first.");
                    return;
                }
                
                var empleadoId = UIHelper.RequestInput("Enter employee ID");
                if (string.IsNullOrWhiteSpace(empleadoId))
                {
                    UIHelper.ShowWarning("Operation cancelled. Employee ID is required.");
                    return;
                }
                
                // Verify employee exists
                var empleado = _context.Terceros.Find(empleadoId);
                if (empleado == null)
                {
                    UIHelper.ShowError($"Third party with ID {empleadoId} does not exist. You must create the third party first.");
                    return;
                }
                
                var docCompra = UIHelper.RequestInput("Enter purchase document");
                if (string.IsNullOrWhiteSpace(docCompra))
                {
                    UIHelper.ShowWarning("Operation cancelled. Purchase document is required.");
                    return;
                }
                
                var fechaStr = UIHelper.RequestInput("Enter date (YYYY-MM-DD)", DateTime.Now.ToString("yyyy-MM-dd"));
                var fecha = DateTime.Parse(fechaStr);

                var compra = new Compra { 
                    TerceroProvId = proveedorId, 
                    TerceroEmpId = empleadoId, 
                    DocCompra = docCompra,
                    Fecha = fecha
                };
                
                // Show summary before confirming
                UIHelper.ShowTitle("Purchase Summary");
                Console.WriteLine($"Supplier: {compra.TerceroProvId}");
                Console.WriteLine($"Employee: {compra.TerceroEmpId}");
                Console.WriteLine($"Document: {compra.DocCompra}");
                Console.WriteLine($"Date: {compra.Fecha.ToShortDateString()}");
                
                if (UIHelper.Confirm("Do you want to save this purchase?"))
                {
                    _context.Compras.Add(compra);
                    _context.SaveChanges();
                    UIHelper.ShowSuccess($"Purchase created successfully with ID: {compra.Id}");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating purchase", ex);
            }
        }

        private void UpdatePurchase()
        {
            UIHelper.ShowTitle("Edit Purchase");
            
            try
            {
                var idStr = UIHelper.RequestInput("Enter ID of the purchase to edit");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var compra = _context.Compras.Find(id);

                if (compra != null)
                {
                    // Show current information
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {compra.Id}");
                    Console.WriteLine($"Supplier: {compra.TerceroProvId}");
                    Console.WriteLine($"Employee: {compra.TerceroEmpId}");
                    Console.WriteLine($"Document: {compra.DocCompra}");
                    Console.WriteLine($"Date: {compra.Fecha.ToShortDateString()}");
                    Console.WriteLine("\nEnter new values or leave blank to keep current:");
                    
                    var proveedorId = UIHelper.RequestInput("New supplier ID", compra.TerceroProvId);
                    
                    // Verify supplier exists
                    var proveedor = _context.Terceros.Find(proveedorId);
                    if (proveedor == null)
                    {
                        UIHelper.ShowError($"Third party with ID {proveedorId} does not exist. You must create the third party first.");
                        return;
                    }
                    
                    var empleadoId = UIHelper.RequestInput("New employee ID", compra.TerceroEmpId);
                    
                    // Verify employee exists
                    var empleado = _context.Terceros.Find(empleadoId);
                    if (empleado == null)
                    {
                        UIHelper.ShowError($"Third party with ID {empleadoId} does not exist. You must create the third party first.");
                        return;
                    }
                    
                    var docCompra = UIHelper.RequestInput("New purchase document", compra.DocCompra);
                    var fechaStr = UIHelper.RequestInput("New date (YYYY-MM-DD)", compra.Fecha.ToString("yyyy-MM-dd"));
                    
                    compra.TerceroProvId = proveedorId;
                    compra.TerceroEmpId = empleadoId;
                    compra.DocCompra = docCompra;
                    compra.Fecha = DateTime.Parse(fechaStr);

                    if (UIHelper.Confirm("Confirm these changes?"))
                    {
                        _context.Update(compra);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Purchase updated successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Purchase not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating purchase", ex);
            }
        }

        private async Task DeletePurchaseAsync()
        {
            UIHelper.ShowTitle("Delete Purchase");
            
            try
            {
                // Show list of available purchases
                ListPurchases();
                
                var idStr = UIHelper.RequestInput("Enter ID of the purchase to delete");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var compra = await _context.Compras.FindAsync(id);

                if (compra != null)
                {
                    // Check if there are associated details
                    var detalles = await _context.DetalleCompras.Where(d => d.CompraId == id).ToListAsync();
                    if (detalles.Any())
                    {
                        UIHelper.ShowWarning($"The purchase has {detalles.Count} associated details. These will also be deleted.");
                        
                        // Show the details to be deleted
                        var columnasDetalles = new Dictionary<string, Func<DetalleCompra, object>>
                        {
                            { "ID", d => d.Id },
                            { "Product", d => d.ProductoId },
                            { "Quantity", d => d.Cantidad },
                            { "Unit Price", d => $"{d.Valor:C}" },
                            { "Total", d => $"{(d.Cantidad * d.Valor):C}" }
                        };
                        UIHelper.DrawTable(detalles, columnasDetalles, "Details to be deleted");
                    }
                    
                    // Show information of the purchase to delete
                    UIHelper.ShowTitle("Purchase Information to Delete");
                    Console.WriteLine($"ID: {compra.Id}");
                    Console.WriteLine($"Supplier: {compra.TerceroProvId}");
                    Console.WriteLine($"Employee: {compra.TerceroEmpId}");
                    Console.WriteLine($"Document: {compra.DocCompra}");
                    Console.WriteLine($"Date: {compra.Fecha.ToShortDateString()}");
                    Console.WriteLine($"Total: {GetPurchaseTotal(compra.Id)}");
                    
                    if (UIHelper.Confirm("Are you ABSOLUTELY sure you want to delete this purchase and all its details?"))
                    {
                        var strategy = _context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using (var transaction = await _context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    // First delete the details
                                    if (detalles.Any())
                                    {
                                        _context.DetalleCompras.RemoveRange(detalles);
                                        await _context.SaveChangesAsync(); // Save changes to details first
                                    }
                                    
                                    // Then delete the purchase
                                    _context.Compras.Remove(compra);
                                    await _context.SaveChangesAsync(); // Save purchase changes
                                    
                                    await transaction.CommitAsync(); // Confirm transaction
                                    UIHelper.ShowSuccess("Purchase and its details deleted successfully.");
                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync();
                                    throw new Exception("Error deleting the purchase and its details. Changes have been reverted.", ex);
                                }
                            }
                        });
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Purchase not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting purchase", ex);
            }
        }
        
        // Maintain backward compatibility
        private void ListarCompras() => ListPurchases();
        private void CrearCompra() => CreatePurchase();
        private void EditarCompra() => UpdatePurchase();
        private Task EliminarCompraAsync() => DeletePurchaseAsync();
        private string ObtenerTotalCompra(int compraId) => GetPurchaseTotal(compraId);
    }
}