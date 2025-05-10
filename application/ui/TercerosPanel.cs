using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class TercerosPanel
    {
        private readonly YourDbContext _context;

        public TercerosPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Panel de Terceros ===");
                Console.WriteLine("1. Listar Terceros");
                Console.WriteLine("2. Crear Tercero");
                Console.WriteLine("3. Editar Tercero");
                Console.WriteLine("4. Eliminar Tercero");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarTerceros();
                        break;
                    case "2":
                        CrearTercero();
                        break;
                    case "3":
                        EditarTercero();
                        break;
                    case "4":
                        EliminarTercero();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private void ListarTerceros()
        {
            var terceros = _context.Terceros.ToList();
            foreach (var tercero in terceros)
            {
                Console.WriteLine($"ID: {tercero.Id}, Nombre: {tercero.Nombre}, Apellidos: {tercero.Apellidos}, Email: {tercero.Email}");
            }
        }

        private void CrearTercero()
        {
            Console.Write("Ingrese el ID del tercero: ");
            var id = Console.ReadLine();
            Console.Write("Ingrese el nombre: ");
            var nombre = Console.ReadLine();
            Console.Write("Ingrese los apellidos: ");
            var apellidos = Console.ReadLine();
            Console.Write("Ingrese el email: ");
            var email = Console.ReadLine();

            var tercero = new Terceros { Id = id, Nombre = nombre, Apellidos = apellidos, Email = email };
            _context.Terceros.Add(tercero);
            _context.SaveChanges();

            Console.WriteLine("Tercero creado exitosamente.");
        }

        private void EditarTercero()
        {
            Console.Write("Ingrese el ID del tercero a editar: ");
            var id = Console.ReadLine();
            var tercero = _context.Terceros.Find(id);

            if (tercero != null)
            {
                Console.Write("Ingrese el nuevo nombre: ");
                tercero.Nombre = Console.ReadLine();
                Console.Write("Ingrese los nuevos apellidos: ");
                tercero.Apellidos = Console.ReadLine();
                Console.Write("Ingrese el nuevo email: ");
                tercero.Email = Console.ReadLine();

                _context.Update(tercero);
                _context.SaveChanges();

                Console.WriteLine("Tercero actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("Tercero no encontrado.");
            }
        }

        private void EliminarTercero()
        {
            Console.Write("Ingrese el ID del tercero a eliminar: ");
            var id = Console.ReadLine();
            var tercero = _context.Terceros.Find(id);

            if (tercero != null)
            {
                _context.Terceros.Remove(tercero);
                _context.SaveChanges();

                Console.WriteLine("Tercero eliminado exitosamente.");
            }
            else
            {
                Console.WriteLine("Tercero no encontrado.");
            }
        }
    }
}