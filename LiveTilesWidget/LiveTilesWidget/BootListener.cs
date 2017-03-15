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

namespace LiveTilesWidget
{
    /// <summary>
    /// ����ϵͳ�������
    /// </summary>
    [BroadcastReceiver]
    [IntentFilter(new string[] { Intent.ActionBootCompleted })]
    class BootListener : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //ˢ�����д���
            var editor = new TilesPreferenceEditor(context);
            foreach (var item in editor.Tiles)
            {
                Codes.UpdateTiles(item.Id, context, null, null);
            }
            //�ڴ˴������Զ�����Rss
            Codes.ArrangeRssUpdate(context);
        }
    }
}