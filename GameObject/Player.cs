using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class Player : GameObject
    {
        const float MoveSpeed = 10f; // 移動速度
        const int MutekiJikan = 60; // 無敵時間

        public float vx = 0;
        public float vy = 0;
        public float angle = 0;

        public static float enemyDistanceX = 0;
        public static float enemyDistanceY = 0;

        public Direction direction = Direction.Down;
        public Pad padPlaying;
        public PlayerBullet.BulletType bulletType;
        Game game = new Game();

        public int mutekiTimer = 0; // 残り無敵時間
        int animationCounter = 0; // アニメーションのカウンター
        public bool attackFlg = false;

        public float life = 30;           // プレイヤーのライフ
        public static float maxLife = 30;         // プレイヤーのライフの最大値
        public static float defencePoint = 1; // プレイヤーの防御力
        public static int level = 1;    // プレイヤーのレベル
        public static float expMax = 500; // プレイヤー次レベルに必要な経験値
        public static bool lifeSet = true; // ライフ初期値設定用フラグ

        int onCollisionEnemyNum = 0; // ぶつかった敵の種類
        int onCoolisionPlayerNum = 0; // 敵とぶつかったプレイヤー

        public int selectBulletindex = 0;

        public float diffusionCoolTime = PlayerBullet.diffusionCoolTimeMax; // 拡散弾リロードタイム
        public float trackingCoolTime = PlayerBullet.trackingCoolTimeMax; // 追尾弾リロードタイム
        public float everyDirectionCoolTime = PlayerBullet.everyDirectionCoolTimeMax; // 全方位弾リロードタイム

        public Player(BattlePhase battlePhase, Pad padPlaying, float x, float y)
            : base(battlePhase)
        {
            if (MainMenu.loadFlg == false)
            {
                maxLife = 30;
                defencePoint = 1;
                level = 1;
                expMax = 500;
            }

            bulletType = PlayerBullet.BulletType.Nomal;
            this.battlePhase = battlePhase;
            this.padPlaying = padPlaying;
            this.x = x;
            this.y = y;

            imageWidth = 60;
            imageHeight = 70;
            hitboxOffsetLeft = 20;
            hitboxOffsetRight = 14;
            hitboxOffsetTop = 14;
            hitboxOffsetBottom = 10;

            if (lifeSet == true)
            {
                life = maxLife;
                lifeSet = false;
            }
        }

        public override void Update()
        {
            if (battlePhase.stageClearFlginFlg == false)
                return;

            HandleInput();

            MoveX();
            MoveY();

            diffusionCoolTime--;
            trackingCoolTime--;
            everyDirectionCoolTime--;

            if (diffusionCoolTime <= 0)
                diffusionCoolTime = 0;
            if (trackingCoolTime <= 0)
                trackingCoolTime = 0;
            if (everyDirectionCoolTime <= 0)
                everyDirectionCoolTime = 0;

            mutekiTimer--;
            animationCounter++;
        }

        void HandleInput()
        {
            ChangeBullet();

            if (Input.GetButton((Pad)padPlaying, DX.PAD_INPUT_UP)) // ↑
            {
                vy = -MoveSpeed;
                direction = Direction.Up;
            }
            else if (Input.GetButton((Pad)padPlaying, DX.PAD_INPUT_LEFT)) // ←
            {
                vx = -MoveSpeed;
                direction = Direction.Left;
            }
            else if (Input.GetButton((Pad)padPlaying, DX.PAD_INPUT_DOWN)) // ↓
            {
                vy = MoveSpeed;
                direction = Direction.Down;
            }
            else if (Input.GetButton((Pad)padPlaying, DX.PAD_INPUT_RIGHT)) // →
            {
                vx = MoveSpeed;
                direction = Direction.Right;
            }

            else
            {
                vx = 0;
                vy = 0;
            }

            // 斜め移動も同じ速度になるように調整
            if (vx != 0 && vy != 0)
            {
                vx /= MyMath.Sqrt2;
                vy /= MyMath.Sqrt2;
            }

            // 攻撃処理
            if (Input.GetButtonDown((Pad)padPlaying, DX.PAD_INPUT_1))
            {
                attackFlg = true;

                if (bulletType == PlayerBullet.BulletType.Nomal)
                {
                    float bulletX = x + hitboxOffsetRight;
                    float bulletY = y + hitboxOffsetTop;

                    if (direction == Direction.Right)
                        battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Nomal, bulletX, bulletY, direction, 0 * MyMath.Deg2Rad));
                    if (direction == Direction.Left)
                        battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Nomal, bulletX, bulletY, direction, 180 * MyMath.Deg2Rad));
                    if (direction == Direction.Down)
                        battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Nomal, bulletX, bulletY, direction, 90 * MyMath.Deg2Rad));
                    if (direction == Direction.Up)
                        battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Nomal, bulletX, bulletY, direction, 270 * MyMath.Deg2Rad));
                }

                if (bulletType == PlayerBullet.BulletType.Diffusion)
                {
                    if (diffusionCoolTime <= 0)
                    {
                        float bulletX = x + imageWidth / 2;
                        float bulletY = y + imageWidth / 2;

                        if (direction == Direction.Right)
                        {
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, 0 * MyMath.Deg2Rad)); // 右
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, -15 * MyMath.Deg2Rad)); // 右上
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, +15 * MyMath.Deg2Rad)); // 右下
                        }
                        if (direction == Direction.Left)
                        {
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, 180 * MyMath.Deg2Rad)); // 左
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, (180 - 15) * MyMath.Deg2Rad)); // 右上
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, (180 + 15) * MyMath.Deg2Rad)); // 右下
                        }
                        if (direction == Direction.Down)
                        {
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, 90 * MyMath.Deg2Rad)); // 下
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, (90 - 15) * MyMath.Deg2Rad)); // 右上
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, (90 + 15) * MyMath.Deg2Rad)); // 右下
                        }
                        if (direction == Direction.Up)
                        {
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, 270 * MyMath.Deg2Rad)); // 上
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, (270 - 15) * MyMath.Deg2Rad)); // 右上
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Diffusion, bulletX, bulletY, direction, (270 + 15) * MyMath.Deg2Rad)); // 右下
                        }

                        diffusionCoolTime = PlayerBullet.diffusionCoolTimeMax;
                    }
                }

                if (bulletType == PlayerBullet.BulletType.Tracking)
                {
                    if (trackingCoolTime <= 0)
                    {
                        if (Map.enemyBases.Count != 0)
                        {
                            float bulletX = x + imageWidth / 2;
                            float bulletY = y + imageWidth / 2;

                            float maximumDist = 0; // 一番遠い距離
                            int nearEnemyIndex = 0; // 敵カウント

                            float farhestEnemyX; // 一番遠い敵のx座標
                            float farhestEnemyY; // 一番遠い敵のy座標

                            float enemyCount = battlePhase.enemyBases.Count;

                            for (int i = 0; i < enemyCount; i++)
                            {
                                float dx = battlePhase.enemyBases[i].x - x;
                                float dy = battlePhase.enemyBases[i].y - y;

                                float dist2 = dx * dx + dy * dy;
                                if (dist2 > maximumDist)
                                {
                                    nearEnemyIndex = i;
                                    maximumDist = dist2;
                                }
                            }

                            farhestEnemyX = battlePhase.enemyBases[nearEnemyIndex].x;
                            farhestEnemyY = battlePhase.enemyBases[nearEnemyIndex].y;

                            float angleToEnemy = MyMath.PointToPointAngle(x, y, farhestEnemyX, farhestEnemyY);

                            enemyDistanceX = (float)Math.Cos(angleToEnemy) * MoveSpeed;
                            enemyDistanceY = (float)Math.Sin(angleToEnemy) * MoveSpeed;

                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.Tracking, bulletX, bulletY, direction, angleToEnemy));
                        }

                        trackingCoolTime = PlayerBullet.trackingCoolTimeMax;
                    }
                }

                if (bulletType == PlayerBullet.BulletType.EveryDirection)
                {
                    if (everyDirectionCoolTime <= 0)
                    {
                        float bulletX = x + imageWidth / 2;
                        float bulletY = y + imageWidth / 2;

                        for (float angle = 0f; angle < 360f; angle += 10f)
                        {
                            battlePhase.gameObjects.Add(new PlayerBullet(battlePhase, this, PlayerBullet.BulletType.EveryDirection, bulletX, bulletY, direction, angle * MyMath.Deg2Rad));
                        }

                        everyDirectionCoolTime = PlayerBullet.everyDirectionCoolTimeMax;
                    }
                }

            }
            if (Input.GetButtonUp((Pad)padPlaying, DX.PAD_INPUT_1))
            {
                attackFlg = false;
            }
        }

        // 横の移動処理
        void MoveX()
        {
            x += vx;

            float left = GetLeft();
            float right = GetRight() - 0.01f;
            float top = GetTop();
            float middle = top + 32;
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

        void MoveY()
        {
            y += vy;

            float left = GetLeft();
            float right = GetRight() - 0.01f;
            float top = GetTop();
            float bottom = GetBottom() - 0.01f;

            if (battlePhase.map.IsWall(left, top) || // 左上が壁か？
                battlePhase.map.IsWall(right, top))   // 右上が壁か？
            {
                float wallBottom = top - top % Map.CellSize + Map.CellSize; // 天井のy座標
                SetTop(wallBottom); // プレイヤーの頭を天井に沿わす
                vy = 0; // 縦の移動速度を0に
            }
            else if (
                battlePhase.map.IsWall(left, bottom) ||
                battlePhase.map.IsWall(right, bottom))
            {
                float wallTop = bottom - bottom % Map.CellSize;
                SetBottom(wallTop);
                vy = 0;
            }
        }

        public override void Draw()
        {
            if (padPlaying == Pad.Key)
            {
                if (mutekiTimer <= 0 || mutekiTimer % 2 == 0)
                    Camera.DrawRectGraph(x, y, imageWidth * (animationCounter / 16 % 2), imageHeight, imageWidth, imageHeight, Image.player, direction, vx, vy, attackFlg);
            }
            if (padPlaying == Pad.One)
            {
                if (mutekiTimer <= 0 || mutekiTimer % 2 == 0)
                    Camera.DrawRectGraph(x, y, imageWidth * (animationCounter / 16 % 2), imageHeight, imageWidth, imageHeight, Image.player2, direction, vx, vy, attackFlg);
            }

            for (int i = 0; i < battlePhase.playingPad.Count; i++)
            {
                DX.DrawBox(100, 800 - i * 75, 500, 850 - i * 75, DX.GetColor(122, 122, 122), 1);
                DX.DrawBox(100, 800 - i * 75, 100 + (int)(400 * (Math.Round(battlePhase.players[i].life, MidpointRounding.AwayFromZero) / maxLife)), 850 - i * 75, DX.GetColor(0, 255, 110), 1);
                DX.SetFontSize(25);
                DX.DrawString(400, 800 - i * 75, Math.Round(battlePhase.players[i].life, MidpointRounding.AwayFromZero) + " / " + (int)maxLife, DX.GetColor(255, 255, 255));
            }

            DX.DrawString(20, 810, "Lv." + level, DX.GetColor(255, 255, 255));
        }

        public override void OnCollision(GameObject other)
        {
            if (other is HealingItem)
            {
                DX.PlaySoundMem(Music.heal_se, DX.DX_PLAYTYPE_BACK);
                life += 15;
                if (life > maxLife)
                    life = maxLife;
            }
        }

        public override void OnCollisionEnemy(EnemyBase other)
        {// 敵との当たり判定
            if ((Pad)padPlaying == Pad.Key)
            {
                onCoolisionPlayerNum = 0;
            }
            if ((Pad)padPlaying == Pad.One)
            {
                if (battlePhase.playingPad.Contains(Pad.Key))
                    onCoolisionPlayerNum = 1;
                else
                    onCoolisionPlayerNum = 0;
            }

            if (other is Enemy1)
            {
                onCollisionEnemyNum = 1;
                if (mutekiTimer <= 0)
                {
                    DX.PlaySoundMem(Music.oncollisionenemy_se, DX.DX_PLAYTYPE_BACK);
                    TakeDamage();
                }
            }
            if (other is EnemyBullet)
            {
                onCollisionEnemyNum = 2;
                if (mutekiTimer <= 0)
                {
                    DX.PlaySoundMem(Music.oncollisionbullet_se, DX.DX_PLAYTYPE_BACK);
                    TakeDamage();
                }
            }
            if (other is Enemy3)
            {
                onCollisionEnemyNum = 3;
                if (mutekiTimer <= 0)
                {
                    DX.PlaySoundMem(Music.oncollisionenemy_se, DX.DX_PLAYTYPE_BACK);
                    TakeDamage();
                }
            }
            if (other is Enemy4)
            {
                onCollisionEnemyNum = 4;
                if (mutekiTimer <= 0)
                {
                    DX.PlaySoundMem(Music.oncollisionenemy_se, DX.DX_PLAYTYPE_BACK);
                    TakeDamage();
                }
            }

            if (other is Boss1)
            {
                onCollisionEnemyNum = 50;
                if (mutekiTimer <= 0)
                {
                    DX.PlaySoundMem(Music.oncollisionenemy_se, DX.DX_PLAYTYPE_BACK);
                    TakeDamage();
                }
            }
            if (other is Boss2)
            {
                onCollisionEnemyNum = 51;
                if (mutekiTimer <= 0)
                {
                    DX.PlaySoundMem(Music.oncollisionenemy_se, DX.DX_PLAYTYPE_BACK);
                    TakeDamage();
                }
            }
        }

        void TakeDamage()
        {
            if (onCollisionEnemyNum == 1)
            {
                battlePhase.players[onCoolisionPlayerNum].life -= Enemy1.attackPoint / defencePoint;
                if (life > 0)
                    onCoolisionPlayerNum = 0;
            }
            else if (onCollisionEnemyNum == 2)
            {
                battlePhase.players[onCoolisionPlayerNum].life -= Enemy2.attackPoint / defencePoint;
                if (life > 0)
                    onCoolisionPlayerNum = 0;
            }
            else if (onCollisionEnemyNum == 3)
            {
                battlePhase.players[onCoolisionPlayerNum].life -= Enemy3.attackPoint / defencePoint;
                if (life > 0)
                    onCoolisionPlayerNum = 0;
            }
            else if (onCollisionEnemyNum == 4)
            {
                battlePhase.players[onCoolisionPlayerNum].life -= Enemy4.attackPoint / defencePoint;
                if (life > 0)
                    onCoolisionPlayerNum = 0;
            }
            else if (onCollisionEnemyNum == 50)
            {
                battlePhase.players[onCoolisionPlayerNum].life -= Boss1.attackPoint / defencePoint;
                if (life > 0)
                    onCoolisionPlayerNum = 0;
            }
            else if (onCollisionEnemyNum == 51)
            {
                battlePhase.players[onCoolisionPlayerNum].life -= Boss2.attackPoint / defencePoint;
                if (life > 0)
                    onCoolisionPlayerNum = 0;
            }

            if (battlePhase.players[onCoolisionPlayerNum].life <= 0)
            {
                // ライフが無くなったら死亡
                isDead = true;
                battlePhase.players.Remove(this);
                battlePhase.playingPad.Remove((Pad)onCoolisionPlayerNum);
            }
            else
            {
                // 無敵時間発動
                mutekiTimer = MutekiJikan;
            }
        }

        void ChangeBullet()
        {

            if (Input.GetButtonDown((Pad)padPlaying, DX.PAD_INPUT_4))
            {
                if (selectBulletindex <= 0)
                    return;
                selectBulletindex--;
            }
            if (Input.GetButtonDown((Pad)padPlaying, DX.PAD_INPUT_5))
            {
                if (selectBulletindex >= StandbyPhase.waponChoice.Count - 1)
                    return;

                selectBulletindex++;

            }

            bulletType = (PlayerBullet.BulletType)StandbyPhase.waponChoice[selectBulletindex];
        }
    }
}
