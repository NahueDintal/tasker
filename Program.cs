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
                PrintHelp();
                return;
            }

            // Usar el directorio actual como base
            string currentDir = Environment.CurrentDirectory;
            string taskDir = Path.Combine(currentDir, ".tasker");
            string taskFile = Path.Combine(taskDir, "tasks.txt");

            string command = args[0].ToLower();

            switch (command)
            {
                case "init":
                    InitTasker(taskDir);
                    break;
                case "add":
                case "a":
                case "new":
                    AddTask(args, taskDir, taskFile);
                    break;
                case "list":
                case "ls":
                    ListTasks(taskFile);
                    break;
                case "delete":
                case "del":
                case "rm":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Error: Se requiere un ID para eliminar una tarea.");
                        Console.WriteLine("Uso: tasker delete <id>");
                        return;
                    }
                    DeleteTask(args[1], taskFile);
                    break;
                case "status":
                    ShowStatus(taskDir, taskFile);
                    break;
                default:
                    Console.WriteLine($"Comando no reconocido: {command}");
                    PrintHelp();
                    break;
            }
        }

        static void InitTasker(string taskDir)
        {
            if (!Directory.Exists(taskDir))
            {
                Directory.CreateDirectory(taskDir);
                Console.WriteLine($"Tasker inicializado en: {taskDir}");
                Console.WriteLine("Ahora puedes agregar tareas específicas de este proyecto.");
            }
            else
            {
                Console.WriteLine("Tasker ya está inicializado en este directorio.");
            }
        }

        static void AddTask(string[] args, string taskDir, string taskFile)
        {
            // Verificar si tasker está inicializado
            if (!Directory.Exists(taskDir))
            {
                Console.WriteLine("Tasker no está inicializado en este directorio.");
                Console.WriteLine("Ejecuta primero: tasker init");
                return;
            }

            string? name = null;
            string date = "Sin fecha";
            string description = "Sin descripción";
            string type = "Sin tipo";
            string priority = "media";

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-n":
                        if (i + 1 < args.Length) name = args[++i];
                        break;
                    case "-f":
                        if (i + 1 < args.Length) date = args[++i];
                        break;
                    case "-d":
                        if (i + 1 < args.Length) description = args[++i];
                        break;
                    case "-t":
                        if (i + 1 < args.Length) type = args[++i];
                        break;
                    case "-p":
                        if (i + 1 < args.Length) priority = args[++i];
                        break;
                    default:
                        Console.WriteLine($"Opción desconocida: {args[i]}");
                        break;
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Error: El nombre de la tarea es obligatorio (-n)");
                return;
            }

            int nextId = GetNextId(taskFile);
            string taskLine = $"{nextId}|{name}|{date}|{description}|{type}|{priority}";

            File.AppendAllText(taskFile, taskLine + Environment.NewLine);
            Console.WriteLine($"Tarea agregada con ID: {nextId} en {Path.GetFileName(Environment.CurrentDirectory)}");
        }

        static void ListTasks(string taskFile)
        {
            if (!File.Exists(taskFile))
            {
                Console.WriteLine($"No hay tareas guardadas en este proyecto ({Path.GetFileName(Environment.CurrentDirectory)})");
                Console.WriteLine("Para empezar: tasker init && tasker add -n \"Mi primera tarea\"");
                return;
            }

            Console.WriteLine($"Tareas del proyecto: {Path.GetFileName(Environment.CurrentDirectory)}");
            Console.WriteLine(new string('-', 50));

            foreach (string line in File.ReadAllLines(taskFile))
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 6)
                {
                    Console.WriteLine($"{parts[0],3}. {parts[1]}");
                    Console.WriteLine($"     Fecha: {parts[2]} | Tipo: {parts[4]} | Prioridad: {parts[5]}");
                    if (parts[3] != "Sin descripción")
                        Console.WriteLine($"     Descripción: {parts[3]}");
                    Console.WriteLine();
                }
            }
        }

        static void DeleteTask(string idToDelete, string taskFile)
        {
            if (!File.Exists(taskFile))
            {
                Console.WriteLine($"No hay tareas guardadas en este proyecto ({Path.GetFileName(Environment.CurrentDirectory)})");
                return;
            }

            var lines = File.ReadAllLines(taskFile).ToList();
            bool found = false;
            var newLines = new List<string>();

            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 2 && parts[0] == idToDelete)
                {
                    found = true;
                    Console.WriteLine($"Tarea eliminada: {parts[0]} - {parts[1]}");
                }
                else
                {
                    newLines.Add(line);
                }
            }

            if (!found)
            {
                Console.WriteLine($"No se encontró ninguna tarea con ID: {idToDelete}");
                return;
            }

            File.WriteAllLines(taskFile, newLines);
            Console.WriteLine($"Tarea con ID {idToDelete} eliminada exitosamente.");
        }

        static void ShowStatus(string taskDir, string taskFile)
        {
            bool isInitialized = Directory.Exists(taskDir);
            Console.WriteLine($"Proyecto: {Path.GetFileName(Environment.CurrentDirectory)}");
            Console.WriteLine($"Tasker inicializado: {(isInitialized ? "SÍ ✅" : "NO ❌")}");
            
            if (isInitialized && File.Exists(taskFile))
            {
                var lines = File.ReadAllLines(taskFile);
                int totalTasks = lines.Length;
                int completed = lines.Count(line => line.Contains("|completada|"));
                int pending = totalTasks - completed;
                
                Console.WriteLine($"Tareas totales: {totalTasks}");
                Console.WriteLine($" Pendientes: {pending}");
                Console.WriteLine($" Completadas: {completed}");
            }
            else if (isInitialized)
            {
                Console.WriteLine("Tareas: 0 (usa 'tasker add' para agregar la primera)");
            }
        }

        static int GetNextId(string taskFile)
        {
            if (!File.Exists(taskFile)) return 1;

            int maxId = 0;
            foreach (string line in File.ReadAllLines(taskFile))
            {
                string[] parts = line.Split('|');
                if (parts.Length > 0 && int.TryParse(parts[0], out int id))
                {
                    if (id > maxId) maxId = id;
                }
            }

            return maxId + 1;
        }

        static void PrintHelp()
        {
            Console.WriteLine("Tasker - Gestor de tareas por proyecto (estilo Git)");
            Console.WriteLine();
            Console.WriteLine("USO:");
            Console.WriteLine("  tasker <COMANDO> [OPCIONES]");
            Console.WriteLine();
            Console.WriteLine("COMANDOS:");
            Console.WriteLine("  init               Inicializar tasker en el directorio actual");
            Console.WriteLine("  add, a, new        Agregar una nueva tarea al proyecto actual");
            Console.WriteLine("  list, ls           Listar tareas del proyecto actual");
            Console.WriteLine("  delete, del, rm    Eliminar una tarea por ID");
            Console.WriteLine("  status             Mostrar estado del proyecto actual");
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
