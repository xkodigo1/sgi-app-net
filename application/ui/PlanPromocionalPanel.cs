using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using sgi_app.infrastructure.sql;
using sgi_app.domain.entities;

namespace sgi_app.application.ui
{
    public class PlanPromocionalPanel
    {
        private readonly YourDbContext _context;

        public PlanPromocionalPanel(YourDbContext context)
        {
            _context = context;
        }

        public void ShowMenu()
        {
            while (true)
            {
                UIHelper.ShowTitle("Promotional Plans Panel");
                
                var options = new Dictionary<string, string>
                {
                    { "1", "List Promotional Plans" },
                    { "2", "Create New Promotional Plan" },
                    { "3", "Edit Promotional Plan" },
                    { "4", "Delete Promotional Plan" },
                    { "5", "Add Products to Plan" },
                    { "6", "View Products in Plan" },
                    { "7", "Remove Products from Plan" }
                };
                
                UIHelper.ShowMenuOptions(options);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListPlans();
                        break;
                    case "2":
                        CreatePlan();
                        break;
                    case "3":
                        UpdatePlan();
                        break;
                    case "4":
                        DeletePlan();
                        break;
                    case "5":
                        AddProductsToPlan();
                        break;
                    case "6":
                        ViewProductsInPlan();
                        break;
                    case "7":
                        RemoveProductsFromPlan();
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

        private void ListPlans()
        {
            UIHelper.ShowTitle("Promotional Plans List");
            
            try
            {
                var plans = _context.Set<Plan>().ToList();
                
                var columns = new Dictionary<string, Func<Plan, object>>
                {
                    { "ID", p => p.Id },
                    { "Name", p => p.Nombre },
                    { "Start Date", p => p.FechaInicio.ToShortDateString() },
                    { "End Date", p => p.FechaFin.ToShortDateString() },
                    { "Discount %", p => p.Dcto }
                };
                
                UIHelper.DrawTable(plans, columns, "Active Promotional Plans");
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error listing plans", ex);
            }
        }

        private void CreatePlan()
        {
            UIHelper.ShowTitle("Create New Promotional Plan");
            
            try
            {
                var name = UIHelper.RequestInput("Enter the plan name");
                if (string.IsNullOrWhiteSpace(name))
                {
                    UIHelper.ShowWarning("The name is required.");
                    return;
                }

                var startDateStr = UIHelper.RequestInput("Enter the start date (YYYY-MM-DD)");
                var startDate = DateTime.Parse(startDateStr);

                var endDateStr = UIHelper.RequestInput("Enter the end date (YYYY-MM-DD)");
                var endDate = DateTime.Parse(endDateStr);

                if (endDate <= startDate)
                {
                    UIHelper.ShowError("The end date must be after the start date.");
                    return;
                }

                var discountStr = UIHelper.RequestInput("Enter the discount percentage");
                var discount = double.Parse(discountStr);

                if (discount <= 0 || discount > 100)
                {
                    UIHelper.ShowError("The discount must be between 0 and 100.");
                    return;
                }

                var plan = new Plan
                {
                    Nombre = name,
                    FechaInicio = startDate,
                    FechaFin = endDate,
                    Dcto = discount
                };

                if (UIHelper.Confirm("Do you want to save this promotional plan?"))
                {
                    _context.Set<Plan>().Add(plan);
                    _context.SaveChanges();
                    UIHelper.ShowSuccess("Promotional plan created successfully.");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error creating plan", ex);
            }
        }

        private void UpdatePlan()
        {
            UIHelper.ShowTitle("Edit Promotional Plan");
            
            try
            {
                var idStr = UIHelper.RequestInput("Enter the ID of the plan to edit");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("The ID is required.");
                    return;
                }

                var id = int.Parse(idStr);
                var plan = _context.Set<Plan>().Find(id);

                if (plan != null)
                {
                    UIHelper.ShowTitle("Current Information");
                    Console.WriteLine($"ID: {plan.Id}");
                    Console.WriteLine($"Name: {plan.Nombre}");
                    Console.WriteLine($"Start Date: {plan.FechaInicio.ToShortDateString()}");
                    Console.WriteLine($"End Date: {plan.FechaFin.ToShortDateString()}");
                    Console.WriteLine($"Discount: {plan.Dcto}%");
                    Console.WriteLine("\nEnter new values or leave blank to keep current ones:");

                    var name = UIHelper.RequestInput("New name", plan.Nombre);
                    plan.Nombre = name;

                    var startDateStr = UIHelper.RequestInput("New start date (YYYY-MM-DD)", plan.FechaInicio.ToString("yyyy-MM-dd"));
                    plan.FechaInicio = DateTime.Parse(startDateStr);

                    var endDateStr = UIHelper.RequestInput("New end date (YYYY-MM-DD)", plan.FechaFin.ToString("yyyy-MM-dd"));
                    plan.FechaFin = DateTime.Parse(endDateStr);

                    var discountStr = UIHelper.RequestInput("New discount percentage", plan.Dcto.ToString());
                    plan.Dcto = double.Parse(discountStr);

                    if (UIHelper.Confirm("Do you confirm these changes?"))
                    {
                        _context.Update(plan);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Promotional plan updated successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Plan not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error updating plan", ex);
            }
        }

        private void DeletePlan()
        {
            UIHelper.ShowTitle("Delete Promotional Plan");
            
            try
            {
                var idStr = UIHelper.RequestInput("Enter the ID of the plan to delete");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.ShowWarning("The ID is required.");
                    return;
                }

                var id = int.Parse(idStr);
                var plan = _context.Set<Plan>().Find(id);

                if (plan != null)
                {
                    UIHelper.ShowTitle("Information of Plan to Delete");
                    Console.WriteLine($"ID: {plan.Id}");
                    Console.WriteLine($"Name: {plan.Nombre}");
                    Console.WriteLine($"Start Date: {plan.FechaInicio.ToShortDateString()}");
                    Console.WriteLine($"End Date: {plan.FechaFin.ToShortDateString()}");
                    Console.WriteLine($"Discount: {plan.Dcto}%");

                    if (UIHelper.Confirm("Are you sure you want to delete this plan?"))
                    {
                        // First delete relationships with products
                        var productsInPlan = _context.Set<PlanProducto>().Where(pp => pp.PlanId == id);
                        _context.Set<PlanProducto>().RemoveRange(productsInPlan);
                        
                        // Then delete the plan
                        _context.Set<Plan>().Remove(plan);
                        _context.SaveChanges();
                        UIHelper.ShowSuccess("Promotional plan and its product relationships deleted successfully.");
                    }
                    else
                    {
                        UIHelper.ShowWarning("Operation cancelled by user.");
                    }
                }
                else
                {
                    UIHelper.ShowError("Plan not found.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error deleting plan", ex);
            }
        }

        private void AddProductsToPlan()
        {
            UIHelper.ShowTitle("Add Products to Promotional Plan");
            
            try
            {
                // First select the plan
                var plans = _context.Set<Plan>().ToList();
                
                if (!plans.Any())
                {
                    UIHelper.ShowError("No promotional plans available. Create a plan first.");
                    return;
                }
                
                var planColumns = new Dictionary<string, Func<Plan, object>>
                {
                    { "ID", p => p.Id },
                    { "Name", p => p.Nombre },
                    { "Start Date", p => p.FechaInicio.ToShortDateString() },
                    { "End Date", p => p.FechaFin.ToShortDateString() },
                    { "Discount %", p => p.Dcto }
                };
                
                UIHelper.DrawTable(plans, planColumns, "Available Promotional Plans");
                
                var planIdStr = UIHelper.RequestInput("Enter the ID of the plan to add products to");
                var planId = int.Parse(planIdStr);
                
                var plan = _context.Set<Plan>().Find(planId);
                if (plan == null)
                {
                    UIHelper.ShowError("Plan not found.");
                    return;
                }
                
                // Now show available products
                var products = _context.Productos.ToList();
                
                if (!products.Any())
                {
                    UIHelper.ShowError("No products available to add to the plan.");
                    return;
                }
                
                // Get products already in the plan to filter them out
                var existingProductIds = _context.Set<PlanProducto>()
                    .Where(pp => pp.PlanId == planId)
                    .Select(pp => pp.ProductoId)
                    .ToList();
                
                // Show only products not already in the plan
                var availableProducts = products.Where(p => !existingProductIds.Contains(p.Id)).ToList();
                
                if (!availableProducts.Any())
                {
                    UIHelper.ShowWarning("All products are already included in this plan.");
                    return;
                }
                
                var productColumns = new Dictionary<string, Func<Producto, object>>
                {
                    { "ID", p => p.Id },
                    { "Name", p => p.Nombre },
                    { "Unit Price", p => $"{p.Precio:C}" },
                    { "Stock", p => p.Stock }
                };
                
                UIHelper.DrawTable(availableProducts, productColumns, "Available Products");
                
                var productIdStr = UIHelper.RequestInput("Enter the ID of the product to add to the plan (or leave blank to finish)");
                
                while (!string.IsNullOrWhiteSpace(productIdStr))
                {
                    var productId = productIdStr;
                    
                    var product = _context.Productos.Find(productId);
                    if (product == null)
                    {
                        UIHelper.ShowError($"Product with ID {productId} not found.");
                    }
                    else if (existingProductIds.Contains(productId))
                    {
                        UIHelper.ShowWarning($"Product with ID {productId} is already in this plan.");
                    }
                    else
                    {
                        var planProduct = new PlanProducto
                        {
                            PlanId = planId,
                            ProductoId = productId
                        };
                        
                        _context.Set<PlanProducto>().Add(planProduct);
                        _context.SaveChanges();
                        
                        existingProductIds.Add(productId);
                        UIHelper.ShowSuccess($"Product '{product.Nombre}' added to plan '{plan.Nombre}'.");
                    }
                    
                    productIdStr = UIHelper.RequestInput("Enter the ID of another product to add (or leave blank to finish)");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error adding products to plan", ex);
            }
        }

        private void ViewProductsInPlan()
        {
            UIHelper.ShowTitle("View Products in Promotional Plan");
            
            try
            {
                // First select the plan
                var plans = _context.Set<Plan>().ToList();
                
                if (!plans.Any())
                {
                    UIHelper.ShowError("No promotional plans available.");
                    return;
                }
                
                var planColumns = new Dictionary<string, Func<Plan, object>>
                {
                    { "ID", p => p.Id },
                    { "Name", p => p.Nombre },
                    { "Start Date", p => p.FechaInicio.ToShortDateString() },
                    { "End Date", p => p.FechaFin.ToShortDateString() },
                    { "Discount %", p => p.Dcto }
                };
                
                UIHelper.DrawTable(plans, planColumns, "Available Promotional Plans");
                
                var planIdStr = UIHelper.RequestInput("Enter the ID of the plan to view products");
                var planId = int.Parse(planIdStr);
                
                var plan = _context.Set<Plan>().Find(planId);
                if (plan == null)
                {
                    UIHelper.ShowError("Plan not found.");
                    return;
                }
                
                // Get products in the plan
                var planProducts = _context.Set<PlanProducto>()
                    .Where(pp => pp.PlanId == planId)
                    .ToList();
                
                if (!planProducts.Any())
                {
                    UIHelper.ShowWarning($"No products are associated with plan '{plan.Nombre}'.");
                    return;
                }
                
                // Create a list to store product information
                var productsInfo = new List<(string Id, string Nombre, decimal ValorUnitario, decimal ValorConDescuento)>();
                
                foreach (var pp in planProducts)
                {
                    var product = _context.Productos.Find(pp.ProductoId);
                    if (product != null)
                    {
                        var discountedPrice = product.Precio * (1 - (decimal)(plan.Dcto / 100));
                        productsInfo.Add((product.Id, product.Nombre, product.Precio, discountedPrice));
                    }
                }
                
                // Show the products with their discounted prices
                UIHelper.ShowTitle($"Products in Plan: {plan.Nombre}");
                Console.WriteLine($"Discount: {plan.Dcto}%");
                Console.WriteLine($"Valid from {plan.FechaInicio.ToShortDateString()} to {plan.FechaFin.ToShortDateString()}");
                Console.WriteLine();
                
                Console.WriteLine("┌───────────┬────────────────────────────────┬──────────────┬──────────────────┐");
                Console.WriteLine("│ ID        │ Product Name                   │ Regular Price│ Discounted Price │");
                Console.WriteLine("├───────────┼────────────────────────────────┼──────────────┼──────────────────┤");
                
                foreach (var info in productsInfo)
                {
                    Console.WriteLine($"│ {info.Id.PadRight(9)} │ {info.Nombre.PadRight(30)} │ {info.ValorUnitario,12:C} │ {info.ValorConDescuento,16:C} │");
                }
                
                Console.WriteLine("└───────────┴────────────────────────────────┴──────────────┴──────────────────┘");
                
                Console.WriteLine($"\nTotal products in plan: {productsInfo.Count}");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error viewing products in plan", ex);
            }
        }

        private void RemoveProductsFromPlan()
        {
            UIHelper.ShowTitle("Remove Products from Promotional Plan");
            
            try
            {
                // First select the plan
                var plans = _context.Set<Plan>().ToList();
                
                if (!plans.Any())
                {
                    UIHelper.ShowError("No promotional plans available.");
                    return;
                }
                
                var planColumns = new Dictionary<string, Func<Plan, object>>
                {
                    { "ID", p => p.Id },
                    { "Name", p => p.Nombre },
                    { "Start Date", p => p.FechaInicio.ToShortDateString() },
                    { "End Date", p => p.FechaFin.ToShortDateString() },
                    { "Discount %", p => p.Dcto }
                };
                
                UIHelper.DrawTable(plans, planColumns, "Available Promotional Plans");
                
                var planIdStr = UIHelper.RequestInput("Enter the ID of the plan to remove products from");
                var planId = int.Parse(planIdStr);
                
                var plan = _context.Set<Plan>().Find(planId);
                if (plan == null)
                {
                    UIHelper.ShowError("Plan not found.");
                    return;
                }
                
                // Get products in the plan
                var planProducts = _context.Set<PlanProducto>()
                    .Where(pp => pp.PlanId == planId)
                    .ToList();
                
                if (!planProducts.Any())
                {
                    UIHelper.ShowWarning($"No products are associated with plan '{plan.Nombre}'.");
                    return;
                }
                
                // Show products in the plan
                var productsInfo = new List<(int RelationId, string ProductId, string ProductName)>();
                int index = 1;
                
                foreach (var pp in planProducts)
                {
                    var product = _context.Productos.Find(pp.ProductoId);
                    if (product != null)
                    {
                        productsInfo.Add((index++, product.Id, product.Nombre));
                    }
                }
                
                UIHelper.ShowTitle($"Products in Plan: {plan.Nombre}");
                
                Console.WriteLine("┌───────┬───────────┬────────────────────────────────┐");
                Console.WriteLine("│ Index │ ID        │ Product Name                   │");
                Console.WriteLine("├───────┼───────────┼────────────────────────────────┤");
                
                foreach (var info in productsInfo)
                {
                    Console.WriteLine($"│ {info.RelationId,5} │ {info.ProductId.PadRight(9)} │ {info.ProductName.PadRight(30)} │");
                }
                
                Console.WriteLine("└───────┴───────────┴────────────────────────────────┘");
                
                var productIdStr = UIHelper.RequestInput("Enter the ID of the product to remove from the plan");
                
                if (string.IsNullOrWhiteSpace(productIdStr))
                {
                    UIHelper.ShowWarning("Operation cancelled.");
                    return;
                }
                
                var productId = productIdStr;
                var planProduct = planProducts.FirstOrDefault(pp => pp.ProductoId == productId);
                
                if (planProduct == null)
                {
                    UIHelper.ShowError($"Product with ID {productId} is not in this plan.");
                    return;
                }
                
                if (UIHelper.Confirm($"Are you sure you want to remove this product from plan '{plan.Nombre}'?"))
                {
                    _context.Set<PlanProducto>().Remove(planProduct);
                    _context.SaveChanges();
                    UIHelper.ShowSuccess("Product removed from plan successfully.");
                }
                else
                {
                    UIHelper.ShowWarning("Operation cancelled by user.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error removing products from plan", ex);
            }
        }
        
        // Maintain backward compatibility with Spanish methods
        private void ListarPlanes() => ListPlans();
        private void CrearPlan() => CreatePlan();
        private void EditarPlan() => UpdatePlan();
        private void EliminarPlan() => DeletePlan();
        private void AgregarProductosAlPlan() => AddProductsToPlan();
        private void VerProductosEnPlan() => ViewProductsInPlan();
        private void EliminarProductosDelPlan() => RemoveProductsFromPlan();
    }
} 