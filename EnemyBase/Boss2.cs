using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class Boss2 : EnemyBase
    {
        public static int attackPoint = 30; // 攻撃力
        int coolTime = 0;

        int animationCounter = 0;
        public bool attackFlg = false;

        public Boss2(BattlePhase battlePhase, float x, float y)
            : base(battlePhase)
        {
            this.x = x;
            this.y = y;

            direction = Direction.Down;

            imageWidth = 256;
            imageHeight = 256;
            hitboxOffsetLeft = 50;
            hitboxOffsetRight = 50;
            hitboxOffsetTop = 65;
            hitboxOffsetBottom = 55;

            life = 1000; // ライフ
        }
        public override void Update()
        {
            float playerCount = battlePhase.players.Count;

            float closestPlayerX; // 一番近いプレイヤーのx座標
            float closestPlayerY; // 一番近いプレイヤーのy座標

            float minimumDist = 1000000; // 一番近い距離
            int nearPlayerIndex = 0; // プレイヤーカウント

            animationCounter++;

            for (int i = 0; i < playerCount; i++)
            {
                float dx = battlePhase.players[i].x - x;
                float dy = battlePhase.players[i].y - y;

                float dist2 = dx * dx + dy * dy;
                if (dist2 < minimumDist)
                {
                    nearPlayerIndex = i;
                    minimumDist = dist2;
                }
            }

            closestPlayerX = battlePhase.players[nearPlayerIndex].x;
            closestPlayerY = battlePhase.players[nearPlayerIndex].y;

            float angleToPlayer = MyMath.PointToPointAngle(x, y, closestPlayerX, closestPlayerY);
            float speed = 5f;
            playerDistanceX = (float)Math.Cos(angleToPlayer) * speed;
            playerDistanceY = (float)Math.Sin(angleToPlayer) * speed;

            MoveX();
            MoveY();

            if (coolTime <= 0)
            {
                battlePhase.enemyBases.Add(new EnemyBullet(battlePhase, x + imageHeight / 2, y + imageWidth / 2, angleToPlayer, 4f));
                coolTime = 20;
            }
            coolTime--;

            // ★★デバッグ用★★ すぐ死ぬボタン
            if (Title.debugModeFlg)
            {
                if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_8))
                    life = 0;
            }
            // ★★★★

            if (life <= 0)
            {
                isDead = true;
                Map.count--;
                Map.enemyBases.RemoveAt(Map.count);
                ResultPhase.defeatedEnemyCount++;
                ResultPhase.exp += 15000;
            }
        }

        public override void Draw()
        {
            Camera.DrawRectGraph(x, y, imageWidth * (animationCounter / 16 % 2), imageHeight, imageWidth, imageHeight, Image.Boss2, direction, playerDistanceX, playerDistanceY, attackFlg);
        }

        public override void OnCollisionEnemy(EnemyBase other)
        {
            return;
        }

    }
}
