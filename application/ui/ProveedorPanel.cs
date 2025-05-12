using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;
using sgi_app.infrastructure.repositories;

namespace sgi_app.application.ui
{
    public class ProveedorPanel
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly YourDbContext _context;

        public ProveedorPanel(IProveedorRepository proveedorRepository, YourDbContext context)
        {
            _proveedorRepository = proveedorRepository;
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.MostrarTitulo("Panel de Proveedores");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Proveedores" },
                    { "2", "Crear Nuevo Proveedor" },
                    { "3", "Editar Proveedor" },
                    { "4", "Eliminar Proveedor" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

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
                    case "0":
                        return;
                    default:
                        UIHelper.MostrarAdvertencia("Opción no válida. Intente de nuevo.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ListarProveedores(string titulo = "Listado de Proveedores")
        {
            UIHelper.MostrarTitulo(titulo);
            
            try
            {
                var proveedores = _proveedorRepository.GetAll().ToList();
                
                if (!proveedores.Any())
                {
                    UIHelper.MostrarAdvertencia("No hay proveedores registrados.");
                    Console.ReadKey();
                    return;
                }
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<Proveedor, object>>
                {
                    { "ID", p => p.Id },
                    { "Tercero", p => p.TerceroId },
                    { "Descuento", p => $"{p.Dcto:P2}" },
                    { "Día de Pago", p => p.DiaPago },
                    { "Datos Tercero", p => ObtenerDatosTercero(p.TerceroId) }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(proveedores, columnas, titulo);
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar los proveedores", ex);
            }
        }
        
        // Método auxiliar para obtener datos del tercero
        private string ObtenerDatosTercero(string terceroId)
        {
            try
            {
                var tercero = _context.Terceros.Find(terceroId);
                if (tercero == null)
                    return "No encontrado";
                
                return $"{tercero.Nombre} {tercero.Apellidos}";
            }
            catch
            {
                return "Error";
            }
        }

        private void CrearProveedor()
        {
            UIHelper.MostrarTitulo("Crear Nuevo Proveedor");
            
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
                
                // Definir las columnas para mostrar los terceros
                var columnasTerceros = new Dictionary<string, Func<Terceros, object>>
                {
                    { "ID", t => t.Id },
                    { "Nombre", t => t.Nombre },
                    { "Apellidos", t => t.Apellidos },
                    { "Email", t => t.Email ?? "No registrado" }
                };
                
                // Mostrar la tabla de terceros
                UIHelper.DibujarTabla(terceros, columnasTerceros, "Terceros Disponibles");
                
                var terceroId = UIHelper.SolicitarEntrada("Ingrese el ID del tercero para asociar al proveedor");
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
                
                // Verificar que no exista ya un proveedor con este tercero
                var proveedorExistente = _proveedorRepository.GetAll().FirstOrDefault(p => p.TerceroId == terceroId);
                if (proveedorExistente != null)
                {
                    UIHelper.MostrarError($"Ya existe un proveedor asociado al tercero con ID {terceroId}.");
                    return;
                }
                
                var dctoStr = UIHelper.SolicitarEntrada("Ingrese el descuento (0-1)");
                var dcto = double.Parse(dctoStr);
                
                if (dcto < 0 || dcto > 1)
                {
                    UIHelper.MostrarError("El descuento debe ser un valor entre 0 y 1.");
                    return;
                }
                
                var diaPagoStr = UIHelper.SolicitarEntrada("Ingrese el día de pago (1-31)");
                var diaPago = int.Parse(diaPagoStr);
                
                if (diaPago < 1 || diaPago > 31)
                {
                    UIHelper.MostrarError("El día de pago debe ser un valor entre 1 y 31.");
                    return;
                }

                var proveedor = new Proveedor { 
                    TerceroId = terceroId, 
                    Dcto = dcto, 
                    DiaPago = diaPago 
                };
                
                // Mostrar resumen antes de confirmar
                UIHelper.MostrarTitulo("Resumen del Proveedor");
                Console.WriteLine($"Tercero: {tercero.Nombre} {tercero.Apellidos} ({tercero.Id})");
                Console.WriteLine($"Descuento: {proveedor.Dcto:P2}");
                Console.WriteLine($"Día de Pago: {proveedor.DiaPago}");
                
                if (UIHelper.Confirmar("¿Desea guardar este proveedor?"))
                {
                    _proveedorRepository.Add(proveedor);
                    UIHelper.MostrarExito("Proveedor creado exitosamente.");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear el proveedor", ex);
            }
        }

        private void EditarProveedor()
        {
            UIHelper.MostrarTitulo("Editar Proveedor");
            
            try
            {
                // Mostrar lista de proveedores disponibles
                ListarProveedores("Proveedores Disponibles para Editar");
                
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del proveedor a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("El ID es obligatorio.");
                    return;
                }

                var id = int.Parse(idStr);
                var proveedor = _proveedorRepository.GetById(id);

                if (proveedor != null)
                {
                    var tercero = _context.Terceros.Find(proveedor.TerceroId);
                    
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {proveedor.Id}");
                    Console.WriteLine($"Tercero: {tercero?.Nombre} {tercero?.Apellidos} ({proveedor.TerceroId})");
                    Console.WriteLine($"Descuento: {proveedor.Dcto:P2}");
                    Console.WriteLine($"Día de Pago: {proveedor.DiaPago}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");

                    var dctoStr = UIHelper.SolicitarEntrada("Nuevo descuento (0-1)", proveedor.Dcto.ToString());
                    var dcto = double.Parse(dctoStr);
                    
                    if (dcto < 0 || dcto > 1)
                    {
                        UIHelper.MostrarError("El descuento debe ser un valor entre 0 y 1.");
                        return;
                    }
                    
                    var diaPagoStr = UIHelper.SolicitarEntrada("Nuevo día de pago (1-31)", proveedor.DiaPago.ToString());
                    var diaPago = int.Parse(diaPagoStr);
                    
                    if (diaPago < 1 || diaPago > 31)
                    {
                        UIHelper.MostrarError("El día de pago debe ser un valor entre 1 y 31.");
                        return;
                    }

                    proveedor.Dcto = dcto;
                    proveedor.DiaPago = diaPago;

                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _proveedorRepository.Update(proveedor);
                        UIHelper.MostrarExito("Proveedor actualizado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Proveedor no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar el proveedor", ex);
            }
        }

        private void EliminarProveedor()
        {
            UIHelper.MostrarTitulo("Eliminar Proveedor");
            
            try
            {
                // Mostrar lista de proveedores
                ListarProveedores();
                
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del proveedor a eliminar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("El ID es obligatorio.");
                    return;
                }

                var id = int.Parse(idStr);
                var proveedor = _proveedorRepository.GetById(id);

                if (proveedor != null)
                {
                    var tercero = _context.Terceros.Find(proveedor.TerceroId);
                    
                    UIHelper.MostrarTitulo("Información del Proveedor a Eliminar");
                    Console.WriteLine($"ID: {proveedor.Id}");
                    Console.WriteLine($"Tercero: {tercero?.Nombre} {tercero?.Apellidos} ({proveedor.TerceroId})");
                    Console.WriteLine($"Descuento: {proveedor.Dcto:P2}");
                    Console.WriteLine($"Día de Pago: {proveedor.DiaPago}");

                    if (UIHelper.Confirmar("¿Está seguro que desea eliminar este proveedor?"))
                    {
                        _proveedorRepository.Delete(id);
                        UIHelper.MostrarExito("Proveedor eliminado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Proveedor no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar el proveedor", ex);
            }
        }
    }
}