using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RPGC
{
    public abstract class CardEffect : IComparable<CardEffect>  
    {
        protected int time;  //by convention enemy is played on odd times, players even

        protected Battle.Side side;

        protected Piece actor;
        protected Piece target;
        protected Enemy enemy;

        protected bool fizzle;

        /*** constructor ***/

        protected CardEffect(Piece actor, Piece target, Enemy enemy)
        {
            Game.Log(Game.LogLevel.TRACE, "% CardEffect Constructor %");
            this.actor = actor;
            this.target = target;
            this.enemy = enemy;
            this.fizzle = false;
            if (this.actor is Player)
            {
                this.side = Battle.Side.PLAYERS;
            }
            else
            {
                this.side = Battle.Side.ENEMY;
            }
        }

        /*** interface ***/

        int IComparable<CardEffect>.CompareTo(CardEffect effect)
        {
            // A null value means that this object is greater.
            if (effect == null)
                return 1;
            else
                return this.time.CompareTo(effect.time);
        }

        /*** public ***/

        public abstract int ResolveEffect(); //this will set the player to ready or dead

        public void SetFirstStrike()
        {
            this.time = 0;
        }

        public int GetTime()
        {
            return this.time;
        }

        public Battle.Side GetSide()
        {
            return this.side;
        }

        public bool Fizzled()
        {
            return this.fizzle;
        }

        public Piece GetActor()
        {
            return this.actor;
        }
    }
}
