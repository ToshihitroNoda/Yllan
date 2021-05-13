using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yılan
{
    public abstract class Phase
    {
        protected Game game;

        public Phase(Game game)
        {
            this.game = game;
        }
        public abstract void Update();
        public abstract void Draw();
    }
}
