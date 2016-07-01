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
using Android.Graphics;
using System.Net;
using System.Threading.Tasks;
using Android.Support.V7.Graphics;
using static Android.Support.V7.Graphics.Palette;

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
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);

            //����RemoteViews���󣬲����ó�ʼ��ֵ
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
            //��������
            views.SetTextViewText(Resource.Id.tileLabel, "���ô˴���" + appWidgetIds[0]);
            views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
            //���ñ���ɫ
            int color = preference.GetInt("AutoTileColor", 0x000000);
            Bitmap bitmap = Bitmap.CreateBitmap(new int[] { color }, 1, 1, Bitmap.Config.Argb8888);
            views.SetImageViewBitmap(Resource.Id.tileBackground, bitmap);
            //���õ��ʱִ�е���ͼ
            Intent intent = new Intent(context, typeof(TileSetting));
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[0]);//��Id����Activity�Ա��������
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

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
                app.Icon = ((BitmapDrawable)item.ActivityInfo.LoadIcon(manager)).Bitmap;
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
        /// <param name="icon">Ҫ�ڴ�������ʾ��֪ͨ��ͼ�꣬û����Ϊnull</param>
        public static void UpdateTiles(int id, Context context, object notification, Bitmap icon)
        {
            //��ȡ��¼�Ĵ�������
            TilesPreferenceEditor editor = new TilesPreferenceEditor(context);
            AppDetail tile = editor.GetTileById(id);
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);

            //���ݲ�ͬ�������RemoteViews����
            RemoteViews views;
            if (notification != null) //ʹ��֪ͨ����
            {
                if (notification is string)
                {
                    if (icon != null) //ʹ�ô�ͼ���֪ͨ����
                    {
                        views = new RemoteViews(context.PackageName, Resource.Layout.IconNotifTile);
                        //����ͼ�������
                        views.SetImageViewBitmap(Resource.Id.tileNotifIcon, icon);
                        views.SetTextViewText(Resource.Id.tileNotifText, (string)notification);
                    }
                    else //ʹ�ô����ֵ�֪ͨ����
                    {
                        views = new RemoteViews(context.PackageName, Resource.Layout.TextNotifTile);
                        //��������
                        views.SetTextViewText(Resource.Id.tileNotification, (string)notification);
                    }
                }
                else if (notification is Notification) //ʹ����ǶRemoteViews��֪ͨ����
                {
                    views = new RemoteViews(context.PackageName, Resource.Layout.ViewNotifTile);
                    //����ϴβ�����Notification����
                    views.RemoveAllViews(Resource.Id.tileNotifParent);
                    //ֱ��Ƕ��Notification��RemoteViews
                    views.AddView(Resource.Id.tileNotifParent, ((Notification)notification).ContentView);
                }
                else { return; }
            }
            else //ʹ����ͨ����
            {
                views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
                //����ͼ��
                views.SetImageViewBitmap(Resource.Id.tileIcon, tile.Icon);
            }
            if (views == null) { return; }

            //����Ӧ������
            views.SetTextViewText(Resource.Id.tileLabel, tile.Label);
            //���ñ���ɫ
            if (tile.AutoTileColor)
            {
                int color = preference.GetInt("AutoTileColor", 0x000000);
                Bitmap bitmap = Bitmap.CreateBitmap(new int[] { color }, 1, 1, Bitmap.Config.Argb8888);
                views.SetImageViewBitmap(Resource.Id.tileBackground, bitmap);
            }
            
            //���õ��ʱִ�е���ͼ
            Intent intent = context.PackageManager.GetLaunchIntentForPackage(tile.Name);
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            //���͸���
            AppWidgetManager manager = AppWidgetManager.GetInstance(context);
            manager.UpdateAppWidget(id, views);
        }

        /// <summary>
        /// ��ȡ����ı�Ӧ��ҳͼƬ
        /// </summary>
        /// <returns></returns>
        public static async Task<Bitmap> GetBingImage()
        {
            WebClient wc = new WebClient();
            byte[] buffer = await wc.DownloadDataTaskAsync(new Uri(@"http://area.sinaapp.com/bingImg"));
            Bitmap img = await BitmapFactory.DecodeByteArrayAsync(buffer, 0, buffer.Length);
            return img;
        }

        /// <summary>
        /// ��ȡPalette�г���������ɫ
        /// </summary>
        /// <param name="swatches">Ҫ������palette��swatches����</param>
        /// <returns></returns>
        public static int GetMainColor(IList<Swatch> swatches)
        {
            int max = -1, color = -1;
            if (swatches[0] != null)
            {
                max = swatches[0].Population;
                color = swatches[0].Rgb;
            }
            foreach (var item in swatches)
            {
                if (item != null)
                {
                    if (item.Population > max)
                    {
                        max = item.Population;
                        color = item.Rgb;
                    }
                }
            }
            return color;
        }
    }
}