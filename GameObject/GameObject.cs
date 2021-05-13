using System.Collections.Generic;
using DxLibDLL;
using MyLib;

namespace Yılan
{
    public abstract class GameObject
    {
        public float x;
        public float y;
        public bool isDead = false;

        public int enemyCount;

        protected BattlePhase battlePhase;
        protected int imageWidth;
        protected int imageHeight;
        protected int hitboxOffsetLeft = 0;
        protected int hitboxOffsetRight = 0;
        protected int hitboxOffsetTop = 0;
        protected int hitboxOffsetBottom = 0;

        float prevX;
        float prevY;
        float prevLeft;
        float prevRight;
        float prevTop;
        float prevBottom;

        // コンストラクタ
        public GameObject(BattlePhase battlePhase)
        {
            this.battlePhase = battlePhase;
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
            // 四角形を描画
            //Camera.DrawLineBox(GetLeft(), GetTop(), GetRight(), GetBottom(), DX.GetColor(255, 0, 0));
        }

        // 他のオブジェクトと衝突したときに呼ばれる
        public abstract void OnCollision(GameObject other);

        public abstract void OnCollisionEnemy(EnemyBase other);

        // 画面内に映っているか？
        public virtual bool IsVisible()
        {
            return MyMath.RectRectIntersect(
                x, y, x + imageWidth, y + imageHeight,
                Camera.x, Camera.y, Camera.x + Screen.Width, Camera.y + Screen.Height);
        }
    }
}

