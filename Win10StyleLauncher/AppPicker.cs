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
    /// ѡ�������ָ���Ӧ��
    /// </summary>
    [Activity(Label = "ѡ��һ��Ӧ��", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class AppPicker : ListActivity
    {
        protected List<AppDetail> apps;
        private int Id;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //��ֹ�����˳�
            SetResult(Result.Canceled);
            base.OnCreate(savedInstanceState);

            //����Ӧ���б�
            apps = Codes.LoadApps(PackageManager);
            //��ʾӦ��
            ListAdapter = new AppListAdapter(this, Resource.Layout.AppPickerItems, apps.ToArray());

            //��Extra�л�ȡҪ�����Զ������õ�AppWidgetId
            Id = Intent.GetIntExtra("id", -1);
            if (Id == -1)
            {
                Finish();
            }
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            ////����ѡ��Ӧ����Ϣ���浽SharedPreferences���Թ�����
            //var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            //var editor = preference.Edit();
            //editor.PutString(Id + "Label", apps[position].Label);
            //editor.PutString(Id + "Name", apps[position].Name);
            //editor.Commit();

            //����Ӧ�õ�Label��Name
            Intent i = new Intent();
            i.PutExtra("Label", apps[position].Label);
            i.PutExtra("Name", apps[position].Name);
            //i.PutExtra("Icon", apps[position].Icon);
            SetResult(Result.Ok, i);
            Finish();
            //Intent intent = PackageManager.GetLaunchIntentForPackage(apps[position].Name);
            //StartActivity(intent);
        }
    }
}