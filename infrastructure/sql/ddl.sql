CREATE DATABASE IF NOT EXISTS `sgi-db`;
USE `sgi-db`;

CREATE TABLE `tipo_documentos` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Descripcion` VARCHAR(50)
);

CREATE TABLE `Pais` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(50)
);

CREATE TABLE `Region` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(50),
    `Pais_id` INT,
    FOREIGN KEY (`Pais_id`) REFERENCES `Pais`(`id`)
);

CREATE TABLE `Ciudad` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(50),
    `Region_id` INT,
    FOREIGN KEY (`Region_id`) REFERENCES `Region`(`id`)
);

CREATE TABLE `tipo_terceros` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `descripcion` VARCHAR(50)
);

CREATE TABLE `Terceros` (
    `id` VARCHAR(20) PRIMARY KEY,
    `Nombre` VARCHAR(50),
    `Apellidos` VARCHAR(50),
    `Email` VARCHAR(80) UNIQUE,
    `TipoDoc_id` INT,
    `TipoTercero_id` INT,
    `Ciudad_id` INT,
    FOREIGN KEY (`TipoDoc_id`) REFERENCES `tipo_documentos`(`id`),
    FOREIGN KEY (`TipoTercero_id`) REFERENCES `tipo_terceros`(`id`),
    FOREIGN KEY (`Ciudad_id`) REFERENCES `Ciudad`(`id`)
);

CREATE TABLE `tercero_telefonos` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `numero` VARCHAR(50),
    `tipo` VARCHAR(50),
    `Tercero_id` VARCHAR(20),
    FOREIGN KEY (`Tercero_id`) REFERENCES `Terceros`(`id`)
);

CREATE TABLE `Proveedores` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Tercero_id` VARCHAR(20),
    `Dcto` DOUBLE,
    `DiaPago` INT,
    FOREIGN KEY (`Tercero_id`) REFERENCES `Terceros`(`id`)
);

CREATE TABLE `EPS` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(50)
);

CREATE TABLE `ARL` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(50)
);

CREATE TABLE `Empleados` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Tercero_id` VARCHAR(20),
    `FechaIngreso` DATE,
    `SalarioBase` DOUBLE,
    `Eps_id` INT,
    `arl_id` INT,
    FOREIGN KEY (`Tercero_id`) REFERENCES `Terceros`(`id`),
    FOREIGN KEY (`Eps_id`) REFERENCES `EPS`(`id`),
    FOREIGN KEY (`arl_id`) REFERENCES `ARL`(`id`)
);

CREATE TABLE `Clientes` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Tercero_id` VARCHAR(20),
    `FechaNac` DATE,
    `FechaCompra` DATE,
    FOREIGN KEY (`Tercero_id`) REFERENCES `Terceros`(`id`)
);

CREATE TABLE `Productos` (
    `id` VARCHAR(20) PRIMARY KEY,
    `Nombre` VARCHAR(50),
    `Stock` INT,
    `StockMin` INT,
    `StockMax` INT,
    `CreatedAt` DATE,
    `UpdatedAt` DATE,
    `Barcode` VARCHAR(50) UNIQUE
);

CREATE TABLE `Compras` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `TerceroProv_id` VARCHAR(20),
    `Fecha` DATE,
    `TerceroEmp_id` VARCHAR(20),
    `DocCompra` VARCHAR(255),
    FOREIGN KEY (`TerceroProv_id`) REFERENCES `Terceros`(`id`),
    FOREIGN KEY (`TerceroEmp_id`) REFERENCES `Terceros`(`id`)
);

CREATE TABLE `Facturacion` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `FechaResolucion` VARCHAR(255),
    `Numero` INT,
    `NumFinal` INT,
    `FechaActual` DATE
);

CREATE TABLE `Venta` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Fecha` DATE,
    `Fact_id` INT,
    `TerceroEn_id` VARCHAR(20),
    `TerceroCli_id` VARCHAR(20),
    FOREIGN KEY (`Fact_id`) REFERENCES `Facturacion`(`id`),
    FOREIGN KEY (`TerceroEn_id`) REFERENCES `Terceros`(`id`),
    FOREIGN KEY (`TerceroCli_id`) REFERENCES `Terceros`(`id`)
);

CREATE TABLE `detalle_venta` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `venta_id` INT,
    `Productos_id` VARCHAR(20),
    `Cantidad` INT,
    `Valor` DECIMAL(10, 2),
    FOREIGN KEY (`venta_id`) REFERENCES `Venta`(`id`),
    FOREIGN KEY (`Productos_id`) REFERENCES `Productos`(`id`)
);

CREATE TABLE `detalle_compra` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Fecha` DATE,
    `Producto_id` VARCHAR(20),
    `Cantidad` INT,
    `Valor` DECIMAL(10, 2),
    `Compra_id` INT,
    FOREIGN KEY (`Producto_id`) REFERENCES `Productos`(`id`),
    FOREIGN KEY (`Compra_id`) REFERENCES `Compras`(`id`)
);

CREATE TABLE `productos_proveedores` (
    `Producto_id` VARCHAR(20),
    `Proveedor_id` INT,
    FOREIGN KEY (`Producto_id`) REFERENCES `Productos`(`id`),
    FOREIGN KEY (`Proveedor_id`) REFERENCES `Proveedores`(`id`)
);

CREATE TABLE `tipo_MovCaja` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(50),
    `Tipo` VARCHAR(50)
);

CREATE TABLE `MovCaja` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Fecha` DATE,
    `TipoMov` INT,
    `Valor` DECIMAL(10, 2),
    `Concepto` VARCHAR(50),
    `Tercero_id` VARCHAR(20),
    FOREIGN KEY (`TipoMov`) REFERENCES `tipo_MovCaja`(`id`),
    FOREIGN KEY (`Tercero_id`) REFERENCES `Terceros`(`id`)
);

CREATE TABLE `Planes` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(30),
    `FechaInicio` DATE,
    `FechaFin` DATE,
    `Dcto` DOUBLE
);

CREATE TABLE `plan_Producto` (
    `Plan_id` INT,
    `Producto_id` VARCHAR(20),
    FOREIGN KEY (`Plan_id`) REFERENCES `Planes`(`id`),
    FOREIGN KEY (`Producto_id`) REFERENCES `Productos`(`id`)
);

CREATE TABLE `Empresa` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `nombre` VARCHAR(50),
    `fecha_reg` DATE,
    `Ciudad_id` INT,
    FOREIGN KEY (`Ciudad_id`) REFERENCES `Ciudad`(`id`)
);