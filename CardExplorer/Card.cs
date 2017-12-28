using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CardExplorer  
{
    public abstract class Card
    {
        public enum Stat { STRENGTH, GRIT, SPEED, BALANCE, FAITH, FOCUS, LUCK, ALLURE };

        public static String[] stat_long_string = { "Strength", "Grit", "Speed", "Balance", "Faith", "Focus", "Luck", "Allure" };
        public static String[] stat_short_string = { "STR", "GRT", "SPD", "BAL", "FTH", "FOC", "LCK", "ALL" };
        public static String[] group_string = { "Actor", "Equipment", "Skill", "Glamor" };
        public static int group_mask = 0xC000;
        public static int group_shift = 14;

        public enum Group { ACTOR, EQUIPMENT, SKILL, GLAMOR };

        protected int cid;

        protected Card.Group group;

        /*** public ***/

        public static Card Factory(int cid)
        {
            switch( (Card.Group)((cid & Card.group_mask) >> Card.group_shift) )
            {
                case Group.ACTOR:
                    switch( (Actor.Type)((cid & Actor.type_mask) >> Actor.type_shift) )
                    {
                        case Actor.Type.CHARACTER:
                            return (Card)(new Character(cid));
                        case Actor.Type.CREATURE:
                            return (Card)(new Creature(cid));
                        case Actor.Type.FAMILIAR:
                            return (Card)(new Familiar(cid));
                        case Actor.Type.ROLE:
                            return (Card)(new Role(cid));
                        default:
                            return null;
                    }
                case Group.EQUIPMENT:
                    return (Card)(new Equipment(cid));
                case Group.SKILL:
                    switch ((Skill.Type)((cid & Skill.type_mask) >> Skill.type_shift))
                    {
                        case Skill.Type.HEAL:
                            return (Card)(new Heal(cid));
                        case Skill.Type.CURE:
                            return (Card)(new Cure(cid));
                        default:
                            return (Card)(new Skill(cid));
                    }
                case Group.GLAMOR:
                    return (Card)( new Glamour(cid) );
                default:
                    return null;
            }
        }

        public static Card Factory(string value)
        {
            int num = -1;
            Int32.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out num);

            //check if the string is the correct format then try it
            if ((value.Length == 4) && (num >= 0) )
            {
                return Card.Factory(num);
            }

            return null;
        }

        /*** protected ***/

        protected Card( int cid )
        {
            this.cid = cid;
            this.group = (Card.Group)((cid & Card.group_mask) >> Card.group_shift);
        }

        protected static Matrix Tradeoff(int value, int length)
        {
            if (length <= 0)
                return null;

            Matrix matrix = new Matrix(length, 1);
            matrix[0, 0] = value & 1;
            for (int i = 1; i < length; i++)
            {
                value >>= 1;
                matrix[i, 0] = value & 1;
            }
            return matrix;
        }

        protected static String StatLine(Matrix stats, bool mod_only)
        {
            if ((stats.rows != 8) && (stats.cols != 1))
            {
                throw new Exception("StatLine can only print 8x1 matrices.");
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                if (mod_only && stats[i, 0] == 0)
                    continue;

                builder.Append("  " + Card.stat_short_string[i] + "=");
                builder.Append(((int)stats[i, 0]).ToString("+#;-#;0").PadLeft(2));
            }
            return builder.ToString().Trim();
        }

    }
}
