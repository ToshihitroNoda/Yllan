using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class PlayerBullet : GameObject
    {
        public enum BulletType
        {
            Nomal,         // 通常弾
            Diffusion,     // 拡散弾
            Tracking,      // 追尾弾
            EveryDirection // 全方位弾
        }

        public const float MoveSpeed = 15f; // 横移動速度
        const int VisibleRadius = 16;

        public float angle;

        float vx; // 横移動速度
        float vy; // 縦移動速度

        Direction direction = new Direction();
        public static Pad padPlaying;
        static Game game = new Game();
        public BulletType bulletType = new BulletType();
        Player player;

        int timeSinceStartedShooting = 0; // 弾を撃ち始めてからの時間


        public static int normalLevel = 1; // 通常弾のレベル
        public static int diffusionLevel = 1; // 拡散弾のレベル
        public static int trackingLevel = 1; // 追尾弾のレベル
        public static int everyDirectionLevel = 1; // 全方位弾のレベル

        public static int normalAttackpoint = 2; // 通常弾の攻撃力
        public static int diffusionAttackpoint = 1; // 拡散弾の攻撃力
        public static int trackingAttackpoint = 2; // 追尾弾の攻撃力
        public static int everyDirectionAttackpoint = 1; // 全方位弾の攻撃力

        public static int normalRange = 30; // 通常弾の射程距離（弾を撃ち始めてから消えるまでの時間）
        public static int diffusionRange = 30; // 拡散弾の射程距離（弾を撃ち始めてから消えるまでの時間）
        public static int everyDirectionRange = 15;  // 全方位弾の射程距離（弾を撃ち始めてから消えるまでの時間）

        public static float diffusionCoolTimeMax = 30; // 拡散弾リロードタイム
        public static float trackingCoolTimeMax = 60; // 追尾弾リロードタイム
        public static float everyDirectionCoolTimeMax = 150; // 全方位弾リロードタイム

        // プレイヤーの弾
        public PlayerBullet(BattlePhase battlePhase, Player player, BulletType type, float x, float y, Direction direction, float angle)
            : base(battlePhase)
        {
            if (MainMenu.loadFlg == false)
            {
                normalLevel = 1; 
                diffusionLevel = 1; 
                trackingLevel = 1; 
                everyDirectionLevel = 1; 

                normalAttackpoint = 2; 
                diffusionAttackpoint = 1; 
                trackingAttackpoint = 2; 
                everyDirectionAttackpoint = 1; 

                normalRange = 30; 
                diffusionRange = 30; 
                everyDirectionRange = 15;  

                diffusionCoolTimeMax = 30; 
                trackingCoolTimeMax = 60; 
                everyDirectionCoolTimeMax = 150;
            }

            bulletType = type;
            this.x = x;
            this.y = y;
            this.direction = direction;
            this.angle = angle;
            this.player = player;

            imageWidth = 32;
            imageHeight = 32;
            hitboxOffsetLeft = 6;
            hitboxOffsetRight = 6;
            hitboxOffsetTop = 6;
            hitboxOffsetBottom = 6;

            if (direction == Direction.Right) vx = MoveSpeed;
            else if (direction == Direction.Left) vx = -MoveSpeed;

            if (direction == Direction.Down) vy = MoveSpeed;
            else if (direction == Direction.Up) vy = -MoveSpeed;

            DX.PlaySoundMem(Music.bullet_se, DX.DX_PLAYTYPE_BACK);
            DX.ChangeVolumeSoundMem(200, Music.bullet_se);
        }

        public override void Update()
        {
            if (bulletType == BulletType.Nomal)
            {
                vx = (float)Math.Cos(angle) * MoveSpeed;
                vy = (float)Math.Sin(angle) * MoveSpeed;

                // 移動
                x += vx;
                y += vy;

                timeSinceStartedShooting++;

                // 画面外に出たら死亡フラグを立てる
                if (y + VisibleRadius < 0 || y - VisibleRadius > Screen.Height ||
                    x + VisibleRadius < 0 || x - VisibleRadius > Screen.Width ||
                    timeSinceStartedShooting >= normalRange)
                {
                    isDead = true;
                }
            }

            if (bulletType == BulletType.Diffusion)
            {
                vx = (float)Math.Cos(angle) * MoveSpeed;
                vy = (float)Math.Sin(angle) * MoveSpeed;

                // 移動
                x += vx;
                y += vy;

                timeSinceStartedShooting++;

                // 画面外に出たら死亡フラグを立てる
                if (y + VisibleRadius < 0 || y - VisibleRadius > Screen.Height ||
                    x + VisibleRadius < 0 || x - VisibleRadius > Screen.Width ||
                    timeSinceStartedShooting >= diffusionRange)
                {
                    isDead = true;
                }
            }

            if (bulletType == BulletType.Tracking)
            {
                x += Player.enemyDistanceX;
                y += Player.enemyDistanceY;

                // 画面外に出たら死亡フラグを立てる
                if (y + VisibleRadius < 0 || y - VisibleRadius > Screen.Height ||
                    x + VisibleRadius < 0 || x - VisibleRadius > Screen.Width)
                {
                    isDead = true;
                }
            }
             
            if (bulletType == BulletType.EveryDirection)
            {
                vx = (float)Math.Cos(angle) * MoveSpeed;
                vy = (float)Math.Sin(angle) * MoveSpeed;

                x += vx;
                y += vy;

                timeSinceStartedShooting++;

                // 画面外に出たら死亡フラグを立てる
                if (y + VisibleRadius < 0 || y - VisibleRadius > Screen.Height ||
                    x + VisibleRadius < 0 || x - VisibleRadius > Screen.Width ||
                    timeSinceStartedShooting >= everyDirectionRange)
                {
                    isDead = true;
                }
            }

            float left = GetLeft();
            float right = GetRight() - 0.01f;
            float top = GetTop();
            float bottom = GetBottom() - 0.01f;

            // 壁に当たっていたら削除
            if (
                battlePhase.map.IsWall(left, top) ||
                battlePhase.map.IsWall(left, bottom) ||
                battlePhase.map.IsWall(right, top) ||
                battlePhase.map.IsWall(right, bottom)
               )
            {
                if (bulletType != BulletType.Tracking)
                    isDead = true;
            }

            // カメラの範囲外に出たら削除
            if (!IsVisible())
            {
                if (bulletType != BulletType.Tracking)
                    isDead = true;
            }
        }

        public override void Draw()
        {
            Camera.DrawGraph(x + 16, y + 16, 1, angle, Image.playerbullet);
        }

        public override void OnCollision(GameObject other)
        {
        }

        public override void OnCollisionEnemy(EnemyBase other)
        {
            if (bulletType != BulletType.Tracking)
            {
                if (other is Enemy1 || other is Enemy2 || other is Enemy3 || other is Enemy4 ||
                    other is Boss1 || other is Boss2)
                    isDead = true;
            }
        }
    }
}
