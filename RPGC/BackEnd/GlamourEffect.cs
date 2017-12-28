using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardExplorer;

namespace RPGC
{
    public class GlamourEffect : CardEffect
    {
        protected Glamour glamour;

        /*** constructor ***/

        public GlamourEffect(Glamour glamour, Piece actor, Piece target, Enemy enemy) : base( actor, target, enemy )
        {
            Game.Log(Game.LogLevel.TRACE, "% GlamourEffect Constructor %");
            this.glamour = glamour;

            //set the time
            int time = actor.GetLastActionTime();
            this.time = time + Battle.GetGlamourTime(glamour, actor, target,time);

            //check if we have the MP to cast this
            this.fizzle = (this.actor.GetMp() < this.CalculateCost());

            //spend the MP as part of creating this.
            if(!this.fizzle)
            {
                actor.ChangeMp(-this.CalculateCost(),time);
            }
        }

        /*** public ***/

        public override int ResolveEffect()
        {
            Game.Log(Game.LogLevel.TRACE, "% GlamourEffect.ResolveEffect %");
            //check if the actor is dead
            if (actor.IsDead()) return this.time;

            //set the time of the last action in the piece
            actor.SetLastActionTime(this.time);

            //the actor is ready for another action
            actor.SetResolvedAction();

            //find when the effect will wear off
            int stop = this.time + this.glamour.getDuration();

            //create a new Glamour Modifier
            GlamourModifier modifier = new GlamourModifier(this.glamour.GetAffect(), this.glamour.GetPotency(), this.time, stop);

            //apply it to the target
            target.ApplyGlamour(modifier);

            //find out if we need to update the enemy enmity
            bool attack = ( (this.target is Enemy) && (this.actor is Player) );

            //log the resolution
            Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + (attack ? " attacked ":" modified ") + this.target.ToString() + " with " + modifier.ToString() );

            //this was an attack
            if (attack)
            {
                //add some rightious hate
                this.enemy.AddEnmity( new Enmity(modifier), (Player)this.actor) ;
            }

            //return the time
            return this.time;
        }

        public override string ToString()
        {
            return this.actor.ToString() + " targets " + this.target.ToString() + " at " + this.time + " with " + this.glamour.ToString();
        }

        /*** protected ***/

        protected int CalculateCost()
        {
            int baseCost = 0;

            switch (this.glamour.GetAffect())
            {
                case Glamour.Affect.STRENGTH:
                case Glamour.Affect.GRIT:
                case Glamour.Affect.SPEED:
                case Glamour.Affect.BALANCE:
                case Glamour.Affect.FAITH:
                case Glamour.Affect.FOCUS:
                case Glamour.Affect.LUCK:
                case Glamour.Affect.ALLURE:
                    baseCost = 1 * this.glamour.GetPotency();
                    break;

                case Glamour.Affect.MAX_HIT_POINTS:
                case Glamour.Affect.MAX_MAGIC_POINTS:
                    baseCost = 2 * this.glamour.GetPotency();
                    break;

                case Glamour.Affect.RANGE:
                case Glamour.Affect.POSITION:

                    int potent = this.glamour.GetPotency();

                    //check if we overloaded the potency, see the Piece range and position, modifiers and correctors
                    if (potent > 5)
                    {
                        //overloaded
                        potent -= 10;
                        potent = Math.Abs(potent);
                        baseCost = 3 * potent;
                    }
                    else
                    {
                        potent = Math.Abs(potent);
                        baseCost = 2 * potent;
                    }
                    break;

                case Glamour.Affect.HIT_POINTS:
                case Glamour.Affect.MAGIC_POINTS:
                    baseCost = 3 * this.glamour.GetPotency();
                    break;

                case Glamour.Affect.MAX_DAMAGE:
                    baseCost = 4 * this.glamour.GetPotency();
                    break;

                case Glamour.Affect.LEVEL:
                    baseCost = 6 * this.glamour.GetPotency();
                    break;        
            }//switch

            int cost = baseCost * this.glamour.getDuration() / 10;

            return cost;
        }

    }
}
