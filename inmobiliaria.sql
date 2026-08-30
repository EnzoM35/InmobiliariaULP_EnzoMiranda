CREATE DATABASE IF NOT EXISTS inmobiliaria;
USE inmobiliaria;

-- Tabla Propietarios
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

-- Tabla Inquilinos
CREATE TABLE IF NOT EXISTS Inquilinos (
    IdInquilino INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(20),
    Email VARCHAR(100) NOT NULL UNIQUE,
    Activo TINYINT(1) DEFAULT 1
);

-- Tabla TiposInmueble
CREATE TABLE IF NOT EXISTS TiposInmueble (
    IdTipoInmueble INT AUTO_INCREMENT PRIMARY KEY,
    Descripcion VARCHAR(50) NOT NULL UNIQUE,
    Activo TINYINT(1) DEFAULT 1
);

-- Tabla Inmuebles
CREATE TABLE IF NOT EXISTS Inmuebles (
    IdInmueble INT AUTO_INCREMENT PRIMARY KEY,
    Direccion VARCHAR(150) NOT NULL,
    Cupo INT NOT NULL DEFAULT 1,
    Latitud DECIMAL(10,8) NULL,
    Longitud DECIMAL(11,8) NULL,
    PrecioDia DECIMAL(10,2) NOT NULL,
    PorcentajeReserva DECIMAL(5,2) NOT NULL DEFAULT 10.00,
    Disponible TINYINT(1) NOT NULL DEFAULT 1,
    Portada VARCHAR(255) NULL,
    IdTipoInmueble INT NOT NULL,
    IdPropietario INT NOT NULL,
    Activo TINYINT(1) DEFAULT 1,
    CONSTRAINT FK_Inmueble_Tipo FOREIGN KEY (IdTipoInmueble) REFERENCES TiposInmueble(IdTipoInmueble),
    CONSTRAINT FK_Inmueble_Propietario FOREIGN KEY (IdPropietario) REFERENCES Propietarios(IdPropietario)
);

-- Tabla Reservas
CREATE TABLE IF NOT EXISTS Reservas (
    IdReserva INT AUTO_INCREMENT PRIMARY KEY,
    IdInquilino INT NOT NULL,
    IdInmueble INT NOT NULL,
    FechaDesde DATE NOT NULL,
    FechaHasta DATE NOT NULL,
    PrecioPorDia DECIMAL(10,2) NOT NULL,
    MontoTotal DECIMAL(10,2) NOT NULL,
    FechaTerminacion DATETIME NULL,
    Multa DECIMAL(10,2) DEFAULT 0,
    Estado VARCHAR(30) NOT NULL DEFAULT 'Vigente',
    Activo TINYINT(1) DEFAULT 1,
    CONSTRAINT FK_Reserva_Inquilino FOREIGN KEY (IdInquilino) REFERENCES Inquilinos(IdInquilino),
    CONSTRAINT FK_Reserva_Inmueble FOREIGN KEY (IdInmueble) REFERENCES Inmuebles(IdInmueble)
);

-- Datos de prueba
INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email, Clave, Activo) VALUES 
('Juan', 'Perez', '12345678', '2664123456', 'juan@mail.com', '1234', 1),
('Maria', 'Gomez', '87654321', '2664654321', 'maria@mail.com', '1234', 1)
ON DUPLICATE KEY UPDATE Nombre=VALUES(Nombre);

INSERT INTO Inquilinos (Nombre, Apellido, Dni, Telefono, Email, Activo) VALUES 
('Carlos', 'Lopez', '11223344', '2664112233', 'carlos@mail.com', 1),
('Ana', 'Martinez', '44332211', '2664443322', 'ana@mail.com', 1)
ON DUPLICATE KEY UPDATE Nombre=VALUES(Nombre);

INSERT INTO TiposInmueble (Descripcion, Activo) VALUES 
('Casa', 1),
('Departamento', 1),
('Monoambiente', 1),
('Loft', 1),
('Cabaña', 1)
ON DUPLICATE KEY UPDATE Descripcion=VALUES(Descripcion);

INSERT INTO Inmuebles (Direccion, Cupo, Latitud, Longitud, PrecioDia, PorcentajeReserva, Disponible, Portada, IdTipoInmueble, IdPropietario, Activo) VALUES 
('Av. Illia 450, San Luis', 4, -33.29910000, -66.33560000, 45000.00, 20.00, 1, '/img/casa1.jpg', 1, 1, 1),
('San Martín 780, San Luis', 2, -33.30150000, -66.33820000, 30000.00, 15.00, 1, '/img/depto1.jpg', 2, 2, 1);

INSERT INTO Reservas (IdInquilino, IdInmueble, FechaDesde, FechaHasta, PrecioPorDia, MontoTotal, Estado, Activo) VALUES 
(1, 1, '2026-09-01', '2026-09-07', 45000.00, 270000.00, 'Vigente', 1),
(2, 2, '2026-09-10', '2026-09-15', 30000.00, 150000.00, 'Vigente', 1);
