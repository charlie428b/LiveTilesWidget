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

namespace LiveTilesWidget
{
    /*
     * ��лCSDN������xiaanming�������� http://blog.csdn.net/xiaanming/article/details/12684155 
     * �����򲿷ִ����Ǿ���ѧϰ�����µ�Java�������C#����д���ġ�
     */
    public class PinyinComparer : IComparer<TileDetail>//Comparator<SortModel> {
    {
        public int Compare(TileDetail x, TileDetail y)
        {
            //������Ҫ��������ListView��������ݸ���ABCDEFG...������
            if (y.GetSortLetters() == "#")
            {
                return -1;
            }
            else if (x.GetSortLetters() == "#")
            {
                return 1;
            }
            else
            {
                return x.GetSortLetters().CompareTo(y.GetSortLetters());
            }
        }
    }
}