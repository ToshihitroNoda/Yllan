using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yılan
{
    public abstract class Scene
    {
        public Game game = new Game();

        public abstract void Update();
        public abstract void Draw();
    }
}
