using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace Yılan
{
    public class WaponText
    {
        public static string normal; // 通常弾メッセージ 
        public static string diffusion; // 拡散弾メッセージ
        public static string tracking; // 追尾弾メッセージ
        public static string everyDirection; // 全方位弾メッセージ

        public static void Update()
        {
            normal = "通常弾 : Lv." + PlayerBullet.normalLevel + "\n" +
                     "　　　   射程 : " + PlayerBullet.normalRange + "\n" +
                     "　　　   リロードタイム : なし" + "\n" + 
                     "\n" +
                     "説明　 : 向いている方向に真っすぐ弾が飛びます。";

            diffusion = "拡散弾 : Lv." + PlayerBullet.diffusionLevel + "\n" +
                        "　　　   射程 : " + PlayerBullet.diffusionRange + "\n" +
                        "　　　   リロードタイム : " + Math.Round(PlayerBullet.diffusionCoolTimeMax / 60, 2, MidpointRounding.AwayFromZero) + "秒" + "\n" +
                        "\n" +
                        "説明　 : 向いている方向に弾が拡散して飛びます。";

            tracking = "追尾弾 : Lv." + PlayerBullet.trackingLevel + "\n" +
                       "　　　   射程 : なし" + "\n" +
                       "　　　   リロードタイム : " + Math.Round(PlayerBullet.trackingCoolTimeMax / 60, 2, MidpointRounding.AwayFromZero) + "秒" + "\n" +
                       "\n" +
                       "説明　 : 一番遠くの敵に向かって弾が飛びます。" + "\n" +
                       "　　　 　弾は敵、壁を貫通します。";

            everyDirection = "全方位弾 : Lv." + PlayerBullet.everyDirectionLevel + "\n" +
                             "　　　   　射程 : " + PlayerBullet.everyDirectionRange + "\n" +
                             "　　　   　リロードタイム : " + Math.Round(PlayerBullet.everyDirectionCoolTimeMax / 60, 2, MidpointRounding.AwayFromZero) + "秒" + "\n" +
                             "\n" +
                             "説明　　 : プレイヤーを中心に全方向に弾が飛びます。";
        }
    }
}
