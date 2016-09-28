﻿using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DNVLibrary.Planning
{
    /// <summary>
    /// PAllPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PAllPanel : UserControl
    {
        public PAllPanel(DistNetLibrary.DistNet Distnet)
        {
            distnet = Distnet;
            InitializeComponent();
        }

        internal DistNetLibrary.DistNet distnet;

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            simData();

            timeline = new PAllTimeLine(this) { VerticalAlignment = System.Windows.VerticalAlignment.Bottom, Margin = new Thickness(250, 0, 250, 20) };
            timeline.selYearChanged += new EventHandler(timeline_selYearChanged);
            timeline.projectChanged += new EventHandler(timeline_projectChanged);
            timeline.PlayBegin += new EventHandler(timeline_PlayBegin);
            grdMain.Children.Add(timeline);
            timeline.selYear = startYear;

            gauge.btnMoreIndex.Click += new RoutedEventHandler(btnMoreIndex_Click);
            gauge.lstObjects.MouseDoubleClick += new MouseButtonEventHandler(lstObjects_MouseDoubleClick);
        }

        void lstObjects_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WpfEarthLibrary.PowerBasicObject obj = gauge.lstObjects.SelectedItem as WpfEarthLibrary.PowerBasicObject;
            distnet.scene.camera.aniLook(obj.VecLocation,5);

        }


        void timeline_PlayBegin(object sender, EventArgs e)
        {
            chkFlow.IsChecked = chkLoad.IsChecked = chkVL.IsChecked = chkNP1.IsChecked = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        ///<summary>年选中事件</summary>
        void timeline_selYearChanged(object sender, EventArgs e)
        {
            grdPrjview.Children.Clear();
            grdPrjview.Children.Add(years[timeline.selYear].view);

            pnlCompare.Visibility = years[timeline.selYear].projects.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

            if (aniidx.To == 1)  //年变化处理指标对比
            {
                if (years[timeline.selYear].projects.Count < 2)
                    hideindexcompare();
                else
                    showindexcompare();

            }
        }
        void timeline_projectChanged(object sender, EventArgs e)
        {
            oldprj = curprj;
            curprj = years[timeline.selYear].view.selprj.Tag as ProjectData;
            prjchange();
        }

        ProjectData oldprj, curprj;
        YearData curyear { get { return years[timeline.selYear]; } }

        PAllTimeLine timeline;

        Random rd = new Random();
        internal int startYear;
        internal int endYear;
        internal Dictionary<int, YearData> years = new Dictionary<int, YearData>();



        void simData()
        {
            startYear = 2017;
            endYear = 2030;

            int id = 0;
            YearData year = new YearData() { year = startYear };
            ProjectData pd;
            pd = new ProjectData() { prjid = id, year = year.year, name = " 配网现状", note = "配网现状情况。" };
            simindex(pd, (1.0 * year.year - startYear) / (endYear - startYear));
            year.projects.Add(pd);
            years.Add(startYear, year);
            gauge.nowPrj = pd;  //填写现状数据

            year = new YearData() { year = endYear };
            pd = new ProjectData() { prjid = id, year = year.year, name = " 规划方案" + id, note = string.Format(" 规划方案{0}。", id) };
            simindex(pd, (1.0 * year.year - startYear) / (endYear - startYear));
            year.projects.Add(pd);
            years.Add(endYear, year);
            for (int i = 0; i < 10 + rd.Next(7); i++)
            {
                id++;
                int tmp = startYear + rd.Next(endYear - startYear);
                if (tmp == startYear) continue;
                if (!years.TryGetValue(tmp, out year))
                {
                    year = new YearData() { year = tmp };
                    years.Add(tmp, year);
                }
                pd = new ProjectData() { prjid = id, year = year.year, name = " 规划方案" + id, note = string.Format(" 规划方案{0}。", id) };
                simindex(pd, (1.0 * year.year - startYear) / (endYear - startYear));
                year.projects.Add(pd);

            }
            id++;

            //----- 创建方案集合视图


            foreach (var item in years.Values)
            {
                PAllYearPorjects prjview = new PAllYearPorjects(item) { Margin = new Thickness(20, 0, 0, 50) };
                item.view = prjview;
                prjview.projectChanged += new EventHandler(prjview_projectChanged);
            }

            //----- 模拟投退运,仅设施、主配变、连接线路
            IEnumerable<WpfEarthLibrary.PowerBasicObject> tmpobjs;
            IEnumerable<WpfEarthLibrary.PowerBasicObject> objs = distnet.getAllObjListByCategory(DistNetLibrary.EObjectCategory.变电设施类);
            tmpobjs = distnet.getAllObjListByCategory(DistNetLibrary.EObjectCategory.开关设施类);
            objs = objs.Union(tmpobjs);
            objs = distnet.getAllObjListByCategory(DistNetLibrary.EObjectCategory.导线类);
            objs = objs.Union(tmpobjs);
            tmpobjs = distnet.getAllObjListByCategory(DistNetLibrary.EObjectCategory.变压器类);
            objs = objs.Union(tmpobjs);
            List<WpfEarthLibrary.PowerBasicObject> allobjs = objs.ToList();

            var allprjs = from e0 in years.Values
                          from e1 in e0.projects
                          select e1;
            foreach (ProjectData prj in allprjs.OrderBy(p => p.year))
            {
                if (prj.year == startYear) continue;  //跳过现状年
                for (int i = 0; i < 2 + rd.Next(3); i++)
                {
                    WpfEarthLibrary.PowerBasicObject obj = allobjs[rd.Next(allobjs.Count)];
                    if (obj.busiAccount is DistNetLibrary.AcntDistBase)
                    {
                        (obj.busiAccount as DistNetLibrary.AcntDistBase).runDate = new DateTime(prj.year, 1 + rd.Next(12), 1);  //模拟增加投运日期
                        prj.addobjs.Add(obj);  //加该设备为方案增量设备
                        obj.logicVisibility = false;  //初始设定该增量设备不可见
                    }
                }
            }



        }

        void simindex(ProjectData pd, double xs)
        {
            pd.idx1.value = 1.5 + rd.NextDouble();
            pd.idx2.value = 0.1 + 0.2 * rd.NextDouble();
            pd.idx3.value = 0.99 + 0.0099 * rd.NextDouble();

            foreach (var item in pd.idxes.indexes)
            {
                item.Value.simData(xs * (0.95 + 0.1 * rd.NextDouble()));
            }
        }


        void prjview_projectChanged(object sender, EventArgs e)
        {
            oldprj = curprj;
            curprj = (sender as PAllYearPorjects).selprj.Tag as ProjectData;
            prjchange();
        }

        ///<summary>方案改变</summary>
        void prjchange()
        {
            txtPrjName.Text = String.Format("{0}（{1}年）", curprj.name, curprj.year);
            gauge.prjdata = curprj;

            //===== 设备变化
            var allprjs = from e0 in years.Values
                          from e1 in e0.projects.Where(p => p.year > curprj.year || (p.year == curprj.year && p != curprj))
                          select e1;
            foreach (var e0 in allprjs)  //大于选定方案规划年的，以及同规划年的其它方案的增量设备隐藏 注：暂没实现退运和改造
            {
                foreach (var e1 in e0.addobjs)
                {
                    e1.logicVisibility = false;
                }
            }

            allprjs = from e0 in years.Values
                      from e1 in e0.projects.Where(p => p.year < curprj.year || p == curprj)
                      select e1;
            foreach (var e0 in allprjs)  //小于选定方案规划年的，本方案直接显示  注：暂没实现退运和改造
            {
                foreach (var e1 in e0.addobjs)
                {
                    e1.color = Colors.Cyan;
                    e1.logicVisibility = true;
                }
            }
            distnet.scene.UpdateModel();

            foreach (var e1 in curprj.addobjs)   //本方案，闪烁
            {
                if (e1 is WpfEarthLibrary.pPowerLine)
                {
                    e1.color = Colors.Red;
                    (e1 as WpfEarthLibrary.pPowerLine).aniTwinkle.doCount = 20;
                    (e1 as WpfEarthLibrary.pPowerLine).AnimationBegin(WpfEarthLibrary.pPowerLine.EAnimationType.闪烁);
                }
                else if (e1 is WpfEarthLibrary.pSymbolObject)
                {
                    e1.color = Colors.Red;
                    (e1 as WpfEarthLibrary.pSymbolObject).aniTwinkle.doCount = 20;
                    (e1 as WpfEarthLibrary.pSymbolObject).AnimationBegin(WpfEarthLibrary.pSymbolObject.EAnimationType.闪烁);
                }
            }




            //===== 运行显示
            Run.DataGenerator.StartGenData(distnet);
            Run.DataGenerator.StopGenData();
            refreshScreen();

        }

        void refreshScreen()
        {
            if ((bool)chkFlow.IsChecked)
                distnet.showFlow(null, null, false, true);
            if ((bool)chkLoad.IsChecked)
                distnet.showLoadCol();
            if ((bool)chkVL.IsChecked)
                distnet.showVLContour();
        }

        private void chkFlow_Checked(object sender, RoutedEventArgs e)
        {
            distnet.showFlow(null, null, false, true);
        }

        private void chkFlow_Unchecked(object sender, RoutedEventArgs e)
        {
            distnet.clearFlow();
        }

        private void chkLoad_Checked(object sender, RoutedEventArgs e)
        {
            distnet.showLoadCol();
        }

        private void chkLoad_Unchecked(object sender, RoutedEventArgs e)
        {
            distnet.clearLoadCol();
        }

        private void chkVL_Checked(object sender, RoutedEventArgs e)
        {
            distnet.showVLContour();
        }

        private void chkVL_Unchecked(object sender, RoutedEventArgs e)
        {
            distnet.clearVLContour();
        }

        private void chkNP1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkNP1_Unchecked(object sender, RoutedEventArgs e)
        {

        }


        #region ===== 指标鱼骨图相关 =====
        FishBone.UserControl1 fish;// = new FishBone.UserControl1() {Margin=new Thickness(200,200,200,100), HorizontalAlignment= HorizontalAlignment.Right, VerticalAlignment= VerticalAlignment.Bottom};
        System.Windows.Media.Animation.DoubleAnimation anifish = new System.Windows.Media.Animation.DoubleAnimation();
        System.Windows.Media.Animation.DoubleAnimation anidistnet = new System.Windows.Media.Animation.DoubleAnimation();
        bool isFishInited;
        void initfish()
        {
            if (isFishInited) return;
            isFishInited = true;
            fish = new FishBone.UserControl1();//{Margin=new Thickness(200,200,200,100), HorizontalAlignment= HorizontalAlignment.Right, VerticalAlignment= VerticalAlignment.Bottom};
            anifish.Duration = anidistnet.Duration = TimeSpan.FromSeconds(0.5);
            fish.Opacity = 0;
            grdAdd.Children.Add(fish);
            hidefish();
            fish.IsHitTestVisible = false;
        }

        void btnMoreIndex_Click(object sender, RoutedEventArgs e)
        {
   initfish();
            if (anifish.To != 1)
                showfish();
            else
                hidefish();
        }


        void showfish()
        {
            anifish.To = 1;
            fish.BeginAnimation(UserControl.OpacityProperty, anifish);
            anidistnet.To = 0.4;
            distnet.scene.BeginAnimation(UserControl.OpacityProperty, anidistnet);
            fish.IsHitTestVisible = true;
        }
        void hidefish()
        {
            anifish.To = 0;
            fish.BeginAnimation(UserControl.OpacityProperty, anifish);
            anidistnet.To = 1;
            distnet.scene.BeginAnimation(UserControl.OpacityProperty, anidistnet);
            fish.IsHitTestVisible = false;
        }



        #endregion

        #region ===== 指标对比表格 =====
        System.Windows.Media.Animation.DoubleAnimation aniidx = new System.Windows.Media.Animation.DoubleAnimation() { Duration = TimeSpan.FromSeconds(0.5) };

        private void btnCompareIndex_Click(object sender, RoutedEventArgs e)
        {
            if (aniidx.To != 1)
                showindexcompare();
            else
                hideindexcompare();


        }

        void showindexcompare()
        {
            //创建表
            DataTable dt = new DataTable();
            DataColumn dc;
            dc = new DataColumn("分类", typeof(string)); dt.Columns.Add(dc);
            dc = new DataColumn("指标", typeof(string)); dt.Columns.Add(dc);
            YearData yd = years[timeline.selYear];
            foreach (var item in yd.projects)
            {
                dc = new DataColumn(item.name, typeof(string)); dt.Columns.Add(dc);
            }
            //DataTable dtidx = DataLayer.DataProvider.getDataTableFromSQL("select cast(ID as nvarchar(100)) id, sort1,sort2,ord,indexname, definition,IMPORTANT,format,VALUE,UNIT,VALUENOTE,REFER1,REFER2,REFERTYPE,refernote from d_index where SORT0='863' order by ord,IMPORTANT");

            DataTable dtidx = DataLayer.DataProvider.getDataTableFromSQL("select cast(ID as nvarchar(100)) id, sort1,sort2,ord,indexname, definition,IMPORTANT,format,VALUE,UNIT,VALUENOTE,REFER1,REFER2,REFERTYPE,refernote from Dic_Index where Category='863' order by ord,IMPORTANT");
            //创建树的枝记录
            DataRow adddr;
            var tmp = from e0 in dtidx.AsEnumerable() group e0 by e0["sort2"];
            foreach (var ee in tmp)
            {
                adddr = dt.NewRow();
                adddr[1] = ee.Key;
                dt.Rows.Add(adddr);
            }

            //读指标
            foreach (DataRow dr in dtidx.Rows)
            {
                adddr = dt.NewRow();
                adddr[0] = dr["sort2"].ToString();
                adddr[1] = String.Format("{0}（{1}）", dr["indexname"], dr["unit"]);
                if (adddr[0].ToString() == adddr[1].ToString())
                    adddr[1] = dr["indexname"].ToString() + "指标";
                int idx = 2;
                foreach (var item in yd.projects)
                {
                    adddr[idx] = String.Format("{0}{1}", (double.Parse(dr["value"].ToString()) * (0.9 + 0.2 * rd.NextDouble())).ToString(dr["format"].ToString()), dr["unit"]);
                    idx++;
                }
                dt.Rows.Add(adddr);
            }
            gridIndexCompare.ItemsSource = null;
            gridIndexCompare.ItemsSource = dt;

            aniidx.To = 1;
            gridIndexCompare.BeginAnimation(Grid.OpacityProperty, aniidx);
            anidistnet.To = 0.4;
            distnet.scene.BeginAnimation(UserControl.OpacityProperty, anidistnet);
            gridIndexCompare.IsHitTestVisible = true;
        }
        void hideindexcompare()
        {
            aniidx.To = 0;
            gridIndexCompare.BeginAnimation(UserControl.OpacityProperty, aniidx);
            anidistnet.To = 1;
            distnet.scene.BeginAnimation(UserControl.OpacityProperty, anidistnet);
            gridIndexCompare.IsHitTestVisible = false;
        }
        private void gridIndexCompare_AutoGeneratingColumn(object sender, DevExpress.Xpf.Grid.AutoGeneratingColumnEventArgs e)
        {
            e.Column.EditSettings = new DevExpress.Xpf.Editors.Settings.TextEditSettings();
            e.Column.ActualEditSettings.HorizontalContentAlignment = DevExpress.Xpf.Editors.Settings.EditSettingsHorizontalAlignment.Center;
        }


        #endregion

        bool isGridCompare;
        private void btnCompareDistnet_Click(object sender, RoutedEventArgs e)
        {
            if (isGridCompare)
            {
                distnet.scene.objManager.restoreVisionProperty();

                curyear.view.colorVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {

                //===== 保存状态
                distnet.scene.objManager.clearVisionProperty();
                distnet.scene.objManager.saveVisionProperty();
                //===== 设置所有设备
                IEnumerable<WpfEarthLibrary.PowerBasicObject> objs = distnet.scene.objManager.getAllObjList();
                foreach (var item in objs)
                {
                    item.color = Color.FromArgb(80, 255, 255, 255);
                }

                //===== 设备变化
                var allprjs = from e0 in years.Values
                              from e1 in e0.projects.Where(p => p.year > curprj.year)
                              select e1;
                foreach (var e0 in allprjs)  //大于选定方案规划年的，以及同规划年的其它方案的增量设备隐藏 注：暂没实现退运和改造
                {
                    foreach (var e1 in e0.addobjs)
                    {
                        e1.logicVisibility = false;
                    }
                }

                allprjs = from e0 in years.Values
                          from e1 in e0.projects.Where(p => p.year == curprj.year)
                          select e1;
                foreach (var e0 in allprjs)  //本年各方案
                {
                    
                    foreach (var e1 in e0.addobjs)
                    {
                        e1.color = e0.color;
                        e1.logicVisibility = true;

                        if (e1 is WpfEarthLibrary.pPowerLine)
                        {
                            (e1 as WpfEarthLibrary.pPowerLine).aniTwinkle.doCount = 0;
                            (e1 as WpfEarthLibrary.pPowerLine).aniTwinkle.duration = 500;
                            (e1 as WpfEarthLibrary.pPowerLine).AnimationBegin(WpfEarthLibrary.pPowerLine.EAnimationType.闪烁);
                        }
                        else if (e1 is WpfEarthLibrary.pSymbolObject)
                        {
                            (e1 as WpfEarthLibrary.pSymbolObject).aniTwinkle.doCount = 0;
                            (e1 as WpfEarthLibrary.pSymbolObject).aniTwinkle.duration = 500;
                            (e1 as WpfEarthLibrary.pSymbolObject).AnimationBegin(WpfEarthLibrary.pSymbolObject.EAnimationType.闪烁);
                        }
                    }
                }
                distnet.scene.UpdateModel();

                curyear.view.colorVisibility = System.Windows.Visibility.Visible;

                //===== 运行显示
                Run.DataGenerator.StartGenData(distnet);
                Run.DataGenerator.StopGenData();
                refreshScreen();


            }
            isGridCompare = !isGridCompare;
        }

        private void chkForecast_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkForecast_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chkAuto_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chkAuto_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkReliability_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkReliability_Unchecked(object sender, RoutedEventArgs e)
        {

        }

    }



}
