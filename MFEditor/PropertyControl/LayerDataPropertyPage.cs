// Copyright 2008, 2010 - http://code.google.com/p/mfeditor/
//
// Author:   WanliYun 
// Email:    wanliyun2009@gmail.com
// QQ Group: 81979380
// Blog:     http://blog.csdn.net/wanliyun2009
//
//
// This file is part of MFEditor.
// MFEditor is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// MFEditor is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with MFEditor; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.MapServer;
using System.IO;
using MFEditor.Utils;


namespace MFEditor.PropertyControl
{
    public partial class LayerDataPropertyPage : UserControl, IPropertyPage
    {

        private string m_strShapePath = "";

        public LayerDataPropertyPage()
        {
            InitializeComponent();
        }


        #region "Implement IPropertyPage"

        private layerObj m_pLayer = null;

        //if there is a change
        public bool _IsPageDirty = false;
        /// <summary>
        /// active the current page
        /// </summary>
        public void Activate()
        {
            if (m_pLayer == null) return;
            Initialize();
        }

        /// <summary>
        /// apply the current setting
        /// </summary>
        public void Apply()
        {
            if (m_pLayer == null) return;

            SaveEdit();
        }

        /// <summary>
        /// destory the current page
        /// </summary>
        public void Deactivate()
        {
        }

        /// <summary>
        /// return if there is any change of current object
        /// </summary>
        public bool IsPageDirty
        {
            get { return _IsPageDirty; }
        }


        /// <summary>
        /// set the object of the page
        /// </summary>
        /// <param name="inObject"></param>
        public void SetObjects(object inObject)
        {
            if (inObject is layerObj)
                m_pLayer = inObject as layerObj;
        }

        #endregion


        /// <summary>
        /// initialize the control
        /// </summary>
        private void Initialize()
        {
            if (m_pLayer == null) return;

            m_strShapePath = m_pLayer.map.shapepath;

            textData.Text = Utility.GetAbsolutePathOfData(m_pLayer);


            int iType = Convert.ToInt32(m_pLayer.type);
            if ((iType >= 0) && (iType < comboBoxDataType.Items.Count))
                comboBoxDataType.SelectedIndex = iType;


            int iConnType = Convert.ToInt32(m_pLayer.connectiontype);
            if ((iConnType > 0) && (iConnType < comboBoxConnType.Items.Count))
                comboBoxConnType.SelectedIndex = iConnType;

            textConnection.Text = m_pLayer.connection;

            //textLabelItem.Text = m_pLayer.labelitem;

            //set the layer extents
            rectObj pRect = m_pLayer.getExtent();
            labelExtentTop.Text = "Top:" + pRect.maxy.ToString();
            labelExtentBottom.Text = "Bottom:" + pRect.miny.ToString();
            labelExtentLeft.Text = "Left:" + pRect.minx.ToString();
            labelExtentRight.Text = "Right:" + pRect.maxx.ToString();

        }

        /// <summary>
        /// save the edit
        /// </summary>
        private void SaveEdit()
        {
            if (m_pLayer == null) return;

            //if there is a shapepath,remove the shapepath
            if ((m_pLayer.map.shapepath != null) ||(m_pLayer.map.shapepath == ""))
            {
                string strShpPath = "";
                if (Path.IsPathRooted(m_pLayer.map.shapepath))
                    strShpPath = m_pLayer.map.shapepath;
                else
                    strShpPath = MFEditor.Utils.Utility.GetAbsolutePath(m_pLayer.map.mappath, m_pLayer.map.shapepath);

                if (Path.GetDirectoryName(textData.Text) == strShpPath)
                    m_pLayer.data = Path.GetFileName(textData.Text);
                else
                    m_pLayer.data = textData.Text;
            }
            else
                m_pLayer.data = textData.Text;

            m_pLayer.type = (MS_LAYER_TYPE)comboBoxDataType.SelectedIndex;

            m_pLayer.connectiontype = (MS_CONNECTION_TYPE)comboBoxConnType.SelectedIndex;
            if (textConnection.Text.Trim().Length == 0)
                m_pLayer.connection = null;
            else
                m_pLayer.connection = textConnection.Text;
        }


        

        /// <summary>
        /// check the input value
        /// </summary>
        /// <returns></returns>
        private bool checkInput()
        {
            if (textData.Text.Trim().Length == 0)
            {
                Utility.PostMessage("dataname未设置！");
                return false;
            }
            else
            {
                //get the absolute path of the data
                string strTruePath = Path.GetFullPath(m_strShapePath);
                string strSHPpath = Path.Combine(strTruePath, textData.Text);

                if (strSHPpath.IndexOf('.') > 0)
                {
                    if (!File.Exists(strSHPpath))
                    {
                        Utility.PostMessage("数据文件不存在，请重新设置data！");
                        return false;
                    }
                }
                else if (!(File.Exists(strSHPpath + ".shp") || File.Exists(strSHPpath + ".tif") || File.Exists(strSHPpath + ".img")))
                {
                    Utility.PostMessage("数据文件不存在，请重新设置data！");
                    return false;
                }
            }

            if (comboBoxDataType.SelectedIndex == -1)
            {
                Utility.PostMessage("DataType没有设置！");
                return false;
            }

            if (comboBoxConnType.SelectedIndex == -1)
            {
                Utility.PostMessage("ConnectionType没有设置！");
                return false;
            }

            return true;

        }

        private void buttonShpPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog pOpenNmp = new OpenFileDialog();
            pOpenNmp.Title = "打开fontset";
            pOpenNmp.Multiselect = false;
            pOpenNmp.InitialDirectory = m_strShapePath;
            pOpenNmp.Filter = "All Formats|*.shp;*.tif;*.img|Shapefile(*.shp)|*.shp|Image(*.img)|*.img|Tiff(*.tif)|*.tif";

            if (pOpenNmp.ShowDialog() == DialogResult.OK)
            {
                string strFullPath = pOpenNmp.FileName;
                textData.Text = Path.GetFileNameWithoutExtension(strFullPath);
            }
        }

        private void comboBoxConnType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxConnType.SelectedIndex != -1)
            {
                if (comboBoxConnType.SelectedItem.ToString() == "SHAPEFILE")
                    buttonShpPath.Enabled = true;
                else
                    buttonShpPath.Enabled = false;
            }

            _IsPageDirty = true;
        }
        

        private void comboBoxDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _IsPageDirty = true;
        }

        private void textConnection_TextChanged(object sender, EventArgs e)
        {
            _IsPageDirty = true;
        }
    }
}
