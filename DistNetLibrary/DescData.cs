using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfEarthLibrary;

namespace DistNetLibrary
{
    public class DescData
    {
        internal EObjectType _objType;
        ///<summary>��������</summary>
        public EObjectType objType { get { return _objType; } }

        internal EObjectScope _objScope;
        ///<summary>���󷶳�</summary>
        public EObjectScope objScope { get { return _objScope; } }

        internal EObjectCategory _objCategory;
        ///<summary>��������</summary>
        public EObjectCategory objCategory { get { return _objCategory; } }

        internal bool _isFacility;
        ///<summary>�Ƿ�����ʩ</summary>
        public bool isFacility { get { return _isFacility; } }

        internal bool _isEquipment;
        ///<summary>�Ƿ����豸</summary>
        public bool isEquipment { get { return _isEquipment; } }

        internal System.Windows.Media.Brush _icon=(System.Windows.Media.Brush)System.Windows.Application.Current.FindResource("ͨ�ö���");
        ///<summary>ͼ��brush</summary>
        public System.Windows.Media.Brush icon { get { return _icon; } }

            

    }
}
