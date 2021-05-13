using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class Enemy3 : EnemyBase
    {
        public static int attackPoint = 10; // 攻撃力

        int animationCounter = 0;
        public bool attackFlg = false;
        int attackCounter = 20;

        public Enemy3(BattlePhase battlePhase, float x, float y)
            : base(battlePhase)
        {
            this.x = x;
            this.y = y;

            direction = Direction.Down;

            imageWidth = 64;
            imageHeight = 64;
            hitboxOffsetLeft = imageWidth / 4;
            hitboxOffsetRight = imageWidth / 4;
            hitboxOffsetTop = imageHeight / 4;
            hitboxOffsetBottom = imageHeight / 4;

            life = 100;　// ライフ
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
            float speed = 2.5f;
            playerDistanceX = (float)Math.Cos(angleToPlayer) * speed;
            playerDistanceY = (float)Math.Sin(angleToPlayer) * speed;

            MoveX();
            MoveY();

            foreach (EnemyBase e in battlePhase.enemyBases)
            {
                if (e == this)
                    continue;

                // オブジェクトBが死んでたらスキップ
                if (e.isDead) continue;

                // オブジェクトAとBが重なっているか？
                if (MyMath.RectRectIntersect(
                    GetLeft(), GetTop(), GetRight(), GetBottom(),
                    e.GetLeft(), e.GetTop(), e.GetRight(), e.GetBottom()))
                {
                    SetLeft(GetPrevLeft()); SetTop(GetPrevTop());

                }
            }

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
                ResultPhase.exp += 600;
            }

            attackCounter--;
        }

        public override void Draw()
        {
            Camera.DrawRectGraph(x, y, imageWidth * (animationCounter / 16 % 2), imageHeight, imageWidth, imageHeight, Image.Enemy3, direction, playerDistanceX, playerDistanceY, attackFlg);
        }

    }
}
