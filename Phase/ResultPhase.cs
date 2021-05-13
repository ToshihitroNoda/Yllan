using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;
using MyLib;

namespace Yılan
{
    public class ResultPhase : Phase
    {
        Player player;

        public static int defeatedEnemyCount = 0; // 敵撃破数
        int defeatedEnemyCountDraw = 0; // 敵撃破数（表示用）
        public static float exp = 0; // 獲得経験値
        public static float remainingExp = 0; // 残り必要経験値
        public static float acquiredExp = 0; // 獲得済み経験値

        int cursorX = 930; // カーソルX座標

        bool levelUpFlg = false; // レベルアップフラグ
        bool newWaponFlg = false; // 新装備追加フラグ

        bool normalLevelUpFlg = false; // 通常弾レベルアップフラグ
        bool diffusionLevelUpFlg = false; // 拡散弾レベルアップフラグ
        bool trackingLevelUpFlg = false; // 追尾弾レベルアップフラグ
        bool everyDirectionLevelUpFlg = false; // 全方向弾レベルアップフラグ

        public static bool diffusionAddFlg = false; // 拡散弾追加フラグ
        public static bool trackingAddFlg = false; // 追尾弾追加フラグ
        public static bool everyDirectionAddFlg = false; // 全方位弾追加フラグ

        public static bool defeatedEnemyCountDrawFlg; // 倒した敵数表示フラグ

        public static int normalLvUpPLv = 10; // 通常弾がレベルアップする為のプレイヤーのレベル
        public static int diffusionLvUpPLv = 12; // 拡散弾がレベルアップする為のプレイヤーのレベル
        public static int trackingLvUpPLv = 14; // 追尾弾がレベルアップする為のプレイヤーのレベル
        public static int everyDirectionLvUpPLv = 16; // 全方位弾がレベルアップする為のプレイヤーのレベル

        List<String> saveData; // セーブデータリスト
        bool saveDataWriting; // セーブデータ書き込みフラグ

        public ResultPhase(Game game, Player player)
            : base(game)
        {
            this.game = game;
            this.player = player;

            if (MainMenu.loadFlg == false)
            {
                normalLvUpPLv = 10;
                diffusionLvUpPLv = 12;
                trackingLvUpPLv = 14;
                everyDirectionLvUpPLv = 16;
                acquiredExp = 0;
            }

            defeatedEnemyCountDrawFlg = false;

            // セーブデータ格納リスト
            saveData = new List<String>();

            AdvPhase.massegeNum++;
            BattlePhase.stageNum++;
        }

        public override void Update()
        {
            // buttlePhaseで倒した敵の数 * 100 EXP獲得。
            // Lv++ で life += 6, Lv % 3 = 0 でbulletType追加 &  defencePoint++、16以降は攻撃力アップ
            // Lv++ でexpMax *= 1.1

            if (defeatedEnemyCountDrawFlg == false)
            {
                defeatedEnemyCountDraw = defeatedEnemyCount;
                defeatedEnemyCountDrawFlg = true;
            }
            if (newWaponFlg)
            {
                if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1))
                {
                    newWaponFlg = false;
                }
                return;
            }

            if (normalLevelUpFlg || diffusionLevelUpFlg || trackingLevelUpFlg || everyDirectionLevelUpFlg)
            {
                if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1))
                {
                    normalLevelUpFlg = false;
                    diffusionLevelUpFlg = false;
                    trackingLevelUpFlg = false;
                    everyDirectionLevelUpFlg = false;
                }
                return;
            }

            if (levelUpFlg)
            {
                if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1))
                {
                    levelUpFlg = false;
                }
                return;
            }

            defeatedEnemyCount = 0; // 連続レベルアップ時用撃破数初期化

            remainingExp = Player.expMax - acquiredExp;

            if (exp >= remainingExp)
            {
                DX.PlaySoundMem(Music.levelup_se, DX.DX_PLAYTYPE_BACK);

                acquiredExp = exp + acquiredExp - Player.expMax;
                exp = 0;

                Player.level++; // レベルアップ
                Player.maxLife += 6; // ライフ増加
                Player.expMax *= 1.1f; // 最大経験値増加

                levelUpFlg = true;

                if (Player.level % 3 == 0 && Player.level <= 9)
                {
                    newWaponFlg = true;
                }

                if (Player.level > 9)
                {
                    if (Player.level == normalLvUpPLv) // 通常弾レベルアップ
                    {
                        PlayerBullet.normalLevel++;
                        PlayerBullet.normalAttackpoint *= 2;
                        PlayerBullet.normalRange *= 2;

                        normalLvUpPLv += 8;

                        normalLevelUpFlg = true;
                    }
                    if (Player.level == diffusionLvUpPLv) // 拡散弾レベルアップ
                    {
                        PlayerBullet.diffusionLevel++;
                        PlayerBullet.diffusionAttackpoint *= 2;
                        PlayerBullet.diffusionRange *= 2;
                        PlayerBullet.diffusionCoolTimeMax /= 2;

                        diffusionLvUpPLv += 8;

                        diffusionLevelUpFlg = true;
                    }
                    if (Player.level == trackingLvUpPLv) // 追尾弾レベルアップ
                    {
                        PlayerBullet.trackingLevel++;
                        PlayerBullet.trackingAttackpoint *= 2;
                        PlayerBullet.trackingCoolTimeMax /= 2;

                        trackingLvUpPLv += 8;

                        trackingLevelUpFlg = true;
                    }
                    if (Player.level == everyDirectionLvUpPLv) // 全方位弾レベルアップ
                    {
                        PlayerBullet.everyDirectionLevel++;
                        PlayerBullet.everyDirectionAttackpoint *= 2;
                        PlayerBullet.everyDirectionRange *= 2;
                        PlayerBullet.everyDirectionCoolTimeMax /= 2;

                        everyDirectionLvUpPLv += 8;

                        everyDirectionLevelUpFlg = true;
                    }
                }

                if (Player.level % 4 == 0)
                {
                    Player.defencePoint *= 1.1f; // 防御力増加
                }
            }

            else
            {
                acquiredExp += exp;
                if (saveDataWriting == false)
                    DataAutoSave();
            }

            if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_RIGHT) && cursorX != 1230)
            {
                DX.PlaySoundMem(Music.cursor_se, DX.DX_PLAYTYPE_BACK);
                cursorX = 1230;
            }
            else if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_LEFT) && cursorX != 930)
            {
                DX.PlaySoundMem(Music.cursor_se, DX.DX_PLAYTYPE_BACK);
                cursorX = 930;
            }

            if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1) && cursorX == 930)
            {
                exp = 0;
                DX.PlaySoundMem(Music.enter_se, DX.DX_PLAYTYPE_BACK);
                DX.StopSoundMem(Music.result_bgm);
                Play.ChangePhase(new AdvPhase(game));
            }
            if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1) && cursorX == 1230)
            {
                DX.PlaySoundMem(Music.enter_se, DX.DX_PLAYTYPE_BACK);
                exp = 0;
                DX.StopSoundMem(Music.result_bgm);
                DX.PlaySoundMem(Music.title_bgm, DX.DX_PLAYTYPE_LOOP);
                Game.ChangeScene(new Title());
            }
        }

        // オートセーブ
        void DataAutoSave()
        {
            if (File.Exists("savedata.dat"))
            {
                // ファイル形式を通常に
                File.SetAttributes("savedata.dat", FileAttributes.Normal);

                // ファイルサイズを0(データ全削除)に
                using (var fileStream = new FileStream("savedata.dat", FileMode.Open))
                {
                    fileStream.SetLength(0);
                }
            }
            // セーブデータ配列にセーブする変数追加
            saveData.Add(Player.level.ToString());
            saveData.Add(Player.maxLife.ToString());
            saveData.Add(Player.expMax.ToString());
            saveData.Add(PlayerBullet.normalLevel.ToString());
            saveData.Add(PlayerBullet.normalAttackpoint.ToString());
            saveData.Add(PlayerBullet.normalRange.ToString());
            saveData.Add(PlayerBullet.diffusionLevel.ToString());
            saveData.Add(PlayerBullet.diffusionAttackpoint.ToString());
            saveData.Add(PlayerBullet.diffusionRange.ToString());
            saveData.Add(PlayerBullet.diffusionCoolTimeMax.ToString());
            saveData.Add(PlayerBullet.trackingLevel.ToString());
            saveData.Add(PlayerBullet.trackingAttackpoint.ToString());
            saveData.Add(PlayerBullet.trackingCoolTimeMax.ToString());
            saveData.Add(PlayerBullet.everyDirectionLevel.ToString());
            saveData.Add(PlayerBullet.everyDirectionAttackpoint.ToString());
            saveData.Add(PlayerBullet.everyDirectionRange.ToString());
            saveData.Add(PlayerBullet.everyDirectionCoolTimeMax.ToString());
            saveData.Add(Player.defencePoint.ToString());
            saveData.Add(normalLvUpPLv.ToString());
            saveData.Add(diffusionLvUpPLv.ToString());
            saveData.Add(trackingLvUpPLv.ToString());
            saveData.Add(everyDirectionLvUpPLv.ToString());
            saveData.Add(acquiredExp.ToString());
            saveData.Add(AdvPhase.massegeNum.ToString());
            saveData.Add(BattlePhase.stageNum.ToString());

            saveDataWriting = false;

            File.WriteAllLines("savedata.dat", saveData); // ファイルに1行ずつ書き込み
            File.SetAttributes("savedata.dat", FileAttributes.ReadOnly); // ファイルを読み取り専用に変更
            saveDataWriting = true;
        }

        public override void Draw()
        {
            DX.DrawGraph(0, 0, Image.buttleField_back);
            DX.DrawGraph(900, 0, Image.player_standillust_buttle);

            DX.SetFontSize(120);
            DX.DrawString(50, 50, "RESULT", DX.GetColor(255, 255, 255));
            DX.SetFontSize(80);
            DX.DrawString(200, 250, "TIME", DX.GetColor(255, 255, 255));
            DX.DrawString(550, 360, BattlePhase.min.ToString("00") + ".", DX.GetColor(255, 255, 255));
            DX.DrawString(660, 360, BattlePhase.sec.ToString("00.00"), DX.GetColor(255, 255, 255));
            DX.DrawString(200, 500, "DEFEATED ENEMY", DX.GetColor(255, 255, 255), DX.GetColor(0, 0, 0));
            DX.DrawString(700, 610, "" + defeatedEnemyCountDraw, DX.GetColor(255, 255, 255));
            DX.DrawString(200, 700, "NEXT LEVEL", DX.GetColor(255, 255, 255));
            DX.DrawBox(200, 800, 800, 850, DX.GetColor(122, 122, 122), 1);
            if (acquiredExp > Player.expMax)
                DX.DrawBox(200, 800, 800, 850, DX.GetColor(0, 255, 110), 1);
            else
                DX.DrawBox(200, 800, 200 + (int)(700 * (Math.Round(acquiredExp, MidpointRounding.AwayFromZero) / Math.Round(Player.expMax, MidpointRounding.AwayFromZero))), 850, DX.GetColor(0, 255, 110), 1);
            DX.SetFontSize(30);
            if (acquiredExp > Player.expMax)
                DX.DrawString(650, 800, Math.Round(Player.expMax, MidpointRounding.AwayFromZero) + " / " + Math.Round(Player.expMax, MidpointRounding.AwayFromZero), DX.GetColor(255, 255, 255));
            else
                DX.DrawString(650, 800, Math.Round(acquiredExp, MidpointRounding.AwayFromZero) + " / " + Math.Round(Player.expMax, MidpointRounding.AwayFromZero), DX.GetColor(255, 255, 255));
            DX.SetFontSize(80);
            DX.DrawString(1000, 700, "NEXT", DX.GetColor(255, 255, 255), DX.GetColor(0, 0, 0));
            DX.DrawString(1300, 700, "TITLE", DX.GetColor(255, 255, 255), DX.GetColor(0, 0, 0));
            DX.DrawGraph(cursorX, 710, Image.cursor);

            if (levelUpFlg)
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200);
                DX.DrawBox(0, 0, 1600, 900, DX.GetColor(0, 0, 0), 1);
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
                DX.DrawString(500, 400, "LEVEL UP !!! ", DX.GetColor(255, 255, 255));
                DX.DrawString(700, 500, "LEVEL : " + Player.level, DX.GetColor(255, 255, 255));
                DX.DrawString(700, 590, "最大HP : " + Player.maxLife, DX.GetColor(255, 255, 255));
            }

            if (newWaponFlg)
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200);
                DX.DrawBox(0, 0, 1600, 900, DX.GetColor(0, 0, 0), 1);
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
                if (Player.level == 3)
                {
                    DX.DrawString(600, 400, "拡散弾 GET!! ", DX.GetColor(255, 255, 255));
                    diffusionAddFlg = true;
                }
                if (Player.level == 6)
                {
                    DX.DrawString(600, 400, "追尾弾 GET!! ", DX.GetColor(255, 255, 255));
                    trackingAddFlg = true;
                }
                if (Player.level == 9)
                {
                    DX.DrawString(600, 400, "全方位弾 GET!! ", DX.GetColor(255, 255, 255));
                    everyDirectionAddFlg = true;
                }
            }

            if (normalLevelUpFlg || diffusionLevelUpFlg || trackingLevelUpFlg || everyDirectionLevelUpFlg)
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200);
                DX.DrawBox(0, 0, 1600, 900, DX.GetColor(0, 0, 0), 1);
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
                if (normalLevelUpFlg)
                {
                    DX.DrawString(500, 400, "通常弾 LEVEL UP!! ", DX.GetColor(255, 255, 255));
                }
                if (diffusionLevelUpFlg)
                {
                    DX.DrawString(500, 400, "拡散弾 LEVEL UP!! ", DX.GetColor(255, 255, 255));
                }
                if (trackingLevelUpFlg)
                {
                    DX.DrawString(500, 400, "追尾弾 LEVEL UP!! ", DX.GetColor(255, 255, 255));
                }
                if (everyDirectionLevelUpFlg)
                {
                    DX.DrawString(500, 400, "全方位弾 LEVEL UP!! ", DX.GetColor(255, 255, 255));
                }
            }
        }
    }
}
