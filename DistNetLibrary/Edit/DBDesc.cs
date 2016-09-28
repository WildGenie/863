using System;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MyClassLibrary;
using WpfEarthLibrary;
using System.Xml.Serialization;

namespace DistNetLibrary.Edit
{
    public enum ETopoType { ��������, ��������, ��չ���� }

    [Serializable]
    public class DBDesc
    {
        ///<summary>�������ݿ�������xml�ļ�����ȱʡΪDBDesc.xml</summary>
        public string xmlFileName { get; set; }

        public DBDesc()
        {
            SQLS = new BindingList<SQL>();
            exTopo = new EXTOPO();
        }

        public static void SaveToXml(DBDesc dbdesc)
        {
            XmlHelper.saveToXml(dbdesc.xmlFileName, dbdesc);
        }

        public static DBDesc ReadFromXml(string xmlFileName)
        {
            DBDesc dbdesc = (DBDesc)XmlHelper.readFromXml(xmlFileName, typeof(DBDesc));
            if (dbdesc == null) return null; //dbdesc = new DBDesc();
            dbdesc.xmlFileName = xmlFileName;
            if (string.IsNullOrWhiteSpace(dbdesc.datasourceName))
                dbdesc.datasourceName = DataLayer.DataProvider.curDataSourceName;
            dbdesc.setParent();
            return dbdesc;
        }

        ///<summary>��������ʹ�õ�����Դ����</summary>
        public string datasourceName { get; set; }

        ///<summary>���û����õ����������ֵ�</summary>
        [XmlIgnore]
        public Dictionary<string, SQL> DictSQLS { get { return SQLS.ToDictionary(p => p.key); } }

        ///<summary>���ݿ������б��û���ʹ��DictSQLS�ֵ�</summary>
        public BindingList<SQL> SQLS { get; set; }


        ///<summary>ȫ�ֵ���չ������������</summary>
        public EXTOPO exTopo { get; set; }

        ///<summary>ȫ�ֵ������ֶ����ݽ���ģʽ</summary>
        public EParseTopoMode parseTopoMode { get; set; }

        ///<summary>ע��</summary>
        public string note { get; set; }

        ///<summary>�༭������ʹ��</summary>
        [XmlIgnore]
        public SQL selectedSQL { get; set; }

        internal void setParent()
        {
            foreach (var item in SQLS)
            {
                item.dbdesc = this;
            }
        }




    }

    [Serializable]
    public class EXTOPO : MyClassLibrary.MVVM.NotificationObject
    {
        public EXTOPO()
        {
            topoTableRelation = new TableRelation();
            topoexpanddesces = new BindingList<PropertyDesc>();
        }

        //--- ȫ����չ�������ݱ������Ĳ������˹�ϵ���ݱ�
        public TableRelation topoTableRelation { get; set; }
        public BindingList<PropertyDesc> topoexpanddesces { get; set; }

        private string _topoExSelect;
        public string topoExSelect
        {
            get { return _topoExSelect; }
            set { _topoExSelect = value; RaisePropertyChanged(() => topoExSelect); }
        }
        private string _topoExSelectAll;
        public string topoExSelectAll
        {
            get { return _topoExSelectAll; }
            set { _topoExSelectAll = value; RaisePropertyChanged(() => topoExSelectAll); }
        }
        private string _topoExInsert;
        public string topoExInsert
        {
            get { return _topoExInsert; }
            set { _topoExInsert = value; RaisePropertyChanged(() => topoExInsert); }
        }
        private string _topoExDelete;
        public string topoExDelete
        {
            get { return _topoExDelete; }
            set { _topoExDelete = value; RaisePropertyChanged(() => topoExDelete); }
        }

    }

    [Serializable]
    public class SQL : MyClassLibrary.MVVM.NotificationObject
    {
        public SQL()
        {
            keypdesces = new BindingList<PropertyDesc>();
            anctdesces = new BindingList<PropertyDesc>();
            rundatadesces = new BindingList<PropertyDesc>();
            planningdesces = new BindingList<PropertyDesc>();
            toposubordinatedesces = new BindingList<PropertyDesc>();
            toporelationdesces = new BindingList<PropertyDesc>();

            acntTableRelation = new TableRelation();
            rundataTableRelation = new TableRelation();
            planningTableRelation = new TableRelation();

            acntInsert = new BindingList<string>();
            acntUpdate = new BindingList<string>();
            acntDelete = new BindingList<string>();

            simRunDataFields = new BindingList<FieldDesc>();
            simPlanningFields = new BindingList<FieldDesc>();

            acntfreedesces = new BindingList<PropertyDesc>();
            //topoReInsert = new BindingList<string>();
            //topoReUpdate = new BindingList<string>();
            //topoReDelete = new BindingList<string>();
            //topoSuInsert = new BindingList<string>();
            //topoSuUpdate = new BindingList<string>();
            //topoSuDelete = new BindingList<string>();
        }

        internal DBDesc dbdesc;

        public string key { get; set; }


        public string DNObjTypeName { get; set; }
        public string DNObjTypeFullName { get; set; }

        #region ========== �������� ==========
        //===== ̨�����
        public TableRelation acntTableRelation { get; set; }

        ///<summary>�޹���������Select��䣬�����ڳ�ʼ������������, �û������������ڴ˻����ϸ���where����</summary>
        private string _acntSelect;
        public string acntSelect
        {
            get { return _acntSelect; }
            set { _acntSelect = value; RaisePropertyChanged(() => acntSelect); }
        }
        private string _acntSelectAll;
        public string acntSelectAll
        {
            get { return _acntSelectAll; }
            set { _acntSelectAll = value; RaisePropertyChanged(() => acntSelectAll); }
        }
        private string _acntSelectAllID;
        public string acntSelectAllID
        {
            get { return _acntSelectAllID; }
            set { _acntSelectAllID = value; RaisePropertyChanged(() => acntSelectAllID); }
        }
        private BindingList<string> _acntInsert;
        public BindingList<string> acntInsert
        {
            get { return _acntInsert; }
            set { _acntInsert = value; RaisePropertyChanged(() => acntInsert); }
        }
        private BindingList<string> _acntUpdate;
        public BindingList<string> acntUpdate
        {
            get { return _acntUpdate; }
            set { _acntUpdate = value; RaisePropertyChanged(() => acntUpdate); }
        }
        private BindingList<string> _acntDelete;
        public BindingList<string> acntDelete
        {
            get { return _acntDelete; }
            set { _acntDelete = value; RaisePropertyChanged(() => acntDelete); }
        }

        public string acntTypeName { get; set; }
        public string acntTypeFullName { get; set; }

        public BindingList<PropertyDesc> keypdesces { get; set; }  //�ؼ��ֶ�����

        //public SerializableDictionary<string, AcntPropertyDesc> dicAnct = new SerializableDictionary<string, AcntPropertyDesc>();
        public BindingList<PropertyDesc> anctdesces { get; set; }  //̨������

        public BindingList<PropertyDesc> acntfreedesces { get; set; }  //�����ֶ�����
        //===== �������
        public TableRelation rundataTableRelation { get; set; }
        public string rundataFilterFieldName { get; set; }

        ///<summary>�޹���������Select��䣬�����������������ʵʱ��������, �û������������ڴ˻����ϸ���where����</summary>
        public string rundataSelectAll { get; set; }

        private string _rundataSelect;
        public string rundataSelect
        {
            get { return _rundataSelect; }
            set { _rundataSelect = value; RaisePropertyChanged(() => rundataSelect); }
        }
        public string rundataTestSQL { get; set; }  //������sql, ����ȡ�õ�һ����¼��ID

        public string rundataTypeName { get; set; }
        public string rundataTypeFullName { get; set; }
        public BindingList<PropertyDesc> rundatadesces { get; set; }

        // ģ��
        private string _rundataSimAll;
        ///<summary>��������ģ�����</summary>
        public string rundataSimAll
        {
            get { return _rundataSimAll; }
            set { _rundataSimAll = value; RaisePropertyChanged(() => rundataSimAll); }
        }

        [XmlIgnore]
        internal DataTable simRunDataDataTable { get; set; }

        private BindingList<FieldDesc> _simRunDataFields;
        [XmlIgnore]
        public BindingList<FieldDesc> simRunDataFields
        {
            get { return _simRunDataFields; }
            set { _simRunDataFields = value; RaisePropertyChanged(() => simRunDataFields); }
        }


        //===== �滮���
        public TableRelation planningTableRelation { get; set; }
        public string planningFilterFieldName { get; set; }

        ///<summary>�޹���������Select��䣬�����������������滮ģ����������, �û������������ڴ˻����ϸ���where����</summary>
        private string _planningSelectAll;
        public string planningSelectAll
        {
            get { return _planningSelectAll; }
            set { _planningSelectAll = value; RaisePropertyChanged(() => planningSelectAll); }
        }


        private string _planningSelect;
        public string planningSelect
        {
            get { return _planningSelect; }
            set { _planningSelect = value; RaisePropertyChanged(() => planningSelect); }
        }
        public string planningTestSQL { get; set; }  //������sql, ����ȡ�õ�һ����¼��ID

        public BindingList<PropertyDesc> planningdesces { get; set; }
        // ģ��
        private string _planningSimAll;
        ///<summary>�滮��������ģ�����</summary>
        public string planningSimAll
        {
            get { return _planningSimAll; }
            set { _planningSimAll = value; RaisePropertyChanged(() => planningSimAll); }
        }

        [XmlIgnore]
        internal DataTable simPlanningDataTable { get; set; }

        private BindingList<FieldDesc> _simPlanningFields;
        [XmlIgnore]
        public BindingList<FieldDesc> simPlanningFields
        {
            get { return _simPlanningFields; }
            set { _simPlanningFields = value; RaisePropertyChanged(() => simPlanningFields); }
        }



        //===== �������, ע���������˲���̨�˹ؼ����ݣ�����չ���˵�������
        private PropertyDesc _topoBelontToFacility;
        public PropertyDesc topoBelontToFacility
        {
            get { return _topoBelontToFacility; }
            set { _topoBelontToFacility = value; RaisePropertyChanged(() => topoBelontToFacility); }
        }
        private PropertyDesc _topoBelongToEquipment;
        public PropertyDesc topoBelongToEquipment
        {
            get { return _topoBelongToEquipment; }
            set { _topoBelongToEquipment = value; RaisePropertyChanged(() => topoBelongToEquipment); }
        }


        ////--- �������˹�����������
        //public string topoReSelectAll { get; set; }

        //private string _topoReSelect;
        //public string topoReSelect
        //{
        //    get { return _topoReSelect; }
        //    set { _topoReSelect = value; RaisePropertyChanged(() => topoReSelect); }
        //}
        //private BindingList<string> _topoReInsert;
        //public BindingList<string> topoReInsert
        //{
        //    get { return _topoReInsert; }
        //    set { _topoReInsert = value; RaisePropertyChanged(() => topoReInsert); }
        //}

        //private BindingList<string> _topoReUpdate;
        //public BindingList<string> topoReUpdate
        //{
        //    get { return _topoReUpdate; }
        //    set { _topoReUpdate = value; RaisePropertyChanged(() => topoReUpdate); }
        //}
        //private BindingList<string> _topoReDelete;
        //public BindingList<string> topoReDelete
        //{
        //    get { return _topoReDelete; }
        //    set { _topoReDelete = value; RaisePropertyChanged(() => topoReDelete); }
        //}

        ////--- �������˴�����������
        //public string topoSuSelectAll { get; set; }

        //private string _topoSuSelect;
        //public string topoSuSelect
        //{
        //    get { return _topoSuSelect; }
        //    set { _topoSuSelect = value; RaisePropertyChanged(() => topoSuSelect); }
        //}
        //private BindingList<string> _topoSuInsert;
        //public BindingList<string> topoSuInsert
        //{
        //    get { return _topoSuInsert; }
        //    set { _topoSuInsert = value; RaisePropertyChanged(() => topoSuInsert); }
        //}

        //private BindingList<string> _topoSuUpdate;
        //public BindingList<string> topoSuUpdate
        //{
        //    get { return _topoSuUpdate; }
        //    set { _topoSuUpdate = value; RaisePropertyChanged(() => topoSuUpdate); }
        //}
        //private BindingList<string> _topoSuDelete;
        //public BindingList<string> topoSuDelete
        //{
        //    get { return _topoSuDelete; }
        //    set { _topoSuDelete = value; RaisePropertyChanged(() => topoSuDelete); }
        //}




        public BindingList<PropertyDesc> toporelationdesces { get; set; }
        public BindingList<PropertyDesc> toposubordinatedesces { get; set; }


        #endregion

        #region ========== �������� ==========

        #region ----- ������������ -----
        ///<summary>
        ///����dbopkeyȡ�õ����ݿ��������壬���������������뵽distnet�У�ͬʱ����̨�����ݺ��������ݡ�
        ///�Զ���Ĵ�����Ϊ����
        ///��ͬ��ͬID�����Ѵ��ڣ������
        ///addwhereΪ���ӵ�selectAll�ĸ��ӹ������
        ///layernameΪ��������ʱ��ָ�������ƣ���Ϊ�����Զ�����������Ϊ������
        ///���ش��������б���������������
        ///</summary>
        public List<PowerBasicObject> batchCreateDNObjects(DistNet distnet, string addwhere = "", string layername = null)
        {
            MyBaseControls.Screen.ScreenProgress.info = string.Format("����{0}...", this.key);

            DataLayer.DataProvider.curDataSourceName = dbdesc.datasourceName;

            List<PowerBasicObject> result = new List<PowerBasicObject>();

            PowerBasicObject obj;
            pLayer player;
            string s = acntSelectAll + " " + addwhere;
            DataTable dt = DataLayer.DataProvider.getDataTableFromSQL(s);
            if (dt.Rows.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(layername))
                {
                    pLayer tmplayer = new pLayer(null);
                    obj = createDNObject(tmplayer);
                    player = distnet.addLayer((obj.busiDesc as DescData).objCategory.ToString());
                }
                else
                    player = distnet.addLayer(layername);

                //������������
                foreach (DataRow dr in dt.Rows)
                {

                    //string zid = dr.getString(this.acntTableRelation.mainTable.keyFieldName);
                    string zid = dr.getString(this.keypdesces.FirstOrDefault(p => p.propertyname == "ID").fieldname);
                    if (string.IsNullOrWhiteSpace(zid))
                    {
                        MyBaseControls.StatusBarTool.StatusBarTool.reportInfo.showInfo("�����豸IDΪ�յĶ�����Щ����δ������������������Դ��", 30);
                        MyBaseControls.LogTool.Log.addLog(string.Format("{1}�����豸IDΪ�յĶ�����Щ����δ��������({0})", this, this.key), MyBaseControls.LogTool.ELogType.�澯);
                        continue;  //���豸idΪ�գ�����������
                    }

                    bool isExist = false;
                    foreach (pLayer lay in player.parent.zLayers.Values)
                    {
                        isExist = lay.pModels.ContainsKey(zid);
                        if (isExist)
                        {
                            MyBaseControls.StatusBarTool.StatusBarTool.reportInfo.showInfo("������ͬID�Ķ���ֻ������һ����������������Դ��", 30);
                            MyBaseControls.LogTool.Log.addLog(string.Format("{1}������ͬID�Ķ���ֻ������һ����������������Դ��({0})", this, this.key), MyBaseControls.LogTool.ELogType.�澯);
                            break;
                        }
                    }
                    if (!isExist)  //û��ͬID���󣬲Ŵ���
                    {
                        obj = createDNObject(player);
                        obj.DBOPKey = key;
                        loadKeyAcnt(dr, obj);
                        player.AddObject(obj);
                        result.Add(obj);
                    }
                }
                loadExTopo(result); //������չ����
            }
            return result;
        }

        #region ----- �ؼ����ԡ�̨�ˡ��������˵Ĳ��� -----
        ///<summary>
        ///�����ݿ�����ؼ����ԡ�̨�����ݺͻ������ˣ�����dr�ṩ���������롣
        ///�����ڳ�ʼ������������:
        ///1. ��acntSelectAll����ȡ��DataTable
        ///2. ��DataTable�е�DataRow(dr)��������(obj)�������������Ϣid, dbopkey
        ///3. ���ñ��������ؼ����ԡ�̨�˺ͻ�������
        ///</summary>
        public void loadKeyAcnt(DataRow dr, PowerBasicObject obj)
        {
            //=====����ؼ�����
            //obj.id = dr.getString(this.acntTableRelation.mainTable.keyFieldName);
            obj.id = dr.getString(this.keypdesces.FirstOrDefault(p => p.propertyname == "ID").fieldname);
            if (this.keypdesces.FirstOrDefault(p => p.propertyname == "ID2") != null)
                obj.id2 = dr.getString(this.keypdesces.FirstOrDefault(p => p.propertyname == "ID2").fieldname);

            double x, y; x = y = 0;
            string ps = "";
            foreach (PropertyDesc item in keypdesces)
            {
                if (item.propertyname == "name")
                    obj.name = dr.getString(item.fieldname);
                else if (item.propertyname == "X")
                {
                    x = dr.getDouble(item.fieldname);
                    ps = (new System.Windows.Point(x, y)).ToString();
                }
                else if (item.propertyname == "Y")
                {
                    y = dr.getDouble(item.fieldname);
                    ps = (new System.Windows.Point(x, y)).ToString();
                }
                else if (item.propertyname == "points")
                {
                    ps = dr.getString(item.fieldname);
                    if (ps.IndexOf(" ")<ps.IndexOf(",")) //ת��Ϊ��׼��ʽ
                    {
                        ps = ps.Replace(',', '_');
                        ps=ps.Replace(' ',',');
                        ps = ps.Replace('_', ' ');
                        ps=ps.Replace(";","");
                    }
                    else if (!ps.Contains(','))
                    {
                        ps = ps.Replace(' ', ',');
                    }
                }
                else if (item.propertyname == "shape")
                    ps = analShape(dr.getString(item.fieldname));

            }

            if (obj is pDotObject)
                (obj as pDotObject).location = ps;
            else if (obj is pPowerLine)
                (obj as pPowerLine).strPoints = ps;
            else if (obj is pArea)
                (obj as pPowerLine).strPoints = ps;
            //=====����̨��
            if (obj.busiAccount == null)
                obj.busiAccount = createAcnt();
            else
                if (obj.busiAccount.GetType().FullName != acntTypeFullName)
                {
                    System.Windows.MessageBox.Show("����̨��������������һ��, ǿ��ʹ���������͡�" + key);
                    obj.busiAccount = createAcnt();
                }

            foreach (DataColumn item in dr.Table.Columns)
            {
                PropertyDesc pd = anctdesces.FirstOrDefault(p => p.fieldname != null && p.fieldname.ToLower() == item.ColumnName.ToLower());
                if (pd != null)
                {
                    setPropertyValue(obj.busiAccount, dr, pd, DataLayer.EReadMode.���ݿ��ȡ);
                }
                //������Ϣ
                pd = acntfreedesces.FirstOrDefault(p => p.fieldname != null && p.fieldname.ToLower() == item.ColumnName.ToLower());
                if (pd != null)
                {
                    (obj.busiAccount as AcntDataBase).additionInfoes.Add(new AdditionInfo() { name = pd.propertycname, value = dr[pd.fieldname].ToString() });
                }
            }

            //�������
            if (toporelationdesces.Count > 0 || toposubordinatedesces.Count > 0 || topoBelontToFacility != null || topoBelongToEquipment != null)
            {
                if (obj.busiTopo == null) obj.busiTopo = new TopoData(obj);
                foreach (DataColumn item in dr.Table.Columns)
                {
                    PropertyDesc pd = toporelationdesces.FirstOrDefault(p => p.fieldname != null && p.fieldname.ToLower() == item.ColumnName.ToLower());
                    if (pd != null) //��ӻ�������
                    {
                        if (dbdesc.parseTopoMode == EParseTopoMode.��������Ժ��ʽ)
                        {
                            List<string> ss = parseTopoByBJJYY(dr[item].ToString());
                            foreach (string sid in ss)
                                (obj.busiTopo as TopoData).relationObjs.Add(new TopoObjDesc() { id = sid, table = pd.tablename, field = pd.fieldname });
                        }
                        else  //ֱ���ֶ�ֵΪ��������ID
                            (obj.busiTopo as TopoData).relationObjs.Add(new TopoObjDesc() { id = dr[item].ToString(), table = pd.tablename, field = pd.fieldname });
                    }
                    pd = toposubordinatedesces.FirstOrDefault(p => p.fieldname != null && p.fieldname.ToLower() == item.ColumnName.ToLower());
                    if (pd != null) //��ӻ�������
                    {
                        (obj.busiTopo as TopoData).subordinateObjs.Add(new TopoObjDesc() { id = dr[item].ToString(), table = pd.tablename, field = pd.fieldname });
                    }
                    if (topoBelontToFacility != null && topoBelontToFacility.fieldname.ToLower() == item.ColumnName.ToLower())  //��дֱ��������ʩ
                        (obj.busiTopo as TopoData).belontToFacilityID = new TopoObjDesc() { id = dr[item].ToString(), table = topoBelontToFacility.tablename, field = topoBelontToFacility.fieldname };
                    if (topoBelongToEquipment != null && topoBelongToEquipment.fieldname.ToLower() == item.ColumnName.ToLower())  //��дֱ�������豸����Ч�豸��
                        (obj.busiTopo as TopoData).belongToEquipmentID = new TopoObjDesc() { id = dr[item].ToString(), table = topoBelongToEquipment.tablename, field = topoBelongToEquipment.fieldname };
                }
            }
        }

        ///<summary>������������Ժ��CONNECTION���������ֶ�</summary>
        List<string> parseTopoByBJJYY(string s)
        {
            //2;23580694:<1397383_36000000:36000000:(513626.330492,287090.556251019):-1>;23454695:<1397367_36000000:36000000:(513625.610492,287090.556250981):-1>
            List<string> list = new List<string>();

            Regex rgx = new Regex(";(\\w*|\\d*|-):<.[^<>]*>", RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(s);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                    list.Add(match.Groups[1].Value);
            }
            return list;
        }


        ///<summary>����shape��ʽ������������</summary>
        string analShape(string shape)
        {
            //��2:2:0(539411.543202,287283.9925172764);(539411.543202,287283.99251);(539411.543202,287283.99250272365)
            //��ʾ: 
            //��":"�ָ�,  ��һλ2��ʾ����, 1-���豸, 2-���豸, 3-������豸
            //�ڶ�λ2��ʾά��, 2 - ��ά, 3-��ά
            //����λ ��ʾSRID, 
            //����λ,��ʾ�����, �����ű�ʾһ���������,����Ľṹ��ʾ�����������
            string s = shape.Substring(shape.IndexOf('('));
            s = s.Replace("(", "").Replace(")", "").Replace(";", " ");
            return s;
        }

        ///<summary>�����ݿ�����ؼ����Ժ�̨������, ʹ��ָ����ID��������id�޸ĺ�Ļָ���ǿ���������������̨��</summary>
        public void loadKeyAcnt(string id, PowerBasicObject obj)
        {
            DataLayer.DataProvider.curDataSourceName = dbdesc.datasourceName;

            string s = string.Format(acntSelect, id);
            DataTable dt = DataLayer.DataProvider.getDataTableFromSQL(s);
            if (dt.Rows.Count > 1) System.Windows.MessageBox.Show("��ȡ�����������ϼ�¼���������ݿ�������" + key);
            DataRow dr = dt.Rows[0];
            loadKeyAcnt(dr, obj);
        }
        ///<summary>�����ݿ�����ؼ����Ժ�̨�����ݣ�ʹ��obj�����id����̨��</summary>
        public void loadKeyAcnt(PowerBasicObject obj)
        {
            loadKeyAcnt(obj.id, obj);
        }

        ///<summary>����ؼ����Ժ�̨�����ݵ����ݿ�, idΪwhere��λʹ�õľ�ֵ����ֵͨ��obj.id��ȡ</summary>
        public void saveKeyAcnt(string id, PowerBasicObject obj)
        {
            DataLayer.DataProvider.curDataSourceName = dbdesc.datasourceName;

            string s = string.Format(acntSelect, id);
            DataTable dt = DataLayer.DataProvider.getDataTableFromSQL(s);
            if (dt.Rows.Count == 0)   //����
            {
                foreach (string ss in acntInsert)
                {
                    s = ss;
                    //����ؼ�����
                    foreach (var item in keypdesces)
                    {
                        if (item.propertyname == "ID")
                            s = s.Replace(string.Format("��{0}��", item.propertyname), obj.id);
                        else if (item.propertyname == "name")
                            s = s.Replace(string.Format("��{0}��", item.propertyname), obj.name);
                        else if (item.propertyname == "points")
                        {
                            if (obj is pPowerLine)
                                s = s.Replace(string.Format("��{0}��", item.propertyname), (obj as pPowerLine).strPoints);
                            else if (obj is pArea)
                                s = s.Replace(string.Format("��{0}��", item.propertyname), (obj as pArea).strPoints);
                        }
                        else if (item.propertyname == "X")
                            s = s.Replace(string.Format("��{0}��", item.propertyname), (obj as pDotObject).center.X.ToString());
                        else if (item.propertyname == "Y")
                            s = s.Replace(string.Format("��{0}��", item.propertyname), (obj as pDotObject).center.Y.ToString());
                    }
                    //����̨������
                    foreach (var item in anctdesces)
                    {
                        s = s.Replace(string.Format("��{0}��", item.propertyname), getPropertyValue(obj.busiAccount, item));
                    }
                    DataLayer.DataProvider.ExecuteSQL(s);
                }
            }
            else  //����
            {
                foreach (string ss in acntUpdate)
                {
                    s = ss;
                    //����ؼ�����
                    foreach (var item in keypdesces)
                    {
                        if (item.propertyname == "ID")
                            s = s.Replace(string.Format("��{0}��", item.propertyname), obj.id);
                        else if (item.propertyname == "name")
                            s = s.Replace(string.Format("��{0}��", item.propertyname), obj.name);
                        else if (item.propertyname == "points")
                        {
                            if (obj is pPowerLine)
                                s = s.Replace(string.Format("��{0}��", item.propertyname), (obj as pPowerLine).strPoints);
                            else if (obj is pArea)
                                s = s.Replace(string.Format("��{0}��", item.propertyname), (obj as pArea).strPoints);
                        }
                        else if (item.propertyname == "X")
                            s = s.Replace(string.Format("��{0}��", item.propertyname), (obj as pDotObject).center.X.ToString());
                        else if (item.propertyname == "Y")
                            s = s.Replace(string.Format("��{0}��", item.propertyname), (obj as pDotObject).center.Y.ToString());
                    }
                    //����̨������
                    foreach (var item in anctdesces)
                    {
                        s = s.Replace(string.Format("��{0}��", item.propertyname), getPropertyValue(obj.busiAccount, item));
                    }

                    s = string.Format(s, id); //where���֮id

                    DataLayer.DataProvider.ExecuteSQL(s);
                }
            }

        }

        ///<summary>�����ݿ���ɾ���ؼ����Ժ�̨�˼�¼</summary>
        public void delKeyAcnt(string id)
        {
            DataLayer.DataProvider.curDataSourceName = dbdesc.datasourceName;

            foreach (string item in acntDelete)
            {
                string s = string.Format(item, id);
                DataLayer.DataProvider.ExecuteSQL(s);
            }
        }
        #endregion




        #endregion

        #region ----- ʵʱ�͹滮ģ���������ݵĲ��� -----
        ///<summary>
        ///����dbopkeyȡ�õ����ݿ��������壬���������������ݡ�
        ///isRealRun������ture����ʵʱ�������ݣ�false����滮ģ����������
        ///�����������������ݵĶ����б�
        ///</summary>
        public List<PowerBasicObject> batchLoadRunData(DistNet distnet, bool isRealRun)
        {
            DataLayer.DataProvider.curDataSourceName = dbdesc.datasourceName;

            List<PowerBasicObject> result = new List<PowerBasicObject>();
            string sql = isRealRun ? this.rundataSelectAll : this.planningSelectAll;
            string sim = isRealRun ? this.rundataSimAll : this.planningSimAll;
            //��Ϊģ�����ݣ���Ԥ�����id
            KeyValuePair<DataLayer.EReadMode, DataTable> kvp;
            if (DataLayer.DataProvider.dataStatus == DataLayer.EDataStatus.ģ�� && (simRunDataDataTable == null || simPlanningDataTable == null))
            {
                DataTable dtids = DataLayer.DataProvider.getDataTableFromSQL(acntSelectAllID, false);
                if (dtids == null || dtids.Rows.Count == 0)
                    System.Windows.MessageBox.Show("ģ�����������Դ��̨��ҳ���acntSelectAllID��䣬����ѯΪ�յ���ģ��ʧ�ܣ����飡");
                List<string> ids = dtids.AsEnumerable().Select(p => p[0].ToString()).ToList();
                kvp = DataLayer.DataProvider.getDataTable(null, sim, ids, DataLayer.EReadMode.ģ��);
            }
            else
                kvp = DataLayer.DataProvider.getDataTable(sql, sim, isRealRun ? this.simRunDataDataTable : this.simPlanningDataTable);
            if (kvp.Key == DataLayer.EReadMode.ģ��)
            {
                if (isRealRun)
                    this.simRunDataDataTable = kvp.Value;
                else
                    this.simPlanningDataTable = kvp.Value;
            }
            foreach (DataRow dr in kvp.Value.Rows)
            {
                string id;
                if (kvp.Key == DataLayer.EReadMode.���ݿ��ȡ)
                    id = dr[(isRealRun ? rundataTableRelation : planningTableRelation).mainTable.keyFieldName].ToString();
                else
                    id = dr[0].ToString(); //ģ�����ݹ̶�Ϊ��һ�ֶ�ΪID�ֶ�
                PowerBasicObject obj = distnet.findObj(id);
                if (obj != null)
                {
                    loadRundata(dr, obj, kvp.Key, isRealRun);
                    result.Add(obj);
                }
            }
            return result;
        }

        ///<summary>
        ///������ʵʱ��������, �����������������롣
        ///1. ʹ��rundataSelectAll��ȡDataTable
        ///2. ���ó���Ӧ����dr�е�id�ֶΣ�rundataTableRelation.mainTable.keyFieldNameָ����id�ֶ��������Ҷ���obj
        ///3. ���ñ����������������
        ///</summary>
        void loadRundata(DataRow dr, PowerBasicObject obj, DataLayer.EReadMode readmode, bool isRealRun)
        {
            if (obj.busiRunData == null)
                obj.busiRunData = createRundata(obj);
            else
                if (obj.busiRunData.GetType().FullName != rundataTypeFullName)
                {
                    System.Windows.MessageBox.Show("������������������������һ��, ǿ��ʹ���������͡�" + key);
                    obj.busiRunData = createRundata(obj);
                }

            foreach (DataColumn item in dr.Table.Columns)
            {
                PropertyDesc pd;
                if (readmode == DataLayer.EReadMode.���ݿ��ȡ)
                    pd = (isRealRun ? rundatadesces : planningdesces).FirstOrDefault(p => p.fieldname != null && p.fieldname.ToLower() == item.ColumnName.ToLower());
                else
                    pd = (isRealRun ? rundatadesces : planningdesces).FirstOrDefault(p => p.simFieldName != null && p.simFieldName.ToLower() == item.ColumnName.ToLower());
                if (pd != null)
                {
                    setPropertyValue(obj.busiRunData, dr, pd, readmode);
                }
            }
        }

        ///<summary>�����ݿ������������ݣ�����������</summary>
        public void loadRundata(PowerBasicObject obj, bool isRealRun)
        {
            DataLayer.DataProvider.curDataSourceName = dbdesc.datasourceName;

            string s = string.Format(rundataSelect, obj.id);
            DataTable dt = DataLayer.DataProvider.getDataTableFromSQL(s);
            if (dt.Rows.Count > 1) System.Windows.MessageBox.Show("��ȡ�����������ϼ�¼���������ݿ�������" + key);
            DataRow dr = dt.Rows[0];
            loadRundata(dr, obj, DataLayer.EReadMode.���ݿ��ȡ, isRealRun);
        }




        #endregion

        #region ----- �������ݵĲ��� -----
        ///<summary>�Ը����Ķ����б�������չ����</summary>
        public void loadExTopo(List<PowerBasicObject> result)
        {
            DataLayer.DataProvider.curDataSourceName = dbdesc.datasourceName;

            //������չ����
            DataTable dtex;
            if (!string.IsNullOrWhiteSpace(dbdesc.exTopo.topoExSelectAll))
            {
                string sqlex = dbdesc.exTopo.topoExSelectAll + string.Format(" where {1} in ({0}) or {2} in ({0})", acntSelectAllID, dbdesc.exTopo.topoexpanddesces[0].fieldname, dbdesc.exTopo.topoexpanddesces[1].fieldname);
                dtex = DataLayer.DataProvider.getDataTableFromSQL(sqlex);
                foreach (DataRow dr in dtex.Rows)
                {
                    string id1 = dr[0].ToString();
                    string id2 = dr[1].ToString();
                    PowerBasicObject findobj = result.FirstOrDefault(p => p.id == id1);
                    if (findobj != null)
                        if ((findobj.busiTopo as TopoData).relationObjs.FirstOrDefault(p => p.id == id2) == null)
                            (findobj.busiTopo as TopoData).relationObjs.Add(new TopoObjDesc() { id = id2, isExpand = true });
                    findobj = result.FirstOrDefault(p => p.id == id2);
                    if (findobj != null)
                        if ((findobj.busiTopo as TopoData).relationObjs.FirstOrDefault(p => p.id == id1) == null)
                            (findobj.busiTopo as TopoData).relationObjs.Add(new TopoObjDesc() { id = id1, isExpand = true });
                }

            }
        }


        #endregion






        //---------------------------------------------------------------------------------------------------
        ///<summary>������������</summary>
        PowerBasicObject createDNObject(pLayer player)
        {
            Object[] parameters = new Object[1]; // ���幹�캯����Ҫ�Ĳ��������в���������ΪObject
            parameters[0] = player;
            return (PowerBasicObject)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(DNObjTypeFullName, false,
               System.Reflection.BindingFlags.Default, null, parameters, null, null);

        }

        ///<summary>����̨�˶���</summary>
        AcntDataBase createAcnt()
        {
            return (AcntDataBase)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(acntTypeFullName);

        }
        ///<summary>�������ж���</summary>
        RunDataBase createRundata(PowerBasicObject parent)
        {
            Object[] parameters = new Object[1]; // ���幹�캯����Ҫ�Ĳ��������в���������ΪObject
            parameters[0] = parent;
            return (RunDataBase)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(rundataTypeFullName, false,
               System.Reflection.BindingFlags.Default, null, parameters, null, null);
        }
        ///<summary>�������˶���</summary>
        TopoData createTopo(PowerBasicObject parent)
        {
            return new TopoData(parent);
        }

        ///<summary>��������ֵ</summary>
        void setPropertyValue(object obj, DataRow dr, PropertyDesc pd, DataLayer.EReadMode readmode)
        {
            //������������ֵ
            PropertyInfo pi = obj.GetType().GetProperty(pd.propertyname);
            string fieldtypename = readmode == DataLayer.EReadMode.ģ�� ? pd.simtypename : pd.fieldtypename;
            string fieldname = readmode == DataLayer.EReadMode.ģ�� ? pd.simFieldName : pd.fieldname;
            switch (pd.propertyTypeName)
            {
                case "Float":
                    pi.SetValue(obj, getValueFromDB<float>(dr, fieldtypename, fieldname), null);
                    break;
                case "Double":
                    pi.SetValue(obj, getValueFromDB<double>(dr, fieldtypename, fieldname), null);
                    break;
                case "Int32":
                    pi.SetValue(obj, getValueFromDB<int>(dr, fieldtypename, fieldname), null);
                    break;
                case "String":
                    pi.SetValue(obj, getValueFromDB<string>(dr, fieldtypename, fieldname), null);
                    break;
                case "Boolean":
                    pi.SetValue(obj, getValueFromDB<bool>(dr, fieldtypename, fieldname), null);
                    break;
                case "DateTime":
                    pi.SetValue(obj, getValueFromDB<DateTime>(dr, fieldtypename, fieldname), null);
                    break;
                default:  //zhע��ö�ٵĴ�����Ҫ���
                    pi.SetValue(obj, getValueFromDB<int>(dr, fieldtypename, fieldname), null);
                    break;
            }


        }

        ///<summary>��ȡ����ֵ</summary>
        string getPropertyValue(object obj, PropertyDesc pd)
        {
            //������������ֵ
            PropertyInfo pi = obj.GetType().GetProperty(pd.propertyname);
            object value = pi.GetValue(obj, null);
            if (value == null)
                return "";
            else if (value is Boolean)
                return (Boolean)value ? "1" : "0";
            else if (value is Enum)  //ö������
            {
                return Convert.ToInt32(value).ToString();
            }
            return value.ToString();
        }

        ///<summary>�����ݿ��ȡֵ</summary>
        T getValueFromDB<T>(DataRow dr, string fieldtypename, string fieldname)
        {
            switch (fieldtypename)
            {
                case "System.Double":
                    return (T)(object)dr.getDouble(fieldname);
                case "System.String":
                    return (T)(object)dr.getString(fieldname);
                case "System.DateTime":
                    return (T)(object)dr.getDatetime(fieldname);
                case "System.Int32":
                    return (T)(object)dr.getInt(fieldname);

                case "float":
                    return (T)(object)dr.getDouble(fieldname);
                case "real":
                    if (typeof(T) == typeof(int))
                        return (T)(object)(int)dr.getDouble(fieldname);
                    else
                        return (T)(object)dr.getDouble(fieldname);
                case "decimal":
                    if (typeof(T) == typeof(string))
                        return (T)(object)dr.getDouble(fieldname).ToString();
                    else
                        return (T)(object)dr.getDouble(fieldname);
                case "nvarchar":
                    return (T)(object)dr.getString(fieldname);
                case "varchar":
                    return (T)(object)dr.getString(fieldname).TrimEnd();
                case "datetime":
                    return (T)(object)dr.getDatetime(fieldname);
                case "datetime2":
                    return (T)(object)dr.getDatetime(fieldname);
                case "smallint":
                    if (typeof(T) == typeof(Boolean))
                        return (T)(object)(dr.getIntN1(fieldname) != 0);
                    else
                        return (T)(object)dr.getIntN1(fieldname);
                case "int":
                    if (typeof(T) == typeof(Boolean))
                        return (T)(object)(dr.getIntN1(fieldname) != 0);
                    else
                        //return (T)(object)dr.getIntN1(pd.fieldname);
                        return (T)Convert.ChangeType(dr.getIntN1(fieldname), typeof(T));
                case "bit":  //bit��Ϊ��bool��
                    return (T)(object)(dr.getInt(fieldname) == 1);

                default:
                    return default(T);
            }


            
        }



        #endregion

    }

    [Serializable]
    public class PropertyDesc : MyClassLibrary.MVVM.NotificationObject
    {

        public string propertyname { get; set; }
        public string propertycname { get; set; }
        public string propertyTypeName { get; set; }
        public string propertyTypeFullName { get; set; }



        private string _tablename;
        public string tablename
        {
            get { return _tablename; }
            set { _tablename = value; RaisePropertyChanged(() => tablename); }
        }
        private string _fieldname;
        public string fieldname
        {
            get { return _fieldname; }
            set { _fieldname = value; RaisePropertyChanged(() => fieldname); }
        }
        private string _fieldcname;
        public string fieldcname
        {
            get { return _fieldcname; }
            set { _fieldcname = value; RaisePropertyChanged(() => fieldcname); }
        }

        private string _fieldtypename;
        public string fieldtypename
        {
            get { return _fieldtypename; }
            set { _fieldtypename = value; RaisePropertyChanged(() => fieldtypename); }
        }

        private string _simtypename;
        public string simtypename
        {
            get { return _simtypename; }
            set { _simtypename = value; RaisePropertyChanged(() => simtypename); }
        }

        private string _simFieldName;
        public string simFieldName
        {
            get { return _simFieldName; }
            set { _simFieldName = value; RaisePropertyChanged(() => simFieldName); }
        }



        //[XmlIgnore]
        //public string properyinfo { get { return string.Format("{0}({1})[2]", propertycname, propertyname, propertyTypeName); } }

    }


    ///<summary>�������õı����ϵ</summary>
    public class TableRelation : MyClassLibrary.MVVM.NotificationObject
    {
        public TableRelation()
        {
            tables = new BindingList<TableDesc>();
            tableKeyPairs = new BindingList<TableKeyPair>();
        }

        private TableDesc _mainTable;
        public TableDesc mainTable
        {
            get { return _mainTable; }
            set { _mainTable = value; RaisePropertyChanged(() => mainTable); }
        }

        public BindingList<TableDesc> tables { get; set; }

        public BindingList<TableKeyPair> tableKeyPairs { get; set; }
    }

    public class TableDesc : MyClassLibrary.MVVM.NotificationObject
    {
        public TableDesc()
        {
            fields = new BindingList<FieldDesc>();
        }

        private bool _isMainTable;
        ///<summary>�Ƿ�������</summary>
        public bool isMainTable
        {
            get { return _isMainTable; }
            set { _isMainTable = value; RaisePropertyChanged(() => isMainTable); }
        }


        private string _tableName;
        public string tableName
        {
            get { return _tableName; }
            set { _tableName = value; RaisePropertyChanged(() => tableName); }
        }
        public string tableCName { get; set; }


        private string _keyFieldName;
        public string keyFieldName
        {
            get { return _keyFieldName; }
            set { _keyFieldName = value; RaisePropertyChanged(() => keyFieldName); }
        }


        private string _filter;
        public string filter
        {
            get { return _filter; }
            set { _filter = value; RaisePropertyChanged(() => filter); }
        }



        public BindingList<FieldDesc> fields { get; set; }

        ///<summary>��¡����ʵ���������е�fields��Ϊ����</summary>
        public TableDesc Clone()
        {
            TableDesc newinstance = new TableDesc()
            {
                tableName = this.tableName,
                isMainTable = this.isMainTable,
                filter = this.filter,
                keyFieldName = this.keyFieldName,
                tableCName = this.tableCName,
                fields = this.fields
            };
            return newinstance;
        }
    }
    public class FieldDesc
    {
        public string fieldName { get; set; }
        public string fieldCName { get; set; }
        public string fieldTypeName { get; set; }
    }


    public class TableKeyPair
    {
        public string mainTableName { get; set; }
        public string mainKeyFieldName { get; set; }
        public string subTableName { get; set; }
        public string subKeyFieldName { get; set; }
    }
}
