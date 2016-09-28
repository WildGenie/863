using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace WpfEarthLibrary
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class EarthManager 
    {
        public EarthManager(Earth pearth)
        {
            datas = new EarthData(this,null, 0, 0);
            earth = pearth;
            bworker.DoWork += new DoWorkEventHandler(bworker_DoWork);
            bworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bworker_RunWorkerCompleted);


            //==== ��ʼ��c#���
            float tmp = 1.001f * Para.scalepara;
            earth.camera = new Camera(new Vector3(2175.563f, 4090f, 4384f) * tmp, new Vector3(0, 0, 0), Vector3.Up, earth);

            //==== ��ʼ��d3d���

            initEarth();
        }

        internal Earth earth;

        internal enum ECalStatus { ����, ����, ���  }

        #region ===== �������� =====

        #region =============================== ƽ��ģʽ��� =================================
        private System.Windows.Rect _planeViewBox=new Rect(0,0,800,600);
        ///<summary>ƽ��ģʽ�³����ķ�Χ</summary>
        public System.Windows.Rect planeViewBox
        {
            get { return _planeViewBox; }
            set { _planeViewBox = value; planeAdjustCamera(); }
        }
        
        private float _planeCameraHeight=1.5f;
        ///<summary>ƽ��ģʽ����������߶�</summary>
        public float planeCameraHeight
        {
            get { return _planeCameraHeight; }
            set { _planeCameraHeight = value; }
        }

        ///<summary>����planeviewbox��planecameraheight�Զ��������λ��</summary>
        void planeAdjustCamera()
        {
            //�������λ��
            System.Windows.Point pnt = new System.Windows.Point(planeViewBox.Width / 2, planeViewBox.Height / 2);
            System.Windows.Point geopnt = geohelper.planeToGeo(pnt.ToString());
            VECTOR3D vc = MapHelper.JWHToPoint(geopnt.Y, geopnt.X, Para.LineHeight, earthpara);

            float tmp = 1.0f + planeCameraHeight / Para.Radius;
            earth.camera = new Camera(new Vector3(vc.x,vc.y,vc.z) * tmp, new Vector3(0, 0, 0), Vector3.Up, earth);
        }
        #endregion

        ///<summary>�Ƿ�������ʾ3D����</summary>
        public bool TerrainAllow {get;set;}
        ///<summary>������ʾ��ظ߶����ƣ���ʾ3D���ε������ظ߶ȣ�ֻ��С�ڸ�ֵ�Ż���ʾ3D���Σ�ȱʡ0</summary>
        public double TerrainMaxDistance { get; set; }
        ///<summary>������ʾ�����߽Ǹ߶����ƣ���ʾ3D���ε�������н�(�Ƕ�ֵ)��ֻ�д��ڸ�ֵ����ʾ3D���Σ�ȱʡ0</summary>
        public double TerrainMaxAngle { get; set; }
        ///<summary>�߳�ͼ��ɫ�������С�߶ȣ�ϵͳ�ڲ�����ϵ��</summary>
        public float TerrainMinHeight = 0;
        ///<summary>�߳�ͼ���߶�����minHeight+dropHeightΪ�߳�ͼ��ɫ��ʾ�����߶ȣ�ϵͳ�ڲ�����ϵ��</summary>
        public float TerrainDropHeight = 2;
        ///<summary>������ʾ������ƣ�������ʾ���ε�����ţ�ֻ�д��ڵ����������С�ڵ�����С��ŵ���Ƭ������ʾ����</summary>
        public int TerrainMaxLayer = 12;
        ///<summary>������ʾ������ƣ�������ʾ���ε���С���</summary>
        public int TerrainMinLayer = 10;
        ///<summary>������ʾʱ��TerrainMaxLayer���ԣ�������ʾ���ε�����ţ�����Ƭ���з���(ȱʡ16)����Ƭ��ÿС1���з���*2</summary>
        public int TerrainMaxLayerSliceCount = 16;

        //������Ʋ�����ʼ��
        public STRUCT_EarthPara earthpara = new STRUCT_EarthPara()
        {
            mapType = (int)EMapType.����, 
            isShowOverlay = true, 
            background= Helpler.ColorToUInt(System.Windows.Media.Colors.Black),
            SceneMode= ESceneMode.����,
            InputCoordinate= EInputCoordinate.WGS84��������,
            AdjustAspect=1.3,
            ArrowSpan=0.1
        };


        ///<summary>��ͼ����</summary>
        public EMapType mapType
        {
            get { return earth.earthManager.earthpara.mapType; }
            set
            {
                if (earth.earthManager.earthpara.mapType != value)
                {
                    earth.earthManager.earthpara.mapType = value;
                    if (value == EMapType.����)
                        earth.earthManager.earthpara.isShowOverlay = true;
                    else
                        earth.earthManager.earthpara.isShowOverlay = false;
                    earth.earthManager.updateEarthPara();
                    earth.refreshColor();
                    earth.global.isUpdate = true;
                }
            }
        }

        #endregion

        public void updateEarthPara()
        {
            //��ʵ������
            IntPtr ipPara = Marshal.AllocCoTaskMem(Marshal.SizeOf(earthpara));
            Marshal.StructureToPtr(earthpara, ipPara, false);
            D3DManager.ChangeProperty(earth.earthkey,(int)EModelType.��ͼ,(int)EPropertyType.���� , 0, 0, ipPara, 0,IntPtr.Zero,0);
            Marshal.FreeCoTaskMem(ipPara);
            //��map ip��ַ
            IntPtr ipLabel = Marshal.StringToCoTaskMemUni(Config.MapIP); //������Ƭ��ȡģʽ��IP��path
            D3DManager.ChangeProperty(earth.earthkey, (int)EModelType.��ͼ, (int)EPropertyType.��ַ, 0, 0, ipLabel, 0, IntPtr.Zero, 0);
            Marshal.FreeCoTaskMem(ipLabel);

            //��·��2 overlay
            if (earthpara.tileReadMode == ETileReadMode.�Զ����ļ���Ƭ || earthpara.tileReadMode == ETileReadMode.�Զ���Web��Ƭ)
            {
                ipLabel = Marshal.StringToCoTaskMemUni(Config.MapPath); //·��
                D3DManager.ChangeProperty(earth.earthkey, (int)EModelType.��ͼ, (int)EPropertyType.·��, 0, 0, ipLabel, 0, IntPtr.Zero, 0);
                Marshal.FreeCoTaskMem(ipLabel);
                ipLabel = Marshal.StringToCoTaskMemUni(Config.MapPath2); //·��2, overlay
                D3DManager.ChangeProperty(earth.earthkey, (int)EModelType.��ͼ, (int)EPropertyType.·��2, 0, 0, ipLabel, 0, IntPtr.Zero, 0);
                Marshal.FreeCoTaskMem(ipLabel);
            }
        }


        BackgroundWorker bworker = new BackgroundWorker();

        internal bool isShowEarth = true;


        ///<summary>��̨ɨ���Ԥ����</summary>
        void bworker_DoWork(object sender, DoWorkEventArgs e)
        {
            //earth.camera.cameraFrustum = new BoundingFrustum(earth.camera.view * Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4 * 1.2f, (float)earth.global.ScreenWidth / earth.global.ScreenHeight, earth.camera.Near, earth.camera.Far));
            earth.camera.cameraFrustum = new BoundingFrustum(earth.camera.view * earth.camera.projection);
            earth.global.maxlayer = 0;
            earth.global.maxlayertileinfo = "";

            scan(datas);

            int result = D3DManager.BeginTransfer(earth.earthkey);
            if (result != 0)
            {
                update(datas);
                D3DManager.EndTransfer(earth.earthkey);
            }

        }

        void bworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            calStatus = ECalStatus.���;
            
            earth.global.isUpdate = false;
        }

        internal ECalStatus calStatus = ECalStatus.����;
        ///<summary>���㲢�������ݣ���timer����</summary>
        public void updateEarth()
        {
            //����
            if (isShowEarth)
            {
                if (earth.global.isUpdate && calStatus == ECalStatus.����)
                {
                    calStatus = ECalStatus.����;
                    bworker.RunWorkerAsync();
                }
                if (calStatus == ECalStatus.���)
                {
                    //update(datas);
                    calStatus = ECalStatus.����;
                }
            }
        }


        #region ������Ƭ���

        internal EarthData datas;  //��������ӻ��ڵ���
        bool isCanUpdate = true;


        public delegate void UpdateModelDelegate();
        public UpdateModelDelegate updatemodel { get; set; }


        public void initEarth()
        {

            if (isCanUpdate)
            {

                //MapHelper.maxTerrainLayer = 0;
                isCanUpdate = false;

                //global.cameraFrustum = new BoundingFrustum(global.camera.view * Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4 * 1.2f, (float)global.ScreenWidth / global.ScreenHeight, 0.1f, 100f));
                //scan(datas);
                //update(datas);


                //if (updatemodel != null)
                    //updatemodel();

                isCanUpdate = true;

            }
        }



        ///<summary>��������ˢ�£����ò���ָ���״̬</summary>
        private void scan(EarthData node)
        {


            if (earth.global.maxlayer < node.layer) { earth.global.maxlayer = node.layer; earth.global.maxlayertileinfo = string.Format("{0}-{1}-{2}",node.layer,node.idxx,node.idxy); }

            node.handled = false;
            node.prehandled = false;
            node.tileStatus = ETileStatus.���ɼ�;
            node.isShowTerrain = false;
            node.info = "0";
            //Rect rectViewport = new Rect(-1000, -1000, earth.global.ScreenWidth + 2000, earth.global.ScreenHeight + 2000);
            Rect rectViewport = new Rect(0, 0, earth.global.ScreenWidth, earth.global.ScreenHeight);
            double LimitWH = 400;

            if ((earth.earthManager.mapType == EMapType.���� && node.layer + 1 <= Config.satmaxLayer) || (earth.earthManager.mapType != EMapType.���� && node.layer + 1 <= Config.roadmaxLayer))//��һ���������������
            {
                node.info = "1";
                //===�жϿɼ���
                //int ycount = (int)Math.Pow(2, node.layer);
                //double angle = 360.0 / ycount;



                if ((node.layer < (earthpara.SceneMode== ESceneMode.����? 3:earthpara.StartLayer) || isFaceAndIntersect(node)))   //����Ƿ����������㼶С��2������׶���ཻ
                {
                    //���ȿ��ǻ�һ����֤��С����ķ���������Ļת��Ϊ���棬���Աȣ����ַ��������õĿ�ת����Ļ�෴������ת����Ļ��б���²��ɵã���Ϊ��ƽ���������ƽ���ཻ�ˣ�

                    //���жϷ�ʽ������Ƭ��ʵ�ʾ�γ����Ļӳ����Ƚ����ж�
                    if (node.range.Contains(earth.camera.curCamearaViewRange.rect) )  //�������Ļ
                    {
                        node.setExpand();
                        foreach (EarthData item in node.subCurve)
                            scan(item);
                    }
                    else if (node.range.IntersectsWith(earth.camera.curCamearaViewRange.rect)) //������Ļ�ཻ
                    {
                        bool handle=false;
                        foreach (var rec in earth.camera.curCamearaViewRange.rects)
                        {
                            if (node.range.IntersectsWith(rec))
                            {
                                if (node.range.Width>0.3*rec.Width)  //zhע���Ժ�����Ż���ͨ��Ԥ�ȼ���γ�ȴ������ߣ��Լ���������ж��Ƿ�ֲ�
                                {
                                    node.setExpand();
                                    foreach (EarthData item in node.subCurve)
                                        scan(item);
                                }
                                else
                                {
                                    node.setFold();
                                    node.isShowTerrain = true;  //����ע��ֻ�пɼ��ſ�����ʾ����
                                }
                                handle = true;
                                break;
                            }
                        }
                        if (!handle) //ָ�ھ����е����ε����ಿ�֣�δ��������Ļ�ཻ
                            node.setFold();

                    }
                    else //���ཻ, Ӧ������
                    { 
                        node.setFold();
                    }

                    #region ===== �ɵ��жϷ�ʽ =====
                    //node.info = "2";
                    //Rect rectsub = MapHelper.GetTileRect(node,earth.global);
                    //node.rect = rectsub;
                    //if (rectsub.Contains(rectViewport) || node.layer < (earthpara.SceneMode == ESceneMode.���� ? 3 : earthpara.StartLayer)) //���������Ļ, ת��һ�㼶
                    //{
                    //    node.info = "3";
                    //    node.setExpand();
                    //    foreach (EarthData item in node.subCurve)
                    //        scan(item);
                    //}
                    //else
                    //{
                    //    node.info = "4";
                    //    if (rectViewport.IntersectsWith(rectsub)) //�ཻ
                    //    {
                    //        node.info = "5";

                    //        if (rectsub.Width * rectsub.Height > LimitWH * LimitWH || rectsub.Width>1000 )//������ж��Ƿ��з�
                    //        {
                    //            node.info = "6";
                    //            node.setExpand();
                    //            foreach (EarthData item in node.subCurve)
                    //                scan(item);
                    //        }
                    //        else
                    //        {
                    //            node.info = "7";
                    //            node.tileStatus = ETileStatus.���ֿɼ�;
                    //            node.isShowTerrain = true;
                    //            node.setFold();
                    //            node.texture = "box3";
                    //        }
                    //    }
                    //    else //���ཻ, �ˣ�ԭ����Ӧ���֣���3D�ཻ��2D���ཻ�������������⣬����3D��Χ�� 
                    //    {
                    //        if (rectsub.Width > 5000)
                    //        {
                    //            node.setExpand();
                    //            foreach (EarthData item in node.subCurve)
                    //                scan(item);
                    //        }
                    //        else
                    //        {
                    //            node.info = "8";
                    //            node.tileStatus = ETileStatus.���ֿɼ�;
                    //            node.isShowTerrain = true;
                    //            //node.setDelete();
                    //            node.setFold();
                    //            node.texture = "box2";
                    //        }
                    //    }
                    //}

                    #endregion

                }
                else //���ɼ�
                {
                    //node.setDelete();
                    node.setFold();


                    node.texture = "box";
                }
            }
            else //��С��
            {
                node.info = "a";
                //node.setDelete();
                node.setFold();
                node.texture = "box4";
            }



        }

  

        /// <summary>
        /// �������в���
        /// </summary>
        /// <param name="node"></param>
        private void update(EarthData node)  //�������и��²���
        {
            while (node.subCurve.Count(p => !p.handled) > 0)
            {
                update(node.subCurve.First(p => !p.handled));
            }

            //int nn;
            //if (node.layer >= 17)
            //{
            //    int a = 0;
            //    a = 10;
            //}

            switch (node.operate)
            {
                case EOperate.�ڵ�ģ��ɾ��:
                    //mapTiles.Remove(node.id);
                    //Earth.DelMapTile(node.layer,node.idxx,node.idxy);
                    node.parent.subCurve.Remove(node);
                    node.parent = null;
                    break;
                case EOperate.�ڵ�ɾ��:
                    node.parent.subCurve.Remove(node);
                    node.parent = null;
                    break;
                case EOperate.ģ��ɾ��:
                    //mapTiles.Remove(node.id);
                    //Earth.DelMapTile(node.layer,node.idxx,node.idxy);
                    node.curStatus = node.oprStatus;
                    break;
                case EOperate.ģ�ͼ���:
                    //mapTiles.Add(node.id, updateTiles[node.id]);
                    {
                        if (node.mustModelStatus == EMeshStatus.��ά)
                        {
                            int count = node.terrainHeigList.Count;
                            IntPtr ipHigh = Marshal.AllocCoTaskMem(Marshal.SizeOf(node.terrainHeigList[0]) * count);  //���ݵ����нṹ����ָ��
                            for (int i = 0; i < count; i++)
                                Marshal.StructureToPtr(node.terrainHeigList[i], (IntPtr)(ipHigh.ToInt32() + i * Marshal.SizeOf(node.terrainHeigList[i])), false);
                            D3DManager.AddMapTile(earth.earthkey, node.hashcode, node.layer, node.idxx, node.idxy, node.mustModelStatus == EMeshStatus.��ά, node.terrainSliceCount, ipHigh); // ��isshowterrainΪtrue��terrainspan��ԭֵ��ͬ�������ؽ�����mesh
                            Marshal.FreeCoTaskMem(ipHigh);
                        }
                        else
                            D3DManager.AddMapTile(earth.earthkey, node.hashcode, node.layer, node.idxx, node.idxy, false, 0, IntPtr.Zero);  //isShowTerrainΪfalseʱ��������ĵ���������ݣ���Ϊ���ؿ���
                    }
                    node.curStatus = node.oprStatus;
                    break;
                case EOperate.ģ�͸���:
                    {
                        if (node.mustModelStatus == EMeshStatus.��ά)
                        {
                            int count = node.terrainHeigList.Count;
                            IntPtr ipHigh = Marshal.AllocCoTaskMem(Marshal.SizeOf(node.terrainHeigList[0]) * count);  //���ݵ����нṹ����ָ��
                            for (int i = 0; i < count; i++)
                                Marshal.StructureToPtr(node.terrainHeigList[i], (IntPtr)(ipHigh.ToInt32() + i * Marshal.SizeOf(node.terrainHeigList[i])), false);
                            D3DManager.AddMapTile(earth.earthkey, node.hashcode, node.layer, node.idxx, node.idxy, node.mustModelStatus == EMeshStatus.��ά, node.terrainSliceCount, ipHigh); // ��isshowterrainΪtrue��terrainspan��ԭֵ��ͬ�������ؽ�����mesh
                            Marshal.FreeCoTaskMem(ipHigh);
                        }
                        else
                            D3DManager.AddMapTile(earth.earthkey, node.hashcode, node.layer, node.idxx, node.idxy, false, 0, IntPtr.Zero);  //isShowTerrainΪfalseʱ��������ĵ���������ݣ���Ϊ���ؿ���
                    }

                    //D3DManager.AddMapTile(earth.earthkey, node.hashcode, node.layer, node.idxx, node.idxy);  


                    //models.Remove(node.model);
                    //node.updateModel();
                    //models.Add(node.model);
                    //node.curStatus = node.oprStatus;
                    break;
                case EOperate.none:
                    node.curStatus = node.oprStatus;
                    break;
            }
            node.operate = EOperate.none;
            node.handled = true;




        }


        /// <summary>�жϻ�����Ľǵ��Ƿ��������������������׶���ཻ</summary>
        private bool isFaceAndIntersect(EarthData node)
        {

            Vector3 vecDir = earth.camera.cameraDirection - earth.camera.cameraPosition;
            Vector3[] normals = node.normals;
            bool isface = false;
            for (int i = 0; i < normals.Length; i++)
            {
                if (Vector3.Dot(normals[i], vecDir) < 0)
                {
                    isface = true;
                    break;
                }
            }

            if (isface)
            {
                //BoundingFrustum cameraFrustum = new BoundingFrustum(global.camera.view * global.camera.projection);
                //BoundingFrustum cameraFrustum = new BoundingFrustum(global.camera.view * Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4*1.2f, (float)global.ScreenWidth / global.ScreenHeight, 0.1f, 100f));
                //if (cameraFrustum.Intersects(node.boundingBox))
                if (earth.camera.cameraFrustum.Intersects(node.boundingBox))
                    return true;
            }




            return false;
        }

        #endregion


        ///<summary>���س�����3D���ڳ���ƽ�������</summary>
        public  System.Windows.Point transformD3DToScreen(VECTOR3D point3d)
        {
            POINT p = D3DManager.TransformD3DToScreen(earth.earthkey, point3d);
            return new System.Windows.Point(p.x, p.y);
        }

        public VECTOR3D? transformScreenToD3D(System.Windows.Point point)
        {
            Vector3? result = Helpler.GetProjectPoint3D(new Vector2((float)point.X, (float)point.Y), earth.camera, earth.global.ScreenWidth, earth.global.ScreenHeight,0, earth.earthManager.earthpara);
            if (result == null)
                return null;
            else
            {
                Vector3 v =(Vector3)result;
                return new VECTOR3D(v.X, v.Y, v.Z);
            }
        }

    }
}
