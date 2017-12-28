using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardExplorer;

namespace RPGC
{
    public abstract class Piece
    {
        protected List<GlamourModifier> glamours = new List<GlamourModifier>();

        public enum State { INIT, WAITING, BUSY, MOVE, TARGET, ACT, DEAD };

        protected Piece.State state;
        protected static String[] state_string = { "INIT", "WAIT", "BUSY", "MOVE", "TARGET", "ACT", "DEAD"};

        protected const double POW = 1.1;

        protected Piece target;

        protected Hex lastLocation;
        protected Hex location;

        protected Hex cursor;

        protected int level;
        protected int max_hp;
        protected int hp;
        protected int max_mp;
        protected int mp;

        protected int lastActionTime;

        /*** constructor ***/
        public Piece()
        {
            this.state = Piece.State.INIT;
            this.lastActionTime = 0;
        }

        /*** public ***/

        public abstract bool ApplyCard(Card card);

        public abstract int GetBaseStat(Card.Stat stat);

        public abstract int GetStat(Card.Stat stat, int time);

        public void InitializeStats()
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.InitializeStats %");
            //set the HP and MP for our character

            //for hp we use strength and grit
            int strength = this.GetBaseStat(Card.Stat.STRENGTH);
            int grit = this.GetBaseStat(Card.Stat.GRIT);
            this.max_hp = (int)((2 * strength + 3 * grit) * Piece.Level(this.level));
            this.hp = this.max_hp;

            //for mp we use faith and focus
            int faith = this.GetBaseStat(Card.Stat.FAITH);
            int focus = this.GetBaseStat(Card.Stat.FOCUS);
            this.max_mp = (int)((1.5 * faith + 2 * focus) * Piece.Level(this.level));
            this.mp = this.max_mp;
        }

        public static double Level( double level )
        {
            return Math.Pow(Piece.POW, level);
        }

        public virtual int GetTimeDelay(int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.GetTimeDelay %");
            //this provides the time from moving
            int distance = Board.Distance(this.location, this.lastLocation);
            double runTime = 10 * distance;
            double modifier = Battle.GetSpeedModifier(this, time);
            return (int)(runTime * modifier);
        }

        public bool IsInitialized()
        {
            return (this.state != Piece.State.INIT);
        }

        public bool IsReady()
        {
            return ( (this.state == Piece.State.MOVE) ||
                     (this.state == Piece.State.TARGET) ||
                     (this.state == Piece.State.ACT) );
        }

        public bool IsUsingCursor()
        {
            return ((this.state == Piece.State.MOVE) ||
                     (this.state == Piece.State.TARGET));
        }

        public bool CanUndo()
        {
            return ((this.state == Piece.State.ACT) ||
                     (this.state == Piece.State.TARGET));
        }

        public bool IsActing()
        {
            return (this.state == Piece.State.ACT);
        }

        public bool IsMoving()
        {
            return (this.state == Piece.State.MOVE);
        }

        public bool IsTargeting()
        {
            return (this.state == Piece.State.TARGET);
        }

        public bool IsBusy()
        {
            return (this.state == Piece.State.BUSY);
        }

        public bool IsDead()
        {
            return (this.state == Piece.State.DEAD);
        }

        public void SetLastActionTime(int time)
        {
            this.lastActionTime = time;
        }

        //only called by the board
        public void SetLocation(Hex hex)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.SetLocation %");
            //are we initializing
            if ( this.location == null )
            {
                //initializing
                this.location = hex;
                this.lastLocation = hex;
            }
            this.location = hex; 
        }

        public Hex GetLocation()
        {
            return this.location;
        }

        public bool MoveCursor(Board.Move move)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.MoveCursor %");
            Hex nextCursor = this.cursor.GetHexByMove(move);
            if (nextCursor == null) return false;
            this.cursor = nextCursor;
            return true;
        }

        public Hex GetCursor()
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.GetCursor %");
            if (this.cursor == null)
                this.cursor = this.location;
            return this.cursor;
        }

        public virtual void SetResolvedAction( bool always = false )
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.SetResolvedAction %");
            if ( always || (this.state == Piece.State.WAITING) || (this.state == Piece.State.BUSY) )
            {
                Game.Log(Game.LogLevel.DEBUG, this.ToString() + " is now MOVING.");
                this.state = Piece.State.MOVE;
                this.cursor = this.location;
            }
        }

        public virtual void SetHavingMoved(bool always = false)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.SetHavingMoved %");
            if (always || (this.state == Piece.State.MOVE) )
            {
                Game.Log(Game.LogLevel.DEBUG, this.ToString() + " is now TARGETING.");
                this.state = Piece.State.TARGET;
                this.cursor = this.target.GetLocation();
                if (this.cursor == null)
                    this.cursor = this.location;
            }
        }

        public virtual void SetHavingTargeted()
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.SetHavingTargeted %");
            //handle state transition
            if (this.state == Piece.State.TARGET)
            {
                Game.Log(Game.LogLevel.DEBUG, this.ToString() + " is now ACTING.");
                this.state = Piece.State.ACT;
            }
        }

        public virtual void SetTookAction(bool always = false)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.SetTookAction %");
            if ( always || (this.state == Piece.State.WAITING) || (this.state == Piece.State.ACT) )
            {
                Game.Log(Game.LogLevel.DEBUG, this.ToString() + " is now BUSY.");
                this.lastLocation = this.location;
                this.state = Piece.State.BUSY;
            }                
        }

        public void SetHavingDefeatedEnemy()
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.SetHavingDefeatedEnemy %");
            Game.Log(Game.LogLevel.DEBUG, this.ToString() + " is now WAITING.");
            this.state = Piece.State.WAITING;
        }

        public int GetLastActionTime()
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.GetLastActionTime %");
            //if we are a player and our last time was odd, add one
            if ((this is Player) && ((this.lastActionTime % 2) == 1)) this.lastActionTime++;

            //if we are an enemy and our last time was even, subtract one
            if ((this is Enemy) && ((this.lastActionTime % 2) == 0)) this.lastActionTime--;

            return this.lastActionTime;
        }

        public Piece GetTarget()
        {
            //Game.Log(Game.LogLevel.TRACE, "% Piece.GetTarget %");
            if (this.target == null)
                this.target = this;
            return this.target;
        }

        public void SetTarget(Piece target)
        {
            this.target = target;
        }

        public int GetBaseLevel()
        {
            return this.level;
        }

        public int GetLevel(int time)
        {
            //Game.Log(Game.LogLevel.TRACE, "% Piece.GetLevel %");
            int level = this.level;

            foreach (GlamourModifier glamour in this.glamours)
            {
                if (glamour.GetAffect() == Glamour.Affect.LEVEL)
                {
                    level += glamour.GetCurrentPotency(time);
                }
            }
            if (level < 1) level = 1;
            return level;
        }

        public int GetDamageModifier( int time )
        {
            //Game.Log(Game.LogLevel.TRACE, "% Piece.GetDamageModifier %");
            int damage = 0;
            foreach (GlamourModifier glamour in this.glamours)
            {
                if (glamour.GetAffect() == Glamour.Affect.MAX_DAMAGE)
                {
                    //add in the multiple
                    damage += glamour.GetCurrentPotency(time) * Glamour.MAX_MULTIPLE;
                }
            }//foreach
            return damage;
        }

        public void ApplyGlamour( GlamourModifier modifier)
        {
            //add the modifier to the list
            glamours.Add(modifier);
        }

        public void ResolveGlamours(int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.ResolveGlamours %");
            //this modifies the hp/mp points and checks for death
            foreach (GlamourModifier glamour in this.glamours)
            {
                //hit points
                if (glamour.GetAffect() == Glamour.Affect.HIT_POINTS)
                {
                    this.hp += glamour.IntegrateGlamour(time);
                }

                //magic points
                if (glamour.GetAffect() == Glamour.Affect.MAGIC_POINTS)
                {
                    this.mp += glamour.IntegrateGlamour(time);
                }
            }

            //make sure we did not get too big or die
            this.CoercePoints(time);
            this.CheckForDeath();
        }

        public void RemoveGlamours()
        {
            this.glamours.Clear();
        }

        public int CureGlamours( Cure.Effect effect, int num, int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.CureGlamours %");
            int all_enmity = 0;
            int cured = 0;
            int enmity = 0;
            for (int i = this.glamours.Count - 1; i >= 0; i--)
            {
                //if its not active or not harmful, skip it
                if ( (glamours[i].GetCurrentPotency(time) == 0) ||
                      !glamours[i].IsHarmful() ) continue;

                // if it matches, remove it
                enmity = glamours[i].IsCured(effect);
                if(enmity > 0)
                {
                    glamours.RemoveAt(i);
                    all_enmity += enmity;
                    cured++;
                }

                //check if we have cured all we are allowed
                if (cured >= num) break;
            }//for

            return all_enmity;
        }

        public int GetRangeModifier(int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.GetRangeModifier %");
            int modifier = 0;
            int potency = 0;

            //this modifies the range
            foreach (GlamourModifier glamour in this.glamours)
            {
                if (glamour.GetAffect() == Glamour.Affect.RANGE)
                {
                    potency = glamour.GetCurrentPotency(time);
                    if ((potency > Glamour.MAX_RANGE_MOD) || (potency <Glamour.MIN_RANGE_MOD)) potency = 0;
                    modifier += potency;
                }
            }//foreach
            return modifier;
        }

        public int GetRangeCorrection(int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.GetRangeCorrection %");
            int modifier = 0;
            int potency = 0;

            //this modifies the range
            foreach (GlamourModifier glamour in this.glamours)
            {
                if (glamour.GetAffect() == Glamour.Affect.RANGE)
                {
                    //use the range shifted 10 up to store information about range correction
                    potency = glamour.GetCurrentPotency(time) - Glamour.CORRECTION_OFFSET;
                    if ((potency > Glamour.MAX_CORRECTION) || (potency < Glamour.MIN_CORRECTION)) potency = 0;
                    modifier += potency;
                }
            }//foreach
            return modifier;
        }

        public int GetPositionModifier(int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.GetPositionModifier %");
            int modifier = 0;
            int potency = 0;

            //this modifies the range
            foreach (GlamourModifier glamour in this.glamours)
            {
                if (glamour.GetAffect() == Glamour.Affect.POSITION)
                {
                    potency = glamour.GetCurrentPotency(time);
                    if ((potency > Glamour.MAX_POSITION_MOD) || (potency < Glamour.MIN_POSITION_MOD)) potency = 0;
                    modifier += potency;
                }
            }//foreach
            return modifier;
        }

        public int GetPositionCorrection(int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.GetPositionCorrection %");
            int modifier = 0;
            int potency = 0;

            //this modifies the range
            foreach (GlamourModifier glamour in this.glamours)
            {
                if (glamour.GetAffect() == Glamour.Affect.POSITION)
                {
                    //use the position shifted 10 up to store information about range correction
                    potency = glamour.GetCurrentPotency(time) - Glamour.CORRECTION_OFFSET;
                    if ((potency > Glamour.MAX_CORRECTION) || (potency < Glamour.MIN_CORRECTION)) potency = 0;
                    modifier += potency;
                }
            }//foreach
            return modifier;
        }

        public void CheckForDeath()
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.CheckForDeath %");
            if (hp < 0)
            {
                if( this.state != Piece.State.DEAD )
                {
                    Game.Log(Game.LogLevel.DEBUG, this.ToString() + " has died.");
                }
                this.state = Piece.State.DEAD;
            }
        }

        public void CoercePoints( int time )
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.CoercePoints %");
            //hp
            int max_hp = this.GetMaxHp(time);

            if (this.hp > max_hp)
            {
                this.hp = this.max_hp;
            }

            //mp
            int max_mp = this.GetMaxMp(time);

            if ( this.mp > max_mp)
            {
                this.mp = max_mp;
            }

            Game.Log(Game.LogLevel.DEBUG, this.ToString() + " health: HP(" + this.hp + "/" + max_hp + ")  MP(" + this.mp + "/" + max_mp + ")");

        }

        public int GetHp()
        {
            //update our hp
            return this.hp;
        }

        public void ChangeHp(int points, int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.ChangeHp %");
            //update our hp
            this.hp += points;
            Game.Log(Game.LogLevel.MINIMAL, this.ToString() + " has " + ((points > 0) ? "gained ":"lost ") + points + " Health Points.");


            //make sure we did not get too big or die
            this.CoercePoints(time);
            this.CheckForDeath();
        }

        public int GetMaxHp(int time)
        {
            //Game.Log(Game.LogLevel.TRACE, "% Piece.GetMaxHp %");
            int max_hp = this.max_hp;
            foreach (GlamourModifier glamour in this.glamours)
            {
                if (glamour.GetAffect() == Glamour.Affect.MAX_HIT_POINTS)
                {
                    //use a multiplyer to adjust the ammount
                    max_hp += glamour.GetCurrentPotency(time) * Glamour.MAX_MULTIPLE;
                }
            }//foreach
            if (max_hp < 1) max_hp = 1;
            return max_hp;
        }

        public int GetMp()
        {
            //update our hp
            return this.mp;
        }
        
        public void ChangeMp(int points, int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Piece.ChangeMp %");
            //update our hp
            this.mp += points;
            Game.Log(Game.LogLevel.NORMAL, this.ToString() + " has " + ((points > 0) ? "gained " : "lost ") + points + " Magic Points.");

            //make sure we did not get too big
            this.CoercePoints(time);
        }

        public int GetMaxMp(int time)
        {
            //Game.Log(Game.LogLevel.TRACE, "% Piece.GetMaxMp %");
            int max_mp = this.max_mp;
            foreach (GlamourModifier glamour in this.glamours)
            {
                if (glamour.GetAffect() == Glamour.Affect.MAX_MAGIC_POINTS)
                {
                    //use a multiplyer to adjust the ammount
                    max_mp += glamour.GetCurrentPotency(time) * Glamour.MAX_MULTIPLE;
                }
            }//foreach
            if (max_mp < 1) max_mp = 1;
            return max_mp;
        }

        public string GetState()
        {
            return state_string[(int)this.state];
        }

    }
}
