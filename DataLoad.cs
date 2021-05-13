using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DxLibDLL;

namespace Yılan
{
    // データロード
    public class DataLoad : Scene
    {
        string[] loadData = File.ReadAllLines("savedata.dat"); // セーブデータを一行ずつ読み込み配列に格納
        float[] floatLoadData; // ロードデータ保存用配列

        public override void Update()
        {
            floatLoadData = loadData.Select(float.Parse).ToArray(); // ロードデータ保存用配列にセーブデータを保存ん

            Load(); // ロード

            Game.ChangeScene(new Play()); // プレイシーンへ
        }

        // ロード
        void Load()
        {
            Player.level                           = (int)floatLoadData[0];
            Player.maxLife                         = floatLoadData[1];
            Player.expMax                          = floatLoadData[2];
            PlayerBullet.normalLevel               = (int)floatLoadData[3];
            PlayerBullet.normalAttackpoint         = (int)floatLoadData[4];
            PlayerBullet.normalRange               = (int)floatLoadData[5];
            PlayerBullet.diffusionLevel            = (int)floatLoadData[6];
            PlayerBullet.diffusionAttackpoint      = (int)floatLoadData[7];
            PlayerBullet.diffusionRange            = (int)floatLoadData[8];
            PlayerBullet.diffusionCoolTimeMax      = (int)floatLoadData[9];
            PlayerBullet.trackingLevel             = (int)floatLoadData[10];
            PlayerBullet.trackingAttackpoint       = (int)floatLoadData[11];
            PlayerBullet.trackingCoolTimeMax       = floatLoadData[12];
            PlayerBullet.everyDirectionLevel       = (int)floatLoadData[13];
            PlayerBullet.everyDirectionAttackpoint = (int)floatLoadData[14];
            PlayerBullet.everyDirectionRange       = (int)floatLoadData[15];
            PlayerBullet.everyDirectionCoolTimeMax = floatLoadData[16];
            Player.defencePoint			   = floatLoadData[17];
            ResultPhase.normalLvUpPLv              = (int)floatLoadData[18];
            ResultPhase.diffusionLvUpPLv           = (int)floatLoadData[19];
            ResultPhase.trackingLvUpPLv            = (int)floatLoadData[20];
            ResultPhase.everyDirectionLvUpPLv      = (int)floatLoadData[21];
            ResultPhase.acquiredExp                = floatLoadData[22];
            // メッセージが最後まで行っていたら
            if ((int)floatLoadData[23] == 11)
            {// メッセージ番号とステージ番号を初期化
                AdvPhase.massegeNum                = 1;
                BattlePhase.stageNum               = 1;
            }
            else
            {// 普通にロード
                AdvPhase.massegeNum                = (int)floatLoadData[23];
                BattlePhase.stageNum               = (int)floatLoadData[24];
            }

            // プレイヤーのレベルに応じで弾開放フラグを立てる
            if (Player.level >= 3)
                ResultPhase.diffusionAddFlg        = true;
            if (Player.level >= 6)
                ResultPhase.trackingAddFlg         = true;
            if (Player.level >= 9)
                ResultPhase.everyDirectionAddFlg   = true;
        }

        public override void Draw()
        {
        }
    }
}
