# 🏦 BTG Funds - Full Cloud Deployment Guide

Proyecto de ejemplo que demuestra un entorno completo de despliegue en la nube utilizando:
- **Backend:** .NET 8 Web API hospedado en EC2  
- **Frontend:** Angular hospedado en Amazon S3 (con hosting estático)  
- **Base de Datos:** MongoDB Atlas (DBaaS gestionado)  
- **Infraestructura:** AWS CloudFormation (Infraestructura como código)

---

## ⚙️ 1. Estructura del Proyecto

```
BTG.Funds.API/
├── BTG.Funds.Api/                 # API principal .NET 8
├── BTG.Funds.Application/         # Lógica de negocio
├── BTG.Funds.Domain/              # Entidades y modelos
├── BTG.Funds.Infrastructure/      # Repositorios y conexión a MongoDB
├── Utilities/
│   └── seed-funds.js              # Script de inicialización MongoDB
├── btg-funds-stack.yaml           # Plantilla CloudFormation
└── README.md                      # Este archivo
```

---

## ☁️ 2. Despliegue de Infraestructura (CloudFormation)

### 🔹 Paso 1: Crear la pila (Stack)
1. Entra a [AWS CloudFormation](https://console.aws.amazon.com/cloudformation/home).
2. Clic en **Create stack → With new resources (standard)**.
3. En “Template source” selecciona:
   - **Upload a template file**
   - Carga el archivo `btg-funds-stack.yaml`
4. Click en **Next** y llena los parámetros:

| Parámetro | Descripción |
|------------|-------------|
| **KeyName** | Par de llaves EC2 existente (ej: `btg-key`) |
| **InstanceType** | `t2.micro` (capa gratuita) |
| **SSHLocation** | IP desde la que te conectarás (ej: `0.0.0.0/0`) |
| **BucketName** | Nombre único para el frontend (ej: `btg-funds-frontend-jposada`) |

5. Clic en **Next** → **Create Stack**

⏳ Espera unos 3–5 minutos.

---

### 🔹 Paso 2: Revisar salidas del stack
Al finalizar, CloudFormation mostrará las siguientes **Outputs**:

| Output | Descripción |
|---------|-------------|
| **BackendURL** | URL pública de la API .NET |
| **FrontendURL** | URL del sitio Angular en S3 |

---

## 💻 3. Backend (.NET 8 Web API en EC2)

Tu instancia EC2 se configurará automáticamente con:
- .NET SDK 8.0
- Git
- Publicación y ejecución del proyecto

Si deseas ingresar manualmente:

```bash
ssh -i btg-key.pem ec2-user@<PUBLIC_IP>
```

Para verificar que la API está corriendo:
```bash
curl http://localhost:5016/api/users/current
```

Deberías obtener:
```json
{ "message": "Usuario no encontrado." }
```

👉 URL pública (según salida del stack):
```
http://<Public-DNS>:5016/swagger/index.html
```

---

## 🍃 4. Base de Datos - MongoDB Atlas

### 🔹 Paso 1: Conexión
Tu API se conecta a **MongoDB Atlas**, no se instala en EC2.  
El `connection string` se configura desde el `appsettings.json`:

```json
"ConnectionStrings": {
  "MongoDB": "mongodb+srv://btguser:btg1234@cluster.egnxniz.mongodb.net/BTGFundsDB"
}
```

---

### 🔹 Paso 2: Inicialización de datos

Desde tu máquina local, ejecuta el script `seed-funds.js` para insertar los fondos iniciales y el usuario base:

```bash
mongosh "mongodb+srv://cluster.egnxniz.mongodb.net/"   --username btguser --password btg1234   < Utilities/seed-funds.js
```

✅ Este script crea:
- 5 fondos (FPV y FIC)
- 1 usuario con balance inicial de 500,000

---

## 🌐 5. Frontend Angular en Amazon S3

### 🔹 Paso 1: Compilar el proyecto Angular
En tu entorno local del frontend:
```bash
ng build --configuration production
```

Esto genera la carpeta:
```
dist/btg-funds-web/browser/
```

### 🔹 Paso 2: Subir archivos a S3
```bash
aws s3 sync dist/btg-funds-web/browser s3://btg-funds-frontend-jposada --delete
```

### 🔹 Paso 3: Verificar URL pública
Tu sitio estará disponible en:
```
http://btg-funds-frontend-jposada.s3-website.us-east-2.amazonaws.com
```

---

## 🔄 6. Conexión Frontend ↔ Backend

Edita el archivo `src/environments/environment.prod.ts` de Angular:

```typescript
export const environment = {
  production: true,
  apiUrl: 'http://<Public-DNS-EC2>:5016'
};
```

Vuelve a compilar y sincroniza con S3:
```bash
ng build --configuration production
aws s3 sync dist/btg-funds-web/browser s3://btg-funds-frontend-jposada --delete
```

---

## 🔐 7. Seguridad y CORS

En el backend (`Program.cs`):

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://btg-funds-frontend-jposada.s3-website.us-east-2.amazonaws.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors("AllowFrontend");
```

---

## 🧩 8. Verificación final

✅ **Swagger:**  
`http://<EC2-DNS>:5016/swagger/index.html`

✅ **Frontend (Angular):**  
`http://btg-funds-frontend-jposada.s3-website.us-east-2.amazonaws.com`

✅ **MongoDB Atlas:**  
Verifica colecciones:
```
BTGFundsDB
  ├── Funds
  └── UserAccounts
```

---

## 🧰 9. Comandos útiles

Ver logs de la API en EC2:
```bash
sudo cat /home/ec2-user/api.log
```

Reiniciar la API:
```bash
sudo pkill -f "BTG.Funds.Api.dll"
nohup dotnet /home/ec2-user/BTG.Funds.API/BTG.Funds.Api/out/BTG.Funds.Api.dll --urls "http://0.0.0.0:5016" &
```

---

## 🏁 Conclusión

Esta configuración deja completamente desplegado el ecosistema **BTG Funds**:
- 🌐 **Backend:** .NET 8 en EC2  
- 🗄️ **Base de datos:** MongoDB Atlas con datos semilla  
- 💻 **Frontend:** Angular en S3 (host estático público)  
- 🧱 **Infraestructura:** Automatizada con CloudFormation  

---

© 2025 BTG Funds - Full Stack Cloud Deployment
