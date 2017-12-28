using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardExplorer;

namespace RPGC
{
    public class Battle
    {
        public enum Side { PLAYERS , ENEMY, VICTORY, DEFEAT, UNKNOWN };
        public const int MAX_LUCK = 31;
        public const int CRITICAL_LUCK = 301;
        public const double POW = 1.5;
        public const int MAX_LEVEL = 30;
        public const int AVG_STAT = 15;

        protected static string[] side_text = new string[] { "Players", "Enemy", "Victory", "Defeat", "Unknown" };

        /*** public ***/

        public static Battle.Side Opposite( Battle.Side side )
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.Opposite %");
            switch (side)
            {
                case Battle.Side.PLAYERS:
                    return Battle.Side.ENEMY;
                case Battle.Side.ENEMY:
                    return Battle.Side.PLAYERS;
                default:
                    return Battle.Side.PLAYERS;
            }
        }

        public static Battle.Side FirstStrike( Enemy enemy )
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.FirstStrike %");
            //the enemy can strike first if it is above average in focus and speed.
            if ( (enemy.GetStat(Card.Stat.FOCUS,0) > Creature.BASE_ABILITY) &&
                (enemy.GetStat(Card.Stat.SPEED,0) > Creature.BASE_ABILITY) )
            {
                if( IsLucky( enemy.GetStat(Card.Stat.LUCK,0) ) )
                {
                    //todo put this back to enemy
                    //return Battle.Side.ENEMY;
                    return Battle.Side.PLAYERS;
                }
            }
            return Battle.Side.PLAYERS;
        }

        public static string GetSideText( Battle.Side side)
        {
            return side_text[(int)side];
        }

        public static bool IsLucky(int luck)
        {
            //Game.Log(Game.LogLevel.TRACE, "% Battle.IsLucky %");
            //This only looks at single player luck.
            Random random = new Random();
            if (luck > random.Next(MAX_LUCK))
                return true;
            else
                return false;
        }

        public static bool CriticalLucky(int luck)
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.CriticalLicky %");
            //This only looks at single player luck.
            Random random = new Random();
            if (luck > random.Next(CRITICAL_LUCK))
                return true;
            else
                return false;
        }

        public static int GetSkillTime(Skill skill, Piece actor, int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.GetSkillTime %");
            //this must return an even time
            int seconds = 0;

            //add in time it took to change weapons and move on the board
            seconds += actor.GetTimeDelay(time); //this is delay from equip and movement

            //we need to find the base speed of the skill
            double baseSpeed = Battle.BaseSkillSpeed(skill.GetSpeed(), skill.GetSkillType() );

            //this multiplies by the base to get the actual time taken
            double actorSpeed = Battle.SkillSpeedModifier(skill.GetSkillType(), actor, time);

            //combine the base with the modifier
            seconds += (int)(baseSpeed * actorSpeed);

            //ensure even
            if (seconds % 2 == 1) seconds++;

            return seconds;
        }

        public static int GetGlamourTime(Glamour glamour, Piece actor, Piece target, int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.GetGlamourTime %");
            //this must return an even time
            int seconds = 0;

            //add in time it took to change weapons and move on the board
            seconds += actor.GetTimeDelay(time); //this is delay from equip and movement

            //we need to find the base speed of the skill
            double baseSpeed = Math.Abs(glamour.GetPotency()) * glamour.getDuration() / 10;

            //this multiplies by the base to get the actual time taken
            double actorSpeed = Battle.GlamourSpeedModifier(glamour.GetAffect(), actor, target, time);

            //combine the base with the modifier
            seconds += (int)(baseSpeed * actorSpeed);

            //ensure even
            if (seconds % 2 == 1) seconds++;

            return seconds;
        }

        public static int AddLuck(int chances, int luck)
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.AddLuck %");
            int increase = -chances / 3;
            for (int i = 0; i < chances; i++)
            {
                if (Battle.IsLucky(luck)) increase++;
            }
            return increase;
        }

        public static int AddCures(int chances, int luck)
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.AddCures %");
            int increase = 0;
            for (int i = 0; i < chances; i++)
            {
                if (Battle.IsLucky(luck)) increase++;
            }
            return increase;
        }

        public static double GetSpeedModifier(Piece piece, int time)
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.SetSpeedModifier %");
            //calculate the speed modifier
            double speed = piece.GetStat(Card.Stat.SPEED, time);
            double modifier = speed * Battle.Level(piece.GetLevel(time));
            modifier = Battle.Scale( 5/6, modifier);
            return 1 - modifier;
        }

        public static double DamageScore(int level, int raw_luck, int stats)
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.DamageScore %");
            double luck = Battle.AddLuck(raw_luck, raw_luck);
            double damage_score = stats + luck;
            if (damage_score < 1) damage_score = 1;
            if (level < 1) level = 1;
            return damage_score * Battle.Level(level);
        }

        public static double HealScore(int level, int raw_luck, int stats)
        {
            Game.Log(Game.LogLevel.TRACE, "% Battle.HealScore %");
            double luck = Battle.AddLuck(3, raw_luck);
            double heal_score = stats + luck;
            if (heal_score < 1) heal_score = 1;
            if (level < 1) level = 1;
            return heal_score * Battle.Level(level);
        }

        public static double Level( double level )
        {
            return Math.Pow(Battle.POW, level);
        }

        public static double Scale( double fraction , double value)
        {
            double max = Battle.AVG_STAT * Math.Pow(Battle.POW, Battle.MAX_LEVEL);
            double max_fraction = fraction * max;
            if (value > max_fraction) value = max_fraction;
            return value / max;
        }

        /*** protected ***/

        protected static double BaseSkillSpeed( Skill.Speed speed, Skill.Type type )
        {
            double baseSpeed = 120;

            //is it a slow skill
            if (speed == Skill.Speed.SLOW)
            {
                switch (type)
                {
                    case Skill.Type.EDGE:
                        //sword or long sword
                        baseSpeed = 25;
                        break;
                    case Skill.Type.BLUNT:
                        //hammer or mace
                        baseSpeed = 20;
                        break;
                    case Skill.Type.PIERCING:
                        //halbred or long spear
                        baseSpeed = 30;
                        break;
                    case Skill.Type.RANGED:
                        //cross bow or musket
                        baseSpeed = 60;
                        break;
                    case Skill.Type.SPELL:
                        //major incantation
                        baseSpeed = 80;
                        break;
                    case Skill.Type.HEAL:
                        //major healing
                        baseSpeed = 50;
                        break;
                    case Skill.Type.CURE:
                        //complete cure
                        baseSpeed = 30;
                        break;
                    case Skill.Type.RECRUIT:
                        //resurection in battle
                        baseSpeed = 90;
                        break;
                }//switch slow
            }//if slow
            else //fast
            {
                switch (type)
                {
                    case Skill.Type.EDGE:
                        //knife or short sword
                        baseSpeed = 15;
                        break;
                    case Skill.Type.BLUNT:
                        //bow (stick) or gauntlet or kick
                        baseSpeed = 10;
                        break;
                    case Skill.Type.PIERCING:
                        //sword or short spear
                        baseSpeed = 20;
                        break;
                    case Skill.Type.RANGED:
                        //bow or pistol
                        baseSpeed = 25;
                        break;
                    case Skill.Type.SPELL:
                        //minor incantation
                        baseSpeed = 35;
                        break;
                    case Skill.Type.HEAL:
                        //minor healing
                        baseSpeed = 30;
                        break;
                    case Skill.Type.CURE:
                        //partial or single cure
                        baseSpeed = 15;
                        break;
                    case Skill.Type.RECRUIT:
                        //call pet or summon pet
                        baseSpeed = 30;
                        break;
                }//switch fase
            }//else fast

            return baseSpeed;
        }

        protected static double SkillSpeedModifier(Skill.Type type, Piece actor, int time)
        {
            double stats = 0;

            //pull out the needed stats
            double strength = actor.GetStat(Card.Stat.STRENGTH, time);
            double speed = actor.GetStat(Card.Stat.SPEED, time);
            double balance = actor.GetStat(Card.Stat.BALANCE, time);
            double faith = actor.GetStat(Card.Stat.FAITH, time);
            double focus = actor.GetStat(Card.Stat.FOCUS, time);
            double allure = actor.GetStat(Card.Stat.ALLURE, time);
            int luck = actor.GetStat(Card.Stat.LUCK, time);

            switch (type)
            {
                case Skill.Type.EDGE:
                    stats += 1 * speed + .2 * balance + .2 * focus + .1 * strength;
                    //future modify this by HP percent and grit
                    //add some luck
                    stats += Battle.AddLuck(2, luck);
                    break;
                case Skill.Type.BLUNT:
                    stats += 1 * speed + .25 * balance + .25 * focus;
                    //future modify this by HP percent and grit
                    //add some luck
                    stats += Battle.AddLuck(1, luck);
                    break;
                case Skill.Type.PIERCING:
                    stats += 1 * speed + .2 * balance + .3 * strength;
                    //future modify this by HP percent and grit
                    //add some luck
                    stats += Battle.AddLuck(3, luck);
                    break;
                case Skill.Type.RANGED:
                    stats += 1 * speed + .4 * focus + .1 * strength;
                    //future modify this by HP percent and grit
                    //add some luck
                    stats += Battle.AddLuck(2, luck);
                    break;
                case Skill.Type.SPELL:
                    stats += .75 * speed + .5 * focus + .25 * balance;
                    //future modify this by HP percent and focus
                    //add some luck
                    stats += Battle.AddLuck(5, luck);
                    break;
                case Skill.Type.HEAL:
                    stats += .75 * speed + .5 * faith + .25 * focus;
                    //future modify this by HP percent and faith
                    //add some luck
                    stats += Battle.AddLuck(7, luck);
                    break;
                case Skill.Type.CURE:
                    stats += 1 * speed + .25 * faith + .25 * focus;
                    //future modify this by HP percent and focus
                    //add some luck
                    stats += Battle.AddLuck(4, luck);
                    break;
                case Skill.Type.RECRUIT:
                    stats += .75 * allure + .5 * faith + .25 * speed;
                    //future modify this by HP percent and focus
                    //add some luck
                    stats += Battle.AddLuck(9, luck);
                    break;
            }//switch skill

            double power = stats * Battle.Level(actor.GetLevel(time) );
            power = Battle.Scale(2/3,power);
            return 1 - power;
        }

        protected static double GlamourSpeedModifier(Glamour.Affect affect, Piece actor, Piece target, int time)
        {
            double stats = 0;
            bool friendly = (actor.GetType() == target.GetType());

            double strength = actor.GetStat(Card.Stat.STRENGTH, time);
            double speed = actor.GetStat(Card.Stat.SPEED, time);
            double balance = actor.GetStat(Card.Stat.BALANCE, time);
            double faith = actor.GetStat(Card.Stat.FAITH, time);
            double focus = actor.GetStat(Card.Stat.FOCUS, time);
            //double allure = actor.GetStat(Card.Stat.ALLURE, time);
            int luck = actor.GetStat(Card.Stat.LUCK, time);

            double t_strength = target.GetStat(Card.Stat.STRENGTH, time);
            double t_speed = target.GetStat(Card.Stat.SPEED, time);
            double t_focus = target.GetStat(Card.Stat.FOCUS, time);
            double t_faith = target.GetStat(Card.Stat.FAITH, time);
            double t_allure = target.GetStat(Card.Stat.ALLURE, time);
            int t_luck = target.GetStat(Card.Stat.LUCK, time);

            switch (affect)
            {
                case Glamour.Affect.STRENGTH:
                case Glamour.Affect.GRIT:
                case Glamour.Affect.SPEED:
                case Glamour.Affect.BALANCE:
                case Glamour.Affect.FAITH:
                case Glamour.Affect.FOCUS:
                case Glamour.Affect.LUCK:
                case Glamour.Affect.ALLURE:
                    if( friendly )
                    {
                        //target allure modifies
                        stats += .5 * speed + .5 * focus + .5 * t_allure;
                    }
                    else
                    {
                        //target faith resists
                        stats += .5 * speed + .5 * focus + .5 * balance - .5 * t_faith;
                    }
                    //future modify this by HP percent and focus
                    //add some luck and lose some
                    stats += Battle.AddLuck(3, luck);
                    if( friendly)
                    {
                        stats += Battle.AddLuck(1, t_luck);
                    }
                    else
                    {
                        stats -= Battle.AddLuck(2, t_luck);
                    }
                    break;
                case Glamour.Affect.HIT_POINTS:
                case Glamour.Affect.MAX_HIT_POINTS:
                    if (friendly)
                    {
                        //target faith modifies
                        stats += .5 * speed + .5 * focus + .5 * t_faith;
                    }
                    else
                    {
                        //target strength resists
                        stats += .5 * speed + .5 * focus + .5 * balance - .5 * t_strength;
                    }
                    //future modify this by HP percent and focus
                    //add some luck and lose some
                    stats += Battle.AddLuck(3, luck);
                    if (friendly)
                    {
                        stats += Battle.AddLuck(1, t_luck);
                    }
                    else
                    {
                        stats -= Battle.AddLuck(2, t_luck);
                    }
                    break;
                case Glamour.Affect.MAGIC_POINTS:
                case Glamour.Affect.MAX_MAGIC_POINTS:
                    if (friendly)
                    {
                        //target focus modifies
                        stats += .5 * speed + .5 * focus + .5 * t_focus;
                    }
                    else
                    {
                        //target focus resists
                        stats += .5 * speed + .5 * focus + .5 * faith - .5 * t_focus;
                    }
                    //future modify this by HP percent and focus
                    //add some luck and lose some
                    stats += Battle.AddLuck(3, luck);
                    if (friendly)
                    {
                        stats += Battle.AddLuck(1, t_luck);
                    }
                    else
                    {
                        stats -= Battle.AddLuck(2, t_luck);
                    }
                    break;

                case Glamour.Affect.RANGE:
                case Glamour.Affect.POSITION:
                case Glamour.Affect.MAX_DAMAGE:
                    if (friendly)
                    {
                        //target speed modifies
                        stats += .5 * speed + .5 * focus + .5 * t_speed;
                    }
                    else
                    {
                        //target speed resists
                        stats += .5 * speed + .5 * focus + .5 * balance - .5 * t_speed;
                    }
                    //future modify this by HP percent and focus
                    //add some luck and lose some
                    stats += Battle.AddLuck(3, luck);
                    if (friendly)
                    {
                        stats += Battle.AddLuck(1, t_luck);
                    }
                    else
                    {
                        stats -= Battle.AddLuck(5, t_luck);
                    }
                    break;
                case Glamour.Affect.LEVEL:
                    if (friendly)
                    {
                        stats += 1 * focus;
                    }
                    else
                    {
                        //target speed resists
                        stats += .5 * speed + .5 * focus + .5 * balance - .5 * t_speed;
                    }
                    //future modify this by HP percent and focus
                    //add some luck and lose some
                    if (friendly)
                    {
                        stats += Battle.AddLuck(12, luck);
                        stats += Battle.AddLuck(12, t_luck);
                    }
                    else
                    {
                        stats -= Battle.AddLuck(5, t_luck);
                    }
                    break;
            }//switch

            if (stats < 0) stats = 0;
            double power = stats * Battle.Level(actor.GetLevel(time));
            power = Battle.Scale(2 / 3, power);
            return 1 - power;
        }
    }
}
