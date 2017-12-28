using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    public class Creature : Actor
    {
        public static int BASE_ABILITY = 10;

        public static String[] level_string = { "1", "2", "3", "4", "5", "6", "7", "8", "9","10","11","12","13","14","15","16", 
                                               "17","18","19","20","21","22","23","24","25","26","27","28","29","30","31","32" };
        public static int level_mask = 0x0F80;
        public static int level_shift = 7;

        public static int tradeoff_mask = 0x007F;
        public static int tradeoff_shift = 0;
        public static int tradeoff_num = 7;
        public static String tradeoff_table =
       // 0  1  2  3  4  5  6  
        " 2 -1  0  0  0  0  0 \r\n" + //STR
        " 0  2  0 -1  0  0 -1 \r\n" + //GRT
        "-1  0  2  0  0  0  0 \r\n" + //SPD
        " 0  0 -1  2  0  0  0 \r\n" + //BAL
        " 0  0  0 -1  2 -1  0 \r\n" + //FTH
        " 0  0 -1  0 -1  2  0 \r\n" + //FOC
        "-1  0  0  0 -1  0  2 \r\n" + //LCK
        " 0 -1  0  0  0 -1 -1 \r\n";  //ALL


        protected int level;

        protected int tradeoff;
        protected Matrix tradeoff_choice;
        protected Matrix tradeoff_stats;
        protected Matrix ability_stats;

        protected static Matrix tradeoff_matrix;

        /*** constructor ***/

        internal Creature(int cid) : base(cid)
        {
            if (Creature.tradeoff_matrix == null)
            {
                Creature.tradeoff_matrix = Matrix.Parse(Creature.tradeoff_table);
            }

            this.level = (cid & Creature.level_mask) >> Creature.level_shift;
            this.level++;

            this.tradeoff = (cid & Creature.tradeoff_mask) >> Creature.tradeoff_shift;
            this.tradeoff_choice = Card.Tradeoff(this.tradeoff, Creature.tradeoff_num);
            this.tradeoff_stats = Creature.tradeoff_matrix * this.tradeoff_choice;

            this.ability_stats = Matrix.OnesMatrix(8, 1);
            this.ability_stats = (double)Creature.BASE_ABILITY * this.ability_stats;
            this.ability_stats += this.tradeoff_stats;

        }

        /*** public ***/

        public override string ToString()
        {
            return "Actor: Creature: Level " + this.level + ": " + Card.StatLine(this.ability_stats, false);
        }

        public override Matrix GetStats()
        {
            return ability_stats;
        }

        public int GetLevel()
        {
            return this.level;
        }

        /*** protected ***/

    }
}
