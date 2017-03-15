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
using System.Threading;

namespace LiveTilesWidget
{
    [BroadcastReceiver(Label = "��̬����")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/normal_tile")]
    public class NormalTileProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            //��ȡ������Ϣ�Ĵ洢
            TilesPreferenceEditor editor = new TilesPreferenceEditor(context);
            //��������IDδ��¼�ڴ洢��ʱ�Ž��г�ʼ��
            if (editor.GetTileById(appWidgetIds[0]) == null)
            {
                Codes.InitializeTile(context, appWidgetManager, appWidgetIds);
                //Intent i = new Intent(context, typeof(TileSetting));
                //i.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[0]);
                //i.AddFlags(ActivityFlags.NewTask);
                //Thread.Sleep(3000);//��ֹ�û�����������С����
                //context.StartActivity(i);
            }
            //else
            //{
            //    Codes.UpdateTiles(appWidgetIds[0], context, null);
            //}
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            base.OnDeleted(context, appWidgetIds);

            //ɾ��С����ʱ�Ƴ���¼
            TilesPreferenceEditor editor = new TilesPreferenceEditor(context);
            if (editor.GetTileById(appWidgetIds[0]) != null)
            {
                editor.Tiles.Remove(editor.GetTileById(appWidgetIds[0]));
                editor.CommitChanges();
            }
        }
    }
}