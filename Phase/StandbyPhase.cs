using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class StandbyPhase : Phase
    {
        public static int waponWindowSize = 170; // 装備ウィンドウサイズ
        public static int[] wapon = new int[] { Image.wapon_nomal }; // 装備リスト
        string[] waponTextDrawString; // 装備テキスト保存配列
        public static List<int> waponChoice = new List<int>(); // 選択済み装備リスト
        public int waponBarX = 80; // 装備選択バーX座標
        int textNum = 0; // 表示する装備テキスト番号
        bool nonWaponAlert = false; // 装備未選択フラグ

        int counter = 0; // 選択済点滅カウンター

        BattlePhase battlePhase;

        public StandbyPhase(Game game)
            : base(game)
        {
            battlePhase = new BattlePhase(game);

            WaponText.Update(); // WaponText読み込み

            waponTextDrawString = new string[] { WaponText.normal, WaponText.diffusion, WaponText.tracking, WaponText.everyDirection }; // 装備テキスト保存配列に読み込んだテキストを追加
        }

        public override void Update()
        {
            if (ResultPhase.diffusionAddFlg) // 拡散弾追加フラグがtrueだったら
            {
                if (wapon.Length != 2) // 装備配列の要素数が2でなかったら
                {
                    Array.Resize(ref wapon, wapon.Length + 1); // 装備配列の要素数を増やす
                    wapon[wapon.Length - 1] = Image.wapon_diffusion; // 装備配列に拡散弾を追加
                    ResultPhase.diffusionAddFlg = false; // 拡散弾追加フラグをfalse
                }
            }
            if (ResultPhase.trackingAddFlg) // 追尾弾追加フラグがtrueだったら
            {
                if (wapon.Length != 3) // 装備配列の要素数が3でなかったら
                {
                    Array.Resize(ref wapon, wapon.Length + 1); // 装備配列の要素数を増やす
                    wapon[wapon.Length - 1] = Image.wapon_tracking; // 装備配列に追尾弾を追加
                    ResultPhase.trackingAddFlg = false; // 追尾弾追加フラグをfalse
                }
            }
            if (ResultPhase.everyDirectionAddFlg) // 全方位弾追加フラグがtrueだったら
            {
                if (wapon.Length != 4) // 装備配列の要素数が4でなかったら
                {
                    Array.Resize(ref wapon, wapon.Length + 1); // 装備配列の要素数を増やす
                    wapon[wapon.Length - 1] = Image.wapon_everyDirection; // 装備配列に全方位弾を追加
                    ResultPhase.everyDirectionAddFlg = false; // 全方位弾追加フラグをfalse
                }
            }

            if (nonWaponAlert) // 装備未選択フラグがtrue
            {
                if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1)) // 選択に戻るボダン押された
                {
                    nonWaponAlert = false; // 装備未選択フラグをfalse
                }
                return;
            }

            if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_3)) // Cキーが押されたら
            {
                if (waponChoice.Count != 0) // 装備が一つでも選択されていたら
                {
                    DX.PlaySoundMem(Music.enter_se, DX.DX_PLAYTYPE_BACK); // 決定SEを鳴らす
                    DX.StopSoundMem(Music.standbyphase_bgm); // スタンバイ画面BGＭを止める
                    if (BattlePhase.stageNum == 4 || BattlePhase.stageNum == 10) // ボス戦の場合
                        DX.PlaySoundMem(Music.buttle_boss_bgm, DX.DX_PLAYTYPE_LOOP); // ボス戦BGM再生
                    else 
                        DX.PlaySoundMem(Music.buttle_normal_bgm, DX.DX_PLAYTYPE_LOOP); // 通常戦闘BGM再生
                    Play.ChangePhase(new BattlePhase(game)); // 戦闘画面へ
                }
                else // 装備が一つも選択されていなかったら
                {
                    DX.PlaySoundMem(Music.alert_se, DX.DX_PLAYTYPE_BACK); // 警告SEを鳴らす
                    nonWaponAlert = true; // 装備未選択フラグをtrueに
                    if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1)) // 装備未選択フラグがfalseになったら
                    {
                        Update(); // 戻る
                    }
                }
            }

            if (wapon.Length >= 2) // 装備配列要素数が2以上だったら
            {
                if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_RIGHT)) // 右押されたら
                {
                    DX.PlaySoundMem(Music.cursor_se, DX.DX_PLAYTYPE_BACK); // カーソルSE鳴らす
                    if (waponBarX != 80 + (170 * (wapon.Length - 1))) // 装備選択バーが右端になかったら
                    {
                        waponBarX = waponBarX + waponWindowSize; // バーを右に移動
                        textNum++; // 装備テキストを次のテキストに
                    }
                }
                if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_LEFT)) // 左押されたら
                {
                    DX.PlaySoundMem(Music.cursor_se, DX.DX_PLAYTYPE_BACK); // カーソルSE鳴らす
                    if (waponBarX != 80) // 装備選択バーが左端になかったら
                    {
                        waponBarX = waponBarX - waponWindowSize; // バーを左に移動
                        textNum--; // 装備テキストを前のテキストに
                    }
                }
            }

            if (waponChoice.Count <= 2) // 選択済み装備の数が2以下だったら
            {
                if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1)) // Zキーが押されたら
                {
                    DX.PlaySoundMem(Music.enter_se, DX.DX_PLAYTYPE_BACK); // 決定SEを鳴らす
                    for (int i = 0; i < wapon.Length; i++) // 現在選択できる装備数for文
                    {
                        if (waponBarX == 80 + waponWindowSize * i && waponChoice.Contains(i) == false) // 装備選択バーの場所にある装備が選択されていなかったら
                        {
                            waponChoice.Add(i); // 選択済み装備に追加
                        }
                    }
                }
            }

            if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_2)) // Xキーが押されたら
            {
                DX.PlaySoundMem(Music.cancel_se, DX.DX_PLAYTYPE_BACK); // キャンセルSEを鳴らす
                waponChoice.RemoveAt(waponChoice.Count - 1); // 直前に追加された装備を消す
            }

            counter++; // 選択済点滅カウンターを足す
        }

        public override void Draw()
        {
            DX.DrawGraph(0, 0, Image.standby); // スタンバイ画面背景
            DX.SetFontSize(25); // フォントサイズ

            DX.DrawGraph(waponBarX, 260, Image.wapon_bar); // 選択バー

            for (int i = 0; i < wapon.Length; i++) // 選択できる装備数for文
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 150); // アルファブレンド
                DX.DrawGraph(80 + waponWindowSize * i, 100, Image.wapon); // 装備ウィンドウ
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0); // ブレンドモードを通常に
                DX.DrawGraph(80 + waponWindowSize * i, 100, wapon[i]); // 装備アイコン
            }

            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 150); // アルファブレンド
            DX.DrawGraph(80, 300, Image.wapon_text_window); // 装備テキストウィンドウ
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0); // ブレンドモードを通常に

            DX.DrawString(100, 350, waponTextDrawString[textNum], DX.GetColor(255, 255, 255)); // 装備テキスト

            for (int i = 0; i < waponChoice.Count; i++) // 選択済み装備数for文
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 150); // アルファブレンド
                DX.DrawGraph(80 + waponWindowSize * i, 700, Image.wapon); // 装備ウィンドウ
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0); // ブレンドモードを通常に
                DX.DrawGraph(80 + waponWindowSize * i, 700, wapon[waponChoice[i]]); // 選択済みの装備アイコン
            }

            for (int i = 0; i < wapon.Length; i++) // 選択できる装備数for文
            {
                if (waponChoice.Contains(i)) // 選択済み装備リストにi番目があったら
                {
                    DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 150); // アルファブレンド
                    DX.DrawBox(80 + waponWindowSize * i, 100, 234 + waponWindowSize * i, 255, DX.GetColor(0, 0, 0), 1); // 暗い画像を選択できる装備アイコンに重ねる
                    DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0); // ブレンドモードを通常に
                    if (counter % 30 < 20) // 点滅
                        DX.DrawString(115 + waponWindowSize * i, 160, "選択中", DX.GetColor(255, 255, 255)); // 選択中
                }
            }

            if (nonWaponAlert) // 未選択フラグがtrue
            {
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 200); // ブレンドモードを通常に
                DX.DrawBox(0, 0, 1600, 900, DX.GetColor(0, 0, 0), 1); // 画面全体を暗く
                DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0); // ブレンドモードを通常に
                DX.DrawString(500, 400, "※※ 1つ以上武器を選択してください。 ※※", DX.GetColor(255, 255, 255)); // ※※ 1つ以上武器を選択してください。 ※※
            }
        }
    }
}
