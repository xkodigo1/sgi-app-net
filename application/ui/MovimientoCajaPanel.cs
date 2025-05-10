using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class MovimientoCajaPanel
    {
        private readonly YourDbContext _context;

        public MovimientoCajaPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.MostrarTitulo("Panel de Movimiento de Caja");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Movimientos" },
                    { "2", "Crear Nuevo Movimiento" },
                    { "3", "Editar Movimiento" },
                    { "4", "Eliminar Movimiento" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarMovimientos();
                        break;
                    case "2":
                        CrearMovimiento();
                        break;
                    case "3":
                        EditarMovimiento();
                        break;
                    case "4":
                        EliminarMovimiento();
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

        private void ListarMovimientos()
        {
            UIHelper.MostrarTitulo("Listado de Movimientos de Caja");
            
            try
            {
                var movimientos = _context.MovCaja.ToList();
                
                if (!movimientos.Any())
                {
                    UIHelper.MostrarAdvertencia("No hay movimientos de caja registrados.");
                    Console.ReadKey();
                    return;
                }
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<MovCaja, object>>
                {
                    { "ID", m => m.Id },
                    { "Fecha", m => m.Fecha.ToShortDateString() },
                    { "Tipo", m => ObtenerTipoMovimiento(m.TipoMovId) },
                    { "Concepto", m => m.Concepto },
                    { "Tercero", m => m.TerceroId },
                    { "Valor", m => $"{m.Valor:C}" }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(movimientos, columnas, "Registro de Movimientos de Caja");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar los movimientos", ex);
            }
        }
        
        // Método auxiliar para obtener el tipo de movimiento
        private string ObtenerTipoMovimiento(int tipoMovId)
        {
            switch (tipoMovId)
            {
                case 1:
                    return "Entrada";
                case 2:
                    return "Salida";
                default:
                    return "Desconocido";
            }
        }

        private void CrearMovimiento()
        {
            UIHelper.MostrarTitulo("Crear Nuevo Movimiento de Caja");
            
            try
            {
                // Listar los terceros disponibles
                UIHelper.MostrarTitulo("Terceros Disponibles");
                var terceros = _context.Terceros.ToList();
                
                if (!terceros.Any())
                {
                    UIHelper.MostrarError("No hay terceros registrados. Debe crear un tercero primero.");
                    return;
                }
                
                foreach (var t in terceros)
                {
                    Console.WriteLine($"{t.Id} - {t.Nombre} {t.Apellidos}");
                }
                
                var terceroId = UIHelper.SolicitarEntrada("Ingrese el ID del tercero");
                if (string.IsNullOrWhiteSpace(terceroId))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID del tercero es obligatorio.");
                    return;
                }
                
                // Verificar que el tercero exista
                var tercero = _context.Terceros.Find(terceroId);
                if (tercero == null)
                {
                    UIHelper.MostrarError($"El tercero con ID {terceroId} no existe. Debe crear el tercero primero.");
                    return;
                }
                
                var concepto = UIHelper.SolicitarEntrada("Ingrese el concepto del movimiento");
                if (string.IsNullOrWhiteSpace(concepto))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El concepto es obligatorio.");
                    return;
                }
                
                // Mostrar opciones de tipo de movimiento
                UIHelper.MostrarTitulo("Tipos de Movimiento");
                Console.WriteLine("1 - Entrada");
                Console.WriteLine("2 - Salida");
                
                var tipoMovIdStr = UIHelper.SolicitarEntrada("Ingrese el tipo de movimiento (1=Entrada, 2=Salida)");
                var tipoMovId = int.Parse(tipoMovIdStr);
                
                if (tipoMovId != 1 && tipoMovId != 2)
                {
                    UIHelper.MostrarError("El tipo de movimiento debe ser 1 (Entrada) o 2 (Salida).");
                    return;
                }
                
                var valorStr = UIHelper.SolicitarEntrada("Ingrese el valor del movimiento");
                var valor = decimal.Parse(valorStr);
                
                if (valor <= 0)
                {
                    UIHelper.MostrarError("El valor debe ser mayor que cero.");
                    return;
                }
                
                var fechaStr = UIHelper.SolicitarEntrada("Ingrese la fecha (YYYY-MM-DD)", DateTime.Now.ToString("yyyy-MM-dd"));
                var fecha = DateTime.Parse(fechaStr);

                var movimiento = new MovCaja { 
                    Concepto = concepto, 
                    TerceroId = terceroId, 
                    TipoMovId = tipoMovId,
                    Valor = valor,
                    Fecha = fecha
                };
                
                // Mostrar resumen antes de confirmar
                UIHelper.MostrarTitulo("Resumen del Movimiento");
                Console.WriteLine($"Concepto: {movimiento.Concepto}");
                Console.WriteLine($"Tercero: {tercero.Nombre} {tercero.Apellidos} ({tercero.Id})");
                Console.WriteLine($"Tipo: {ObtenerTipoMovimiento(movimiento.TipoMovId)}");
                Console.WriteLine($"Valor: {movimiento.Valor:C}");
                Console.WriteLine($"Fecha: {movimiento.Fecha.ToShortDateString()}");
                
                if (UIHelper.Confirmar("¿Desea guardar este movimiento?"))
                {
                    _context.MovCaja.Add(movimiento);
                    _context.SaveChanges();
                    UIHelper.MostrarExito($"Movimiento creado exitosamente con ID: {movimiento.Id}");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear el movimiento", ex);
            }
        }

        private void EditarMovimiento()
        {
            UIHelper.MostrarTitulo("Editar Movimiento de Caja");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del movimiento a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var movimiento = _context.MovCaja.Find(id);

                if (movimiento != null)
                {
                    var tercero = _context.Terceros.Find(movimiento.TerceroId);
                    
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {movimiento.Id}");
                    Console.WriteLine($"Concepto: {movimiento.Concepto}");
                    Console.WriteLine($"Tercero: {tercero?.Nombre} {tercero?.Apellidos} ({movimiento.TerceroId})");
                    Console.WriteLine($"Tipo: {ObtenerTipoMovimiento(movimiento.TipoMovId)}");
                    Console.WriteLine($"Valor: {movimiento.Valor:C}");
                    Console.WriteLine($"Fecha: {movimiento.Fecha.ToShortDateString()}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    var concepto = UIHelper.SolicitarEntrada("Nuevo concepto", movimiento.Concepto);
                    
                    // Listar los terceros disponibles
                    UIHelper.MostrarTitulo("Terceros Disponibles");
                    var terceros = _context.Terceros.ToList();
                    foreach (var t in terceros)
                    {
                        Console.WriteLine($"{t.Id} - {t.Nombre} {t.Apellidos}");
                    }
                    
                    var terceroId = UIHelper.SolicitarEntrada("Nuevo ID del tercero", movimiento.TerceroId);
                    
                    // Verificar que el tercero exista
                    var nuevoTercero = _context.Terceros.Find(terceroId);
                    if (nuevoTercero == null)
                    {
                        UIHelper.MostrarError($"El tercero con ID {terceroId} no existe. Debe crear el tercero primero.");
                        return;
                    }
                    
                    // Mostrar opciones de tipo de movimiento
                    UIHelper.MostrarTitulo("Tipos de Movimiento");
                    Console.WriteLine("1 - Entrada");
                    Console.WriteLine("2 - Salida");
                    
                    var tipoMovIdStr = UIHelper.SolicitarEntrada("Nuevo tipo de movimiento (1=Entrada, 2=Salida)", movimiento.TipoMovId.ToString());
                    var tipoMovId = int.Parse(tipoMovIdStr);
                    
                    if (tipoMovId != 1 && tipoMovId != 2)
                    {
                        UIHelper.MostrarError("El tipo de movimiento debe ser 1 (Entrada) o 2 (Salida).");
                        return;
                    }
                    
                    var valorStr = UIHelper.SolicitarEntrada("Nuevo valor", movimiento.Valor.ToString());
                    var valor = decimal.Parse(valorStr);
                    
                    if (valor <= 0)
                    {
                        UIHelper.MostrarError("El valor debe ser mayor que cero.");
                        return;
                    }
                    
                    var fechaStr = UIHelper.SolicitarEntrada("Nueva fecha (YYYY-MM-DD)", movimiento.Fecha.ToString("yyyy-MM-dd"));
                    var fecha = DateTime.Parse(fechaStr);
                    
                    movimiento.Concepto = concepto;
                    movimiento.TerceroId = terceroId;
                    movimiento.TipoMovId = tipoMovId;
                    movimiento.Valor = valor;
                    movimiento.Fecha = fecha;

                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(movimiento);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Movimiento actualizado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Movimiento no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar el movimiento", ex);
            }
        }

        private void EliminarMovimiento()
        {
            UIHelper.MostrarTitulo("Eliminar Movimiento de Caja");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del movimiento a eliminar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var movimiento = _context.MovCaja.Find(id);

                if (movimiento != null)
                {
                    var tercero = _context.Terceros.Find(movimiento.TerceroId);
                    
                    // Mostrar información a eliminar
                    UIHelper.MostrarTitulo("Información del Movimiento a Eliminar");
                    Console.WriteLine($"ID: {movimiento.Id}");
                    Console.WriteLine($"Concepto: {movimiento.Concepto}");
                    Console.WriteLine($"Tercero: {tercero?.Nombre} {tercero?.Apellidos} ({movimiento.TerceroId})");
                    Console.WriteLine($"Tipo: {ObtenerTipoMovimiento(movimiento.TipoMovId)}");
                    Console.WriteLine($"Valor: {movimiento.Valor:C}");
                    Console.WriteLine($"Fecha: {movimiento.Fecha.ToShortDateString()}");
                    
                    if (UIHelper.Confirmar("¿Está seguro que desea eliminar este movimiento?"))
                    {
                        _context.MovCaja.Remove(movimiento);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Movimiento eliminado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Movimiento no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar el movimiento", ex);
            }
        }
    }
}