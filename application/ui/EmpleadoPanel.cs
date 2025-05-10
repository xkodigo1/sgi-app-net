using System;
using System.Collections.Generic;
using System.Linq;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class EmpleadoPanel
    {
        private readonly YourDbContext _context;

        public EmpleadoPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("=== Panel de Empleados ===");
                Console.WriteLine("1. Listar Empleados");
                Console.WriteLine("2. Crear Empleado");
                Console.WriteLine("3. Editar Empleado");
                Console.WriteLine("4. Eliminar Empleado");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarEmpleados();
                        break;
                    case "2":
                        CrearEmpleado();
                        break;
                    case "3":
                        EditarEmpleado();
                        break;
                    case "4":
                        EliminarEmpleado();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private void ListarEmpleados()
        {
            var empleados = _context.Empleados.ToList();
            foreach (var empleado in empleados)
            {
                Console.WriteLine($"ID: {empleado.Id}, TerceroId: {empleado.TerceroId}, FechaIngreso: {empleado.FechaIngreso}, SalarioBase: {empleado.SalarioBase}");
            }
        }

        private void CrearEmpleado()
        {
            Console.Write("Ingrese el ID del tercero: ");
            var terceroId = Console.ReadLine();
            Console.Write("Ingrese la fecha de ingreso (YYYY-MM-DD): ");
            var fechaIngreso = DateTime.Parse(Console.ReadLine());
            Console.Write("Ingrese el salario base: ");
            var salarioBase = double.Parse(Console.ReadLine());
            Console.Write("Ingrese el ID de la EPS: ");
            var epsId = int.Parse(Console.ReadLine());
            Console.Write("Ingrese el ID de la ARL: ");
            var arlId = int.Parse(Console.ReadLine());

            var empleado = new Empleado { TerceroId = terceroId, FechaIngreso = fechaIngreso, SalarioBase = salarioBase, EpsId = epsId, ArlId = arlId };
            _context.Empleados.Add(empleado);
            _context.SaveChanges();

            Console.WriteLine("Empleado creado exitosamente.");
        }

        private void EditarEmpleado()
        {
            Console.Write("Ingrese el ID del empleado a editar: ");
            var id = int.Parse(Console.ReadLine());
            var empleado = _context.Empleados.Find(id);

            if (empleado != null)
            {
                Console.Write("Ingrese el nuevo ID del tercero: ");
                empleado.TerceroId = Console.ReadLine();
                Console.Write("Ingrese la nueva fecha de ingreso (YYYY-MM-DD): ");
                empleado.FechaIngreso = DateTime.Parse(Console.ReadLine());
                Console.Write("Ingrese el nuevo salario base: ");
                empleado.SalarioBase = double.Parse(Console.ReadLine());
                Console.Write("Ingrese el nuevo ID de la EPS: ");
                empleado.EpsId = int.Parse(Console.ReadLine());
                Console.Write("Ingrese el nuevo ID de la ARL: ");
                empleado.ArlId = int.Parse(Console.ReadLine());

                _context.Update(empleado);
                _context.SaveChanges();

                Console.WriteLine("Empleado actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("Empleado no encontrado.");
            }
        }

        private void EliminarEmpleado()
        {
            Console.Write("Ingrese el ID del empleado a eliminar: ");
            var id = int.Parse(Console.ReadLine());
            var empleado = _context.Empleados.Find(id);

            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
                _context.SaveChanges();

                Console.WriteLine("Empleado eliminado exitosamente.");
            }
            else
            {
                Console.WriteLine("Empleado no encontrado.");
            }
        }
    }
}