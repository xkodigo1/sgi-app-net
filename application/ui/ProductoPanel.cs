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

        private void MostrarListaProductos(string titulo = "Productos Disponibles")
        {
            try
            {
                var productos = _context.Set<Producto>().ToList();
                
                if (!productos.Any())
                {
                    UIHelper.MostrarAdvertencia("No hay productos registrados.");
                    Console.ReadKey();
                    return;
                }
                
                var columnas = new Dictionary<string, Func<Producto, object>>
                {
                    { "ID", p => p.Id },
                    { "Nombre", p => p.Nombre },
                    { "Stock", p => p.Stock },
                    { "Precio", p => p.Precio.ToString("C2") }
                };
                UIHelper.DibujarTabla(productos, columnas, titulo);
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al mostrar la lista de productos", ex);
            }
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.MostrarTitulo("Panel de Productos");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Productos" },
                    { "2", "Crear Nuevo Producto" },
                    { "3", "Editar Producto" },
                    { "4", "Eliminar Producto" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarProductos();
                        break;
                    case "2":
                        CrearProducto();
                        break;
                    case "3":
                        EditarProducto();
                        break;
                    case "4":
                        EliminarProducto();
                        break;
                    case "0":
                        return;
                    default:
                        UIHelper.MostrarAdvertencia("Opción no válida. Intente de nuevo.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ListarProductos()
        {
            UIHelper.MostrarTitulo("Listado de Productos");
            
            try
            {
                var productos = _context.Set<Producto>().ToList();
                
                if (!productos.Any())
                {
                    UIHelper.MostrarAdvertencia("No hay productos registrados.");
                    Console.ReadKey();
                    return;
                }
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<Producto, object>>
                {
                    { "ID", p => p.Id },
                    { "Nombre", p => p.Nombre },
                    { "Stock", p => p.Stock },
                    { "Stock Mín", p => p.StockMin },
                    { "Stock Máx", p => p.StockMax },
                    { "Precio", p => p.Precio.ToString("C2") },
                    { "Estado", p => ObtenerEstadoStock(p) }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(productos, columnas, "Inventario de Productos");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar los productos", ex);
            }
        }
        
        // Método auxiliar para determinar el estado del stock
        private string ObtenerEstadoStock(Producto producto)
        {
            if (producto.Stock <= producto.StockMin)
                return "⚠️ Bajo mínimo";
            else if (producto.Stock >= producto.StockMax)
                return "⚠️ Sobre máximo";
            else
                return "✓ Normal";
        }

        private void CrearProducto()
        {
            UIHelper.MostrarTitulo("Crear Nuevo Producto");
            
            try
            {
                var id = UIHelper.SolicitarEntrada("Ingrese el ID del producto (ejemplo: P003)");
                
                // Verificar que no exista ya un producto con este ID
                var productoExistente = _context.Productos.Find(id);
                if (productoExistente != null)
                {
                    UIHelper.MostrarError($"Ya existe un producto con el ID {id}.");
                    return;
                }
                
                var nombre = UIHelper.SolicitarEntrada("Ingrese el nombre del producto");
                var stockStr = UIHelper.SolicitarEntrada("Ingrese el stock inicial");
                var stockMinStr = UIHelper.SolicitarEntrada("Ingrese el stock mínimo");
                var stockMaxStr = UIHelper.SolicitarEntrada("Ingrese el stock máximo");
                var precioStr = UIHelper.SolicitarEntrada("Ingrese el precio del producto");
                
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
                
                // Mostrar resumen antes de confirmar
                UIHelper.MostrarTitulo("Resumen del Producto");
                Console.WriteLine($"ID: {producto.Id}");
                Console.WriteLine($"Nombre: {producto.Nombre}");
                Console.WriteLine($"Stock: {producto.Stock}");
                Console.WriteLine($"Stock Min: {producto.StockMin}");
                Console.WriteLine($"Stock Max: {producto.StockMax}");
                Console.WriteLine($"Precio: {producto.Precio:C2}");
                
                if (UIHelper.Confirmar("¿Desea guardar este producto?"))
                {
                    _context.Productos.Add(producto);
                    _context.SaveChanges();
                    UIHelper.MostrarExito("Producto creado exitosamente.");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear el producto", ex);
            }
        }

        private void EditarProducto()
        {
            UIHelper.MostrarTitulo("Editar Producto");
            
            try
            {
                // Mostrar lista de productos disponibles
                MostrarListaProductos("Productos Disponibles para Editar");
                Console.WriteLine("\nSeleccione el ID del producto que desea editar:");
                
                var id = UIHelper.SolicitarEntrada("ID del producto");
                var producto = _context.Productos.Find(id);

                if (producto != null)
                {
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {producto.Id}");
                    Console.WriteLine($"Nombre: {producto.Nombre}");
                    Console.WriteLine($"Stock: {producto.Stock}");
                    Console.WriteLine($"Stock Min: {producto.StockMin}");
                    Console.WriteLine($"Stock Max: {producto.StockMax}");
                    Console.WriteLine($"Precio: {producto.Precio:C2}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    var nombre = UIHelper.SolicitarEntrada("Nuevo nombre", producto.Nombre);
                    var stockStr = UIHelper.SolicitarEntrada("Nuevo stock", producto.Stock.ToString());
                    var stockMinStr = UIHelper.SolicitarEntrada("Nuevo stock mínimo", producto.StockMin.ToString());
                    var stockMaxStr = UIHelper.SolicitarEntrada("Nuevo stock máximo", producto.StockMax.ToString());
                    var precioStr = UIHelper.SolicitarEntrada("Nuevo precio", producto.Precio.ToString());
                    
                    producto.Nombre = nombre;
                    producto.Stock = int.Parse(stockStr);
                    producto.StockMin = int.Parse(stockMinStr);
                    producto.StockMax = int.Parse(stockMaxStr);
                    producto.Precio = decimal.Parse(precioStr);
                    producto.UpdatedAt = DateTime.Now;

                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(producto);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Producto actualizado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Producto no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar el producto", ex);
            }
        }

        private void EliminarProducto()
        {
            UIHelper.MostrarTitulo("Eliminar Producto");
            
            try
            {
                // Mostrar lista de productos disponibles
                MostrarListaProductos("Productos Disponibles para Eliminar");
                Console.WriteLine("\nSeleccione el ID del producto que desea eliminar:");
                
                var id = UIHelper.SolicitarEntrada("ID del producto");
                var producto = _context.Productos.Find(id);

                if (producto != null)
                {
                    // Mostrar información a eliminar
                    UIHelper.MostrarTitulo("Información del Producto a Eliminar");
                    Console.WriteLine($"ID: {producto.Id}");
                    Console.WriteLine($"Nombre: {producto.Nombre}");
                    Console.WriteLine($"Stock: {producto.Stock}");
                    Console.WriteLine($"Stock Min: {producto.StockMin}");
                    Console.WriteLine($"Stock Max: {producto.StockMax}");
                    Console.WriteLine($"Precio: {producto.Precio:C2}");
                    
                    if (UIHelper.Confirmar("¿Está seguro que desea eliminar este producto?"))
                    {
                        _context.Productos.Remove(producto);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Producto eliminado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Producto no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar el producto", ex);
            }
        }
    }
}