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
                UIHelper.ShowTitle("Cash Flow Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Transactions" },
                    { "2", "Create New Transaction" },
                    { "3", "Edit Transaction" },
                    { "4", "Delete Transaction" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListTransactions();
                        break;
                    case "2":
                        CreateTransaction();
                        break;
                    case "3":
                        UpdateTransaction();
                        break;
                    case "4":
                        DeleteTransaction();
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

        private void ListTransactions()
        {
            UIHelper.ShowTitle("Cash Flow Transactions List");
            
            try
            {
                var movimientos = _context.MovCaja.ToList();
                
                if (!movimientos.Any())
                {
                    UIHelper.ShowWarning("No cash flow transactions are registered.");
                    Console.ReadKey();
                    return;
                }
                
                // Define columns and values to display
                var columns = new Dictionary<string, Func<MovCaja, object>>
                {
                    { "ID", m => m.Id },
                    { "Date", m => m.Fecha.ToShortDateString() },
                    { "Type", m => GetTransactionType(m.TipoMovId) },
                    { "Concept", m => m.Concepto },
                    { "Third Party", m => m.TerceroId },
                    { "Amount", m => $"{m.Valor:C}" }
                };
                
                // Use DrawTable method to show formatted data
                UIHelper.DrawTable(movimientos, columns, "Cash Flow Transactions Records");
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing transactions", ex);
            }
        }
        
        // Helper method to get transaction type
        private string GetTransactionType(int tipoMovId)
        {
            switch (tipoMovId)
            {
                case 1:
                    return "Income";
                case 2:
                    return "Expense";
                default:
                    return "Unknown";
            }
        }

        private void CreateTransaction()
        {
            UIHelper.ShowTitle("Create New Cash Flow Transaction");
            
            try
            {
                // List available third parties
                UIHelper.ShowTitle("Available Third Parties");
                var terceros = _context.Terceros.ToList();
                
                if (!terceros.Any())
                {
                    UIHelper.ShowError("No third parties are registered. You must create a third party first.");
                    return;
                }
                
                foreach (var t in terceros)
                {
                    Console.WriteLine($"{t.Id} - {t.Nombre} {t.Apellidos}");
                }
                
                var terceroId = UIHelper.RequestInput("Enter third party ID");
                if (string.IsNullOrWhiteSpace(terceroId))
                {
                    UIHelper.ShowWarning("Operation cancelled. Third party ID is required.");
                    return;
                }
                
                // Verify third party exists
                var tercero = _context.Terceros.Find(terceroId);
                if (tercero == null)
                {
                    UIHelper.ShowError($"Third party with ID {terceroId} does not exist. You must create the third party first.");
                    return;
                }
                
                var concepto = UIHelper.RequestInput("Enter transaction concept");
                if (string.IsNullOrWhiteSpace(concepto))
                {
                    UIHelper.ShowWarning("Operation cancelled. Concept is required.");
                    return;
                }
                
                // Show transaction type options
                UIHelper.ShowTitle("Transaction Types");
                Console.WriteLine("1 - Income");
                Console.WriteLine("2 - Expense");
                
                var tipoMovIdStr = UIHelper.RequestInput("Enter transaction type (1=Income, 2=Expense)");
                var tipoMovId = int.Parse(tipoMovIdStr);
                
                if (tipoMovId != 1 && tipoMovId != 2)
                {
                    UIHelper.ShowError("Transaction type must be 1 (Income) or 2 (Expense).");
                    return;
                }
                
                var valorStr = UIHelper.RequestInput("Enter transaction amount");
                var valor = decimal.Parse(valorStr);
                
                if (valor <= 0)
                {
                    UIHelper.ShowError("Amount must be greater than zero.");
                    return;
                }
                
                var fechaStr = UIHelper.RequestInput("Enter date (YYYY-MM-DD)", DateTime.Now.ToString("yyyy-MM-dd"));
                var fecha = DateTime.Parse(fechaStr);

                var movimiento = new MovCaja { 
                    Concepto = concepto, 
                    TerceroId = terceroId, 
                    TipoMovId = tipoMovId,
                    Valor = valor,
                    Fecha = fecha
                };
                
                // Show summary before confirming
                UIHelper.ShowTitle("Transaction Summary");
                Console.WriteLine($"Concept: {movimiento.Concepto}");
                Console.WriteLine($"Third Party: {tercero.Nombre} {tercero.Apellidos} ({tercero.Id})");
                Console.WriteLine($"Type: {GetTransactionType(movimiento.TipoMovId)}");
                Console.WriteLine($"Amount: {movimiento.Valor:C}");
                Console.WriteLine($"Date: {movimiento.Fecha.ToShortDateString()}");
                
                if (UIHelper.Confirm("Do you want to save this transaction?"))
                {
                    _context.MovCaja.Add(movimiento);
                    _context.SaveChanges();
                    UIHelper.ShowSuccess($"Transaction created successfully with ID: {movimiento.Id}");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating transaction", ex);
            }
        }

        private void UpdateTransaction()
        {
            UIHelper.ShowTitle("Edit Cash Flow Transaction");
            
            try
            {
                // Show list of available transactions
                ListTransactions();
                
                var idStr = UIHelper.RequestInput("Enter ID of the transaction to edit");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var movimiento = _context.MovCaja.Find(id);

                if (movimiento != null)
                {
                    // Get third party info
                    var tercero = _context.Terceros.Find(movimiento.TerceroId);
                    
                    // Show current information
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {movimiento.Id}");
                    Console.WriteLine($"Concept: {movimiento.Concepto}");
                    Console.WriteLine($"Third Party: {(tercero != null ? $"{tercero.Nombre} {tercero.Apellidos}" : "Unknown")} ({movimiento.TerceroId})");
                    Console.WriteLine($"Type: {GetTransactionType(movimiento.TipoMovId)}");
                    Console.WriteLine($"Amount: {movimiento.Valor:C}");
                    Console.WriteLine($"Date: {movimiento.Fecha.ToShortDateString()}");
                    Console.WriteLine("\nEnter new values or leave blank to keep current:");
                    
                    // List available third parties
                    UIHelper.ShowTitle("Available Third Parties");
                    var terceros = _context.Terceros.ToList();
                    foreach (var t in terceros)
                    {
                        Console.WriteLine($"{t.Id} - {t.Nombre} {t.Apellidos}");
                    }
                    
                    var terceroId = UIHelper.RequestInput("New third party ID", movimiento.TerceroId);
                    
                    // Verify third party exists
                    var nuevoTercero = _context.Terceros.Find(terceroId);
                    if (nuevoTercero == null)
                    {
                        UIHelper.ShowError($"Third party with ID {terceroId} does not exist. You must create the third party first.");
                        return;
                    }
                    
                    var concepto = UIHelper.RequestInput("New concept", movimiento.Concepto);
                    
                    // Show transaction type options
                    UIHelper.ShowTitle("Transaction Types");
                    Console.WriteLine("1 - Income");
                    Console.WriteLine("2 - Expense");
                    
                    var tipoMovIdStr = UIHelper.RequestInput("New transaction type (1=Income, 2=Expense)", movimiento.TipoMovId.ToString());
                    var tipoMovId = int.Parse(tipoMovIdStr);
                    
                    if (tipoMovId != 1 && tipoMovId != 2)
                    {
                        UIHelper.ShowError("Transaction type must be 1 (Income) or 2 (Expense).");
                        return;
                    }
                    
                    var valorStr = UIHelper.RequestInput("New amount", movimiento.Valor.ToString());
                    var valor = decimal.Parse(valorStr);
                    
                    if (valor <= 0)
                    {
                        UIHelper.ShowError("Amount must be greater than zero.");
                        return;
                    }
                    
                    var fechaStr = UIHelper.RequestInput("New date (YYYY-MM-DD)", movimiento.Fecha.ToString("yyyy-MM-dd"));
                    
                    movimiento.Concepto = concepto;
                    movimiento.TerceroId = terceroId;
                    movimiento.TipoMovId = tipoMovId;
                    movimiento.Valor = valor;
                    movimiento.Fecha = DateTime.Parse(fechaStr);

                    if (UIHelper.Confirm("Do you confirm these changes?"))
                    {
                        _context.Update(movimiento);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Transaction updated successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Transaction not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating transaction", ex);
            }
        }

        private void DeleteTransaction()
        {
            UIHelper.ShowTitle("Delete Cash Flow Transaction");
            
            try
            {
                // Show list of available transactions
                ListTransactions();
                
                var idStr = UIHelper.RequestInput("Enter ID of the transaction to delete");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("Operation cancelled. ID is required.");
                    return;
                }
                
                var id = int.Parse(idStr);
                var movimiento = _context.MovCaja.Find(id);

                if (movimiento != null)
                {
                    // Get third party info
                    var tercero = _context.Terceros.Find(movimiento.TerceroId);
                    
                    // Show information of the transaction to delete
                    UIHelper.ShowTitle("Transaction Information to Delete");
                    Console.WriteLine($"ID: {movimiento.Id}");
                    Console.WriteLine($"Concept: {movimiento.Concepto}");
                    Console.WriteLine($"Third Party: {(tercero != null ? $"{tercero.Nombre} {tercero.Apellidos}" : "Unknown")} ({movimiento.TerceroId})");
                    Console.WriteLine($"Type: {GetTransactionType(movimiento.TipoMovId)}");
                    Console.WriteLine($"Amount: {movimiento.Valor:C}");
                    Console.WriteLine($"Date: {movimiento.Fecha.ToShortDateString()}");
                    
                    if (UIHelper.Confirm("Are you ABSOLUTELY sure you want to delete this transaction?"))
                    {
                        _context.MovCaja.Remove(movimiento);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Transaction deleted successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Transaction not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting transaction", ex);
            }
        }
        
        // Maintain backward compatibility
        private void ListarMovimientos() => ListTransactions();
        private void CrearMovimiento() => CreateTransaction();
        private void EditarMovimiento() => UpdateTransaction();
        private void EliminarMovimiento() => DeleteTransaction();
        private string ObtenerTipoMovimiento(int tipoMovId) => GetTransactionType(tipoMovId);
    }
}