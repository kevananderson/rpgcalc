using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    public class Role : Actor
    {
        public static int tradeoff_mask = 0x0FFF;
        public static int tradeoff_shift = 0;
        public static int tradeoff_num = 12;
        public static String tradeoff_table =
       // 0  1  2  3  4  5  6  7  8  9 10 11
        " 0  0  0 -1  2  0  0 -1  1  0  1  0 \r\n" + //STR
        "-1  0  0  0  0  2  0  0  0  1  1  0 \r\n" + //GRT
        " 0 -1  0 -1 -1  0  2  0 -1  0 -1  0 \r\n" + //SPD
        " 0  0  0  2 -1  0  0  0  0 -1 -1  0 \r\n" + //BAL
        " 0 -1  2  0  0  0 -1  0  0  1  0  1 \r\n" + //FTH
        " 0  2 -1  0  0  0  0  0 -1  0  0  1 \r\n" + //FOC
        "-1  0 -1  0  0 -1 -1  2  0 -1  0 -1 \r\n" + //LCK
        " 2  0  0  0  0 -1  0 -1  1  0  0 -1 \r\n";  //ALL

        protected int tradeoff;
        protected Matrix tradeoff_choice;
        protected Matrix tradeoff_stats;

        protected static Matrix tradeoff_matrix;

        /*** constructor ***/

        internal Role( int cid ) : base(cid)
        {
            if( Role.tradeoff_matrix == null)
            {
                Role.tradeoff_matrix = Matrix.Parse(Role.tradeoff_table);
            }

            this.tradeoff = (cid & Role.tradeoff_mask) >> Role.tradeoff_shift;
            this.tradeoff_choice = Card.Tradeoff(this.tradeoff, Role.tradeoff_num);
            this.tradeoff_stats = Role.tradeoff_matrix * this.tradeoff_choice;
        }

        /*** public ***/

        public override string ToString()
        {
            return "Actor: Role: " + Card.StatLine(this.tradeoff_stats, false);
        }

        public override Matrix GetStats()
        {
            return tradeoff_stats;
        }

        public string GetRoleID()
        {
            return this.tradeoff.ToString("D4");
        }

        /*** protected ***/

    }
}
