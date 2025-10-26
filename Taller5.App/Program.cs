using System;
using Taller5.Core;

namespace Taller5.App
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Taller #5 - Lista Doblemente Enlazada Genérica ===");
            Console.WriteLine("Ingrese valores de cualquier tipo: números, fechas o texto.\n");

            var list = new DoublyLinkedList<UniversalValue>();

            while (true)
            {
                ShowMenu();
                Console.Write("Elija una opción: ");
                var op = Console.ReadLine();

                switch (op)
                {
                    case "1":
                        Console.Write("Ingrese valor: ");
                        var val = Console.ReadLine() ?? "";
                        list.AddSorted(UniversalValue.Parse(val));
                        Console.WriteLine("✔ Agregado (orden ascendente).");
                        break;

                    case "2":
                        PrintSequence("Adelante", list.Forward());
                        break;

                    case "3":
                        PrintSequence("Atrás", list.Backward());
                        break;

                    case "4":
                        list.SortDescendingInPlace();
                        Console.WriteLine("✔ Lista en orden DESCENDENTE:");
                        PrintSequence("Descendente", list.Forward());
                        break;

                    case "5":
                        var (modas, max) = list.GetModes();
                        Console.WriteLine(list.Count == 0
                            ? "Lista vacía."
                            : $"Moda(s) (frecuencia {max}): {string.Join(", ", modas)}");
                        break;

                    case "6":
                        DrawGraph(list);
                        break;

                    case "7":
                        Console.Write("Valor a buscar: ");
                        var q = Console.ReadLine() ?? "";
                        Console.WriteLine(list.Contains(UniversalValue.Parse(q))
                            ? "✔ Sí existe."
                            : "✖ No existe.");
                        break;

                    case "8":
                        if (list.RemoveLast())
                        {
                            Console.WriteLine("✔ Se eliminó la última ocurrencia.");
                            PrintSequence("Lista actual", list.Forward());
                        }
                        else
                        {
                            Console.WriteLine("Lista vacía, no hay qué eliminar.");
                            PrintSequence("Lista actual", list.Forward());
                        }
                        break;

                    case "9":
                        list.Clear();
                        Console.WriteLine("✔ Se eliminaron todas las ocurrencias (lista vaciada).");
                        PrintSequence("Lista actual", list.Forward());
                        break;

                    case "0":
                        Console.WriteLine("Saliendo...");
                        return;

                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n----- MENÚ -----");
            Console.WriteLine("1. Adicionar");
            Console.WriteLine("2. Mostrar hacia adelante");
            Console.WriteLine("3. Mostrar hacia atrás");
            Console.WriteLine("4. Ordenar descendentemente y mostrar");
            Console.WriteLine("5. Mostrar la(s) moda(s)");
            Console.WriteLine("6. Mostrar gráfico (ocurrencias con '*')");
            Console.WriteLine("7. Existe");
            Console.WriteLine("8. Eliminar una ocurrencia (última)");
            Console.WriteLine("9. Eliminar todas las ocurrencias");
            Console.WriteLine("0. Salir");
        }

        static void PrintSequence<T>(string title, System.Collections.Generic.IEnumerable<T> seq)
        {
            Console.Write(title + ": ");
            bool any = false;
            foreach (var x in seq) { Console.Write(x + " "); any = true; }
            Console.WriteLine(any ? "" : "(lista vacía)");
        }

        static void DrawGraph<T>(DoublyLinkedList<T> list) where T : IComparable<T>
        {
            var freqs = list.GetFrequenciesAscending();
            if (freqs.Count == 0)
            {
                Console.WriteLine("Lista vacía.");
                return;
            }
            foreach (var (value, count) in freqs)
                Console.WriteLine($"{value} {new string('*', count)}");
        }
    }
}
