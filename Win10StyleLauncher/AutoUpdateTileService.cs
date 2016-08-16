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
using System.Xml;
using Android.Graphics;
using System.Net;
using Android.Util;

namespace LiveTilesWidget
{
    /// <summary>
    /// ����ʱ������и��¶�̬�����ϵ�rss���ݵķ���
    /// </summary>
    [Service(Label = "��̬����10�����Զ�ˢ�·���")]
    [IntentFilter(new string[] { "com.LiveTilesWidget.UpdateRss" })]
    public class AutoUpdateTileService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {
            Log.Debug("AutoUpdateTileService", "Running");
            TilesPreferenceEditor editor = new TilesPreferenceEditor(this);
            foreach (var item in editor.Tiles)
            {
                switch (item.TileType)
                {
                    case LiveTileType.Rss:
                        string text;
                        Bitmap img;
                        Codes.ReadRss(item.RssUrl, out text, out img);
                        //����С��������
                        Codes.UpdateTiles(item.Id, this, text, img);
                        break;
                }
            }
        }

    }
}