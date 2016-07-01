using Android.App;
using Android.Content;
using Android.Graphics;
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
        public TilesPreferenceEditor(Context context)
        {
            //��ȡSharedPreferences����Editor
            preferences = context.GetSharedPreferences("tiles", FileCreationMode.Private);
            editor = preferences.Edit();
            tiles = new List<string>(preferences.GetStringSet("Tiles", new List<string>()));
            Tiles = new List<AppDetail>();

            //�����л���������
            foreach (var item in tiles)
            {
                AppDetail tile = JsonConvert.DeserializeObject<AppDetail>(item);
                //����ͼ��
                tile.LoadIcon(context);
                Tiles.Add(tile);
            }
        }

        //SharedPreferences����Editor
        private ISharedPreferences preferences;
        private ISharedPreferencesEditor editor;
        //�洢�������õ�StringSet
        private List<string> tiles;

        /// <summary>
        /// ���еĴ�����������Ϣ
        /// </summary>
        public List<AppDetail> Tiles
        {
            get;
            set;
        }

        /// <summary>
        /// ��ȡ����ָ��Id�Ĵ���С������������Ϣ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AppDetail GetTileById(int id)
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
            //���л���������
            List<string> list = new List<string>();
            foreach (var item in Tiles)
            {
                
                item.Icon = null;//���Icon���ԣ���ֹ����
                list.Add(JsonConvert.SerializeObject(item));
            }
            editor.PutStringSet("Tiles", list);
            editor.Commit();
        }
    }
}