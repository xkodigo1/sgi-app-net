using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class ClientePanel
    {
        private readonly YourDbContext _context;

        public ClientePanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Panel de Clientes ===");
                Console.WriteLine("1. Listar Clientes");
                Console.WriteLine("2. Crear Cliente");
                Console.WriteLine("3. Editar Cliente");
                Console.WriteLine("4. Eliminar Cliente");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarClientes();
                        break;
                    case "2":
                        CrearCliente();
                        break;
                    case "3":
                        EditarCliente();
                        break;
                    case "4":
                        EliminarCliente();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private void ListarClientes()
        {
            var clientes = _context.Clientes.ToList();
            foreach (var cliente in clientes)
            {
                Console.WriteLine($"ID: {cliente.Id}, Nombre: {cliente.Nombre}, Email: {cliente.Email}");
            }
        }

        private void CrearCliente()
        {
            Console.Write("Ingrese el nombre del cliente: ");
            var nombre = Console.ReadLine();
            Console.Write("Ingrese el email del cliente: ");
            var email = Console.ReadLine();

            var cliente = new Cliente { Nombre = nombre, Email = email };
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            Console.WriteLine("Cliente creado exitosamente.");
        }

        private void EditarCliente()
        {
            Console.Write("Ingrese el ID del cliente a editar: ");
            var id = int.Parse(Console.ReadLine());
            var cliente = _context.Clientes.Find(id);

            if (cliente != null)
            {
                Console.Write("Ingrese el nuevo nombre del cliente: ");
                cliente.Nombre = Console.ReadLine();
                Console.Write("Ingrese el nuevo email del cliente: ");
                cliente.Email = Console.ReadLine();

                _context.Update(cliente);
                _context.SaveChanges();

                Console.WriteLine("Cliente actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("Cliente no encontrado.");
            }
        }

        private void EliminarCliente()
        {
            Console.Write("Ingrese el ID del cliente a eliminar: ");
            var id = int.Parse(Console.ReadLine());
            var cliente = _context.Clientes.Find(id);

            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                _context.SaveChanges();

                Console.WriteLine("Cliente eliminado exitosamente.");
            }
            else
            {
                Console.WriteLine("Cliente no encontrado.");
            }
        }
    }
}