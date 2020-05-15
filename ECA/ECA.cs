using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ECA
{
    class ECA
    {
        private Dictionary<int, int> Table = new Dictionary<int, int>();
        public int[,] Field = new int[1000,1000];
        public bool ready;

        public ECA(int rule, int density, bool random)
        {
            ready = false;
            for (int i = 0; i < 8; i++)
            {
                Debug.WriteLine("Pattern: " + i + "  Result: " + (rule % 2));
                Table.Add(i, rule % 2);
                rule /= 2;
            }

            if (random)
            {
                Random rand = new Random();
                for (int i = 0; i < 1000; i++)
                {
                    Field[i, 0] = rand.Next(0, 100) < density ? 1 : 0;
                }
            }
            else
            {
                Field[499, 0] = 1;
            }

            for (int i = 1; i < 1000; i++)
            {
                Field[0, i] = Table[(Field[0, i - 1] * 2) + Field[1, i - 1]];

                for (int j = 1; j < 999; j++)
                {
                    int key = (Field[j - 1, i - 1] * 4) + (Field[j, i - 1] * 2) + Field[j + 1, i - 1];
                    Field[j, i] = Table[key];
                }

                Field[999, i] = Table[(Field[998, i - 1] * 4) + (Field[999, i - 1] * 2)];
            }
            ready = true;
        }
    }
}
