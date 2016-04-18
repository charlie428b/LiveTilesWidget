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

            //����appWidget����
            appWidgetManager.UpdateAppWidget(appWidgetIds[0], views);

            //���µĴ�����ID��ӵ�SharedPreference��
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();
            List<string> ids = new List<string>(preference.GetStringSet("Ids", new List<string>()));
            ids.Add(appWidgetIds[0].ToString());
            editor.PutStringSet("Ids", ids);
            editor.Commit();
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
        /// <param name="id">С������Id</param>
        /// <param name="context">��ǰ������</param>
        /// <param name="notification">Ҫ�ڴ�������ʾ������֪ͨ��û����Ϊnull</param>
        public static void UpdateTiles(int id, Context context, object notification)
        {
            //��ȡ��¼�Ĵ�������
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);

            //����RemoteViews����
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
            //����ϴβ�����Notification����
            views.RemoveAllViews(Resource.Id.tileNotifParent);
            //����Ӧ������
            views.SetTextViewText(Resource.Id.tileLabel, preference.GetString(id + "Label", "����"));
            //����ͼ��
            AppDetail app = null;
            foreach (var item in LoadApps(context.PackageManager))
            {
                if (item.Name == preference.GetString(id + "Name", null))
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
            if (notification == null)
            {
                views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
                views.SetViewVisibility(Resource.Id.tileIcon, ViewStates.Visible);
            }
            else
            {
                //�ж�֪ͨ�ǽ����õ��ַ�������ֱ���հἴ�ɵ�RemoteViews
                if (notification is string)
                {
                    views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Visible);
                    views.SetTextViewText(Resource.Id.tileNotification, (string)notification);
                }
                else if (notification is Notification)
                {
                    views.AddView(Resource.Id.tileNotifParent, ((Notification)notification).ContentView);
                }
                views.SetViewVisibility(Resource.Id.tileIcon, ViewStates.Invisible);
            }
            //���õ��ʱִ�е���ͼ
            Intent intent = context.PackageManager.GetLaunchIntentForPackage(preference.GetString(id + "Name", context.PackageName));
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            //���͸���
            AppWidgetManager manager = AppWidgetManager.GetInstance(context);
            manager.UpdateAppWidget(id, views);
        }
    }
}