using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class ProveedorPanel
    {
        private readonly YourDbContext _context;

        public ProveedorPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Panel de Proveedores ===");
                Console.WriteLine("1. Listar Proveedores");
                Console.WriteLine("2. Crear Proveedor");
                Console.WriteLine("3. Editar Proveedor");
                Console.WriteLine("4. Eliminar Proveedor");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarProveedores();
                        break;
                    case "2":
                        CrearProveedor();
                        break;
                    case "3":
                        EditarProveedor();
                        break;
                    case "4":
                        EliminarProveedor();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private void ListarProveedores()
        {
            var proveedores = _context.Proveedores.ToList();
            foreach (var proveedor in proveedores)
            {
                Console.WriteLine($"ID: {proveedor.Id}, TerceroId: {proveedor.TerceroId}, Dcto: {proveedor.Dcto}, DiaPago: {proveedor.DiaPago}");
            }
        }

        private void CrearProveedor()
        {
            Console.Write("Ingrese el ID del proveedor: ");
            var id = int.Parse(Console.ReadLine());
            Console.Write("Ingrese el ID del tercero: ");
            var terceroId = Console.ReadLine();
            Console.Write("Ingrese el descuento: ");
            var dcto = double.Parse(Console.ReadLine());
            Console.Write("Ingrese el día de pago: ");
            var diaPago = int.Parse(Console.ReadLine());

            var proveedor = new Proveedor { Id = id, TerceroId = terceroId, Dcto = dcto, DiaPago = diaPago };
            _context.Proveedores.Add(proveedor);
            _context.SaveChanges();

            Console.WriteLine("Proveedor creado exitosamente.");
        }

        private void EditarProveedor()
        {
            Console.Write("Ingrese el ID del proveedor a editar: ");
            var id = Console.ReadLine();
            var proveedor = _context.Proveedores.Find(id);

            if (proveedor != null)
            {
                Console.Write("Ingrese el nuevo ID del tercero: ");
                proveedor.TerceroId = Console.ReadLine();
                Console.Write("Ingrese el nuevo descuento: ");
                proveedor.Dcto = double.Parse(Console.ReadLine());
                Console.Write("Ingrese el nuevo día de pago: ");
                proveedor.DiaPago = int.Parse(Console.ReadLine());

                _context.Update(proveedor);
                _context.SaveChanges();

                Console.WriteLine("Proveedor actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("Proveedor no encontrado.");
            }
        }

        private void EliminarProveedor()
        {
            Console.Write("Ingrese el ID del proveedor a eliminar: ");
            var id = Console.ReadLine();
            var proveedor = _context.Proveedores.Find(id);

            if (proveedor != null)
            {
                _context.Proveedores.Remove(proveedor);
                _context.SaveChanges();

                Console.WriteLine("Proveedor eliminado exitosamente.");
            }
            else
            {
                Console.WriteLine("Proveedor no encontrado.");
            }
        }
    }
}