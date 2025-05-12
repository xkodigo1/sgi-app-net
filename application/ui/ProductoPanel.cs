using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class ProductoPanel
    {
        private readonly YourDbContext _context;

        public ProductoPanel(YourDbContext context)
        {
            _context = context;
        }

        private void ShowProductList(string title = "Available Products")
        {
            try
            {
                var productos = _context.Set<Producto>().ToList();
                
                if (!productos.Any())
                {
                    UIHelper.ShowWarning("No products are registered.");
                    Console.ReadKey();
                    return;
                }
                
                var columns = new Dictionary<string, Func<Producto, object>>
                {
                    { "ID", p => p.Id },
                    { "Name", p => p.Nombre },
                    { "Stock", p => p.Stock },
                    { "Price", p => p.Precio.ToString("C2") }
                };
                UIHelper.DrawTable(productos, columns, title);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error displaying product list", ex);
            }
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.ShowTitle("Products Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Products" },
                    { "2", "Create New Product" },
                    { "3", "Edit Product" },
                    { "4", "Delete Product" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListProducts();
                        break;
                    case "2":
                        CreateProduct();
                        break;
                    case "3":
                        UpdateProduct();
                        break;
                    case "4":
                        DeleteProduct();
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

        private void ListProducts()
        {
            UIHelper.ShowTitle("Product List");
            
            try
            {
                var productos = _context.Set<Producto>().ToList();
                
                if (!productos.Any())
                {
                    UIHelper.ShowWarning("No products are registered.");
                    Console.ReadKey();
                    return;
                }
                
                // Define columns and values to display
                var columns = new Dictionary<string, Func<Producto, object>>
                {
                    { "ID", p => p.Id },
                    { "Name", p => p.Nombre },
                    { "Stock", p => p.Stock },
                    { "Min Stock", p => p.StockMin },
                    { "Max Stock", p => p.StockMax },
                    { "Price", p => p.Precio.ToString("C2") },
                    { "Status", p => GetStockStatus(p) }
                };
                
                // Use DrawTable method to show formatted data
                UIHelper.DrawTable(productos, columns, "Product Inventory");
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing products", ex);
            }
        }
        
        // Helper method to determine stock status
        private string GetStockStatus(Producto producto)
        {
            if (producto.Stock <= producto.StockMin)
                return "⚠️ Below minimum";
            else if (producto.Stock >= producto.StockMax)
                return "⚠️ Above maximum";
            else
                return "✓ Normal";
        }

        private void CreateProduct()
        {
            UIHelper.ShowTitle("Create New Product");
            
            try
            {
                var id = UIHelper.RequestInput("Enter product ID (example: P003)");
                
                // Verify that a product with this ID doesn't already exist
                var productoExistente = _context.Productos.Find(id);
                if (productoExistente != null)
                {
                    UIHelper.ShowError($"A product with ID {id} already exists.");
                    return;
                }
                
                var nombre = UIHelper.RequestInput("Enter product name");
                var stockStr = UIHelper.RequestInput("Enter initial stock");
                var stockMinStr = UIHelper.RequestInput("Enter minimum stock");
                var stockMaxStr = UIHelper.RequestInput("Enter maximum stock");
                var precioStr = UIHelper.RequestInput("Enter product price");
                
                var stock = int.Parse(stockStr);
                var stockMin = int.Parse(stockMinStr);
                var stockMax = int.Parse(stockMaxStr);
                var precio = decimal.Parse(precioStr);

                var producto = new Producto { 
                    Id = id, 
                    Nombre = nombre, 
                    Stock = stock, 
                    StockMin = stockMin, 
                    StockMax = stockMax,
                    Precio = precio,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                
                // Show summary before confirming
                UIHelper.ShowTitle("Product Summary");
                Console.WriteLine($"ID: {producto.Id}");
                Console.WriteLine($"Name: {producto.Nombre}");
                Console.WriteLine($"Stock: {producto.Stock}");
                Console.WriteLine($"Min Stock: {producto.StockMin}");
                Console.WriteLine($"Max Stock: {producto.StockMax}");
                Console.WriteLine($"Price: {producto.Precio:C2}");
                
                if (UIHelper.Confirm("Do you want to save this product?"))
                {
                    _context.Productos.Add(producto);
                    _context.SaveChanges();
                    UIHelper.ShowSuccess("Product created successfully.");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating product", ex);
            }
        }

        private void UpdateProduct()
        {
            UIHelper.ShowTitle("Edit Product");
            
            try
            {
                // Show list of available products
                ShowProductList("Products Available for Editing");
                Console.WriteLine("\nSelect the ID of the product you want to edit:");
                
                var id = UIHelper.RequestInput("Product ID");
                var producto = _context.Productos.Find(id);

                if (producto != null)
                {
                    // Show current information
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {producto.Id}");
                    Console.WriteLine($"Name: {producto.Nombre}");
                    Console.WriteLine($"Stock: {producto.Stock}");
                    Console.WriteLine($"Min Stock: {producto.StockMin}");
                    Console.WriteLine($"Max Stock: {producto.StockMax}");
                    Console.WriteLine($"Price: {producto.Precio:C2}");
                    Console.WriteLine("\nEnter new values or leave blank to keep current:");
                    
                    var nombre = UIHelper.RequestInput("New name", producto.Nombre);
                    var stockStr = UIHelper.RequestInput("New stock", producto.Stock.ToString());
                    var stockMinStr = UIHelper.RequestInput("New minimum stock", producto.StockMin.ToString());
                    var stockMaxStr = UIHelper.RequestInput("New maximum stock", producto.StockMax.ToString());
                    var precioStr = UIHelper.RequestInput("New price", producto.Precio.ToString());
                    
                    producto.Nombre = nombre;
                    producto.Stock = int.Parse(stockStr);
                    producto.StockMin = int.Parse(stockMinStr);
                    producto.StockMax = int.Parse(stockMaxStr);
                    producto.Precio = decimal.Parse(precioStr);
                    producto.UpdatedAt = DateTime.Now;

                    if (UIHelper.Confirm("Do you confirm these changes?"))
                    {
                        _context.Update(producto);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Product updated successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Product not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating product", ex);
            }
        }

        private void DeleteProduct()
        {
            UIHelper.ShowTitle("Delete Product");
            
            try
            {
                // Show list of available products
                ShowProductList("Products Available for Deletion");
                Console.WriteLine("\nSelect the ID of the product you want to delete:");
                
                var id = UIHelper.RequestInput("Product ID");
                var producto = _context.Productos.Find(id);

                if (producto != null)
                {
                    // Verify if there are associated purchase details
                    var detallesCompra = _context.DetalleCompras.Where(d => d.ProductoId == id).ToList();
                    
                    // Verify if there are associated sale details
                    var detallesVenta = _context.DetalleVentas.Where(d => d.ProductosId == producto.Id).ToList();
                    
                    if (detallesCompra.Any() || detallesVenta.Any())
                    {
                        UIHelper.ShowWarning($"This product has {detallesCompra.Count} purchase details and {detallesVenta.Count} sale details associated with it.");
                        UIHelper.ShowWarning("Cannot delete a product that has associated records.");
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        return;
                    }
                    
                    // Show information of the product to delete
                    UIHelper.ShowTitle("Product Information to Delete");
                    Console.WriteLine($"ID: {producto.Id}");
                    Console.WriteLine($"Name: {producto.Nombre}");
                    Console.WriteLine($"Stock: {producto.Stock}");
                    Console.WriteLine($"Min Stock: {producto.StockMin}");
                    Console.WriteLine($"Max Stock: {producto.StockMax}");
                    Console.WriteLine($"Price: {producto.Precio:C2}");
                    
                    if (UIHelper.Confirm("Are you ABSOLUTELY sure you want to delete this product?"))
                    {
                        _context.Productos.Remove(producto);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Product deleted successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Product not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting product", ex);
            }
        }
        
        // Maintain backward compatibility
        private void MostrarListaProductos(string titulo = "Productos Disponibles") => ShowProductList(titulo);
        private void ListarProductos() => ListProducts();
        private void CrearProducto() => CreateProduct();
        private void EditarProducto() => UpdateProduct();
        private void EliminarProducto() => DeleteProduct();
        private string ObtenerEstadoStock(Producto producto) => GetStockStatus(producto);
    }
}