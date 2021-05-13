using DxLibDLL;

namespace MyLib
{
    // パッド番号
    public enum Pad
    {
        Key = 0,
        One,
    }

    // 入力クラス
    public static class Input
    {
        public const int MaxPadNum = 2; // 最大パッド数

        static int[] prevStates; // 1フレーム前の状態
        static int[] currentStates; // 現在の状態

        //★ついに発見【DXコントローラバグ検知】初回プッシュが被ればDXバグで同一コントローラ
        static int[] firstPushTiming;
        static int timing = 0;
        static public bool isBugCheckMode = false; //バグチェックを開始するか

        //コントローラのボタンの初回プッシュを検知し
        //他コントローラと初回タイミングが完全被りかチェック
        //★初回タイミングが完全被りなら【怪しい】重複コントローラとみなし判定処理スルー
        public static bool ControllerBugCheck(Pad pad, int buttonId)
        {   // 今は押されていて、かつ1フレーム前は押されていない場合
            bool buttonDown = ((currentStates[(int)pad] & buttonId) & ~(prevStates[(int)pad] & buttonId)) != 0;
            //if (isBugCheckMode == false) return false; 
            //コントローラの初回ボタンプッシュを検知
            if (buttonDown && firstPushTiming[(int)pad] == 0)
            {
                for (int i = 0; i < MaxPadNum; i++)
                {   //全コントローラ初回プッシュ被りがないかチェック完全同時は怪しいので-1
                    bool isDown = ((currentStates[i] & buttonId) & ~(prevStates[i] & buttonId)) != 0;
                    if ((Pad)i != pad && isDown ////初回プッシュ被り検出
                     && (timing == firstPushTiming[i] || firstPushTiming[i] == 0))
                    {   //初回プッシュのタイミング被りは-1フラグを記録
                        firstPushTiming[i] = -1;
                    }
                }
                if (firstPushTiming[(int)pad] != -1)
                    firstPushTiming[(int)pad] = timing;//初回プッシュを記録
            }

            if (firstPushTiming[(int)pad] == -1) return true; //DXのコントローラ被りバグ！
            else return false; //コントローラ被りOK!
        }

        // 初期化。最初に1回だけ呼んでください。
        public static void Init()
        {
            firstPushTiming = new int[MaxPadNum];
            prevStates = new int[MaxPadNum];
            currentStates = new int[MaxPadNum];
        }

        // 最新の入力状況に更新する処理。
        // 毎フレームの最初に（ゲームの処理より先に）呼んでください。
        public static void Update()
        {
            currentStates.CopyTo(prevStates, 0); // 前フレームの最新の情報を1つ前の情報とする

            currentStates[(int)Pad.Key] = DX.GetJoypadInputState(DX.DX_INPUT_KEY);
            currentStates[(int)Pad.One] = DX.GetJoypadInputState(DX.DX_INPUT_PAD1);

            isBugCheckMode = true;
            timing++; //タイミングのカウントを+1
            if (timing == int.MaxValue) timing = 0; // long型最大値になったら0に戻す
        }

        // ボタンが押されているか？
        public static bool GetButton(Pad pad, int buttonId)
        {   //初回プッシュコントローラバグチェック
            if (ControllerBugCheck(pad, buttonId)) return false;

            // 今ボタンが押されているかどうかを返却
            return (currentStates[(int)pad] & buttonId) != 0;
        }

        // ボタンが押された瞬間か？
        public static bool GetButtonDown(Pad pad, int buttonId)
        {   //初回プッシュコントローラバグチェック
            if (ControllerBugCheck(pad, buttonId)) return false;

            // 今は押されていて、かつ1フレーム前は押されていない場合はtrueを返却
            return ((currentStates[(int)pad] & buttonId) & ~(prevStates[(int)pad] & buttonId)) != 0;
        }

        // ボタンが離された瞬間か？
        public static bool GetButtonUp(Pad pad, int buttonId)
        {   //初回プッシュでコントローラバグチェック
            if (ControllerBugCheck(pad, buttonId)) return false;

            // 1フレーム前は押されていて、かつ今は押されている場合はtrueを返却
            return ((prevStates[(int)pad] & buttonId) & ~(currentStates[(int)pad] & buttonId)) != 0;
        }
    }
}
