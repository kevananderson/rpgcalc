using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    public class Character : Actor
    {
        public static int BASE_ABILITY = 7;
        public static int MAX_START_LEVEL = 20;

        public static String[] level_string = { "1", "5", "10", "15", "20"};
        public static int level_mask = 0x0E00;
        public static int level_shift = 9;

        public static int tradeoff_mask = 0x01FF;
        public static int tradeoff_shift = 0;
        public static int tradeoff_num = 9;
        public static String tradeoff_table =
       // 0  1  2  3  4  5  6  7  8  
        " 1  0  0 -1  0  0  0 -1  1  \r\n" + //STR
        " 0  1  0  0  0  0  0 -1  1  \r\n" + //GRT
        " 0  0  1  0  0  0 -1 -1  1  \r\n" + //SPD
        "-1  0  0  1  0  0  0 -1  1  \r\n" + //BAL
        " 0  0  0  0  1 -1  0  1 -1  \r\n" + //FTH
        " 0  0  0  0 -1  1  0  1 -1  \r\n" + //FOC
        " 0  0 -1  0  0  0  0  1 -1  \r\n" + //LCK
        " 0 -1  0  0  0  0  1  1 -1  \r\n";  //ALL

        protected int level;

        protected int tradeoff;
        protected Matrix tradeoff_choice;
        protected Matrix tradeoff_stats;
        protected Matrix ability_stats;

        protected static Matrix tradeoff_matrix;


        /*** constructor ***/

        internal Character(int cid) : base(cid)
        {
            if(Character.tradeoff_matrix == null )
            {
                Character.tradeoff_matrix = Matrix.Parse(Character.tradeoff_table);
            }

            this.level = (cid & Character.level_mask) >> Character.level_shift;
            this.level *= 5;
            if (this.level == 0) this.level++;
            if (this.level > Character.MAX_START_LEVEL) this.level = Character.MAX_START_LEVEL;

            this.tradeoff = (cid & Character.tradeoff_mask) >> Character.tradeoff_shift;
            this.tradeoff_choice = Card.Tradeoff(this.tradeoff, Character.tradeoff_num);
            this.tradeoff_stats = Character.tradeoff_matrix * this.tradeoff_choice;

            this.ability_stats = Matrix.OnesMatrix(8, 1);
            this.ability_stats = (double)Character.BASE_ABILITY * this.ability_stats;
            this.ability_stats += this.tradeoff_stats;
        }

        /*** public ***/

        public override string ToString()
        {
            return "Actor: Character: Level " + this.level + ": " + Card.StatLine(this.ability_stats, false);
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
