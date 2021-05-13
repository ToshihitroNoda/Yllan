using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class EnemyBullet : EnemyBase
    {
        const int VisibleRadius = 8;

        public float collisionRadius = 8f;

        float vx;
        float vy;
        float angle;

        public EnemyBullet (BattlePhase battlePhase, float x, float y, float angle, float speed)
            :base(battlePhase)
        {
            this.x = x;
            this.y = y;
            this.angle = angle;

            imageWidth = 16;
            imageHeight = 16;
            hitboxOffsetLeft = 2;
            hitboxOffsetRight = 2;
            hitboxOffsetTop = 2;
            hitboxOffsetBottom = 2;

            vx = (float)Math.Cos(angle) * speed;
            vy = (float)Math.Sin(angle) * speed;

            DX.PlaySoundMem(Music.bullet_se, DX.DX_PLAYTYPE_BACK);
            DX.ChangeVolumeSoundMem(200, Music.bullet_se);

        }

        public override void Update()
        {

            x += vx;
            y += vy;

            // 画面外に出たら死亡フラグを立てる
            if (y + VisibleRadius < 0 || y - VisibleRadius > Screen.Height ||
                x + VisibleRadius < 0 || x - VisibleRadius > Screen.Width)
            {
                isDead = true;
            }
        }

        public override void Draw()
        {
            Camera.DrawGraph(x, y, 1, angle, Image.EnemyBullet);
        }

        public override void OnCollision(GameObject other)
        {
         if (other is Player)
            {
                isDead = true;
            }
        }

        public override void OnCollisionEnemy(EnemyBase other)
        {
            return;
        }

    }
}
