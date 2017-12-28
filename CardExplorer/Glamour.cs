using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    public class Glamour : Card
    {
        public enum Affect { STRENGTH, GRIT, SPEED, BALANCE, FAITH, FOCUS, LUCK, ALLURE,
            HIT_POINTS, MAGIC_POINTS, MAX_HIT_POINTS, MAX_MAGIC_POINTS, RANGE, POSITION,
            MAX_DAMAGE, LEVEL };

        public static int MAX_RANGE_MOD = 2;
        public static int MIN_RANGE_MOD = -2;
        public static int MAX_POSITION_MOD = 3;
        public static int MIN_POSITION_MOD = -3;
        public static int MAX_CORRECTION = -1;
        public static int MIN_CORRECTION = -4;
        public static int CORRECTION_OFFSET = 10;

        public static int MAX_MULTIPLE = 5;


        public static String[] affect_string = { "Strength", "Grit", "Speed", "Balance",
            "Faith", "Focus", "Luck", "Allure", "Hit Points", "Magic Points", "Maximum Hit Points",
            "Maximum Magic Points", "Range", "Position", "Maximum Damage", "Level" };
        public static int affect_mask = 0x3C00;
        public static int affect_shift = 10;

        public static String[] potency_string = {   "0",  "1",  "2",  "3",  "4",  "5",  "6", "7", "8", "9","10","11","12","13","14","15",
                                                  "-16","-15","-14","-13","-12","-11","-10","-9","-8","-7","-6","-5","-4","-3","-2","-1" };
        public static String[] max_string =     {   "0",  "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75",
                                                  "-80","-75","-70","-65","-60","-55","-50","-45","-40","-35","-30","-25","-20","-15","-10","-5" };
        public static String[] range_string =     { "0", "1", "2", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*",
                                                    "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*","-2","-1" };
        public static String[] position_string =  { "0", "1", "2", "3", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*",
                                                    "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*","-3","-2","-1" };
        public static String[] correct_string =   { "*", "*", "*", "*", "*", "*","-4","-3","-2","-1", "*", "*", "*", "*", "*", "*",
                                                    "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*" };
        public static int potency_mask = 0x03F0;
        public static int potency_shift = 4;

        public static String[] duration_string = { "600", "20", "40", "60", "80","100","120","140","160","180","200","220","240","260","280","300" };
        public static int duration_mask = 0x000F;
        public static int duration_shift = 0;

        protected Glamour.Affect affect;
        protected int potency;
        protected int duration;

        /*** constructor ***/

        internal Glamour( int cid ) : base(cid)
        {
            this.affect = (Glamour.Affect)((cid & Glamour.affect_mask) >> Glamour.affect_shift);
            this.potency = (cid & Glamour.potency_mask) >> Glamour.potency_shift;
            ushort u16 = (ushort)(this.potency << 11);
            short s16 = (short)u16;
            this.potency = (int)s16;
            this.potency >>= 11;
            this.duration = (cid & Glamour.duration_mask) >> Glamour.duration_shift;
            this.duration *= 20;
            if (this.duration == 0) this.duration = 600;
        }

        /*** public ***/

        public override string ToString()
        {
            int potent = this.potency;
            string potentToString = potent.ToString("+#;-#;0");

            //are we a max
            if( this.affect == Affect.MAX_HIT_POINTS || this.affect == Affect.MAX_MAGIC_POINTS || this.affect == Affect.MAX_DAMAGE)
            {
                potent *= Glamour.MAX_MULTIPLE;
                potentToString = potent.ToString("+#;-#;0");
            }

            //are we range or position?
            if( this.affect == Affect.RANGE)
            {
                if (potent < Glamour.MIN_RANGE_MOD)
                    potentToString = "Invalid";
                else if (potent > Glamour.MAX_RANGE_MOD)
                {
                    potent -= Glamour.CORRECTION_OFFSET;
                    if (potent < Glamour.MIN_CORRECTION || potent > Glamour.MAX_CORRECTION)
                    {
                        potentToString = "Invalid";
                    }
                    else
                    {
                        potentToString = "Correction " + potent.ToString("+#;-#;0");
                    }//else not a correction
                }//else not a modification
                else
                {
                    potentToString = "Modification " + potentToString;
                }
            }//if range

            if (this.affect == Affect.POSITION)
            {
                if (potent < Glamour.MIN_POSITION_MOD)
                    potentToString = "Invalid";
                else if (potent > Glamour.MAX_POSITION_MOD)
                {
                    potent -= Glamour.CORRECTION_OFFSET;
                    if (potent < Glamour.MIN_CORRECTION || potent > Glamour.MAX_CORRECTION)
                    {
                        potentToString = "Invalid";
                    }
                    else
                    {
                        potentToString = "Correction " + potent.ToString("+#;-#;0");
                    }//else not a correction
                }//else not a modification
                else
                {
                    potentToString = "Modification " + potentToString;
                }
            }//if position

            return "Glamour: " + Glamour.affect_string[(int) this.affect] + ": Duration " + this.duration + ": Potency " + potentToString;
        }

        public Glamour.Affect GetAffect()
        {
            return this.affect;
        }

        public string GetAffectName()
        {
            return Glamour.affect_string[(int)this.affect];
        }

        public static string GetAffectName(Glamour.Affect affect)
        {
            return Glamour.affect_string[(int)affect];
        }

        public int GetPotency()
        {
            return this.potency;
        }

        public int getDuration()
        {
            return this.duration;
        }

        /*** protected ***/

    }
}
