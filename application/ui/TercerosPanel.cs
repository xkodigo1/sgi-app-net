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
                UIHelper.ShowTitle("Third Parties Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Third Parties" },
                    { "2", "Create New Third Party" },
                    { "3", "Edit Third Party" },
                    { "4", "Delete Third Party" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListThirdParties();
                        break;
                    case "2":
                        CreateThirdParty();
                        break;
                    case "3":
                        UpdateThirdParty();
                        break;
                    case "4":
                        DeleteThirdParty();
                        break;
                    case "0":
                        return;
                    default:
                        UIHelper.ShowWarning("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ListThirdParties()
        {
            UIHelper.ShowTitle("Third Parties List");
            
            try
            {
                var terceros = _context.Terceros.ToList();
                
                // Define columns and values to display
                var columns = new Dictionary<string, Func<Terceros, object>>
                {
                    { "ID", t => t.Id },
                    { "First Name", t => t.Nombre },
                    { "Last Name", t => t.Apellidos },
                    { "Email", t => t.Email ?? "Not registered" },
                    { "Doc. Type", t => GetDocumentTypeName(t.TipoDocId) },
                    { "Party Type", t => GetThirdPartyTypeName(t.TipoTerceroId) },
                    { "City", t => GetCityName(t.CiudadId) }
                };
                
                // Use DrawTable method to show formatted data
                UIHelper.DrawTable(terceros, columns, "Third Parties Registry");
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing third parties", ex);
            }
        }
        
        // Helper methods to get reference names
        private string GetDocumentTypeName(int tipoDocId)
        {
            try
            {
                var tipoDoc = _context.TipoDocumentos.Find(tipoDocId);
                return tipoDoc?.Descripcion ?? "Unknown";
            }
            catch
            {
                return "Error";
            }
        }
        
        private string GetThirdPartyTypeName(int tipoTerceroId)
        {
            try
            {
                var tipoTercero = _context.TipoTerceros.Find(tipoTerceroId);
                return tipoTercero?.Descripcion ?? "Unknown";
            }
            catch
            {
                return "Error";
            }
        }
        
        private string GetCityName(int ciudadId)
        {
            try
            {
                var ciudad = _context.Ciudades.Find(ciudadId);
                return ciudad?.Nombre ?? "Unknown";
            }
            catch
            {
                return "Error";
            }
        }

        private void CreateThirdParty()
        {
            UIHelper.ShowTitle("Create New Third Party");
            
            try
            {
                var id = UIHelper.RequestInput("Enter third party ID");
                if (string.IsNullOrWhiteSpace(id))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                // Verify that a third party with this ID doesn't already exist
                var terceroExistente = _context.Terceros.Find(id);
                if (terceroExistente != null)
                {
                    UIHelper.ShowError($"A third party with ID {id} already exists.");
                    return;
                }
                
                var nombre = UIHelper.RequestInput("Enter first name");
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    UIHelper.ShowWarning("Operation cancelled. First name is required.");
                    return;
                }
                
                var apellidos = UIHelper.RequestInput("Enter last name");
                if (string.IsNullOrWhiteSpace(apellidos))
                {
                    UIHelper.ShowWarning("Operation cancelled. Last name is required.");
                    return;
                }
                
                var email = UIHelper.RequestInput("Enter email (optional)");
                
                // List available document types
                UIHelper.ShowTitle("Available Document Types");
                var tiposDoc = _context.TipoDocumentos.ToList();
                foreach (var tipo in tiposDoc)
                {
                    Console.WriteLine($"{tipo.Id} - {tipo.Descripcion}");
                }
                
                var tipoDocIdStr = UIHelper.RequestInput("Enter document type ID");
                var tipoDocId = int.Parse(tipoDocIdStr);
                
                // Verify document type exists
                var tipoDoc = _context.TipoDocumentos.Find(tipoDocId);
                if (tipoDoc == null)
                {
                    UIHelper.ShowError($"Document type with ID {tipoDocId} does not exist.");
                    return;
                }
                
                // List available third party types
                UIHelper.ShowTitle("Available Third Party Types");
                var tiposTercero = _context.TipoTerceros.ToList();
                foreach (var tipo in tiposTercero)
                {
                    Console.WriteLine($"{tipo.Id} - {tipo.Descripcion}");
                }
                
                var tipoTerceroIdStr = UIHelper.RequestInput("Enter third party type ID");
                var tipoTerceroId = int.Parse(tipoTerceroIdStr);
                
                // Verify third party type exists
                var tipoTercero = _context.TipoTerceros.Find(tipoTerceroId);
                if (tipoTercero == null)
                {
                    UIHelper.ShowError($"Third party type with ID {tipoTerceroId} does not exist.");
                    return;
                }
                
                // List available cities
                UIHelper.ShowTitle("Available Cities");
                var ciudades = _context.Ciudades.ToList();
                foreach (var ciudadItem in ciudades)
                {
                    Console.WriteLine($"{ciudadItem.Id} - {ciudadItem.Nombre}");
                }
                
                var ciudadIdStr = UIHelper.RequestInput("Enter city ID");
                var ciudadId = int.Parse(ciudadIdStr);
                
                // Verify city exists
                var ciudad = _context.Ciudades.Find(ciudadId);
                if (ciudad == null)
                {
                    UIHelper.ShowError($"City with ID {ciudadId} does not exist.");
                    return;
                }
                
                var telefono = UIHelper.RequestInput("Enter phone number");
                var direccion = UIHelper.RequestInput("Enter address");

                var tercero = new Terceros 
                { 
                    Id = id, 
                    Nombre = nombre, 
                    Apellidos = apellidos, 
                    Email = email,
                    TipoDocId = tipoDocId,
                    TipoTerceroId = tipoTerceroId,
                    CiudadId = ciudadId,
                };
                
                // Show summary before confirming
                UIHelper.ShowTitle("Third Party Summary");
                Console.WriteLine($"ID: {tercero.Id}");
                Console.WriteLine($"Name: {tercero.Nombre} {tercero.Apellidos}");
                Console.WriteLine($"Email: {tercero.Email ?? "Not provided"}");
                Console.WriteLine($"Document Type: {GetDocumentTypeName(tercero.TipoDocId)}");
                Console.WriteLine($"Third Party Type: {GetThirdPartyTypeName(tercero.TipoTerceroId)}");
                Console.WriteLine($"City: {GetCityName(tercero.CiudadId)}");
                
                if (UIHelper.Confirm("Do you want to save this third party?"))
                {
                    _context.Terceros.Add(tercero);
                    _context.SaveChanges();
                    UIHelper.ShowSuccess("Third party created successfully.");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating third party", ex);
            }
        }

        private void UpdateThirdParty()
        {
            UIHelper.ShowTitle("Edit Third Party");
            
            try
            {
                var id = UIHelper.RequestInput("Enter ID of the third party to edit");
                if (string.IsNullOrWhiteSpace(id))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var tercero = _context.Terceros.Find(id);

                if (tercero != null)
                {
                    // Show current information
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {tercero.Id}");
                    Console.WriteLine($"First Name: {tercero.Nombre}");
                    Console.WriteLine($"Last Name: {tercero.Apellidos}");
                    Console.WriteLine($"Email: {tercero.Email ?? "Not provided"}");
                    Console.WriteLine($"Document Type: {GetDocumentTypeName(tercero.TipoDocId)}");
                    Console.WriteLine($"Third Party Type: {GetThirdPartyTypeName(tercero.TipoTerceroId)}");
                    Console.WriteLine($"City: {GetCityName(tercero.CiudadId)}");
                    Console.WriteLine("\nEnter new values or leave blank to keep current:");
                    
                    var nombre = UIHelper.RequestInput("New first name", tercero.Nombre);
                    var apellidos = UIHelper.RequestInput("New last name", tercero.Apellidos);
                    var email = UIHelper.RequestInput("New email", tercero.Email ?? "");
                    
                    // List available document types
                    UIHelper.ShowTitle("Available Document Types");
                    var tiposDoc = _context.TipoDocumentos.ToList();
                    foreach (var tipo in tiposDoc)
                    {
                        Console.WriteLine($"{tipo.Id} - {tipo.Descripcion}");
                    }
                    
                    var tipoDocIdStr = UIHelper.RequestInput("New document type ID", tercero.TipoDocId.ToString());
                    var tipoDocId = int.Parse(tipoDocIdStr);
                    
                    // Verify document type exists
                    var tipoDoc = _context.TipoDocumentos.Find(tipoDocId);
                    if (tipoDoc == null)
                    {
                        UIHelper.ShowError($"Document type with ID {tipoDocId} does not exist.");
                        return;
                    }
                    
                    // List available third party types
                    UIHelper.ShowTitle("Available Third Party Types");
                    var tiposTercero = _context.TipoTerceros.ToList();
                    foreach (var tipo in tiposTercero)
                    {
                        Console.WriteLine($"{tipo.Id} - {tipo.Descripcion}");
                    }
                    
                    var tipoTerceroIdStr = UIHelper.RequestInput("New third party type ID", tercero.TipoTerceroId.ToString());
                    var tipoTerceroId = int.Parse(tipoTerceroIdStr);
                    
                    // Verify third party type exists
                    var tipoTercero = _context.TipoTerceros.Find(tipoTerceroId);
                    if (tipoTercero == null)
                    {
                        UIHelper.ShowError($"Third party type with ID {tipoTerceroId} does not exist.");
                        return;
                    }
                    
                    // List available cities
                    UIHelper.ShowTitle("Available Cities");
                    var ciudades = _context.Ciudades.ToList();
                    foreach (var ciudadItem in ciudades)
                    {
                        Console.WriteLine($"{ciudadItem.Id} - {ciudadItem.Nombre}");
                    }
                    
                    var ciudadIdStr = UIHelper.RequestInput("New city ID", tercero.CiudadId.ToString());
                    var ciudadId = int.Parse(ciudadIdStr);
                    
                    // Verify city exists
                    var ciudad = _context.Ciudades.Find(ciudadId);
                    if (ciudad == null)
                    {
                        UIHelper.ShowError($"City with ID {ciudadId} does not exist.");
                        return;
                    }
                    
                    
                    tercero.Nombre = nombre;
                    tercero.Apellidos = apellidos;
                    tercero.Email = string.IsNullOrWhiteSpace(email) ? null : email;
                    tercero.TipoDocId = tipoDocId;
                    tercero.TipoTerceroId = tipoTerceroId;
                    tercero.CiudadId = ciudadId;


                    if (UIHelper.Confirm("Do you confirm these changes?"))
                    {
                        _context.Update(tercero);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Third party updated successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Third party not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating third party", ex);
            }
        }

        private void DeleteThirdParty()
        {
            UIHelper.ShowTitle("Delete Third Party");
            
            try
            {
                var id = UIHelper.RequestInput("Enter ID of the third party to delete");
                if (string.IsNullOrWhiteSpace(id))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var tercero = _context.Terceros.Find(id);

                if (tercero != null)
                {
                    // Check if there are any related records
                    var clientes = _context.Clientes.Where(c => c.TerceroId == id).ToList();
                    var empleados = _context.Empleados.Where(e => e.TerceroId == id).ToList();
                    var ventasComoCliente = _context.Ventas.Where(v => v.TerceroCliId == id).ToList();
                    var ventasComoEmpleado = _context.Ventas.Where(v => v.TerceroEnId == id).ToList();
                    var comprasComoProveedor = _context.Compras.Where(c => c.TerceroProvId == id).ToList();
                    var comprasComoEmpleado = _context.Compras.Where(c => c.TerceroEmpId == id).ToList();
                    
                    if (clientes.Any() || empleados.Any() || ventasComoCliente.Any() ||
                        ventasComoEmpleado.Any() || comprasComoProveedor.Any() || comprasComoEmpleado.Any())
                    {
                        UIHelper.ShowWarning("This third party has associated records and cannot be deleted.");
                        UIHelper.ShowWarning($"Associated records: {clientes.Count} clients, {empleados.Count} employees, {ventasComoCliente.Count} sales as client, " +
                                             $"{ventasComoEmpleado.Count} sales as employee, {comprasComoProveedor.Count} purchases as supplier, " +
                                             $"{comprasComoEmpleado.Count} purchases as employee.");
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        return;
                    }
                    
                    // Show information of the third party to delete
                    UIHelper.ShowTitle("Third Party Information to Delete");
                    Console.WriteLine($"ID: {tercero.Id}");
                    Console.WriteLine($"Name: {tercero.Nombre} {tercero.Apellidos}");
                    Console.WriteLine($"Email: {tercero.Email ?? "Not provided"}");
                    Console.WriteLine($"Document Type: {GetDocumentTypeName(tercero.TipoDocId)}");
                    Console.WriteLine($"Third Party Type: {GetThirdPartyTypeName(tercero.TipoTerceroId)}");
                    Console.WriteLine($"City: {GetCityName(tercero.CiudadId)}");
                    
                    if (UIHelper.Confirm("Are you ABSOLUTELY sure you want to delete this third party?"))
                    {
                        _context.Terceros.Remove(tercero);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Third party deleted successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Third party not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting third party", ex);
            }
        }
        
        // Maintain backward compatibility
        private void ListarTerceros() => ListThirdParties();
        private void CrearTercero() => CreateThirdParty();
        private void EditarTercero() => UpdateThirdParty();
        private void EliminarTercero() => DeleteThirdParty();
        private string ObtenerNombreTipoDocumento(int tipoDocId) => GetDocumentTypeName(tipoDocId);
        private string ObtenerNombreTipoTercero(int tipoTerceroId) => GetThirdPartyTypeName(tipoTerceroId);
        private string ObtenerNombreCiudad(int ciudadId) => GetCityName(ciudadId);
    }
}