using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardExplorer;

namespace RPGC
{
    class SkillEffect : CardEffect
    {
        protected Skill skill;

        /*** constructor ***/

        public SkillEffect(Skill skill, Piece actor, Piece target, Enemy enemy) : base(actor, target, enemy)
        {
            Game.Log(Game.LogLevel.TRACE, "% SkilEffect Constructor %");
            this.skill = skill;

            //set the time
            int time = actor.GetLastActionTime();
            this.time = time + Battle.GetSkillTime(skill, actor, time);

            //check if we have the MP to cast this
            this.fizzle = ( (this.skill.GetSkillType() >= Skill.Type.SPELL) && 
                            (this.actor.GetMp() < this.CalculateCost()) );

            //spend the MP as part of creating this.
            if ( (this.skill.GetSkillType() >= Skill.Type.SPELL) && (!this.fizzle) )
            {
                actor.ChangeMp(-this.CalculateCost(), time);
            }

        }

        /*** public ***/

        public override int ResolveEffect()
        {
            Game.Log(Game.LogLevel.TRACE, "% SkilEffect.ResolveEffect %");
            //check if the actor is dead
            if (actor.IsDead()) return this.time;

            //set the time of the last action in the piece
            actor.SetLastActionTime(this.time);

            //the actor is ready for another action
            actor.SetResolvedAction();

            //decide how the skill affects the target
            Enmity enmity = null;
            switch (this.skill.GetSkillType())
            {
                case Skill.Type.EDGE:
                    enmity = this.EdgeAttack();
                    break;
                case Skill.Type.BLUNT:
                    enmity = this.BluntAttack();
                    break;
                case Skill.Type.PIERCING:
                    enmity = this.PiercingAttack();
                    break;
                case Skill.Type.RANGED:
                    enmity = this.RangedAttack();
                    break;
                case Skill.Type.SPELL:
                    enmity = this.SpellAttack();
                    break;
                case Skill.Type.HEAL:
                    enmity = this.HealAssist();
                    break;
                case Skill.Type.CURE:
                    enmity = this.CureAssist();
                    break;
                case Skill.Type.RECRUIT:
                    //future add ability to recruit
                    enmity = null;
                    break;
                default:
                    break;
            }//switch skill type

            //find out if we need to update the enemy enmity

            //this is a physical attack against the enemy
            bool attack = ((skill.GetSkillType() < Skill.Type.HEAL) && (this.target is Enemy) && (this.actor is Player));

            //or this is a player healing another player
            attack |= ((skill.GetSkillType() == Skill.Type.HEAL) && (this.target is Player) && (this.actor is Player) );

            //this was an attack
            if (attack)
            {
                //add some rightious hate
                this.enemy.AddEnmity(enmity, (Player)this.actor);
            }

            return this.time;
        }

        public override string ToString()
        {
            return this.actor.ToString() + " targets " + this.target.ToString() + " at " + this.time + " with " + this.skill.ToString();
        }

        /*** protected ***/

        protected Enmity EdgeAttack()
        {
            Game.Log(Game.LogLevel.TRACE, "% SkilEffect.EdgeAttack %");
            //we cannot attack a friend
            if( ( (this.actor is Player) && (this.target is Player) ) || (this.actor == this.target) ) return null;

            //calculate our max damage possible
            double maxDamage = this.skill.GetMaxDamage() + this.actor.GetDamageModifier(this.time);

            //use the range error to modify the max damage
            maxDamage = this.CalculateRangedMax(maxDamage, 0.5 );

            //use the positional error to modify the max damage
            maxDamage = this.CalculatePositionalMax(maxDamage, 0.6, 0.8 );

            //future attack with an area attack - for now we ignore that bit in this skill

            //calculate how much damage we do to our target
            double actor_score = Battle.DamageScore(this.actor.GetLevel(time), 
                                                    this.actor.GetStat(Card.Stat.LUCK, this.time),
                                                    2 * this.actor.GetStat(Card.Stat.STRENGTH, this.time) +
                                                    this.actor.GetStat(Card.Stat.SPEED, this.time) );

            double target_score = Battle.DamageScore(this.target.GetLevel(time),
                                                     this.target.GetStat(Card.Stat.LUCK, this.time),
                                                     this.target.GetStat(Card.Stat.GRIT, this.time) +
                                                     this.target.GetStat(Card.Stat.BALANCE, this.time));

            double damage = maxDamage * actor_score / (actor_score + target_score);

            //check for miss or critical hit
            if (Battle.CriticalLucky(2 * this.target.GetStat(Card.Stat.LUCK, this.time)))
            {
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " missed " + this.target.ToString() + " with "+ this.skill.GetSkillName() + ".");
                return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)damage, 30);
            }
            else if (Battle.CriticalLucky(this.actor.GetStat(Card.Stat.LUCK, this.time)))
            {
                damage = maxDamage;
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " scored a lucky hit on " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
            }

            //inflict the damage
            target.ChangeHp((int)-damage, this.time);
            Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " hit " + this.target.ToString() + " with " + this.skill.GetSkillName() + " for " + damage + " damage.");

            //create the enmity this generates
            return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)damage, 90);
        }

        protected Enmity BluntAttack()
        {
            Game.Log(Game.LogLevel.TRACE, "% SkilEffect.BluntAttack %");
            //we cannot attack a friend
            if (((this.actor is Player) && (this.target is Player)) || (this.actor == this.target)) return null;

            //calculate our max damage possible
            double maxDamage = this.skill.GetMaxDamage() + this.actor.GetDamageModifier(this.time);

            //use the range error to modify the max damage
            maxDamage = this.CalculateRangedMax(maxDamage, 0.75);

            //use the positional error to modify the max damage
            maxDamage = this.CalculatePositionalMax(maxDamage, 0.9, 0.8 );

            //future attack with an area attack - for now we ignore that bit in this skill

            //calculate how much damage we do to our target
            double actor_score = Battle.DamageScore(this.actor.GetLevel(time),
                                                    this.actor.GetStat(Card.Stat.LUCK, this.time),
                                                    this.actor.GetStat(Card.Stat.STRENGTH, this.time) +
                                                    this.actor.GetStat(Card.Stat.SPEED, this.time) +
                                                    this.actor.GetStat(Card.Stat.BALANCE, this.time));

            double target_score = Battle.DamageScore(this.target.GetLevel(time),
                                                     this.target.GetStat(Card.Stat.LUCK, this.time),
                                                     this.target.GetStat(Card.Stat.GRIT, this.time) +
                                                     this.target.GetStat(Card.Stat.BALANCE, this.time));

            double damage = maxDamage * actor_score / (actor_score + target_score);

            //check for miss or critical hit
            if (Battle.CriticalLucky(2 * this.target.GetStat(Card.Stat.LUCK, this.time)))
            {
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " missed " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
                return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)damage, 20);
            }
            else if (Battle.CriticalLucky(this.actor.GetStat(Card.Stat.LUCK, this.time)))
            {
                damage = maxDamage;
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " scored a lucky hit on " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
            }

            //inflict the damage
            target.ChangeHp((int)-damage, this.time);
            Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " hit " + this.target.ToString() + " with " + this.skill.GetSkillName() + " for " + damage + " damage.");

            //create the enmity this generates
            return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)damage, 120);
        }

        protected Enmity PiercingAttack()
        {
            Game.Log(Game.LogLevel.TRACE, "% SkilEffect.Piercing %");
            //we cannot attack a friend
            if (((this.actor is Player) && (this.target is Player)) || (this.actor == this.target)) return null;

            //calculate our max damage possible
            double maxDamage = this.skill.GetMaxDamage() + this.actor.GetDamageModifier(this.time);

            //use the range error to modify the max damage
            maxDamage = this.CalculateRangedMax(maxDamage, 0.25);

            //use the positional error to modify the max damage
            maxDamage = this.CalculatePositionalMax(maxDamage, 0.7, 0.9);

            //future attack with an area attack - for now we ignore that bit in this skill

            //calculate how much damage we do to our target
            int actor_stats = 0;
            if ( this.skill.GetSpeed() == Skill.Speed.FAST )
            {
                actor_stats = 2 * this.actor.GetStat(Card.Stat.SPEED, this.time) +
                              this.actor.GetStat(Card.Stat.BALANCE, this.time);
            }
            else
            if (this.skill.GetSpeed() == Skill.Speed.FAST)
            {
                actor_stats = 2 * this.actor.GetStat(Card.Stat.STRENGTH, this.time) +
                              this.actor.GetStat(Card.Stat.BALANCE, this.time);
            }

            double actor_score = Battle.DamageScore(this.actor.GetLevel(time),
                                                    this.actor.GetStat(Card.Stat.LUCK, this.time),
                                                    actor_stats);

            double target_score = Battle.DamageScore(this.target.GetLevel(time),
                                                     this.target.GetStat(Card.Stat.LUCK, this.time),
                                                     this.target.GetStat(Card.Stat.GRIT, this.time) +
                                                     this.target.GetStat(Card.Stat.BALANCE, this.time));

            double damage = maxDamage * actor_score / (actor_score + target_score);

            //check for miss or critical hit
            if (Battle.CriticalLucky(2 * this.target.GetStat(Card.Stat.LUCK, this.time)))
            {
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " missed " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
                return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)damage, 20);
            }
            else if (Battle.CriticalLucky(this.actor.GetStat(Card.Stat.LUCK, this.time)))
            {
                damage = maxDamage;
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " scored a lucky hit on " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
            }

            //inflict the damage
            target.ChangeHp((int)-damage, this.time);
            Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " hit " + this.target.ToString() + " with " + this.skill.GetSkillName() + " for " + damage + " damage.");

            //create the enmity this generates
            return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)damage, 60);
        }

        protected Enmity RangedAttack()
        {
            Game.Log(Game.LogLevel.TRACE, "% SkilEffect.RangedAttack %");
            //we cannot attack a friend
            if (((this.actor is Player) && (this.target is Player)) || (this.actor == this.target)) return null;

            //calculate our max damage possible
            double maxDamage = this.skill.GetMaxDamage() + this.actor.GetDamageModifier(this.time);

            //use the positional error to modify the max damage
            maxDamage = this.CalculatePositionalMax(maxDamage, 0.6, 0.5 );

            //future attack with an area attack - for now we ignore that bit in this skill

            //calculate how much damage we do to our target
            double actor_score = Battle.DamageScore(this.actor.GetLevel(time),
                                                    this.actor.GetStat(Card.Stat.LUCK, this.time),
                                                    this.actor.GetStat(Card.Stat.STRENGTH, this.time) +
                                                    this.actor.GetStat(Card.Stat.BALANCE, this.time) +
                                                    this.actor.GetStat(Card.Stat.FOCUS, this.time));

            double target_score = Battle.DamageScore(this.target.GetLevel(time),
                                                     this.target.GetStat(Card.Stat.LUCK, this.time),
                                                     this.target.GetStat(Card.Stat.GRIT, this.time) +
                                                     this.target.GetStat(Card.Stat.SPEED, this.time));

            double damage = maxDamage * actor_score / (actor_score + target_score);

            //check for miss or critical hit
            if (Battle.CriticalLucky(2 * this.target.GetStat(Card.Stat.LUCK, this.time)))
            {
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " missed " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
                return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)damage, 20);
            }
            else if (Battle.CriticalLucky(this.actor.GetStat(Card.Stat.LUCK, this.time)))
            {
                damage = maxDamage;
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " scored a lucky hit on " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
            }

            //inflict the damage
            target.ChangeHp((int)-damage, this.time);
            Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " hit " + this.target.ToString() + " with " + this.skill.GetSkillName() + " for " + damage + " damage.");

            //create the enmity this generates
            return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)damage, 60);
        }

        protected Enmity SpellAttack()
        {
            Game.Log(Game.LogLevel.TRACE, "% SkilEffect.SpellAttack %");
            //we cannot attack a friend
            if (((this.actor is Player) && (this.target is Player)) || (this.actor == this.target)) return null;

            //calculate our max damage possible
            double maxDamage = this.skill.GetMaxDamage() + this.actor.GetDamageModifier(this.time);

            //future attack with an area attack - for now we ignore that bit in this skill

            //calculate how much damage we do to our target
            double actor_score = Battle.DamageScore(this.actor.GetLevel(time),
                                                    this.actor.GetStat(Card.Stat.LUCK, this.time),
                                                    3 * this.actor.GetStat(Card.Stat.FOCUS, this.time) +
                                                    this.actor.GetStat(Card.Stat.BALANCE, this.time));

            double target_score = Battle.DamageScore(this.target.GetLevel(time),
                                                     this.target.GetStat(Card.Stat.LUCK, this.time),
                                                     this.target.GetStat(Card.Stat.GRIT, this.time) +
                                                     2 * this.target.GetStat(Card.Stat.FAITH, this.time));

            double damage = maxDamage * actor_score / (actor_score + target_score);

            //check for critical hit
            if ( Battle.CriticalLucky(this.actor.GetStat(Card.Stat.LUCK, this.time) / 2) )
            {
                damage = maxDamage;
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " had a lucky spellcast on " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
            }

            //inflict the damage
            target.ChangeHp((int)-damage, this.time);
            Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " hit " + this.target.ToString() + " with " + this.skill.GetSkillName() + " for " + damage + " damage.");

            //create the enmity this generates
            return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)damage, 60);
        }

        protected Enmity HealAssist()
        {
            Game.Log(Game.LogLevel.TRACE, "% SkilEffect.HealAssist %");
            //we cannot heal an enemy
            if ( this.target.GetType() != this.actor.GetType() ) return null;

            //calculate our max damage possible
            double maxHeal = this.skill.GetMaxDamage() + this.actor.GetDamageModifier(this.time);

            //use the range error to modify the max damage
            maxHeal = this.CalculateRangedMax(maxHeal, 0.9);

            //future heal with an area attack - for now we ignore that bit in this skill

            //calculate how much healing we do to our target
            double actor_score = Battle.HealScore(this.actor.GetLevel(time),
                                                    this.actor.GetStat(Card.Stat.LUCK, this.time),
                                                    3 * this.actor.GetStat(Card.Stat.FAITH, this.time) +
                                                    this.actor.GetStat(Card.Stat.GRIT, this.time));

            //this is a referance to compare assume all skills are 10
            double referance_score = Battle.HealScore(((Heal)(this.skill)).GetLevel(), 10, 40 );

            //after the actor is at the level of the spell, no more gain will be seen
            double heal =  (actor_score * maxHeal) / (2 * referance_score);
            if (heal > maxHeal) heal = maxHeal;

            //check critical heal
            if (Battle.CriticalLucky(this.actor.GetStat(Card.Stat.LUCK, this.time)))
            {
                heal = maxHeal;
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " had a lucky spellcast on " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
            }

            //perform the magic
            target.ChangeHp((int)heal, this.time);
            Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " healed " + this.target.ToString() + " with " + this.skill.GetSkillName() + " for " + heal + " health.");

            //create the enmity this generates
            return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    (int)heal, 60);
        }

        protected Enmity CureAssist()
        {
            Game.Log(Game.LogLevel.TRACE, "% SkilEffect.CureAssist %");
            //we cannot heal an enemy
            if (this.target.GetType() != this.actor.GetType()) return null;

            //calculate our max effects cured possible
            int maxCure = this.skill.GetMaxDamage();

            //future cure with an area attack - for now we ignore that bit in this skill

            //calculate how many cures we do to our target
            int faith = this.actor.GetStat(Card.Stat.LUCK, this.time);
            int cure_number = Battle.AddCures(faith, faith);
            if (cure_number > maxCure) cure_number = maxCure;

            //check critical cure
            if (Battle.CriticalLucky(this.actor.GetStat(Card.Stat.LUCK, this.time)))
            {
                cure_number = maxCure;
                Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " had a lucky spellcast on " + this.target.ToString() + " with " + this.skill.GetSkillName() + ".");
            }

            //perform the magic
            int numCured = target.CureGlamours(((Cure)this.skill).GetEffect(), cure_number, this.time);
            Game.Log(Game.LogLevel.NORMAL, this.actor.ToString() + " cured " + this.target.ToString() + " with " + ((Cure)this.skill).GetEffectName() + " for " + numCured + " curses.");

            //create the enmity this generates
            return new Enmity(this.time, this.actor.GetLevel(time), this.actor.GetStat(Card.Stat.ALLURE, this.time),
                                    10 * numCured, 60);
        }

        protected double CalculateRangedMax(double max, double scale)
        {
            //find our range
            int range = Board.Distance(this.actor.GetLocation(), this.target.GetLocation());
            range += this.actor.GetRangeModifier(this.time) + this.target.GetRangeModifier(this.time);

            //find our range error
            int range_error = Math.Abs(range - this.skill.GetRange());
            range_error += this.actor.GetRangeCorrection(time);
            if (range_error < 0) range_error = 0;

            //modify the max points we can get
            max *= Math.Pow(scale, range_error);

            return max;
        }

        protected double CalculatePositionalMax(double max, double fastScale, double slowScale)
        {
            //find our position
            int position = (int)Board.Position( this.enemy.GetLocation(), 
                this.enemy.GetTarget().GetLocation(), this.actor.GetLocation());
            position += this.actor.GetPositionModifier(this.time) + this.target.GetPositionModifier(this.time);
            if ( position < (int)Skill.Position.FORWARD ) position = (int)Skill.Position.FORWARD;
            if ( position > (int)Skill.Position.REAR ) position = (int)Skill.Position.REAR;

            //find our position error
            int position_error = Math.Abs(position - (int)this.skill.GetPosition());
            position_error += this.actor.GetPositionCorrection(time);
            if (position_error < 0) position_error = 0;

            //modify the max points we can get
            if( this.skill.GetSpeed() == Skill.Speed.FAST )
            {
                max *= Math.Pow(fastScale, position_error);
            }
            else
            {
                max *= Math.Pow(slowScale, position_error);
            }
            return max;
        }

        protected int CalculateCost()
        {
            //actions cost nothing
            if (this.skill.GetSkillType() < Skill.Type.SPELL) return 0;

            switch( this.skill.GetSkillType() )
            {
                case Skill.Type.SPELL:
                case Skill.Type.HEAL:
                    return this.skill.GetMaxDamage() / 5;
                case Skill.Type.CURE:
                    Cure cure = (Cure)this.skill;
                    int baseCost = 3;
                    switch (cure.GetEffect() )
                    {
                        case Cure.Effect.STRENGTH:
                        case Cure.Effect.GRIT:
                        case Cure.Effect.SPEED:
                        case Cure.Effect.BALANCE:
                        case Cure.Effect.FAITH:
                        case Cure.Effect.FOCUS:
                        case Cure.Effect.LUCK:
                        case Cure.Effect.ALLURE:
                            return baseCost * 1 * cure.GetMaxDamage();
                            
                        case Cure.Effect.MAX_HIT_POINTS:
                        case Cure.Effect.MAX_MAGIC_POINTS:
                        case Cure.Effect.RANGE:
                        case Cure.Effect.POSITION:
                        case Cure.Effect.HIT_POINTS:
                        case Cure.Effect.MAGIC_POINTS:
                        case Cure.Effect.MAX_DAMAGE:
                        case Cure.Effect.STATS:
                            return baseCost * 2 * cure.GetMaxDamage();

                        case Cure.Effect.LOCATION:
                        case Cure.Effect.MAXES:
                        case Cure.Effect.POINTS:
                        case Cure.Effect.LEVEL:
                            return baseCost * 4 * cure.GetMaxDamage();

                        case Cure.Effect.ALL:
                            return baseCost * 5 * cure.GetMaxDamage();
                        default:
                            return 0;
                    }//switch cure effect
                default:
                    return 0;
            }//switch skill type
        }
    }
}
