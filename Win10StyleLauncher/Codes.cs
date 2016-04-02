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

namespace Win10StyleLauncher
{
    public static class Codes
    {
        /// <summary>
        /// �״δ���һ����̬����ʱ���еĳ�ʼ�����������Ϊ�ճ�AppWidgetProvider.OnUpdate()�����в������ɡ�
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appWidgetManager"></param>
        /// <param name="appWidgetIds"></param>
        public static void InitializeTile(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
            views.SetTextViewText(Resource.Id.tileLabel, "���ô˴���" + appWidgetIds[0]);
            views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
            Intent intent = new Intent(context, typeof(InitializeTile));
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            appWidgetManager.UpdateAppWidget(appWidgetIds[0], views);
        }
    }
}