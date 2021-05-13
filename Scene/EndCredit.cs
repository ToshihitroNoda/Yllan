using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;
using MyLib;

namespace Yılan
{
    public class EndCredit : Scene
    {
        int scrollSpeed = 2;
        int creditY;

        bool backTitleFlg = false;

        bool massegeFlg = false;
        int drawWidth;
        int massege = 0;
        string[] clearMassege = new string[] {
            "“Yılan”を最後まで遊んでいただきありがとうございました！",
            "クリア後、メインメニューでNew Gameを選択すると初期ステータスで最初から、",
            "Continueを選択するとこれまでのステータスを引き継いで最初から",
            "プレイすることができます。",
            "タイムアタックや無双などお好きな遊び方で楽しんでみてください"
        };

        public EndCredit()
        {
            drawWidth = DX.GetDrawStringWidth(clearMassege[massege], -1);

            creditY = 1600;
        }

        public override void Update()
        {
            creditY -= scrollSpeed;

            if (creditY <= -5280)
            {
                DX.StopSoundMem(Music.endcredit_bgm);
                massegeFlg = true;

                if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_1))
                {
                    DX.PlaySoundMem(Music.enter_se, DX.DX_PLAYTYPE_BACK);
                    if (massege < 4)
                    {
                        massege++;
                        drawWidth = DX.GetDrawStringWidth(clearMassege[massege], -1);
                    }
                    else
                        backTitleFlg = true;
                }
            }

            if (backTitleFlg)
            {
                DX.PlaySoundMem(Music.title_bgm, DX.DX_PLAYTYPE_LOOP);
                Game.ChangeScene(new Title());
            }

        }

        public override void Draw()
        {
            DX.DrawBox(0, 0, 1600, 900, DX.GetColor(0, 0, 0), 1);
            DX.DrawGraph(306, creditY, Image.endcredit);

            if (massegeFlg)
                DX.DrawString((Screen.Width - drawWidth) / 2, 450, clearMassege[massege], DX.GetColor(255, 255, 255));
        }
    }
}
