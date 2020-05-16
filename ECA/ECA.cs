using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECA
{
    class ECA
    {
        private Dictionary<int, int> Table = new Dictionary<int, int>();
        public int[,] Field;

        public ECA(int rule, int density, bool random, Size area)
        {
            for (int i = 0; i < 8; i++)
            {
                Debug.WriteLine("Pattern: " + i + "  Result: " + (rule % 2));
                Table.Add(i, rule % 2);
                rule /= 2;
            }

            Field = new int[area.Width, area.Height];

            if (random)
            {
                Random rand = new Random();
                for (int i = 0; i < area.Width - 1; i++)
                {
                    Field[i, 0] = rand.Next(0, 100) < density ? 1 : 0;
                }
            }
            else
            {
                Field[area.Width / 2, 0] = 1;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 1; i < area.Height - 2; i++)
            {
                Field[0, i] = Table[(Field[0, i - 1] * 2) + Field[1, i - 1]];

                Parallel.For(1, area.Width - 3, (j) =>
                {
                    int key = (Field[j - 1, i - 1] * 4) + (Field[j, i - 1] * 2) + Field[j + 1, i - 1];
                    Field[j, i] = Table[key];
                });

                Field[area.Width - 1, i] = Table[(Field[area.Width - 2, i - 1] * 4) + (Field[area.Width - 1, i - 1] * 2)];
            }
            stopwatch.Stop();
            Debug.WriteLine(stopwatch.Elapsed);
        }
    }
}
