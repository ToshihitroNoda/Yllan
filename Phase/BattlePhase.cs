using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class BattlePhase : Phase
    {
        public static int testCounter;
        public int playingNum = 1;
        public int waponBarButtleX = 1440;

        public Map map;
        public PlayerBullet.BulletType type = new PlayerBullet.BulletType();

        public List<Player> players;
        public List<Pad> playingPad;
        public List<GameObject> gameObjects = new List<GameObject>();
        public List<EnemyBase> enemyBases = new List<EnemyBase>();
        public List<PlayerBullet> playerBullets = new List<PlayerBullet>();

        public static int bulletChoiceCount;
        public static int stageNum = 1;

        public static int playTimeMin = 0; // バトル開始からのプレイ時間（分）
        public static int playTimeSec = 0; // バトル開始からのプレイ時間（秒）
        public static float min; // 表示用タイマー（分）
        public static float sec; // 表示用タイマー（秒） 
        int resultTimer = 240; // ResultPhase移行までのタイマー
        int gameOverTimer = 540;

        bool gameStartFlg = false;
        bool stageClearFlgDraw = true;
        bool stageClearFlg = false;
        public bool stageClearFlginFlg = true;
        bool gameOverFlg = false;
        bool gameOverFlginFlg = true;

        public BattlePhase(Game game)
            : base(game)
        {
            this.game = game;

            players = new List<Player>();
            playingPad = new List<Pad>();

            map = new Map(this, "Map" + stageNum);

            bulletChoiceCount = StandbyPhase.waponChoice.Count - 1;

            if (MainMenu.loadFlg == false)
                stageNum = 1;

        }

        public override void Update()
        {
            for (int i = 0; i < Input.MaxPadNum; i++)
            {
                if (Input.GetButtonDown((Pad)i, DX.PAD_INPUT_3))
                {
                    stageClearFlgDraw = false;
                    if (playingPad.Contains((Pad)i))
                    {
                        break;
                    }
                    Player player = new Player(this, (Pad)i, 100 + 100 * i, 100);
                    players.Add(player);
                    playingPad.Add((Pad)i);
                    playingNum = players.Count;
                    gameObjects.Add(player);
                    gameStartFlg = true;
                    Player.lifeSet = true;

                    players[0].bulletType = (PlayerBullet.BulletType)StandbyPhase.waponChoice.Last();

                    break;
                }
            }

            int gameObjectCount = gameObjects.Count; // ループの前に個数を取得しておく
            for (int i = 0; i < gameObjectCount; i++)
            {
                gameObjects[i].StorePostionAndHitBox(); // 1フレーム前の情報を記憶させる
                if (this.players.Count >= 1)
                    gameObjects[i].Update();
            }

            int enemyBaseCount = enemyBases.Count;
            for (int i = 0; i < enemyBaseCount; i++)
            {
                enemyBases[i].StorePostionAndHitBox(); // 1フレーム前の情報を記憶させる
                if (this.players.Count >= 1)
                    enemyBases[i].Update();

            }

            for (int i = 0; i < players.Count; i++)
            {
                type = players[i].bulletType;
            }

            // オブジェクト同士の衝突を判定
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject a = gameObjects[i];

                for (int j = i + 1; j < gameObjects.Count; j++)
                {
                    // オブジェクトAが死んでたらこのループは終了
                    if (a.isDead) break;

                    GameObject b = gameObjects[j];

                    // オブジェクトBが死んでたらスキップ
                    if (b.isDead) continue;

                    // オブジェクトAとBが重なっているか？
                    if (MyMath.RectRectIntersect(
                        a.GetLeft(), a.GetTop(), a.GetRight(), a.GetBottom(),
                        b.GetLeft(), b.GetTop(), b.GetRight(), b.GetBottom()))
                    {
                        a.OnCollision(b);
                        b.OnCollision(a);
                    }
                }

                for (int k = 0; k < enemyBases.Count; k++)
                {
                    EnemyBase e = enemyBases[k];
                    //e.currentHitObjects = new List<GameObject>();

                    // エネミーEが死んでたらこのループは終了
                    if (e.isDead) break;

                    // オブジェクトAが死んでたらスキップ
                    if (a.isDead) continue;

                    // エネミーEとオブジェクトAが重なっているか？
                    if (MyMath.RectRectIntersect(
                        e.GetLeft(), e.GetTop(), e.GetRight(), e.GetBottom(),
                        a.GetLeft(), a.GetTop(), a.GetRight(), a.GetBottom()))
                    {
                        e.OnCollision(a);
                        a.OnCollisionEnemy(e);
                        if (type == PlayerBullet.BulletType.Tracking)
                            enemyBases[k].onCollisionTrackingFlg = false;
                    }

                    e.prevHitObjects = new List<GameObject>(e.currentHitObjects.ToArray());
                }
            }

            for (int i = 0; i < enemyBases.Count; i++)
            {
                EnemyBase a = enemyBases[i];

                for (int j = i + 1; j < enemyBases.Count; j++)
                {
                    // オブジェクトAが死んでたらこのループは終了
                    if (a.isDead) break;

                    EnemyBase b = enemyBases[j];

                    // オブジェクトBが死んでたらスキップ
                    if (b.isDead) continue;

                    // オブジェクトAとBが重なっているか？
                    if (MyMath.RectRectIntersect(
                        a.GetLeft(), a.GetTop(), a.GetRight(), a.GetBottom(),
                        b.GetLeft(), b.GetTop(), b.GetRight(), b.GetBottom()))
                    {
                        a.OnCollisionEnemy(b);
                        b.OnCollisionEnemy(a);
                    }
                }
            }

            gameObjects.RemoveAll(go => go.isDead);
            enemyBases.RemoveAll(eb => eb.isDead);

            if (Map.enemyBases.Count != 0 && players.Count >= 1)
            {
                playTimeMin++;
                playTimeSec++;
                if (playTimeSec / 60f >= 60)
                {
                    playTimeSec = 0;
                }
            }

            if (players.Count > 0)
            {
                Camera.LookAt(players[0].x, players[0].y); // プレイヤーに追従するようにカメラを移動
            }

            if (stageClearFlginFlg)
            {
                if (stageNum == 1 || stageNum == 2 || stageNum == 4 || stageNum == 6 || stageNum == 8 || stageNum == 9 || stageNum == 10)
                {
                    if (Map.enemyBases.Count <= 0)
                    {
                        DX.StopSoundMem(Music.buttle_normal_bgm);
                        DX.StopSoundMem(Music.buttle_boss_bgm);
                        DX.PlaySoundMem(Music.stageclear_bgm, DX.DX_PLAYTYPE_LOOP);
                        enemyBases.Clear();
                        stageClearFlg = true;
                        stageClearFlginFlg = false;
                    }
                }
                else
                {
                    if (min >= 1 || Map.enemyBases.Count <= 0)
                    {
                        DX.StopSoundMem(Music.buttle_normal_bgm);
                        DX.StopSoundMem(Music.buttle_boss_bgm);
                        DX.PlaySoundMem(Music.stageclear_bgm, DX.DX_PLAYTYPE_LOOP);
                        enemyBases.Clear();
                        stageClearFlg = true;
                        stageClearFlginFlg = false;
                    }
                }
            }

            if (gameOverFlginFlg)
            {
                if (players.Count <= 0 && gameStartFlg)
                {
                    DX.StopSoundMem(Music.buttle_normal_bgm);
                    DX.StopSoundMem(Music.buttle_boss_bgm);
                    DX.PlaySoundMem(Music.gameover_bgm, DX.DX_PLAYTYPE_BACK);
                    gameOverFlg = true;
                    gameOverFlginFlg = false;
                }
            }

            if (resultTimer <= 0 || gameOverTimer <= 0) // ResultPhaseへ
            {// 色々初期化
                playTimeMin = 0;
                playTimeSec = 0;
                Map.count = 0;
                Map.maxCount = -1;
                Map.enemyBases.Clear();
                StandbyPhase.waponChoice.Clear();
                gameStartFlg = false;
                stageClearFlgDraw = true;
                stageClearFlg = false;
                stageClearFlginFlg = true;
                gameOverFlg = false;
                gameOverFlginFlg = true;

                if (resultTimer <= 0)
                {
                    resultTimer = 240;
                    DX.StopSoundMem(Music.stageclear_bgm);
                    DX.PlaySoundMem(Music.result_bgm, DX.DX_PLAYTYPE_LOOP);
                    Play.ChangePhase(new ResultPhase(game, players[0]));
                }

                if (gameOverTimer <= 0)
                {
                    gameOverTimer = 540;
                    DX.StopSoundMem(Music.buttle_normal_bgm);
                    DX.StopSoundMem(Music.buttle_boss_bgm);
                    DX.PlaySoundMem(Music.standbyphase_bgm, DX.DX_PLAYTYPE_LOOP);
                    Play.ChangePhase(new StandbyPhase(game));
                }
            }
        }

        public override void Draw()
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 122);
            DX.DrawGraph(0, 0, Image.buttle_Back);
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
            map.DrawTerrain();

            foreach (EnemyBase eb in enemyBases)
            {
                eb.Draw();
                eb.DrawHitBox();
            }
            foreach (GameObject go in gameObjects)
            {
                go.Draw();
                go.DrawHitBox();
            }

            DX.DrawString(70, 100, "Enemies  ", DX.GetColor(255, 255, 255));
            DX.DrawString(200, 100, Map.count + " / " + Map.maxCount, DX.GetColor(255, 255, 255));
            min = playTimeMin / 3600;
            sec = playTimeSec / 60f;
            DX.DrawString(410, 100, "Time  ", DX.GetColor(255, 255, 255));
            DX.DrawString(500, 100, min.ToString("00") + ".", DX.GetColor(255, 255, 255));
            DX.DrawString(540, 100, sec.ToString("00.00"), DX.GetColor(255, 255, 255));

            for (int i = 0; i < playingPad.Count; i++)
            {
                if (players.Count - 1 < i)
                    break;

                int offset = StandbyPhase.waponChoice.Count - 1 - players[i].selectBulletindex;
                DX.DrawGraph(1440 - 170 * offset, 860 - i * (StandbyPhase.waponWindowSize + 30), Image.wapon_bar);

                for (int j = 0; j < StandbyPhase.waponChoice.Count; j++)
                {
                    DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 122);
                    DX.DrawGraph(1440 - 170 * j, 700 - i * 200, Image.wapon);
                    DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
                    StandbyPhase.waponChoice.Reverse();
                    DX.DrawGraph(1440 - 170 * j, 700 - i * 200, StandbyPhase.wapon[StandbyPhase.waponChoice[j]]);
                    if (StandbyPhase.wapon[StandbyPhase.waponChoice[j]] == Image.wapon_diffusion)
                    {
                        int diffusionX = 1440 - 170 * j;
                        if (players[i].diffusionCoolTime > 0)
                            DX.DrawGraph(diffusionX, 700 - i * 200, Image.coolTimeShadow);
                        DX.DrawString(diffusionX, 700 - i * 200, players[i].diffusionCoolTime.ToString(), DX.GetColor(255, 255, 255));
                    }
                    if (StandbyPhase.wapon[StandbyPhase.waponChoice[j]] == Image.wapon_tracking)
                    {
                        int trackingX = 1440 - 170 * j;
                        if (players[i].trackingCoolTime > 0)
                            DX.DrawGraph(trackingX, 700 - i * 200, Image.coolTimeShadow);
                        DX.DrawString(trackingX, 700 - i * 200, players[i].trackingCoolTime.ToString(), DX.GetColor(255, 255, 255));
                    }
                    if (StandbyPhase.wapon[StandbyPhase.waponChoice[j]] == Image.wapon_everyDirection)
                    {
                        int everyDirectionX = 1440 - 170 * j;
                        if (players[i].everyDirectionCoolTime > 0)
                            DX.DrawGraph(everyDirectionX, 700 - i * 200, Image.coolTimeShadow);
                        DX.DrawString(everyDirectionX, 700 - i * 200, players[i].everyDirectionCoolTime.ToString(), DX.GetColor(255, 255, 255));
                    }
                    StandbyPhase.waponChoice.Reverse();
                }
            }

            if (stageClearFlg)
            {
                if (stageNum == 3 || stageNum == 5 || stageNum == 7)
                    playTimeSec--;
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200);
                DX.DrawBox(0, 0, 1600, 900, DX.GetColor(0, 0, 0), 1);
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
                DX.DrawString(700, 400, "STAGE CLEAR !!", DX.GetColor(255, 255, 255));
                resultTimer--;
            }

            if (players.Count <= 0 && gameStartFlg)
            {
                if (gameOverFlg)
                {
                    DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200);
                    DX.DrawBox(0, 0, 1600, 900, DX.GetColor(0, 0, 0), 1);
                    DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
                    DX.DrawString(700, 400, "GAME OVER !!", DX.GetColor(255, 255, 255));
                    gameOverTimer--;
                }
            }

            if (stageClearFlgDraw)
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200);
                DX.DrawBox(0, 0, 1600, 900, DX.GetColor(0, 0, 0), 1);
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
                if (stageNum == 1 || stageNum == 2 || stageNum == 4 || stageNum == 6 || stageNum == 8 || stageNum == 9 || stageNum == 10)
                {
                    DX.DrawString(600, 400, "クリア目標 : 敵の殲滅をせよ！", DX.GetColor(255, 255, 255));
                    DX.DrawString(700, 750, "Cキーで戦闘開始", DX.GetColor(255, 255, 255));
                }
                else
                {
                    DX.DrawString(550, 400, "クリア目標 : 敵の攻撃から1分耐えきれ！", DX.GetColor(255, 255, 255));
                    DX.DrawString(700, 750, "Cキーで戦闘開始", DX.GetColor(255, 255, 255));
                }
            }

            //DX.DrawString(Screen.Width / 2, Screen.Height / 2, testCounter.ToString(), DX.GetColor(255, 255, 255));

        }
    }
}
