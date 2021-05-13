using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class Enemy4 : EnemyBase
    {
        public static int attackPoint = 7; // 攻撃力
        int coolTime = 0;

        int animationCounter = 0;
        public bool attackFlg = false;
        int attackCounter;

        public Enemy4 (BattlePhase battlePhase, float x, float y)
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

            life = 80;　// ライフ
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
            float speed = 3f;
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

            if (coolTime <= 0)
            {
                battlePhase.enemyBases.Add(new EnemyBullet(battlePhase, x, y, angleToPlayer, 4f));
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
                ResultPhase.exp += 700;
            }

            attackCounter--;
        }

        public override void Draw()
        {
            Camera.DrawRectGraph(x, y, imageWidth * (animationCounter / 4 % 2), imageHeight, imageWidth, imageHeight, Image.Enemy4, direction, playerDistanceX, playerDistanceY, attackFlg);
        }
    }
}
