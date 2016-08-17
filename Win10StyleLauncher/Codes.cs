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
using System.Xml;

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
            //���ñ���ɫ
            //int color = preference.GetInt("AutoTileColor", 0x000000);
            //Bitmap bitmap = Bitmap.CreateBitmap(new int[] { color }, 1, 1, Bitmap.Config.Argb8888);
            //views.SetImageViewBitmap(Resource.Id.tileBackground, bitmap);
            ////���õ��ʱִ�е���ͼ
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
        public static List<TileDetail> LoadApps(PackageManager manager)
        {
            List<TileDetail> apps = new List<TileDetail>();
            Intent intentApps = new Intent(Intent.ActionMain, null);
            intentApps.AddCategory(Intent.CategoryLauncher);
            var availableActivities = manager.QueryIntentActivities(intentApps, 0);
            foreach (var item in availableActivities)
            {
                TileDetail app = new TileDetail();
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
        /// <param name="id">С������Id��洢С�������õ�TileDetail���󣬽�������int����ʱ�Ż���ʽ����С�������£�������TileDetail��������Ϊ�������Ԥ���������������С��������</param>
        /// <param name="context">��ǰ������</param>
        /// <param name="notification">Ҫ�ڴ�������ʾ������֪ͨ��û����Ϊnull</param>
        /// <param name="icon">Ҫ�ڴ�������ʾ��֪ͨ��ͼ�꣬û����Ϊnull</param>
        public static RemoteViews UpdateTiles(object id, Context context, object notification, Bitmap icon)
        {
            //��ȡ��¼�Ĵ�������
            TilesPreferenceEditor editor = new TilesPreferenceEditor(context);
            TileDetail tile = (id is TileDetail) ? id as TileDetail : editor.GetTileById((int)id);

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
                else { return null; }
            }
            else //ʹ����ͨ����
            {
                views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
                //����ͼ��
                views.SetImageViewBitmap(Resource.Id.tileIcon, tile.Icon);
            }
            if (views == null) { return null; }

            //����Ӧ������
            views.SetTextViewText(Resource.Id.tileLabel, tile.Label);
            //���ñ���ɫ
            int color;
            switch (tile.TileColor)
            {
                case -1://�Զ�
                    color = editor.AutoTileColor;
                    break;
                case -2://ȫ��Ĭ��ɫ
                    color = editor.DefaultTileColor;
                    break;
                default:
                    color = tile.TileColor;
                    break;
            }
            Bitmap bitmap = Bitmap.CreateBitmap(new int[] { color }, 1, 1, Bitmap.Config.Argb8888);
            views.SetImageViewBitmap(Resource.Id.tileBackground, bitmap);

            //���õ��ʱִ�е���ͼ
            Intent intent = context.PackageManager.GetLaunchIntentForPackage(tile.Name);
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            if (id is int)
            {
                //���͸���
                AppWidgetManager manager = AppWidgetManager.GetInstance(context);
                manager.UpdateAppWidget((int)id, views);
            }

            return views;
        }

        /// <summary>
        /// ��ȡ����ı�Ӧ��ҳͼƬ
        /// </summary>
        /// <returns></returns>
        public static async Task<Bitmap> GetBingImage()
        {
            try
            {
                WebClient wc = new WebClient();
                byte[] buffer = await wc.DownloadDataTaskAsync(new Uri(@"http://area.sinaapp.com/bingImg"));
                Bitmap img = await BitmapFactory.DecodeByteArrayAsync(buffer, 0, buffer.Length);
                return img;
            }
            catch
            {
                return null;
            }
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

        /// <summary>
        /// ���Ŷ�ʱ����Rss�Զ����·���
        /// </summary>
        /// <param name="context"></param>
        public static void ArrangeRssUpdate(Context context)
        {
            Intent i = new Intent(context, typeof(AutoUpdateTileService));
            i.SetAction("com.LiveTilesWidget.UpdateRss");
            PendingIntent pi = PendingIntent.GetService(context, 0, i, PendingIntentFlags.CancelCurrent);
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            am.SetRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime(), 30 * 60 * 1000, pi);
        }

        /// <summary>
        /// ��ȡSharedPreferences
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ISharedPreferences GetPreferences(Context context)
        {
            return context.GetSharedPreferences(Application.Context.PackageName + ".tiles", FileCreationMode.Private);
        }

        /// <summary>
        /// Ǩ�ƾɰ�preferences�������°�
        /// </summary>
        /// <param name="originVersion">ԭ���İ汾��</param>
        /// <param name="context"></param>
        public static void MovePreferences(int originVersion, Context context)
        {
            switch (originVersion)
            {
                case 0:
                    var old = context.GetSharedPreferences("tiles", FileCreationMode.Private);
                    var pref = GetPreferences(context);
                    var editor = pref.Edit();
                    editor.Clear();
                    foreach (var item in old.All)
                    {
                        if (item.Value is bool)
                        {
                            editor.PutBoolean(item.Key, (bool)item.Value);
                        }
                        else if (item.Value is float)
                        {
                            editor.PutFloat(item.Key, (float)item.Value);
                        }
                        else if (item.Value is int)
                        {
                            editor.PutInt(item.Key, (int)item.Value);
                        }
                        else if (item.Value is long)
                        {
                            editor.PutLong(item.Key, (long)item.Value);
                        }
                        else if (item.Value is string)
                        {
                            editor.PutString(item.Key, (string)item.Value);
                        }
                        else if (item.Value is ICollection<string>)
                        {
                            editor.PutStringSet(item.Key, (ICollection<string>)item.Value);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// ��ָ��rss url��ȡ���е�һ��ı����ͼƬ
        /// </summary>
        /// <param name="url"></param>
        /// <param name="text"></param>
        /// <param name="img"></param>
        public static void ReadRss(string url, out string text, out Bitmap img)
        {
            //http://api.ithome.com/xml/newslist/news.xml
            text = null;
            string imgUrl = null;
            //��ʼrss����
            try
            {
                using (XmlTextReader reader = new XmlTextReader(url))
                {
                    bool read = false;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "entry":
                                case "item":
                                    read = true;
                                    continue;
                                case "title":
                                    if (read == true)
                                    {
                                        text = reader.ReadElementContentAsString();
                                    }
                                    break;
                                //case "summary":
                                //case "description":
                                //    if (read == true)
                                //    {
                                //        text += "\n" + reader.ReadElementContentAsString().Trim();
                                //    }
                                //    break;
                                case "image":
                                    if (read == true)
                                    {
                                        imgUrl = reader.ReadElementContentAsString();
                                    }
                                    break;
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            if (reader.Name == "item" || reader.Name == "entry")
                            {
                                read = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            img = null;
            if (imgUrl != null)
            {
                try
                {
                    WebClient wc = new WebClient();
                    byte[] buffer = wc.DownloadData(imgUrl);
                    img = BitmapFactory.DecodeByteArray(buffer, 0, buffer.Length);
                }
                catch { }
            }
        }
    }
}