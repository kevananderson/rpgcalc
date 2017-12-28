using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    public abstract class Actor : Card
    {
        public enum Type { ROLE, CHARACTER, CREATURE, FAMILIAR };
        public static String[] type_string = {"Role", "Character", "Creature", "Familiar"};
        public static int type_mask = 0x3000;
        public static int type_shift = 12;

        protected Actor.Type type;

        /*** constructor ***/

        internal Actor(int cid) : base( cid )
        {
            this.type = (Actor.Type)((cid & Actor.type_mask) >> Actor.type_shift);
        }

        /*** public ***/

        public abstract Matrix GetStats();

    }
}
