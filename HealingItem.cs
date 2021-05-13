using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class HealingItem : GameObject
    {
        float angle = 0;

        public HealingItem(BattlePhase battlePhase, float x, float y)
            : base(battlePhase)
        {
            this.x = x;
            this.y = y;
            this.battlePhase = battlePhase;

            imageWidth = 32;
            imageHeight = 64;
            hitboxOffsetLeft = 0;
            hitboxOffsetRight = 0;
            hitboxOffsetTop = 0;
            hitboxOffsetBottom = 0;
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            Camera.DrawGraph(x, y, 1, angle, Image.item1);
        }

        public override void OnCollision(GameObject other)
        {
            if (other is Player)
            {// プレイヤーと当たったら消す
                isDead = true;
            }
        }

        public override void OnCollisionEnemy(EnemyBase other)
        {
        }
    }
}
