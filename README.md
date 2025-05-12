# 🏪 Integrated Management System (SGI)

Console system for inventory, sales, purchases, and general business management, developed in C# with MySQL connection.

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge\&logo=c-sharp\&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge\&logo=dotnet\&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-005C84?style=for-the-badge\&logo=mysql\&logoColor=white)

## 📋 Description

SGI is a robust console application designed for comprehensive business management, allowing you to handle clients, products, suppliers, sales, purchases, and cash flow. It follows a clean architecture and is developed using good programming practices.

## 🏗️ Architecture

The project follows **Clean Architecture** principles with three main layers:

* **Domain**: Contains business entities and domain rules
* **Infrastructure**: Manages data persistence and access
* **Application**: Implements application logic and the user interface

## ✨ Main Features

* **Full CRUD panels** for all entities:

  * 👤 Clients
  * 👒 Purchases and Purchase Details
  * 👨‍💼 Employees
  * 📦 Products
  * 🏢 Suppliers
  * 🧾 Third Parties
  * 💰 Sales and Sale Details
  * 💸 Cash Movements

* **Intuitive console interface** with:

  * 🎨 Consistent color scheme
  * 📊 Formatted tables for data display
  * ✅ Comprehensive validations with clear messages
  * ❓ Confirmations for important operations
  * ⚠️ Robust error handling

* **Advanced features**:

  * 🔄 Referential integrity validation
  * 🛡️ Foreign key constraint checks
  * 🚫 Duplicate ID prevention
  * 📋 Basic sales and cash flow reports

## 📋 Prerequisites

* .NET 6.0 or later
* MySQL 5.7 or later
* Configured MySQL user and database

## 🔧 Installation & Configuration

### 1. Clone the repository

```bash
git clone https://github.com/xkodigo1/sgi-app.git
cd sgi-app
```

### 2. Configure the database

1. Create a MySQL database named `sgi-db`
2. Create a user with privileges on that database:

   ```sql
   CREATE USER 'sgiapp'@'localhost' IDENTIFIED BY 'kodigo777';
   GRANT ALL PRIVILEGES ON `sgi-db`.* TO 'sgiapp'@'localhost';
   FLUSH PRIVILEGES;
   ```
3. Run the table creation script:

   ```bash
   mysql -u sgiapp -p sgi-db < database/scripts/create_tables.sql
   ```

> ⚠️ **Note:** If you need to customize the database connection, edit the file `infrastructure/sql/MySqlSingletonConnection.cs`

### 3. Build and run the application

```bash
dotnet build
dotnet run
```

## 📁 Project Structure

```
sgi-app/
├── domain/
│   └── entities/     # Business entities
├── infrastructure/
│   ├── repositories/ # Repository implementations
│   └── sql/          # Database connection
├── application/
│   └── ui/           # User interface panels
└── Program.cs        # Entry point
```

## 🚀 How to Use

1. **Startup**: Upon launching the app, a main menu with all available modules will be displayed.
2. **Navigation**: Use the keyboard numbers to select different options.
3. **Operations**: Each entity has its own panel with CRUD operations (List, Create, Edit, Delete).
4. **Validation**: The system validates referential integrity and prevents invalid operations.
5. **Exit**: Use option "0" or "Exit" to go back or close the app.

### Basic Workflow:

1. Register **Third Parties** (individuals or companies)
2. Set up **Clients** and **Suppliers** linked to third parties
3. Register **Products** in inventory
4. Create **Purchases** and related **Details**
5. Register **Sales** to clients and related **Details**
6. Manage **Cash Movements** for other income and expenses

## 🔒 Data Integrity & Security

The system includes validations to ensure data integrity:

* Checks for related records before insert/update
* Validates foreign key constraints to avoid invalid references
* Prevents deletions that would break referential integrity

## 🛠️ Technologies Used

* **C# (.NET)**: Main programming language and framework
* **Entity Framework Core**: ORM for data access
* **MySQL**: Relational database system
* **Clean Architecture**: Design pattern
* **Repository Pattern**: For data abstraction
* **Singleton**: For database connection management

## 👥 Contribution

If you want to contribute:

1. Fork the repository
2. Create a new branch (`git checkout -b feature/amazing-feature`)
3. Make your changes and commit (`git commit -m 'feat: add amazing feature'`)
4. Push the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License – see the [LICENSE](LICENSE) file for details.

---

Developed with effort as an educational project.
