# CREAR LA MIGRACION EN LA DB

dotnet ef migrations add InitialMigration

dotnet ef database update
o
dotnet ef database update --context ApplicationDbContext

# normal

dotnet run

# or with auto-reload while you code

dotnet watch run

# Te sientes perdido? -> entra a este url para ver la api info

http://localhost:5136/swagger/index.html

# DB SEED

INSERT INTO dbo.Usuario (CreatedDate, [Name], [Desc])
VALUES
('2025-11-01', N'Laura Soto', N'Tesorería'),
('2025-11-02', N'Miguel Díaz', N'Compras'),
(CONVERT(date, GETUTCDATE()), N'Paola Rivas', N'RH');

INSERT INTO dbo.FondoMonetario (Nombre, TipoFondo, NumeroCuenta, Descripcion)
VALUES (N'Cuenta Corriente', N'CuentaBancaria', N'123-456', N'Principal');
