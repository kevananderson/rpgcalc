using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    public class Skill : Card
    {
        public enum Type { EDGE, BLUNT, PIERCING, RANGED, SPELL, HEAL, CURE, RECRUIT };
        public enum Speed { SLOW, FAST };
        public enum Area { DIRECTED, AREA };
        public enum Position { FORWARD, FRONT_FLANK, BACK_FLANK, REAR };

        public static String[] type_string = { "Edged Weapon", "Blunt Weapon", "Piercing Weapon", "Ranged Weapon",
                                                    "Spell", "Heal", "Cure", "Recruit" };
        public static int type_mask = 0x3800;
        public static int type_shift = 11;

        public static String[] range_string = { "1", "2", "3", "4" };
        public static int range_mask = 0x0600;
        public static int range_shift = 9;

        public static String[] speed_string = { "Slow", "Fast" };
        public static int speed_mask = 0x0100;
        public static int speed_shift = 8;

        public static String[] area_string = { "Directed", "Area" };
        public static int area_mask = 0x0080;
        public static int area_shift = 7;

        public static String[] position_string = { "Forward", "Front Flank", "Back Flank", "Rear" };
        public static int position_mask = 0x0060;
        public static int position_shift = 5;

        public static String[] max_string = { "14", "21", "28", "35", "42", "49", "56", "63", "70", "77", "84", "91", "98","105","112","119",
                                             "126","133","140","147","154","161","168","175","182","189","196","203","210","217","224","231" };
        public static int max_mask = 0x001F;
        public static int max_shift = 0;

        protected Skill.Type type;
        protected int range;
        protected Skill.Speed speed;
        protected Skill.Area area;
        protected Skill.Position position;
        protected int max;

        /*** constructor ***/

        internal Skill( int cid ) : base(cid)
        {
            this.type = (Skill.Type)((cid & Skill.type_mask) >> Skill.type_shift);
            this.range = (cid & Skill.range_mask) >> Skill.range_shift;
            this.range++;
            this.speed = (Skill.Speed)((cid & Skill.speed_mask) >> Skill.speed_shift);
            this.area = (Skill.Area)((cid & Skill.area_mask) >> Skill.area_shift);
            this.position = (Skill.Position)((cid & Skill.position_mask) >> Skill.position_shift);
            this.max = (cid & Skill.max_mask) >> Skill.max_shift;
            this.max *= 7;
            this.max += 14;
        }

        /*** public ***/

        public override string ToString()
        {
            return "Skill: " + Skill.type_string[(int)this.type] + ": Range " + this.range +
                ": Speed " + Skill.speed_string[(int)this.speed] + ": " + Skill.area_string[(int)this.area] + 
                ": Position " + Skill.position_string[(int)this.position] + ": Max Affect " + this.max;
        }

        public Skill.Type GetSkillType()
        {
            return this.type;
        }

        public string GetSkillName()
        {
            return Skill.type_string[(int)this.type];
        }

        public static string GetSkillName(Skill.Type type)
        {
            return Skill.type_string[(int)type];
        }


        public int GetRange()
        {
            return this.range;
        }

        public Skill.Speed GetSpeed()
        {
            return this.speed;
        }

        public Skill.Area GetArea()
        {
            return this.area;
        }

        public Skill.Position GetPosition()
        {
            return this.position;
        }

        public int GetMaxDamage()
        {
            return this.max;
        }

        /*** protected ***/

    }
}
