using System;
using System.Collections.Generic;
using System.Globalization;

namespace Taller5.Core
{
    // ────────────────────────────────
    // Tipo UniversalValue: acepta cualquier tipo de dato (int, double, fecha, string)
    // ────────────────────────────────
    public readonly struct UniversalValue : IComparable<UniversalValue>, IEquatable<UniversalValue>
    {
        public enum Kind { Integer, Double, DateTime, String }
        public Kind Type { get; }
        public string Raw { get; }
        readonly long _i;
        readonly double _d;
        readonly DateTime _dt;

        private UniversalValue(string raw, Kind k, long i, double d, DateTime dt)
        {
            Raw = raw;
            Type = k;
            _i = i;
            _d = d;
            _dt = dt;
        }

        public static UniversalValue Parse(string input)
        {
            var s = (input ?? "").Trim();

            if (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var li))
                return new UniversalValue(s, Kind.Integer, li, li, default);

            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var dd))
                return new UniversalValue(s, Kind.Double, default, dd, default);

            if (DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt))
                return new UniversalValue(s, Kind.DateTime, default, default, dt);

            return new UniversalValue(s, Kind.String, default, default, default);
        }

        public override string ToString() => Raw;

        public bool Equals(UniversalValue other)
        {
            if (IsNumeric(this) && IsNumeric(other))
                return ToDouble(this) == ToDouble(other);
            if (Type == Kind.DateTime && other.Type == Kind.DateTime)
                return _dt == other._dt;
            return string.Equals(Raw, other.Raw, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) => obj is UniversalValue v && Equals(v);

        public override int GetHashCode()
        {
            if (IsNumeric(this)) return ToDouble(this).GetHashCode();
            if (Type == Kind.DateTime) return _dt.GetHashCode();
            return Raw.GetHashCode();
        }

        public int CompareTo(UniversalValue other)
        {
            if (IsNumeric(this) && IsNumeric(other))
                return ToDouble(this).CompareTo(ToDouble(other));

            if (Type == Kind.DateTime && other.Type == Kind.DateTime)
                return _dt.CompareTo(other._dt);

            int Rank(Kind k) => k switch
            {
                Kind.Integer => 0,
                Kind.Double => 1,
                Kind.DateTime => 2,
                _ => 3
            };

            int r = Rank(Type).CompareTo(Rank(other.Type));
            if (r != 0) return r;

            return string.Compare(Raw, other.Raw, StringComparison.Ordinal);
        }

        static bool IsNumeric(UniversalValue v) => v.Type == Kind.Integer || v.Type == Kind.Double;
        static double ToDouble(UniversalValue v) => v.Type == Kind.Integer ? v._i : v._d;
    }

    // ────────────────────────────────
    // Clase DoublyLinkedList<T>
    // ────────────────────────────────
    public class Node<T>
    {
        public T Data;
        public Node<T>? Prev;
        public Node<T>? Next;
        public Node(T data) => Data = data;
    }

    public class DoublyLinkedList<T> where T : IComparable<T>
    {
        public Node<T>? Head { get; private set; }
        public Node<T>? Tail { get; private set; }
        public int Count { get; private set; }

        // Inserta manteniendo orden ascendente
        public void AddSorted(T value)
        {
            var n = new Node<T>(value);

            if (Head == null) { Head = Tail = n; Count = 1; return; }

            if (value.CompareTo(Head.Data) <= 0)
            {
                n.Next = Head;
                Head.Prev = n;
                Head = n;
                Count++;
                return;
            }

            if (value.CompareTo(Tail!.Data) >= 0)
            {
                Tail.Next = n;
                n.Prev = Tail;
                Tail = n;
                Count++;
                return;
            }

            var cur = Head;
            while (cur != null && value.CompareTo(cur.Data) > 0)
                cur = cur.Next;

            var prev = cur!.Prev!;
            prev.Next = n;
            n.Prev = prev;
            n.Next = cur;
            cur.Prev = n;
            Count++;
        }

        // Mostrar adelante / atrás
        public IEnumerable<T> Forward()
        {
            var cur = Head;
            while (cur != null) { yield return cur.Data; cur = cur.Next; }
        }

        public IEnumerable<T> Backward()
        {
            var cur = Tail;
            while (cur != null) { yield return cur.Data; cur = cur.Prev; }
        }

        // Invertir enlaces (descendente)
        public void SortDescendingInPlace()
        {
            var cur = Head;
            while (cur != null)
            {
                var tmp = cur.Next;
                cur.Next = cur.Prev;
                cur.Prev = tmp;
                cur = tmp;
            }
            var oldHead = Head;
            Head = Tail;
            Tail = oldHead;
        }

        // Buscar elemento
        public bool Contains(T value)
        {
            var cur = Head;
            var eq = EqualityComparer<T>.Default;
            while (cur != null)
            {
                if (eq.Equals(cur.Data, value))
                    return true;
                cur = cur.Next;
            }
            return false;
        }

        // Eliminar último nodo
        public bool RemoveLast()
        {
            if (Tail == null) return false;
            var n = Tail;
            if (n.Prev != null) n.Prev.Next = null; else Head = null;
            Tail = n.Prev;
            n.Prev = n.Next = null;
            Count--;
            return true;
        }

        // Vaciar lista
        public void Clear()
        {
            var cur = Head;
            while (cur != null)
            {
                var next = cur.Next;
                cur.Prev = cur.Next = null;
                cur = next;
            }
            Head = Tail = null;
            Count = 0;
        }

        // Modas
        public (List<T> modas, int frecuenciaMax) GetModes()
        {
            var dict = new Dictionary<T, int>();
            var cur = Head;
            while (cur != null)
            {
                dict.TryGetValue(cur.Data, out int c);
                dict[cur.Data] = c + 1;
                cur = cur.Next;
            }

            if (dict.Count == 0) return (new List<T>(), 0);

            int max = 0;
            foreach (var kv in dict)
                if (kv.Value > max) max = kv.Value;

            var modas = new List<T>();
            foreach (var kv in dict)
                if (kv.Value == max) modas.Add(kv.Key);

            modas.Sort();
            return (modas, max);
        }

        // Para gráfico de frecuencias
        public List<(T value, int count)> GetFrequenciesAscending()
        {
            var dict = new Dictionary<T, int>();
            var cur = Head;
            while (cur != null)
            {
                dict.TryGetValue(cur.Data, out int c);
                dict[cur.Data] = c + 1;
                cur = cur.Next;
            }

            var list = new List<(T, int)>();
            foreach (var kv in dict) list.Add((kv.Key, kv.Value));
            list.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            return list;
        }
    }
}
