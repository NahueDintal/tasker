Bienvenido a mi programa para gestionar tareas por repositorios.

Breve reseña del desarrollo del mismo.

Programa tipo cli para terminal.

Objetivo: Tener una herramienta fluida para gestionar tareas desde la misma terminal donde se trabaja,
manteniendo el foco de concentración. Evitando entrar al navegador buscar Google Calendar por ejemplo.

En una primera versión, con un fácil ABM (aderir, bajar, o modificar) de tareas.

Con la idea de tener un archivo compatible con Google Calendar por ejemplo para sincronizar.

Al poco tiempo de uso, se me lleno de tareas y se me hizo complicado la gestión de las mismas. Probé 
agregarle prioridades, descripciones, fecha, pero el problema es que era general, para todo el sistema
por así decirlo.

En un cambio de paradigma para mantenerlo fiel al objetivo, decidi copiar el concepto de Git, y generar un 
Tasker init, por repositorio, por ende si trabajo en un proyecto, tengo la lista de tareas del mismo, con un
registro o vitacora de las tareas de cada proyecto.

Por el momento en la semana de uso es simplemente lo que necesitaba.

Tasker - Gestor de tareas por proyecto.

USO:
  tasker <COMANDO> [OPCIONES]

COMANDOS:
  init               Inicializar tasker en el directorio actual
  add, a, new        Agregar una nueva tarea al proyecto actual
  list, ls           Listar tareas del proyecto actual
  delete, del, rm    Eliminar una tarea por ID
  status             Mostrar estado del proyecto actual
  -h, --help         Mostrar esta ayuda

OPCIONES PARA 'add':
  -n <nombre>      Nombre de la tarea (OBLIGATORIO)
  -f <fecha>       Fecha de la tarea
  -d <descripción> Descripción de la tarea
  -t <tipo>        Tipo de tarea
  -p <prioridad>   Prioridad (alta, media, baja)

FLUJO DE TRABAJO:
  1. tasker init                    # Inicializar en un proyecto
  2. tasker add -n "Mi tarea"       # Agregar tareas
  3. tasker list                    # Ver tareas del proyecto

EJEMPLOS:
  tasker init
  tasker add -n "Implementar login" -p alta
  tasker add -n "Documentar API" -t documentación
  tasker list
  tasker delete 2
  tasker status

