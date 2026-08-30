# Proyecto Inmobiliaria

> Aplicación web para la gestión de reservas temporales de una inmobiliaria, desarrollada con ASP.NET Core MVC y ADO.NET.

---

## 👥 Integrantes del Grupo

* **Enzo Miranda** - *enzo792015@gmail.com* - [@Enzo](https://github.com/Enzo)

---

## 📐 Modelado de Datos

A continuación se presenta el esquema del modelo de datos correspondiente a la aplicación (Diagrama Entidad-Relación):

### Diagrama Entidad-Relación (DER)

```mermaid
erDiagram
    PROPIETARIOS ||--o{ INMUEBLES : posee
    TIPOS_INMUEBLE ||--o{ INMUEBLES : clasifica
    INMUEBLES ||--o{ RESERVAS : tiene
    INQUILINOS ||--o{ RESERVAS : realiza
    RESERVAS ||--o{ PAGOS : genera

    PROPIETARIOS {
        int IdPropietario PK
        string Nombre
        string Apellido
        string Dni
        string Telefono
        string Email
        string Clave
        tinyint Activo
    }

    INQUILINOS {
        int IdInquilino PK
        string Nombre
        string Apellido
        string Dni
        string Telefono
        string Email
        tinyint Activo
    }

    TIPOS_INMUEBLE {
        int IdTipoInmueble PK
        string Descripcion
        tinyint Activo
    }

    INMUEBLES {
        int IdInmueble PK
        string Direccion
        int Cupo
        decimal Latitud
        decimal Longitud
        decimal PrecioDia
        decimal PorcentajeReserva
        tinyint Disponible
        string Portada
        int IdTipoInmueble FK
        int IdPropietario FK
        tinyint Activo
    }

    RESERVAS {
        int IdReserva PK
        int IdInquilino FK
        int IdInmueble FK
        date FechaDesde
        date FechaHasta
        decimal PrecioPorDia
        decimal MontoTotal
        datetime FechaTerminacion
        decimal Multa
        string Estado
        tinyint Activo
    }

    PAGOS {
        int IdPago PK
        int ReservaId FK
        string Concepto
        datetime FechaPago
        decimal Importe
        tinyint Activo
    }
```

---

## ⚙️ Instrucciones para levantar la base de datos

1. Abre tu gestor de base de datos MySQL (por ejemplo, phpMyAdmin o MySQL Workbench).
2. Asegúrate de tener el servicio de MySQL en ejecución (por ejemplo desde el panel de control de XAMPP).
3. Ejecuta el script `inmobiliaria.sql` provisto en la raíz de este repositorio. Esto creará la base de datos `inmobiliaria`, las tablas correspondientes (`Propietarios`, `Inquilinos`, `TiposInmueble`, `Inmuebles`, `Reservas`) y cargará datos de prueba.
4. Verifica que la cadena de conexión en el archivo `appsettings.json` coincida con las credenciales de tu servidor MySQL local (por defecto `Server=localhost;Database=inmobiliaria;User=root;Password=;`).
