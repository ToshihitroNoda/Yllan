using DxLibDLL;

namespace Yılan
{
    public static class Camera
    {
        public static float x;
        public static float y;

        public static void LookAt(float targetX, float targetY)
        {
            x = targetX - Screen.Width / 2;
            y = targetY - Screen.Height / 2;
        }

        /// <summary>
        /// 画像を描画する。スクロールの影響を受ける。
        /// </summary>
        /// <param name="worldX">画像の中心のx座標</param>
        /// <param name="worldY">画像の中心のy座標</param>
        /// <param name="rate">拡大率</param>
        /// <param name="angle">描画角度</param>
        /// <param name="handle">画像のハンドル</param>
        /// <param name="filp">左右反転するならtrue, しないならfalse（反転しない場合は省略可）</param>
        public static void DrawGraph(float worldX, float worldY, double rate, double angle, int handle)
        {
            if (handle == Image.playerbullet)
                DX.DrawRotaGraphF(worldX - x, worldY - y, 1, angle, handle);
            else
                DX.DrawGraphF(worldX - x, worldY - y, handle);
        }

        /// <summary>
        /// スクロール影響受けアニメーションをしながら画像を描画する
        /// </summary>
        /// <param name="worldX">画像の中心のx座標</param>
        /// <param name="worldY">画像の中心のy座標</param>
        /// <param name="drawX">描画元範囲x座標</param>
        /// <param name="drawY">描画元範囲y座標</param>
        /// <param name="imageWidth">描画元範囲の幅</param>
        /// <param name="imageHeight">描画元範囲の高さ</param>
        /// <param name="handle">画像ハンドル</param>
        /// <param name="direction">画像の向きを取得</param>>
        /// <param name="vx">x方向の移動速度（0のときは待機中イラスト）</param>
        /// <param name="vy">y方向の移動速度（0のときは待機中イラスト）</param>
        public static void DrawRectGraph(float worldX, float worldY, int drawX, int drawY, int imageWidth, int imageHeight, int handle, Direction direction, float vx, float vy, bool attackFlg)
        {
            if (direction == Direction.Down)
            {
                if (vx != 0 || vy != 0)
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), drawX, drawY * 0, imageWidth, imageHeight, handle);
                }
                else if (attackFlg == true)
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), imageWidth * 3, drawY * 0, imageWidth, imageHeight, handle);
                }
                else
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), imageWidth * 2, drawY * 0, imageWidth, imageHeight, handle);
                }
            }
            if (direction == Direction.Right)
            {
                if (vy != 0 || vx != 0)
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), drawX, drawY * 1, imageWidth, imageHeight, handle);
                }
                else if (attackFlg == true)
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), imageWidth * 3, drawY * 1, imageWidth, imageHeight, handle);
                }
                else
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), imageWidth * 2, drawY * 1, imageWidth, imageHeight, handle);
                }
            }
            if (direction == Direction.Left)
            {
                if (vy != 0 || vx != 0)
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), drawX, drawY * 2, imageWidth, imageHeight, handle);
                }
                else if (attackFlg == true)
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), imageWidth * 3, drawY * 2, imageWidth, imageHeight, handle);
                }
                else
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), imageWidth * 2, drawY * 2, imageWidth, imageHeight, handle);
                }
            }
            if (direction == Direction.Up)
            {
                if (vx != 0 || vy != 0)
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), drawX, drawY * 3, imageWidth, imageHeight, handle);
                }
                else if (attackFlg == true)
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), imageWidth * 3, drawY * 3, imageWidth, imageHeight, handle);
                }
                else
                {
                    DX.DrawRectGraph((int)(worldX - x), (int)(worldY - y), imageWidth * 2, drawY * 3, imageWidth, imageHeight, handle);
                }
            }
        }

        /// <summary>
        /// 四角形（枠線のみ）を描画する
        /// </summary>
        /// <param name="left">左端</param>
        /// <param name="top">上端</param>
        /// <param name="right">右端</param>
        /// <param name="bottom">下端</param>
        /// <param name="color">色</param>
        public static void DrawLineBox(float left, float top, float right, float bottom, uint color)
        {
            DX.DrawBox(
            (int)(left - x + 0.5f),
            (int)(top - y + 0.5f),
            (int)(right - x + 0.5f),
            (int)(bottom - y + 0.5f),
            color,
            DX.FALSE);
        }
    }
}
