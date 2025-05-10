-- tipo_documentos
INSERT INTO tipo_documentos (Descripcion) VALUES ('Cédula'), ('Pasaporte');

-- tipo_terceros
INSERT INTO tipo_terceros (descripcion) VALUES ('Cliente'), ('Empleado'), ('Proveedor');

-- Pais
INSERT INTO Pais (Nombre) VALUES ('Colombia');

-- Region
INSERT INTO Region (Nombre, Pais_id) VALUES ('Antioquia', 1);

-- Ciudad
INSERT INTO Ciudad (Nombre, Region_id) VALUES ('Medellín', 1);

-- EPS
INSERT INTO EPS (Nombre) VALUES ('SURA');

-- ARL
INSERT INTO ARL (Nombre) VALUES ('Colpatria');

-- Terceros
INSERT INTO Terceros (id, Nombre, Apellidos, Email, TipoDoc_id, TipoTercero_id, Ciudad_id) VALUES
('123', 'Juan', 'Pérez', 'juan@mail.com', 1, 1, 1),
('456', 'Ana', 'Gómez', 'ana@mail.com', 1, 2, 1),
('789', 'Proveedor', 'Global', 'prov@global.com', 2, 3, 1);

-- tercero_telefonos
INSERT INTO tercero_telefonos (numero, tipo, Tercero_id) VALUES
('3001234567', 'Móvil', '123'),
('3007654321', 'Móvil', '456');

-- Proveedores
INSERT INTO Proveedores (Tercero_id, Dcto, DiaPago) VALUES
('789', 10.5, 15);

-- Empleados
INSERT INTO Empleados (Tercero_id, FechaIngreso, SalarioBase, Eps_id, arl_id) VALUES
('456', '2023-01-01', 3000000, 1, 1);

-- Clientes
INSERT INTO Clientes (Tercero_id, FechaNac, FechaCompra) VALUES
('123', '1990-05-05', '2024-04-10');

-- Productos
INSERT INTO Productos (id, Nombre, Stock, StockMin, StockMax, CreatedAt, UpdatedAt, Barcode) VALUES
('P001', 'Teclado', 50, 10, 100, '2024-01-01', '2024-05-01', 'ABC123XYZ'),
('P002', 'Mouse', 75, 20, 200, '2024-01-05', '2024-05-02', 'DEF456UVW');

-- Compras
INSERT INTO Compras (TerceroProv_id, Fecha, TerceroEmp_id, DocCompra) VALUES
('789', '2024-05-01', '456', 'COMP-001');

-- detalle_compra
INSERT INTO detalle_compra (Fecha, Producto_id, Cantidad, Valor, Compra_id) VALUES
('2024-05-01', 'P001', 10, 15000.00, 1),
('2024-05-01', 'P002', 20, 10000.00, 1);

-- productos_proveedores
INSERT INTO productos_proveedores (Producto_id, Proveedor_id) VALUES
('P001', 1),
('P002', 1);

-- Facturación
INSERT INTO Facturacion (FechaResolucion, Numero, NumFinal, FechaActual) VALUES
('2024-01-01', 1000, 2000, '2024-05-01');

-- Venta
INSERT INTO Venta (Fecha, Fact_id, TerceroEn_id, TerceroCli_id) VALUES
('2024-05-02', 1, '456', '123');

-- detalle_venta
INSERT INTO detalle_venta (venta_id, Productos_id, Cantidad, Valor) VALUES
(1, 'P001', 2, 20000.00),
(1, 'P002', 1, 12000.00);

-- tipo_MovCaja
INSERT INTO tipo_MovCaja (Nombre, Tipo) VALUES
('Ingreso por venta', 'Ingreso'),
('Egreso por compra', 'Egreso');

-- MovCaja
INSERT INTO MovCaja (Fecha, TipoMov, Valor, Concepto, Tercero_id) VALUES
('2024-05-02', 1, 52000.00, 'Venta productos', '123'),
('2024-05-01', 2, 350000.00, 'Compra inventario', '789');

-- Planes
INSERT INTO Planes (Nombre, FechaInicio, FechaFin, Dcto) VALUES
('Plan Primavera', '2024-04-01', '2024-06-30', 15.0);

-- plan_Producto
INSERT INTO plan_Producto (Plan_id, Producto_id) VALUES
(1, 'P001'),
(1, 'P002');

-- Empresa
INSERT INTO Empresa (nombre, fecha_reg, Ciudad_id) VALUES
('Tech Store', '2024-01-15', 1);
