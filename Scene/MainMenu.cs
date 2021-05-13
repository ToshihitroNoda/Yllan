using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MyLib;
using DxLibDLL;

namespace Yılan
{
    public class MainMenu : Scene
    {
        int cursorY;
        public static bool loadFlg;

        public MainMenu()
        {
            if (File.Exists("savedata.dat"))
                cursorY = 500;
            else
                cursorY = 400;

            loadFlg = true;
        }

        public override void Update()
        {
            if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_DOWN))
            {
                DX.PlaySoundMem(Music.cursor_se, DX.DX_PLAYTYPE_BACK);
                if (cursorY != 700)
                {
                    cursorY += 100;
                    if (!File.Exists("savedata.dat") && cursorY == 500)
                    {
                        cursorY = 600;
                    }
                }
            }
            if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_UP))
            {
                DX.PlaySoundMem(Music.cursor_se, DX.DX_PLAYTYPE_BACK);
                if (cursorY != 400)
                {
                    cursorY -= 100;
                    if (!File.Exists("savedata.dat") && cursorY == 500)
                    {
                        cursorY = 400;
                    }
                }
            }
            if (Input.GetButtonDown((Pad)0,DX.PAD_INPUT_1))
            {
                DX.PlaySoundMem(Music.enter_se, DX.DX_PLAYTYPE_BACK);
                if (cursorY == 400)
                {
                    loadFlg = false;
                    DX.StopSoundMem(Music.title_bgm);
                    Game.ChangeScene(new Play());
                }
                else if (cursorY == 500)
                {
                    loadFlg = true;
                    DX.StopSoundMem(Music.title_bgm);
                    Game.ChangeScene(new DataLoad());
                }
                else if (cursorY == 600)
                    Game.ChangeScene(new Title());
                else
                    Environment.Exit(0);
            }

            // ★★デバッグ用★★ セーブデータ削除
            //if (Input.GetButtonDown((Pad)0, DX.PAD_INPUT_7) && File.Exists("savedata.dat"))
            //{
            //    File.SetAttributes("savedata.dat", FileAttributes.Normal);
            //    File.Delete("savedata.dat");
            //    cursorY = 400;
            //}
            // ★★★★
        }

        public override void Draw()
        {
            DX.DrawGraph(0, 0, Image.title_bg);

            DX.ChangeFontType(DX.DX_FONTTYPE_EDGE);

            DX.SetFontSize(120);
            DX.DrawString(50, 50, "MainMenu", DX.GetColor(255, 255, 255));

            DX.SetFontSize(50);
            DX.DrawString(1200, 400, "New Game", DX.GetColor(255, 255, 255));
            if (File.Exists("savedata.dat"))
                DX.DrawString(1200, 500, "Continue", DX.GetColor(255, 255, 255));
            DX.DrawString(1200, 600, "Title", DX.GetColor(255, 255, 255));
            DX.DrawString(1200, 700, "Quit", DX.GetColor(255, 255, 255));

            DX.SetFontSize(15);
            if (cursorY == 400)
                DX.DrawString(1450, 880, "新しく始める", DX.GetColor(255, 255, 255));
            if (cursorY == 500)
                DX.DrawString(1450, 880, "続きから始める", DX.GetColor(255, 255, 255));
            if (cursorY == 600)
                DX.DrawString(1450, 880, "タイトルへ戻る", DX.GetColor(255, 255, 255));
            if (cursorY == 700)
                DX.DrawString(1450, 880, "終了する", DX.GetColor(255, 255, 255));
            DX.SetFontSize(25);

            DX.DrawGraph(1100, cursorY, Image.cursor);

            // ★★デバッグ用★★ デバッグモード表示
            if (Title.debugModeFlg)
            {
                DX.SetFontSize(10);
                DX.DrawString(0, 0, "デバッグモード", DX.GetColor(255, 255, 255));
                DX.SetFontSize(25);
            }
            // ★★★★
        }
    }
}
