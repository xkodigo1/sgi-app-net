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
                UIHelper.MostrarTitulo("Panel de Compras");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Compras" },
                    { "2", "Crear Nueva Compra" },
                    { "3", "Editar Compra" },
                    { "4", "Eliminar Compra" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

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
                    case "0":
                        return;
                    default:
                        UIHelper.MostrarAdvertencia("Opción no válida. Intente de nuevo.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ListarCompras()
        {
            UIHelper.MostrarTitulo("Listado de Compras");
            
            try
            {
                var compras = _context.Compras.ToList();
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<Compra, object>>
                {
                    { "ID", c => c.Id },
                    { "Proveedor", c => c.TerceroProvId },
                    { "Empleado", c => c.TerceroEmpId },
                    { "Documento", c => c.DocCompra },
                    { "Fecha", c => c.Fecha.ToShortDateString() },
                    { "Total", c => ObtenerTotalCompra(c.Id) }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(compras, columnas, "Registro de Compras");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar las compras", ex);
            }
        }
        
        // Método auxiliar para calcular el total de la compra
        private string ObtenerTotalCompra(int compraId)
        {
            try
            {
                var detalles = _context.DetalleCompras.Where(d => d.CompraId == compraId).ToList();
                decimal total = detalles.Sum(d => d.Cantidad * d.Valor);
                return $"{total:C}";
            }
            catch
            {
                return "No calculado";
            }
        }

        private void CrearCompra()
        {
            UIHelper.MostrarTitulo("Crear Nueva Compra");
            
            try
            {
                var proveedorId = UIHelper.SolicitarEntrada("Ingrese el ID del proveedor");
                if (string.IsNullOrWhiteSpace(proveedorId))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID del proveedor es obligatorio.");
                    return;
                }
                
                // Verificar que el proveedor exista
                var proveedor = _context.Terceros.Find(proveedorId);
                if (proveedor == null)
                {
                    UIHelper.MostrarError($"El tercero con ID {proveedorId} no existe. Debe crear el tercero primero.");
                    return;
                }
                
                var empleadoId = UIHelper.SolicitarEntrada("Ingrese el ID del empleado");
                if (string.IsNullOrWhiteSpace(empleadoId))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID del empleado es obligatorio.");
                    return;
                }
                
                // Verificar que el empleado exista
                var empleado = _context.Terceros.Find(empleadoId);
                if (empleado == null)
                {
                    UIHelper.MostrarError($"El tercero con ID {empleadoId} no existe. Debe crear el tercero primero.");
                    return;
                }
                
                var docCompra = UIHelper.SolicitarEntrada("Ingrese el documento de compra");
                if (string.IsNullOrWhiteSpace(docCompra))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El documento de compra es obligatorio.");
                    return;
                }
                
                var fechaStr = UIHelper.SolicitarEntrada("Ingrese la fecha (YYYY-MM-DD)", DateTime.Now.ToString("yyyy-MM-dd"));
                var fecha = DateTime.Parse(fechaStr);

                var compra = new Compra { 
                    TerceroProvId = proveedorId, 
                    TerceroEmpId = empleadoId, 
                    DocCompra = docCompra,
                    Fecha = fecha
                };
                
                // Mostrar resumen antes de confirmar
                UIHelper.MostrarTitulo("Resumen de la Compra");
                Console.WriteLine($"Proveedor: {compra.TerceroProvId}");
                Console.WriteLine($"Empleado: {compra.TerceroEmpId}");
                Console.WriteLine($"Documento: {compra.DocCompra}");
                Console.WriteLine($"Fecha: {compra.Fecha.ToShortDateString()}");
                
                if (UIHelper.Confirmar("¿Desea guardar esta compra?"))
                {
                    _context.Compras.Add(compra);
                    _context.SaveChanges();
                    UIHelper.MostrarExito($"Compra creada exitosamente con ID: {compra.Id}");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear la compra", ex);
            }
        }

        private void EditarCompra()
        {
            UIHelper.MostrarTitulo("Editar Compra");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID de la compra a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var compra = _context.Compras.Find(id);

                if (compra != null)
                {
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {compra.Id}");
                    Console.WriteLine($"Proveedor: {compra.TerceroProvId}");
                    Console.WriteLine($"Empleado: {compra.TerceroEmpId}");
                    Console.WriteLine($"Documento: {compra.DocCompra}");
                    Console.WriteLine($"Fecha: {compra.Fecha.ToShortDateString()}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    var proveedorId = UIHelper.SolicitarEntrada("Nuevo ID del proveedor", compra.TerceroProvId);
                    
                    // Verificar que el proveedor exista
                    var proveedor = _context.Terceros.Find(proveedorId);
                    if (proveedor == null)
                    {
                        UIHelper.MostrarError($"El tercero con ID {proveedorId} no existe. Debe crear el tercero primero.");
                        return;
                    }
                    
                    var empleadoId = UIHelper.SolicitarEntrada("Nuevo ID del empleado", compra.TerceroEmpId);
                    
                    // Verificar que el empleado exista
                    var empleado = _context.Terceros.Find(empleadoId);
                    if (empleado == null)
                    {
                        UIHelper.MostrarError($"El tercero con ID {empleadoId} no existe. Debe crear el tercero primero.");
                        return;
                    }
                    
                    var docCompra = UIHelper.SolicitarEntrada("Nuevo documento de compra", compra.DocCompra);
                    var fechaStr = UIHelper.SolicitarEntrada("Nueva fecha (YYYY-MM-DD)", compra.Fecha.ToString("yyyy-MM-dd"));
                    
                    compra.TerceroProvId = proveedorId;
                    compra.TerceroEmpId = empleadoId;
                    compra.DocCompra = docCompra;
                    compra.Fecha = DateTime.Parse(fechaStr);

                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(compra);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Compra actualizada exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Compra no encontrada.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar la compra", ex);
            }
        }

        private void EliminarCompra()
        {
            UIHelper.MostrarTitulo("Eliminar Compra");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID de la compra a eliminar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var compra = _context.Compras.Find(id);

                if (compra != null)
                {
                    // Verificar si existen detalles asociados
                    var detalles = _context.DetalleCompras.Where(d => d.CompraId == id).ToList();
                    if (detalles.Any())
                    {
                        UIHelper.MostrarAdvertencia($"La compra tiene {detalles.Count} detalles asociados. Estos serán eliminados también.");
                    }
                    
                    // Mostrar información a eliminar
                    UIHelper.MostrarTitulo("Información de la Compra a Eliminar");
                    Console.WriteLine($"ID: {compra.Id}");
                    Console.WriteLine($"Proveedor: {compra.TerceroProvId}");
                    Console.WriteLine($"Empleado: {compra.TerceroEmpId}");
                    Console.WriteLine($"Documento: {compra.DocCompra}");
                    Console.WriteLine($"Fecha: {compra.Fecha.ToShortDateString()}");
                    
                    if (UIHelper.Confirmar("¿Está seguro que desea eliminar esta compra y todos sus detalles?"))
                    {
                        // Eliminar detalles asociados primero
                        foreach (var detalle in detalles)
                        {
                            _context.DetalleCompras.Remove(detalle);
                        }
                        
                        _context.Compras.Remove(compra);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Compra y sus detalles eliminados exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Compra no encontrada.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar la compra", ex);
            }
        }
    }
}