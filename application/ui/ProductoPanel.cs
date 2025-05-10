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

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Panel de Productos ===");
                Console.WriteLine("1. Listar Productos");
                Console.WriteLine("2. Crear Producto");
                Console.WriteLine("3. Editar Producto");
                Console.WriteLine("4. Eliminar Producto");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

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
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private void ListarProductos()
        {
            var productos = _context.Productos.ToList();
            foreach (var producto in productos)
            {
                Console.WriteLine($"ID: {producto.Id}, Nombre: {producto.Nombre}, Stock: {producto.Stock}, StockMin: {producto.StockMin}, StockMax: {producto.StockMax}");
            }
        }

        private void CrearProducto()
        {
            Console.Write("Ingrese el ID del producto: ");
            var id = Console.ReadLine();
            Console.Write("Ingrese el nombre del producto: ");
            var nombre = Console.ReadLine();
            Console.Write("Ingrese el stock: ");
            var stock = int.Parse(Console.ReadLine());
            Console.Write("Ingrese el stock mínimo: ");
            var stockMin = int.Parse(Console.ReadLine());
            Console.Write("Ingrese el stock máximo: ");
            var stockMax = int.Parse(Console.ReadLine());
            Console.Write("Ingrese el código de barras: ");
            var barcode = Console.ReadLine();

            var producto = new Producto { Id = id, Nombre = nombre, Stock = stock, StockMin = stockMin, StockMax = stockMax, Barcode = barcode };
            _context.Productos.Add(producto);
            _context.SaveChanges();

            Console.WriteLine("Producto creado exitosamente.");
        }

        private void EditarProducto()
        {
            Console.Write("Ingrese el ID del producto a editar: ");
            var id = Console.ReadLine();
            var producto = _context.Productos.Find(id);

            if (producto != null)
            {
                Console.Write("Ingrese el nuevo nombre del producto: ");
                producto.Nombre = Console.ReadLine();
                Console.Write("Ingrese el nuevo stock: ");
                producto.Stock = int.Parse(Console.ReadLine());
                Console.Write("Ingrese el nuevo stock mínimo: ");
                producto.StockMin = int.Parse(Console.ReadLine());
                Console.Write("Ingrese el nuevo stock máximo: ");
                producto.StockMax = int.Parse(Console.ReadLine());
                Console.Write("Ingrese el nuevo código de barras: ");
                producto.Barcode = Console.ReadLine();

                _context.Update(producto);
                _context.SaveChanges();

                Console.WriteLine("Producto actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("Producto no encontrado.");
            }
        }

        private void EliminarProducto()
        {
            Console.Write("Ingrese el ID del producto a eliminar: ");
            var id = Console.ReadLine();
            var producto = _context.Productos.Find(id);

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                _context.SaveChanges();

                Console.WriteLine("Producto eliminado exitosamente.");
            }
            else
            {
                Console.WriteLine("Producto no encontrado.");
            }
        }
    }
}