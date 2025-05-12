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
                UIHelper.MostrarTitulo("Panel de Planes Promocionales");
                
                var opciones = new Dictionary<string, string>
                {
                    { "1", "Listar Planes Promocionales" },
                    { "2", "Crear Nuevo Plan Promocional" },
                    { "3", "Editar Plan Promocional" },
                    { "4", "Eliminar Plan Promocional" },
                    { "5", "Agregar Productos al Plan" },
                    { "6", "Ver Productos en Plan" },
                    { "7", "Eliminar Productos del Plan" }
                };
                
                UIHelper.MostrarMenuOpciones(opciones);

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListarPlanes();
                        break;
                    case "2":
                        CrearPlan();
                        break;
                    case "3":
                        EditarPlan();
                        break;
                    case "4":
                        EliminarPlan();
                        break;
                    case "5":
                        AgregarProductosAlPlan();
                        break;
                    case "6":
                        VerProductosEnPlan();
                        break;
                    case "7":
                        EliminarProductosDelPlan();
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

        private void ListarPlanes()
        {
            UIHelper.MostrarTitulo("Listado de Planes Promocionales");
            
            try
            {
                var planes = _context.Set<Plan>().ToList();
                
                var columnas = new Dictionary<string, Func<Plan, object>>
                {
                    { "ID", p => p.Id },
                    { "Nombre", p => p.Nombre },
                    { "Fecha Inicio", p => p.FechaInicio.ToShortDateString() },
                    { "Fecha Fin", p => p.FechaFin.ToShortDateString() },
                    { "Descuento %", p => p.Dcto }
                };
                
                UIHelper.DibujarTabla(planes, columnas, "Planes Promocionales Activos");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al listar los planes", ex);
            }
        }

        private void CrearPlan()
        {
            UIHelper.MostrarTitulo("Crear Nuevo Plan Promocional");
            
            try
            {
                var nombre = UIHelper.SolicitarEntrada("Ingrese el nombre del plan");
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    UIHelper.MostrarAdvertencia("El nombre es obligatorio.");
                    return;
                }

                var fechaInicioStr = UIHelper.SolicitarEntrada("Ingrese la fecha de inicio (YYYY-MM-DD)");
                var fechaInicio = DateTime.Parse(fechaInicioStr);

                var fechaFinStr = UIHelper.SolicitarEntrada("Ingrese la fecha de fin (YYYY-MM-DD)");
                var fechaFin = DateTime.Parse(fechaFinStr);

                if (fechaFin <= fechaInicio)
                {
                    UIHelper.MostrarError("La fecha de fin debe ser posterior a la fecha de inicio.");
                    return;
                }

                var dctoStr = UIHelper.SolicitarEntrada("Ingrese el porcentaje de descuento");
                var dcto = double.Parse(dctoStr);

                if (dcto <= 0 || dcto > 100)
                {
                    UIHelper.MostrarError("El descuento debe estar entre 0 y 100.");
                    return;
                }

                var plan = new Plan
                {
                    Nombre = nombre,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Dcto = dcto
                };

                if (UIHelper.Confirmar("¿Desea guardar este plan promocional?"))
                {
                    _context.Set<Plan>().Add(plan);
                    _context.SaveChanges();
                    UIHelper.MostrarExito("Plan promocional creado exitosamente.");
                }
                else
                {
                    UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al crear el plan", ex);
            }
        }

        private void EditarPlan()
        {
            UIHelper.MostrarTitulo("Editar Plan Promocional");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del plan a editar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("El ID es obligatorio.");
                    return;
                }

                var id = int.Parse(idStr);
                var plan = _context.Set<Plan>().Find(id);

                if (plan != null)
                {
                    UIHelper.MostrarTitulo("Información Actual");
                    Console.WriteLine($"ID: {plan.Id}");
                    Console.WriteLine($"Nombre: {plan.Nombre}");
                    Console.WriteLine($"Fecha Inicio: {plan.FechaInicio.ToShortDateString()}");
                    Console.WriteLine($"Fecha Fin: {plan.FechaFin.ToShortDateString()}");
                    Console.WriteLine($"Descuento: {plan.Dcto}%");
                    Console.WriteLine("\nIngrese nuevos valores o deje en blanco para mantener los actuales:");

                    var nombre = UIHelper.SolicitarEntrada("Nuevo nombre", plan.Nombre);
                    plan.Nombre = nombre;

                    var fechaInicioStr = UIHelper.SolicitarEntrada("Nueva fecha de inicio (YYYY-MM-DD)", plan.FechaInicio.ToString("yyyy-MM-dd"));
                    plan.FechaInicio = DateTime.Parse(fechaInicioStr);

                    var fechaFinStr = UIHelper.SolicitarEntrada("Nueva fecha de fin (YYYY-MM-DD)", plan.FechaFin.ToString("yyyy-MM-dd"));
                    plan.FechaFin = DateTime.Parse(fechaFinStr);

                    var dctoStr = UIHelper.SolicitarEntrada("Nuevo porcentaje de descuento", plan.Dcto.ToString());
                    plan.Dcto = double.Parse(dctoStr);

                    if (UIHelper.Confirmar("¿Confirma estos cambios?"))
                    {
                        _context.Update(plan);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Plan promocional actualizado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Plan no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al actualizar el plan", ex);
            }
        }

        private void EliminarPlan()
        {
            UIHelper.MostrarTitulo("Eliminar Plan Promocional");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del plan a eliminar");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("El ID es obligatorio.");
                    return;
                }

                var id = int.Parse(idStr);
                var plan = _context.Set<Plan>().Find(id);

                if (plan != null)
                {
                    UIHelper.MostrarTitulo("Información del Plan a Eliminar");
                    Console.WriteLine($"ID: {plan.Id}");
                    Console.WriteLine($"Nombre: {plan.Nombre}");
                    Console.WriteLine($"Fecha Inicio: {plan.FechaInicio.ToShortDateString()}");
                    Console.WriteLine($"Fecha Fin: {plan.FechaFin.ToShortDateString()}");
                    Console.WriteLine($"Descuento: {plan.Dcto}%");

                    if (UIHelper.Confirmar("¿Está seguro que desea eliminar este plan?"))
                    {
                        // Primero eliminar las relaciones con productos
                        var productosEnPlan = _context.Set<PlanProducto>().Where(pp => pp.PlanId == id);
                        _context.Set<PlanProducto>().RemoveRange(productosEnPlan);
                        
                        // Luego eliminar el plan
                        _context.Set<Plan>().Remove(plan);
                        _context.SaveChanges();
                        UIHelper.MostrarExito("Plan promocional eliminado exitosamente.");
                    }
                    else
                    {
                        UIHelper.MostrarAdvertencia("Operación cancelada por el usuario.");
                    }
                }
                else
                {
                    UIHelper.MostrarError("Plan no encontrado.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar el plan", ex);
            }
        }

        private void AgregarProductosAlPlan()
        {
            UIHelper.MostrarTitulo("Agregar Productos al Plan");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del plan");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("El ID del plan es obligatorio.");
                    return;
                }

                var planId = int.Parse(idStr);
                var plan = _context.Set<Plan>().Find(planId);

                if (plan == null)
                {
                    UIHelper.MostrarError("Plan no encontrado.");
                    return;
                }

                while (true)
                {
                    var productoId = UIHelper.SolicitarEntrada("Ingrese el ID del producto (0 para terminar)");
                    if (productoId == "0") break;

                    var producto = _context.Set<Producto>().Find(productoId);
                    if (producto == null)
                    {
                        UIHelper.MostrarError("Producto no encontrado.");
                        continue;
                    }

                    // Verificar si el producto ya está en el plan
                    var existente = _context.Set<PlanProducto>()
                        .Any(pp => pp.PlanId == planId && pp.ProductoId == productoId);

                    if (existente)
                    {
                        UIHelper.MostrarAdvertencia("Este producto ya está en el plan.");
                        continue;
                    }

                    var planProducto = new PlanProducto
                    {
                        PlanId = planId,
                        ProductoId = productoId
                    };

                    _context.Set<PlanProducto>().Add(planProducto);
                    _context.SaveChanges();
                    UIHelper.MostrarExito($"Producto {productoId} agregado al plan.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al agregar productos al plan", ex);
            }
        }

        private void VerProductosEnPlan()
        {
            UIHelper.MostrarTitulo("Ver Productos en Plan");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del plan");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("El ID del plan es obligatorio.");
                    return;
                }

                var planId = int.Parse(idStr);
                
                // Obtener los productos usando una consulta directa
                var productos = _context.Set<Producto>()
                    .Join(_context.Set<PlanProducto>(),
                        p => p.Id,
                        pp => pp.ProductoId,
                        (p, pp) => new { Producto = p, PlanProducto = pp })
                    .Where(x => x.PlanProducto.PlanId == planId)
                    .Select(x => x.Producto)
                    .ToList();

                if (!productos.Any())
                {
                    UIHelper.MostrarAdvertencia("No hay productos en este plan.");
                    return;
                }

                var columnas = new Dictionary<string, Func<Producto, object>>
                {
                    { "ID", p => p.Id },
                    { "Nombre", p => p.Nombre },
                    { "Precio", p => p.Precio.ToString("C2") },
                    { "Stock", p => p.Stock }
                };

                UIHelper.DibujarTabla(productos, columnas, $"Productos en Plan");
                
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al ver productos en el plan", ex);
            }
        }

        private void EliminarProductosDelPlan()
        {
            UIHelper.MostrarTitulo("Eliminar Productos del Plan");
            
            try
            {
                var idStr = UIHelper.SolicitarEntrada("Ingrese el ID del plan");
                if (string.IsNullOrWhiteSpace(idStr))
                {
                    UIHelper.MostrarAdvertencia("El ID del plan es obligatorio.");
                    return;
                }

                var planId = int.Parse(idStr);
                var plan = _context.Set<Plan>().Find(planId);

                if (plan == null)
                {
                    UIHelper.MostrarError("Plan no encontrado.");
                    return;
                }

                while (true)
                {
                    var productoId = UIHelper.SolicitarEntrada("Ingrese el ID del producto a eliminar (0 para terminar)");
                    if (productoId == "0") break;

                    var planProducto = _context.Set<PlanProducto>()
                        .FirstOrDefault(pp => pp.PlanId == planId && pp.ProductoId == productoId);

                    if (planProducto == null)
                    {
                        UIHelper.MostrarAdvertencia("Este producto no está en el plan.");
                        continue;
                    }

                    _context.Set<PlanProducto>().Remove(planProducto);
                    _context.SaveChanges();
                    UIHelper.MostrarExito($"Producto {productoId} eliminado del plan.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.MostrarError("Error al eliminar productos del plan", ex);
            }
        }
    }
} 