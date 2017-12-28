using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardExplorer;

namespace RPGC
{
    public class Enemy : Piece
    {
        protected Creature creaturecard;

        protected Dictionary<Piece, List<Enmity>> enmities;

        /*** constructor ***/

        public Enemy() : base()
        {
            Game.Log(Game.LogLevel.TRACE, "% Enemy Constructor %");
            this.enmities = new Dictionary<Piece, List<Enmity>>();
        }

        /*** public ***/

        public override string ToString()
        {
            return "Enemy";
        }

        public override bool ApplyCard( Card card )
        {
            Game.Log(Game.LogLevel.TRACE, "% Enemy.ApplyCard %");
            if ( this.state != Piece.State.INIT )
            {
                return false;
            }

            if ( card is Creature )
            {
                creaturecard = (Creature)card;
                this.level = creaturecard.GetLevel();
                this.InitializeStats();
                this.state = Piece.State.WAITING;
                Game.Log(Game.LogLevel.NORMAL, this.ToString() + " becomes creature: " + card.ToString());
                return true;
            }

            return false;
        }

        public override int GetBaseStat(Card.Stat stat)
        {
            Game.Log(Game.LogLevel.TRACE, "% Enemy.GetBaseStat %");
            Matrix stats = creaturecard.GetStats();

            return (int)stats[(int)stat, 0];
        }

        public override int GetStat(Card.Stat stat, int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Enemy.GetStat %");
            int value = this.GetBaseStat(stat);

            //look through the glamours for more modifiers
            foreach (GlamourModifier modifier in this.glamours)
            {
                //see if the glamour affectes this stat
                if ((modifier.GetAffect() < Glamour.Affect.HIT_POINTS) && (Card.Stat)modifier.GetAffect() == stat)
                {
                    value += modifier.GetCurrentPotency(time);
                }
            }

            //check its not too small
            if (value < 1) value = 1;

            return value;
        }

        public override void SetHavingMoved(bool always = false)
        {
            Game.Log(Game.LogLevel.TRACE, "% Enemy.SetHavingMoved %");
            //handle state transition
            if (this.state == Piece.State.ACT)
            {
                //the enmey piece does not have a targeting state
                //this is called to undo act=>target, but for the 
                //enemy we will take act=>move
                Game.Log(Game.LogLevel.DEBUG, this.ToString() + " is now MOVING.");
                this.state = Piece.State.MOVE;
                this.cursor = this.location;
            }
            else if(always || (this.state == Piece.State.MOVE) )
            {
                Game.Log(Game.LogLevel.DEBUG, this.ToString() + " is now ACTING.");
                this.state = Piece.State.ACT;
            }
                
        }

        public void SelectTarget(Player[] players, int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Enemy.SelectTarget %");
            int[] total = new int[players.Length];
            int max = 0;
            int max_id = 0;
            for (int i = 0; i < players.Length; i++)
            {
                total[i] = 0;
                List<Enmity> personalEnmity;
                if (this.enmities.TryGetValue(players[i], out personalEnmity))
                {
                    foreach (Enmity enmity in personalEnmity)
                    {
                        total[i] += enmity.AtTime(time);
                    }
                }

                //degrade the enemity by a lot if the player is dead.
                if( players[i].IsDead() )
                {
                    total[i] /= 100;
                }

                //check if it is the max
                if (total[i] > max)
                {
                    max = total[i];
                    max_id = i;
                }
            }//for players

            //no player had enmity, use allure
            if (max == 0)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    double allure = players[i].GetStat(Card.Stat.ALLURE, time);
                    allure = allure * Battle.Level(players[i].GetLevel(time));
                    if (allure > max)
                    {
                        max = (int)allure;
                        max_id = i;
                    }
                }//for players
            }//if max =  0

            this.SetTarget(players[max_id]);
        }

        public void AddEnmity( Enmity enmity, Player player)
        {
            Game.Log(Game.LogLevel.TRACE, "% Enemy.AddEnmity %");
            if (enmity == null) return;
            List<Enmity> personalEnmity;
            if( this.enmities.TryGetValue(player, out personalEnmity) )
            {
                personalEnmity.Add(enmity);
            }
            else
            {
                personalEnmity = new List<Enmity>();
                personalEnmity.Add(enmity);
                this.enmities.Add(player,personalEnmity);
            }
        }
    }

    public class Enmity
    {
        protected int enmity;
        protected int time;
        protected int halflife;

        /*** constructor ***/

        public Enmity(GlamourModifier modifier)
        {
            Game.Log(Game.LogLevel.TRACE, "% Enmity Constructor %");
            this.enmity = Math.Abs( modifier.GetPotency() * modifier.GetDuration() / 10 );
            this.time = modifier.GetStartTime();
            this.halflife = modifier.GetDuration() / 2;
            if (this.halflife <= 0) this.halflife = 1;
        }

        public Enmity(int time, int level, int allure, int damage, int memory )
        {
            Game.Log(Game.LogLevel.TRACE, "% Enmity Constructor %");
            if (allure < 1) allure = 1;
            this.enmity = (int)( damage * allure * Battle.Level(level) );
            this.time = time;
            this.halflife = memory / 2;
            if (this.halflife <= 0) this.halflife = 1;
        }

        /*** public ***/

        public int AtTime( int time )
        {
            Game.Log(Game.LogLevel.TRACE, "% Enmity.AtTime %");
            if ( time < this.time )
            {
                return 0;
            }

            int elapsed = time - this.time;

            return (int)(this.enmity * Math.Pow(2, -elapsed / this.halflife));
        }
    }
}
