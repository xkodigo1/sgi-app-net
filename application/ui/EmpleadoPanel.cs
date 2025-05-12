using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class EmpleadoPanel
    {
        private readonly YourDbContext _context;

        public EmpleadoPanel(YourDbContext context)
        {
            _context = context;
        }

        public async Task ShowMenu()
        {
            while (true)
            {
                UIHelper.ShowTitle("Employees Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Employees" },
                    { "2", "Create New Employee" },
                    { "3", "Edit Employee" },
                    { "4", "Delete Employee" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListEmployees();
                        break;
                    case "2":
                        await CreateEmployeeAsync();
                        break;
                    case "3":
                        await UpdateEmployeeAsync();
                        break;
                    case "4":
                        await DeleteEmployeeAsync();
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

        private void ListEmployees()
        {
            UIHelper.ShowTitle("Employee List");
            
            try
            {
                var empleados = _context.Empleados.ToList();
                
                if (!empleados.Any())
                {
                    UIHelper.ShowWarning("No employees are registered.");
                    Console.ReadKey();
                    return;
                }
                
                // Define columns and values to display
                var columns = new Dictionary<string, Func<Empleado, object>>
                {
                    { "ID", e => e.Id },
                    { "Third party", e => e.TerceroId },
                    { "Hire Date", e => e.FechaIngreso.ToShortDateString() },
                    { "Base Salary", e => $"{e.SalarioBase:C0}" },
                    { "EPS", e => e.EpsId },
                    { "ARL", e => e.ArlId }
                };
                
                // Use DrawTable method to show formatted data
                UIHelper.DrawTable(empleados, columns, "Employee Records");
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing employees", ex);
            }
        }

        private async Task CreateEmployeeAsync()
        {
            UIHelper.ShowTitle("Create New Employee");
            
            try
            {
                // Show available third parties
                await ShowAvailableThirdParties();
                
                var terceroId = UIHelper.RequestInput("Enter ID of the third party");
                if (string.IsNullOrWhiteSpace(terceroId))
                {
                    UIHelper.ShowWarning("Operation cancelled. Third party ID is required.");
                    return;
                }
                
                // Verify that third party exists
                var tercero = await _context.Terceros.FindAsync(terceroId);
                if (tercero == null)
                {
                    UIHelper.ShowError($"Third party with ID {terceroId} does not exist. You must create the third party first.");
                    return;
                }
                
                // Verify that third party is not already assigned to another employee
                var empleadoExistente = await _context.Empleados.FirstOrDefaultAsync(e => e.TerceroId == terceroId);
                if (empleadoExistente != null)
                {
                    UIHelper.ShowError($"Third party with ID {terceroId} is already assigned to another employee (ID: {empleadoExistente.Id}).");
                    return;
                }
                
                var fechaIngresoStr = UIHelper.RequestInput("Enter hire date (YYYY-MM-DD)", DateTime.Now.ToString("yyyy-MM-dd"));
                var fechaIngreso = DateTime.Parse(fechaIngresoStr);
                
                var salarioBaseStr = UIHelper.RequestInput("Enter base salary");
                if (string.IsNullOrWhiteSpace(salarioBaseStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. Base salary is required.");
                    return;
                }
                var salarioBase = double.Parse(salarioBaseStr);
                
                // Show available EPS
                await ShowAvailableEPS();
                
                var epsIdStr = UIHelper.RequestInput("Enter EPS ID");
                if (string.IsNullOrWhiteSpace(epsIdStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. EPS ID is required.");
                    return;
                }
                var epsId = int.Parse(epsIdStr);
                
                // Verify that EPS exists
                var eps = await _context.Set<EPS>().FindAsync(epsId);
                if (eps == null)
                {
                    UIHelper.ShowError($"EPS with ID {epsId} does not exist. You must create it first.");
                    return;
                }
                
                // Show available ARL
                await ShowAvailableARL();
                
                var arlIdStr = UIHelper.RequestInput("Enter ARL ID");
                if (string.IsNullOrWhiteSpace(arlIdStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. ARL ID is required.");
                    return;
                }
                var arlId = int.Parse(arlIdStr);
                
                // Verify that ARL exists
                var arl = await _context.Set<ARL>().FindAsync(arlId);
                if (arl == null)
                {
                    UIHelper.ShowError($"ARL with ID {arlId} does not exist. You must create it first.");
                    return;
                }

                var empleado = new Empleado { 
                    TerceroId = terceroId, 
                    FechaIngreso = fechaIngreso, 
                    SalarioBase = salarioBase, 
                    EpsId = epsId, 
                    ArlId = arlId 
                };
                
                // Show summary before confirming
                UIHelper.ShowTitle("Employee Summary");
                Console.WriteLine($"Third party: {empleado.TerceroId}");
                Console.WriteLine($"Hire Date: {empleado.FechaIngreso.ToShortDateString()}");
                Console.WriteLine($"Base Salary: {empleado.SalarioBase:C0}");
                Console.WriteLine($"EPS: {empleado.EpsId}");
                Console.WriteLine($"ARL: {empleado.ArlId}");
                
                if (UIHelper.Confirm("Do you want to save this employee?"))
                {
                    _context.Empleados.Add(empleado);
                    await _context.SaveChangesAsync();
                    UIHelper.ShowSuccess($"Employee created successfully with ID: {empleado.Id}");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating employee", ex);
            }
        }

        private async Task UpdateEmployeeAsync()
        {
            UIHelper.ShowTitle("Edit Employee");
            
            try
            {
                // Show list of available employees
                ListEmployees();
                
                var idStr = UIHelper.RequestInput("Enter ID of the employee to edit");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var empleado = await _context.Empleados.FindAsync(id);

                if (empleado != null)
                {
                    // Show current information
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {empleado.Id}");
                    Console.WriteLine($"Third party: {empleado.TerceroId}");
                    Console.WriteLine($"Hire Date: {empleado.FechaIngreso.ToShortDateString()}");
                    Console.WriteLine($"Base Salary: {empleado.SalarioBase:C0}");
                    Console.WriteLine($"EPS: {empleado.EpsId}");
                    Console.WriteLine($"ARL: {empleado.ArlId}");
                    Console.WriteLine("\nEnter new values or leave blank to keep current:");
                    
                    // Show available third parties
                    await ShowAvailableThirdParties();
                    
                    var terceroId = UIHelper.RequestInput("New Third party ID", empleado.TerceroId.ToString());
                    
                    // Verify that third party exists
                    var tercero = await _context.Terceros.FindAsync(terceroId);
                    if (tercero == null)
                    {
                        UIHelper.ShowError($"Third party with ID {terceroId} does not exist. You must create the third party first.");
                        return;
                    }
                    
                    // If third party changes, verify it's not assigned to another employee
                    if (terceroId != empleado.TerceroId)
                    {
                        var empleadoExistente = await _context.Empleados.FirstOrDefaultAsync(e => e.TerceroId == terceroId && e.Id != id);
                        if (empleadoExistente != null)
                        {
                            UIHelper.ShowError($"Third party with ID {terceroId} is already assigned to another employee (ID: {empleadoExistente.Id}).");
                            return;
                        }
                    }
                    
                    var fechaIngresoStr = UIHelper.RequestInput("New hire date (YYYY-MM-DD)", empleado.FechaIngreso.ToString("yyyy-MM-dd"));
                    var salarioBaseStr = UIHelper.RequestInput("New base salary", empleado.SalarioBase.ToString());
                    
                    // Show available EPS
                    await ShowAvailableEPS();
                    
                    var epsIdStr = UIHelper.RequestInput("New EPS ID", empleado.EpsId.ToString());
                    var epsId = int.Parse(epsIdStr);
                    
                    // Verify that EPS exists
                    var eps = await _context.Set<EPS>().FindAsync(epsId);
                    if (eps == null)
                    {
                        UIHelper.ShowError($"EPS with ID {epsId} does not exist. You must create it first.");
                        return;
                    }
                    
                    // Show available ARL
                    await ShowAvailableARL();
                    
                    var arlIdStr = UIHelper.RequestInput("New ARL ID", empleado.ArlId.ToString());
                    var arlId = int.Parse(arlIdStr);
                    
                    // Verify that ARL exists
                    var arl = await _context.Set<ARL>().FindAsync(arlId);
                    if (arl == null)
                    {
                        UIHelper.ShowError($"ARL with ID {arlId} does not exist. You must create it first.");
                        return;
                    }
                    
                    empleado.TerceroId = terceroId;
                    empleado.FechaIngreso = DateTime.Parse(fechaIngresoStr);
                    empleado.SalarioBase = double.Parse(salarioBaseStr);
                    empleado.EpsId = epsId;
                    empleado.ArlId = arlId;

                    // Show changes summary before confirming
                    UIHelper.ShowTitle("Changes Summary");
                    Console.WriteLine($"ID: {empleado.Id}");
                    Console.WriteLine($"Third party: {empleado.TerceroId}");
                    Console.WriteLine($"Hire Date: {empleado.FechaIngreso.ToShortDateString()}");
                    Console.WriteLine($"Base Salary: {empleado.SalarioBase:C0}");
                    Console.WriteLine($"EPS: {empleado.EpsId}");
                    Console.WriteLine($"ARL: {empleado.ArlId}");
                    
                    if (UIHelper.Confirm("Do you confirm these changes?"))
                    {
                        _context.Update(empleado);
                        await _context.SaveChangesAsync();
                        UIHelper.ShowSuccess("Employee updated successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Employee not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating employee", ex);
            }
        }

        private async Task DeleteEmployeeAsync()
        {
            UIHelper.ShowTitle("Delete Employee");
            
            try
            {
                // Show list of available employees
                ListEmployees();
                
                var idStr = UIHelper.RequestInput("Enter ID of the employee to delete");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var empleado = await _context.Empleados.FindAsync(id);

                if (empleado != null)
                {
                    // Verify if there are associated purchases or sales
                    var compras = await _context.Compras.Where(c => c.TerceroEmpId == empleado.TerceroId).ToListAsync();
                    var ventas = await _context.Ventas.Where(v => v.TerceroEnId == empleado.TerceroId).ToListAsync();
                    
                    if (compras.Any() || ventas.Any())
                    {
                        UIHelper.ShowWarning($"This employee has {compras.Count} purchases and {ventas.Count} sales associated.");
                        if (!UIHelper.Confirm("Are you ABSOLUTELY sure you want to delete this employee and their associations?"))
                        {
                            UIHelper.ShowWarning("Operation cancelled by user.");
                            return;
                        }
                    }
                    
                    // Show information to delete
                    UIHelper.ShowTitle("Employee Information to Delete");
                    Console.WriteLine($"ID: {empleado.Id}");
                    Console.WriteLine($"Third party: {empleado.TerceroId}");
                    Console.WriteLine($"Hire Date: {empleado.FechaIngreso.ToShortDateString()}");
                    Console.WriteLine($"Base Salary: {empleado.SalarioBase:C0}");
                    Console.WriteLine($"EPS: {empleado.EpsId}");
                    Console.WriteLine($"ARL: {empleado.ArlId}");
                    
                    if (UIHelper.Confirm("Are you ABSOLUTELY sure you want to delete this employee?"))
                    {
                        var strategy = _context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using (var transaction = await _context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    _context.Empleados.Remove(empleado);
                                    await _context.SaveChangesAsync();
                                    
                                    await transaction.CommitAsync();
                                    UIHelper.ShowSuccess("Employee deleted successfully.");
                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync();
                                    throw new Exception("Error deleting employee. Changes have been reverted.", ex);
                                }
                            }
                        });
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Employee not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting employee", ex);
            }
        }
        
        // Helper methods for showing related entities
        
        private async Task ShowAvailableThirdParties()
        {
            UIHelper.ShowTitle("Available Third Parties");
            var terceros = await _context.Terceros.ToListAsync();
            
            if (!terceros.Any())
            {
                UIHelper.ShowWarning("No third parties are registered. You must create a third party first.");
                return;
            }
            
            var columns = new Dictionary<string, Func<Terceros, object>>
            {
                { "ID", t => t.Id },
                { "Name", t => t.Nombre },
                { "Last Name", t => t.Apellidos },
                { "Email", t => t.Email ?? "N/A" }
            };
            
            UIHelper.DrawTable(terceros, columns, "Third Party List");
            Console.WriteLine();
        }
        
        private async Task ShowAvailableEPS()
        {
            UIHelper.ShowTitle("Available EPS");
            var epsList = await _context.Set<EPS>().ToListAsync();
            
            if (!epsList.Any())
            {
                UIHelper.ShowWarning("No EPS are registered.");
                return;
            }
            
            var columns = new Dictionary<string, Func<EPS, object>>
            {
                { "ID", e => e.Id },
                { "Name", e => e.Nombre }
            };
            
            UIHelper.DrawTable(epsList, columns, "EPS List");
            Console.WriteLine();
        }
        
        private async Task ShowAvailableARL()
        {
            UIHelper.ShowTitle("Available ARL");
            var arlList = await _context.Set<ARL>().ToListAsync();
            
            if (!arlList.Any())
            {
                UIHelper.ShowWarning("No ARL are registered.");
                return;
            }
            
            var columns = new Dictionary<string, Func<ARL, object>>
            {
                { "ID", a => a.Id },
                { "Name", a => a.Nombre }
            };
            
            UIHelper.DrawTable(arlList, columns, "ARL List");
            Console.WriteLine();
        }
        
        // Maintain backward compatibility
        private void ListarEmpleados() => ListEmployees();
        private Task CrearEmpleadoAsync() => CreateEmployeeAsync();
        private Task EditarEmpleadoAsync() => UpdateEmployeeAsync();
        private Task EliminarEmpleadoAsync() => DeleteEmployeeAsync();
        private Task MostrarTercerosDisponibles() => ShowAvailableThirdParties();
        private Task MostrarEPSDisponibles() => ShowAvailableEPS();
        private Task MostrarARLDisponibles() => ShowAvailableARL();
    }
}