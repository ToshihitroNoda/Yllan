using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace Yılan
{
    public static class Image
    {
        public static int player;                    // プレイヤー
        public static int player2;                   // プレイヤー2
        public static int playerbullet;              // 自機弾

        public static int Enemy1;                    // 敵1
        public static int Enemy2;                    // 敵2
        public static int Enemy3;                    // 敵3
        public static int Enemy4;                    // 敵4
        public static int Boss1;                     // ボス1
        public static int Boss2;                     // ボス2
        public static int EnemyBullet;               // 敵弾

        public static int item1;                     // 回復アイテム

        public static int[] mapchip;                 // マップチップ

        public static int player_standillust_normal; // プレイヤー通常時立ち絵
        public static int player_standillust_buttle; // プレイヤー戦闘時立ち絵
        public static int miria_standillust;         // ミリア立ち絵
        public static int commander_standillust;     // 司令官立ち絵

        public static int title_bg;                  // タイトル画面背景
        public static int title_logo;                // タイトルロゴ
        public static int pressstartbutton;          // PRESS START BUTTON

        public static int commandroom_bg;            // 指令室背景
        public static int room_bg;                   // 部屋背景
        public static int buttleField_back;          // 戦場背景
        public static int massege_window;            // メッセージウィンドウ

        public static int standby;                   // スタンバイ画面背景

        public static int buttle_Back;               // 戦闘背景

        public static int wapon;                     // 装備背景
        public static int coolTimeShadow;            // クールタイム用暗い重ね
        public static int wapon_nomal;               // 通常弾イラスト
        public static int wapon_diffusion;           // 拡散弾イラスト
        public static int wapon_tracking;            // 追尾弾イラスト
        public static int wapon_everyDirection;      // 全方位弾イラスト
        public static int wapon_bar;                 // 装備選択バー
        public static int wapon_text_window;         // 装備テキストウィンドウ
        public static int cursor;                    // カーソル

        public static int endcredit;                 // エンドクレジット

        // 読み込み
        public static void Load()
        {
            player                    = DX.LoadGraph("Resource/Image/player_1.png");
            player2                   = DX.LoadGraph("Resource/Image/player_2.png");
            playerbullet              = DX.LoadGraph("Resource/Image/player_bullet.png");

            Enemy1                    = DX.LoadGraph("Resource/Image/zako0.png");
            Enemy2                    = DX.LoadGraph("Resource/Image/zako1.png");
            Enemy3                    = DX.LoadGraph("Resource/Image/zako2.png");
            Enemy4                    = DX.LoadGraph("Resource/Image/zako3.png");
            Boss1                     = DX.LoadGraph("Resource/Image/boss1.png");
            Boss2                     = DX.LoadGraph("Resource/Image/boss2.png");
            EnemyBullet               = DX.LoadGraph("Resource/Image/enemy_bullet_16.png");

            item1                     = DX.LoadGraph("Resource/Image/item1.png");

            mapchip = new int[4];
            DX.LoadDivGraph("Resource/Image/mapchip.png", 4, 4, 1, 32, 32, mapchip);

            player_standillust_normal = DX.LoadGraph("Resource/Image/player_standillust_normal.png");
            player_standillust_buttle = DX.LoadGraph("Resource/Image/player_standillust_buttle.png"); 
            miria_standillust         = DX.LoadGraph("Resource/Image/miria_standillust.png");
            commander_standillust     = DX.LoadGraph("Resource/Image/commander_standillust.png");

            title_bg                  = DX.LoadGraph("Resource/Image/title_bg.jpg");
            title_logo                = DX.LoadGraph("Resource/Image/title_logo.png");
            pressstartbutton          = DX.LoadGraph("Resource/Image/pressstartbutton.png");

            commandroom_bg            = DX.LoadGraph("Resource/Image/commandroom_bg.jpg");
            room_bg                   = DX.LoadGraph("Resource/Image/room_bg.jpg");
            massege_window            = DX.LoadGraph("Resource/Image/massege_window.png");

            standby                   = DX.LoadGraph("Resource/Image/standby.jpg");

            buttle_Back               = DX.LoadGraph("Resource/Image/buttle_Back.jpg");
            buttleField_back          = DX.LoadGraph("Resource/Image/buttleField_back.jpg");

            wapon                     = DX.LoadGraph("Resource/Image/wapon.png");
            coolTimeShadow            = DX.LoadGraph("Resource/Image/coolTimeShadow.png");
            wapon_nomal               = DX.LoadGraph("Resource/Image/wapon_normal.png");
            wapon_diffusion           = DX.LoadGraph("Resource/Image/wapon_diffusion.png");
            wapon_tracking            = DX.LoadGraph("Resource/Image/wapon_tracking.png");
            wapon_everyDirection      = DX.LoadGraph("Resource/Image/wapon_everyDirection.png");
            wapon_bar                 = DX.LoadGraph("Resource/Image/wapon_bar.png");
            wapon_text_window         = DX.LoadGraph("Resource/Image/wapon_text_window.png");
            cursor                    = DX.LoadGraph("Resource/Image/cursor.png");

            endcredit                 = DX.LoadGraph("Resource/Image/endcredit.png");
        }
    }
}