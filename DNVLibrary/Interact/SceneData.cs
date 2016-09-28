using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DNVLibrary.Interact
{
    #region ===== ʱ���� =====
    ///<summary>ʱ�����ݼ���</summary>
    class TimeDatas
    {
        public TimeDatas()
        {
            items = new List<TimeDataItem>();
        }

        public List<TimeDataItem> items { get; set; }

        public double maxidx1 { get { return items.Max(p => p.idx1.value); } }
        public double maxidx2 { get { return items.Max(p => p.idx2.value); } }
        public double maxidx3 { get { return items.Max(p => p.idx3.value); } }
        public double maxidx4 { get { return items.Max(p => p.idx4.value); } }
        public double maxidx5 { get { return items.Max(p => p.idx5.value); } }
        public double maxidx6 { get { return items.Max(p => p.idx6.value); } }


        public DateTime starttime { get { return items.Min(p => p.time); } }
        public DateTime endtime { get { return items.Max(p => p.time); } }
    }

    ///<summary>ʱ����������</summary>
    class TimeDataItem
    {
        public TimeDataItem(TimeDatas Datas)
        {
            datas = Datas;
            idx1 = new Index1();
            idx2 = new Index2();
            idx3 = new Index3();
            idx4 = new Index4();
            idx5 = new Index5();
            idx6 = new Index6();
        }
        public TimeDatas datas;

        public DateTime time { get; set; }

        public Index1 idx1 { get; set; }
        public Index2 idx2 { get; set; }
        public Index3 idx3 { get; set; }
        public Index4 idx4 { get; set; }
        public Index5 idx5 { get; set; }
        public Index6 idx6 { get; set; }

        public double load { get; set; } //���縺��
        public double greenpower { get; set; } //��ɫ����
        public double autoload { get; set; }  //���׮����

    }
    #endregion

    #region ===== ������ =====
    ///<summary>����������</summary>
    class SceneDatas
    {
        public SceneDatas()
        {
            items = new List<SceneDataItem>();
        }

        public List<SceneDataItem> items { get; set; }

        public double maxidx1 { get { return items.Max(p => p.idx1.value); } }
        public double maxidx2 { get { return items.Max(p => p.idx2.value); } }
        public double maxidx3 { get { return items.Max(p => p.idx3.value); } }
        public double maxidx4 { get { return items.Max(p => p.idx4.value); } }
        public double maxidx5 { get { return items.Max(p => p.idx5.value); } }
        public double maxidx6 { get { return items.Max(p => p.idx6.value); } }

        public void init()
        {
            foreach (var item in items)
            {
                item.button = new SceneButton(item);
            }
        }
    }

    ///<summary>����������</summary>
    class SceneDataItem
    {
        public SceneDataItem(SceneDatas Datas)
        {
            datas = Datas;
            idx1 = new Index1();
            idx2 = new Index2();
            idx3 = new Index3();
            idx4 = new Index4();
            idx5 = new Index5();
            idx6 = new Index6();
        }
        public SceneDatas datas;

        public int num { get; set; }  
        public int hours { get; set; } 

        public Index1 idx1 { get; set; }
        public Index2 idx2 { get; set; }
        public Index3 idx3 { get; set; }
        public Index4 idx4 { get; set; }
        public Index5 idx5 { get; set; }
        public Index6 idx6 { get; set; }

        public string info { get { return string.Format("����{0}({1}h)��{2};{3};{4}", num, hours, idx1.info, idx2.info, idx3.info); } }

        public SceneButton button { get; set; }
    }

    ///<summary>������ť��</summary>
    class SceneButton : Border
    {
        public SceneButton(SceneDataItem Data)
        {
            data = Data;
            Width = 27;
            Height = 27;
            Background = Brushes.Black;
            BorderThickness = new Thickness(1);
            Margin = new Thickness(1.5, 1, 1.5, 0);
            BorderBrush = Brushes.Gray;
            Cursor = Cursors.Hand;

            Grid grd = new Grid();
            this.Child = grd;
            Rectangle rect;
            rect = new Rectangle() { Width = 2, Height = 30.0 * data.idx1.value / data.datas.maxidx1, Fill = Brushes.Red, Margin = new Thickness(6, 0, 0, 0), VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left };
            grd.Children.Add(rect);
            rect = new Rectangle() { Width = 2, Height = 30.0 * data.idx2.value / data.datas.maxidx2, Fill = Brushes.Green, Margin = new Thickness(12, 0, 0, 0), VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left };
            grd.Children.Add(rect);
            rect = new Rectangle() { Width = 2, Height = 30.0 * data.idx3.value / data.datas.maxidx3, Fill = Brushes.Blue, Margin = new Thickness(20, 0, 0, 0), VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left };
            grd.Children.Add(rect);
            txtNum = new TextBlock() { Text = data.num.ToString(), Foreground = new SolidColorBrush(Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF)), FontSize = 20, FontWeight = FontWeights.ExtraBold, FontStretch = FontStretches.ExtraExpanded, VerticalAlignment = System.Windows.VerticalAlignment.Center, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            grd.Children.Add(txtNum);


        }

        internal SceneDataItem data;

        public TextBlock txtNum;

    }

    #endregion

    #region ----- ָ�궨�� -----
    abstract class IndexBase
    {
        public string name { get; set; }
        public string unit { get; set; }
        public double value { get; set; }
        public string note { get; set; }

        ///<summary>������Ϊname,value,unit,note, ȱʡΪ{0}{1:f0}{2}</summary>
        public string format { get; set; }

        public double min { get; set; } 
        public double max { get; set; }

        public string labformat {get;set;}  //�Ǳ��ǩ��ʽ
        public string lab0 { get { return string.Format(labformat, min + (max - min) / 5 * 0); } }  //�Ǳ��ñ�ǩ
        public string lab1 { get { return string.Format(labformat, min + (max - min) / 5 * 1); } }
        public string lab2 { get { return string.Format(labformat, min + (max - min) / 5 * 2); } }
        public string lab3 { get { return string.Format(labformat, min + (max - min) / 5 * 3); } }
        public string lab4 { get { return string.Format(labformat, min + (max - min) / 5 * 4); } }
        public string lab5 { get { return string.Format(labformat, min + (max - min) / 5 * 5); } }
        public double loc0 {get {return min+0.02*(max-min); }}
        public double loc1 {get {return min+0.22*(max-min); }}
        public double loc2 {get {return min+0.385*(max-min); }}
        public double loc3 {get {return min+0.57*(max-min); }}
        public double loc4 {get {return min+0.78*(max-min); }}
        public double loc5 {get {return min+1.02*(max-min); }}

        public string info { get { return string.Format(format,name,value,unit,note); } }  

    }

    class Index1:IndexBase
    {
        public Index1()
        {
            name = "ƽ��������";
            unit = "%";
            note = "�豸�������ʵ�����ƽ��ֵ";
            format = "{0} {1:p1} ";
            min = 0;
            max = 1;
            labformat = "{0:p0}";

        }
    }
    class Index2 : IndexBase
    {
        public Index2()
        {
            name = "ƽ����ѹ����ֵ";
            unit = "";
            note = "���ڵ��ѹ����ֵ������ƽ��ֵ";
            format = "{0} {1:f2} ";
            min = 0.9;
            max = 1.1;
            labformat = "{0:f2}";
        }
    }
    class Index3 : IndexBase
    {
        public Index3()
        {
            name = "���縺��";
            unit = "MW";
            note = "���縺������";
            format = "{0} {1:f0}{2} ";
            min = 0;
            max = 5000;
            labformat = "{0:f0}";
        }
    }
    class Index4 : IndexBase
    {
        public Index4()
        {
            name = "�����Դ��͸��";
            unit = "%";
            note = "�����Դռ���縺�ɵı���";
            format = "{0} {1:p1} ";
            min = 0;
            max = 0.5;
            labformat = "{0:p0}";
        }
    }
    class Index5 : IndexBase
    {
        public Index5()
        {
            name = "����ɿ���";
            unit = "%";
            note = "ͨ�������豸�ɿ��Ժ͹���ת�������õ����ۺϹ���ɿ���ָ�ꡣ";
            format = "{0} {1:p1} ";
            min = 0.99;
            max = 1;
            labformat = "{0:p1}";
        }
    }
    class Index6 : IndexBase
    {
        public Index6()
        {
            name = "���Ը���ռ����";
            unit = "%";
            note = "ָ���Ը��ɵ�ʹ����";
            format = "{0} {1:p1} ";
            min = 0;
            max = 1;
            labformat = "{0:p0}";
        }
    }

    #endregion

}
