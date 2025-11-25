dotnet ef migrations add InitialMigration

dotnet ef database update --context ApplicationDbContext

# normal

dotnet run

# or with auto-reload while you code

dotnet watch run

INSERT INTO dbo.Usuario (CreatedDate, [Name], [Desc])
VALUES
('2025-11-01', N'Laura Soto', N'Tesorería'),
('2025-11-02', N'Miguel Díaz', N'Compras'),
(CONVERT(date, GETUTCDATE()), N'Paola Rivas', N'RH');
