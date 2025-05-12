using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class ClientePanel
    {
        private readonly YourDbContext _context;

        public ClientePanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.ShowTitle("Client Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Clients" },
                    { "2", "Create New Client" },
                    { "3", "Edit Client" },
                    { "4", "Delete Client" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListClients();
                        break;
                    case "2":
                        CreateClient();
                        break;
                    case "3":
                        UpdateClient();
                        break;
                    case "4":
                        DeleteClient();
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

        private void ListClients()
        {
            UIHelper.ShowTitle("Client List");
            
            try
            {
                var clientes = _context.Clientes.ToList();
                
                // Define columns and values to display
                var columns = new Dictionary<string, Func<Cliente, object>>
                {
                    { "ID", c => c.Id },
                    { "Third Party ID", c => c.TerceroId },
                    { "Birth Date", c => c.FechaNac.ToShortDateString() },
                    { "Purchase Date", c => c.FechaCompra.ToShortDateString() }
                };
                
                // Use DrawTable method to show formatted data
                UIHelper.DrawTable(clientes, columns, "Registered Clients");
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing clients", ex);
            }
        }

        private void CreateClient()
        {
            UIHelper.ShowTitle("Create New Client");
            
            try
            {
                // Check if a client with this third party already exists
                var terceroId = UIHelper.RequestInput("Enter third party ID");
                
                // Verify that the third party exists
                var tercero = _context.Terceros.Find(terceroId);
                if (tercero == null)
                {
                    UIHelper.ShowError($"Third party with ID {terceroId} does not exist. You must create the third party first.");
                    return;
                }
                
                // Check that no client is already associated with this third party
                var clienteExistente = _context.Clientes.FirstOrDefault(c => c.TerceroId == terceroId);
                if (clienteExistente != null)
                {
                    UIHelper.ShowError($"A client is already associated with third party ID {terceroId}.");
                    return;
                }
                
                var fechaNacStr = UIHelper.RequestInput("Enter birth date (YYYY-MM-DD)");
                var fechaNac = DateTime.Parse(fechaNacStr);
                
                var fechaCompraStr = UIHelper.RequestInput("Enter purchase date (YYYY-MM-DD)");
                var fechaCompra = DateTime.Parse(fechaCompraStr);

                var cliente = new Cliente { TerceroId = terceroId, FechaNac = fechaNac, FechaCompra = fechaCompra };
                
                if (UIHelper.Confirm("Do you want to save this client?"))
                {
                    _context.Clientes.Add(cliente);
                    _context.SaveChanges();
                    UIHelper.ShowSuccess("Client created successfully.");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating client", ex);
            }
        }

        private void UpdateClient()
        {
            UIHelper.ShowTitle("Edit Client");
            
            try
            {
                var idStr = UIHelper.RequestInput("Enter ID of the client to edit");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var cliente = _context.Clientes.Find(id);

                if (cliente != null)
                {
                    // Show current information
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {cliente.Id}");
                    Console.WriteLine($"Third Party ID: {cliente.TerceroId}");
                    Console.WriteLine($"Birth Date: {cliente.FechaNac.ToShortDateString()}");
                    Console.WriteLine($"Purchase Date: {cliente.FechaCompra.ToShortDateString()}");
                    Console.WriteLine("\nEnter new values or leave blank to keep current:");
                    
                    var terceroId = UIHelper.RequestInput("New third party ID", cliente.TerceroId);
                    
                    // Verify that the third party exists
                    var tercero = _context.Terceros.Find(terceroId);
                    if (tercero == null)
                    {
                        UIHelper.ShowError($"Third party with ID {terceroId} does not exist. You must create the third party first.");
                        return;
                    }
                    
                    // If the third party is different, check it's not associated with another client
                    if (terceroId != cliente.TerceroId)
                    {
                        var clienteExistente = _context.Clientes.FirstOrDefault(c => c.TerceroId == terceroId && c.Id != id);
                        if (clienteExistente != null)
                        {
                            UIHelper.ShowError($"Third party with ID {terceroId} is already associated with another client.");
                            return;
                        }
                    }
                    
                    cliente.TerceroId = terceroId;
                    
                    var fechaNacStr = UIHelper.RequestInput("New birth date (YYYY-MM-DD)", cliente.FechaNac.ToString("yyyy-MM-dd"));
                    cliente.FechaNac = DateTime.Parse(fechaNacStr);
                    
                    var fechaCompraStr = UIHelper.RequestInput("New purchase date (YYYY-MM-DD)", cliente.FechaCompra.ToString("yyyy-MM-dd"));
                    cliente.FechaCompra = DateTime.Parse(fechaCompraStr);

                    if (UIHelper.Confirm("Do you confirm these changes?"))
                    {
                        _context.Update(cliente);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Client updated successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Client not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating client", ex);
            }
        }

        private void DeleteClient()
        {
            UIHelper.ShowTitle("Delete Client");
            
            try
            {
                var idStr = UIHelper.RequestInput("Enter ID of the client to delete");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var cliente = _context.Clientes.Find(id);

                if (cliente != null)
                {
                    // Check if there are sales associated with this client
                    var ventas = _context.Ventas.Where(v => v.TerceroCliId == cliente.TerceroId).ToList();
                    
                    if (ventas.Any())
                    {
                        UIHelper.ShowWarning($"This client has {ventas.Count} sales associated with it.");
                        UIHelper.ShowWarning("Cannot delete a client that has associated sales.");
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        return;
                    }
                    
                    // Show information of the client to delete
                    UIHelper.ShowTitle("Client Information to Delete");
                    Console.WriteLine($"ID: {cliente.Id}");
                    Console.WriteLine($"Third Party ID: {cliente.TerceroId}");
                    Console.WriteLine($"Birth Date: {cliente.FechaNac.ToShortDateString()}");
                    Console.WriteLine($"Purchase Date: {cliente.FechaCompra.ToShortDateString()}");
                    
                    if (UIHelper.Confirm("Are you ABSOLUTELY sure you want to delete this client?"))
                    {
                        _context.Clientes.Remove(cliente);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Client deleted successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Client not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting client", ex);
            }
        }
        
        // Maintain backward compatibility
        private void ListarClientes() => ListClients();
        private void CrearCliente() => CreateClient();
        private void EditarCliente() => UpdateClient();
        private void EliminarCliente() => DeleteClient();
    }
}