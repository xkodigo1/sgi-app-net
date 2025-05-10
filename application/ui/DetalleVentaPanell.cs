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
                Console.WriteLine("=== Panel de Detalle de Ventas ===");
                Console.WriteLine("1. Listar Detalles");
                Console.WriteLine("2. Crear Detalle");
                Console.WriteLine("3. Editar Detalle");
                Console.WriteLine("4. Eliminar Detalle");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarDetalles();
                        break;
                    case "2":
                        CrearDetalle();
                        break;
                    case "3":
                        EditarDetalle();
                        break;
                    case "4":
                        EliminarDetalle();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private void ListarDetalles()
        {
            var detalles = _context.DetalleVentas.ToList();
            foreach (var detalle in detalles)
            {
                Console.WriteLine($"ID: {detalle.Id}, ProductoId: {detalle.ProductosId}, VentaId: {detalle.VentaId}");
            }
        }

        private void CrearDetalle()
        {
            Console.Write("Ingrese el ID del detalle: ");
            var id = int.Parse(Console.ReadLine());
            Console.Write("Ingrese el ID del producto: ");
            var productoId = Console.ReadLine();
            Console.Write("Ingrese el ID de la venta: ");
            var ventaId = int.Parse(Console.ReadLine());

            var detalle = new DetalleVenta { Id = id, ProductosId = productoId, VentaId = ventaId };
            _context.DetalleVentas.Add(detalle);
            _context.SaveChanges();

            Console.WriteLine("Detalle creado exitosamente.");
        }

        private void EditarDetalle()
        {
            Console.Write("Ingrese el ID del detalle a editar: ");
            var id = Console.ReadLine();
            var detalle = _context.DetalleVentas.Find(id);

            if (detalle != null)
            {
                Console.Write("Ingrese el nuevo ID del producto: ");
                detalle.ProductosId = Console.ReadLine();
                Console.Write("Ingrese el nuevo ID de la venta: ");
                detalle.VentaId = int.Parse(Console.ReadLine());

                _context.Update(detalle);
                _context.SaveChanges();

                Console.WriteLine("Detalle actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("Detalle no encontrado.");
            }
        }

        private void EliminarDetalle()
        {
            Console.Write("Ingrese el ID del detalle a eliminar: ");
            var id = Console.ReadLine();
            var detalle = _context.DetalleVentas.Find(id);

            if (detalle != null)
            {
                _context.DetalleVentas.Remove(detalle);
                _context.SaveChanges();

                Console.WriteLine("Detalle eliminado exitosamente.");
            }
            else
            {
                Console.WriteLine("Detalle no encontrado.");
            }
        }
    }
}