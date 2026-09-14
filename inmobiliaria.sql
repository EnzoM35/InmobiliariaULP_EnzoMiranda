CREATE DATABASE IF NOT EXISTS inmobiliaria_ulp;
USE inmobiliaria_ulp;

-- =============================================================
-- TABLA USUARIOS (MÃ“DULO 1: AUTENTICACIÃ“N Y ROLES)
-- =============================================================
CREATE TABLE IF NOT EXISTS Usuarios (
    IdUsuario INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Rol VARCHAR(20) NOT NULL DEFAULT 'Empleado',
    AvatarUrl VARCHAR(255) NULL,
    Activo TINYINT(1) DEFAULT 1
);

-- Usuarios semilla (PasswordHasher admite tanto hash seguro con sal como fallback texto plano inicial)
INSERT INTO Usuarios (Nombre, Apellido, Email, PasswordHash, Rol, AvatarUrl, Activo) VALUES
('Enzo', 'Miranda', 'admin@inmobiliaria.com', 'admin123', 'Administrador', '/img/avatars/admin.png', 1),
('Carlos', 'Perez', 'empleado@inmobiliaria.com', 'empleado123', 'Empleado', '/img/avatars/empleado.png', 1)
ON DUPLICATE KEY UPDATE 
    Nombre=VALUES(Nombre), 
    Apellido=VALUES(Apellido), 
    Rol=VALUES(Rol),
    Activo=VALUES(Activo);

-- =============================================================
-- TABLA PROPIETARIOS
-- =============================================================
CREATE TABLE IF NOT EXISTS Propietarios (
    IdPropietario INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(20),
    Email VARCHAR(100) NOT NULL UNIQUE,
    Activo TINYINT(1) DEFAULT 1
);

-- Compatibilidad con entregas anteriores que tenÃ­an columna Clave
SET @tiene_clave := (
    SELECT COUNT(*)
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'Propietarios'
      AND COLUMN_NAME = 'Clave'
);
SET @sql_drop_clave := IF(@tiene_clave > 0, 'ALTER TABLE Propietarios DROP COLUMN Clave', 'SELECT 1');
PREPARE stmt FROM @sql_drop_clave;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- =============================================================
-- TABLA INQUILINOS
-- =============================================================
CREATE TABLE IF NOT EXISTS Inquilinos (
    IdInquilino INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(20),
    Email VARCHAR(100) NOT NULL UNIQUE,
    Activo TINYINT(1) DEFAULT 1
);

-- =============================================================
-- TABLA TIPOS DE INMUEBLE
-- =============================================================
CREATE TABLE IF NOT EXISTS TiposInmueble (
    IdTipoInmueble INT AUTO_INCREMENT PRIMARY KEY,
    Descripcion VARCHAR(50) NOT NULL UNIQUE,
    Activo TINYINT(1) DEFAULT 1
);

-- =============================================================
-- TABLA INMUEBLES
-- =============================================================
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

-- =============================================================
-- TABLA IMAGENES DE INMUEBLE (GALERIA)
-- =============================================================
CREATE TABLE IF NOT EXISTS ImagenesInmueble (
    IdImagen INT AUTO_INCREMENT PRIMARY KEY,
    IdInmueble INT NOT NULL,
    Url VARCHAR(255) NOT NULL,
    EsPortada TINYINT(1) NOT NULL DEFAULT 0,
    CONSTRAINT FK_Imagen_Inmueble FOREIGN KEY (IdInmueble) REFERENCES Inmuebles(IdInmueble) ON DELETE CASCADE
);

-- =============================================================
-- TABLA RESERVAS (INCLUYE AUDITORÃA Y RENOVACIONES)
-- =============================================================
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
    IdUsuarioCreador INT NULL,
    IdUsuarioTerminador INT NULL,
    IdReservaOrigen INT NULL,
    Activo TINYINT(1) DEFAULT 1,
    CONSTRAINT FK_Reserva_Inquilino FOREIGN KEY (IdInquilino) REFERENCES Inquilinos(IdInquilino),
    CONSTRAINT FK_Reserva_Inmueble FOREIGN KEY (IdInmueble) REFERENCES Inmuebles(IdInmueble),
    CONSTRAINT FK_Reserva_UsuarioCreador FOREIGN KEY (IdUsuarioCreador) REFERENCES Usuarios(IdUsuario),
    CONSTRAINT FK_Reserva_UsuarioTerminador FOREIGN KEY (IdUsuarioTerminador) REFERENCES Usuarios(IdUsuario),
    CONSTRAINT FK_Reserva_ReservaOrigen FOREIGN KEY (IdReservaOrigen) REFERENCES Reservas(IdReserva)
);

-- MigraciÃ³n segura de columnas en caso de base de datos existente
SET @col_creador := (SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Reservas' AND COLUMN_NAME = 'IdUsuarioCreador');
SET @sql_creador := IF(@col_creador = 0, 'ALTER TABLE Reservas ADD COLUMN IdUsuarioCreador INT NULL, ADD CONSTRAINT FK_Reserva_UsuarioCreador FOREIGN KEY (IdUsuarioCreador) REFERENCES Usuarios(IdUsuario)', 'SELECT 1');
PREPARE stmt FROM @sql_creador; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_terminador := (SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Reservas' AND COLUMN_NAME = 'IdUsuarioTerminador');
SET @sql_terminador := IF(@col_terminador = 0, 'ALTER TABLE Reservas ADD COLUMN IdUsuarioTerminador INT NULL, ADD CONSTRAINT FK_Reserva_UsuarioTerminador FOREIGN KEY (IdUsuarioTerminador) REFERENCES Usuarios(IdUsuario)', 'SELECT 1');
PREPARE stmt FROM @sql_terminador; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @col_origen := (SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Reservas' AND COLUMN_NAME = 'IdReservaOrigen');
SET @sql_origen := IF(@col_origen = 0, 'ALTER TABLE Reservas ADD COLUMN IdReservaOrigen INT NULL, ADD CONSTRAINT FK_Reserva_ReservaOrigen FOREIGN KEY (IdReservaOrigen) REFERENCES Reservas(IdReserva)', 'SELECT 1');
PREPARE stmt FROM @sql_origen; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- =============================================================
-- TABLA PAGOS (MÃ“DULO 2: PAGOS Y AUDITORÃA)
-- =============================================================
CREATE TABLE IF NOT EXISTS Pagos (
    IdPago INT AUTO_INCREMENT PRIMARY KEY,
    IdReserva INT NOT NULL,
    NumeroPago INT NOT NULL DEFAULT 1,
    FechaPago DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Importe DECIMAL(10,2) NOT NULL,
    Concepto VARCHAR(255) NOT NULL,
    Estado VARCHAR(30) NOT NULL DEFAULT 'Activo',
    IdUsuarioCreador INT NULL,
    IdUsuarioAnulador INT NULL,
    FechaAnulacion DATETIME NULL,
    Activo TINYINT(1) DEFAULT 1,
    CONSTRAINT FK_Pago_Reserva FOREIGN KEY (IdReserva) REFERENCES Reservas(IdReserva),
    CONSTRAINT FK_Pago_UsuarioCreador FOREIGN KEY (IdUsuarioCreador) REFERENCES Usuarios(IdUsuario),
    CONSTRAINT FK_Pago_UsuarioAnulador FOREIGN KEY (IdUsuarioAnulador) REFERENCES Usuarios(IdUsuario)
);

-- =============================================================
-- DATOS SEMILLA PARA TESTING Y DEMOSTRACIÃ“N
-- =============================================================
INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email, Activo) VALUES
('Juan', 'Perez', '12345678', '2664123456', 'juan@mail.com', 1),
('Maria', 'Gomez', '87654321', '2664654321', 'maria@mail.com', 1),
('Roberto', 'Diaz', '23456789', '2664234567', 'roberto@mail.com', 1)
ON DUPLICATE KEY UPDATE Nombre=VALUES(Nombre);

INSERT INTO Inquilinos (Nombre, Apellido, Dni, Telefono, Email, Activo) VALUES
('Carlos', 'Lopez', '11223344', '2664112233', 'carlos@mail.com', 1),
('Ana', 'Martinez', '44332211', '2664443322', 'ana@mail.com', 1),
('Sofia', 'Rodriguez', '33445566', '2664334455', 'sofia@mail.com', 1)
ON DUPLICATE KEY UPDATE Nombre=VALUES(Nombre);

INSERT INTO TiposInmueble (Descripcion, Activo) VALUES
('Casa', 1),
('Departamento', 1),
('Monoambiente', 1),
('Loft', 1),
('CabaÃ±a', 1)
ON DUPLICATE KEY UPDATE Descripcion=VALUES(Descripcion);

INSERT INTO Inmuebles (Direccion, Cupo, Latitud, Longitud, PrecioDia, PorcentajeReserva, Disponible, Portada, IdTipoInmueble, IdPropietario, Activo) VALUES
('Av. Illia 450, San Luis', 4, -33.29910000, -66.33560000, 45000.00, 20.00, 1, '/img/casa1.jpg', 1, 1, 1),
('San MartÃ­n 780, San Luis', 2, -33.30150000, -66.33820000, 30000.00, 15.00, 1, '/img/depto1.jpg', 2, 2, 1),
('Los Pinos 120, Potrero de los Funes', 6, -33.22140000, -66.23410000, 75000.00, 25.00, 1, '/img/cabana1.jpg', 5, 3, 1);

INSERT INTO ImagenesInmueble (IdInmueble, Url, EsPortada)
SELECT 1, '/img/casa1.jpg', 1 FROM DUAL
WHERE EXISTS (SELECT 1 FROM Inmuebles WHERE IdInmueble = 1)
  AND NOT EXISTS (SELECT 1 FROM ImagenesInmueble WHERE IdInmueble = 1 AND Url = '/img/casa1.jpg');

INSERT INTO ImagenesInmueble (IdInmueble, Url, EsPortada)
SELECT 1, '/img/casa1-living.jpg', 0 FROM DUAL
WHERE EXISTS (SELECT 1 FROM Inmuebles WHERE IdInmueble = 1)
  AND NOT EXISTS (SELECT 1 FROM ImagenesInmueble WHERE IdInmueble = 1 AND Url = '/img/casa1-living.jpg');

INSERT INTO ImagenesInmueble (IdInmueble, Url, EsPortada)
SELECT 2, '/img/depto1.jpg', 1 FROM DUAL
WHERE EXISTS (SELECT 1 FROM Inmuebles WHERE IdInmueble = 2)
  AND NOT EXISTS (SELECT 1 FROM ImagenesInmueble WHERE IdInmueble = 2 AND Url = '/img/depto1.jpg');

INSERT INTO Reservas (IdInquilino, IdInmueble, FechaDesde, FechaHasta, PrecioPorDia, MontoTotal, Estado, IdUsuarioCreador, Activo) 
SELECT 1, 1, '2026-10-01', '2026-10-07', 45000.00, 270000.00, 'Vigente', 1, 1 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Reservas WHERE IdInquilino = 1 AND IdInmueble = 1 AND FechaDesde = '2026-10-01');

INSERT INTO Reservas (IdInquilino, IdInmueble, FechaDesde, FechaHasta, PrecioPorDia, MontoTotal, Estado, IdUsuarioCreador, Activo) 
SELECT 2, 2, '2026-10-10', '2026-10-15', 30000.00, 150000.00, 'Vigente', 2, 1 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Reservas WHERE IdInquilino = 2 AND IdInmueble = 2 AND FechaDesde = '2026-10-10');

-- Pagos iniciales (SeÃ±as)
INSERT INTO Pagos (IdReserva, NumeroPago, FechaPago, Importe, Concepto, Estado, IdUsuarioCreador, Activo)
SELECT 1, 1, '2026-09-14 10:00:00', 54000.00, 'SeÃ±a de reserva (20%)', 'Activo', 1, 1 FROM DUAL
WHERE EXISTS (SELECT 1 FROM Reservas WHERE IdReserva = 1)
  AND NOT EXISTS (SELECT 1 FROM Pagos WHERE IdReserva = 1 AND NumeroPago = 1);

INSERT INTO Pagos (IdReserva, NumeroPago, FechaPago, Importe, Concepto, Estado, IdUsuarioCreador, Activo)
SELECT 2, 1, '2026-09-14 11:30:00', 22500.00, 'SeÃ±a de reserva (15%)', 'Activo', 2, 1 FROM DUAL
WHERE EXISTS (SELECT 1 FROM Reservas WHERE IdReserva = 2)
  AND NOT EXISTS (SELECT 1 FROM Pagos WHERE IdReserva = 2 AND NumeroPago = 1);
