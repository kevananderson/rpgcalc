using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardExplorer;
namespace RPGC
{
    public class Player : Piece
    {
        protected Character charcard = null;
        protected Role rolecard = null;
        protected Equipment armorcard = null;
        protected Equipment weaponcard = null;
        protected Equipment footcard = null;
        protected Equipment accessorycard = null;
        protected int number; //player 1,2,3 etc
        protected int experience = 0;
        protected static int[] equipTime = new int[] { 5, 60, 30, 10 };
        protected bool[] equipAction = new bool[] { false, false, false, false };

        /*** constructor ***/

        public Player(int number) : base()
        {
            Game.Log(Game.LogLevel.TRACE, "% Player Constructor %");
            this.number = number;
        }

        /*** public ***/

        public override bool ApplyCard( Card card )
        {
            Game.Log(Game.LogLevel.TRACE, "% Player.ApplyCard %");
            //you can change equipment if you are already initialized
            if ( card is Equipment )
            {
                Equipment equip = (Equipment)card;

                switch (equip.GetEquipType())
                {
                    case Equipment.Type.ARMOR:
                        this.armorcard = (Equipment)card;
                        this.equipAction[(int)Equipment.Type.ARMOR] = true;
                        Game.Log(Game.LogLevel.MINIMAL, this.ToString() + " equips armor: " + this.armorcard);
                        return true;
                    case Equipment.Type.WEAPON:
                        this.weaponcard = (Equipment)card;
                        this.equipAction[(int)Equipment.Type.WEAPON] = true;
                        Game.Log(Game.LogLevel.MINIMAL, this.ToString() + " equips weapon: " + this.weaponcard);
                        return true;
                    case Equipment.Type.FOOTWARE:
                        this.footcard = (Equipment)card;
                        this.equipAction[(int)Equipment.Type.FOOTWARE] = true;
                        Game.Log(Game.LogLevel.MINIMAL, this.ToString() + " equips footwear: " + this.footcard);
                        return true;
                    case Equipment.Type.ACCESSORY:
                        this.accessorycard = (Equipment)card;
                        this.equipAction[(int)Equipment.Type.ACCESSORY] = true;
                        Game.Log(Game.LogLevel.MINIMAL, this.ToString() + " equips accessory: " + this.accessorycard);
                        return true;
                }//switch
            }//if equipment


            //you cannot change character or role after init 
            if( this.IsInitialized() )
            {
                return false;
            }

            //we still need to set our character or role
            if ( card is Character )
            {
                this.charcard = (Character)card;

                //initialize our level from the character at the start
                this.level = charcard.GetLevel();
                this.experience = 0;
                this.InitializeStats();
                if (rolecard != null) this.state = Piece.State.WAITING;
                Game.Log(Game.LogLevel.DEBUG, this.ToString() + " chooses character: " + this.charcard);
                return true;
            }

            if ( card is Role )
            {
                this.rolecard = (Role)card;
                this.InitializeStats();
                if (charcard != null) this.state = Piece.State.WAITING;
                Game.Log(Game.LogLevel.DEBUG, this.ToString() + " chooses role: " + this.rolecard);
                return true;
            }

            return false;
        }

        public override int GetBaseStat(Card.Stat stat)
        {
            //Game.Log(Game.LogLevel.TRACE, "% Player.GetBaseStat %");
            int value = 0;
            Matrix stats = Matrix.ZeroMatrix(8, 1);

            //sum the charcard and rolecard together
            if (this.charcard != null)
            {
                stats = charcard.GetStats();
                value += (int)stats[(int)stat, 0];
            }

            if( this.rolecard != null)
            {
                stats = rolecard.GetStats();
                value += (int)stats[(int)stat, 0];
            }

            //return the base stat
            return value;
        }

        public override int GetStat(Card.Stat stat, int time)
        {
            //Game.Log(Game.LogLevel.TRACE, "% Player.GetStat %");
            Matrix stats;
            int value = this.GetBaseStat(stat);

            //add in the effects of the equipment, which can be level dependent
            Equipment[] equipcards = { armorcard, weaponcard, footcard, accessorycard };
            foreach (Equipment equip in equipcards)
            {
                if (equip == null) continue;
                stats = equip.GetStats();
                int equipstat = (int)stats[(int)stat, 0];

                //if the stat is 0 or negative it is not affected by level
                if (equipstat <= 0)
                {
                    value += equipstat;
                    continue;
                }

                //equip level matters
                int equiplevel = equip.GetLevel();

                //find out how our level modifies the stat
                switch (this.level - equiplevel)
                {
                    case -1:
                        equipstat--;
                        break;
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        break;
                    case 5:
                        equipstat--;
                        break;
                    case 6:
                        equipstat -= 2;
                        break;
                    case 7:
                        equipstat -= 3;
                        break;
                    default:
                        equipstat = 0;
                        break;
                }//switch

                //equipstat, once positive, cannot go negative
                if (equipstat < 0) equipstat = 0;

                //add it to the value
                value += equipstat;

            }//foreach

            //look through the glamours for more modifiers
            foreach (GlamourModifier modifier in this.glamours)
            {
                //see if the glamour affectes this stat
                if ((modifier.GetAffect() < Glamour.Affect.HIT_POINTS) && (Card.Stat)modifier.GetAffect() == stat)
                {
                    value += modifier.GetCurrentPotency(time);
                }
            }

            //the value cannot be less than one
            if (value < 1) value = 1;

            //return the requested stat
            return value;
        }

        public override int GetTimeDelay(int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Player.GetTimeDelay %");
            int runTime = base.GetTimeDelay(time);
            double equipTime = 0;
            for( int i = 0; i < equipAction.Length; i++)
            {
                if(this.equipAction[i])
                {
                    equipTime += Player.equipTime[i];
                }
            }
            double modifier = Battle.GetSpeedModifier(this, time);

            return runTime + ((int)(equipTime * modifier));
        }

        public override void SetTookAction(bool always = false)
        {
            Game.Log(Game.LogLevel.TRACE, "% Player.SetTookAction %");
            base.SetTookAction(always);
            for( int i = 0; i < equipAction.Length; i++ )
            {
                equipAction[i] = false;
            }
        }

        public override string ToString()
        {
            return "Player" + this.number;
        }

        public bool CheckForNewLevel()
        {
            Game.Log(Game.LogLevel.TRACE, "% Player.CheckForNewLevel %");
            bool leveled = false;

            //see if we have enough experience to level
            int nextLevel = 1000;
            if( this.experience >= nextLevel )
            {
                //we level
                this.level++;
                this.experience -= nextLevel;
                leveled = true;
            }

            //set the mp and hp to full
            this.InitializeStats();
            return leveled;
        }

        public void AddExperience(Piece enemy)
        {
            Game.Log(Game.LogLevel.TRACE, "% Player.AddExperience %");
            int levelGap = enemy.GetBaseLevel() - this.level;
            double experienceGain = 100 * Math.Pow(1.8, levelGap);
            this.experience += (int)experienceGain;
        }

        public int GetExperience()
        {
            return this.experience;
        }

        public int GetNumber()
        {
            return this.number;
        }
    }
}
