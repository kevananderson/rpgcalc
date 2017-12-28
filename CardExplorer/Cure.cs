using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    public class Cure : Skill
    {
        public enum Effect
        {
            STRENGTH, GRIT, SPEED, BALANCE, FAITH, FOCUS, LUCK, ALLURE,
            HIT_POINTS, MAGIC_POINTS, MAX_HIT_POINTS, MAX_MAGIC_POINTS, RANGE, POSITION,
            MAX_DAMAGE, LEVEL, STATS, POINTS, MAXES, LOCATION, ALL
        };
        public static String[] effect_string = { "Strength", "Grit", "Speed", "Balance",
            "Faith", "Focus", "Luck", "Allure", "Hit Points", "Magic Points", "Maximum Hit Points",
            "Maximum Magic Points", "Range", "Position", "Maximum Damage", "Level",
            "All Stats", "All Points", "All Maxes", "All Locations", "All Glamours" };

        public static String[] num_string = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };

        public static int level_shift = 2;

        protected Cure.Effect effect;

        /*** constructor ***/

        internal Cure( int cid ) : base(cid)
        {
            //we convert the max damage to the type of the glamour we can cure
            this.max -= 14;
            this.max /= 7;
            if (this.max > (int)Cure.Effect.ALL) this.max = (int)Cure.Effect.ALL;
            this.effect = (Cure.Effect)this.max;

            //this card does not use the range and position combine them to make the max
            this.range--;
            this.max = (((int)(this.range)) << 2) + (((int)(this.position)) << 0);
            this.max++;

            this.range = 1; //this will not matter
            this.position = Skill.Position.FORWARD; // this will not matter
        }

        /*** public ***/

        public override string ToString()
        {
            return "Skill: " + Skill.type_string[(int)this.type] + ": Speed " + Skill.speed_string[(int)this.speed] + 
                ": " + Skill.area_string[(int)this.area] + ": Effect " + Cure.effect_string[(int)this.effect] + ": Num Effect " + this.max;
        }

        public Cure.Effect GetEffect()
        {
            return this.effect;
        }

        public string GetEffectName()
        {
            return Cure.effect_string[(int)this.effect];
        }
        /*** protected ***/

    }
}
