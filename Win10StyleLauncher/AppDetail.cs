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
using Android.Graphics.Drawables;

namespace LiveTilesWidget
{
    /// <summary>
    /// һ�����Ա���̬����С����������Ӧ�õ���Ϣ,��һ����̬������������Ϣ
    /// </summary>
    public class AppDetail
    {
        /// <summary>
        /// ��ʾ�ı�ǩ����
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// ���������
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// ͼ��
        /// </summary>
        public Drawable Icon
        {
            get;
            set;
        }

        /// <summary>
        /// ��̬����С������Id
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ��ڴ�������ʾ����֪ͨ
        /// </summary>
        public bool ShowNotification
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ�ӱ�ֽ���Զ���ȡ�ʺϵı���ɫ
        /// </summary>
        public bool AutoTileColor
        {
            get;
            set;
        }

        /// <summary>
        /// ���ص�ǰ�����������Ӧ�õ�ͼ�굽Icon����
        /// </summary>
        /// <param name="context"></param>
        public void LoadIcon(Context context)
        {
            AppDetail app = null;
            foreach (var item in Codes.LoadApps(context.PackageManager))
            {
                if (item.Name == this.Name && item.Label == this.Label)
                {
                    app = item;
                    break;
                }
            }
            if (app != null)
            {
                this.Icon = app.Icon;
            }
        }
    }
}