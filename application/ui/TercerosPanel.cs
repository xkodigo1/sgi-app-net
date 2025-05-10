using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class TercerosPanel
    {
        private readonly YourDbContext _context;

        public TercerosPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.MostrarTitulo("Panel de Terceros");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Terceros" },
                    { "2", "Crear Nuevo Tercero" },
                    { "3", "Editar Tercero" },
                    { "4", "Eliminar Tercero" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarTerceros();
                        break;
                    case "2":
                        CrearTercero();
                        break;
                    case "3":
                        EditarTercero();
                        break;
                    case "4":
                        EliminarTercero();
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

        private void ListarTerceros()
        {
            UIHelper.MostrarTitulo("Listado de Terceros");
            
            try
            {
                var terceros = _context.Terceros.ToList();
                
                // Definir las columnas y los valores a mostrar
                var columnas = new Dictionary<string, Func<Terceros, object>>
                {
                    { "ID", t => t.Id },
                    { "Nombre", t => t.Nombre },
                    { "Apellidos", t => t.Apellidos },
                    { "Email", t => t.Email ?? "No registrado" },
                    { "Tipo Doc.", t => ObtenerNombreTipoDocumento(t.TipoDocId) },
                    { "Tipo Tercero", t => ObtenerNombreTipoTercero(t.TipoTerceroId) },
                    { "Ciudad", t => ObtenerNombreCiudad(t.CiudadId) }
                };
                
                // Usar el método DibujarTabla para mostrar los datos formateados
                UIHelper.DibujarTabla(terceros, columnas, "Registro de Terceros");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar los terceros", ex);
            }
        }
        
        // Métodos auxiliares para obtener nombres de referencias
        private string ObtenerNombreTipoDocumento(int tipoDocId)
        {
            try
            {
                var tipoDoc = _context.TipoDocumentos.Find(tipoDocId);
                return tipoDoc?.Descripcion ?? "Desconocido";
            }
            catch
            {
                return "Error";
            }
        }
        
        private string ObtenerNombreTipoTercero(int tipoTerceroId)
        {
            try
            {
                var tipoTercero = _context.TipoTerceros.Find(tipoTerceroId);
                return tipoTercero?.Descripcion ?? "Desconocido";
            }
            catch
            {
                return "Error";
            }
        }
        
        private string ObtenerNombreCiudad(int ciudadId)
        {
            try
            {
                var ciudad = _context.Ciudades.Find(ciudadId);
                return ciudad?.Nombre ?? "Desconocida";
            }
            catch
            {
                return "Error";
            }
        }

        private void CrearTercero()
        {
            UIHelper.MostrarTitulo("Crear Nuevo Tercero");
            
            try
            {
                var id = UIHelper.SolicitarEntrada("Ingrese el ID del tercero");
                if (string.IsNullOrWhiteSpace(id))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                // Verificar que no exista ya un tercero con este ID
                var terceroExistente = _context.Terceros.Find(id);
                if (terceroExistente != null)
                {
                    UIHelper.MostrarError($"Ya existe un tercero con el ID {id}.");
                    return;
                }
                
                var nombre = UIHelper.SolicitarEntrada("Ingrese el nombre");
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El nombre es obligatorio.");
                    return;
                }
                
                var apellidos = UIHelper.SolicitarEntrada("Ingrese los apellidos");
                if (string.IsNullOrWhiteSpace(apellidos))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. Los apellidos son obligatorios.");
                    return;
                }
                
                var email = UIHelper.SolicitarEntrada("Ingrese el email (opcional)");
                
                // Listar los tipos de documento disponibles
                UIHelper.MostrarTitulo("Tipos de Documento Disponibles");
                var tiposDoc = _context.TipoDocumentos.ToList();
                foreach (var tipo in tiposDoc)
                {
                    Console.WriteLine($"{tipo.Id} - {tipo.Descripcion}");
                }
                
                var tipoDocIdStr = UIHelper.SolicitarEntrada("Ingrese el ID del tipo de documento");
                var tipoDocId = int.Parse(tipoDocIdStr);
                
                // Verificar que el tipo de documento exista
                var tipoDoc = _context.TipoDocumentos.Find(tipoDocId);
                if (tipoDoc == null)
                {
                    UIHelper.MostrarError($"El tipo de documento con ID {tipoDocId} no existe.");
                    return;
                }
                
                // Listar los tipos de tercero disponibles
                UIHelper.MostrarTitulo("Tipos de Tercero Disponibles");
                var tiposTercero = _context.TipoTerceros.ToList();
                foreach (var tipo in tiposTercero)
                {
                    Console.WriteLine($"{tipo.Id} - {tipo.Descripcion}");
                }
                
                var tipoTerceroIdStr = UIHelper.SolicitarEntrada("Ingrese el ID del tipo de tercero");
                var tipoTerceroId = int.Parse(tipoTerceroIdStr);
                
                // Verificar que el tipo de tercero exista
                var tipoTercero = _context.TipoTerceros.Find(tipoTerceroId);
                if (tipoTercero == null)
                {
                    UIHelper.MostrarError($"El tipo de tercero con ID {tipoTerceroId} no existe.");
                    return;
                }
                
                // Listar las ciudades disponibles
                UIHelper.MostrarTitulo("Ciudades Disponibles");
                var ciudades = _context.Ciudades.ToList();
                foreach (var c in ciudades)
                {
                    Console.WriteLine($"{c.Id} - {c.Nombre}");
                }
                
                var ciudadIdStr = UIHelper.SolicitarEntrada("Ingrese el ID de la ciudad");
                var ciudadId = int.Parse(ciudadIdStr);
                
                // Verificar que la ciudad exista
                var ciudadSeleccionada = _context.Ciudades.Find(ciudadId);
                if (ciudadSeleccionada == null)
                {
                    UIHelper.MostrarError($"La ciudad con ID {ciudadId} no existe.");
                    return;
                }

                var tercero = new Terceros { 
                    Id = id, 
                    Nombre = nombre, 
                    Apellidos = apellidos, 
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    TipoDocId = tipoDocId,
                    TipoTerceroId = tipoTerceroId,
                    CiudadId = ciudadId
                };
                
                // Mostrar resumen antes de confirmar
                UIHelper.MostrarTitulo("Resumen del Tercero");
                Console.WriteLine($"ID: {tercero.Id}");
                Console.WriteLine($"Nombre: {tercero.Nombre}");
                Console.WriteLine($"Apellidos: {tercero.Apellidos}");
                Console.WriteLine($"Email: {tercero.Email ?? "No registrado"}");
                Console.WriteLine($"Tipo Doc.: {ObtenerNombreTipoDocumento(tercero.TipoDocId)}");
                Console.WriteLine($"Tipo Tercero: {ObtenerNombreTipoTercero(tercero.TipoTerceroId)}");
                Console.WriteLine($"Ciudad: {ObtenerNombreCiudad(tercero.CiudadId)}");
                
                if (UIHelper.Confirmar("¿Desea guardar este tercero?"))
                {
                    _context.Terceros.Add(tercero);
                    _context.SaveChanges();
                    UIHelper.MostrarExito("Tercero creado exitosamente.");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear el tercero", ex);
            }
        }

        private void EditarTercero()
        {
            UIHelper.MostrarTitulo("Editar Tercero");
            
            try
            {
                var id = UIHelper.SolicitarEntrada("Ingrese el ID del tercero a editar");
                if (string.IsNullOrWhiteSpace(id))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var tercero = _context.Terceros.Find(id);

                if (tercero != null)
                {
                    // Mostrar información actual
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {tercero.Id}");
                    Console.WriteLine($"Nombre: {tercero.Nombre}");
                    Console.WriteLine($"Apellidos: {tercero.Apellidos}");
                    Console.WriteLine($"Email: {tercero.Email ?? "No registrado"}");
                    Console.WriteLine($"Tipo Doc.: {ObtenerNombreTipoDocumento(tercero.TipoDocId)}");
                    Console.WriteLine($"Tipo Tercero: {ObtenerNombreTipoTercero(tercero.TipoTerceroId)}");
                    Console.WriteLine($"Ciudad: {ObtenerNombreCiudad(tercero.CiudadId)}");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");
                    
                    var nombre = UIHelper.SolicitarEntrada("Nuevo nombre", tercero.Nombre);
                    var apellidos = UIHelper.SolicitarEntrada("Nuevos apellidos", tercero.Apellidos);
                    var email = UIHelper.SolicitarEntrada("Nuevo email", tercero.Email ?? "");
                    
                    // Listar los tipos de documento disponibles
                    UIHelper.MostrarTitulo("Tipos de Documento Disponibles");
                    var tiposDoc = _context.TipoDocumentos.ToList();
                    foreach (var tipo in tiposDoc)
                    {
                        Console.WriteLine($"{tipo.Id} - {tipo.Descripcion}");
                    }
                    
                    var tipoDocIdStr = UIHelper.SolicitarEntrada("Nuevo ID del tipo de documento", tercero.TipoDocId.ToString());
                    var tipoDocId = int.Parse(tipoDocIdStr);
                    
                    // Verificar que el tipo de documento exista
                    var tipoDoc = _context.TipoDocumentos.Find(tipoDocId);
                    if (tipoDoc == null)
                    {
                        UIHelper.MostrarError($"El tipo de documento con ID {tipoDocId} no existe.");
                        return;
                    }
                    
                    // Listar los tipos de tercero disponibles
                    UIHelper.MostrarTitulo("Tipos de Tercero Disponibles");
                    var tiposTercero = _context.TipoTerceros.ToList();
                    foreach (var tipo in tiposTercero)
                    {
                        Console.WriteLine($"{tipo.Id} - {tipo.Descripcion}");
                    }
                    
                    var tipoTerceroIdStr = UIHelper.SolicitarEntrada("Nuevo ID del tipo de tercero", tercero.TipoTerceroId.ToString());
                    var tipoTerceroId = int.Parse(tipoTerceroIdStr);
                    
                    // Verificar que el tipo de tercero exista
                    var tipoTercero = _context.TipoTerceros.Find(tipoTerceroId);
                    if (tipoTercero == null)
                    {
                        UIHelper.MostrarError($"El tipo de tercero con ID {tipoTerceroId} no existe.");
                        return;
                    }
                    
                    // Listar las ciudades disponibles
                    UIHelper.MostrarTitulo("Ciudades Disponibles");
                    var ciudades = _context.Ciudades.ToList();
                    foreach (var c in ciudades)
                    {
                        Console.WriteLine($"{c.Id} - {c.Nombre}");
                    }
                    
                    var ciudadIdStr = UIHelper.SolicitarEntrada("Nuevo ID de la ciudad", tercero.CiudadId.ToString());
                    var ciudadId = int.Parse(ciudadIdStr);
                    
                    // Verificar que la ciudad exista
                    var ciudadSeleccionada = _context.Ciudades.Find(ciudadId);
                    if (ciudadSeleccionada == null)
                    {
                        UIHelper.MostrarError($"La ciudad con ID {ciudadId} no existe.");
                        return;
                    }
                    
                    tercero.Nombre = nombre;
                    tercero.Apellidos = apellidos;
                    tercero.Email = string.IsNullOrWhiteSpace(email) ? null : email;
                    tercero.TipoDocId = tipoDocId;
                    tercero.TipoTerceroId = tipoTerceroId;
                    tercero.CiudadId = ciudadId;

                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(tercero);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Tercero actualizado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Tercero no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar el tercero", ex);
            }
        }

        private void EliminarTercero()
        {
            UIHelper.MostrarTitulo("Eliminar Tercero");
            
            try
            {
                var id = UIHelper.SolicitarEntrada("Ingrese el ID del tercero a eliminar");
                if (string.IsNullOrWhiteSpace(id))
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada. El ID es obligatorio.");
                    return;
                }
                
                var tercero = _context.Terceros.Find(id);

                if (tercero != null)
                {
                    // Verificar si existen entidades relacionadas
                    bool tieneRelaciones = false;
                    
                    // Verificar clientes
                    var cliente = _context.Clientes.FirstOrDefault(c => c.TerceroId == id);
                    if (cliente != null)
                    {
                        UIHelper.MostrarAdvertencia($"Este tercero está asociado a un cliente (ID: {cliente.Id}).");
                        tieneRelaciones = true;
                    }
                    
                    // Verificar ventas
                    var ventasVendedor = _context.Ventas.Where(v => v.TerceroEnId == id).ToList();
                    if (ventasVendedor.Any())
                    {
                        UIHelper.MostrarAdvertencia($"Este tercero está asociado a {ventasVendedor.Count} ventas como vendedor.");
                        tieneRelaciones = true;
                    }
                    
                    var ventasCliente = _context.Ventas.Where(v => v.TerceroCliId == id).ToList();
                    if (ventasCliente.Any())
                    {
                        UIHelper.MostrarAdvertencia($"Este tercero está asociado a {ventasCliente.Count} ventas como cliente.");
                        tieneRelaciones = true;
                    }
                    
                    // Verificar compras
                    var comprasProveedor = _context.Compras.Where(c => c.TerceroProvId == id).ToList();
                    if (comprasProveedor.Any())
                    {
                        UIHelper.MostrarAdvertencia($"Este tercero está asociado a {comprasProveedor.Count} compras como proveedor.");
                        tieneRelaciones = true;
                    }
                    
                    var comprasEmpleado = _context.Compras.Where(c => c.TerceroEmpId == id).ToList();
                    if (comprasEmpleado.Any())
                    {
                        UIHelper.MostrarAdvertencia($"Este tercero está asociado a {comprasEmpleado.Count} compras como empleado.");
                        tieneRelaciones = true;
                    }
                    
                    if (tieneRelaciones)
                    {
                        if (!UIHelper.Confirmar("⚠️ ADVERTENCIA: Eliminar este tercero afectará múltiples registros en el sistema. ¿Está ABSOLUTAMENTE seguro de continuar?"))
                        {
                            UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                            return;
                        }
                    }
                    
                    // Mostrar información a eliminar
                    UIHelper.MostrarTitulo("Información del Tercero a Eliminar");
                    Console.WriteLine($"ID: {tercero.Id}");
                    Console.WriteLine($"Nombre: {tercero.Nombre}");
                    Console.WriteLine($"Apellidos: {tercero.Apellidos}");
                    Console.WriteLine($"Email: {tercero.Email ?? "No registrado"}");
                    
                    if (UIHelper.Confirmar("¿Está seguro que desea eliminar este tercero?"))
                    {
                        _context.Terceros.Remove(tercero);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Tercero eliminado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Tercero no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar el tercero", ex);
            }
        }
    }
}