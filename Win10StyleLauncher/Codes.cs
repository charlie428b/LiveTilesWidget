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
using Android.Content.PM;
using Android.Graphics.Drawables;

namespace LiveTilesWidget
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
            //����RemoteViews���󣬲����ó�ʼ��ֵ
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
            //��������
            views.SetTextViewText(Resource.Id.tileLabel, "���ô˴���" + appWidgetIds[0]);
            views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
            //���õ��ʱִ�е���ͼ
            Intent intent = new Intent(context, typeof(TileSetting));
            intent.PutExtra("id", appWidgetIds[0]);//��Id����Activity�Ա��������
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            //���µĴ�����ID��ӵ�SharedPreference��
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();
            editor.PutString("Ids", preference.GetString("Ids", "") + appWidgetIds[0] + ",");

            //����appWidget����
            appWidgetManager.UpdateAppWidget(appWidgetIds[0], views);
        }

        /// <summary>
        /// �������пɱ�������Ӧ��
        /// </summary>
        /// <param name="manager">���ô˷����������ĵ�PackageManager����</param>
        /// <returns>�ɱ�������Ӧ�õ��б�</returns>
        public static List<AppDetail> LoadApps(PackageManager manager)
        {
            List<AppDetail> apps = new List<AppDetail>();
            Intent intentApps = new Intent(Intent.ActionMain, null);
            intentApps.AddCategory(Intent.CategoryLauncher);
            var availableActivities = manager.QueryIntentActivities(intentApps, 0);
            foreach (var item in availableActivities)
            {
                AppDetail app = new AppDetail();
                app.Label = item.LoadLabel(manager);
                app.Name = item.ActivityInfo.PackageName;
                app.Icon = item.ActivityInfo.LoadIcon(manager);
                apps.Add(app);
            }
            return apps;
        }

        /// <summary>
        /// ˢ�´���С����
        /// </summary>
        /// <param name="Id">С������Id</param>
        /// <param name="context">��ǰ������</param>
        /// <param name="Notification">Ҫ�ڴ�������ʾ������֪ͨ��û����Ϊnull</param>
        public static void UpdateTiles(int Id, Context context, string Notification)
        {
            //��ȡ��¼�Ĵ�������
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);

            //����RemoteViews����
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
            //����Ӧ������
            views.SetTextViewText(Resource.Id.tileLabel, preference.GetString(Id + "Label", "����"));
            //����ͼ��
            AppDetail app = null;
            foreach (var item in LoadApps(context.PackageManager))
            {
                if (item.Name == preference.GetString(Id + "Name", null))
                {
                    app = item;
                    break;
                }
            }
            if (app != null)
            {
                views.SetImageViewBitmap(Resource.Id.tileIcon, ((BitmapDrawable)app.Icon).Bitmap);
            }
            //����֪ͨ����
            if (Notification == null)
            {
                views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
            }
            else
            {
                views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Visible);
                views.SetTextViewText(Resource.Id.tileNotification, Notification);
            }
            //���õ��ʱִ�е���ͼ
            Intent intent = context.PackageManager.GetLaunchIntentForPackage(app.Name);
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            //���͸���
            AppWidgetManager manager = AppWidgetManager.GetInstance(context);
            manager.UpdateAppWidget(Id, views);
        }
    }
}