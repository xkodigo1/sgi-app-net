# 🏪 Sistema de Gestión Integrado (SGI)

Sistema de consola para la gestión de inventario, ventas, compras y administración general de negocios, desarrollado en C# con conexión a MySQL.

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-005C84?style=for-the-badge&logo=mysql&logoColor=white)

## 📋 Descripción

SGI es una aplicación de consola robusta diseñada para la gestión integral de negocios, permitiendo administrar clientes, productos, proveedores, ventas, compras y movimientos de caja. Implementa una arquitectura limpia y está desarrollada con buenas prácticas de programación.

## 🏗️ Arquitectura

El proyecto sigue los principios de **Arquitectura Limpia (Clean Architecture)** con tres capas principales:

- **Domain**: Contiene las entidades de negocio y reglas de dominio
- **Infrastructure**: Gestiona la persistencia y acceso a datos
- **Application**: Implementa la lógica de aplicación y la interfaz de usuario

## ✨ Características principales

- **Paneles CRUD completos** para todas las entidades:
  - 👤 Clientes
  - 🛒 Compras y Detalles de compra
  - 👨‍💼 Empleados
  - 📦 Productos 
  - 🏭 Proveedores
  - 🧾 Terceros
  - 💰 Ventas y Detalles de venta
  - 💸 Movimientos de Caja

- **Interfaz de consola intuitiva** con:
  - 🎨 Sistema de colores consistente
  - 📊 Tablas formateadas para visualización de datos
  - ✅ Validaciones completas con mensajes claros
  - ❓ Confirmaciones para operaciones importantes
  - ⚠️ Manejo de errores robusto

- **Características avanzadas**:
  - 🔄 Validación de integridad referencial
  - 🛡️ Verificación de restricciones de clave foránea
  - 🚫 Prevención de IDs duplicados
  - 📋 Informes básicos de ventas y movimientos

## 📋 Requisitos previos

- .NET 6.0 o superior
- MySQL 5.7 o superior
- Usuario y base de datos MySQL configurados

## 🔧 Instalación y configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/sgi-app.git
cd sgi-app
```

### 2. Configurar la base de datos

1. Crear una base de datos MySQL llamada `sgi-db`
2. Crear un usuario con permisos sobre la base de datos:
   ```sql
   CREATE USER 'sgiapp'@'localhost' IDENTIFIED BY 'kodigo777';
   GRANT ALL PRIVILEGES ON `sgi-db`.* TO 'sgiapp'@'localhost';
   FLUSH PRIVILEGES;
   ```
3. Ejecutar el script de creación de tablas:
   ```bash
   mysql -u sgiapp -p sgi-db < database/scripts/create_tables.sql
   ```

> ⚠️ **Nota:** Si necesitas personalizar la conexión a la base de datos, modifica el archivo `infrastructure/sql/MySqlSingletonConnection.cs`

### 3. Compilar y ejecutar la aplicación

```bash
dotnet build
dotnet run
```

## 📁 Estructura del proyecto

```
sgi-app/
├── domain/
│   └── entities/     # Entidades de negocio
├── infrastructure/
│   ├── repositories/ # Implementación de repositorios
│   └── sql/          # Conexión a base de datos
├── application/
│   └── ui/           # Paneles de interfaz de usuario
└── Program.cs        # Punto de entrada
```

## 🚀 Uso del programa

1. **Inicio**: Al ejecutar la aplicación, se mostrará un menú principal con todos los módulos disponibles.
2. **Navegación**: Use los números del teclado para seleccionar las diferentes opciones.
3. **Operaciones**: Cada entidad tiene su propio panel con funciones CRUD (Listar, Crear, Editar, Eliminar).
4. **Validación**: El sistema valida la integridad referencial entre entidades y previene operaciones inválidas.
5. **Finalización**: Use la opción "0" o "Salir" para volver al menú anterior o salir de la aplicación.

### Flujo básico de trabajo:

1. Registre **Terceros** (personas físicas o jurídicas)
2. Configure **Clientes** y **Proveedores** asociados a terceros
3. Registre **Productos** en inventario
4. Cree **Compras** a proveedores y sus **Detalles**
5. Registre **Ventas** a clientes y sus **Detalles**
6. Gestione **Movimientos de Caja** para registrar otras entradas y salidas

## 🔒 Seguridad

El sistema implementa validaciones para garantizar la integridad de los datos:

- Verifica la existencia de registros relacionados antes de crear o actualizar
- Valida restricciones de clave foránea para evitar referencias inválidas
- Previene eliminaciones que afectarían la integridad referencial

## 🛠️ Tecnologías utilizadas

- **C# (.NET)**: Lenguaje principal y framework
- **Entity Framework Core**: ORM para acceso a datos
- **MySQL**: Sistema de gestión de base de datos
- **Arquitectura Limpia**: Patrón de arquitectura 
- **Patrón Repositorio**: Para abstracción de acceso a datos
- **Singleton**: Para manejo de conexión a base de datos

## 👥 Contribución

Si deseas contribuir al proyecto:

1. Realiza un fork del repositorio
2. Crea una nueva rama (`git checkout -b feature/amazing-feature`)
3. Realiza tus cambios y commitea (`git commit -m 'feat: add amazing feature'`)
4. Envía tu rama (`git push origin feature/amazing-feature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

---

Desarrollado con esfuerzo como proyecto educativo
