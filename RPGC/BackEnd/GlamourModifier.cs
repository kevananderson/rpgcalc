using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardExplorer;

namespace RPGC
{
    public class GlamourModifier
    {
        Glamour.Affect affect;
        int potency;
        int start;
        int stop;

        /*** constructor ***/

        public GlamourModifier(Glamour.Affect affect, int potency, int start, int stop)
        {
            Game.Log(Game.LogLevel.TRACE, "% GlamourModifier Constructor %");

            this.affect = affect;
            this.potency = potency;
            this.start = start;
            this.stop = stop;
        }

        /*** public ***/

        public Glamour.Affect GetAffect()
        {
            return this.affect;
        }

        public int GetPotency()
        {
            return this.potency;
        }

        public int GetCurrentPotency(int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% GlamourModifier.GetCurrentPotency %");
            if ((time <= this.stop) && (time >= this.start))
            {
                return this.potency;
            }
            return 0;
        }

        public int GetDuration()
        {
            return this.stop - this.start;
        }

        public int GetStartTime()
        {
            return this.start;
        }

        public int IntegrateGlamour(int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% GlamourModifier.IntegrateGlamour %");
            //check if the affect is active
            if (this.GetCurrentPotency(time) == 0) return 0;

            //get the rate of change
            double rate = 0.3 * this.GetCurrentPotency(time);

            //get the time we are acting
            int duration = time - this.start;

            //change the start to now so we dont re-do this
            this.start = time;

            //round the time to the nearest
            int effect = (int)(duration * rate);

            return effect;
        }

        public bool IsHarmful()
        {
            Game.Log(Game.LogLevel.TRACE, "% GlamourModifier.IsHarmful %");
            switch (this.affect)
            {
                case Glamour.Affect.STRENGTH:
                case Glamour.Affect.GRIT:
                case Glamour.Affect.SPEED:
                case Glamour.Affect.BALANCE:
                case Glamour.Affect.FAITH:
                case Glamour.Affect.FOCUS:
                case Glamour.Affect.LUCK:
                case Glamour.Affect.HIT_POINTS:
                case Glamour.Affect.MAGIC_POINTS:
                case Glamour.Affect.MAX_HIT_POINTS:
                case Glamour.Affect.MAX_MAGIC_POINTS:
                case Glamour.Affect.MAX_DAMAGE:
                case Glamour.Affect.LEVEL:
                    return (this.potency < 0);

                case Glamour.Affect.ALLURE:
                    return (this.potency > 0);

                case Glamour.Affect.RANGE:
                case Glamour.Affect.POSITION:
                    return (this.potency < 5);
                default:
                    return true;
            }
        }

        public int IsCured(Cure.Effect effect)
        {
            Game.Log(Game.LogLevel.TRACE, "% GlamourModifier.IsCured %");
            switch (effect)
            {
                case Cure.Effect.STRENGTH:
                case Cure.Effect.GRIT:
                case Cure.Effect.SPEED:
                case Cure.Effect.BALANCE:
                case Cure.Effect.FAITH:
                case Cure.Effect.FOCUS:
                case Cure.Effect.LUCK:
                case Cure.Effect.ALLURE:
                    if (effect == (Cure.Effect)this.affect) return 1;
                    break;

                case Cure.Effect.MAX_HIT_POINTS:
                case Cure.Effect.MAX_MAGIC_POINTS:
                case Cure.Effect.RANGE:
                case Cure.Effect.POSITION:
                    if (effect == (Cure.Effect)this.affect) return 2;
                    break;

                case Cure.Effect.STATS:
                    if (this.affect <= Glamour.Affect.ALLURE) return 2;
                    break;

                case Cure.Effect.HIT_POINTS:
                case Cure.Effect.MAGIC_POINTS:
                case Cure.Effect.MAX_DAMAGE:
                case Cure.Effect.LEVEL:
                    if (effect == (Cure.Effect)this.affect) return 3;
                    break;

                case Cure.Effect.LOCATION:
                    if ( (this.affect == Glamour.Affect.RANGE) ||
                         (this.affect == Glamour.Affect.POSITION)) return 4;
                    break;

                case Cure.Effect.MAXES:
                    if ( (this.affect == Glamour.Affect.MAX_HIT_POINTS) ||
                         (this.affect == Glamour.Affect.MAX_MAGIC_POINTS) ||
                         (this.affect == Glamour.Affect.MAX_DAMAGE) ) return 5;
                    break;

                case Cure.Effect.POINTS:
                    if ( (this.affect == Glamour.Affect.HIT_POINTS) ||
                         (this.affect == Glamour.Affect.MAGIC_POINTS) ) return 6;
                    break;


                case Cure.Effect.ALL:
                    return 8;

                default:
                    return 0;

            }//switch

            return 0;
        }

        public override string ToString()
        {
            return Glamour.GetAffectName(this.affect) + " (potency " + this.potency.ToString("+#;-#;0") + " : start " + this.start + " : stop " + this.stop + ")";
        }
    }
}