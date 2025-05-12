using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class DetalleVentaPanel
    {
        private readonly YourDbContext _context;

        public DetalleVentaPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.ShowTitle("Sales Detail Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Sales Details" },
                    { "2", "Create New Detail" },
                    { "3", "Edit Existing Detail" },
                    { "4", "Delete Detail" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListDetails();
                        break;
                    case "2":
                        CreateDetail();
                        break;
                    case "3":
                        UpdateDetail();
                        break;
                    case "4":
                        DeleteDetail();
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

        private void ListDetails()
        {
            UIHelper.ShowTitle("Sales Detail List");
            
            try
            {
                var details = _context.DetalleVentas.ToList();
                
                // Define columns and values to display
                var columns = new Dictionary<string, Func<DetalleVenta, object>>
                {
                    { "ID", d => d.Id },
                    { "Product", d => d.ProductosId },
                    { "Sale", d => d.VentaId },
                    { "Quantity", d => d.Cantidad },
                    { "Unit Price", d => $"{d.Valor:C}" },
                    { "Total", d => $"{(d.Cantidad * d.Valor):C}" }
                };
                
                // Use DrawTable method to show formatted data
                UIHelper.DrawTable(details, columns, "Sales Details");
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing sales details", ex);
            }
        }

        private void CreateDetail()
        {
            UIHelper.ShowTitle("Create New Sales Detail");
            
            try
            {
                // Use RequestInput method for better UX
                var idStr = UIHelper.RequestInput("Enter the detail ID");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. The ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                
                // Check that no detail with this ID already exists
                var existingDetail = _context.DetalleVentas.Find(id);
                if (existingDetail != null)
                {
                    UIHelper.ShowError($"A sales detail with ID {id} already exists.");
                    return;
                }
                
                var productId = UIHelper.RequestInput("Enter the product ID");
                
                // Check that the product exists
                var product = _context.Productos.Find(productId);
                if (product == null)
                {
                    UIHelper.ShowError($"The product with ID {productId} does not exist. You must create the product first.");
                    return;
                }
                
                var saleIdStr = UIHelper.RequestInput("Enter the sale ID");
                var saleId = int.Parse(saleIdStr);
                
                // Check that the sale exists
                var sale = _context.Ventas.Find(saleId);
                if (sale == null)
                {
                    UIHelper.ShowError($"The sale with ID {saleId} does not exist. You must create the sale first.");
                    return;
                }
                
                // Check that no detail with the same combination of sale and product exists
                var duplicateDetail = _context.DetalleVentas.FirstOrDefault(d => d.VentaId == saleId && d.ProductosId == productId);
                if (duplicateDetail != null)
                {
                    UIHelper.ShowError($"A detail for sale {saleId} and product {productId} already exists.\nIf you want to modify the quantity, edit the detail with ID {duplicateDetail.Id}.");
                    return;
                }
                
                var quantityStr = UIHelper.RequestInput("Enter the quantity");
                var quantity = int.Parse(quantityStr);
                
                var valueStr = UIHelper.RequestInput("Enter the unit price");
                var value = decimal.Parse(valueStr);

                var detail = new DetalleVenta { 
                    Id = id, 
                    ProductosId = productId, 
                    VentaId = saleId,
                    Cantidad = quantity,
                    Valor = value
                };
                
                // Show summary before confirming
                UIHelper.ShowTitle("Detail Summary");
                Console.WriteLine($"ID: {detail.Id}");
                Console.WriteLine($"Product: {detail.ProductosId}");
                Console.WriteLine($"Sale: {detail.VentaId}");
                Console.WriteLine($"Quantity: {detail.Cantidad}");
                Console.WriteLine($"Unit price: {detail.Valor:C}");
                Console.WriteLine($"Total: {(detail.Cantidad * detail.Valor):C}");
                
                if (UIHelper.Confirm("Do you want to save this detail?"))
                {
                    _context.DetalleVentas.Add(detail);
                    _context.SaveChanges();

                    UIHelper.ShowSuccess("Detail created successfully.");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating detail", ex);
            }
        }

        private void UpdateDetail()
        {
            UIHelper.ShowTitle("Edit Sales Detail");
            
            try
            {
                var idStr = UIHelper.RequestInput("Enter the ID of the detail to edit");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. The ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var detail = _context.DetalleVentas.Find(id);

                if (detail != null)
                {
                    // Show current information
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {detail.Id}");
                    Console.WriteLine($"Product: {detail.ProductosId}");
                    Console.WriteLine($"Sale: {detail.VentaId}");
                    Console.WriteLine($"Quantity: {detail.Cantidad}");
                    Console.WriteLine($"Unit price: {detail.Valor:C}");
                    Console.WriteLine($"Total: {(detail.Cantidad * detail.Valor):C}");
                    Console.WriteLine("\nEnter new values or leave blank to keep current ones:");
                    
                    var productId = UIHelper.RequestInput("New product ID", detail.ProductosId);
                    
                    // Check that the product exists
                    var product = _context.Productos.Find(productId);
                    if (product == null)
                    {
                        UIHelper.ShowError($"The product with ID {productId} does not exist. You must create the product first.");
                        return;
                    }
                    
                    var saleIdStr = UIHelper.RequestInput("New sale ID", detail.VentaId.ToString());
                    var saleId = int.Parse(saleIdStr);
                    
                    // Check that the sale exists
                    var sale = _context.Ventas.Find(saleId);
                    if (sale == null)
                    {
                        UIHelper.ShowError($"The sale with ID {saleId} does not exist. You must create the sale first.");
                        return;
                    }
                    
                    // If they change the product or the sale, check that no detail with the same combination exists
                    if (productId != detail.ProductosId || saleId != detail.VentaId)
                    {
                        var duplicateDetail = _context.DetalleVentas.FirstOrDefault(d => 
                            d.VentaId == saleId && 
                            d.ProductosId == productId && 
                            d.Id != id);
                            
                        if (duplicateDetail != null)
                        {
                            UIHelper.ShowError($"A detail for sale {saleId} and product {productId} already exists.");
                            return;
                        }
                    }
                    
                    var quantityStr = UIHelper.RequestInput("New quantity", detail.Cantidad.ToString());
                    var quantity = int.Parse(quantityStr);
                    
                    var valueStr = UIHelper.RequestInput("New unit price", detail.Valor.ToString());
                    var value = decimal.Parse(valueStr);
                    
                    // Update the detail
                    detail.ProductosId = productId;
                    detail.VentaId = saleId;
                    detail.Cantidad = quantity;
                    detail.Valor = value;
                    
                    // Show summary before confirming
                    UIHelper.ShowTitle("Updated Detail Summary");
                    Console.WriteLine($"ID: {detail.Id}");
                    Console.WriteLine($"Product: {detail.ProductosId}");
                    Console.WriteLine($"Sale: {detail.VentaId}");
                    Console.WriteLine($"Quantity: {detail.Cantidad}");
                    Console.WriteLine($"Unit price: {detail.Valor:C}");
                    Console.WriteLine($"Total: {(detail.Cantidad * detail.Valor):C}");
                    
                    if (UIHelper.Confirm("Are you sure you want to apply these changes?"))
                    {
                        _context.Update(detail);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Detail updated successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Detail not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating detail", ex);
            }
        }

        private void DeleteDetail()
        {
            UIHelper.ShowTitle("Delete Sales Detail");
            
            try
            {
                var idStr = UIHelper.RequestInput("Enter the ID of the detail to delete");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. The ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var detail = _context.DetalleVentas.Find(id);

                if (detail != null)
                {
                    // Show information of the detail to delete
                    UIHelper.ShowTitle("Detail Information to Delete");
                    Console.WriteLine($"ID: {detail.Id}");
                    Console.WriteLine($"Product: {detail.ProductosId}");
                    Console.WriteLine($"Sale: {detail.VentaId}");
                    Console.WriteLine($"Quantity: {detail.Cantidad}");
                    Console.WriteLine($"Unit price: {detail.Valor:C}");
                    Console.WriteLine($"Total: {(detail.Cantidad * detail.Valor):C}");
                    
                    if (UIHelper.Confirm("Are you sure you want to delete this detail?"))
                    {
                        _context.DetalleVentas.Remove(detail);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Detail deleted successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Detail not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting detail", ex);
            }
        }
        
        // Maintain backward compatibility with Spanish methods
        private void ListarDetalles() => ListDetails();
        private void CrearDetalle() => CreateDetail();
        private void EditarDetalle() => UpdateDetail();
        private void EliminarDetalle() => DeleteDetail();
    }
}