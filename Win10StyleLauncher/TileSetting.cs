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
    [Activity(Label = "���ô���С����")]
    public class TileSetting : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TileSettings);

            Button btnChooseApp = FindViewById<Button>(Resource.Id.btnChooseApp);
            CheckBox checkShowNotif = FindViewById<CheckBox>(Resource.Id.checkShowNotif);
            Button btnRefresh = FindViewById<Button>(Resource.Id.btnRefresh);

            //��Extra�л�ȡҪ�����Զ������õ�AppWidgetId
            var id = Intent.GetIntExtra("id", -1);
            if (id == -1)
            {
                Finish();//��û�д���id��Ϣ���˳�
            }

            //��ȡ�洢�Ĵ�����Ϣ
            var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();

            //����ѡ��Ӧ�ð�ť������Ϊ�洢��¼�д˴�����ǰָ���Ӧ��
            btnChooseApp.Text = preference.GetString(id.ToString() + "Label", "����Ӧ��");
            //�����Ƿ�������ʾ֪ͨѡ����״̬Ϊ�洢��¼�д˴�����ǰ�Ƿ�������ʾ֪ͨ��״̬
            checkShowNotif.Checked = preference.GetBoolean(id.ToString() + "ShowNotif", true);

            //�����ťʱ��ת��ѡ��Ӧ�õ�Activity
            btnChooseApp.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(AppPicker));
                intent.PutExtra("id", id);//��Ҫ���õĴ�����Id����Activity
                StartActivityForResult(intent, 0);
            };

            //��ѡ״̬����ʱ��������
            checkShowNotif.CheckedChange += (sender, e) =>
            {
                editor.PutBoolean(id + "ShowNotif", checkShowNotif.Checked);
                editor.Commit();
            };

            //�����ťʱ����ˢ�´���
            btnRefresh.Click += (sender, e) =>
            {
                Codes.UpdateTiles(id, this, null);
                Finish();
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case 0://Ӧ��ѡ������������
                    FindViewById<Button>(Resource.Id.btnChooseApp).Text = data.GetStringExtra("Label") ?? "����Ӧ��";
                    break;
                case 1://��ɫѡ������������
                    break;
            }
        }
    }
}