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
                UIHelper.ShowTitle("Supplier Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Suppliers" },
                    { "2", "Create New Supplier" },
                    { "3", "Edit Supplier" },
                    { "4", "Delete Supplier" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListSuppliers();
                        break;
                    case "2":
                        CreateSupplier();
                        break;
                    case "3":
                        UpdateSupplier();
                        break;
                    case "4":
                        DeleteSupplier();
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

        private void ListSuppliers(string title = "Supplier List")
        {
            UIHelper.ShowTitle(title);
            
            try
            {
                var suppliers = _proveedorRepository.GetAll().ToList();
                
                if (!suppliers.Any())
                {
                    UIHelper.ShowWarning("No suppliers are registered.");
                    Console.ReadKey();
                    return;
                }
                
                // Define columns and values to display
                var columns = new Dictionary<string, Func<Proveedor, object>>
                {
                    { "ID", p => p.Id },
                    { "Third Party", p => p.TerceroId },
                    { "Discount", p => $"{p.Dcto:P2}" },
                    { "Payment Day", p => p.DiaPago },
                    { "Third Party Data", p => GetThirdPartyData(p.TerceroId) }
                };
                
                // Use DrawTable method to show formatted data
                UIHelper.DrawTable(suppliers, columns, title);
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing suppliers", ex);
            }
        }
        
        // Helper method to get third party data
        private string GetThirdPartyData(string terceroId)
        {
            try
            {
                var tercero = _context.Terceros.Find(terceroId);
                if (tercero == null)
                    return "Not found";
                
                return $"{tercero.Nombre} {tercero.Apellidos}";
            }
            catch
            {
                return "Error";
            }
        }

        private void CreateSupplier()
        {
            UIHelper.ShowTitle("Create New Supplier");
            
            try
            {
                // List available third parties
                UIHelper.ShowTitle("Available Third Parties");
                var thirdParties = _context.Terceros.ToList();
                
                if (!thirdParties.Any())
                {
                    UIHelper.ShowError("No third parties registered. You must create a third party first.");
                    return;
                }
                
                // Define columns to display third parties
                var thirdPartyColumns = new Dictionary<string, Func<Terceros, object>>
                {
                    { "ID", t => t.Id },
                    { "Name", t => t.Nombre },
                    { "Last Name", t => t.Apellidos },
                    { "Email", t => t.Email ?? "Not registered" }
                };
                
                // Show table of third parties
                UIHelper.DrawTable(thirdParties, thirdPartyColumns, "Available Third Parties");
                
                var thirdPartyId = UIHelper.RequestInput("Enter the third party ID to associate with the supplier");
                if (string.IsNullOrWhiteSpace(thirdPartyId))
                {
                    UIHelper.ShowWarning("Operation cancelled. The third party ID is required.");
                    return;
                }
                
                // Check that the third party exists
                var thirdParty = _context.Terceros.Find(thirdPartyId);
                if (thirdParty == null)
                {
                    UIHelper.ShowError($"Third party with ID {thirdPartyId} does not exist. You must create the third party first.");
                    return;
                }
                
                // Check that there isn't already a supplier with this third party
                var existingSupplier = _proveedorRepository.GetAll().FirstOrDefault(p => p.TerceroId == thirdPartyId);
                if (existingSupplier != null)
                {
                    UIHelper.ShowError($"A supplier already exists associated with third party ID {thirdPartyId}.");
                    return;
                }
                
                var dctoStr = UIHelper.RequestInput("Enter the discount (0-1)");
                var dcto = double.Parse(dctoStr);
                
                if (dcto < 0 || dcto > 1)
                {
                    UIHelper.ShowError("The discount must be a value between 0 and 1.");
                    return;
                }
                
                var paymentDayStr = UIHelper.RequestInput("Enter the payment day (1-31)");
                var paymentDay = int.Parse(paymentDayStr);
                
                if (paymentDay < 1 || paymentDay > 31)
                {
                    UIHelper.ShowError("The payment day must be a value between 1 and 31.");
                    return;
                }

                var supplier = new Proveedor { 
                    TerceroId = thirdPartyId, 
                    Dcto = dcto, 
                    DiaPago = paymentDay 
                };
                
                // Show summary before confirming
                UIHelper.ShowTitle("Supplier Summary");
                Console.WriteLine($"Third Party: {thirdParty.Nombre} {thirdParty.Apellidos} ({thirdParty.Id})");
                Console.WriteLine($"Discount: {supplier.Dcto:P2}");
                Console.WriteLine($"Payment Day: {supplier.DiaPago}");
                
                if (UIHelper.Confirm("Do you want to save this supplier?"))
                {
                    _proveedorRepository.Add(supplier);
                    UIHelper.ShowSuccess("Supplier created successfully.");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating supplier", ex);
            }
        }

        private void UpdateSupplier()
        {
            UIHelper.ShowTitle("Edit Supplier");
            
            try
            {
                // Show list of available suppliers
                ListSuppliers("Available Suppliers to Edit");
                
                var idStr = UIHelper.RequestInput("Enter the ID of the supplier to edit");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("The ID is required.");
                    return;
                }

                var id = int.Parse(idStr);
                var supplier = _proveedorRepository.GetById(id);

                if (supplier != null)
                {
                    var thirdParty = _context.Terceros.Find(supplier.TerceroId);
                    
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {supplier.Id}");
                    Console.WriteLine($"Third Party: {thirdParty?.Nombre} {thirdParty?.Apellidos} ({supplier.TerceroId})");
                    Console.WriteLine($"Discount: {supplier.Dcto:P2}");
                    Console.WriteLine($"Payment Day: {supplier.DiaPago}");
                    Console.WriteLine("\nEnter new values or leave blank to keep current ones:");

                    var dctoStr = UIHelper.RequestInput("New discount (0-1)", supplier.Dcto.ToString());
                    var dcto = double.Parse(dctoStr);
                    
                    if (dcto < 0 || dcto > 1)
                    {
                        UIHelper.ShowError("The discount must be a value between 0 and 1.");
                        return;
                    }
                    
                    var paymentDayStr = UIHelper.RequestInput("New payment day (1-31)", supplier.DiaPago.ToString());
                    var paymentDay = int.Parse(paymentDayStr);
                    
                    if (paymentDay < 1 || paymentDay > 31)
                    {
                        UIHelper.ShowError("The payment day must be a value between 1 and 31.");
                        return;
                    }

                    supplier.Dcto = dcto;
                    supplier.DiaPago = paymentDay;

                    if (UIHelper.Confirm("Are you sure you want to apply these changes?"))
                    {
                        _proveedorRepository.Update(supplier);
                        UIHelper.ShowSuccess("Supplier updated successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Supplier not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating supplier", ex);
            }
        }

        private void DeleteSupplier()
        {
            UIHelper.ShowTitle("Delete Supplier");
            
            try
            {
                // Show list of available suppliers
                ListSuppliers("Available Suppliers to Delete");
                
                var idStr = UIHelper.RequestInput("Enter the ID of the supplier to delete");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("The ID is required.");
                    return;
                }

                var id = int.Parse(idStr);
                var supplier = _proveedorRepository.GetById(id);

                if (supplier != null)
                {
                    var thirdParty = _context.Terceros.Find(supplier.TerceroId);
                    
                    UIHelper.ShowTitle("Information of Supplier to Delete");
                    Console.WriteLine($"ID: {supplier.Id}");
                    Console.WriteLine($"Third Party: {thirdParty?.Nombre} {thirdParty?.Apellidos} ({supplier.TerceroId})");
                    Console.WriteLine($"Discount: {supplier.Dcto:P2}");
                    Console.WriteLine($"Payment Day: {supplier.DiaPago}");
                    
                    // Check if there are any purchases associated with this supplier
                    var linkedPurchases = _context.Compras.Count(c => c.TerceroProvId == supplier.TerceroId);
                    if (linkedPurchases > 0)
                    {
                        UIHelper.ShowWarning($"This supplier has {linkedPurchases} associated purchases. Deleting it may affect system integrity.");
                    }
                    
                    if (UIHelper.Confirm("Are you ABSOLUTELY sure you want to delete this supplier?"))
                    {
                        _proveedorRepository.Delete(id);
                        UIHelper.ShowSuccess("Supplier deleted successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Supplier not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting supplier", ex);
            }
        }
        
        // Maintain backward compatibility with Spanish methods
        private void ListarProveedores(string titulo = "Listado de Proveedores") => ListSuppliers(titulo);
        private void CrearProveedor() => CreateSupplier();
        private void EditarProveedor() => UpdateSupplier();
        private void EliminarProveedor() => DeleteSupplier();
        private string ObtenerDatosTercero(string terceroId) => GetThirdPartyData(terceroId);
    }
}