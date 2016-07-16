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
        private TileDetail tile;
        private TilesPreferenceEditor editor;
        private int id;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //��Extra�л�ȡҪ�����Զ������õ�AppWidgetId
            id = Intent.GetIntExtra("id", -1);
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

            //ʹ֪ͨ����Ӧ�ñ�������ɫһ��
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            }

            //ҳ���ϵĿؼ�
            Button btnChooseApp = FindViewById<Button>(Resource.Id.btnChooseApp);
            CheckBox checkShowNotif = FindViewById<CheckBox>(Resource.Id.checkShowNotif);
            CheckBox checkShowNotifIcon = FindViewById<CheckBox>(Resource.Id.checkShowNotifIcon);
            RadioGroup radioGroupTileType = FindViewById<RadioGroup>(Resource.Id.radioGroupTileType);
            EditText editRssUrl = FindViewById<EditText>(Resource.Id.editRssUrl);
            Button btnColor = FindViewById<Button>(Resource.Id.btnColor);

            //��ȡ�洢�Ĵ�����Ϣ
            editor = new TilesPreferenceEditor(this);

            if (isInitialize)
            {
                tile = new TileDetail();
                tile.ShowNotification = true;
                checkShowNotif.Checked = true;
                tile.ShowNotifIcon = false;
                checkShowNotifIcon.Checked = false;
                tile.TileType = LiveTileType.None;
                radioGroupTileType.Check(Resource.Id.radioTypeNone);
                editRssUrl.Visibility = ViewStates.Gone;
                tile.TileColor = -1;
                btnColor.SetBackgroundColor(new Color(editor.AutoTileColor));
                btnColor.Text = "�Զ��ӱ�ֽȡɫ";
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
                    checkShowNotifIcon.Checked = tile.ShowNotifIcon;
                    switch (tile.TileType)
                    {
                        case LiveTileType.None:
                            radioGroupTileType.Check(Resource.Id.radioTypeNone);
                            editRssUrl.Visibility = ViewStates.Gone;
                            break;
                        case LiveTileType.Rss:
                            radioGroupTileType.Check(Resource.Id.radioTypeRss);
                            editRssUrl.Visibility = ViewStates.Visible;
                            editRssUrl.Text = tile.RssUrl;
                            break;
                    }
                    int color;
                    string colorName = "";
                    switch (tile.TileColor)
                    {
                        case -1://�Զ�
                            color = editor.AutoTileColor;
                            colorName = "�Զ��ӱ�ֽȡɫ";
                            break;
                        case -2://ȫ��Ĭ��ɫ
                            color = editor.DefaultTileColor;
                            colorName = "ȫ���Զ�����ɫ";
                            break;
                        default:
                            color = tile.TileColor;
                            break;
                    }
                    btnColor.Text = colorName;
                    btnColor.SetBackgroundColor(new Color(color));
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
            //�����ťʱ��ת��ѡ����ɫ��Activity
            btnColor.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(ColorPicker));
                StartActivityForResult(intent, 1);
            };

            //��ѡ״̬����ʱ��������
            checkShowNotif.CheckedChange += (sender, e) =>
            {
                tile.ShowNotification = checkShowNotif.Checked;
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
                        if (label != null && isInitialize)
                        {
                            var optFinish = _menu.Add(0, 0, 0, "����");
                            optFinish.SetShowAsAction(ShowAsAction.Always);
                        }
                        //����һЩӦ���Զ��Ƽ���̬��������
                        switch (label)
                        {
                            case "��̬����10":
                            case "IT֮��":
                                FindViewById<RadioGroup>(Resource.Id.radioGroupTileType).Check(Resource.Id.radioTypeRss);
                                FindViewById<EditText>(Resource.Id.editRssUrl).Text = @"http://api.ithome.com/xml/newslist/news.xml";
                                break;
                        }
                        break;
                    case 1://��ɫѡ������������
                        tile.TileColor = data.GetIntExtra("Color", -1);
                        int color;
                        string colorName = "";
                        switch (tile.TileColor)
                        {
                            case -1://�Զ�
                                color = editor.AutoTileColor;
                                colorName = "�Զ��ӱ�ֽȡɫ";
                                break;
                            case -2://ȫ��Ĭ��ɫ
                                color = editor.DefaultTileColor;
                                colorName = "ȫ���Զ�����ɫ";
                                break;
                            default:
                                color = tile.TileColor;
                                break;
                        }
                        var button = FindViewById<Button>(Resource.Id.btnColor);
                        button.Text = colorName;
                        button.SetBackgroundColor(new Color(color));
                        break;
                }
            }
        }

        /// <summary>
        /// �洢action bar menu�ϵı��水ť�Ķ��������ط�ʹ��
        /// </summary>
        private IMenu _menu;

        /// <summary>
        /// ����action bar�˵�
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            _menu = menu;
            if (!isInitialize)
            {
                var optFinish = menu.Add(0, 0, 0, "����");
                optFinish.SetShowAsAction(ShowAsAction.Always);
                var option = menu.Add(0, 1, 1, "Debug Delete");
                option.SetShowAsAction(ShowAsAction.CollapseActionView);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0://���� �����ťʱ������Ĳ�����ˢ�´���
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

                    break;

                case 1://Debug Delete
                    AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    dialog.SetIcon(Resource.Drawable.Icon);
                    dialog.SetTitle("ɾ���������ã�");
                    dialog.SetMessage("�˹��ܽ����ڿ���������ʹ�ã������ء�");
                    dialog.SetCancelable(true);
                    dialog.SetPositiveButton("ȷ��ɾ��", (sender, e) =>
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