CREATE DATABASE IF NOT EXISTS inmobiliaria;
USE inmobiliaria;

CREATE TABLE IF NOT EXISTS Propietarios (
    IdPropietario INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(20),
    Email VARCHAR(100) NOT NULL UNIQUE,
    Clave VARCHAR(255) NOT NULL,
    Activo TINYINT(1) DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Inquilinos (
    IdInquilino INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(20),
    Email VARCHAR(100) NOT NULL UNIQUE,
    Activo TINYINT(1) DEFAULT 1
);

-- Opcional: Insertar datos de prueba
INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email, Clave) VALUES 
('Juan', 'Perez', '12345678', '2664123456', 'juan@mail.com', '1234'),
('Maria', 'Gomez', '87654321', '2664654321', 'maria@mail.com', '1234');

INSERT INTO Inquilinos (Nombre, Apellido, Dni, Telefono, Email) VALUES 
('Carlos', 'Lopez', '11223344', '2664112233', 'carlos@mail.com'),
('Ana', 'Martinez', '44332211', '2664443322', 'ana@mail.com');
