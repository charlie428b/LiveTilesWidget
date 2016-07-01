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
using System.Threading.Tasks;

namespace LiveTilesWidget
{
    /// <summary>
    /// 选择磁贴所指向的应用
    /// </summary>
    [Activity(Label = "选择一个应用", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class AppPicker : ListActivity
    {
        protected List<AppDetail> apps;
        private int Id;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            //防止意外退出
            SetResult(Result.Canceled);
            base.OnCreate(savedInstanceState);

            ListView.Visibility = ViewStates.Invisible;
            await Task.Run(() =>
            {
                //加载应用列表
                apps = Codes.LoadApps(PackageManager);
                //排序列表
                apps.Sort(new PinyinComparer());
            });

            //显示应用
            ListView.FastScrollEnabled = true;
            ListView.FastScrollAlwaysVisible = true;
            ListAdapter = new AppListAdapter(this, Resource.Layout.AppPickerItems, apps.ToArray());
            ListView.Visibility = ViewStates.Visible;

            //从Extra中获取要进行自定义设置的AppWidgetId
            Id = Intent.GetIntExtra("id", -1);
            if (Id == -1)
            {
                Finish();
            }
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            ////将所选的应用信息保存到SharedPreferences中以供保存
            //var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            //var editor = preference.Edit();
            //editor.PutString(Id + "Label", apps[position].Label);
            //editor.PutString(Id + "Name", apps[position].Name);
            //editor.Commit();

            //返回应用的Label、Name
            Intent i = new Intent();
            i.PutExtra("Label", apps[position].Label);
            i.PutExtra("Name", apps[position].Name);
            i.PutExtra("Icon", apps[position].Icon);
            SetResult(Result.Ok, i);
            Finish();
            //Intent intent = PackageManager.GetLaunchIntentForPackage(apps[position].Name);
            //StartActivity(intent);
        }
    }
}