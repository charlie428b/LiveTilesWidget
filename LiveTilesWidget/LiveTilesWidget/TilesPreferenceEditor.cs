using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveTilesWidget
{
    /// <summary>
    /// �ṩ�޸Ĵ������õĸ��ַ���
    /// </summary>
    public class TilesPreferenceEditor
    {
        /// <summary>
        /// ��ǰ��Preferences���ݸ�ʽ�汾��
        /// </summary>
        private const int CurrentPreferencesVersion = 2;

        /// <summary>
        /// ������ʼ��һ����ʵ��
        /// </summary>
        /// <param name="context"></param>
        public TilesPreferenceEditor(Context context) : this(context, false)
        {
        }
        /// <summary>
        /// ��ʼ��һ����ʵ��
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dontReadTiles">���ָ��Ϊtrue�򲻻ὫJSON��ʽ�洢�Ĵ������ö�ȡΪ����</param>
        public TilesPreferenceEditor(Context context, bool dontReadTiles)
        {
            _context = context;
            //��ȡSharedPreferences����Editor
            _preferences = Codes.GetPreferences(context);
            _editor = _preferences.Edit();
            //�Զ�Ǩ�ƾɰ�preferences�������°�
            if (PreferencesVersion < CurrentPreferencesVersion)
            {
                Codes.MovePreferences(PreferencesVersion, context);
                PreferencesVersion = CurrentPreferencesVersion;
            }
            _tiles = new List<string>(_preferences.GetStringSet("Tiles", new List<string>()));
            Tiles = new List<TileDetail>();

            _dontReadTiles = dontReadTiles;
            if (!dontReadTiles)
            {
                //�����л���������
                foreach (var item in _tiles)
                {
                    try
                    {
                        TileDetail tile = JsonConvert.DeserializeObject<TileDetail>(item);
                        //����ͼ��
                        tile.LoadIcon(context);
                        Tiles.Add(tile);
                    }
                    catch { }
                }
            }
        }

        //SharedPreferences����Editor
        private ISharedPreferences _preferences;
        private ISharedPreferencesEditor _editor;
        //�洢�������õ�StringSet
        private List<string> _tiles;
        //�����˵�ǰ�����������
        private Context _context;
        //�Ƿ�JSON��ʽ�洢�Ĵ������ö�ȡΪ����
        private bool _dontReadTiles;

        /// <summary>
        /// ���еĴ�����������Ϣ
        /// </summary>
        public List<TileDetail> Tiles
        {
            get;
            set;
        }

        /// <summary>
        /// ��ȡ����ָ��Id�Ĵ���С������������Ϣ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TileDetail GetTileById(int id)
        {
            foreach (var item in Tiles)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// �������жԴ������õ��޸�
        /// </summary>
        public void CommitChanges()
        {
            if (!_dontReadTiles)
            {
                //���л���������
                List<string> list = new List<string>();
                foreach (var item in Tiles)
                {
                    item.Icon = null;//���Icon���ԣ���ֹ����
                    list.Add(JsonConvert.SerializeObject(item));
                }
                _editor.PutStringSet("Tiles", list);
            }
                _editor.Commit();
        }

        /// <summary>
        /// SharedPreferencesʹ�õ����ݸ�ʽ�İ汾��
        /// </summary>
        public int PreferencesVersion
        {
            get
            {
                return _preferences.GetInt("PreferencesVersion", 0);
            }
            set
            {
                _editor.PutInt("PreferencesVersion", value);
                _editor.Commit();
            }
        }

        /// <summary>
        /// ȫ��Ĭ�ϴ�������ɫ�����Ļ��Զ�����
        /// </summary>
        public int DefaultTileColor
        {
            get
            {
                return _preferences.GetInt("DefaultTileColor", _context.GetColor(Resource.Color.lightblue500));
            }
            set
            {
                _editor.PutInt("DefaultTileColor", value);
                _editor.Commit();
            }
        }

        /// <summary>
        /// �ӱ�ֽ���Զ�ȡ�õĴ�������ɫ�����Ļ��Զ�����
        /// </summary>
        public int AutoTileColor
        {
            get
            {
                return _preferences.GetInt("AutoTileColor", _context.GetColor(Resource.Color.lightblue500));
            }
            set
            {
                _editor.PutInt("AutoTileColor", value);
                _editor.Commit();
            }
        }
    }
}