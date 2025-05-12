using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class DetalleCompraPanel
    {
        private readonly YourDbContext _context;

        public DetalleCompraPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.MostrarTitulo("Panel de Detalle de Compras");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Detalles de Compras" },
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
            UIHelper.MostrarTitulo("Listado de Detalles de Compras");
            
            try
            {
                var detalles = _context.DetalleCompras.ToList();
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<DetalleCompra, object>>
                {
                    { "ID", d => d.Id },
                    { "Producto", d => d.ProductoId },
                    { "Compra", d => d.CompraId },
                    { "Fecha", d => d.Fecha.ToShortDateString() },
                    { "Cantidad", d => d.Cantidad },
                    { "Valor Unit.", d => $"{d.Valor:C}" },
                    { "Total", d => $"{(d.Cantidad * d.Valor):C}" }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(detalles, columnas, "Detalles de Compras");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar los detalles de compras", ex);
            }
        }

        private void CrearDetalle()
        {
            try
            {
                Console.Write("Ingrese el ID del detalle: ");
                var id = int.Parse(Console.ReadLine());
                
                // Verificar que no exista ya un detalle con este ID
                var detalleExistente = _context.DetalleCompras.Find(id);
                if (detalleExistente != null)
                {
                    Console.WriteLine($"Error: Ya existe un detalle de compra con el ID {id}.");
                    return;
                }
                
                Console.Write("Ingrese el ID del producto: ");
                var productoId = Console.ReadLine();
                
                // Verificar que el producto exista
                var producto = _context.Productos.Find(productoId);
                if (producto == null)
                {
                    Console.WriteLine("Error: El producto con ID " + productoId + " no existe. Debe crear el producto primero.");
                    return;
                }
                
                Console.Write("Ingrese el ID de la compra: ");
                var compraId = int.Parse(Console.ReadLine());
                
                // Verificar que la compra exista
                var compra = _context.Compras.Find(compraId);
                if (compra == null)
                {
                    Console.WriteLine("Error: La compra con ID " + compraId + " no existe. Debe crear la compra primero.");
                    return;
                }
                
                // Verificar que no exista un detalle con la misma combinación de compra y producto
                var detalleDuplicado = _context.DetalleCompras.FirstOrDefault(d => d.CompraId == compraId && d.ProductoId == productoId);
                if (detalleDuplicado != null)
                {
                    Console.WriteLine($"Error: Ya existe un detalle para la compra {compraId} y el producto {productoId}.");
                    Console.WriteLine($"Si desea modificar la cantidad, edite el detalle con ID {detalleDuplicado.Id}.");
                    return;
                }
                
                Console.Write("Ingrese la fecha (YYYY-MM-DD): ");
                var fecha = DateTime.Parse(Console.ReadLine());
                
                Console.Write("Ingrese la cantidad: ");
                var cantidad = int.Parse(Console.ReadLine());
                
                Console.Write("Ingrese el valor unitario: ");
                var valor = decimal.Parse(Console.ReadLine());

                var detalle = new DetalleCompra { 
                    Id = id, 
                    ProductoId = productoId, 
                    CompraId = compraId,
                    Fecha = fecha,
                    Cantidad = cantidad,
                    Valor = valor
                };
                
                _context.DetalleCompras.Add(detalle);
                _context.SaveChanges();

                Console.WriteLine("Detalle creado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el detalle: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Detalle del error: {ex.InnerException.Message}");
                }
            }
        }

        private void EditarDetalle()
        {
            UIHelper.MostrarTitulo("Editar Detalle de Compra");
            
            try
            {
                // Mostrar lista de detalles disponibles
                ListarDetalles();
                
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del detalle a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var detalle = _context.DetalleCompras.Find(id);

                if (detalle != null)
                {
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {detalle.Id}");
                    Console.WriteLine($"Producto: {detalle.ProductoId}");
                    Console.WriteLine($"Compra: {detalle.CompraId}");
                    Console.WriteLine($"Fecha: {detalle.Fecha.ToShortDateString()}");
                    Console.WriteLine($"Cantidad: {detalle.Cantidad}");
                    Console.WriteLine($"Valor unitario: {detalle.Valor:C}");
                    Console.WriteLine($"Total: {(detalle.Cantidad * detalle.Valor):C}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    var productoId = UIHelper.SolicitarEntrada("Nuevo ID del producto", detalle.ProductoId);
                    
                    // Verificar que el producto exista
                    var producto = _context.Productos.Find(productoId);
                    if (producto == null)
                    {
                        UIHelper.MostrarError($"El producto con ID {productoId} no existe. Debe crear el producto primero.");
                        return;
                    }
                    
                    var compraIdStr = UIHelper.SolicitarEntrada("Nuevo ID de la compra", detalle.CompraId.ToString());
                    var compraId = int.Parse(compraIdStr);
                    
                    // Verificar que la compra exista
                    var compra = _context.Compras.Find(compraId);
                    if (compra == null)
                    {
                        UIHelper.MostrarError($"La compra con ID {compraId} no existe. Debe crear la compra primero.");
                        return;
                    }
                    
                    // Si cambian el producto o la compra, verificar que no exista otro detalle con la misma combinación
                    if (productoId != detalle.ProductoId || compraId != detalle.CompraId)
                    {
                        var detalleDuplicado = _context.DetalleCompras.FirstOrDefault(d => 
                            d.CompraId == compraId && 
                            d.ProductoId == productoId && 
                            d.Id != id);
                            
                        if (detalleDuplicado != null)
                        {
                            UIHelper.MostrarError($"Ya existe un detalle para la compra {compraId} y el producto {productoId}.");
                            return;
                        }
                    }
                    
                    var fechaStr = UIHelper.SolicitarEntrada("Nueva fecha (YYYY-MM-DD)", detalle.Fecha.ToString("yyyy-MM-dd"));
                    var cantidadStr = UIHelper.SolicitarEntrada("Nueva cantidad", detalle.Cantidad.ToString());
                    var valorStr = UIHelper.SolicitarEntrada("Nuevo valor unitario", detalle.Valor.ToString());
                    
                    detalle.ProductoId = productoId;
                    detalle.CompraId = compraId;
                    detalle.Fecha = DateTime.Parse(fechaStr);
                    detalle.Cantidad = int.Parse(cantidadStr);
                    detalle.Valor = decimal.Parse(valorStr);

                    // Mostrar resumen de cambios antes de confirmar
                    UIHelper.MostrarTitulo("Resumen de Cambios");
                    Console.WriteLine($"ID: {detalle.Id}");
                    Console.WriteLine($"Producto: {detalle.ProductoId}");
                    Console.WriteLine($"Compra: {detalle.CompraId}");
                    Console.WriteLine($"Fecha: {detalle.Fecha.ToShortDateString()}");
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
            try
            {
                Console.Write("Ingrese el ID del detalle a eliminar: ");
                var id = int.Parse(Console.ReadLine());
                var detalle = _context.DetalleCompras.Find(id);

                if (detalle != null)
                {
                    _context.DetalleCompras.Remove(detalle);
                    _context.SaveChanges();

                    Console.WriteLine("Detalle eliminado exitosamente.");
                }
                else
                {
                    Console.WriteLine("Detalle no encontrado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el detalle: {ex.Message}");
            }
        }
    }
}