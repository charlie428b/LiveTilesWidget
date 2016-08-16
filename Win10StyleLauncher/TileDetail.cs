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
using System.ComponentModel;

namespace LiveTilesWidget
{
    /// <summary>
    /// һ�����Ա���̬����С����������Ӧ�õ���Ϣ,��һ����̬������������Ϣ
    /// </summary>
    public class TileDetail : INotifyPropertyChanged
    {
        //Fields
        private string _label;
        private string _name;
        private bool _showNotification;
        private int _tileColor;
        private bool _showNotifIcon;
        private LiveTileType _tileType;
        private string _rssUrl;

        //Properties
        /// <summary>
        /// ��ʾ�ı�ǩ����
        /// </summary>
        public string Label
        {
            get
            {
                return _label;
            }

            set
            {
                _label = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
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
            get
            {
                return _showNotification;
            }

            set
            {
                _showNotification = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowNotification"));
            }
        }

        /// <summary>
        /// �����ı���ɫ��-1Ϊ�Զ���-2Ϊȫ���Զ���ɫ
        /// </summary>
        public int TileColor
        {
            get
            {
                return _tileColor;
            }

            set
            {
                _tileColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TileColor"));
            }
        }

        /// <summary>
        /// �Ƿ�����֪ͨʱ��ʾ֪ͨ��ͼ��
        /// </summary>
        public bool ShowNotifIcon
        {
            get
            {
                return _showNotifIcon;
            }

            set
            {
                _showNotifIcon = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowNotifIcon"));
            }
        }

        /// <summary>
        /// ��ǰ��̬������̬���ݵ�����
        /// </summary>
        public LiveTileType TileType
        {
            get
            {
                return _tileType;
            }

            set
            {
                _tileType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TileType"));
            }
        }

        /// <summary>
        /// ���ڸ��¶�̬������̬���ݵ�Rss Url
        /// </summary>
        public string RssUrl
        {
            get
            {
                return _rssUrl;
            }

            set
            {
                _rssUrl = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RssUrl"));
            }
        }

        //Events
        public event PropertyChangedEventHandler PropertyChanged;

        //Methods
        /// <summary>
        /// ���ص�ǰ�����������Ӧ�õ�ͼ�굽Icon����
        /// </summary>
        /// <param name="context"></param>
        public void LoadIcon(Context context)
        {
            Icon = ((BitmapDrawable)context.PackageManager.GetActivityIcon(context.PackageManager.GetLaunchIntentForPackage(Name))).Bitmap;
        }

        /// <summary>
        /// ��ȡ������ListView����������ģ�ƴ��������ĸ
        /// </summary>
        /// <returns></returns>
        public string GetSortLetters()
        {
            // ������ʽ���ж�����ĸ�Ƿ���Ӣ����ĸ  
            Regex reg = new Regex("[A-Z]");

            //����ת����ƴ��
            string pinyin = CharacterParser.GetCharSpellCode(Label.Substring(0, 1));

            // ������ʽ���ж�����ĸ�Ƿ���Ӣ����ĸ  
            if (reg.IsMatch(pinyin))
            {
                return pinyin;
            }
            else
            {
                return "#";
            }
        }
    }

    /// <summary>
    /// ��̬������̬���ݵ�����
    /// </summary>
    public enum LiveTileType
    {
        /// <summary>
        /// �����մ������ã�����֪ͨʱ��ʾ֪ͨ���ݣ���������̬����
        /// </summary>
        None,
        /// <summary>
        /// ��ָ��RSSԴ���¶�̬����
        /// </summary>
        Rss
    }
}