# 📊 Configuración de Base de Datos Access - Torneo de Golf

## ✅ Cadena de Conexión Actualizada

La aplicación ahora está configurada para usar la base de datos **`Torneo.accdb`** ubicada en la carpeta `Database`.

```csharp
Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\...\Database\Torneo.accdb
```

## 🗄️ Estructura de Tablas Requeridas

### 1. **Tabla: Torneo**
| Campo          | Tipo de Dato    | Descripción                |
|----------------|-----------------|----------------------------|
| CodigoTorneo   | Texto (20)      | Clave principal           |
| NombreTorneo   | Texto (100)     | Nombre del torneo         |
| Fecha          | Fecha/Hora      | Fecha del evento          |
| Ubicacion      | Texto (200)     | Lugar del torneo          |
| Descripcion    | Memo/Texto largo| Descripción               |

**Clave principal:** CodigoTorneo

---

### 2. **Tabla: Equipo**
| Campo          | Tipo de Dato    | Descripción                    |
|----------------|-----------------|--------------------------------|
| CodigoEquipo   | Texto (20)      | Clave principal               |
| NombreCapitan  | Texto (100)     | Nombre del capitán            |
| NumeroContacto | Texto (20)      | Teléfono del capitán          |
| CodigoTorneo   | Texto (20)      | FK a Torneo (opcional)        |

**Clave principal:** CodigoEquipo  
**Relación:** CodigoTorneo → Torneo.CodigoTorneo (opcional)

---

### 3. **Tabla: Equipo_participa** ⭐ (Tabla principal)
| Campo          | Tipo de Dato    | Descripción                           |
|----------------|-----------------|---------------------------------------|
| Id             | Autonumérico    | Clave principal                      |
| CodigoEquipo   | Texto (20)      | FK a Equipo                          |
| Apellido       | Texto (100)     | Apellido del participante            |
| Nombre         | Texto (100)     | Nombre del participante              |
| Telefono       | Texto (20)      | Teléfono de contacto                 |
| Transporte     | Texto (2)       | "Si" o "No"                          |
| Segundo        | Texto (100)     | Plato de comida preferido            |
| Alergias       | Texto (200)     | Alergias alimentarias                |
| Asistencia     | Sí/No           | Confirmación de asistencia (bit)     |
| Comentario     | Memo/Texto largo| Observaciones adicionales            |
| FechaRegistro  | Fecha/Hora      | Fecha de registro (opcional)         |

**Clave principal:** Id (Autonumérico)  
**Relación:** CodigoEquipo → Equipo.CodigoEquipo

---

## 🔧 Pasos para Crear las Tablas en Access

### **Opción 1: Usar el Asistente de Access (Recomendado)**

1. **Abrir la base de datos:**
   ```
   C:\Users\madrid\source\repos\WebApplication2\WebApplication2\Database\Torneo.accdb
   ```
   Doble clic en el archivo para abrirlo con Microsoft Access.

2. **Crear Tabla "Equipo_participa":**
   - Clic en **"Crear"** → **"Diseño de tabla"**
   - Agregar los campos según la estructura de arriba:
     - `Id`: Autonumérico (Clave principal)
     - `CodigoEquipo`: Texto (20)
     - `Apellido`: Texto (100)
     - `Nombre`: Texto (100)
     - `Telefono`: Texto (20)
     - `Transporte`: Texto (2)
     - `Segundo`: Texto (100)
     - `Alergias`: Texto (200)
     - `Asistencia`: Sí/No
     - `Comentario`: Memo (texto largo)
   - Hacer clic derecho en `Id` → **"Clave principal"**
   - Guardar como **"Equipo_participa"**

3. **Crear Tabla "Equipo":**
   - Similar al paso anterior con los campos:
     - `CodigoEquipo`: Texto (20) - Clave principal
     - `NombreCapitan`: Texto (100)
     - `NumeroContacto`: Texto (20)
     - `CodigoTorneo`: Texto (20)
   - Guardar como **"Equipo"**

4. **Crear Tabla "Torneo":**
   - Campos:
     - `CodigoTorneo`: Texto (20) - Clave principal
     - `NombreTorneo`: Texto (100)
     - `Fecha`: Fecha/Hora
     - `Ubicacion`: Texto (200)
     - `Descripcion`: Memo
   - Guardar como **"Torneo"**

5. **Establecer Relaciones (opcional pero recomendado):**
   - Clic en **"Herramientas de base de datos"** → **"Relaciones"**
   - Arrastrar `CodigoEquipo` de la tabla `Equipo` a `CodigoEquipo` en `Equipo_participa`
   - Marcar **"Integridad referencial"**
   - Aceptar

---

### **Opción 2: Usar SQL (Avanzado)**

Abre el editor SQL en Access (**Crear** → **Diseño de consulta** → Vista SQL):

```sql
-- Tabla Torneo
CREATE TABLE Torneo (
    CodigoTorneo TEXT(20) PRIMARY KEY,
    NombreTorneo TEXT(100),
    Fecha DATETIME,
    Ubicacion TEXT(200),
    Descripcion MEMO
);

-- Tabla Equipo
CREATE TABLE Equipo (
    CodigoEquipo TEXT(20) PRIMARY KEY,
    NombreCapitan TEXT(100),
    NumeroContacto TEXT(20),
    CodigoTorneo TEXT(20)
);

-- Tabla Equipo_participa
CREATE TABLE Equipo_participa (
    Id AUTOINCREMENT PRIMARY KEY,
    CodigoEquipo TEXT(20),
    Apellido TEXT(100),
    Nombre TEXT(100),
    Telefono TEXT(20),
    Transporte TEXT(2),
    Segundo TEXT(100),
    Alergias TEXT(200),
    Asistencia YESNO,
    Comentario MEMO,
    FechaRegistro DATETIME
);
```

---

## 📝 Datos de Prueba

Después de crear las tablas, puedes insertar datos de ejemplo:

```sql
-- Insertar un torneo
INSERT INTO Torneo (CodigoTorneo, NombreTorneo, Fecha, Ubicacion)
VALUES ('T001', 'Torneo Primavera 2024', #2024-04-15#, 'Club de Golf Los Pinos');

-- Insertar un equipo
INSERT INTO Equipo (CodigoEquipo, NombreCapitan, NumeroContacto, CodigoTorneo)
VALUES ('EQ001', 'Juan Pérez', '555-1234', 'T001');

-- Insertar participantes
INSERT INTO Equipo_participa (CodigoEquipo, Apellido, Nombre, Telefono, Transporte, Segundo, Alergias, Asistencia, Comentario)
VALUES ('EQ001', 'Pérez', 'Juan', '555-1234', 'Si', 'Filete', 'Ninguna', True, 'Capitán del equipo');

INSERT INTO Equipo_participa (CodigoEquipo, Apellido, Nombre, Telefono, Transporte, Segundo, Alergias, Asistencia, Comentario)
VALUES ('EQ001', 'López', 'Carlos', '555-2345', 'Si', 'Pollo', 'Mariscos', True, '');
```

---

## ⚠️ Notas Importantes

### **1. Provider de Access:**
La aplicación usa **`Microsoft.ACE.OLEDB.12.0`** que requiere:
- **Microsoft Access Database Engine 2010 Redistributable**
- Descargar: https://www.microsoft.com/en-us/download/details.aspx?id=13255
- **IMPORTANTE**: Si tu Visual Studio es 64-bit, descarga la versión de 64-bit

### **2. Compilación x86 vs x64:**
Si tienes problemas con el provider, configura el proyecto para **x86**:
1. En Visual Studio: **Build** → **Configuration Manager**
2. **Active solution platform** → **New** → **x86**
3. Compilar nuevamente

### **3. Permisos de Archivo:**
Asegúrate de que IIS Express tenga permisos de lectura/escritura sobre `Torneo.accdb`.

---

## 🧪 Verificar la Conexión

1. **Ejecuta la aplicación** (F5)
2. Si todo está correcto, verás:
   - ✅ Datos cargados desde la BD
   - ✅ Puedes agregar/editar/eliminar integrantes
3. Si hay error de conexión:
   - ⚠️ Verás el mensaje de error con datos de ejemplo
   - Revisa el provider y los permisos del archivo

---

## 📂 Ubicación del Archivo

```
WebApplication2/
└── Database/
    ├── Torneo.accdb          ← Tu base de datos
    ├── CreateTables.sql      ← Script SQL (para SQL Server)
    └── README.md             ← Documentación original
```

---

## 🔍 Consultas Útiles para Access

```sql
-- Ver todos los participantes
SELECT * FROM Equipo_participa;

-- Ver participantes de un equipo específico
SELECT * FROM Equipo_participa WHERE CodigoEquipo = 'EQ001';

-- Ver participantes que confirmaron asistencia
SELECT * FROM Equipo_participa WHERE Asistencia = True;

-- Contar integrantes por equipo
SELECT CodigoEquipo, COUNT(*) AS Total 
FROM Equipo_participa 
GROUP BY CodigoEquipo;
```

---

## ✅ ¡Listo!

Tu aplicación ahora está configurada para usar la base de datos Access local. Solo necesitas:
1. Crear las tablas según las instrucciones
2. Insertar algunos datos de prueba
3. ¡Ejecutar la aplicación!
