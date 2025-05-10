using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Panel de Compras ===");
                Console.WriteLine("1. Listar Compras");
                Console.WriteLine("2. Crear Compra");
                Console.WriteLine("3. Editar Compra");
                Console.WriteLine("4. Eliminar Compra");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarCompras();
                        break;
                    case "2":
                        CrearCompra();
                        break;
                    case "3":
                        EditarCompra();
                        break;
                    case "4":
                        EliminarCompra();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private void ListarCompras()
        {
            var compras = _context.Compras.ToList();
            foreach (var compra in compras)
            {
                Console.WriteLine($"ID: {compra.Id}, TerceroProvId: {compra.TerceroProvId}, Fecha: {compra.Fecha}, DocCompra: {compra.DocCompra}");
            }
        }

        private void CrearCompra()
        {
            Console.Write("Ingrese el ID del proveedor: ");
            var terceroProvId = Console.ReadLine();
            Console.Write("Ingrese la fecha de la compra (YYYY-MM-DD): ");
            var fecha = DateTime.Parse(Console.ReadLine());
            Console.Write("Ingrese el ID del empleado: ");
            var terceroEmpId = Console.ReadLine();
            Console.Write("Ingrese el documento de compra: ");
            var docCompra = Console.ReadLine();

            var compra = new Compra { TerceroProvId = terceroProvId, Fecha = fecha, TerceroEmpId = terceroEmpId, DocCompra = docCompra };
            _context.Compras.Add(compra);
            _context.SaveChanges();

            Console.WriteLine("Compra creada exitosamente.");
        }

        private void EditarCompra()
        {
            Console.Write("Ingrese el ID de la compra a editar: ");
            var id = int.Parse(Console.ReadLine());
            var compra = _context.Compras.Find(id);

            if (compra != null)
            {
                Console.Write("Ingrese el nuevo ID del proveedor: ");
                compra.TerceroProvId = Console.ReadLine();
                Console.Write("Ingrese la nueva fecha de la compra (YYYY-MM-DD): ");
                compra.Fecha = DateTime.Parse(Console.ReadLine());
                Console.Write("Ingrese el nuevo ID del empleado: ");
                compra.TerceroEmpId = Console.ReadLine();
                Console.Write("Ingrese el nuevo documento de compra: ");
                compra.DocCompra = Console.ReadLine();

                _context.Update(compra);
                _context.SaveChanges();

                Console.WriteLine("Compra actualizada exitosamente.");
            }
            else
            {
                Console.WriteLine("Compra no encontrada.");
            }
        }

        private void EliminarCompra()
        {
            Console.Write("Ingrese el ID de la compra a eliminar: ");
            var id = int.Parse(Console.ReadLine());
            var compra = _context.Compras.Find(id);

            if (compra != null)
            {
                _context.Compras.Remove(compra);
                _context.SaveChanges();

                Console.WriteLine("Compra eliminada exitosamente.");
            }
            else
            {
                Console.WriteLine("Compra no encontrada.");
            }
        }
    }
}