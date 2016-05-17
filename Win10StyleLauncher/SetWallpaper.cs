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
using Android.Graphics;
using Android.Util;

namespace LiveTilesWidget
{
    /// <summary>
    /// ����ֽ����Ϊ��Ӧ��ͼ�ķ���
    /// </summary>
    [Service]
    public class SetWallpaper : IntentService
    {
        protected override async void OnHandleIntent(Intent intent)
        {
            WallpaperManager wall = WallpaperManager.GetInstance(this);
            Bitmap img = await Codes.GetBingImage();
            wall.SetBitmap(img);
            Log.Debug("Walpaper", "Done");
        }
    }
}