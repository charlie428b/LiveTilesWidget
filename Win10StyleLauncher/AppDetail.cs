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
using Android.Graphics;

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
        public Bitmap Icon
        {
            get;
            set;
        }
        //private Bitmap icon;

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
            Icon = ((BitmapDrawable)context.PackageManager.GetActivityIcon(context.PackageManager.GetLaunchIntentForPackage(Name))).Bitmap;
        }
    }
}