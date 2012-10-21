using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Utilities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Type of object in the table</typeparam>
    public class GradientTable<T>
    {
        public T[] Objects;
        public int[] Thresholds;
        public int[][] Weights;

        public GradientTable(T[] objects, int[] thresholds, int[][] weights)
        {
            Objects = objects;
            Thresholds = thresholds;
            Weights = weights;

        }

        public static GradientTable<E> CreateFromCSV<E>(E[] objects, IEnumerable<string[]> lines)
        {
            int rows = lines.Count();
            int columns = lines.First().Length - 1;

            int[] thresholds = new int[rows];
            int[][] weights = new int[rows][];
            int k = 0;
            lines = lines.OrderBy(l => int.Parse(l[0]));
            foreach (var line in lines)
            {
                thresholds[k] = int.Parse(line[0]);
                weights[k] = new int[columns];
                for (int j = 0; j < columns; j++)
                {
                    weights[k][j] = int.Parse(line[j + 1]);
                }
                k++;
            }

            return new GradientTable<E>(objects, thresholds, weights);
        }
        public static GradientTable<int> CreateFromCSV(IEnumerable<string[]> lines)
        {
            int columns = lines.First().Length - 1;
            int[] objects = new int[columns];
            for (int i = 0; i < columns; i++)
            {
                objects[i] = i;
            }

            return GradientTable<int>.CreateFromCSV(objects, lines);
        }

        public T GradientDistribution(int value)
        {
            if (value < Thresholds[0])
                value = Thresholds[0];
            if (value > Thresholds[Thresholds.Length - 1])
                value = Thresholds[Thresholds.Length - 1];

            int num_items = Objects.Length;
            int pre;
            int post;

            int k = 0;
            while (value > Thresholds[k])
                k++;

            pre = k - 1;
            post = k;

            if (pre < 0)
                pre = 0;
            if (post >= Thresholds.Length)
                post = Thresholds.Length - 1;

            int dist_pre = Math.Max(1,Math.Abs(value - Thresholds[pre]));
            int dist_post = Math.Max(1, Math.Abs(Thresholds[post] - value));

            int[] mixed_weights = new int[num_items];
            int sum = 0;
            for (int i = 0; i < num_items; i++)
            {
                mixed_weights[i] = ((Weights[pre][i] * dist_pre) + (Weights[post][i] * dist_post)) / (dist_pre + dist_post);
                sum += mixed_weights[i];
            }

            int r = Rand.Int(sum);
            int s = 0;
            for (int j = 0; j < num_items; j++)
            {
                s += mixed_weights[j];
                if (r < s)
                    return Objects[j];
            }
            throw new ArgumentException("GradientDistribution failed");
        }



        public static void Test()
        {
            int[] thresholds = new int[] { 0, 100, 200, 300 };
            int[][] weights = new int[][]
                {
                    new int[] {10,10,80},
                    new int[] {20,20,60},
                    new int[] {50,30,20},
                    new int[] {90,5,5},
                };

            string[] objects = new string[] { "A", "B", "C" };
            GradientTable<string> t = new GradientTable<string>(objects, thresholds, weights);

            var a = t.GradientDistribution(120);
        }
    }

    class GradientTableTest
    {
        public static void Main()
        {
            string[] objects = 
            {
                "string1", 
                "string2",
                "string3",
            };

            int[] thresholds = 
            {
                100,
                200,
                300,
            };

            int[][] weights = 
            {
                new[] {100, 0, 0},
                new[] {0, 100, 0},
                new[] {0, 0, 100},
            };

            GradientTable<string> table = new GradientTable<string>(objects, thresholds, weights);

            var string1 = table.GradientDistribution(100);
            var string2 = table.GradientDistribution(200);
            var string3 = table.GradientDistribution(300);

            System.Diagnostics.Debug.Assert(string1 == "string1");
            System.Diagnostics.Debug.Assert(string2 == "string2");
            System.Diagnostics.Debug.Assert(string3 == "string3");
        }
    }

}
