using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyLib;
using DxLibDLL;
using System.IO;

namespace Yılan
{
    public class AdvPhase : Phase
    {
        public int massege = 0; // 現在のメッセージ番号
        public bool massegeEnd = false; // メッセージが終了したかのフラグ
        public static int massegeNum = 1; // 読み込むファイル番号

        int massegeY = 630; // メッセージのy座標
        int newLineFlg = 0; // 改行フラグ
        int newLineString1; // 2行目
        int newLineString2; // 3行目
        int newLineString3; // 4行目
        int massegeY1; // 1行目y座標
        int massegeY2; // 2行目y座標
        int massegeY3; // 3行目y座標

        static bool playerNormalStandIllustFlg = false; // セルビア通常時立ち絵フラグ
        static bool playerButtleStandIllustFlg = false; // セルビア戦闘立ち絵フラグ
        static bool commanderStandIllustFlg = false; // 司令立ち絵フラグ
        static bool miriaStandIllustFlg = false; // ミリア立ち絵フラグ

        // 立ち絵x、y座標
        static int playerX = 0; // セルビアX座標
        static int playerY = 0; // セルビアY座標
        static int commanderX = 0; // 司令X座標
        static int commanderY = 0; // 司令Y座標
        static int miriaX = 0; // ミリアX座標
        static int miriaY = 0; // ミリアY座標

        int musichandle; // 音楽ハンドル

        static bool bg_CommandRoom = false; // 指令室背景
        static bool bg_Room = false; // 部屋背景
        static bool bg_ButtleField = false; // 戦場背景

        string charcterName = ""; // キャラクター名変数

        bool massegeSkipFlg = false; // メッセージをスキップするかのフラグ

        List <string> massegeList = new List<string>(); // メッセージ表示用リスト

        // キャラクター名表示メッセージリスト
        string[] characterNameList = new string[]
        {
		"ナレーション", "セルビア", "司令",
		"アナウンス", "ミリア", "？？？"
	    };

        // 立ち絵表示メッセージリスト
        string[] characterStansIllustList = new string[]
        {
		"セルビア通常立ち絵", "セルビア戦闘立ち絵", "司令立ち絵",
		"ミリア立ち絵" , "セルビア通常立ち絵消し", "セルビア戦闘立ち絵消し",
		"司令立ち絵消し",  "ミリア立ち絵消し"
	    };

        // 立ち絵フラグリスト
        bool[] StandIllustFlgList = new bool[]
        {
            playerNormalStandIllustFlg, 
	        playerButtleStandIllustFlg, 
		    commanderStandIllustFlg, 
		    miriaStandIllustFlg
        };

        // キャラクター立ち絵表示メッセージ座標リスト
        string[] characterCoordinateList = new string[]
        {	
            "セルビアX" ,"セルビアY", "司令X" ,
		    "司令Y" ,"ミリアX" , "ミリアY"
	    };

        // 立ち絵座標リスト
        int[] CoordinateList = new int[]
        {
            playerX, playerY,
		    commanderX, commanderY,
		    miriaX, miriaY
        };

        // 背景表示メッセージリスト
        string[] bgList = new string[]
        {
		"指令室背景", "部屋背景", "戦場背景",
		"指令室背景消し", "部屋背景消し", "戦場背景消し"
	    };

        // 背景表示フラグリスト
        bool[] bgFlgList = new bool[]
        {
            bg_CommandRoom, bg_Room, bg_ButtleField
        };

        // 音楽再生メッセージリスト
        string[] musicList = new string[]
        {
		"bg1", "bg2", "bg3",
		"bg4", "bg5", "bgstop"
	    };

        // 音楽ハンドルリスト
        int[] musicHandleList = new int[]
        {
            Music.adv_narration_bgm, Music.adv_commandroom_bgm, Music.adv_recollection_bgm,
		    Music. adv_buttle_normal_bgm, Music.adv_buttle_boss_bgm
        };

        public AdvPhase(Game game)
            : base(game)
        {
            massege = 0;
            massegeEnd = false;

            if (MainMenu.loadFlg == false)
                massegeNum = 1;

            Load();
        }

        void Load()
        {
            string filePath = "Resource/ADVMassege/advMassege" + massegeNum + ".csv"; // ファイルパス指定

            string[] lines = File.ReadAllLines(filePath); // ファイルを行ごとに読み込み

            for (int i = 0; i < lines.Count(); i++) // 行数分for文
            {
                string[] splitted = lines[i].Split(new char[] { ',' }); // カンマ区切りで文字読み込み
                for (int j = 0; j < splitted.Length; j++) // カンマ区切りで読み込んだ文字列数分for文
                {
                    if (splitted[j] != "") // 文字列に文字が含まれていたら
                    massegeList.Add(splitted[j]); // 表示用リストに追加
                }
            }
        }

        public override void Update()
        {
            for (int i = 0; i < Input.MaxPadNum; i++)
            {
                if (Input.GetButtonDown((Pad)i, DX.PAD_INPUT_1))
                {
                    if (newLineFlg >= 1)
                        break;
                    massege++;
                }
            }
            for (int i = 0; i < Input.MaxPadNum; i++)
            {
                if (Input.GetButtonDown((Pad)i, DX.PAD_INPUT_3))
                {
                    massegeEnd = true;
                }
            }

            if (massegeEnd)
            {
                DX.StopSoundMem(musichandle);
                if (massegeNum != 11)
                    DX.PlaySoundMem(Music.standbyphase_bgm, DX.DX_PLAYTYPE_LOOP);
                Play.ChangePhase(new StandbyPhase(game));
            }

            // ****キャラクター名表示****
            for (int i = 0; i < characterNameList.Count(); i++)
            {
                // csvにキャラクター名があったら
                if (massegeList[massege] == characterNameList[i])
                {
                    massegeSkipFlg = true; // メッセージスキップフラグ
                    massege++; // メッセージスキップ
                    if (characterNameList[i] == "ナレーション") // "ナレーション"の場合は""なのでifで分ける
                        charcterName = "";
                    else // それ以外は配列からキャラクター名引っ張ってくる
                        charcterName = characterNameList[i];
                }
            }
            // ********

            // ****キャラクター立ち絵表示****
            for (int i = 0; i < characterStansIllustList.Count(); i++)
            {
                // csvに立ち絵表示メッセージあったら
                if (massegeList[massege] == characterStansIllustList[i])
                {
                    massegeSkipFlg = true; // メッセージスキップ
                                           // 表示
                    if (i < characterStansIllustList.Count() / 2)
                    {
                        massege++; // メッセージスキップ
                        StandIllustFlgList[i] = true; // Draw用にフラグを立てる
                    }
                    // 消し
                    else
                    {
                        massege++; // メッセージスキップ
                        StandIllustFlgList[i - characterStansIllustList.Count() / 2] = false; // Draw用にフラグを消す
                    }
                }
            }
            // ********

            // ****キャラクター立ち絵表示座標****
            for (int i = 0; i < characterCoordinateList.Count(); i++)
            {
                // csvに座標があったら
                if (massegeList[massege] == characterCoordinateList[i])
                {
                    massegeSkipFlg = true; // メッセージスキップ
                    massege++; // メッセージスキップ
                    CoordinateList[i] = int.Parse(massegeList[massege]); // 座標を代入
                    massege++; // メッセージスキップ
                }
            }
            // ********

            // ****背景表示****
            for (int i = 0; i < bgList.Count(); i++)
            {
                // csvに背景表示メッセージがあったら
                if (massegeList[massege] == bgList[i])
                {
                    massegeSkipFlg = true; // メッセージスキップ
                                           // 表示
                    if (i < bgList.Count() / 2)
                    {
                        massege++; // メッセージスキップ
                        bgFlgList[i] = true; //Draw用にフラグを立てる
                    }
                    // 消し
                    else
                    {
                        massege++; // メッセージスキップ
                        bgFlgList[i - bgList.Count() / 2] = false; //Draw用にフラグを消す
                    }
                }
            }
            // ********

            // ****音楽再生****
            for (int i = 0; i < musicList.Count(); i++)
            {
                // csvに音楽再生メッセージがあったら
                if (massegeList[massege] == musicList[i])
                {
                    massegeSkipFlg = true; // メッセージスキップ
                                           // "bgstop"以外だったら
                    if (i < musicList.Count() - 1)
                    {
                        massege++; // メッセージスキップ
                        DX.StopSoundMem(musichandle); // 前に再生されている音楽を止める
                        DX.PlaySoundMem(musicHandleList[i], DX.DX_PLAYTYPE_LOOP); // メッセージの音楽を再生
                        musichandle = musicHandleList[i]; // 次に再生する時止める用にハンドルを保存
                    }
                    // "bgstop"だったら
                    else
                    {
                        massege++; // メッセージスキップ
                        DX.StopSoundMem(musichandle); // 音楽を止める
                    }
                }
            }
            // ********

            // ****改行処理****
            if (massegeList[massege] == "改行" || massegeList[massege] == "改行初期化")
            {
                massegeSkipFlg = true;
            }

            if (massegeList[massege] == "改行")
            {
                massege++;
                massegeY += 30;

                newLineFlg++;

                if (newLineFlg == 1)
                {
                    newLineString1 = massege;
                    massegeY1 = massegeY - 30;
                }
                if (newLineFlg == 2)
                {
                    newLineString2 = massege;
                    massegeY2 = massegeY - 30;
                }
                if (newLineFlg == 3)
                {
                    newLineString3 = massege;
                    massegeY3 = massegeY - 30;
                }
                if (massegeList[massege + 1] != "改行初期化")
                {
                    massege++;
                }
            }
            if (massege != massegeList.Count() - 1)
            {
                if (massegeList[massege + 1] == "改行初期化")
                {
                    if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1))
                        massege++;
                }
            }
            if (massegeList[massege] == "改行初期化")
            {
                massege++;
                massegeY = 630;
                newLineFlg = 0;
            }        
            //********

        }

        public override void Draw()
        {
            if (bgFlgList[0])
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200);
                DX.DrawGraph(0, 0, Image.commandroom_bg);
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
            }
            if (bgFlgList[1])
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200);
                DX.DrawGraph(0, 0, Image.room_bg);
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
            }
            if (bgFlgList[2])
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200);
                DX.DrawGraph(0, 0, Image.buttleField_back);
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
            }

            if (StandIllustFlgList[0])
                DX.DrawGraph(CoordinateList[0], CoordinateList[1], Image.player_standillust_normal);
            if (StandIllustFlgList[1])
                DX.DrawGraph(CoordinateList[0], CoordinateList[1], Image.player_standillust_buttle);
            if (StandIllustFlgList[2])
                DX.DrawGraph(CoordinateList[2], CoordinateList[3], Image.commander_standillust);
            if (StandIllustFlgList[3])
                DX.DrawGraph(CoordinateList[4], CoordinateList[5], Image.miria_standillust);

            DX.DrawGraph(0, 0, Image.massege_window);

            DX.SetFontSize(25);

            DX.DrawString(30, 560, charcterName, DX.GetColor(255, 255, 255));

            if (newLineFlg <= 0)
            {
                if (massegeSkipFlg)
                {
                    massegeSkipFlg = false;
                    return;
                }
                DX.DrawString(100, massegeY, massegeList[massege], DX.GetColor(255, 255, 255));
            }
            else
            {
                DX.DrawString(100, massegeY1, massegeList[newLineString1], DX.GetColor(255, 255, 255));
                if (newLineFlg >= 2)
                {
                    DX.DrawString(100, massegeY2, massegeList[newLineString2], DX.GetColor(255, 255, 255));
                    if (newLineFlg >= 3)
                    {
                        DX.DrawString(100, massegeY3, massegeList[newLineString3], DX.GetColor(255, 255, 255));
                    }
                }
            }
            if (massege > massegeList.Count() - 2)
            {
                massegeEnd = true;
            }
            DX.SetFontSize(15);
            DX.DrawString(1300, 800, " Cキーでスキップ ", DX.GetColor(255, 255, 255));
            DX.SetFontSize(25);
        }
    }
}
