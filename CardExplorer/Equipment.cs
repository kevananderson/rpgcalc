using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    public class Equipment : Card
    {
        public enum Type { WEAPON, ARMOR, FOOTWARE, ACCESSORY };
        public static String[] type_string = { "Weapon", "Armor", "Footware", "Accessory" };
        public static int type_mask = 0x3000;
        public static int type_shift = 12;

        public static String[] level_string = { "2", "6", "10", "14", "18", "22", "24", "28" };
        public static int level_mask = 0x0E00;
        public static int level_shift = 9;

        public static String[] affect_string = { "STR-SPD-FTH-LCK", "GRT-BAL-FOC-ALL",
                                                 "STR-GRT-FTH-FOC", "SPD-BAL-LCK-ALL",
                                                 "STR-GRT-LCK-ALL", "SPD-BAL-FTH-FOC",
                                                 "STR-BAL-ALL-FTH", "GRT-SPD-LCK-FOC" };
        public static int affect_mask = 0x01C0;
        public static int affect_shift = 6;
        public static int[][] affect_table = new int[][]
            {
                new int[] { (int)Card.Stat.STRENGTH, (int)Card.Stat.SPEED,   (int)Card.Stat.FAITH,  (int)Card.Stat.LUCK   },
                new int[] { (int)Card.Stat.GRIT,     (int)Card.Stat.BALANCE, (int)Card.Stat.FOCUS,  (int)Card.Stat.ALLURE },
                new int[] { (int)Card.Stat.STRENGTH, (int)Card.Stat.GRIT,    (int)Card.Stat.FAITH,  (int)Card.Stat.FOCUS  },
                new int[] { (int)Card.Stat.SPEED,    (int)Card.Stat.BALANCE, (int)Card.Stat.LUCK,   (int)Card.Stat.ALLURE },
                new int[] { (int)Card.Stat.STRENGTH, (int)Card.Stat.GRIT,    (int)Card.Stat.LUCK,   (int)Card.Stat.ALLURE },
                new int[] { (int)Card.Stat.SPEED,    (int)Card.Stat.BALANCE, (int)Card.Stat.FAITH,  (int)Card.Stat.FOCUS  },
                new int[] { (int)Card.Stat.STRENGTH, (int)Card.Stat.BALANCE, (int)Card.Stat.ALLURE, (int)Card.Stat.FAITH  },
                new int[] { (int)Card.Stat.GRIT,     (int)Card.Stat.SPEED,   (int)Card.Stat.LUCK,   (int)Card.Stat.FOCUS  }
            };

        public static String[] push_string = { "0", "1", "2", "3"};
        public static int push_mask = 0x0030;
        public static int push_shift = 4;

        public static String[] stats_string = { "1,  0,  0,  0", "2, -1,  0,  0", "2,  0,  0, -1", "2, -1, -1,  1",
                                                "2,  0,  0,  0", "3, -1,  0,  0", "1,  1,  0,  0", "2,  2, -1, -1",
                                                "3,  0,  0,  0", "2,  1,  0,  0", "2,  0,  0,  1", "1,  1,  1,  0",
                                                "4,  0,  0,  0", "3,  1,  0,  0", "3,  0,  0,  1", "2,  2,  0,  0" };

        public static int stats_mask = 0x000F;
        public static int stats_shift = 0;
        public static int[][] stats_table = new int[][]
            {
                new int[] {  1,  0,  0,  0  },  //0 = 1
                new int[] {  2, -1,  0,  0  },
                new int[] {  2,  0,  0, -1  },
                new int[] {  2, -1, -1,  1  },
                new int[] {  2,  0,  0,  0  },  //4 = 2
                new int[] {  3, -1,  0,  0  },
                new int[] {  1,  1,  0,  0  },
                new int[] {  2,  2, -1, -1  },
                new int[] {  3,  0,  0,  0  },  //8 = 3
                new int[] {  2,  1,  0,  0  },
                new int[] {  2,  0,  0,  1  },
                new int[] {  1,  1,  1,  0  },
                new int[] {  4,  0,  0,  0  },  //12 = 4
                new int[] {  3,  1,  0,  0  },
                new int[] {  3,  0,  0,  1  },
                new int[] {  2,  2,  0,  0  }
            };

        protected Equipment.Type type;
        protected int level;
        protected int affect;
        protected int[] affect_choice;
        protected int push;
        protected int stats;
        protected int[] stats_choice;

        protected Matrix equiped_stats;

        /*** constructor ***/

        internal Equipment( int cid ) : base(cid)
        {
            this.type = (Equipment.Type)((cid & Equipment.type_mask) >> Equipment.type_shift);

            this.level = (cid & Equipment.level_mask) >> Equipment.level_shift;
            this.level *= 4;
            this.level += 2;

            this.affect = (cid & Equipment.affect_mask) >> Equipment.affect_shift;
            this.affect_choice = affect_table[affect];

            this.push = (cid & Equipment.push_mask) >> Equipment.push_shift;
            this.affect_choice = Equipment.RotateArray(this.affect_choice, this.push);

            this.stats = (cid & Equipment.stats_mask) >> Equipment.stats_shift;
            this.stats_choice = Equipment.stats_table[this.stats];

            this.equiped_stats = Matrix.ZeroMatrix(8, 1);
            for(int i = 0; i < this.affect_choice.Length; i++)
            {
                this.equiped_stats[this.affect_choice[i], 0] = this.stats_choice[i];
            }
        }

        /*** public ***/

        public override string ToString()
        {
            return "Equipment: " + Equipment.type_string[(int) this.type] + ": Level: " + this.level + ": " + Card.StatLine(this.equiped_stats, true);
        }

        public Equipment.Type GetEquipType()
        {
            return this.type;
        }

        public Matrix GetStats()
        {
            return equiped_stats;
        }

        public int GetLevel()
        {
            return this.level;
        }

        /*** protected ***/

        protected static int[] RotateArray( int[] array, int rotate)
        {
            int[] result = new int[array.Length];
            int j;
            for( int i = 0; i < array.Length; i++)
            {
                j = (i + rotate) % array.Length;
                result[i] = array[j];
            }
            return result;
        }

    }
}
