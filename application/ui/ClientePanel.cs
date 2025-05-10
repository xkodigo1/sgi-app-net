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
                UIHelper.MostrarTitulo("Panel de Clientes");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Clientes" },
                    { "2", "Crear Nuevo Cliente" },
                    { "3", "Editar Cliente" },
                    { "4", "Eliminar Cliente" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

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
                    case "0":
                        return;
                    default:
                        UIHelper.MostrarAdvertencia("Opción no válida. Intente de nuevo.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ListarClientes()
        {
            UIHelper.MostrarTitulo("Listado de Clientes");
            
            try
            {
                var clientes = _context.Clientes.ToList();
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<Cliente, object>>
                {
                    { "ID", c => c.Id },
                    { "Tercero ID", c => c.TerceroId },
                    { "Fecha Nacimiento", c => c.FechaNac.ToShortDateString() },
                    { "Fecha Compra", c => c.FechaCompra.ToShortDateString() }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(clientes, columnas, "Clientes Registrados");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar los clientes", ex);
            }
        }

        private void CrearCliente()
        {
            UIHelper.MostrarTitulo("Crear Nuevo Cliente");
            
            try
            {
                // Verificar si ya existe un cliente con este tercero
                var terceroId = UIHelper.SolicitarEntrada("Ingrese el ID del tercero");
                
                // Verificar que el tercero exista
                var tercero = _context.Terceros.Find(terceroId);
                if (tercero == null)
                {
                    UIHelper.MostrarError($"El tercero con ID {terceroId} no existe. Debe crear el tercero primero.");
                    return;
                }
                
                // Verificar que no exista ya un cliente con este tercero
                var clienteExistente = _context.Clientes.FirstOrDefault(c => c.TerceroId == terceroId);
                if (clienteExistente != null)
                {
                    UIHelper.MostrarError($"Ya existe un cliente asociado al tercero con ID {terceroId}.");
                    return;
                }
                
                var fechaNacStr = UIHelper.SolicitarEntrada("Ingrese la fecha de nacimiento (YYYY-MM-DD)");
                var fechaNac = DateTime.Parse(fechaNacStr);
                
                var fechaCompraStr = UIHelper.SolicitarEntrada("Ingrese la fecha de compra (YYYY-MM-DD)");
                var fechaCompra = DateTime.Parse(fechaCompraStr);

                var cliente = new Cliente { TerceroId = terceroId, FechaNac = fechaNac, FechaCompra = fechaCompra };
                
                if (UIHelper.Confirmar("¿Desea guardar este cliente?"))
                {
                    _context.Clientes.Add(cliente);
                    _context.SaveChanges();
                    UIHelper.MostrarExito("Cliente creado exitosamente.");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear el cliente", ex);
            }
        }

        private void EditarCliente()
        {
            UIHelper.MostrarTitulo("Editar Cliente");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del cliente a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var cliente = _context.Clientes.Find(id);

                if (cliente != null)
                {
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {cliente.Id}");
                    Console.WriteLine($"Tercero ID: {cliente.TerceroId}");
                    Console.WriteLine($"Fecha de Nacimiento: {cliente.FechaNac.ToShortDateString()}");
                    Console.WriteLine($"Fecha de Compra: {cliente.FechaCompra.ToShortDateString()}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    var terceroId = UIHelper.SolicitarEntrada("Nuevo ID del tercero", cliente.TerceroId);
                    
                    // Verificar que el tercero exista
                    var tercero = _context.Terceros.Find(terceroId);
                    if (tercero == null)
                    {
                        UIHelper.MostrarError($"El tercero con ID {terceroId} no existe. Debe crear el tercero primero.");
                        return;
                    }
                    
                    // Si el tercero es diferente, verificar que no esté asociado a otro cliente
                    if (terceroId != cliente.TerceroId)
                    {
                        var clienteExistente = _context.Clientes.FirstOrDefault(c => c.TerceroId == terceroId && c.Id != id);
                        if (clienteExistente != null)
                        {
                            UIHelper.MostrarError($"El tercero con ID {terceroId} ya está asociado a otro cliente.");
                            return;
                        }
                    }
                    
                    cliente.TerceroId = terceroId;
                    
                    var fechaNacStr = UIHelper.SolicitarEntrada("Nueva fecha de nacimiento (YYYY-MM-DD)", cliente.FechaNac.ToString("yyyy-MM-dd"));
                    cliente.FechaNac = DateTime.Parse(fechaNacStr);
                    
                    var fechaCompraStr = UIHelper.SolicitarEntrada("Nueva fecha de compra (YYYY-MM-DD)", cliente.FechaCompra.ToString("yyyy-MM-dd"));
                    cliente.FechaCompra = DateTime.Parse(fechaCompraStr);

                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(cliente);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Cliente actualizado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Cliente no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar el cliente", ex);
            }
        }

        private void EliminarCliente()
        {
            UIHelper.MostrarTitulo("Eliminar Cliente");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del cliente a eliminar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var cliente = _context.Clientes.Find(id);

                if (cliente != null)
                {
                    // Mostrar información a eliminar
                    UIHelper.MostrarTitulo("Información del Cliente a Eliminar");
                    Console.WriteLine($"ID: {cliente.Id}");
                    Console.WriteLine($"Tercero ID: {cliente.TerceroId}");
                    Console.WriteLine($"Fecha de Nacimiento: {cliente.FechaNac.ToShortDateString()}");
                    Console.WriteLine($"Fecha de Compra: {cliente.FechaCompra.ToShortDateString()}");
                    
                    if (UIHelper.Confirmar("¿Está seguro que desea eliminar este cliente?"))
                    {
                        _context.Clientes.Remove(cliente);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Cliente eliminado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Cliente no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar el cliente", ex);
            }
        }
    }
}