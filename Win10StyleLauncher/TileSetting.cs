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
using Android.Graphics;
using Android.Util;

namespace LiveTilesWidget
{
    [Activity(Label = "���ô���С����", Name = "com.LiveTilesWidget.TileSetting", Exported = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTask, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class TileSetting : Activity
    {
        int count = 1;
        //��Ǵ˴������Ƿ��ǳ�ʼ������(Configuration Activity)
        private bool isInitialize = false;
        //��ǰ�������õĴ�����ʵ������
        private AppDetail tile;
        private TilesPreferenceEditor editor;

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

            //ҳ���ϵĿؼ�
            Button btnChooseApp = FindViewById<Button>(Resource.Id.btnChooseApp);
            CheckBox checkShowNotif = FindViewById<CheckBox>(Resource.Id.checkShowNotif);
            CheckBox checkAutoColor = FindViewById<CheckBox>(Resource.Id.checkAutoColor);
            CheckBox checkShowNotifIcon = FindViewById<CheckBox>(Resource.Id.checkShowNotifIcon);
            Button btnRefresh = FindViewById<Button>(Resource.Id.btnRefresh);
            RadioGroup radioGroupTileType = FindViewById<RadioGroup>(Resource.Id.radioGroupTileType);
            EditText editRssUrl = FindViewById<EditText>(Resource.Id.editRssUrl);

            //��ȡ�洢�Ĵ�����Ϣ
            editor = new TilesPreferenceEditor(this);

            if (isInitialize)
            {
                tile = new AppDetail();
                tile.ShowNotification = true;
                checkShowNotif.Checked = true;
                tile.AutoTileColor = true;
                checkAutoColor.Checked = true;
                tile.ShowNotifIcon = false;
                checkShowNotifIcon.Checked = false;
                btnRefresh.Enabled = false;
                tile.TileType = LiveTileType.None;
                radioGroupTileType.Check(Resource.Id.radioTypeNone);
                editRssUrl.Visibility = ViewStates.Gone;
            }
            else
            {
                tile = editor.GetTileById(id);
                if (tile == null)
                {
                    Finish();
                }
                try
                {
                    //����ѡ��Ӧ�ð�ť������Ϊ�洢��¼�д˴�����ǰָ���Ӧ��
                    btnChooseApp.Text = tile.Label;
                    //�����Ƿ�������ʾ֪ͨѡ����״̬Ϊ�洢��¼�д˴�����ǰ�Ƿ�������ʾ֪ͨ��״̬
                    checkShowNotif.Checked = tile.ShowNotification;
                    checkAutoColor.Checked = tile.AutoTileColor;
                    checkShowNotifIcon.Checked = tile.ShowNotifIcon;
                    switch (tile.TileType)
                    {
                        case LiveTileType.None:
                            radioGroupTileType.Check(Resource.Id.radioTypeNone);
                            break;
                        case LiveTileType.Rss:
                            radioGroupTileType.Check(Resource.Id.radioTypeRss);
                            break;
                    }
                }
                catch { }
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
            checkShowNotifIcon.CheckedChange += (sender, e) =>
            {
                tile.ShowNotifIcon = checkShowNotifIcon.Checked;
            };
            radioGroupTileType.CheckedChange += (sender, e) =>
            {
                switch (radioGroupTileType.CheckedRadioButtonId)
                {
                    case Resource.Id.radioTypeNone://������
                        tile.TileType = LiveTileType.None;
                        editRssUrl.Visibility = ViewStates.Gone;
                        break;
                    case Resource.Id.radioTypeRss://Rss
                        tile.TileType = LiveTileType.Rss;
                        editRssUrl.Visibility = ViewStates.Visible;
                        if (editRssUrl.Text != "")
                        {
                            tile.RssUrl = editRssUrl.Text;
                        }
                        break;
                }
            };
            editRssUrl.TextChanged += (sender, e) =>
            {
                tile.RssUrl = editRssUrl.Text;
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
                 Codes.UpdateTiles(id, this, null, null);
                 Intent iRss = new Intent(this, typeof(ReadRss));
                 iRss.SetAction("com.LiveTilesWidget.UpdateRss");
                 StartService(iRss);

                 Intent result = new Intent();
                 Bundle b = new Bundle();
                 b.PutInt(AppWidgetManager.ExtraAppwidgetId, id);
                 result.PutExtras(b);
                 SetResult(Result.Ok, result);

                 Log.Debug("LiveTileWidget", "TileSettingReturned");
                 Finish();
             };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case 0://Ӧ��ѡ������������
                        string label = data.GetStringExtra("Label");
                        FindViewById<Button>(Resource.Id.btnChooseApp).Text = label ?? "����Ӧ��";
                        tile.Label = label;
                        tile.Name = data.GetStringExtra("Name");
                        //tile.Icon = (Bitmap)data.GetParcelableExtra("Icon");
                        FindViewById<Button>(Resource.Id.btnRefresh).Enabled = (label != null);
                        break;
                    case 1://��ɫѡ������������
                        break;
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (!isInitialize)
            {
                var option = menu.Add(0, 0, 0, "Debug Delete");
                option.SetShowAsAction(ShowAsAction.CollapseActionView);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0://Debug Delete
                    AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    dialog.SetIcon(Resource.Drawable.Icon);
                    dialog.SetTitle("ȷ�����ã�");
                    dialog.SetMessage("�˹��ܽ����ڿ���������ʹ�ã������ء�");
                    dialog.SetCancelable(true);
                    dialog.SetPositiveButton("ȷ��", (sender, e) =>
                    {
                        //�����ǰ����������
                        if (!isInitialize)
                        {
                            editor.Tiles.Remove(tile);
                            editor.CommitChanges();
                        }
                        Finish();
                    });
                    dialog.SetNegativeButton("ȡ��", (sender, e) => { });
                    dialog.Show();
                    break;
            }

            return base.OnMenuItemSelected(featureId, item);
        }

    }
}