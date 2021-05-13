using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace Yılan
{
    public static class Music
    {
        public static int title_bgm;             // タイトルBGM

        public static int adv_buttle_normal_bgm; //ADVパート戦場通常時BGM
        public static int adv_buttle_boss_bgm;   // ADVパート戦場ボス前BGM
        public static int adv_commandroom_bgm;   // ADVパート指令室BGM
        public static int adv_narration_bgm;     // ADVパートナレーションBGM
        public static int adv_recollection_bgm;  // ADVパート回想BGM

        public static int standbyphase_bgm;      // スタンバイ画面BGM

        public static int buttle_normal_bgm;     // 戦闘通常時BGM
        public static int buttle_boss_bgm;       // 戦闘ボスBGM
        public static int stageclear_bgm;        // ステージクリアBGM
        public static int gameover_bgm;          // ゲームオーバーBGM

        public static int result_bgm;            // リザルト画面BGM

        public static int endcredit_bgm;         // エンドクレジットBGM

        public static int cursor_se;             // カーソル移動SE
        public static int enter_se;              // 決定SE
        public static int alert_se;              // 警告SE
        public static int cancel_se;             // 選択解除SE
        public static int levelup_se;            // レベルアップSE

        public static int bullet_se;             // 弾発射SE
        public static int heal_se;               // 回復SE
        public static int oncollisionbullet_se;  // 敵弾命中SE
        public static int oncollisionenemy_se;   // 敵衝突SE

        // 読み込み
        public static void Load()
        {
            title_bgm             = DX.LoadSoundMem("Resource/Music/title_bgm.wav");

            adv_buttle_normal_bgm = DX.LoadSoundMem("Resource/Music/adv_buttle_normal_bgm.wav");
            adv_buttle_boss_bgm   = DX.LoadSoundMem("Resource/Music/adv_buttle_boss_bgm.wav");
            adv_commandroom_bgm   = DX.LoadSoundMem("Resource/Music/adv_commandroom_bgm.wav");
            adv_narration_bgm     = DX.LoadSoundMem("Resource/Music/adv_narration_bgm.wav");
            adv_recollection_bgm  = DX.LoadSoundMem("Resource/Music/adv_recollection_bgm.wav");

            standbyphase_bgm      = DX.LoadSoundMem("Resource/Music/standbyphase_bgm.wav");

            buttle_normal_bgm     = DX.LoadSoundMem("Resource/Music/buttle_normal_bgm.wav");
            buttle_boss_bgm       = DX.LoadSoundMem("Resource/Music/buttle_boss_bgm.wav");
            stageclear_bgm        = DX.LoadSoundMem("Resource/Music/stageclear_bgm.wav");
            gameover_bgm          = DX.LoadSoundMem("Resource/Music/gameover_bgm.wav");

            result_bgm            = DX.LoadSoundMem("Resource/Music/result_bgm.wav");

            endcredit_bgm         = DX.LoadSoundMem("Resource/Music/endcredit_bgm.wav");

            cursor_se             = DX.LoadSoundMem("Resource/Music/cursor_se.wav");
            enter_se              = DX.LoadSoundMem("Resource/Music/enter_se.wav");
            alert_se              = DX.LoadSoundMem("Resource/Music/alert_se.wav");
            cancel_se             = DX.LoadSoundMem("Resource/Music/cancel_se.wav");
            levelup_se            = DX.LoadSoundMem("Resource/Music/levelup_se.wav");
            

            bullet_se             = DX.LoadSoundMem("Resource/Music/bullet_se.wav");
            heal_se               = DX.LoadSoundMem("Resource/Music/heal_se.wav");
            oncollisionbullet_se  = DX.LoadSoundMem("Resource/Music/oncollisionbullet_se.wav");
            oncollisionenemy_se   = DX.LoadSoundMem("Resource/Music/oncollisonenemy_se.wav");
        }
    }
}
