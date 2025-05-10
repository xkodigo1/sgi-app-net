using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class MovimientoCajaPanel
    {
        private readonly YourDbContext _context;

        public MovimientoCajaPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Panel de Movimiento de Caja ===");
                Console.WriteLine("1. Listar Movimientos");
                Console.WriteLine("2. Crear Movimiento");
                Console.WriteLine("3. Editar Movimiento");
                Console.WriteLine("4. Eliminar Movimiento");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarMovimientos();
                        break;
                    case "2":
                        CrearMovimiento();
                        break;
                    case "3":
                        EditarMovimiento();
                        break;
                    case "4":
                        EliminarMovimiento();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private void ListarMovimientos()
        {
            var movimientos = _context.MovCaja.ToList();
            foreach (var movimiento in movimientos)
            {
                Console.WriteLine($"ID: {movimiento.Id}, Concepto: {movimiento.Concepto}, TerceroId: {movimiento.TerceroId}");
            }
        }

        private void CrearMovimiento()
        {
            Console.Write("Ingrese el ID del movimiento: ");
            var id = int.Parse(Console.ReadLine());
            Console.Write("Ingrese el concepto: ");
            var concepto = Console.ReadLine();
            Console.Write("Ingrese el ID del tercero: ");
            var terceroId = Console.ReadLine();

            var movimiento = new MovCaja { Id = id, Concepto = concepto, TerceroId = terceroId };
            _context.MovCaja.Add(movimiento);
            _context.SaveChanges();

            Console.WriteLine("Movimiento creado exitosamente.");
        }

        private void EditarMovimiento()
        {
            Console.Write("Ingrese el ID del movimiento a editar: ");
            var id = Console.ReadLine();
            var movimiento = _context.MovCaja.Find(id);

            if (movimiento != null)
            {
                Console.Write("Ingrese el nuevo concepto: ");
                movimiento.Concepto = Console.ReadLine();
                Console.Write("Ingrese el nuevo ID del tercero: ");
                movimiento.TerceroId = Console.ReadLine();

                _context.Update(movimiento);
                _context.SaveChanges();

                Console.WriteLine("Movimiento actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("Movimiento no encontrado.");
            }
        }

        private void EliminarMovimiento()
        {
            Console.Write("Ingrese el ID del movimiento a eliminar: ");
            var id = Console.ReadLine();
            var movimiento = _context.MovCaja.Find(id);

            if (movimiento != null)
            {
                _context.MovCaja.Remove(movimiento);
                _context.SaveChanges();

                Console.WriteLine("Movimiento eliminado exitosamente.");
            }
            else
            {
                Console.WriteLine("Movimiento no encontrado.");
            }
        }
    }
}