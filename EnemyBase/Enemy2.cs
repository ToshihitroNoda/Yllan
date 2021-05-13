using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class Enemy2 : EnemyBase
    {
        public static int attackPoint = 2; // 攻撃力
        int coolTime = 0;

        int animationCounter = 0;
        public bool attackFlg = false;

        float closestPlayerX; // 一番近いプレイヤーのx座標
        float closestPlayerY; // 一番近いプレイヤーのy座標

        float distToPlayer = 120; // プレイヤーとの保つ距離

        float minimumDist = 1000000; // 一番近い距離
        int nearPlayerIndex = 0; // プレイヤーカウント

        public Enemy2(BattlePhase battlePhase, float x, float y)
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

            life = 60;　// ライフ
        }

        public override void Update()
        {
            float playerCount = battlePhase.players.Count;
           
            animationCounter++;

            for (int i = 0; i < playerCount; i++) // 一番近いプレイヤーを検索
            {
               

                float dx = battlePhase.players[i].x - x; // プレイヤーとの距離（x）
                float dy = battlePhase.players[i].y - y; // プレイヤーとの距離（y）

                float dist2 = dx * dx + dy * dy;
                if (dist2 < minimumDist)
                {
                    nearPlayerIndex = i;
                    minimumDist = dist2;
                }
            }
            if (battlePhase.players.Count-1 >= nearPlayerIndex)
            {
                closestPlayerX = battlePhase.players[nearPlayerIndex].x + 30; // 一番近いプレイヤーのx座標
                closestPlayerY = battlePhase.players[nearPlayerIndex].y + 35; // 一番近いプレイヤーのy座標
            }

            float angleToPlayer = MyMath.PointToPointAngle(x, y, closestPlayerX, closestPlayerY); // 自分と一番近いプレイヤーとの角度
            float speed = 0.75f; // 速さ設定
            playerDistanceX = (float)Math.Cos(angleToPlayer) * speed;
            playerDistanceY = (float)Math.Sin(angleToPlayer) * speed;

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

            MoveX();
            MoveY();

            if (coolTime <= 0)
            {
                battlePhase.enemyBases.Add(new EnemyBullet(battlePhase, x, y, angleToPlayer, 5f));
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
                ResultPhase.exp += 400;
            }
        }
        public override void MoveX()
        {
            if (closestPlayerX > x) // 一番近いプレイヤーが自分より右側にいるとき
            {
                if (closestPlayerX - x <= distToPlayer)
                {
                    x += playerDistanceX;
                }
                else
                {
                    x -= playerDistanceX;
                }
            }
            else if (closestPlayerX < x) // 一番近いプレイヤーが自分より左側にいるとき
            {
                if (x - closestPlayerX >= distToPlayer)
                {
                    x -= playerDistanceX;
                }
                else
                {
                    x += playerDistanceX;
                }
            }

            if (playerDistanceX >= 0)
            {
                direction = Direction.Right;
            }
            else if (playerDistanceX <= 0)
            {
                direction = Direction.Left;
            }

            float left = GetLeft();
            float right = GetRight() - 0.01f;
            float top = GetTop();
            float middle = top + (imageHeight - hitboxOffsetTop - hitboxOffsetBottom) / 2;
            float bottom = GetBottom() - 0.02f;

            // 左端が壁にめり込んでいるか？
            if (battlePhase.map.IsWall(left, top) ||
                battlePhase.map.IsWall(left, middle) ||
                battlePhase.map.IsWall(left, bottom))
            {
                float wallRight = left - left % Map.CellSize + Map.CellSize; // 壁の右端
                SetLeft(wallRight); // プレイヤーの左端を壁の右端に沿わす
            }
            // 右端が壁にめり込んでいるか？
            else if (
                    battlePhase.map.IsWall(right, top) ||
                    battlePhase.map.IsWall(right, middle) ||
                    battlePhase.map.IsWall(right, bottom))
            {
                float wallLeft = right - right % Map.CellSize;
                SetRight(wallLeft);
            }
        }

        public override void MoveY()
        {
            if (closestPlayerY > y && playerDistanceY > playerDistanceX)
            {
                if (closestPlayerY - y >= distToPlayer)
                {
                    y -= playerDistanceY;
                }
                else
                {
                    y += playerDistanceY;
                }
            }
            else if (closestPlayerY < y && playerDistanceY < playerDistanceX)
            {
                if (y - closestPlayerY >= distToPlayer)
                {
                    y -= playerDistanceY;
                }
                else
                {
                    y += playerDistanceY;
                }
            }

            if (playerDistanceY >= 0)
            {
                direction = Direction.Down;
            }
            else if (playerDistanceY <= 0)
            {
                direction = Direction.Up;
            }

            float left = GetLeft();
            float middle = left + (imageWidth - hitboxOffsetLeft - hitboxOffsetRight) / 2;
            float right = GetRight() - 0.01f;
            float top = GetTop();
            float bottom = GetBottom() - 0.01f;

            if (battlePhase.map.IsWall(left, top) ||
                battlePhase.map.IsWall(middle, top) ||
                battlePhase.map.IsWall(right, top))   // 右上が壁か？
            {
                float wallBottom = top - top % Map.CellSize + Map.CellSize; // 天井のy座標
                SetTop(wallBottom); // プレイヤーの頭を天井に沿わす
            }
            else if (
                battlePhase.map.IsWall(left, bottom) ||
                battlePhase.map.IsWall(middle, bottom) ||
                battlePhase.map.IsWall(right, bottom))
            {
                float wallTop = bottom - bottom % Map.CellSize;
                SetBottom(wallTop);
            }
        }

        public override void Draw()
        {
            Camera.DrawRectGraph(x, y, imageWidth * (animationCounter / 16 % 2), imageHeight, imageWidth, imageHeight, Image.Enemy2, direction, playerDistanceX, playerDistanceY, attackFlg);
        }

       
    }
}
