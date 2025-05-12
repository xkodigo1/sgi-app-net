using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task ShowMenu()
        {
            while (true)
            {
                UIHelper.MostrarTitulo("Panel de Empleados");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Empleados" },
                    { "2", "Crear Nuevo Empleado" },
                    { "3", "Editar Empleado" },
                    { "4", "Eliminar Empleado" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarEmpleados();
                        break;
                    case "2":
                        await CrearEmpleadoAsync();
                        break;
                    case "3":
                        await EditarEmpleadoAsync();
                        break;
                    case "4":
                        await EliminarEmpleadoAsync();
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

        private void ListarEmpleados()
        {
            UIHelper.MostrarTitulo("Listado de Empleados");
            
            try
            {
                var empleados = _context.Empleados.ToList();
                
                if (!empleados.Any())
                {
                    UIHelper.MostrarAdvertencia("No hay empleados registrados.");
                    Console.ReadKey();
                    return;
                }
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<Empleado, object>>
                {
                    { "ID", e => e.Id },
                    { "Tercero", e => e.TerceroId },
                    { "Fecha Ingreso", e => e.FechaIngreso.ToShortDateString() },
                    { "Salario Base", e => $"{e.SalarioBase:C0}" },
                    { "EPS", e => e.EpsId },
                    { "ARL", e => e.ArlId }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(empleados, columnas, "Registro de Empleados");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar los empleados", ex);
            }
        }

        private async Task CrearEmpleadoAsync()
        {
            UIHelper.MostrarTitulo("Crear Nuevo Empleado");
            
            try
            {
                // Mostrar terceros disponibles
                await MostrarTercerosDisponibles();
                
                var terceroId = UIHelper.SolicitarEntrada("Ingrese el ID del tercero");
                if (string.IsNullOrWhiteSpace(terceroId))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID del tercero es obligatorio.");
                    return;
                }
                
                // Verificar que el tercero exista
                var tercero = await _context.Terceros.FindAsync(terceroId);
                if (tercero == null)
                {
                    UIHelper.MostrarError($"El tercero con ID {terceroId} no existe. Debe crear el tercero primero.");
                    return;
                }
                
                // Verificar que el tercero no esté ya asignado a otro empleado
                var empleadoExistente = await _context.Empleados.FirstOrDefaultAsync(e => e.TerceroId == terceroId);
                if (empleadoExistente != null)
                {
                    UIHelper.MostrarError($"El tercero con ID {terceroId} ya está asignado a otro empleado (ID: {empleadoExistente.Id}).");
                    return;
                }
                
                var fechaIngresoStr = UIHelper.SolicitarEntrada("Ingrese la fecha de ingreso (YYYY-MM-DD)", DateTime.Now.ToString("yyyy-MM-dd"));
                var fechaIngreso = DateTime.Parse(fechaIngresoStr);
                
                var salarioBaseStr = UIHelper.SolicitarEntrada("Ingrese el salario base");
                if (string.IsNullOrWhiteSpace(salarioBaseStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El salario base es obligatorio.");
                    return;
                }
                var salarioBase = double.Parse(salarioBaseStr);
                
                // Mostrar EPS disponibles
                await MostrarEPSDisponibles();
                
                var epsIdStr = UIHelper.SolicitarEntrada("Ingrese el ID de la EPS");
                if (string.IsNullOrWhiteSpace(epsIdStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID de la EPS es obligatorio.");
                    return;
                }
                var epsId = int.Parse(epsIdStr);
                
                // Verificar que la EPS exista
                var eps = await _context.Set<EPS>().FindAsync(epsId);
                if (eps == null)
                {
                    UIHelper.MostrarError($"La EPS con ID {epsId} no existe. Debe crearla primero.");
                    return;
                }
                
                // Mostrar ARL disponibles
                await MostrarARLDisponibles();
                
                var arlIdStr = UIHelper.SolicitarEntrada("Ingrese el ID de la ARL");
                if (string.IsNullOrWhiteSpace(arlIdStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID de la ARL es obligatorio.");
                    return;
                }
                var arlId = int.Parse(arlIdStr);
                
                // Verificar que la ARL exista
                var arl = await _context.Set<ARL>().FindAsync(arlId);
                if (arl == null)
                {
                    UIHelper.MostrarError($"La ARL con ID {arlId} no existe. Debe crearla primero.");
                    return;
                }

                var empleado = new Empleado { 
                    TerceroId = terceroId, 
                    FechaIngreso = fechaIngreso, 
                    SalarioBase = salarioBase, 
                    EpsId = epsId, 
                    ArlId = arlId 
                };
                
                // Mostrar resumen antes de confirmar
                UIHelper.MostrarTitulo("Resumen del Empleado");
                Console.WriteLine($"Tercero: {empleado.TerceroId}");
                Console.WriteLine($"Fecha Ingreso: {empleado.FechaIngreso.ToShortDateString()}");
                Console.WriteLine($"Salario Base: {empleado.SalarioBase:C0}");
                Console.WriteLine($"EPS: {empleado.EpsId}");
                Console.WriteLine($"ARL: {empleado.ArlId}");
                
                if (UIHelper.Confirmar("¿Desea guardar este empleado?"))
                {
                    _context.Empleados.Add(empleado);
                    await _context.SaveChangesAsync();
                    UIHelper.MostrarExito($"Empleado creado exitosamente con ID: {empleado.Id}");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear el empleado", ex);
            }
        }

        private async Task EditarEmpleadoAsync()
        {
            UIHelper.MostrarTitulo("Editar Empleado");
            
            try
            {
                // Mostrar lista de empleados disponibles
                ListarEmpleados();
                
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del empleado a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var empleado = await _context.Empleados.FindAsync(id);

                if (empleado != null)
                {
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {empleado.Id}");
                    Console.WriteLine($"Tercero: {empleado.TerceroId}");
                    Console.WriteLine($"Fecha Ingreso: {empleado.FechaIngreso.ToShortDateString()}");
                    Console.WriteLine($"Salario Base: {empleado.SalarioBase:C0}");
                    Console.WriteLine($"EPS: {empleado.EpsId}");
                    Console.WriteLine($"ARL: {empleado.ArlId}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    // Mostrar terceros disponibles
                    await MostrarTercerosDisponibles();
                    
                    var terceroId = UIHelper.SolicitarEntrada("Nuevo ID del tercero", empleado.TerceroId);
                    
                    // Verificar que el tercero exista
                    var tercero = await _context.Terceros.FindAsync(terceroId);
                    if (tercero == null)
                    {
                        UIHelper.MostrarError($"El tercero con ID {terceroId} no existe. Debe crear el tercero primero.");
                        return;
                    }
                    
                    // Si cambia el tercero, verificar que no esté asignado a otro empleado
                    if (terceroId != empleado.TerceroId)
                    {
                        var empleadoExistente = await _context.Empleados.FirstOrDefaultAsync(e => e.TerceroId == terceroId && e.Id != id);
                        if (empleadoExistente != null)
                        {
                            UIHelper.MostrarError($"El tercero con ID {terceroId} ya está asignado a otro empleado (ID: {empleadoExistente.Id}).");
                            return;
                        }
                    }
                    
                    var fechaIngresoStr = UIHelper.SolicitarEntrada("Nueva fecha de ingreso (YYYY-MM-DD)", empleado.FechaIngreso.ToString("yyyy-MM-dd"));
                    var salarioBaseStr = UIHelper.SolicitarEntrada("Nuevo salario base", empleado.SalarioBase.ToString());
                    
                    // Mostrar EPS disponibles
                    await MostrarEPSDisponibles();
                    
                    var epsIdStr = UIHelper.SolicitarEntrada("Nuevo ID de la EPS", empleado.EpsId.ToString());
                    var epsId = int.Parse(epsIdStr);
                    
                    // Verificar que la EPS exista
                    var eps = await _context.Set<EPS>().FindAsync(epsId);
                    if (eps == null)
                    {
                        UIHelper.MostrarError($"La EPS con ID {epsId} no existe. Debe crearla primero.");
                        return;
                    }
                    
                    // Mostrar ARL disponibles
                    await MostrarARLDisponibles();
                    
                    var arlIdStr = UIHelper.SolicitarEntrada("Nuevo ID de la ARL", empleado.ArlId.ToString());
                    var arlId = int.Parse(arlIdStr);
                    
                    // Verificar que la ARL exista
                    var arl = await _context.Set<ARL>().FindAsync(arlId);
                    if (arl == null)
                    {
                        UIHelper.MostrarError($"La ARL con ID {arlId} no existe. Debe crearla primero.");
                        return;
                    }
                    
                    empleado.TerceroId = terceroId;
                    empleado.FechaIngreso = DateTime.Parse(fechaIngresoStr);
                    empleado.SalarioBase = double.Parse(salarioBaseStr);
                    empleado.EpsId = epsId;
                    empleado.ArlId = arlId;

                    // Mostrar resumen de cambios antes de confirmar
                    UIHelper.MostrarTitulo("Resumen de Cambios");
                    Console.WriteLine($"ID: {empleado.Id}");
                    Console.WriteLine($"Tercero: {empleado.TerceroId}");
                    Console.WriteLine($"Fecha Ingreso: {empleado.FechaIngreso.ToShortDateString()}");
                    Console.WriteLine($"Salario Base: {empleado.SalarioBase:C0}");
                    Console.WriteLine($"EPS: {empleado.EpsId}");
                    Console.WriteLine($"ARL: {empleado.ArlId}");
                    
                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(empleado);
                        await _context.SaveChangesAsync();
                        UIHelper.MostrarExito("Empleado actualizado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Empleado no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar el empleado", ex);
            }
        }

        private async Task EliminarEmpleadoAsync()
        {
            UIHelper.MostrarTitulo("Eliminar Empleado");
            
            try
            {
                // Mostrar lista de empleados disponibles
                ListarEmpleados();
                
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del empleado a eliminar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var empleado = await _context.Empleados.FindAsync(id);

                if (empleado != null)
                {
                    // Verificar si existen compras o ventas asociadas
                    var compras = await _context.Compras.Where(c => c.TerceroEmpId == empleado.TerceroId).ToListAsync();
                    var ventas = await _context.Ventas.Where(v => v.TerceroEnId == empleado.TerceroId).ToListAsync();
                    
                    if (compras.Any() || ventas.Any())
                    {
                        UIHelper.MostrarAdvertencia($"Este empleado tiene {compras.Count} compras y {ventas.Count} ventas asociadas.");
                        if (!UIHelper.Confirmar("¿Está seguro que desea eliminar este empleado y sus asociaciones?"))
                        {
                            UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                            return;
                        }
                    }
                    
                    // Mostrar información a eliminar
                    UIHelper.MostrarTitulo("Información del Empleado a Eliminar");
                    Console.WriteLine($"ID: {empleado.Id}");
                    Console.WriteLine($"Tercero: {empleado.TerceroId}");
                    Console.WriteLine($"Fecha Ingreso: {empleado.FechaIngreso.ToShortDateString()}");
                    Console.WriteLine($"Salario Base: {empleado.SalarioBase:C0}");
                    Console.WriteLine($"EPS: {empleado.EpsId}");
                    Console.WriteLine($"ARL: {empleado.ArlId}");
                    
                    if (UIHelper.Confirmar("¿Está ABSOLUTAMENTE seguro que desea eliminar este empleado?"))
                    {
                        var strategy = _context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using (var transaction = await _context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    _context.Empleados.Remove(empleado);
                                    await _context.SaveChangesAsync();
                                    
                                    await transaction.CommitAsync();
                                    UIHelper.MostrarExito("Empleado eliminado exitosamente.");
                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync();
                                    throw new Exception("Error al eliminar el empleado. Se han revertido los cambios.", ex);
                                }
                            }
                        });
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Empleado no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar el empleado", ex);
            }
        }
        
        // Métodos auxiliares para mostrar entidades relacionadas
        
        private async Task MostrarTercerosDisponibles()
        {
            UIHelper.MostrarTitulo("Terceros Disponibles");
            var terceros = await _context.Terceros.ToListAsync();
            
            if (!terceros.Any())
            {
                UIHelper.MostrarAdvertencia("No hay terceros registrados. Debe crear un tercero primero.");
                return;
            }
            
            var columnas = new Dictionary<string, Func<Terceros, object>>
            {
                { "ID", t => t.Id },
                { "Nombre", t => t.Nombre },
                { "Apellidos", t => t.Apellidos },
                { "Email", t => t.Email ?? "N/A" }
            };
            
            UIHelper.DibujarTabla(terceros, columnas, "Lista de Terceros");
            Console.WriteLine();
        }
        
        private async Task MostrarEPSDisponibles()
        {
            UIHelper.MostrarTitulo("EPS Disponibles");
            var epsList = await _context.Set<EPS>().ToListAsync();
            
            if (!epsList.Any())
            {
                UIHelper.MostrarAdvertencia("No hay EPS registradas.");
                return;
            }
            
            var columnas = new Dictionary<string, Func<EPS, object>>
            {
                { "ID", e => e.Id },
                { "Nombre", e => e.Nombre }
            };
            
            UIHelper.DibujarTabla(epsList, columnas, "Lista de EPS");
            Console.WriteLine();
        }
        
        private async Task MostrarARLDisponibles()
        {
            UIHelper.MostrarTitulo("ARL Disponibles");
            var arlList = await _context.Set<ARL>().ToListAsync();
            
            if (!arlList.Any())
            {
                UIHelper.MostrarAdvertencia("No hay ARL registradas.");
                return;
            }
            
            var columnas = new Dictionary<string, Func<ARL, object>>
            {
                { "ID", a => a.Id },
                { "Nombre", a => a.Nombre }
            };
            
            UIHelper.DibujarTabla(arlList, columnas, "Lista de ARL");
            Console.WriteLine();
        }
    }
}