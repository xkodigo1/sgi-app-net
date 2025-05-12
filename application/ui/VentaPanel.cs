using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task ShowMenu()
        {
            while (true)
            {
                UIHelper.MostrarTitulo("Panel de Ventas");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Ventas" },
                    { "2", "Crear Nueva Venta" },
                    { "3", "Editar Venta" },
                    { "4", "Eliminar Venta" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarVentas();
                        break;
                    case "2":
                        await CrearVentaAsync();
                        break;
                    case "3":
                        await EditarVentaAsync();
                        break;
                    case "4":
                        await EliminarVentaAsync();
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

        private void ListarVentas()
        {
            UIHelper.MostrarTitulo("Listado de Ventas");
            
            try
            {
                var ventas = _context.Ventas.ToList();
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<Venta, object>>
                {
                    { "ID", v => v.Id },
                    { "Empleado", v => ObtenerNombreEmpleado(v.TerceroEnId) },
                    { "Cliente", v => v.TerceroCliId },
                    { "Factura", v => v.FactId },
                    { "Fecha", v => v.Fecha.ToShortDateString() },
                    { "Total", v => ObtenerTotalVenta(v.Id) }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(ventas, columnas, "Registro de Ventas");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar las ventas", ex);
            }
        }
        
        // Método auxiliar para obtener el nombre del empleado
        private string ObtenerNombreEmpleado(string terceroId)
        {
            try
            {
                var tercero = _context.Terceros.Find(terceroId);
                if (tercero != null)
                {
                    return $"{tercero.Nombre} {tercero.Apellidos}";
                }
                return terceroId;
            }
            catch
            {
                return terceroId;
            }
        }
        
        // Método auxiliar para calcular el total de la venta
        private string ObtenerTotalVenta(int ventaId)
        {
            try
            {
                var detalles = _context.DetalleVentas.Where(d => d.VentaId == ventaId).ToList();
                decimal total = detalles.Sum(d => d.Cantidad * d.Valor);
                return $"{total:C}";
            }
            catch
            {
                return "No calculado";
            }
        }

        private async Task CrearVentaAsync()
        {
            UIHelper.MostrarTitulo("Crear Nueva Venta");
            
            try
            {
                // Mostrar empleados disponibles
                await MostrarEmpleadosDisponibles();
                
                var empleadoIdStr = UIHelper.SolicitarEntrada("Ingrese el ID del empleado");
                if (string.IsNullOrWhiteSpace(empleadoIdStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID del empleado es obligatorio.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                    return;
                }
                
                var empleadoId = int.Parse(empleadoIdStr);
                
                // Verificar que el empleado exista
                var empleado = await _context.Empleados.FindAsync(empleadoId);
                if (empleado == null)
                {
                    UIHelper.MostrarError($"El empleado con ID {empleadoId} no existe.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                    return;
                }
                
                // Obtener el tercero asociado al empleado
                var terceroEmpleadoId = empleado.TerceroId;
                var terceroEmpleado = await _context.Terceros.FindAsync(terceroEmpleadoId);
                if (terceroEmpleado == null)
                {
                    UIHelper.MostrarError($"El tercero asociado al empleado no existe.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                    return;
                }
                
                // Mostrar clientes disponibles
                await MostrarClientesDisponibles();
                
                var clienteId = UIHelper.SolicitarEntrada("Ingrese el ID del cliente");
                if (string.IsNullOrWhiteSpace(clienteId))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID del cliente es obligatorio.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                    return;
                }
                
                // Verificar que el cliente exista
                var cliente = await _context.Terceros.FindAsync(clienteId);
                if (cliente == null)
                {
                    UIHelper.MostrarError($"El cliente con ID {clienteId} no existe. Debe crear el cliente primero.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                    return;
                }
                
                var factIdStr = UIHelper.SolicitarEntrada("Ingrese el ID de la factura");
                if (string.IsNullOrWhiteSpace(factIdStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID de la factura es obligatorio.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                    return;
                }
                
                var factId = int.Parse(factIdStr);
                
                var fechaStr = UIHelper.SolicitarEntrada("Ingrese la fecha (YYYY-MM-DD)", DateTime.Now.ToString("yyyy-MM-dd"));
                var fecha = DateTime.Parse(fechaStr);

                var venta = new Venta { 
                    TerceroEnId = terceroEmpleadoId, 
                    TerceroCliId = clienteId, 
                    FactId = factId,
                    Fecha = fecha 
                };
                
                // Mostrar resumen antes de confirmar
                UIHelper.MostrarTitulo("Resumen de la Venta");
                Console.WriteLine($"Empleado: {ObtenerNombreEmpleado(venta.TerceroEnId)} (ID: {empleadoId})");
                Console.WriteLine($"Cliente: {cliente.Nombre} {cliente.Apellidos}");
                Console.WriteLine($"Factura: {venta.FactId}");
                Console.WriteLine($"Fecha: {venta.Fecha.ToShortDateString()}");
                
                if (UIHelper.Confirmar("¿Desea guardar esta venta?"))
                {
                    _context.Ventas.Add(venta);
                    await _context.SaveChangesAsync();
                    UIHelper.MostrarExito($"Venta creada exitosamente con ID: {venta.Id}");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear la venta", ex);
                Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                Console.ReadKey();
            }
        }

        private async Task EditarVentaAsync()
        {
            UIHelper.MostrarTitulo("Editar Venta");
            
            try
            {
                // Mostrar lista de ventas disponibles
                ListarVentas();
                
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID de la venta a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                    return;
                }
                
                var id = int.Parse(idStr);
                var venta = await _context.Ventas.FindAsync(id);

                if (venta != null)
                {
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {venta.Id}");
                    Console.WriteLine($"Empleado: {ObtenerNombreEmpleado(venta.TerceroEnId)}");
                    Console.WriteLine($"Cliente: {venta.TerceroCliId}");
                    Console.WriteLine($"Factura: {venta.FactId}");
                    Console.WriteLine($"Fecha: {venta.Fecha.ToShortDateString()}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    // Preguntar si se desea cambiar el empleado
                    if (UIHelper.Confirmar("¿Desea cambiar el empleado asignado a esta venta?"))
                    {
                        // Mostrar empleados disponibles
                        await MostrarEmpleadosDisponibles();
                        
                        var empleadoIdStr = UIHelper.SolicitarEntrada("Ingrese el nuevo ID del empleado");
                        if (!string.IsNullOrWhiteSpace(empleadoIdStr))
                        {
                            var empleadoId = int.Parse(empleadoIdStr);
                            
                            // Verificar que el empleado exista
                            var empleado = await _context.Empleados.FindAsync(empleadoId);
                            if (empleado == null)
                            {
                                UIHelper.MostrarError($"El empleado con ID {empleadoId} no existe.");
                                Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                                Console.ReadKey();
                                return;
                            }
                            
                            // Obtener el tercero asociado al empleado
                            venta.TerceroEnId = empleado.TerceroId;
                        }
                    }
                    
                    // Preguntar si se desea cambiar el cliente
                    if (UIHelper.Confirmar("¿Desea cambiar el cliente asignado a esta venta?"))
                    {
                        // Mostrar clientes disponibles
                        await MostrarClientesDisponibles();
                        
                        var clienteId = UIHelper.SolicitarEntrada("Nuevo ID del cliente", venta.TerceroCliId);
                        
                        // Verificar que el cliente exista
                        var cliente = await _context.Terceros.FindAsync(clienteId);
                        if (cliente == null)
                        {
                            UIHelper.MostrarError($"El cliente con ID {clienteId} no existe. Debe crear el cliente primero.");
                            Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                            Console.ReadKey();
                            return;
                        }
                        
                        venta.TerceroCliId = clienteId;
                    }
                    
                    var factIdStr = UIHelper.SolicitarEntrada("Nuevo ID de la factura", venta.FactId.ToString());
                    var factId = int.Parse(factIdStr);
                    
                    var fechaStr = UIHelper.SolicitarEntrada("Nueva fecha (YYYY-MM-DD)", venta.Fecha.ToString("yyyy-MM-dd"));
                    
                    venta.FactId = factId;
                    venta.Fecha = DateTime.Parse(fechaStr);

                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(venta);
                        await _context.SaveChangesAsync();
                        UIHelper.MostrarExito("Venta actualizada exitosamente.");
                        Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                        Console.ReadKey();
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                        Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    UIHelper.MostrarError("Venta no encontrada.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar la venta", ex);
                Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                Console.ReadKey();
            }
        }

        private async Task EliminarVentaAsync()
        {
            UIHelper.MostrarTitulo("Eliminar Venta");
            
            try
            {
                // Mostrar lista de ventas disponibles
                ListarVentas();
                
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID de la venta a eliminar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                    return;
                }
                
                var id = int.Parse(idStr);
                var venta = await _context.Ventas.FindAsync(id);

                if (venta != null)
                {
                    // Verificar si existen detalles asociados
                    var detalles = await _context.DetalleVentas.Where(d => d.VentaId == id).ToListAsync();
                    if (detalles.Any())
                    {
                        UIHelper.MostrarAdvertencia($"La venta tiene {detalles.Count} detalles asociados. Estos serán eliminados también.");
                        
                        // Mostrar los detalles que se eliminarán
                        var columnasDetalles = new Dictionary<string, Func<DetalleVenta, object>>
                        {
                            { "ID", d => d.Id },
                            { "Producto", d => d.ProductosId },
                            { "Cantidad", d => d.Cantidad },
                            { "Valor Unit.", d => $"{d.Valor:C}" },
                            { "Total", d => $"{(d.Cantidad * d.Valor):C}" }
                        };
                        UIHelper.DibujarTabla(detalles, columnasDetalles, "Detalles que serán eliminados");
                    }
                    
                    // Mostrar información de la venta a eliminar
                    UIHelper.MostrarTitulo("Información de la Venta a Eliminar");
                    Console.WriteLine($"ID: {venta.Id}");
                    Console.WriteLine($"Empleado: {ObtenerNombreEmpleado(venta.TerceroEnId)}");
                    Console.WriteLine($"Cliente: {venta.TerceroCliId}");
                    Console.WriteLine($"Factura: {venta.FactId}");
                    Console.WriteLine($"Fecha: {venta.Fecha.ToShortDateString()}");
                    Console.WriteLine($"Total: {ObtenerTotalVenta(venta.Id)}");
                    
                    if (UIHelper.Confirmar("¿Está ABSOLUTAMENTE seguro que desea eliminar esta venta y todos sus detalles?"))
                    {
                        var strategy = _context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using (var transaction = await _context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    // Primero eliminamos los detalles
                                    if (detalles.Any())
                                    {
                                        _context.DetalleVentas.RemoveRange(detalles);
                                        await _context.SaveChangesAsync(); // Guardamos primero los cambios de los detalles
                                    }
                                    
                                    // Luego eliminamos la venta
                                    _context.Ventas.Remove(venta);
                                    await _context.SaveChangesAsync(); // Guardamos los cambios de la venta
                                    
                                    await transaction.CommitAsync(); // Confirmamos la transacción
                                    UIHelper.MostrarExito("Venta y sus detalles eliminados exitosamente.");
                                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                                    Console.ReadKey();
                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync();
                                    throw new Exception("Error al eliminar la venta y sus detalles. Se han revertido los cambios.", ex);
                                }
                            }
                        });
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                        Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    UIHelper.MostrarError("Venta no encontrada.");
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar la venta", ex);
                Console.WriteLine("\nPresione cualquier tecla para volver al menú de ventas...");
                Console.ReadKey();
            }
        }
        
        // Métodos auxiliares para mostrar entidades relacionadas
        
        private async Task MostrarEmpleadosDisponibles()
        {
            UIHelper.MostrarTitulo("Empleados Disponibles");
            var empleados = await _context.Empleados.ToListAsync();
            
            if (!empleados.Any())
            {
                UIHelper.MostrarAdvertencia("No hay empleados registrados.");
                Console.ReadKey();
                return;
            }
            
            var empleadosData = new List<(int Id, string Nombre, string TerceroId)>();
            
            foreach (var empleado in empleados)
            {
                var tercero = await _context.Terceros.FindAsync(empleado.TerceroId);
                if (tercero != null)
                {
                    empleadosData.Add((empleado.Id, $"{tercero.Nombre} {tercero.Apellidos}", empleado.TerceroId));
                }
            }
            
            Console.WriteLine("┌───────┬────────────────────────────────┬──────────────┐");
            Console.WriteLine("│   ID  │ Nombre                         │ ID Tercero   │");
            Console.WriteLine("├───────┼────────────────────────────────┼──────────────┤");
            
            foreach (var emp in empleadosData)
            {
                Console.WriteLine($"│ {emp.Id.ToString().PadRight(5)} │ {emp.Nombre.PadRight(30)} │ {emp.TerceroId.PadRight(12)} │");
            }
            
            Console.WriteLine("└───────┴────────────────────────────────┴──────────────┘");
            Console.WriteLine();
        }
        
        private async Task MostrarClientesDisponibles()
        {
            UIHelper.MostrarTitulo("Clientes Disponibles");
            var clientes = await _context.Clientes.ToListAsync();
            
            if (!clientes.Any())
            {
                UIHelper.MostrarAdvertencia("No hay clientes registrados.");
                
                // Mostrar todos los terceros como alternativa
                var terceros = await _context.Terceros.ToListAsync();
                
                var columnasTerceros = new Dictionary<string, Func<Terceros, object>>
                {
                    { "ID", t => t.Id },
                    { "Nombre", t => t.Nombre },
                    { "Apellidos", t => t.Apellidos }
                };
                
                UIHelper.DibujarTabla(terceros, columnasTerceros, "Lista de Terceros Disponibles");
                return;
            }
            
            var clientesData = new List<(int Id, string Nombre, string TerceroId)>();
            
            foreach (var cliente in clientes)
            {
                var tercero = await _context.Terceros.FindAsync(cliente.TerceroId);
                if (tercero != null)
                {
                    clientesData.Add((cliente.Id, $"{tercero.Nombre} {tercero.Apellidos}", cliente.TerceroId));
                }
            }
            
            Console.WriteLine("┌───────┬────────────────────────────────┬──────────────┐");
            Console.WriteLine("│   ID  │ Nombre                         │ ID Tercero   │");
            Console.WriteLine("├───────┼────────────────────────────────┼──────────────┤");
            
            foreach (var cli in clientesData)
            {
                Console.WriteLine($"│ {cli.Id.ToString().PadRight(5)} │ {cli.Nombre.PadRight(30)} │ {cli.TerceroId.PadRight(12)} │");
            }
            
            Console.WriteLine("└───────┴────────────────────────────────┴──────────────┘");
            Console.WriteLine();
        }
    }
}