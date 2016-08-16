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
            //��Ϊ�˷������ϵͳ��������˿����ڴ˴�:
            //ˢ�����д���
            var editor = new TilesPreferenceEditor(this);
            foreach(var item in editor.Tiles)
            {
                Codes.UpdateTiles(item.Id, this, null, null);
            }
            //�����Զ�����Rss��ʡȥ�˼���ϵͳ������ɵ��鷳
            Codes.ArrangeRssUpdate(this);
        }

        public override void OnNotificationPosted(StatusBarNotification sbn)
        {
            //base.OnNotificationPosted(sbn);

            //�ж��յ���֪ͨ�Ƿ������ѹ̶�������Ӧ��
            TilesPreferenceEditor editor = new TilesPreferenceEditor(this);
            foreach (var item in editor.Tiles)
            {
                //�Ƿ������ѿ�����ʾ֪ͨ���ܵĴ�����Ӧ��
                if (item.ShowNotification && sbn.PackageName == item.Name)
                {
                    //�ж��յ���֪ͨ�Ƿ��ǿɽ����ı�׼��ʽ
                    if (sbn.Notification.Extras.GetString(Notification.ExtraTitle) != null)
                    {
                        string text = sbn.Notification.Extras.GetString(Notification.ExtraTitle, "����") + '\n' + sbn.Notification.Extras.GetString(Notification.ExtraText, "����");
                        //���Ͷ�̬����С��������
                        if (item.ShowNotifIcon) //�Ƿ�������ʾͼ��
                        {
                            Codes.UpdateTiles(item.Id, this, text, sbn.Notification.LargeIcon);
                        }
                        else
                        {
                            Codes.UpdateTiles(item.Id, this, text, null);
                        }
                    }
                    else
                    {
                        //����ֱ���հ�֪ͨ����,����С��������
                        Codes.UpdateTiles(item.Id, this, sbn.Notification, null);
                    }
                }
            }
        }

        public override void OnNotificationRemoved(StatusBarNotification sbn)
        {
            //base.OnNotificationRemoved(sbn);

            //�ж��յ���֪ͨ�Ƿ������ѹ̶�������Ӧ��
            TilesPreferenceEditor editor = new TilesPreferenceEditor(this);
            foreach (var item in editor.Tiles)
            {
                //�Ƿ������ѿ�����ʾ֪ͨ���ܵĴ�����Ӧ��
                if (item.ShowNotification && sbn.PackageName == item.Name)
                {
                    //����С��������
                    Codes.UpdateTiles(item.Id, this, null, null);
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