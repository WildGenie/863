using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistNetLibrary.Edit
{
        public enum EParseTopoMode { �޽�����Ҫ, ��������Ժ��ʽ }

    class WinDBDescToolViewModel
    {
        public WinDBDescToolViewModel(string xmlFileName)
        {
            dbdesc = DBDesc.ReadFromXml(xmlFileName);
            if (dbdesc!=null)  //��ȡ�����ݿ���������������Դ
            {
                if (string.IsNullOrWhiteSpace(dbdesc.datasourceName))
                    dbdesc.datasourceName = DataLayer.DataProvider.curDataSourceName;
                else
                    DataLayer.DataProvider.curDataSourceName = dbdesc.datasourceName;
            }

            tables = new List<TableDesc>();
            //��ȡ���ݿ���Ϣ
            string sql="";
            if (DataLayer.DataProvider.databaseType == DataLayer.EDataBaseType.MsSql)
                sql = @"
select t1.name,t2.value cname from 
(select * from sysobjects t1 where xtype='U') t1 left join
(select * from sys.extended_properties   where minor_id=0 and name='MS_Description') t2 on t1.id=t2.major_id order by name
";
            else if (DataLayer.DataProvider.databaseType== DataLayer.EDataBaseType.MySql)
                sql = "SELECT TABLE_NAME as name,Table_COMMENT as cname FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = (select database() limit 1) and table_type='base table'";

            DataTable dt = DataLayer.DataProvider.getDataTableFromSQL(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string tname = dr.Field<string>("name");
                string cname=dr.Field<string>("cname");
                TableDesc tabledesc = new TableDesc() { tableName = tname, tableCName=cname};
                tables.Add(tabledesc);
                //SELECT * FROM sys.extended_properties WHERE major_id = OBJECT_ID ('mem_server' )
                if (DataLayer.DataProvider.databaseType == DataLayer.EDataBaseType.MsSql)
                    sql = @"
SELECT syscolumns.name,systypes.name ftype,t3.value cname 
FROM syscolumns join systypes on syscolumns.xusertype = systypes.xusertype 
	left join sys.extended_properties t3 on syscolumns.id=t3.major_id and syscolumns.colorder=t3.minor_id
WHERE syscolumns.id = object_id('{0}') order by name
";
                else if (DataLayer.DataProvider.databaseType == DataLayer.EDataBaseType.MySql)
                    sql = @"select column_name as name,data_type as ftype,column_comment as cname from information_schema.columns where table_schema=(select database() limit 1) and table_name='{0}'";

                DataTable dt2 = DataLayer.DataProvider.getDataTableFromSQL(String.Format(sql, tname));
                foreach (DataRow dr2 in dt2.Rows)
                {
                    FieldDesc fd = new FieldDesc() { fieldName = dr2.Field<string>("name"), fieldCName = dr2.Field<string>("cname"), fieldTypeName = dr2.Field<string>("ftype") };
                    if (fd.fieldCName == null) fd.fieldCName = fd.fieldName;
                    tabledesc.fields.Add(fd);
                }
            }

            //

            //��ȡ��������
            objtypedesces = new List<TypeDesc>();
            objtypedesces.Add(new TypeDesc("���վ", typeof(DNSubStation), typeof(AcntSubstation), typeof(RunDataSubstation)));
            objtypedesces.Add(new TypeDesc("���վ����", typeof(DNSubstationOutline), typeof(AcntSubstationOutline), typeof(RunDataSubstationOutline)));
            objtypedesces.Add(new TypeDesc("����վ", typeof(DNSwitchStation), typeof(AcntSwitchStation), typeof(RunDataSwitchStation)));
            objtypedesces.Add(new TypeDesc("�����", typeof(DNSwitchHouse), typeof(AcntSwitchHouse), typeof(RunDataSwitchHouse)));
            objtypedesces.Add(new TypeDesc("����ѹ��", typeof(DNMainTransformer), typeof(AcntMainTransformer), typeof(RunDataMainTransformer)));
            objtypedesces.Add(new TypeDesc("��������", typeof(DNMainTransformer2W), typeof(AcntMainTransformer2W), typeof(RunDataMainTransformer2W)));
            objtypedesces.Add(new TypeDesc("��������", typeof(DNMainTransformer3W), typeof(AcntMainTransformer3W), typeof(RunDataMainTransformer3W)));
            objtypedesces.Add(new TypeDesc("���", typeof(DNDistTransformer), typeof(AcntDistTransformer), typeof(RunDataDistTransformer)));
            objtypedesces.Add(new TypeDesc("���ϱ�", typeof(DNColumnTransformer), typeof(AcntColumnTransformer), typeof(RunDataColumnTransformer)));
            objtypedesces.Add(new TypeDesc("�û���", typeof(DNCustomerTransformer), typeof(AcntCustomerTransformer), typeof(RunDataCustomerTransformer)));
            objtypedesces.Add(new TypeDesc("�ڵ�", typeof(DNNode), typeof(AcntNode), typeof(RunDataNode)));
            objtypedesces.Add(new TypeDesc("���뿪��", typeof(DNSwitch), typeof(AcntSwitch), typeof(RunDataSwitch)));
            objtypedesces.Add(new TypeDesc("���ɿ���", typeof(DNLoadSwitch), typeof(AcntLoadSwitch), typeof(RunDataLoadSwitch)));
            objtypedesces.Add(new TypeDesc("��·��", typeof(DNBreaker), typeof(AcntBreaker), typeof(RunDataBreaker)));
            objtypedesces.Add(new TypeDesc("�۶���", typeof(DNFuse), typeof(AcntFuse), typeof(RunDataFuse)));
            objtypedesces.Add(new TypeDesc("�����·", typeof(DNACLine), typeof(AcntACLine), typeof(RunDataACLine)));
            objtypedesces.Add(new TypeDesc("���߶�", typeof(DNLineSeg), typeof(AcntLineSeg), typeof(RunDataLineSeg)));
            objtypedesces.Add(new TypeDesc("���¶�", typeof(DNCableSeg), typeof(AcntCableSeg), typeof(RunDataCableSeg)));
            objtypedesces.Add(new TypeDesc("ĸ��", typeof(DNBusBar), typeof(AcntBusBar), typeof(RunDataBusBar)));
            objtypedesces.Add(new TypeDesc("ĸ������", typeof(DNBusBarSwitch), typeof(AcntBusBarSwitch), typeof(RunDataBusBarSwitch)));
            objtypedesces.Add(new TypeDesc("������", typeof(DNConnectivityLine), typeof(AcntConnectivityLine), typeof(RunDataConnectivityLine)));
            objtypedesces.Add(new TypeDesc("���ӵ�", typeof(DNConnectivityNode), typeof(AcntConnectivityNode), typeof(RunDataConnectivityNode)));
            objtypedesces.Add(new TypeDesc("�������", typeof(DNPVPlant), typeof(AcntPVPlant), typeof(RunDataPVPlant)));
            objtypedesces.Add(new TypeDesc("��������", typeof(DNWindPlant), typeof(AcntWindPlant), typeof(RunDataWindPlant)));
            objtypedesces.Add(new TypeDesc("��������", typeof(DNGridArea), typeof(AcntGridArea), typeof(RunDataGridArea)));
            objtypedesces.Add(new TypeDesc("�ֽ���", typeof(DNDividingRoom), typeof(AcntDividing), typeof(RunDataDividing)));
            objtypedesces.Add(new TypeDesc("�ֽ���", typeof(DNDividingBox), typeof(AcntDividing), typeof(RunDataDividing)));
            objtypedesces.Add(new TypeDesc("ֱ�߸���", typeof(DNIntermediateSupport), typeof(AcntIntermediateSupport), typeof(RunDataIntermediateSupport)));
            objtypedesces.Add(new TypeDesc("���Ÿ���", typeof(DNStrainSupport), typeof(AcntStrainSupport), typeof(RunDataStrainSupport)));


      

            //==========�������ݲ�������

            if (dbdesc == null)
            {
                dbdesc = new DBDesc();
                dbdesc.xmlFileName = xmlFileName;
            }
            else
            {
                //�������ݿⶨ��
                TableDesc oldtd, newtd;
                foreach (var itemsql in dbdesc.SQLS)
                {
                    //��������
                    if (itemsql.acntTableRelation.mainTable != null)
                    {
                        oldtd = itemsql.acntTableRelation.mainTable;
                        newtd = tables.FirstOrDefault(p => p.tableName == itemsql.acntTableRelation.mainTable.tableName).Clone();
                        if (newtd != null)
                        {
                            newtd.filter = oldtd.filter;
                            newtd.isMainTable = oldtd.isMainTable;
                            newtd.keyFieldName = oldtd.keyFieldName;
                        }
                        itemsql.acntTableRelation.mainTable = newtd;
                    }
                    //���±�
                    BindingList<TableDesc> newlist = new BindingList<TableDesc>();
                    foreach (var itemtable in itemsql.acntTableRelation.tables)
                    {
                        if (itemtable.isMainTable && itemtable.tableName == itemsql.acntTableRelation.mainTable.tableName)
                        {
                            newlist.Add(itemsql.acntTableRelation.mainTable);
                        }
                        else
                        {
                            oldtd = itemtable;
                            newtd = tables.FirstOrDefault(p => p.tableName == itemtable.tableName).Clone();
                            if (newtd != null)
                            {
                                newtd.filter = oldtd.filter;
                                newtd.isMainTable = false;// oldtd.isMainTable;
                                newtd.keyFieldName = oldtd.keyFieldName;
                                newlist.Add(newtd);
                            }
                        }
                    }
                    itemsql.acntTableRelation.tables = newlist;
                }
            }

        }

        //���ݿ�����������
        public DBDesc dbdesc { get; set; }
        

        ///<summary>�������ݱ��б�</summary>
        public List<TableDesc> tables { get; set; }



        ///<summary>�����������������б�</summary>
        public List<TypeDesc> objtypedesces { get; set; }
        //List<Type> objtypes = new List<Type>();



        #region ===== ȫ������ =====
        public List<KeyValuePair<int, string>> topoparsemodes
        {
            get
            {
                List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();
                for (int i = 0; i < Enum.GetNames(typeof(EParseTopoMode)).Count(); i++)
                {
                    result.Add(new KeyValuePair<int, string>(i, ((EParseTopoMode)i).ToString()));
                }
                return result;
            }
        }

        #endregion



        /////<summary>����̨�������б�</summary>
        //public List<TypeDesc> acnttypedesces { get; set; }
        //List<Type> acnttypes = new List<Type>();


        /////<summary>�������������б�</summary>
        //public List<TypeDesc> rundatatypedesces { get; set; }
        //List<Type> rundatatypes = new List<Type>();
    }


    public class TypeDesc
    {
        public TypeDesc(string objName,Type objType, Type acntType,Type rundataType)
        {
            objname = string.Format("{0}({1})", objName , objType.Name);

            objtype = objType;
            objtypename = objType.Name;
            objtypefullname = objType.FullName;

            acnttype = acntType;
            acnttypename = acntType.Name;
            acnttypefullname = acntType.FullName;

            rundatatype = rundataType;
            rundatatypename = rundataType.Name;
            rundatatypefullname = rundataType.FullName;

        }
        public string objname { get; set; }

        public Type objtype;
        public string objtypename {get;set;}
        public string objtypefullname { get; set; }

        public Type acnttype;
        public string acnttypename { get; set; }
        public string acnttypefullname { get; set; }

        public Type rundatatype;
        public string rundatatypename { get; set; }
        public string rundatatypefullname { get; set; }


        //object create()
        //{
        //    return System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(type.FullName);
        //}

    }

}
