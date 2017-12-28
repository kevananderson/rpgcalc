using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    class Familiar : Actor
    {
        protected int level;
        protected static int level_mask = 0x0E00;
        protected static int level_shift = 9;

        protected int tradeoff;
        protected static int tradeoff_mask = 0x01FF;
        protected static int tradeoff_shift = 0;
        protected static int tradeoff_num = 9;
        protected Matrix tradeoff_choice;
        protected Matrix tradeoff_stats;
        protected static int ability_base = 7;
        protected Matrix ability_stats;

        protected static String tradeoff_table =
        " 2 -1  0  0  0  0  0  0  1 \r\n" + //STR
        " 0  2  0 -1  0  0 -1  0  1 \r\n" + //GRT
        "-1  0  2  0  0  0  0  0 -1 \r\n" + //SPD
        " 0  0 -1  2  0  0  0  1 -1 \r\n" + //BAL
        " 0  0  2 -1  2 -1  0  0  0 \r\n" + //FTH
        " 0  0 -1  0 -1  2  0  1  0 \r\n" + //FOC
        "-1  0  0  0 -1  0  2 -1  0 \r\n" + //LCK
        " 0 -1  0  0  0 -1 -1 -1  0 \r\n";  //ALL
        protected static Matrix tradeoff_matrix;

        /*** constructor ***/

        internal Familiar(int cid) : base(cid)
        {
            if (Familiar.tradeoff_matrix == null)
            {
                Familiar.tradeoff_matrix = Matrix.Parse(Familiar.tradeoff_table);
            }

            this.level = (cid & Familiar.level_mask) >> Familiar.level_shift;
            this.level *= 5;
            if (this.level == 0)
                this.level++;

            this.tradeoff = (cid & Familiar.tradeoff_mask) >> Familiar.tradeoff_shift;
            this.tradeoff_choice = Card.Tradeoff(this.tradeoff, Familiar.tradeoff_num);
            this.tradeoff_stats = Familiar.tradeoff_matrix * this.tradeoff_choice;

            this.ability_stats = Matrix.OnesMatrix(8, 1);
            this.ability_stats = (double)Familiar.ability_base * this.ability_stats;
            this.ability_stats += this.tradeoff_stats;

        }

        /*** public ***/

        public override string ToString()
        {
            return "Actor: Familiar: Level " + this.level + ": " + Card.StatLine(this.ability_stats, false);
        }

        public override Matrix GetStats()
        {
            return ability_stats;
        }

        /*** protected ***/

    }
}
