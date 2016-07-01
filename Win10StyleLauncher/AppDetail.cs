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
using System.Text.RegularExpressions;

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
        public byte[] Icon
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
        /// ���ص�ǰ�����������Ӧ�õ�ͼ��
        /// </summary>
        /// <param name="context"></param>
        public Bitmap LoadIcon(Context context)
        {
            return ((BitmapDrawable)context.PackageManager.GetActivityIcon(context.PackageManager.GetLaunchIntentForPackage(Name))).Bitmap;
        }

        /// <summary>
        /// ��ȡ������ListView����������ģ�ƴ��������ĸ
        /// </summary>
        /// <returns></returns>
        public string GetSortLetters()
        {
            // ������ʽ���ж�����ĸ�Ƿ���Ӣ����ĸ  
            Regex reg = new Regex("[A-Z]");
            if (reg.IsMatch(Label.Substring(0, 1).ToUpper()))
            {
                return Label.Substring(0, 1).ToUpper();
            }
            //����ת����ƴ��
            string pinyin = CharacterParser.StrConvertToPinyin(Label.Substring(0, 1));
            string sortString = pinyin.Substring(0, 1).ToUpper();

            // ������ʽ���ж�����ĸ�Ƿ���Ӣ����ĸ  
            if (reg.IsMatch(sortString))
            {
                return sortString;
            }
            else
            {
                return "#";
            }

        }
    }
}