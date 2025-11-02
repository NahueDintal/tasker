using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tasker
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
      {
        ImprimirAyuda();
        return;
      }

      string directorioActual = Environment.CurrentDirectory;
      string directorioTareas = Path.Combine(directorioActual, ".tasker");
      string archivoTareas = Path.Combine(directorioTareas, "tasks.txt");

      string comando = args[0].ToLower();

      switch (comando)
      {
        case "init": InicializarTasker(directorioTareas); break;
        case "add":
        case "a":
        case "new":
            AgregarTarea(args, directorioTareas, archivoTareas); break;
        case "list":
        case "ls": ListarTareas(archivoTareas); break;
        case "delete":
        case "del":
        case "rm":
          if (args.Length < 2)
          {
            Console.WriteLine("Error: Se requiere un ID para eliminar una tarea.");
            Console.WriteLine("Uso: tasker delete <id>");
            return;
          }
          EliminarTarea(args[1], archivoTareas); break;
        case "status": MostrarEstado(directorioTareas, archivoTareas); break;
        case "ok":
        case "lista": 

        default:
          Console.WriteLine($"Comando no reconocido: {comando}");
          ImprimirAyuda();
          break;
      }
    }

    static void InicializarTasker(string directorioTareas)
    {
      if (!Directory.Exists(directorioTareas))
      {
        Directory.CreateDirectory(directorioTareas);
        Console.WriteLine($"Tasker inicializado en: {directorioTareas}");
        Console.WriteLine("Ahora puedes agregar tareas específicas de este proyecto.");
      }
      else
      {
        Console.WriteLine("Tasker ya está inicializado en este directorio.");
      }
    }

    static void AgregarTarea(string[] args, string directorioTareas, string archivoTareas)
    {
      if (!Directory.Exists(directorioTareas))
      {
        Console.WriteLine("Tasker no está inicializado en este directorio.");
        Console.WriteLine("Ejecuta primero: tasker init");
        return;
      }

      string? nombre = null;
      string fecha = "Sin fecha";
      string descripcion = "Sin descripción";
      string tipo = "Sin tipo";
      string prioridad = "media";

      for (int i = 1; i < args.Length; i++)
      {
        switch (args[i])
        {
          case "-n": if (i + 1 < args.Length) nombre = args[++i]; break;
          case "-f": if (i + 1 < args.Length) fecha = args[++i]; break;
          case "-d": if (i + 1 < args.Length) descripcion = args[++i]; break;
          case "-t": if (i + 1 < args.Length) tipo = args[++i]; break;
          case "-p": if (i + 1 < args.Length) prioridad = args[++i]; break;
          default: Console.WriteLine($"Opción desconocida: {args[i]}"); break;
        }
      }

      if (string.IsNullOrEmpty(nombre))
      {
          Console.WriteLine("Error: El nombre de la tarea es obligatorio (-n)");
          return;
      }

      int siguienteId = ObtenerSiguienteId(archivoTareas);
      string lineaTarea = $"{siguienteId}|{nombre}|{fecha}|{descripcion}|{tipo}|{prioridad}";

      File.AppendAllText(archivoTareas, lineaTarea + Environment.NewLine);
      Console.WriteLine($"Tarea agregada con ID: {siguienteId} en {Path.GetFileName(Environment.CurrentDirectory)}");
    }

    static void ListarTareas(string archivoTareas)
    {
      if (!File.Exists(archivoTareas))
      {
          Console.WriteLine($"No hay tareas guardadas en este proyecto ({Path.GetFileName(Environment.CurrentDirectory)})");
          Console.WriteLine("Para empezar: tasker init && tasker add -n \"Mi primera tarea\"");
          return;
      }

      Console.WriteLine($"Tareas del proyecto: {Path.GetFileName(Environment.CurrentDirectory)}");
      Console.WriteLine(new string('-', 50));

      foreach (string linea in File.ReadAllLines(archivoTareas))
      {
        string[] partes = linea.Split('|');
        if (partes.Length >= 6)
        {
          Console.WriteLine($"{partes[0],3}. {partes[1]}");
          Console.WriteLine($"     Fecha: {partes[2]} | Tipo: {partes[4]} | Prioridad: {partes[5]}");
          if (partes[3] != "Sin descripción")
              Console.WriteLine($"     Descripción: {partes[3]}");
          Console.WriteLine();
        }
      }
    }

    static void EliminarTarea(string idAEliminar, string archivoTareas)
    {
      if (!File.Exists(archivoTareas))
      {
          Console.WriteLine($"No hay tareas guardadas en este proyecto ({Path.GetFileName(Environment.CurrentDirectory)})");
          return;
      }

      var lineas = File.ReadAllLines(archivoTareas).ToList();
      bool encontrada = false;
      var nuevasLineas = new List<string>();

      foreach (string linea in lineas)
      {
        string[] partes = linea.Split('|');
        if (partes.Length >= 2 && partes[0] == idAEliminar)
        {
            encontrada = true;
            Console.WriteLine($"Tarea eliminada: {partes[0]} - {partes[1]}");
        }
        else
        {
            nuevasLineas.Add(linea);
        }
      }

      if (!encontrada)
      {
          Console.WriteLine($"No se encontró ninguna tarea con ID: {idAEliminar}");
          return;
      }

      File.WriteAllLines(archivoTareas, nuevasLineas);
      Console.WriteLine($"Tarea con ID {idAEliminar} eliminada exitosamente.");
    }

    static void MostrarEstado(string directorioTareas, string archivoTareas)
    {
      bool estaInicializado = Directory.Exists(directorioTareas);
      Console.WriteLine($"Proyecto: {Path.GetFileName(Environment.CurrentDirectory)}");
      Console.WriteLine($"Tasker inicializado: {(estaInicializado ? "SÍ" : "NO")}");
      
      if (estaInicializado && File.Exists(archivoTareas))
      {
        var lineas = File.ReadAllLines(archivoTareas);
        int tareasTotales = lineas.Length;
        int completadas = lineas.Count(linea => linea.Contains("|Completada!|"));
        int pendientes = tareasTotales - completadas;
        
        Console.WriteLine($"Tareas totales: {tareasTotales}");
        Console.WriteLine($" Pendientes: {pendientes}");
        Console.WriteLine($" Completadas: {completadas}");
    }
      else if (estaInicializado)
      {
          Console.WriteLine("Tareas: 0 (usa 'tasker add' para agregar la primera)");
      }
    }

    static int ObtenerSiguienteId(string archivoTareas)
    {
      if (!File.Exists(archivoTareas)) return 1;

      int idMaximo = 0;
      foreach (string linea in File.ReadAllLines(archivoTareas))
      {
        string[] partes = linea.Split('|');
        if (partes.Length > 0 && int.TryParse(partes[0], out int id))
        {
            if (id > idMaximo) idMaximo = id;
        }
      }

      return idMaximo + 1;
    }

    static void ImprimirAyuda()
    {
      Console.WriteLine("Tasker - Gestor de tareas por directorio.");
      Console.WriteLine();
      Console.WriteLine("USO:");
      Console.WriteLine("  tasker <COMANDO> [OPCIONES]");
      Console.WriteLine();
      Console.WriteLine("COMANDOS:");
      Console.WriteLine("  init               Inicializar tasker en el directorio actual");
      Console.WriteLine("  add, a, new        Agregar una nueva tarea al direcotorio actual");
      Console.WriteLine("  list, ls           Listar tareas del directorio actual");
      Console.WriteLine("  delete, del, rm    Eliminar una tarea por ID");
      Console.WriteLine("  status             Mostrar estado en el directorio actual");
      Console.WriteLine("  -h, --help         Mostrar esta ayuda");
      Console.WriteLine();
      Console.WriteLine("OPCIONES PARA 'add':");
      Console.WriteLine("  -n <nombre>      Nombre de la tarea (OBLIGATORIO)");
      Console.WriteLine("  -f <fecha>       Fecha de la tarea");
      Console.WriteLine("  -d <descripción> Descripción de la tarea");
      Console.WriteLine("  -t <tipo>        Tipo de tarea");
      Console.WriteLine("  -p <prioridad>   Prioridad (alta, media, baja)");
      Console.WriteLine();
      Console.WriteLine("FLUJO DE TRABAJO:");
      Console.WriteLine("  1. tasker init                    # Inicializar en un proyecto");
      Console.WriteLine("  2. tasker add -n \"Mi tarea\"      # Agregar tareas");
      Console.WriteLine("  3. tasker list                    # Ver tareas del proyecto");
      Console.WriteLine();
      Console.WriteLine("EJEMPLOS:");
      Console.WriteLine("  tasker init");
      Console.WriteLine("  tasker add -n \"Implementar login\" -p alta");
      Console.WriteLine("  tasker add -n \"Documentar API\" -t documentación");
      Console.WriteLine("  tasker list");
      Console.WriteLine("  tasker delete 2");
      Console.WriteLine("  tasker status");
    }
  }
}
