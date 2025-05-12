-- tipo_documentos
INSERT INTO tipo_documentos (Descripcion) VALUES ('Cédula');
INSERT INTO tipo_documentos (Descripcion) VALUES ('NIT');
INSERT INTO tipo_documentos (Descripcion) VALUES ('Pasaporte');

-- Pais
INSERT INTO Pais (Nombre) VALUES ('Colombia');
INSERT INTO Pais (Nombre) VALUES ('México');
INSERT INTO Pais (Nombre) VALUES ('Argentina');

-- Region
INSERT INTO Region (Nombre, Pais_id) VALUES ('Antioquia', 1);
INSERT INTO Region (Nombre, Pais_id) VALUES ('Cundinamarca', 1);
INSERT INTO Region (Nombre, Pais_id) VALUES ('Jalisco', 2);

-- Ciudad
INSERT INTO Ciudad (Nombre, Region_id) VALUES ('Medellín', 1);
INSERT INTO Ciudad (Nombre, Region_id) VALUES ('Bogotá', 2);
INSERT INTO Ciudad (Nombre, Region_id) VALUES ('Guadalajara', 3);

-- tipo_terceros
INSERT INTO tipo_terceros (descripcion) VALUES ('Empleado');
INSERT INTO tipo_terceros (descripcion) VALUES ('Cliente');
INSERT INTO tipo_terceros (descripcion) VALUES ('Proveedor');

-- Terceros
INSERT INTO Terceros (id, Nombre, Apellidos, Email, TipoDoc_id, TipoTercero_id, Ciudad_id) VALUES
('1234567890', 'Juan', 'Pérez', 'juan.perez@example.com', 1, 1, 1),
('2345678901', 'Ana', 'Martínez', 'ana.martinez@example.com', 2, 2, 2),
('3456789012', 'Carlos', 'Ramírez', 'carlos.ramirez@example.com', 3, 3, 3);

-- tercero_telefonos
INSERT INTO tercero_telefonos (numero, tipo, Tercero_id) VALUES
('3001234567', 'Móvil', '1234567890'),
('3012345678', 'Fijo', '2345678901'),
('3023456789', 'Móvil', '3456789012');

-- Proveedores
INSERT INTO Proveedores (Tercero_id, Dcto, DiaPago) VALUES
('3456789012', 0.5, 15),
('2345678901', 0.25, 30),
('1234567890', 0, 10);

-- EPS
INSERT INTO EPS (Nombre) VALUES ('SURA');
INSERT INTO EPS (Nombre) VALUES ('Coomeva');
INSERT INTO EPS (Nombre) VALUES ('Sanitas');

-- ARL
INSERT INTO ARL (Nombre) VALUES ('ARL SURA');
INSERT INTO ARL (Nombre) VALUES ('Colpatria');
INSERT INTO ARL (Nombre) VALUES ('AXA Colpatria');

-- Empleados
INSERT INTO Empleados (Tercero_id, FechaIngreso, SalarioBase, Eps_id, arl_id) VALUES
('1234567890', '2023-01-10', 2000000, 1, 1),
('2345678901', '2022-06-15', 2500000, 2, 2),
('3456789012', '2024-03-20', 1800000, 3, 3);

-- Clientes
INSERT INTO Clientes (Tercero_id, FechaNac, FechaCompra) VALUES
('2345678901', '1990-05-15', '2025-01-10'),
('1234567890', '1985-09-20', '2025-02-15'),
('3456789012', '1995-12-01', '2025-03-22');

-- Productos
INSERT INTO Productos (id, Nombre, Stock, StockMin, StockMax, Precio, CreatedAt, UpdatedAt) VALUES
('P001', 'Laptop ASUS', 10, 2, 20, 3500000.00, '2025-01-01', '2025-03-01'),
('P002', 'Monitor LG', 15, 5, 30, 1200000.00, '2025-01-05', '2025-04-01'),
('P003', 'Mouse Logitech', 50, 10, 100, 80000.00, '2025-01-10', '2025-04-10');

-- Compras
INSERT INTO Compras (TerceroProv_id, Fecha, TerceroEmp_id, DocCompra) VALUES
('3456789012', '2025-01-20', '1234567890', 'COMP-001'),
('3456789012', '2025-02-15', '2345678901', 'COMP-002'),
('3456789012', '2025-03-10', '3456789012', 'COMP-003');

-- Facturacion
INSERT INTO Facturacion (FechaResolucion, Numero, NumFinal, FechaActual) VALUES
('2025-01-01', 1000, 1999, '2025-01-01'),
('2025-02-01', 2000, 2999, '2025-02-01'),
('2025-03-01', 3000, 3999, '2025-03-01');

-- Venta
INSERT INTO Venta (Fecha, Fact_id, TerceroEn_id, TerceroCli_id) VALUES
('2025-04-01', 1, '1234567890', '2345678901'),
('2025-04-10', 2, '2345678901', '3456789012'),
('2025-04-15', 3, '3456789012', '1234567890');

-- detalle_venta
INSERT INTO detalle_venta (venta_id, Productos_id, Cantidad, Valor) VALUES
(1, 'P001', 1, 3500000.00),
(2, 'P002', 2, 2400000.00),
(3, 'P003', 5, 400000.00);

-- detalle_compra
INSERT INTO detalle_compra (Fecha, Producto_id, Cantidad, Valor, Compra_id) VALUES
('2025-01-20', 'P001', 5, 3000000.00, 1),
('2025-02-15', 'P002', 10, 1000000.00, 2),
('2025-03-10', 'P003', 20, 60000.00, 3);

-- productos_proveedores
INSERT INTO productos_proveedores (Producto_id, Proveedor_id) VALUES
('P001', 1),
('P002', 1),
('P003', 1);

-- tipo_MovCaja
INSERT INTO tipo_MovCaja (Nombre, Tipo) VALUES
('Ingreso por venta', 'Ingreso'),
('Pago proveedor', 'Egreso'),
('Gasto administrativo', 'Egreso');

-- MovCaja
INSERT INTO MovCaja (Fecha, TipoMov, Valor, Concepto, Tercero_id) VALUES
('2025-04-01', 1, 3500000.00, 'Venta Laptop', '1234567890'),
('2025-04-05', 2, 2400000.00, 'Pago proveedor', '3456789012'),
('2025-04-10', 3, 500000.00, 'Alquiler oficina', '2345678901');

-- Planes
INSERT INTO Planes (Nombre, FechaInicio, FechaFin, Dcto) VALUES
('Plan Básico', '2025-01-01', '2025-12-31', 5.0),
('Plan Premium', '2025-01-01', '2025-12-31', 10.0),
('Plan Empresarial', '2025-01-01', '2025-12-31', 15.0);

-- plan_Producto
INSERT INTO plan_Producto (Plan_id, Producto_id) VALUES
(1, 'P001'),
(2, 'P002'),
(3, 'P003');

-- Empresa
INSERT INTO Empresa (nombre, fecha_reg, Ciudad_id) VALUES
('Tech Solutions', '2023-05-01', 1),
('Logis S.A.', '2024-06-15', 2),
('Comercial XYZ', '2025-01-10', 3);
