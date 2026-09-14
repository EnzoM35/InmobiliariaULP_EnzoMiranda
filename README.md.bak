# Proyecto Inmobiliaria

> Aplicación web para la gestión de reservas temporales de una inmobiliaria, desarrollada con **ASP.NET Core MVC** y **ADO.NET (MySqlConnector)**.

---

## 👥 Integrantes del Grupo

* **Enzo Miranda** - *enzo792015@gmail.com* - [GitHub](https://github.com/EnzoM35)

---

## 📋 Requisitos Previos

Antes de ejecutar la aplicación, asegúrate de contar con:
* [.NET 10 SDK](https://dotnet.microsoft.com/download) (o la versión compatible de .NET instalada).
* Servidor **MySQL** (mediante [XAMPP]).


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

## ⚙️ Instrucciones para levantar la Base de Datos

1. Abre tu gestor de base de datos MySQL (por ejemplo, **phpMyAdmin**).
2. Asegúrate de tener el servicio de MySQL en ejecución (iniciando el módulo MySQL en el panel de **XAMPP**).
3. Ejecuta el script [`inmobiliaria.sql`](inmobiliaria.sql) ubicado en la raíz de este proyecto.
   * Este script creará automáticamente la base de datos `inmobiliaria`, todas las tablas (`Propietarios`, `Inquilinos`, `TiposInmueble`, `Inmuebles`, `Reservas`), relaciones/claves foráneas y cargará los datos iniciales de prueba.
4. Verifica que la cadena de conexión en el archivo `appsettings.json` coincida con las credenciales de tu servidor MySQL local:
   ```json
   {
     "ConnectionStrings": {
       "MySql": "Server=localhost;Database=inmobiliaria;User=root;Password=;"
     }
   }
   ```
   *(Si tu usuario o contraseña de MySQL son diferentes, ajústalos en `appsettings.json`)*.

---

## 🚀 Instrucciones para ejecutar el Proyecto

1. Clona el repositorio:
   ```bash
   git clone https://github.com/EnzoM35/InmobiliariaULP_EnzoMiranda.git
   cd InmobiliariaULP_EnzoMiranda
   ```

2. Restaura las dependencias y paquetes NuGet:
   ```bash
   dotnet restore
   ```

3. Ejecuta la aplicación en modo desarrollo:
   * **Modo estándar:**
     ```bash
     dotnet run
     ```
   * **O con recarga en caliente (Hot Reload):**
     ```bash
     dotnet watch
     ```

4. Abre tu navegador web y navega a la URL indicada en la consola (por ejemplo, `http://localhost:5207`).

---

## 📌 Módulos y Funcionalidades Disponibles

* **Propietarios:** ABM completo y listado.
* **Inquilinos:** ABM completo y listado.
* **Tipos de Inmueble:** ABM completo y vista de detalles.
* **Inmuebles:** ABM completo, filtrado, subida de portada, coordenadas/mapa y vista detallada.
* **Reservas:** ABM completo, cálculo automático de importes por estadía, validación de fechas/inmuebles disponibles y vista detallada.
