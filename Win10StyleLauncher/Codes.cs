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
        /// 首次创建一个动态磁贴时进行的初始化，所需参数为照抄AppWidgetProvider.OnUpdate()的所有参数即可。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appWidgetManager"></param>
        /// <param name="appWidgetIds"></param>
        public static void InitializeTile(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);

            //创建RemoteViews对象，并设置初始化值
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
            //设置内容
            views.SetTextViewText(Resource.Id.tileLabel, "设置此磁贴" + appWidgetIds[0]);
            views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
            //设置背景色
            int color = preference.GetInt("AutoTileColor", 0x000000);
            Bitmap bitmap = Bitmap.CreateBitmap(new int[] { color }, 1, 1, Bitmap.Config.Argb8888);
            views.SetImageViewBitmap(Resource.Id.tileBackground, bitmap);
            //设置点击时执行的意图
            Intent intent = new Intent(context, typeof(TileSetting));
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[0]);//将Id传给Activity以便进行设置
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            //推送appWidget更新
            appWidgetManager.UpdateAppWidget(appWidgetIds[0], views);
        }

        /// <summary>
        /// 加载所有可被启动的应用
        /// </summary>
        /// <param name="manager">调用此方法的上下文的PackageManager属性</param>
        /// <returns>可被启动的应用的列表</returns>
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
        /// 刷新磁贴小部件
        /// </summary>
        /// <param name="id">小部件的Id</param>
        /// <param name="context">当前上下文</param>
        /// <param name="notification">要在磁贴上显示的最新通知，没有则为null</param>
        public static void UpdateTiles(int id, Context context, object notification)
        {
            //读取记录的磁贴设置
            TilesPreferenceEditor editor = new TilesPreferenceEditor(context);
            AppDetail tile = editor.GetTileById(id);
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);

            //创建RemoteViews对象
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
            //清空上次残留的Notification内容
            views.RemoveAllViews(Resource.Id.tileNotifParent);
            views.SetTextViewText(Resource.Id.tileNotification, "");
            //设置应用名称
            views.SetTextViewText(Resource.Id.tileLabel, tile.Label);
            //设置图标
            views.SetImageViewBitmap(Resource.Id.tileIcon, tile.Icon);
            //设置背景色
            if (tile.AutoTileColor)
            {
                int color = preference.GetInt("AutoTileColor", 0x000000);
                Bitmap bitmap = Bitmap.CreateBitmap(new int[] { color }, 1, 1, Bitmap.Config.Argb8888);
                views.SetImageViewBitmap(Resource.Id.tileBackground, bitmap);
            }
            //设置通知内容
            if (notification == null)
            {
                views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
                views.SetViewVisibility(Resource.Id.tileIcon, ViewStates.Visible);
            }
            else
            {
                //判断通知是解析好的字符串还是直接照搬即可的RemoteViews
                if (notification is string)
                {
                    views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Visible);
                    views.SetTextViewText(Resource.Id.tileNotification, (string)notification);
                }
                else if (notification is Notification)
                {
                    views.AddView(Resource.Id.tileNotifParent, ((Notification)notification).ContentView);
                    views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
                }
                views.SetViewVisibility(Resource.Id.tileIcon, ViewStates.Invisible);
            }
            //设置点击时执行的意图
            Intent intent = context.PackageManager.GetLaunchIntentForPackage(tile.Name);
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            //推送更新
            AppWidgetManager manager = AppWidgetManager.GetInstance(context);
            manager.UpdateAppWidget(id, views);
        }

        /// <summary>
        /// 获取今天的必应首页图片
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
        /// 获取Palette中出现最多的颜色
        /// </summary>
        /// <param name="swatches">要分析的palette的swatches属性</param>
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