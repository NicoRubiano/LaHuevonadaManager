# LaHuevonada Manager

Sistema profesional offline para gestión de granja de huevos criollos.

## Qué incluye

- Aplicación C# + WPF en .NET 8.
- Interfaz gráfica moderna.
- Logo e icono de LaHuevonada.
- Archivo propio `.lahuevonada` para compartir datos entre usuarios.
- Producción semanal con actualización automática de inventario.
- Ventas pagadas, parciales o pendientes.
- Clientes frecuentes y cartera pendiente.
- Gastos de comida, mantenimiento, transporte, veterinaria y otros.
- Dashboard con indicadores automáticos.
- Reporte CSV compatible con Excel.
- Copias de seguridad.
- GitHub Actions para generar ejecutable e instalador profesional.

## Cómo generar el ejecutable sin instalar Visual Studio

1. Crea un repositorio en GitHub llamado `LaHuevonadaManager`.
2. Sube todo el contenido de esta carpeta al repositorio.
3. En GitHub entra a la pestaña **Actions**.
4. Abre **Build Windows Executable**.
5. Presiona **Run workflow**.
6. Espera a que termine.
7. Abajo en **Artifacts** descarga:
   - `LaHuevonadaManager_Setup`: instalador profesional.
   - `LaHuevonadaManager_Portable_win-x64`: versión portable.

## Qué entregar al cliente

Lo más profesional es entregar:

```txt
LaHuevonadaManager_Setup.exe
```

El cliente solo da doble clic, instala y abre desde el icono del escritorio.

## Notas de seguridad

Windows puede mostrar advertencia SmartScreen porque el programa no está firmado con certificado digital. Para venta comercial avanzada, lo ideal es comprar un certificado de firma de código.

## Archivo de datos

La app usa archivos:

```txt
MiGranja.lahuevonada
```

Ese archivo contiene internamente los datos de la granja y se puede compartir por USB, Drive o WhatsApp.
