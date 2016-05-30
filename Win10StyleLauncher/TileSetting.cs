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

namespace LiveTilesWidget
{
    [Activity(Label = "���ô���С����", Name = "com.LiveTilesWidget.TileSetting", Exported = true)]
    public class TileSetting : Activity
    {
        int count = 1;
        //��Ǵ˴������Ƿ��ǳ�ʼ������(Configuration Activity)
        bool isInitialize = false;
        //��ǰ�������õĴ�����ʵ������
        AppDetail tile;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //��Extra�л�ȡҪ�����Զ������õ�AppWidgetId
            var id = Intent.GetIntExtra("id", -1);
            if (id == -1)
            {
                //��û�д���id��Ϣ�����Գ�ʼ�����̵ķ�ʽȡ��id�������˳�
                id = Intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, -1);
                isInitialize = true;
                if (id == -1)
                {
                    Finish();
                }
            }
            //��ֹ�����˳�
            Intent i = new Intent();
            i.PutExtra(AppWidgetManager.ExtraAppwidgetId, id);
            SetResult(Result.Canceled, i);

            SetContentView(Resource.Layout.TileSettings);

            Button btnChooseApp = FindViewById<Button>(Resource.Id.btnChooseApp);
            CheckBox checkShowNotif = FindViewById<CheckBox>(Resource.Id.checkShowNotif);
            CheckBox checkAutoColor = FindViewById<CheckBox>(Resource.Id.checkAutoColor);
            Button btnRefresh = FindViewById<Button>(Resource.Id.btnRefresh);

            //��ȡ�洢�Ĵ�����Ϣ
            TilesPreferenceEditor editor = new TilesPreferenceEditor(this);

            if (isInitialize)
            {
                tile = new AppDetail();
                tile.ShowNotification = true;
                checkShowNotif.Checked = true;
                tile.AutoTileColor = true;
                checkAutoColor.Checked = true;
                btnRefresh.Enabled = false;
            }
            else
            {
                tile = editor.GetTileById(id);
                //����ѡ��Ӧ�ð�ť������Ϊ�洢��¼�д˴�����ǰָ���Ӧ��
                btnChooseApp.Text = tile.Label;
                //�����Ƿ�������ʾ֪ͨѡ����״̬Ϊ�洢��¼�д˴�����ǰ�Ƿ�������ʾ֪ͨ��״̬
                checkShowNotif.Checked = tile.ShowNotification;
                checkAutoColor.Checked = tile.AutoTileColor;
            }

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
                tile.ShowNotification = checkShowNotif.Checked;
            };
            checkAutoColor.CheckedChange += (sender, e) =>
            {
                tile.AutoTileColor = checkAutoColor.Checked;
            };

            //�����ťʱ������Ĳ�����ˢ�´���
            btnRefresh.Click += (sender, e) =>
             {
                 if (isInitialize)
                 {
                     tile.Id = id;
                     editor.Tiles.Add(tile);
                 }
                 editor.CommitChanges();
                 Codes.UpdateTiles(id, this, null);
                 Intent result = new Intent();
                 i.PutExtra(AppWidgetManager.ExtraAppwidgetId, id);
                 SetResult(Result.Ok, result);
                 Finish();
             };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case 0://Ӧ��ѡ������������
                    string label = data.GetStringExtra("Label");
                    FindViewById<Button>(Resource.Id.btnChooseApp).Text = label ?? "����Ӧ��";
                    tile.Label = label;
                    tile.Name = data.GetStringExtra("Name");
                    FindViewById<Button>(Resource.Id.btnRefresh).Enabled = (label != null);
                    break;
                case 1://��ɫѡ������������
                    break;
            }
        }
    }
}