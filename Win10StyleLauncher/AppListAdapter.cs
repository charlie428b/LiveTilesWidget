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
using Java.Lang;

namespace LiveTilesWidget
{
    /// <summary>
    /// ����������AppDetailΪ���ݵ�ListView
    /// </summary>
    public class AppListAdapter : ArrayAdapter<AppDetail>, ISectionIndexer
    {
        Dictionary<string, int> alphaindex;
        Java.Lang.Object[] sectionsObjects;
        string[] sections;

        public AppListAdapter(Context context, int textViewResourceId, AppDetail[] objects)
            : base(context, textViewResourceId, objects)
        {
            alphaindex = new Dictionary<string, int>();

            //��ȡÿ�ֹؼ��ֵ���ʼ��������
            for (int i = 0; i < objects.Length; i++)
            {
                //objects[i].Label[0].ToString();
                string key = objects[i].GetSortLetters();
                if (!alphaindex.ContainsKey(key))
                {
                    alphaindex.Add(key, i);
                }
            }

            //���ؼ���ת��������
            sections = new string[alphaindex.Keys.Count];
            alphaindex.Keys.CopyTo(sections, 0);

            //���ؼ���ת����Java.Lang.String����
            sectionsObjects = new Java.Lang.Object[alphaindex.Keys.Count];
            for (int i = 0; i < sections.Length; i++)
            {
                sectionsObjects[i] = new Java.Lang.String(sections[i]);
            }

        }

        public int GetPositionForSection(int sectionIndex)
        {
            //���ݹؼ���������ȡ�ؼ��֣�Ȼ���ڸ��ݹؼ��ִ�alphaindex��ȡ��Ӧ��value�����ùؼ��ֵ���ʼ��������
            return alphaindex[sections[sectionIndex]];

        }

        public int GetSectionForPosition(int position)
        {
            int preposition = 0;
            //ѭ���ؼ���
            for (int i = 0; i < sections.Length; i++)
            {
                //�жϵ�ǰ�������Ƿ���i���ڹؼ��ֵķ�Χ��
                if (GetPositionForSection(i) > position)
                    break;
                preposition = i;
            }
            return preposition;
        }

        public Java.Lang.Object[] GetSections()
        {
            return sectionsObjects;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = convertView;
            if (v == null)
            {
                v = ((Activity)Context).LayoutInflater.Inflate(Resource.Layout.AppPickerItems, null);
            }

            ImageView appIcon = v.FindViewById<ImageView>(Resource.Id.item_app_icon);
            appIcon.SetImageBitmap(GetItem(position).Icon);

            TextView appLabel = v.FindViewById<TextView>(Resource.Id.item_app_label);
            appLabel.Text = GetItem(position).Label;

            return v;
        }
    }
}