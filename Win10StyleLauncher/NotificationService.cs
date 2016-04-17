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
using Android.Service.Notification;
using Android.Util;

namespace LiveTilesWidget
{
    [Service(Label = "��̬����10֪ͨ̽�����", Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE")]
    [IntentFilter(new string[] { "android.service.notification.NotificationListenerService" })]
    public class NotificationService : NotificationListenerService
    {
        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug("notif", "creating");
        }

        public override void OnNotificationPosted(StatusBarNotification sbn)
        {
            //base.OnNotificationPosted(sbn);

            //�ж��յ���֪ͨ�Ƿ������ѹ̶�������Ӧ��
            var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            List<string> ids = new List<string>(preference.GetStringSet("Ids", new List<string> { }));
            foreach (var item in ids)
            {
                //�Ƿ������ѿ�����ʾ֪ͨ���ܵĴ�����Ӧ��
                if (preference.GetBoolean(item + "ShowNotif", true) && sbn.PackageName == preference.GetString(item + "Name", ""))
                {
                    //ͨ��ö�������֪ͨ����
                    //View v = sbn.Notification.ContentView.Apply(this, new LinearLayout(this));
                    string text = "";
                    text = sbn.Notification.Extras.GetString(Notification.ExtraTitle, "����") + '\n' + sbn.Notification.Extras.GetString(Notification.ExtraText, "����");
                    //EnumGroupViews(v, ref text);

                    //���Ͷ�̬����С��������
                    Codes.UpdateTiles(Convert.ToInt32(item), this, text);
                }
            }
        }

        public override void OnNotificationRemoved(StatusBarNotification sbn)
        {
            //base.OnNotificationRemoved(sbn);

            //�ж��յ���֪ͨ�Ƿ������ѹ̶�������Ӧ��
            var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            List<string> ids = new List<string>(preference.GetStringSet("Ids", new List<string> { }));
            foreach (var item in ids)
            {
                //�Ƿ������ѿ�����ʾ֪ͨ���ܵĴ�����Ӧ��
                if (preference.GetBoolean(item + "ShowNotif", true) && sbn.PackageName == preference.GetString(item + "Name", ""))
                {
                    //����С��������
                    Codes.UpdateTiles(Convert.ToInt32(item), this, null);
                }
            }
        }

        /// <summary>
        /// ö��GroupView�еĳ�Ա���ҳ����е�TextView
        /// </summary>
        /// <param name="v">Ҫ����ö�ٵ�View</param>
        /// <param name="text">���ҳ���TextView���ı��ᱻ׷�ӵ����ַ�����</param>
        private void EnumGroupViews(View v, ref string text)
        {
            if (v is ViewGroup)
            {
                ViewGroup vg = (ViewGroup)v;
                for (int i = 0; i < vg.ChildCount; i++)
                {
                    View child = vg.GetChildAt(i);
                    if (child is ViewGroup)
                    {
                        //�ݹ鴦��ViewGroup
                        EnumGroupViews(child, ref text);
                    }
                    else if (child is TextView)
                    {
                        //�������������ݣ���׷�ӵ�text��
                        string str = ((TextView)child).Text;
                        if (str.Length > 0)
                        {
                            text += str + '\n';
                        }
                    }
                }
            }
        }
    }
}