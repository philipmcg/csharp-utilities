using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace Utilities
{
    public static partial class Extensions
    {
        public static void SetDate(this IVariableBin me, string key, DateTime date)
        {
            me.Str[key] = date.Ticks.ToString();
        }
        public static DateTime GetDate(this IVariableBin me, string key)
        {
            return new DateTime(long.Parse(me.Str[key]));
        }
        public static string Implode<T>(this IEnumerable<T> list, char delim)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                sb.Append(i.ToString());
                sb.Append(delim);
            }
            if(list.Any())
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        public static string Implode<T>(this IEnumerable<T> list, string delim)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                sb.Append(i.ToString());
                sb.Append(delim);
            }
            if (list.Any())
                sb.Remove(sb.Length - delim.Length, delim.Length);
            return sb.ToString();
        }
        /// <summary>
        /// Split this string at newline characters ("\r\n" or "\n")
        /// </summary>
        public static string[] SplitToLines(this string me)
        {
            return me.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        public static string Fmt(this string me, params object[] args)
        {
            return string.Format(me, args);
        }

        public static string ToHexString(this Guid me)
        {
            return me.ToString().ToLowerInvariant().Strip('-');
        }

        public static string With(this string me, params object[] args)
        {
            return string.Format(me, args);
        }

        public static bool IsAnyOf<T>(this T me, params T[] list)
        {
            return list.Any(o => o.Equals(me));
        }

        /// <summary>
        /// Distance between two points
        /// </summary>
        public static double Distance(this PointF a, PointF b)
        {
            double dist = Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2));
            return dist;
        }

        public static PointF AveragePt<T>(this IEnumerable<T> me, Func<T, PointF> getPoint)
        {
            float y;
            float x;

            x = me.Average(r => getPoint(r).X);
            y = me.Average(r => getPoint(r).Y);

            return new PointF(x, y);
        }

        public static Point AveragePt<T>(this IEnumerable<T> me, Func<T, Point> getPoint)
        {
            double y;
            double x;

            x = me.Average(r => getPoint(r).X);
            y = me.Average(r => getPoint(r).Y);

            return new Point((int)x, (int)y);
        }

        public static Point AveragePoint(this IEnumerable<Point> me)
        {
            double y;
            double x;

            x = me.Average(r => r.X);
            y = me.Average(r => r.Y);

            return new Point((int)x, (int)y);
        }

        public static void ForEach<T>(this IEnumerable<T> me, Action<T> action)
        {
            foreach (T item in me)
                action(item);
        }
        

        /// <summary>
        /// If me == -1, return or
        /// </summary>
        public static int ElseIfNot(this int me, int or)
        {
            if (me == -1)
                return or;
            else
                return me;
        }

        /// <summary>
        /// Shuffles in place.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Like GetRandom but yield returns indefinitely.  There may be duplicates.
        /// </summary>
        public static IEnumerable<T> RandomValues<T>(this T[] array)
        {
            while (true)
            {
                yield return array[Rand.Int(array.Length - 1)];
            }
        }

        /// <summary>
        /// Like GetRandom but yield returns indefinitely.  There may be duplicates.
        /// </summary>
        public static T[] UniqueRandomValues<T>(this T[] array, int count)
        {
            System.Collections.BitArray a = new System.Collections.BitArray(array.Length);
            T[] result = new T[count];

            int len = array.Length;
            for (int i = 0; i < count; i++)
            {
                int r = Rand.Int(len);
                while(a[r] == true)
                    r = (r + 1 == len) ? 0 : r + 1;
                a[r] = true;
                result[i] = array[r];
            }
            return result;
        }

        public static T[] ShuffleNew<T>(this T[] array)
        {
            return array.OrderBy(t => Guid.NewGuid()).ToArray();
        }
        public static T GetRandom<T>(this IList<T> me)
        {
            return me[Rand.Int(me.Count - 1)];
        }
        public static T GetRandom<T>(this T[] me)
        {
            return me[Rand.Int(me.Length)];
        }
        public static T GetRandom<T>(this IList<T> me, ControlledRandom r)
        {
            int intt = r.Int(me.Count);
            return me[intt];
        }
        public static T GetRandom<T>(this T[] me, ControlledRandom r)
        {
            return me[r.Int(me.Length)];
        }
        public static int ToInt(this string me)
        {
            if (string.IsNullOrEmpty(me))
                return 0;
            return int.Parse(me);
        }
        public static sbyte ToSbyte(this string me)
        {
            if (string.IsNullOrEmpty(me))
                return 0;
            return sbyte.Parse(me);
        }
        public static int ToInt(this bool me)
        {
            return me ? 1 : 0;
        }
        public static int Round(this double me)
        {
            return (int)(me + 0.5);
        }
        public static string Truncate1(this double me)
        {
            return "{0:N1}".Fmt((int)(me * 10) / 10d);
        }
        public static string Truncate2(this double me)
        {
            return ((int)(me * 100) / 100d).ToString();
        }
        public static string Truncate4(this double me)
        {
            return ((int)(me * 10000) / 10000d).ToString();
        }

        /// <summary>
        /// false if null, "", "0", "F", "f"
        /// </summary>
        public static bool ToBool(this string me)
        {
            if (string.IsNullOrEmpty(me))
                return false;
            else if (me == "0")
                return false;
            else if (me[0] == 'F')
                return false;
            else if (me[0] == 'f')
                return false;
            else
                return true;
        }

        /// <summary>
        /// false if 0
        /// </summary>
        public static bool ToBool(this int me)
        {
            if (me == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 0 flips with 1
        /// </summary>
        public static int Flip(this int me)
        {
            if (me == 0)
                return 1;
            else
                return 0;
        }
        public static int Bound(this int me, int lower, int upper)
        {
            return Math.Min(upper, Math.Max(lower, me));
        }

        /// <summary>
        /// Add to this integer, but wrap it inside the given range of 0-range.
        /// For instance, 5.AddInRange(1, 7) will return 6,
        /// 5.AddInRange(1, 6) will return 0,
        /// 0.AddInRange(-1, 6) will return 5,
        /// </summary>
        public static int AddInRange(this int me, int amount, int range)
        {
            return me.AddInRange(amount, 0, range);
        }

        /// <summary>
        /// Add to this integer, but wrap it inside the given range of 0-range.
        /// For instance, 5.AddInRange(1, 7) will return 6,
        /// 5.AddInRange(1, 6) will return 0,
        /// 0.AddInRange(-1, 6) will return 5,
        /// </summary>
        public static int AddInRange(this int me, int amount, int lower, int range)
        {
            int i = me + amount;
            if (i >= lower + range)
                i = lower;
            if (i < lower)
                i = lower + range - 1;

            return i;
        }

        /// <summary>
        /// Increment this integer, but if it leaves the given range, set it to 0.
        /// For instance, 5.AddInRange(7) will return 6,
        /// 5.AddInRange(6) will return 0,
        /// </summary>
        public static int AddInRange(this int me,int range)
        {
            return me.AddInRange(1,0,range);
        }


        public static int Round(this float me)
        {
            return (int)Math.Round(me);
        }

        /// <summary>
        /// Returns the file name without extension.
        /// </summary>
        public static string ShortName(this FileInfo file)
        {
            return file.Name.Substring(0, file.Name.IndexOf(file.Extension));
        }

        public static string ToCollectionString(this ICollection<string> me)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var str in me)
            {
                sb.Append(str);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        /// <summary>
        /// Converts the array to a Delimited-Value string.  For instance, an array of three integers might become: "1,2,3,".
        /// </summary>
        public static string ToDelimitedString<T>(this T[] me, char delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < me.Length; k++)
            {
                sb.Append(me[k].ToString());
                sb.Append(delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts each element in the array to Int32 and places it in a new int[].
        /// </summary>
        public static int[] ToIntArray(this string[] me, params int[][] def)
        {
            int[] array = new int[me.Length];
            for (int k = 0; k < me.Length; k++)
            {
                array[k] = me[k].ToInt();
            }
            return array;
        }
        public static Point MidPoint(this Point me, Point other)
        {
            return new Point((me.X + other.X) / 2, (me.Y + other.Y) / 2);
        }
        public static Point Add(this Point me, Point other)
        {
            return new Point((me.X + other.X), (me.Y + other.Y));
        }
        public static Point Add(this Point me, int x, int y)
        {
            return new Point((me.X + x), (me.Y + y));
        }
        public static Point Scale(this Point me, int scalar)
        {
            return new Point((me.X * scalar), (me.Y * scalar));
        }
        public static Point Subtract(this Point me, Point other)
        {
            return new Point((me.X - other.X), (me.Y - other.Y));
        }
        public static Point GetOffset(this Point me, Point other)
        {
            return new Point((other.X - me.X), (other.Y - me.Y));
        }

        public static void Loop(this int max, Action<int> function)
        {
            for (int k = 0; k < max; k++)
            {
                function(k);
            }
        }

        public static int Wheel(this MouseEventArgs me)
        {
            return me.Delta / 120;
        }

        public static long ToSeconds(this DateTime me)
        {
            return (long)(me - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }

    public static class Loop
    {
        public static void For(int max, Action<int> function)
        {
            for (int k = 0; k < max; k++)
            {
                function(k);
            }
        }
        public static void Foreach<T>(IEnumerable<T> list, Action<T> function)
        {
            foreach (T item in list)
			{
                function(item);
			}
        }
    }

    public static class CSVExtensions
    {
        public static char Delimiter = ',';
        public static void WriteCSVLine(this StreamWriter me, params object[] tokens)
        {

            for (int k = 0; k < tokens.Length - 1; k++)
            {
                if (tokens[k] is bool)
                    tokens[k] = ((bool)tokens[k]).ToInt();

                var token = tokens[k];
                string final;


                if (token == null)
                    final = "-";
                else if (token is double)
                    final = ((double)token).ToString(CultureInfo.InvariantCulture);
                else if (token is float)
                    final = ((float)token).ToString(CultureInfo.InvariantCulture);
                else if (token is int)
                    final = ((int)token).ToString(CultureInfo.InvariantCulture);
                else if (token is long)
                    final = ((long)token).ToString(CultureInfo.InvariantCulture);
                else if (token is bool)
                    final = ((bool)token).ToString(CultureInfo.InvariantCulture);
                else
                    final = token.ToString();


                me.Write(final);
                me.Write(Delimiter);
            }
            me.Write(tokens[tokens.Length - 1]);
            me.WriteLine();
        }
        public static void WriteCSVHeader(this StreamWriter me, params string[] tokens)
        {
            me.Write('~');
            for (int k = 0; k < tokens.Length - 1; k++)
            {
                me.Write(tokens[k]);
                me.Write(Delimiter);
            }
            me.Write(tokens[tokens.Length - 1]);
            me.WriteLine();
        }

    }

    public static partial class Extensions
    {

        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
        /// </summary>
        public static long UtcToUnixTime(this DateTime utcDateTime)
        {
            return (long)(utcDateTime - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
        /// </summary>
        public static long UnixTime(this DateTime dt)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dt.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return (long)ts.TotalMilliseconds;
        }

        public static DateTime ToDateFromUnixTime(this long millisecondsUnixTime)
        {
            var d1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime d2 = d1.AddMilliseconds(millisecondsUnixTime);
            return d2;
        }

        public static string ToFileNameString(this DateTime dt)
        {
            return dt.ToString("--MM-dd-yyyy--HH-mm-ss");
        }
    }

}
