using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Runtime.InteropServices;


namespace WpfEarthLibrary
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera
    {
        public Camera(Vector3 pos, Vector3 target, Vector3 up, Earth pearth)
        {
            earth = pearth;
            cameraPosition = pos;
            cameraLookat = target;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            calCameraByDirection();
            tmr.Interval = TimeSpan.FromMilliseconds(1000);
            tmr.Tick += new EventHandler(tmr_Tick);
        }


        Earth earth;

        internal Matrix view { get; set; }
        internal Matrix projection { get; set; }


        private EOperateMode _operateMode;
        ///<summary>�û�����ģʽ,�������������Ӧ�û����������û�����Ϊ�켣��ģʽʱ����ָ���켣�����ģ�traceBallCenter���ԣ�</summary>
        public EOperateMode operateMode
        {
            get { return _operateMode; }
            set
            {
                _operateMode = value;
                if (value == EOperateMode.�����ӽ�)
                    earth.Cursor = System.Windows.Input.Cursors.None;
                else
                    earth.Cursor = System.Windows.Input.Cursors.Arrow;
            }
        }

        ///<summary>���û�����Ϊ�켣��ģʽʱ����ָ���켣�����ģ���ά�����е����꣩</summary>
        public System.Windows.Media.Media3D.Point3D traceBallCenter { get; set; }


        private float _moveDistance = 0.00005f;
        ///<summary>���û�����Ϊ�����ӽ�ģʽʱ��ǰ������λ�ƾ��룬ȱʡ0.00005</summary>
        public float moveDistance
        {
            get { return _moveDistance; }
            set { _moveDistance = value; }
        }

        ///<summary>����Ļ�ҷ���mousr�ƶ������������������תΪX����ת������ȱʡ0.9</summary>
        public double XRotationScale = 0.9;

        ///<summary>�������������С���룬ע��Ӧ�������ƽ���������Ӧ</summary>
        public float MinGroundDistance = 1;
        ///<summary>����������������룬ע��Ӧ�����Զƽ���������Ӧ</summary>
        public float MaxGroundDistance = 10;


        Vector3 _position;
        public Vector3 cameraPosition
        {
            get { return _position; }
            set
            {
                _position = value;
                //_direction = _lookat - _position;
                //_direction.Normalize();
            }
        }
        Vector3 _lookat;
        public Vector3 cameraLookat
        {
            get { return _lookat; }
            set
            {
                _lookat = value;
                //_direction = _lookat - _position;
                //_direction.Normalize();
            }
        }
        //Vector3 _direction;
        public Vector3 cameraDirection;
        //{
        //    get { return _direction; }
        //    set
        //    {
        //        _direction = value;
        //        //_lookat = _position + _direction;
        //    }
        //}

        public Vector3 cameraUp { get; set; }
        public float FieldOfView = MathHelper.PiOver4;
        public float Near = 0.1f;
        public float Far = 10f;

        internal BoundingFrustum cameraFrustum; //��������׶�壬һ�α��������У��̶�����


        public void calCameraByDirection()
        {
            cameraDirection.Normalize();
            cameraLookat = cameraPosition + cameraDirection;

            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
            projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, 1.0f * earth.global.ScreenWidth / earth.global.ScreenHeight, Near, Far);
        }
        public void calCameraByLookAt()
        {
            cameraDirection = _lookat - _position;
            cameraDirection.Normalize();

            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
            projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, 1.0f * earth.global.ScreenWidth / earth.global.ScreenHeight, Near, Far);
        }

        BoundingSphere boundsphere = new BoundingSphere(new Vector3(0, 0, 0), Para.Radius);
        Plane plane = new Plane(new Vector3(0, 0, 1), 0);
        ///<summary>��ȡ������������Ľ���, crosspointΪ����, distanceΪ����뽻�����</summary>
        internal void getCrossGround(out Vector3? crosspoint, out float? distance)
        {
            Ray ray = new Ray(cameraPosition, cameraDirection);
            if (earth.earthManager.earthpara.SceneMode == ESceneMode.����)
                distance = ray.Intersects(boundsphere);
            else
                distance = ray.Intersects(plane);
            cameraDirection.Normalize();
            if (distance == null)
                crosspoint = null;
            else
                crosspoint = cameraPosition + cameraDirection * distance;
        }
        ///<summary>��ȡ������������Ľ���, crosspointΪ����, distanceΪ����뽻�����</summary>
        public void getCrossGround(out VECTOR3D? crosspoint, out float? distance)
        {
            Vector3? vec3;
            getCrossGround(out vec3,out distance);
            if (vec3 == null)
                crosspoint = null;
            else
            {
                crosspoint = new VECTOR3D(((Vector3)vec3).X, ((Vector3)vec3).Y, ((Vector3)vec3).Z);
            }
        }


        /// <summary>
        /// ��������۲��
        /// </summary>
        /// <param name="angle">����洹ֱ�ߵļн�, ��Ч��Χ0-60</param>
        public void adjustCameraAngle(float angle)
        {
            if (angle < 0 || angle > 60) return;
            //��������潻��
            Vector3? crosspnt;
            float? distance;
            getCrossGround(out crosspnt, out distance);
            if (crosspnt != null)
            {
                Vector3 cp = (Vector3)crosspnt;
                float dis = (float)distance;

                if (angle == 0) //����Ϊ����
                {
                    Vector3 dir;
                    if (earth.earthManager.earthpara.SceneMode == ESceneMode.����)
                    {
                        cameraLookat = new Vector3(0, 0, 0);
                        dir = new Vector3(cp.X, cp.Y, cp.Z);
                    }
                    else
                    {
                        cameraLookat = new Vector3(cp.X, cp.Y, 0);
                        dir = new Vector3(0, 0, 1);
                    }
                    dir.Normalize();
                    cameraPosition = cp + dir * dis;

                    cameraDirection = cameraLookat - cameraPosition;
                    cameraDirection.Normalize();
                }
                else//����Ϊб��
                {
                    Vector3 dir;
                    Vector3 axis;
                    if (earth.earthManager.earthpara.SceneMode == ESceneMode.����)
                    {
                        dir = new Vector3(cp.X, cp.Y, cp.Z);
                        dir.Normalize();
                        axis = Vector3.Cross(dir, new Vector3(0, 1, 0));
                        axis.Normalize();
                    }
                    else
                    {
                        dir = new Vector3(0, 0, 1);
                        axis =new Vector3(-1,0,0);
                        axis.Normalize();

                    }
                    Matrix matrix = Matrix.CreateFromAxisAngle(axis, -MathHelper.Pi * angle / 180);
                    dir = Vector3.Transform(dir, matrix);
                    dir.Normalize();
                    cameraLookat = new Vector3(cp.X, cp.Y, cp.Z);
                    cameraPosition = cp + dir * dis;

                    cameraDirection = cameraLookat - cameraPosition;
                    cameraDirection.Normalize();
                }
                calCameraByDirection();

                if (earth.IsLoaded)
                {
                    updateD3DCamera(true, 500);
                    earth.global.isUpdate = true;
                }
            }

        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="Distance">�����Ŀ��ľ���</param>
        public void adjustCameraDistance(float Distance)
        {
            //if (Distance < 1 || Distance > 100) return;
            //��ǰ�۲��
            Vector3? crosspnt;
            float? distance;
            getCrossGround(out crosspnt, out distance);
            if (crosspnt != null)
            {
                float dis = (float)distance;
                float div = dis - Distance;
                // ԭд�� cameraPosition = cameraLookat + cameraDirection * div;
                cameraPosition = cameraPosition + cameraDirection * div;
                calCameraByDirection();
                updateD3DCamera(true, 500);
                earth.global.isUpdate = true;
            }


        }

        /// <summary>
        /// �Զ����������������ʾָ����Χ
        /// </summary>
        public void adjustCameraRange(System.Windows.Rect rect)
        {
            double minjd, maxjd, minwd, maxwd;
            minwd = rect.Left; maxwd = rect.Right; minjd = rect.Top; maxjd = rect.Bottom;
            List<VECTOR3D> corners = new List<VECTOR3D>();
            VECTOR3D lefttop, righttop, leftbottom, rightbottom, center;
            System.Windows.Media.Media3D.Vector3D lt, rt, lb, rb;
            lefttop = MapHelper.JWHToPoint(minjd, maxwd, 0,earth.earthManager.earthpara); corners.Add(lefttop); lt = Helpler.vecD3DToWpf(lefttop);
            righttop = MapHelper.JWHToPoint(maxjd, maxwd, 0, earth.earthManager.earthpara); corners.Add(righttop); rt = Helpler.vecD3DToWpf(righttop);
            leftbottom = MapHelper.JWHToPoint(minjd, minwd, 0, earth.earthManager.earthpara); corners.Add(leftbottom); lb = Helpler.vecD3DToWpf(leftbottom);
            rightbottom = MapHelper.JWHToPoint(maxjd, minwd, 0, earth.earthManager.earthpara); corners.Add(rightbottom); rb = Helpler.vecD3DToWpf(rightbottom);
            System.Windows.Media.Media3D.Vector3D cent = new System.Windows.Media.Media3D.Vector3D((corners.Max(p => p.x) + corners.Min(p => p.x)) / 2, (corners.Max(p => p.y) + corners.Min(p => p.y)) / 2, (corners.Max(p => p.z) + corners.Min(p => p.z)) / 2);
            if (earth.earthManager.earthpara.SceneMode == ESceneMode.����)
            {
                cent = cent * Para.Radius / cent.Length;
                center = new VECTOR3D(cent.X, cent.Y, cent.Z);
            }
            else  //zhע����δ��֤
            {
                center = new VECTOR3D(cent.X, cent.Y, 0);
            }
            double width, height;
            width = Math.Max((rt - lt).Length, (rb - lb).Length) * 1.5;
            height = Math.Max((lb - lt).Length, (rb - rt).Length) * 1.5;
            double distance;
            if (width / height > earth.global.ScreenWidth / earth.global.ScreenHeight) //�Կ��������
            {
                distance = width * earth.global.ScreenHeight / earth.global.ScreenWidth / 2 / Math.Atan(FieldOfView / 2);
            }
            else //�Ը߶�������
            {
                distance = height / 2 / Math.Atan(FieldOfView / 2);
            }

            aniLook(center);
            adjustCameraDistance((float)distance);

        }



        ///<summary>����D3D���, isAni�Ƿ��Զ�����ʽ���������duraion����ʱ������</summary>
        public void updateD3DCamera(bool isAni = false, int duration = 500)
        {
            STRUCT_Camera para = new STRUCT_Camera() { far = Far, near = Near, fieldofview = FieldOfView };
            para.lookat = new VECTOR3D(cameraLookat.X, cameraLookat.Y, cameraLookat.Z);
            para.pos = new VECTOR3D(cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
            para.up = new VECTOR3D(cameraUp.X, cameraUp.Y, cameraUp.Z);
            para.direction = new VECTOR3D(cameraDirection.X, cameraDirection.Y, cameraDirection.Z);

            IntPtr ipPara = Marshal.AllocCoTaskMem(Marshal.SizeOf(para));
            Marshal.StructureToPtr(para, ipPara, false);

            D3DManager.ChangeCameraPara(earth.earthkey, ipPara, isAni, duration);
            Marshal.FreeCoTaskMem(ipPara);
        }


        ///<summary>�����ƶ�����鿴ָ��λ��,���ڲ���ά����, timerFactor: �ٶ�ϵ��, Ϊ0���޶���ֱ��ˢ��</summary>
        public void aniLook(VECTOR3D vecLocation, double speedFactor = 1)
        {

            //���㵱ǰ�����н�
            System.Windows.Media.Media3D.Vector3D v1, v2, v3, v4, v5;

            Vector3 vec = new Vector3(vecLocation.x, vecLocation.y, vecLocation.z);
            if (earth.earthManager.earthpara.SceneMode == ESceneMode.����)
                vec = vec / vec.Length() * Para.Radius; //���㵽�ر�߶�

            //��ǰ�۲��
            Vector3? crosspnt;
            float? distance;
            float height;
            getCrossGround(out crosspnt, out distance);
            if (crosspnt != null)
            {
                float dis = (float)distance;
                Vector3 cp = (Vector3)crosspnt;

                if (earth.earthManager.earthpara.SceneMode == ESceneMode.����)
                {
                    v1 = new System.Windows.Media.Media3D.Vector3D(cp.X, cp.Y, cp.Z);
                    v2 = new System.Windows.Media.Media3D.Vector3D(cameraUp.X, cameraUp.Y, cameraUp.Z);
                    v3 = System.Windows.Media.Media3D.Vector3D.CrossProduct(v1, v2);
                    v4 = System.Windows.Media.Media3D.Vector3D.CrossProduct(v3, v1);
                    v5 = new System.Windows.Media.Media3D.Vector3D(cameraDirection.X, cameraDirection.Y, cameraDirection.Z);
                    float angle = (float)System.Windows.Media.Media3D.Vector3D.AngleBetween(v5, v4);
                    height = (new Vector3(cameraPosition.X, cameraPosition.Y, cameraPosition.Z)).Length() - Para.Radius;

                    Vector3 dir = vec;
                    dir.Normalize();
                    Vector3 axis = Vector3.Cross(dir, cameraUp);
                    axis.Normalize();
                    Matrix matrix = Matrix.CreateFromAxisAngle(axis, -MathHelper.Pi * (90.0f - angle) / 180.0f);
                    dir = Vector3.Transform(dir, matrix);
                    dir.Normalize();
                    cameraLookat = vec;
                    cameraPosition = cameraLookat + dir * dis;

                    cameraDirection = cameraLookat - cameraPosition;
                    cameraDirection.Normalize();
                }
                else
                {
                    Matrix matrix = Matrix.CreateTranslation(vecLocation.x - cp.X, vecLocation.y - cp.Y, 0);
                    cameraPosition = Vector3.Transform(cameraPosition, matrix);
                    height = cameraPosition.Z;
                }


                if (speedFactor == 0)
                {
                    calCameraByDirection();
                    updateD3DCamera();
                    earth.global.isUpdate = true;
                }
                else
                {
                    int time = (int)((cp - cameraLookat).Length() / height / speedFactor * 100); //ʱ������߶ȷ��ȣ����ٶ�ϵ������ 

                    calCameraByDirection();

                    updateD3DCamera(true, time);

                    tmr.Start();
                }
            }
        }


        ///<summary>�����ƶ�����鿴ָ��λ��, ��ԭʼ����, yjdΪԭʼ�����Yֵ, timerFactor: �ٶ�ϵ��, Ϊ0���޶���ֱ��ˢ��</summary>
        public void aniLook(double yjd, double xwd, double gd, double speedFactor = 1)
        {
            System.Windows.Point pnt = new System.Windows.Point(xwd, yjd);
            if (earth.coordinateManager.Enable)
            {
                pnt = earth.coordinateManager.transToInner(pnt);  //������������ת��
                yjd = pnt.Y;
                xwd = pnt.X;
            }
            Vector3 tmp = Helpler.JWHToPoint(yjd, xwd, gd, earth.earthManager.earthpara);
            aniLook(new VECTOR3D(tmp.X, tmp.Y, tmp.Z), speedFactor);
        }
        ///<summary>�����ƶ�����鿴ָ��λ��, ����γ����, timerFactor: �ٶ�ϵ��, Ϊ0���޶���ֱ��ˢ��</summary>
        public void aniLookGeo(double yjd, double xwd, double gd, double speedFactor = 1)
        {
            Vector3 tmp = Helpler.JWHToPoint(yjd, xwd, gd, earth.earthManager.earthpara);
            aniLook(new VECTOR3D(tmp.X, tmp.Y, tmp.Z), speedFactor);
        }

        ///<summary>��ʼ�������ָ��λ��</summary>
        public void initCamera(double jd, double wd, double gd)
        {
            if (earth.coordinateManager.Enable)
            {
                //System.Windows.Point pnt = earth.coordinateManager.transToInner(new System.Windows.Point(wd, jd)); //������������ת����ת��Ϊ�ڲ�����
                System.Windows.Point pnt = earth.coordinateManager.transToInner(new System.Windows.Point(jd, wd)); //������������ת����ת��Ϊ�ڲ�����
                jd = pnt.Y; wd = pnt.X;
            }

            cameraPosition = Helpler.JWHToPoint(jd, wd, gd,earth.earthManager.earthpara);
            if (earth.earthManager.earthpara.SceneMode== ESceneMode.����)
                cameraLookat = new Vector3(0, 0, 0);
            else
                cameraLookat = Helpler.JWHToPoint(jd, wd, 0, earth.earthManager.earthpara);

            cameraDirection = cameraLookat - cameraPosition;
            cameraDirection.Normalize();
            calCameraByDirection();
        }


        void tmr_Tick(object sender, EventArgs e)
        {
            earth.global.isUpdate = true;
            earth.UpdateModel();
            tmr.Stop();
        }
        System.Windows.Threading.DispatcherTimer tmr = new System.Windows.Threading.DispatcherTimer();


        ///<summary>��ȡ��Ļ�����ⲿ����</summary>
        public System.Windows.Point? getScreenCenter()
        {
            return getOuterCoordinate(1.0f * earth.global.ScreenWidth / 2, 1.0f * earth.global.ScreenHeight / 2);
        }
        ///<summary>��ȡ��Ļ���ľ�γ���꣬���أ�xγ��, y����</summary>
        public System.Windows.Point? getScreenCenterGeo()
        {
            return getGeoCoordinate(1.0f * earth.global.ScreenWidth / 2, 1.0f * earth.global.ScreenHeight / 2);
        }

        ///<summary>��ȡָ�����ⲿ����</summary>
        public System.Windows.Point? getOuterCoordinate(float x, float y)
        {
            System.Windows.Point pnt;
            System.Windows.Point? tmp = getGeoCoordinate(x, y);
            if (tmp == null) return null;
            pnt = (System.Windows.Point)tmp;
            if (earth.coordinateManager.Enable) pnt = earth.coordinateManager.transToOuter(pnt);  //������������ת��
            return pnt;
        }
        ///<summary>��ȡָ����Ļ�㾭γ����</summary>
        public System.Windows.Point? getGeoCoordinate(float x, float y)
        {
            Vector3? vectmp = Helpler.GetProjectPoint3D(new Vector2(x, y), this, earth.global.ScreenWidth, earth.global.ScreenHeight,0, earth.earthManager.earthpara);
            if (vectmp == null)
                return null;
            else
            {
                Vector3 vec = (Vector3)vectmp;
                System.Windows.Media.Media3D.Point3D jwh = Helpler.PointToJWH(new System.Windows.Media.Media3D.Point3D(vec.X, vec.Y, vec.Z), earth.earthManager.earthpara);
                System.Windows.Point pnt = new System.Windows.Point(jwh.Y, jwh.X);
                return pnt;
            }
        }


        #region ��γ��Χ���
        ///<summary>��ǰ������ӷ�Χ��γ�ȷ�Χ</summary>
        public ViewRange curCamearaViewRange
        {
            get
            {

                ViewRange viewbox = new ViewRange();
                //zhע���ݼٶ�ȫ���ཻ���棬����������Ч
                Vector3 vec;
                System.Windows.Media.Media3D.Point3D jwh;
                //�ײ�����, ��ȡγ����ʼ
                vec = (Vector3)Helpler.GetProjectPoint3D(new Vector2(1.0f * earth.global.ScreenWidth / 2, earth.global.ScreenHeight), this, earth.global.ScreenWidth, earth.global.ScreenHeight,0, earth.earthManager.earthpara);
                jwh = Helpler.PointToJWH(new System.Windows.Media.Media3D.Point3D(vec.X, vec.Y, vec.Z), earth.earthManager.earthpara);
                viewbox.latitudeStart = (float)jwh.Y;
                if (earth.coordinateManager.Enable) jwh = earth.coordinateManager.transToOuter(jwh);  //������������ת��
                viewbox.yStart = (float)jwh.Y;
                //������ˣ���ȡγ����ֹ, ��ȡԶ�˾�����ʼ
                vec = (Vector3)Helpler.GetProjectPoint3D(new Vector2(0, 0), this, earth.global.ScreenWidth, earth.global.ScreenHeight,0, earth.earthManager.earthpara);
                jwh = Helpler.PointToJWH(new System.Windows.Media.Media3D.Point3D(vec.X, vec.Y, vec.Z), earth.earthManager.earthpara);
                viewbox.latitudeEnd = (float)jwh.Y;

                if (earth.earthManager.TerrainAllow)//�¶�γ����չ1/10����Ӧ�������
                    viewbox.latitudeStart = viewbox.latitudeStart - (viewbox.latitudeEnd - viewbox.latitudeStart) / 10;  

                viewbox.farLongitudeStart = (float)jwh.X;
                if (earth.coordinateManager.Enable) jwh = earth.coordinateManager.transToOuter(jwh);  //������������ת��
                viewbox.yEnd = (float)jwh.Y;
                viewbox.farXStart = (float)jwh.X;
                //�����Ҷˣ���ȡԶ�˾�����ֹ
                vec = (Vector3)Helpler.GetProjectPoint3D(new Vector2(earth.global.ScreenWidth, 0), this, earth.global.ScreenWidth, earth.global.ScreenHeight,0, earth.earthManager.earthpara);
                jwh = Helpler.PointToJWH(new System.Windows.Media.Media3D.Point3D(vec.X, vec.Y, vec.Z), earth.earthManager.earthpara);
                viewbox.farLongitudeEnd = (float)jwh.X;
                if (earth.coordinateManager.Enable) jwh = earth.coordinateManager.transToOuter(jwh);  //������������ת��
                viewbox.farXEnd = (float)jwh.X;
                //�ײ���ˣ���ȡ���˾�����ʼ
                vec = (Vector3)Helpler.GetProjectPoint3D(new Vector2(0, earth.global.ScreenHeight), this, earth.global.ScreenWidth, earth.global.ScreenHeight,0, earth.earthManager.earthpara);
                jwh = Helpler.PointToJWH(new System.Windows.Media.Media3D.Point3D(vec.X, vec.Y, vec.Z), earth.earthManager.earthpara);
                viewbox.nearLongitudeStart = (float)jwh.X;
                if (earth.coordinateManager.Enable) jwh = earth.coordinateManager.transToOuter(jwh);  //������������ת��
                viewbox.nearXStart = (float)jwh.X;
                //�ײ��Ҷˣ���ȡ���˾�����ֹ
                vec = (Vector3)Helpler.GetProjectPoint3D(new Vector2(earth.global.ScreenWidth, earth.global.ScreenHeight), this, earth.global.ScreenWidth, earth.global.ScreenHeight,0, earth.earthManager.earthpara);
                jwh = Helpler.PointToJWH(new System.Windows.Media.Media3D.Point3D(vec.X, vec.Y, vec.Z), earth.earthManager.earthpara);
                viewbox.nearLongitudeEnd = (float)jwh.X;
                if (earth.coordinateManager.Enable) jwh = earth.coordinateManager.transToOuter(jwh);  //������������ת��
                viewbox.nearXEnd = (float)jwh.X;
                
                viewbox.calRanges(earth.coordinateManager.isXAsLong);
                viewbox.calRects(true);

                if (earth.coordinateManager.Enable)
                {
                    viewbox.calRangesOuter(earth.coordinateManager.isXAsLong);
                    viewbox.calRectsOuter(true);
                }
                else
                {
                    viewbox.rangesOuter = viewbox.ranges;
                    viewbox.rectsOuter = viewbox.rects;
                }

               //��x��Ӧ���ȣ�����xywh, range�����У���x��Ӧγ�Ƚ��е�


                return viewbox;
            }
        }


        ///<summary>��ǰ�����������</summary>
        public float curCameraDistanceToGround
        {
            get
            {
                Vector3? vec3;
                float? distance;
                getCrossGround(out vec3, out distance);

                return (float)distance;  //zhע���ٶ�һ���������ཻ
            }
        }
        ///<summary>��ǰ���������߼н�(�Ƕ�ֵ)</summary>
        public float curCameraAngle
        {
            get
            {
                if (earth.earthManager.earthpara.SceneMode == ESceneMode.����)
                {
                    System.Windows.Media.Media3D.Vector3D v1, v2;
                    v1 = new System.Windows.Media.Media3D.Vector3D(-cameraPosition.X, -cameraPosition.Y, -cameraPosition.Z);
                    v1.Normalize();
                    v2 = new System.Windows.Media.Media3D.Vector3D(cameraDirection.X, cameraDirection.Y, cameraDirection.Z);
                    return (float)System.Windows.Media.Media3D.Vector3D.AngleBetween(v1, v2);
                }
                else
                {
                    System.Windows.Media.Media3D.Vector3D v1, v2;
                    v1 = new System.Windows.Media.Media3D.Vector3D(0, 0, -1);
                    v2 = new System.Windows.Media.Media3D.Vector3D(cameraDirection.X, cameraDirection.Y, cameraDirection.Z);
                    return (float)System.Windows.Media.Media3D.Vector3D.AngleBetween(v1, v2);
                }
            }
        }


        #endregion

        public struct ViewRange
        {
            //  �ڲ�ʹ�þ�γ����
            ///<summary>���˾�����ʼ</summary>
            public float nearLongitudeStart { get; set; }
            ///<summary>���˾�����ֹ</summary>
            public float nearLongitudeEnd { get; set; }
            ///<summary>Զ�˾�����ʼ</summary>
            public float farLongitudeStart { get; set; }
            ///<summary>Զ�˾�����ֹ</summary>
            public float farLongitudeEnd { get; set; }
            ///<summary>γ�ȿ�ʼ</summary>
            public float latitudeStart { get; set; }
            ///<summary>γ����ֹ</summary>
            public float latitudeEnd { get; set; }

            //  �ⲿ����
            ///<summary>���˺�������ʼ</summary>
            public float nearXStart { get; set; }
            ///<summary>���˺�������ֹ</summary>
            public float nearXEnd { get; set; }
            ///<summary>Զ�˺�������ʼ</summary>
            public float farXStart { get; set; }
            ///<summary>Զ�˺�������ֹ</summary>
            public float farXEnd { get; set; }
            ///<summary>�����꿪ʼ</summary>
            public float yStart { get; set; }
            ///<summary>��������ֹ</summary>
            public float yEnd { get; set; }



            ///<summary>��չexpandscale��γ��Χ�б�y����xγ</summary>
            public List<System.Windows.Rect> ranges;

            ///<summary>��չexpandscale��γ��Χ�б��ⲿ����</summary>
            public List<System.Windows.Rect> rangesOuter;

            ///<summary>������չexpandscale��ķ�Χ</summary>
            internal void calRanges(bool isXAsLong)
            {
                if (nearLongitudeStart > nearLongitudeEnd) { float tmp = nearLongitudeEnd; nearLongitudeEnd = nearLongitudeStart; nearLongitudeStart = tmp; }
                if (farLongitudeStart > farLongitudeEnd) { float tmp = farLongitudeEnd; farLongitudeEnd = farLongitudeStart; farLongitudeStart = tmp; }
                if (latitudeStart > latitudeEnd) { float tmp = latitudeEnd; latitudeEnd = latitudeStart; latitudeStart = tmp; }


                float farLongLen = farLongitudeEnd - farLongitudeStart;
                float nearLongLen = nearLongitudeEnd - nearLongitudeStart;
                float latiLen = latitudeEnd - latitudeStart;
                float expandscale = 0.5f;

                float nearLongitudeStart2 = nearLongitudeStart - nearLongLen * expandscale;
                float nearLongitudeEnd2 = nearLongitudeEnd + nearLongLen * expandscale;
                float farLongitudeStart2 = farLongitudeStart - farLongLen * expandscale;
                float farLongitudeEnd2 = farLongitudeEnd + farLongLen * expandscale;
                float latitudeStart2 = latitudeStart - latiLen * expandscale;
                float latitudeEnd2 = latitudeEnd + latiLen * expandscale;


                ranges = new List<System.Windows.Rect>();
                int power = (int)((farLongitudeEnd - farLongitudeStart) / (nearLongitudeEnd - nearLongitudeStart));
                power = power < 1 ? 1 : power;
                power = power > 10 ? 10 : power;

                for (int i = 0; i < power; i++)
                {
                    System.Windows.Rect rect = new System.Windows.Rect();

                    if (isXAsLong)               //��x��Ӧ���ȣ�����xywh, range�����У���x��Ӧγ�Ƚ��е�
                    {
                        rect.X = farLongitudeStart2 + (power - i - 1) * (nearLongitudeStart2 - farLongitudeStart2) / power;
                        rect.Y = latitudeStart2 + i * (latitudeEnd2 - latitudeStart2) / power;
                        rect.Width = (nearLongitudeEnd2 - nearLongitudeStart2) + (i + 1) * ((farLongitudeEnd2 - farLongitudeStart2) - (nearLongitudeEnd2 - nearLongitudeStart2)) / power;
                        rect.Height = (latitudeEnd2 - latitudeStart2) / power;
                        ranges.Add(rect);
                    }
                    else
                    {
                        rect.Y = farLongitudeStart2 + (power - i - 1) * (nearLongitudeStart2 - farLongitudeStart2) / power;
                        rect.X = latitudeStart2 + i * (latitudeEnd2 - latitudeStart2) / power;
                        rect.Height = (nearLongitudeEnd2 - nearLongitudeStart2) + (i + 1) * ((farLongitudeEnd2 - farLongitudeStart2) - (nearLongitudeEnd2 - nearLongitudeStart2)) / power;
                        rect.Width = (latitudeEnd2 - latitudeStart2) / power;
                        ranges.Add(rect);
                    }
                }

            }
            ///<summary>������չexpandscale��ķ�Χ���ⲿ����</summary>
            internal void calRangesOuter(bool isXAsLong)
            {
                if (nearXStart > nearXEnd) { float tmp = nearXEnd; nearXEnd = nearXStart; nearXStart = tmp; }
                if (farXStart > farXEnd) { float tmp = farXEnd; farXEnd = farXStart; farXStart = tmp; }
                if (yStart > yEnd) { float tmp = yEnd; yEnd = yStart; yStart = tmp; }


                float farLongLen = farXEnd - farXStart;
                float nearLongLen = nearXEnd - nearXStart;
                float latiLen = yEnd - yStart;
                float expandscale = 0.5f;

                float nearXStart2 = nearXStart - nearLongLen * expandscale;
                float nearXEnd2 = nearXEnd + nearLongLen * expandscale;
                float farXStart2 = farXStart - farLongLen * expandscale;
                float farXEnd2 = farXEnd + farLongLen * expandscale;
                float yStart2 = yStart - latiLen * expandscale;
                float yEnd2 = yEnd + latiLen * expandscale;


                rangesOuter = new List<System.Windows.Rect>();
                int power = (int)((farXEnd - farXStart) / (nearXEnd - nearXStart));
                power = power < 1 ? 1 : power;
                power = power > 10 ? 10 : power;

                for (int i = 0; i < power; i++)
                {
                    System.Windows.Rect rect = new System.Windows.Rect();

                    if (isXAsLong)               //��x��Ӧ���ȣ�����xywh, range�����У���x��Ӧγ�Ƚ��е�
                    {
                        rect.X = farXStart2 + (power - i - 1) * (nearXStart2 - farXStart2) / power;
                        rect.Y = yStart2 + i * (yEnd2 - yStart2) / power;
                        rect.Width = (nearXEnd2 - nearXStart2) + (i + 1) * ((farXEnd2 - farXStart2) - (nearXEnd2 - nearXStart2)) / power;
                        rect.Height = (yEnd2 - yStart2) / power;
                        rangesOuter.Add(rect);
                    }
                    else
                    {
                        rect.Y = farXStart2 + (power - i - 1) * (nearXStart2 - farXStart2) / power;
                        rect.X = yStart2 + i * (yEnd2 - yStart2) / power;
                        rect.Height = (nearXEnd2 - nearXStart2) + (i + 1) * ((farXEnd2 - farXStart2) - (nearXEnd2 - nearXStart2)) / power;
                        rect.Width = (yEnd2 - yStart2) / power;
                        rangesOuter.Add(rect);
                    }
                }

                 //if (isXAsLong)               //��x��Ӧ���ȣ�����xywh, range�����У���x��Ӧγ�Ƚ��е�
                 //    rectOuter= new System.Windows.Rect(farXStart, yStart, farXEnd - farXStart, yEnd - yStart);
                 //else
                 //    rectOuter = new System.Windows.Rect(yStart, farXStart, yEnd - yStart, farXEnd - farXStart);

            }

            ///<summary>��Ļ��Χ���γ�γɵľ��Σ�x��yγ, �����ڵ�ͼ��Ƭ</summary>
            public System.Windows.Rect rect 
            {
                get
                {
                    return new System.Windows.Rect(farLongitudeStart,latitudeStart,farLongitudeEnd-farLongitudeStart,latitudeEnd-latitudeStart);
                }
            }

            /////<summary>��Ļ��Χ�����������γɵľ��Σ���������</summary>
            //public System.Windows.Rect rectOuter { get; set; }
            ////{
            ////    get
            ////    {
            ////        return new System.Windows.Rect(farXStart, yStart, farXEnd - farXStart, yEnd - yStart);
            ////    }
            ////}


            ///<summary>��Ļ��Χ��γ��Χ�б�������Ƭ��x����yγ</summary>
            public List<System.Windows.Rect> rects;
            ///<summary>��Ļ��Χ��γ��Χ�б��ⲿ����</summary>
            public List<System.Windows.Rect> rectsOuter;

            ///<summary>������Ļ��Χ��γ��Χ�б�</summary>
            internal void calRects(bool isXAsLong)
            {
                if (nearLongitudeStart > nearLongitudeEnd) { float tmp = nearLongitudeEnd; nearLongitudeEnd = nearLongitudeStart; nearLongitudeStart = tmp; }
                if (farLongitudeStart > farLongitudeEnd) { float tmp = farLongitudeEnd; farLongitudeEnd = farLongitudeStart; farLongitudeStart = tmp; }
                if (latitudeStart > latitudeEnd) { float tmp = latitudeEnd; latitudeEnd = latitudeStart; latitudeStart = tmp; }


                float farLongLen = farLongitudeEnd - farLongitudeStart;
                float nearLongLen = nearLongitudeEnd - nearLongitudeStart;
                float latiLen = latitudeEnd - latitudeStart;
                float expandscale = 0f;

                float nearLongitudeStart2 = nearLongitudeStart - nearLongLen * expandscale;
                float nearLongitudeEnd2 = nearLongitudeEnd + nearLongLen * expandscale;
                float farLongitudeStart2 = farLongitudeStart - farLongLen * expandscale;
                float farLongitudeEnd2 = farLongitudeEnd + farLongLen * expandscale;
                float latitudeStart2 = latitudeStart - latiLen * expandscale;
                float latitudeEnd2 = latitudeEnd + latiLen * expandscale;


                rects = new List<System.Windows.Rect>();
                int power = (int)((farLongitudeEnd - farLongitudeStart) / (nearLongitudeEnd - nearLongitudeStart));
                power = power < 1 ? 1 : power;
                power = power > 10 ? 10 : power;

                for (int i = 0; i < power; i++)
                {
                    System.Windows.Rect rect = new System.Windows.Rect();

                    if (isXAsLong)               //��x��Ӧ���ȣ�����xywh, range�����У���x��Ӧγ�Ƚ��е�
                    {
                        rect.X = farLongitudeStart2 + (power - i - 1) * (nearLongitudeStart2 - farLongitudeStart2) / power;
                        rect.Y = latitudeStart2 + i * (latitudeEnd2 - latitudeStart2) / power;
                        rect.Width = (nearLongitudeEnd2 - nearLongitudeStart2) + (i + 1) * ((farLongitudeEnd2 - farLongitudeStart2) - (nearLongitudeEnd2 - nearLongitudeStart2)) / power;
                        rect.Height = (latitudeEnd2 - latitudeStart2) / power;
                        rects.Add(rect);
                    }
                    else
                    {
                        rect.Y = farLongitudeStart2 + (power - i - 1) * (nearLongitudeStart2 - farLongitudeStart2) / power;
                        rect.X = latitudeStart2 + i * (latitudeEnd2 - latitudeStart2) / power;
                        rect.Height = (nearLongitudeEnd2 - nearLongitudeStart2) + (i + 1) * ((farLongitudeEnd2 - farLongitudeStart2) - (nearLongitudeEnd2 - nearLongitudeStart2)) / power;
                        rect.Width = (latitudeEnd2 - latitudeStart2) / power;
                        rects.Add(rect);
                    }
                }

            }

            ///<summary>������Ļ��Χ��γ��Χ�б��ⲿ����</summary>
            internal void calRectsOuter(bool isXAsLong)
            {
                if (nearXStart > nearXEnd) { float tmp = nearXEnd; nearXEnd = nearXStart; nearXStart = tmp; }
                if (farXStart > farXEnd) { float tmp = farXEnd; farXEnd = farXStart; farXStart = tmp; }
                if (yStart > yEnd) { float tmp = yEnd; yEnd = yStart; yStart = tmp; }


                float farLongLen = farXEnd - farXStart;
                float nearLongLen = nearXEnd - nearXStart;
                float latiLen = yEnd - yStart;
                float expandscale = 0f;

                float nearXStart2 = nearXStart - nearLongLen * expandscale;
                float nearXEnd2 = nearXEnd + nearLongLen * expandscale;
                float farXStart2 = farXStart - farLongLen * expandscale;
                float farXEnd2 = farXEnd + farLongLen * expandscale;
                float yStart2 = yStart - latiLen * expandscale;
                float yEnd2 = yEnd + latiLen * expandscale;


                rectsOuter = new List<System.Windows.Rect>();
                int power = (int)((farXEnd - farXStart) / (nearXEnd - nearXStart));
                power = power < 1 ? 1 : power;
                power = power > 10 ? 10 : power;

                for (int i = 0; i < power; i++)
                {
                    System.Windows.Rect rect = new System.Windows.Rect();

                    if (isXAsLong)               //��x��Ӧ���ȣ�����xywh, range�����У���x��Ӧγ�Ƚ��е�
                    {
                        rect.X = farXStart2 + (power - i - 1) * (nearXStart2 - farXStart2) / power;
                        rect.Y = yStart2 + i * (yEnd2 - yStart2) / power;
                        rect.Width = (nearXEnd2 - nearXStart2) + (i + 1) * ((farXEnd2 - farXStart2) - (nearXEnd2 - nearXStart2)) / power;
                        rect.Height = (yEnd2 - yStart2) / power;
                        rectsOuter.Add(rect);
                    }
                    else
                    {
                        rect.Y = farXStart2 + (power - i - 1) * (nearXStart2 - farXStart2) / power;
                        rect.X = yStart2 + i * (yEnd2 - yStart2) / power;
                        rect.Height = (nearXEnd2 - nearXStart2) + (i + 1) * ((farXEnd2 - farXStart2) - (nearXEnd2 - nearXStart2)) / power;
                        rect.Width = (yEnd2 - yStart2) / power;
                        rectsOuter.Add(rect);
                    }
                }

            }
        }



    }
}
