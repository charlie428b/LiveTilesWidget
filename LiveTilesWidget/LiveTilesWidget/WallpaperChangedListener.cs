using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using static Android.Support.V7.Graphics.Palette;
//using static Android.Support.V7.Graphics.Palette;

namespace LiveTilesWidget
{
    /// <summary>
    /// ����ֽ����ʱ���´�����ɫ
    /// </summary>
    [BroadcastReceiver]
    [IntentFilter(new string[] { Intent.ActionWallpaperChanged })]
    public class WallpaperChangedListener : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug("wallpaperChanged", "received");

            //��ȡ��ֽ�����ɴ����ɵ�ɫ��
            WallpaperManager wall = WallpaperManager.GetInstance(context);
            Palette palette = From(((BitmapDrawable)wall.Drawable).Bitmap).Generate();

            //��ȡ�洢�Ĵ�����Ϣ
            var editor = new TilesPreferenceEditor(context);

            //����ֽ��ɫ��д��洢
            int color;
            color = palette.GetLightVibrantColor(-1);
            if (color == -1)
            {
                color = Codes.GetMainColor(new List<Swatch>()
                {
                    palette.LightVibrantSwatch,
                    palette.LightMutedSwatch,
                    palette.DarkMutedSwatch,
                    palette.DarkVibrantSwatch,
                    palette.VibrantSwatch,
                    palette.MutedSwatch
                });

                if (color == -1)
                {
                    color = Codes.GetMainColor(palette.Swatches);
                }
            }
            editor.AutoTileColor = color;

            //�������д���
            foreach (var item in editor.Tiles)
            {
                if (item.TileColor == -1)
                {
                    Codes.UpdateTiles(item.Id, context, null, null);
                }
            }
        }
    }
}