using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class Play : Scene
    {
        static Phase phase;
        MainMenu mainMenu = new MainMenu();
        public Play()
        {
            phase = new AdvPhase(game);
        }
        public override void Update()
        {
            phase.Update();
        }
        public override void Draw()
        {
            phase.Draw();

            // ★★デバッグ用★★ デバッグモード表示
            if (Title.debugModeFlg)
            {
                DX.SetFontSize(10);
                DX.DrawString(0, 0, "デバッグモード,Wキーで戦闘スキップ", DX.GetColor(255, 255, 255));
                DX.SetFontSize(25);
            }
            // ★★★★
        }
        public static void ChangePhase(Phase newPhase)
        {
            phase = newPhase;
        }
    }
}
