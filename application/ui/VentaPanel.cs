using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Panel de Ventas ===");
                Console.WriteLine("1. Listar Ventas");
                Console.WriteLine("2. Crear Venta");
                Console.WriteLine("3. Editar Venta");
                Console.WriteLine("4. Eliminar Venta");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarVentas();
                        break;
                    case "2":
                        CrearVenta();
                        break;
                    case "3":
                        EditarVenta();
                        break;
                    case "4":
                        EliminarVenta();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private void ListarVentas()
        {
            var ventas = _context.Ventas.ToList();
            foreach (var venta in ventas)
            {
                Console.WriteLine($"ID: {venta.Id}, TerceroEnId: {venta.TerceroEnId}, TerceroCliId: {venta.TerceroCliId}, Fecha: {venta.Fecha}");
            }
        }

        private void CrearVenta()
        {
            Console.Write("Ingrese el ID de la venta: ");
            var id = int.Parse(Console.ReadLine());
            Console.Write("Ingrese el ID del tercero vendedor: ");
            var terceroEnId = Console.ReadLine();
            Console.Write("Ingrese el ID del cliente: ");
            var terceroCliId = Console.ReadLine();
            Console.Write("Ingrese la fecha de la venta (YYYY-MM-DD): ");
            var fecha = DateTime.Parse(Console.ReadLine());

            var venta = new Venta { Id = id, TerceroEnId = terceroEnId, TerceroCliId = terceroCliId, Fecha = fecha };
            _context.Ventas.Add(venta);
            _context.SaveChanges();

            Console.WriteLine("Venta creada exitosamente.");
        }

        private void EditarVenta()
        {
            Console.Write("Ingrese el ID de la venta a editar: ");
            var id = Console.ReadLine();
            var venta = _context.Ventas.Find(id);

            if (venta != null)
            {
                Console.Write("Ingrese el nuevo ID del tercero vendedor: ");
                venta.TerceroEnId = Console.ReadLine();
                Console.Write("Ingrese el nuevo ID del cliente: ");
                venta.TerceroCliId = Console.ReadLine();
                Console.Write("Ingrese la nueva fecha de la venta (YYYY-MM-DD): ");
                venta.Fecha = DateTime.Parse(Console.ReadLine());

                _context.Update(venta);
                _context.SaveChanges();

                Console.WriteLine("Venta actualizada exitosamente.");
            }
            else
            {
                Console.WriteLine("Venta no encontrada.");
            }
        }

        private void EliminarVenta()
        {
            Console.Write("Ingrese el ID de la venta a eliminar: ");
            var id = Console.ReadLine();
            var venta = _context.Ventas.Find(id);

            if (venta != null)
            {
                _context.Ventas.Remove(venta);
                _context.SaveChanges();

                Console.WriteLine("Venta eliminada exitosamente.");
            }
            else
            {
                Console.WriteLine("Venta no encontrada.");
            }
        }
    }
}