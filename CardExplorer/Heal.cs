using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    public class Heal : Skill
    {
        public static String[] level_string = { "2", "4", "6", "8","10","12","14","16","18","20","22","24","26","28","30","32" };
        public static int level_shift = 2;


        protected int level;

        /*** constructor ***/

        internal Heal( int cid ) : base(cid)
        {
            //this card does not use the range and position combine them
            this.range--;
            this.level = (((int)(this.range)) << 3) + (((int)(this.position)) << 1);
            this.level += 2;
            this.range = 1; //healing works best the closer you are
            this.position = Skill.Position.FORWARD; // this will not matter
        }

        /*** public ***/

        public override string ToString()
        {
            return "Skill: " + Skill.type_string[(int)this.type] + ": Speed " + Skill.speed_string[(int)this.speed] + 
                ": " + Skill.area_string[(int)this.area] + ": Level " + this.level + ": Max Affect " + this.max;
        }

        public int GetLevel()
        {
            return this.level;
        }

        /*** protected ***/

    }
}
