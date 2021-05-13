using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class Title : Scene
    {
        int count = 0; // PRESS START BUTTON の点滅

        public static bool debugModeFlg = false;  // ★★デバッグ用★★ デバッグモードフラグ

        public override void Update()
        {
            if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1))
            {
                DX.PlaySoundMem(Music.enter_se, DX.DX_PLAYTYPE_BACK);
                Game.ChangeScene(new MainMenu());
            }

            // ★★デバッグ用★★ デバッグモードフラグ
            if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_8))
            {
                if (debugModeFlg)
                    debugModeFlg = false;
                else
                    debugModeFlg = true;
            }
            // ★★★★
        }
        public override void Draw()
        {
            DX.DrawGraph(0, 0, Image.title_bg);

            DX.DrawGraph(350, 82, Image.title_logo);
            if (count % 30 < 20)
            {
                DX.DrawGraph(530, 552, Image.pressstartbutton);
            }
            count++;

            // ★★デバッグ用★★ デバッグモード表示
            if (Title.debugModeFlg)
            {
                DX.SetFontSize(10);
                DX.DrawString(0, 0, "デバッグモード,Wキーで通常モードへ", DX.GetColor(255, 255, 255));
                DX.SetFontSize(25);
            }
            // ★★★★
        }
    }
}
