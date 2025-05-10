using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void ShowMenu()
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
                        CrearVenta();
                        break;
                    case "3":
                        EditarVenta();
                        break;
                    case "4":
                        EliminarVenta();
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
                    { "Empleado/Vend.", v => v.TerceroEnId },
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

        private void CrearVenta()
        {
            UIHelper.MostrarTitulo("Crear Nueva Venta");
            
            try
            {
                var vendedorId = UIHelper.SolicitarEntrada("Ingrese el ID del vendedor");
                if (string.IsNullOrWhiteSpace(vendedorId))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID del vendedor es obligatorio.");
                    return;
                }
                
                // Verificar que el vendedor exista
                var vendedor = _context.Terceros.Find(vendedorId);
                if (vendedor == null)
                {
                    UIHelper.MostrarError($"El tercero con ID {vendedorId} no existe. Debe crear el tercero primero.");
                    return;
                }
                
                var clienteId = UIHelper.SolicitarEntrada("Ingrese el ID del cliente");
                if (string.IsNullOrWhiteSpace(clienteId))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID del cliente es obligatorio.");
                    return;
                }
                
                // Verificar que el cliente exista
                var cliente = _context.Terceros.Find(clienteId);
                if (cliente == null)
                {
                    UIHelper.MostrarError($"El cliente con ID {clienteId} no existe. Debe crear el cliente primero.");
                    return;
                }
                
                var factIdStr = UIHelper.SolicitarEntrada("Ingrese el ID de la factura");
                if (string.IsNullOrWhiteSpace(factIdStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID de la factura es obligatorio.");
                    return;
                }
                
                var factId = int.Parse(factIdStr);
                
                var fechaStr = UIHelper.SolicitarEntrada("Ingrese la fecha (YYYY-MM-DD)", DateTime.Now.ToString("yyyy-MM-dd"));
                var fecha = DateTime.Parse(fechaStr);

                var venta = new Venta { 
                    TerceroEnId = vendedorId, 
                    TerceroCliId = clienteId, 
                    FactId = factId,
                    Fecha = fecha 
                };
                
                // Mostrar resumen antes de confirmar
                UIHelper.MostrarTitulo("Resumen de la Venta");
                Console.WriteLine($"Vendedor: {venta.TerceroEnId}");
                Console.WriteLine($"Cliente: {venta.TerceroCliId}");
                Console.WriteLine($"Factura: {venta.FactId}");
                Console.WriteLine($"Fecha: {venta.Fecha.ToShortDateString()}");
                
                if (UIHelper.Confirmar("¿Desea guardar esta venta?"))
                {
                    _context.Ventas.Add(venta);
                    _context.SaveChanges();
                    UIHelper.MostrarExito($"Venta creada exitosamente con ID: {venta.Id}");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear la venta", ex);
            }
        }

        private void EditarVenta()
        {
            UIHelper.MostrarTitulo("Editar Venta");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID de la venta a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var venta = _context.Ventas.Find(id);

                if (venta != null)
                {
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {venta.Id}");
                    Console.WriteLine($"Vendedor: {venta.TerceroEnId}");
                    Console.WriteLine($"Cliente: {venta.TerceroCliId}");
                    Console.WriteLine($"Factura: {venta.FactId}");
                    Console.WriteLine($"Fecha: {venta.Fecha.ToShortDateString()}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    var vendedorId = UIHelper.SolicitarEntrada("Nuevo ID del vendedor", venta.TerceroEnId);
                    
                    // Verificar que el vendedor exista
                    var vendedor = _context.Terceros.Find(vendedorId);
                    if (vendedor == null)
                    {
                        UIHelper.MostrarError($"El tercero con ID {vendedorId} no existe. Debe crear el tercero primero.");
                        return;
                    }
                    
                    var clienteId = UIHelper.SolicitarEntrada("Nuevo ID del cliente", venta.TerceroCliId);
                    
                    // Verificar que el cliente exista
                    var cliente = _context.Terceros.Find(clienteId);
                    if (cliente == null)
                    {
                        UIHelper.MostrarError($"El cliente con ID {clienteId} no existe. Debe crear el cliente primero.");
                        return;
                    }
                    
                    var factIdStr = UIHelper.SolicitarEntrada("Nuevo ID de la factura", venta.FactId.ToString());
                    var factId = int.Parse(factIdStr);
                    
                    var fechaStr = UIHelper.SolicitarEntrada("Nueva fecha (YYYY-MM-DD)", venta.Fecha.ToString("yyyy-MM-dd"));
                    
                    venta.TerceroEnId = vendedorId;
                    venta.TerceroCliId = clienteId;
                    venta.FactId = factId;
                    venta.Fecha = DateTime.Parse(fechaStr);

                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(venta);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Venta actualizada exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Venta no encontrada.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar la venta", ex);
            }
        }

        private void EliminarVenta()
        {
            UIHelper.MostrarTitulo("Eliminar Venta");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID de la venta a eliminar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var venta = _context.Ventas.Find(id);

                if (venta != null)
                {
                    // Verificar si existen detalles asociados
                    var detalles = _context.DetalleVentas.Where(d => d.VentaId == id).ToList();
                    if (detalles.Any())
                    {
                        UIHelper.MostrarAdvertencia($"La venta tiene {detalles.Count} detalles asociados. Estos serán eliminados también.");
                    }
                    
                    // Mostrar información a eliminar
                    UIHelper.MostrarTitulo("Información de la Venta a Eliminar");
                    Console.WriteLine($"ID: {venta.Id}");
                    Console.WriteLine($"Vendedor: {venta.TerceroEnId}");
                    Console.WriteLine($"Cliente: {venta.TerceroCliId}");
                    Console.WriteLine($"Factura: {venta.FactId}");
                    Console.WriteLine($"Fecha: {venta.Fecha.ToShortDateString()}");
                    
                    if (UIHelper.Confirmar("¿Está seguro que desea eliminar esta venta y todos sus detalles?"))
                    {
                        // Eliminar detalles asociados primero
                        foreach (var detalle in detalles)
                        {
                            _context.DetalleVentas.Remove(detalle);
                        }
                        
                        _context.Ventas.Remove(venta);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Venta y sus detalles eliminados exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Venta no encontrada.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar la venta", ex);
            }
        }
    }
}