using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using WpfEarthLibrary;
using System.Windows.Threading;
using WpfEarthLibrary;

namespace DistNetLibrary
{
    public static class Extensions
    {

        #region ========== ��չ���Ӷ��󷽷� ==========
        ///<summary>����ָ���ֶε�˫�������ݣ�ת�����󷵻�double.NaN</summary>
        public static double getDouble(this DataRow dr, string fieldname)
        {
            double result;
            if (double.TryParse(dr[fieldname].ToString(), out result))
                return result;
            else
                return double.NaN;
        }

        ///<summary>����ָ���ֶε��������ݣ�ת�����󷵻�0��������һ�����</summary>
        public static int getInt(this DataRow dr, string fieldname)
        {
            int result;
            if (int.TryParse(dr[fieldname].ToString(), out result))
                return result;
            else
                return 0;
        }

        ///<summary>����ָ���ֶεĲ������ݣ�ת�����󷵻�false</summary>
        public static bool getBool(this DataRow dr, string fieldname)
        {
            bool result;
            if (bool.TryParse(dr[fieldname].ToString(), out result))
                return result;
            else
                return false;
        }


        ///<summary>����ָ���ֶε��������ݣ�ת�����󷵻�-1��������-1��ʾ��ȷ����ö�����</summary>
        public static int getIntN1(this DataRow dr, string fieldname)
        {
            int result;
            if (int.TryParse(dr[fieldname].ToString(), out result))
                return result;
            else
                return -1;
        }

        ///<summary>����ָ���ֶε����������ݣ�ת�����󷵻�1900.1.1</summary>
        public static DateTime getDatetime(this DataRow dr, string fieldname)
        {
            DateTime result;
            if (DateTime.TryParse(dr[fieldname].ToString(), out result))
                return result;
            else
                return new DateTime(1900, 1, 1);
        }

        ///<summary>����ָ���ֶε��ַ���������</summary>
        public static string getString(this DataRow dr, string fieldname)
        {
            return dr[fieldname].ToString();
        }


        #endregion


    }
}
