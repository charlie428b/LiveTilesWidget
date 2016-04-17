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
using Android.Appwidget;

namespace LiveTilesWidget
{
    [BroadcastReceiver(Label = "Normal Live Tile")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/normal_tile")]
    public class NormalTileProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            //��ȡ������Ϣ�Ĵ洢
            ContextWrapper contextWrapper = new ContextWrapper(context);
            var preference = contextWrapper.GetSharedPreferences("tiles", FileCreationMode.Private);
            //��������IDδ��¼�ڴ洢��ʱ�Ž��г�ʼ��
            if (!preference.GetStringSet("Ids", new List<string> { }).Contains(appWidgetIds[0].ToString()))
            {
                Codes.InitializeTile(context, appWidgetManager, appWidgetIds);
            }
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            base.OnDeleted(context, appWidgetIds);

            //ɾ��С����ʱ�Ƴ���¼
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();
            List<string> ids = new List<string>(preference.GetStringSet("Ids", new List<string> { }));
            if (ids.Contains(appWidgetIds[0].ToString()))
            {
                ids.Remove(appWidgetIds[0].ToString());
            }
            editor.PutStringSet("Ids", ids);
            editor.Commit();
        }
    }
}