using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGC
{
    public class Timeline
    {
        protected List<CardEffect> timeline = new List<CardEffect>();
        protected bool firstStrike = true;
        protected Piece[] players;
        protected Piece enemy;
        protected int time;
        protected Battle.Side firstSide;

        /*** constructor ***/

        public Timeline( Piece[] players, Piece enemy)
        {
            Game.Log(Game.LogLevel.TRACE, "% Timeline Constructor %");
            this.players = players;
            this.enemy = enemy;
            this.time = 0;
            this.firstSide = Battle.Side.UNKNOWN;
        } 

        /*** public ***/

        public bool AddEffect( CardEffect effect)
        {
            Game.Log(Game.LogLevel.TRACE, "% Timeline.AddEffect %");
            //a null may be sent, check for it
            if ( (effect == null) || effect.Fizzled() ) return false;

            //if this is the first effect added to the timeline, make it first strike.
            if (this.firstStrike)
            {
                effect.SetFirstStrike();
                this.firstStrike = false;
                if( effect.GetActor() == this.enemy )
                {
                    this.firstSide = Battle.Side.ENEMY;
                }
                else
                {
                    this.firstSide = Battle.Side.PLAYERS;
                }
            }

            this.timeline.Add(effect);
            this.timeline.Sort();

            return true;
        }

        public bool ResolveActions()
        {
            Game.Log(Game.LogLevel.TRACE, "% Timeline.ResolveAction %");
            int time = 0;

            //check that we are not empty
            if (this.timeline.Count == 0) return false;

            //pull the first element out of the array
            CardEffect action = this.timeline.First();

            //ensure that the timeline is full or  first strike 
            if (!this.IsFull() && (action.GetTime() != 0)) return false;

            //now we can remove the action and start resolving
            this.timeline.RemoveAt(0);

            //what side are we resolving
            Battle.Side side = action.GetSide();

            //resolve the glamours up to this time
            this.ResolveGlamours(action.GetTime());

            //resolve the action
            time = action.ResolveEffect();

            //check if it is first strike, cause we stop in that case
            if( action.GetTime() == 0 )
            {
                //this was the first strike
                return true;
            }

            //we will keep resolving until the next action comes from the other side
            while( (this.timeline.FirstOrDefault() != null) && (this.timeline.First().GetSide() == side) )
            {
                //remove the first element
                action = this.timeline.First();
                this.timeline.RemoveAt(0);

                //resolve the glamours up to this time
                this.ResolveGlamours(action.GetTime());

                //resolve the action
                time = action.ResolveEffect();
            }

            //check if the time is after the current time
            if (time > this.time) this.time = time;

            return true;
        }

        public int GetTime()
        {
            return this.time;
        }

        public Battle.Side GetFirstSide()
        {
            return this.firstSide;
        }

        public void ResolveGlamours( int time )
        {
            Game.Log(Game.LogLevel.TRACE, "% Timeline.ResolveGlamours %");
            //resolve player glamour
            foreach (Player player in this.players )
            {
                player.ResolveGlamours(time);
            }

            //resolve enemy glamours
            this.enemy.ResolveGlamours(time);
        }
        
        public bool IsFull()
        {
            foreach (Player player in this.players )
            {
                //no need to check for dead players
                if (player.IsDead()) continue;

                //we have to find something for everyone for it to be full
                if (!this.Active(player)) return false;
            }//foreach player

            //all the players were accounted for or dead, check the enemy
            if (!this.enemy.IsDead() && !this.Active(this.enemy)) return false;

            //we must be full
            return true;
        }

        public bool HasCountered()
        {
            if( this.firstSide == Battle.Side.PLAYERS )
            {
                //we are waiting for the enemy to become active
                return this.Active(this.enemy);
            }
            else
            {
                //we are waiting for all the players to be active
                foreach (Player player in this.players)
                {
                    //no need to check for dead players
                    if (player.IsDead()) continue;

                    //we have to find something for everyone for it to be full
                    if (!this.Active(player)) return false;
                }//foreach player
                return true;
            }
        }

        public bool Active( Piece piece )
        {
            //search the timeline for the effect
            foreach (CardEffect effect in this.timeline)
            {
                if (effect.GetActor() == piece) return true;
            }//foreach effect
            return false;
        }
    }
}
