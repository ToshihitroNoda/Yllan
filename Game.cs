using System.Collections.Generic;
using System;
using DxLibDLL;
using MyLib;

namespace Yılan
{
    public class Game
    {
        static Scene scene;

        public void Init()
        {
            Input.Init(); // 入力クラスの初期化
            MyRandom.Init(); // ランダムクラスの初期化
            Image.Load(); // 画像の読み込み
            Music.Load(); // 音楽の読み込み

            DX.PlaySoundMem(Music.title_bgm, DX.DX_PLAYTYPE_LOOP); // タイトルBGM再生
            scene = new Title(); // タイトルシーンへ
        }

        public void Update()
        {
            Input.Update(); // 入力クラスの更新
            scene.Update(); // シーンの更新

            if (Input.GetButtonDown((Pad)0,DX.PAD_INPUT_9)) //ESC押されたら
                Environment.Exit(0); // ゲーム終了
        }

        public void Draw()
        {
            scene.Draw(); // シーンの描画
        }
        public static void ChangeScene(Scene newScene)
        {
            scene = newScene; // シーン切り替え
        }
    }
}
