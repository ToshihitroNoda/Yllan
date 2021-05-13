using System.Collections.Generic;
using DxLibDLL;
using MyLib;

namespace Yılan
{
    public abstract class EnemyBase
    {
        public float x;
        public float y;
        public bool isDead = false;

        public int enemyCount;
        public bool onCollisionTrackingFlg = false;

        protected BattlePhase battlePhase;
        protected int imageWidth;
        protected int imageHeight;
        protected int hitboxOffsetLeft = 0;
        protected int hitboxOffsetRight = 0;
        protected int hitboxOffsetTop = 0;
        protected int hitboxOffsetBottom = 0;
        protected int life;

        protected float playerDistanceX = 0;
        protected float playerDistanceY = 0;
        protected Direction direction = new Direction();

        public List<GameObject> prevHitObjects = new List<GameObject>();//前のフレームで当たったオブジェクト
        public List<GameObject> currentHitObjects = new List<GameObject>();//現在のフレームで当たったオブジェクト

        float prevX;
        float prevY;
        float prevLeft;
        float prevRight;
        float prevTop;
        float prevBottom;

        // コンストラクタ
        public EnemyBase(BattlePhase battlePhase)
        {
            direction = Direction.Down;
            this.battlePhase = battlePhase;
            prevHitObjects = new List<GameObject>();//前のフレームで当たったオブジェクト
            currentHitObjects = new List<GameObject>();//現在のフレームで当たったオブジェクト


        }

        // 当たり判定の左端を取得
        public virtual float GetLeft()
        {
            return x + hitboxOffsetLeft;
        }

        // 左端を指定することにより位置を設定する
        public virtual void SetLeft(float left)
        {
            x = left - hitboxOffsetLeft;
        }

        // 右端を取得
        public virtual float GetRight()
        {
            return x + imageWidth - hitboxOffsetRight;
        }
        public virtual void SetRight(float right)
        {
            x = right + hitboxOffsetRight - imageWidth;
        }

        // 上端を取得
        public virtual float GetTop()
        {
            return y + hitboxOffsetTop;
        }

        public virtual void SetTop(float top)
        {
            y = top - hitboxOffsetTop;
        }


        // 下端を取得する
        public virtual float GetBottom()
        {
            return y + imageHeight - hitboxOffsetBottom;
        }

        // 下端を取得することにより位置を設定する
        public virtual void SetBottom(float bottom)
        {
            y = bottom + hitboxOffsetBottom - imageHeight;
        }

        // 1フレーム前からの移動量（x方向）
        public float getDeltaX()
        {
            return x - prevX;
        }

        // 1フレーム前からの移動量（y方向）
        public float getDeltaY()
        {
            return y - prevY;
        }

        // 1フレーム前の左端を取得する
        public float GetPrevLeft()
        {
            return prevLeft;
        }

        // 1フレーム前の左端を取得する
        public float GetPrevRight()
        {
            return prevRight;
        }

        // 1フレーム前の左端を取得する
        public float GetPrevTop()
        {
            return prevTop;
        }

        // 1フレーム前の左端を取得する
        public float GetPrevBottom()
        {
            return prevBottom;
        }

        // 1フレーム前の場所と当たり判定を記憶する
        public void StorePostionAndHitBox()
        {
            prevX = x;
            prevY = y;
            prevLeft = GetLeft();
            prevRight = GetPrevRight();
            prevTop = GetTop();
            prevBottom = GetBottom();
        }

        // 更新処理
        public abstract void Update();

        // 描画処理
        public abstract void Draw();

        // 当たり判定を描画（）デバッグ用
        public void DrawHitBox()
        {
            //float left = GetLeft() + 5;
            //float right = GetRight() - 5 - 0.01f;
            //float top = GetTop() + 5;
            //float bottom = GetBottom() - 5 - 0.01f;

            //// 四角形を描画
            //Camera.DrawLineBox(GetLeft(), GetTop(), GetRight(), GetBottom(), DX.GetColor(255, 0, 0));
            //Camera.DrawLineBox(left, top, right, bottom, DX.GetColor(0, 255, 0));
        }

        public virtual void MoveX()
        {
            x += playerDistanceX;

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
            float bottom = GetBottom() - 0.01f;

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

        public virtual void MoveY()
        {
            y += playerDistanceY;

            if (playerDistanceY >= 0 && playerDistanceY > playerDistanceX)
            {
                direction = Direction.Down;
            }
            else if (playerDistanceY <= 0 && playerDistanceY < playerDistanceX)
            {
                direction = Direction.Up;
            }

            float left = GetLeft();
            float middle = left + (imageWidth - hitboxOffsetLeft - hitboxOffsetRight) / 2;
            float right = GetRight() - 0.01f;
            float top = GetTop();
            float bottom = GetBottom() - 0.01f;

            if (battlePhase.map.IsWall(left, top) || // 左上が壁か？
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

        // 他のオブジェクトと衝突したときに呼ばれる
        public virtual void OnCollision(GameObject other)
        {
            currentHitObjects.Add(other);


            foreach (GameObject obj in prevHitObjects)
            {
                if (obj.Equals(other))
                    return;
            }

            if (other is PlayerBullet)
            {

                DX.PlaySoundMem(Music.oncollisionbullet_se, DX.DX_PLAYTYPE_BACK);
                DX.ChangeVolumeSoundMem(200, Music.oncollisionbullet_se);

                PlayerBullet bullet = other as PlayerBullet;

                if (bullet.bulletType == PlayerBullet.BulletType.Tracking && onCollisionTrackingFlg == false)
                {
                    BattlePhase.testCounter++;
                    life -= PlayerBullet.trackingAttackpoint;
                    onCollisionTrackingFlg = true;
                }
                else if (bullet.bulletType == PlayerBullet.BulletType.Nomal)
                {
                    life -= PlayerBullet.normalAttackpoint;
                }
                else if (bullet.bulletType == PlayerBullet.BulletType.Diffusion)
                {
                    life -= PlayerBullet.diffusionAttackpoint;
                }

                else if (bullet.bulletType == PlayerBullet.BulletType.EveryDirection)
                {
                    life -= PlayerBullet.everyDirectionAttackpoint;
                }
            }
        }

        public virtual void OnCollisionEnemy(EnemyBase other)
        {
            if (other is EnemyBullet || other is Boss2)
                return;

            //SetLeft(GetPrevLeft()); SetTop(GetPrevTop());
            if (other.x < x)
            {
                SetLeft(other.GetRight() + hitboxOffsetLeft + 1);
            }
            else
            {
                SetRight(other.GetLeft() - hitboxOffsetRight - 1);
            }

            if (other.y < y)
            {
                SetTop(other.GetBottom() + hitboxOffsetTop + 1);
            }
            else
            {
                SetBottom(other.GetTop() - hitboxOffsetBottom - 1);
            }
        }


        // 画面内に映っているか？
        public virtual bool IsVisible()
        {
            return MyMath.RectRectIntersect(
                x, y, x + imageWidth, y + imageHeight,
                Camera.x, Camera.y, Camera.x + Screen.Width, Camera.y + Screen.Height);
        }
    }
}

