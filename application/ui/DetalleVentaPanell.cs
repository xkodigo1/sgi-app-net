using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class DetalleVentaPanel
    {
        private readonly YourDbContext _context;

        public DetalleVentaPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.MostrarTitulo("Panel de Detalle de Ventas");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Detalles de Ventas" },
                    { "2", "Crear Nuevo Detalle" },
                    { "3", "Editar Detalle Existente" },
                    { "4", "Eliminar Detalle" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarDetalles();
                        break;
                    case "2":
                        CrearDetalle();
                        break;
                    case "3":
                        EditarDetalle();
                        break;
                    case "4":
                        EliminarDetalle();
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

        private void ListarDetalles()
        {
            UIHelper.MostrarTitulo("Listado de Detalles de Ventas");
            
            try
            {
                var detalles = _context.DetalleVentas.ToList();
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<DetalleVenta, object>>
                {
                    { "ID", d => d.Id },
                    { "Producto", d => d.ProductosId },
                    { "Venta", d => d.VentaId },
                    { "Cantidad", d => d.Cantidad },
                    { "Valor Unit.", d => $"{d.Valor:C}" },
                    { "Total", d => $"{(d.Cantidad * d.Valor):C}" }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(detalles, columnas, "Detalles de Ventas");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar los detalles de ventas", ex);
            }
        }

        private void CrearDetalle()
        {
            UIHelper.MostrarTitulo("Crear Nuevo Detalle de Venta");
            
            try
            {
                // Usar el método SolicitarEntrada para mejor UX
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del detalle");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                
                // Verificar que no exista ya un detalle con este ID
                var detalleExistente = _context.DetalleVentas.Find(id);
                if (detalleExistente != null)
                {
                    UIHelper.MostrarError($"Ya existe un detalle de venta con el ID {id}.");
                    return;
                }
                
                var productoId = UIHelper.SolicitarEntrada("Ingrese el ID del producto");
                
                // Verificar que el producto exista
                var producto = _context.Productos.Find(productoId);
                if (producto == null)
                {
                    UIHelper.MostrarError($"El producto con ID {productoId} no existe. Debe crear el producto primero.");
                    return;
                }
                
                var ventaIdStr = UIHelper.SolicitarEntrada("Ingrese el ID de la venta");
                var ventaId = int.Parse(ventaIdStr);
                
                // Verificar que la venta exista
                var venta = _context.Ventas.Find(ventaId);
                if (venta == null)
                {
                    UIHelper.MostrarError($"La venta con ID {ventaId} no existe. Debe crear la venta primero.");
                    return;
                }
                
                // Verificar que no exista un detalle con la misma combinación de venta y producto
                var detalleDuplicado = _context.DetalleVentas.FirstOrDefault(d => d.VentaId == ventaId && d.ProductosId == productoId);
                if (detalleDuplicado != null)
                {
                    UIHelper.MostrarError($"Ya existe un detalle para la venta {ventaId} y el producto {productoId}.\nSi desea modificar la cantidad, edite el detalle con ID {detalleDuplicado.Id}.");
                    return;
                }
                
                var cantidadStr = UIHelper.SolicitarEntrada("Ingrese la cantidad");
                var cantidad = int.Parse(cantidadStr);
                
                var valorStr = UIHelper.SolicitarEntrada("Ingrese el valor unitario");
                var valor = decimal.Parse(valorStr);

                var detalle = new DetalleVenta { 
                    Id = id, 
                    ProductosId = productoId, 
                    VentaId = ventaId,
                    Cantidad = cantidad,
                    Valor = valor
                };
                
                // Mostrar resumen antes de confirmar
                UIHelper.MostrarTitulo("Resumen del Detalle");
                Console.WriteLine($"ID: {detalle.Id}");
                Console.WriteLine($"Producto: {detalle.ProductosId}");
                Console.WriteLine($"Venta: {detalle.VentaId}");
                Console.WriteLine($"Cantidad: {detalle.Cantidad}");
                Console.WriteLine($"Valor unitario: {detalle.Valor:C}");
                Console.WriteLine($"Total: {(detalle.Cantidad * detalle.Valor):C}");
                
                if (UIHelper.Confirmar("¿Desea guardar este detalle?"))
                {
                    _context.DetalleVentas.Add(detalle);
                    _context.SaveChanges();

                    UIHelper.MostrarExito("Detalle creado exitosamente.");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear el detalle", ex);
            }
        }

        private void EditarDetalle()
        {
            UIHelper.MostrarTitulo("Editar Detalle de Venta");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del detalle a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var detalle = _context.DetalleVentas.Find(id);

                if (detalle != null)
                {
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {detalle.Id}");
                    Console.WriteLine($"Producto: {detalle.ProductosId}");
                    Console.WriteLine($"Venta: {detalle.VentaId}");
                    Console.WriteLine($"Cantidad: {detalle.Cantidad}");
                    Console.WriteLine($"Valor unitario: {detalle.Valor:C}");
                    Console.WriteLine($"Total: {(detalle.Cantidad * detalle.Valor):C}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    var productoId = UIHelper.SolicitarEntrada("Nuevo ID del producto", detalle.ProductosId);
                    
                    // Verificar que el producto exista
                    var producto = _context.Productos.Find(productoId);
                    if (producto == null)
                    {
                        UIHelper.MostrarError($"El producto con ID {productoId} no existe. Debe crear el producto primero.");
                        return;
                    }
                    
                    var ventaIdStr = UIHelper.SolicitarEntrada("Nuevo ID de la venta", detalle.VentaId.ToString());
                    var ventaId = int.Parse(ventaIdStr);
                    
                    // Verificar que la venta exista
                    var venta = _context.Ventas.Find(ventaId);
                    if (venta == null)
                    {
                        UIHelper.MostrarError($"La venta con ID {ventaId} no existe. Debe crear la venta primero.");
                        return;
                    }
                    
                    // Si cambian el producto o la venta, verificar que no exista otro detalle con la misma combinación
                    if (productoId != detalle.ProductosId || ventaId != detalle.VentaId)
                    {
                        var detalleDuplicado = _context.DetalleVentas.FirstOrDefault(d => 
                            d.VentaId == ventaId && 
                            d.ProductosId == productoId && 
                            d.Id != id);
                            
                        if (detalleDuplicado != null)
                        {
                            UIHelper.MostrarError($"Ya existe un detalle para la venta {ventaId} y el producto {productoId}.");
                            return;
                        }
                    }
                    
                    var cantidadStr = UIHelper.SolicitarEntrada("Nueva cantidad", detalle.Cantidad.ToString());
                    var valorStr = UIHelper.SolicitarEntrada("Nuevo valor unitario", detalle.Valor.ToString());
                    
                    detalle.ProductosId = productoId;
                    detalle.VentaId = ventaId;
                    detalle.Cantidad = int.Parse(cantidadStr);
                    detalle.Valor = decimal.Parse(valorStr);

                    // Mostrar resumen de cambios antes de confirmar
                    UIHelper.MostrarTitulo("Resumen de Cambios");
                    Console.WriteLine($"ID: {detalle.Id}");
                    Console.WriteLine($"Producto: {detalle.ProductosId}");
                    Console.WriteLine($"Venta: {detalle.VentaId}");
                    Console.WriteLine($"Cantidad: {detalle.Cantidad}");
                    Console.WriteLine($"Valor unitario: {detalle.Valor:C}");
                    Console.WriteLine($"Total: {(detalle.Cantidad * detalle.Valor):C}");
                    
                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(detalle);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Detalle actualizado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Detalle no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar el detalle", ex);
            }
        }

        private void EliminarDetalle()
        {
            UIHelper.MostrarTitulo("Eliminar Detalle de Venta");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del detalle a eliminar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var detalle = _context.DetalleVentas.Find(id);

                if (detalle != null)
                {
                    // Mostrar información a eliminar
                    UIHelper.MostrarTitulo("Información del Detalle a Eliminar");
                    Console.WriteLine($"ID: {detalle.Id}");
                    Console.WriteLine($"Producto: {detalle.ProductosId}");
                    Console.WriteLine($"Venta: {detalle.VentaId}");
                    Console.WriteLine($"Cantidad: {detalle.Cantidad}");
                    Console.WriteLine($"Valor unitario: {detalle.Valor:C}");
                    Console.WriteLine($"Total: {(detalle.Cantidad * detalle.Valor):C}");
                    
                    if (UIHelper.Confirmar("¿Está seguro que desea eliminar este detalle?"))
                    {
                        _context.DetalleVentas.Remove(detalle);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Detalle eliminado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Detalle no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar el detalle", ex);
            }
        }
    }
}