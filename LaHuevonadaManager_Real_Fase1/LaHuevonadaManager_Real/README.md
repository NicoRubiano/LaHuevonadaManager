# LaHuevonada Manager

Aplicación Windows en C# + WPF para gestión de una granja de huevos criollos.

## Funciones de esta primera versión real

- Dashboard con inventario, ventas, cuentas pendientes y utilidad estimada.
- Clientes.
- Ventas con pago pagado, parcial o pendiente.
- Producción semanal: actualiza inventario automáticamente.
- Gastos.
- Archivo propio `.lahuevonada` para guardar y compartir datos entre PCs.
- GitHub Actions para generar ejecutable Windows x64.

## Cómo generar el ejecutable desde GitHub

1. Subir este contenido al repositorio, no el ZIP.
2. Entrar a la pestaña **Actions**.
3. Abrir **Build Windows EXE**.
4. Presionar **Run workflow** si no se ejecuta automáticamente.
5. Cuando salga check verde, abrir la ejecución.
6. Descargar el artifact **LaHuevonadaManager-win-x64**.
7. Dentro está `LaHuevonadaManager.exe`.

## Notas

Esta es la Fase 1 real y compilable del producto. A partir de aquí se agregan módulos más avanzados: reportes PDF/Excel, formularios profesionales, gráficos, backups automáticos e instalador Setup.exe.
