using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class RandEx
    {
        public static int[] DistributeCurved( int amount, int[] p, double distribution)
        {
            int len = p.Length;
            int[] result = new int[len];
            double[] dist = new double[len];
            double total = 0;
            for (int k = 0; k < len; k++)
            {
                dist[k] = Rand.CurvedDouble(p[k], distribution);
                total += dist[k];
            }
            double factor = 100 / total;
            for (int k = 0; k < len; k++)
            {
                dist[k] = dist[k] * factor;
            }

            int rem = amount;

            int i = Rand.Int(len);
            int j = 0;
            while (rem > 0 && j < len)
            {
                int a = Rand.Round((amount * dist[i]) / 100);
                a = Math.Min(rem, a);
                result[i] = a;
                rem -= a;
                i++;
                j++;
                if (i == len)
                    i = 0;
            }

            while (rem > 0)
            {
                int spot = Rand.Int(100);
                double sum = 0;
                for (int k = 0; k < len; k++)
                {
                    sum += dist[k];
                    if (sum > spot)
                    {
                        result[k]++;
                        rem--;
                        break;
                    }
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Provides functions for getting various kinds of random numbers.
    /// </summary>
    public class ControlledRandom
    {
        Random random;

        int distribution;
        double pi2;
        double standard_dev; 
        double max_dev; 
        double mult;
        double dist_div;


        public ControlledRandom(int seed)
        {
            random = new Random(seed);
            distribution = 50;
            pi2 = Math.PI / 2;
            standard_dev = 0.01; //Math.PI; // Higher means less deviation
            max_dev = Math.Atan(standard_dev); // Higher means greater deviation
            mult = pi2 / max_dev;
            dist_div = pi2 * 100;
        }

        /// <summary>
        /// Returns a random number between average / 2 and average + average / 2.  The number is most likely to be close to average.
        /// For instance, Curved(10) will return numbers in the range 5-15, most likely to be close to 10.
        /// </summary>
        public int Curved(double average)
        {
            return (int)Math.Round(CurvedDouble(average), 0);
        }

        // Numbers will be curved around average, so if you call CurvedDouble(100, 50)
        // you will get numbers between 50 and 150, mostly concentrated around 100.
        // 
        // If you call CurvedDouble(100, 10), numbers will be between 90 and 110.  
        public double CurvedDouble(double average, double distribution)
        {
            double dist = (average * distribution) / dist_div;
            double rand = random.NextDouble() * standard_dev;
            double value = pi2 - (mult * Math.Atan(rand));
            double dev = value * dist;

            bool neg = random.Next() % 2 == 0;
            if (neg)
                dev = -dev;
            return average + dev;
        }
        public double CurvedDouble(double average)
        {
            return CurvedDouble(average, distribution);
        }
        /// <summary>
        /// Returns a random number between average / 2 and average + average / 2.  The number is most likely to be close to average.
        /// For instance, Curved(10,50) will return numbers in the range 5-15, most likely to be close to 10.
        /// </summary>
        public int Curved(double average, double distribution)
        {
            return (int)Math.Round(CurvedDouble(average, distribution), 0);
        }

        /// <summary>
        /// Returns a random double between 0 and 1.0
        /// </summary>
        public double OneDouble()
        {
            return random.NextDouble();
        }

        /// <summary>
        /// Returns a random double between -0.5 and 0.5
        /// </summary>
        public double Double()
        {
            return random.NextDouble() - 0.5;
        }

        /// <summary>
        /// Returns either true or false.
        /// </summary>
        public bool Bool()
        {
            return Convert.ToBoolean(random.Next(2));
        }
        /// <summary>
        /// Returns a random character from the list: "ABCDEFGHIK"
        /// </summary>
        public string RandomCharTen()
        {
            char[] chars = "ABCDEFGHIK".ToCharArray();
            return chars[Int(10)].ToString();
        }

        /// <summary>
        /// Returns either -1 or 1.
        /// </summary>
        public int Sign()
        {
            return Bool() ? 1 : -1;
        }

        /// <summary>
        /// Gets a random number between 0-100% and returns true if this number is less than max.
        /// For instance, Percent(75) returns true 75% of the time.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public bool Percent(decimal max)
        {
            if (random.Next(100) < max)
                return true;
            else
                return false;
        }
        /// <summary>
        ///  Gets a random integer in the range of 0 to max, excluding max.
        ///  For instance, Int(3) will get either 0, 1 or 2.
        /// </summary>
        public int Int(int max)
        {
            return random.Next(max);
        }

        /// <summary>
        ///  Gets a random integer in the range of min to max, including max.
        ///  For instance, InRange(1,3) will get either 1, 2 or 3.
        /// </summary>
        public int InRange(int min, int max)
        {
            return random.Next(min, max + 1);
        }

        public int Round(double val)
        {
            int final = (int)val;
            if (Percent((int)((val - (int)val) * 100)))
                final++;

            return final;
        }

        public void Seed(int value)
        {
            random = new Random(value);
        }
    }

    /// <summary>
    /// Provides functions for getting various kinds of random numbers.
    /// </summary>
    public class Rand
    {
        static Random random = new Random();

        static int distribution = 50;
        static double pi2 = Math.PI / 2;
        static double standard_dev = 0.01; //Math.PI; // Higher means less deviation
        static double max_dev = Math.Atan(standard_dev); // Higher means greater deviation
        static double mult = pi2 / max_dev;
        static double dist_div = pi2 * 100;

        /// <summary>
        /// Returns a random number between average / 2 and average + average / 2.  The number is most likely to be close to average.
        /// For instance, Curved(10) will return numbers in the range 5-15, most likely to be close to 10.
        /// </summary>
        public static int Curved(double average)
        {
            return (int)Math.Round(CurvedDouble(average), 0);
        }

        public static double CurvedDouble(double average, double distribution)
        {
            double dist = (average * distribution) / dist_div;
            double rand = random.NextDouble() * standard_dev;
            double value = pi2 - (mult * Math.Atan(rand));
            double dev = value * dist;

            bool neg = random.Next() % 2 == 0;
            if (neg)
                dev = -dev;
            return average + dev;
        }
        public static double CurvedDouble(double average)
        {
            return CurvedDouble(average, distribution);
        }
        /// <summary>
        /// Returns a random number between average / 2 and average + average / 2.  The number is most likely to be close to average.
        /// For instance, Curved(10,50) will return numbers in the range 5-15, most likely to be close to 10.
        /// </summary>
        public static int Curved(double average, double distribution)
        {
            return (int)Math.Round(CurvedDouble(average,distribution), 0);
        }

        /// <summary>
        /// Returns a random double between -0.5 and 0.5
        /// </summary>
        public static double Double()
        {
            return random.NextDouble() - 0.5;
        }

        /// <summary>
        /// Returns a random double between 0 and 1.0
        /// </summary>
        public static double NextDouble()
        {
            return random.NextDouble();
        }

        public static int Next()
        {
            return random.Next();
        }

        /// <summary>
        /// Returns either true or false.
        /// </summary>
        public static bool Bool()
        {
            return Convert.ToBoolean(random.Next(2));
        }
        /// <summary>
        /// Returns a random character from the list: "ABCDEFGHIK"
        /// </summary>
        public static string RandomCharTen()
        {
            char[] chars = "ABCDEFGHIK".ToCharArray();
            return chars[Int(10)].ToString();
        }

        /// <summary>
        /// Returns either -1 or 1.
        /// </summary>
        public static int Sign()
        {
            return Bool() ? 1 : -1;
        }

        /// <summary>
        /// Gets a random number between 0-100% and returns true if this number is less than max.
        /// For instance, Percent(75) returns true 75% of the time.
        /// 
        /// If max >= 100, will always return true.
        /// </summary>
        public static bool Percent(double max)
        {
            return random.Next(100) < max;
        }

        /// <summary>
        /// A One in Max chance of true
        /// </summary>
        public static bool OneIn(int max)
        {
            return random.Next(max) == 0;
        }

        /// <summary>
        ///  Gets a random integer in the range of 0 to max, excluding max.
        ///  For instance, Int(3) will get either 0, 1 or 2.
        /// </summary>
        public static int Int(int max)
        {
            return random.Next(max);
        }

        /// <summary>
        ///  Gets a random integer in the range of min to max, including max.
        ///  For instance, Int(1,3) will get either 1, 2 or 3.
        /// </summary>
        public static int Int(int min, int max)
        {
            return random.Next(min, max + 1);
        }

        public static int Round(double val)
        {
            int final = (int)val;
            if (Rand.Percent((int)((val - (int)val) * 100)))
                final++;

            return final;
        }

        public static void Seed(int value)
        {
            random = new Random(value);
        }
    }
}
