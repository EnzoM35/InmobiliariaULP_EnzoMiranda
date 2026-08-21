# Proyecto Inmobiliaria

> Aplicación web para la gestión de reservas temporales de una inmobiliaria, desarrollada con ASP.NET Core MVC y ADO.NET.

---

## 👥 Integrantes del Grupo

* **Enzo** - **enzo792015@gmail.com** - [@Enzo](https://github.com/Enzo)

---

## 📐 Modelado de Datos

A continuación se presenta el esquema del modelo de datos correspondiente a la aplicación (Diagrama Entidad-Relación):

### Diagrama Entidad-Relación (DER)

```mermaid
erDiagram
    PROPIETARIO ||--o{ INMUEBLE : posee
    INMUEBLE ||--o{ RESERVA : tiene
    INQUILINO ||--o{ RESERVA : realiza
    RESERVA ||--o{ PAGO : genera

    PROPIETARIO {
        int IdPropietario PK
        string Nombre
        string Apellido
        string Dni
        string Telefono
        string Email
        string Clave
    }

    INQUILINO {
        int IdInquilino PK
        string Nombre
        string Apellido
        string Dni
        string Telefono
        string Email
    }

    INMUEBLE {
        int IdInmueble PK
        int PropietarioId FK
        string Direccion
        int Ambientes
        decimal Precio
    }

    RESERVA {
        int IdReserva PK
        int InmuebleId FK
        int InquilinoId FK
        date FechaDesde
        date FechaHasta
        decimal Monto
    }

    PAGO {
        int IdPago PK
        int ReservaId FK
        decimal Importe
        date FechaPago
    }
```

---

## ⚙️ Instrucciones para levantar la base de datos

1. Tener el motor MySQL en ejecución PhpMyAdmin.
3. Ejecuta el script `inmobiliaria.sql` provisto en la raíz de este repositorio. Esto creará la base de datos `inmobiliaria` y las tablas necesarias.
4. Actualiza la cadena de conexión en el archivo `appsettings.json` o usando `dotnet user-secrets` con tu usuario y contraseña de MySQL.
