
-- Crear la base de datos 
CREATE DATABASE ReservaCanchaDB;
GO
 
USE ReservaCanchaDB;
GO
 
-- Tabla básica para registrar las reservas
CREATE TABLE Reservas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreCliente NVARCHAR(100) NOT NULL,
    TipoCancha NVARCHAR(50) NOT NULL,
    Horas INT NOT NULL,
    PrecioTotal DECIMAL(10,2) NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);
GO