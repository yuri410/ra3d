/*
 * Copyright (C) 2008 R3D Development Team
 * 
 * R3D is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * R3D is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with R3D.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ra2Develop.Converters;
using Ra2Develop.Designers;
using Ra2Develop.Projects;
using Ra2Develop.Templates;
using Ra2Develop.Tools;
using R3D;
using R3D.ConfigModel;
using R3D.Base;
using R3D.GraphicsEngine;
using R3D.GraphicsEngine.Effects;
using R3D.IO;
using R3D.IsoMap;
using R3D.Media;
using WeifenLuo.WinFormsUI.Docking;

namespace Ra2Develop
{
    public partial class MainForm : Form
    {
        readonly static string ConfigFile = Path.Combine("Configs", "environment.xml");
        readonly static string DockingConfigFile = Path.Combine("Configs", "docking.xml");


        BasicConfigs configs;

        #region Helpers
        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        bool DocumentCanSave(DocumentBase doc)
        {
            return !doc.Saved && !doc.IsReadOnly;
        }
        bool DocumentCanSave(IDockContent doc)
        {
            DocumentBase d = (DocumentBase)doc;
            return !d.Saved && !d.IsReadOnly;
        }
        #endregion


        PropertyWindow propertyWnd;
        ExplorerWindow explorerWnd;

        ProjectBase currentProject;

        public MainForm()
        {
            InitializeComponent();

            R3D.UI.LanguageParser.ParseLanguage(Program.StringTable, this);
            R3D.UI.LanguageParser.ParseLanguage(Program.StringTable, menuStripMain);
            R3D.UI.LanguageParser.ParseLanguage(Program.StringTable, toolStripMain);

            if (File.Exists(ConfigFile))
            {
                configs = Serialization.XmlDeserialize<BasicConfigs>(ConfigFile);
            }
            else
            {
                configs = new BasicConfigs();
            }
            ConfigurationManager.Instance.Register(new ConfigurationIniFormat());

            PlatformManager.Instance.RegisterPlatform(PresetedPlatform.RedAlert2, PresetedPlatform.RedAlert2Name);
            PlatformManager.Instance.RegisterPlatform(PresetedPlatform.YurisRevenge, PresetedPlatform.YurisRevengeName);
            PlatformManager.Instance.RegisterPlatform(PresetedPlatform.Ra2Reload, PresetedPlatform.Ra2ReloadName);

            ImageManager.Instance.RegisterImageFormat(new PngImageFactory());
            ImageManager.Instance.RegisterImageFormat(new BmpImageFactory());
            ImageManager.Instance.RegisterImageFormat(new JpegImageFactory());
            AnimManager.Instance.RegisterImageFormat(new ShpAnimFactory());

            TextureManager.Initialize(GraphicsDevice.Instance.Device);

            DesignerManager.Instance.RegisterDesigner(new CsfDocFactory());
            DesignerManager.Instance.RegisterDesigner(new Tile3DDocumentFactory());
            DesignerManager.Instance.RegisterDesigner(new Tile3DDocumentFactory());
            DesignerManager.Instance.RegisterDesigner(new ModelDocumentFactory());
            DesignerManager.Instance.RegisterDesigner(new IniDocumentFactory());

            TemplateManager.Instance.RegisterTemplate(new CsfTemplate());
            TemplateManager.Instance.RegisterTemplate(new Tile3DTemplate());

            ToolManager.Instance.RegisterToolType(new PropertyWndFactory());
            ToolManager.Instance.RegisterToolType(new ExplorerWndFactory());

            ConverterManager.Instance.Register(new Map2Map3Converter());
            ConverterManager.Instance.Register(new Tmp2Tile3DConverter());
            ConverterManager.Instance.Register(new Xml2ModelConverter());
            ConverterManager.Instance.Register(new Vxl2ModelConverter());
            ConverterManager.Instance.Register(new XText2ModelConverter());

            TheaterManager.Instance.RegisterTheaterType(new TheaterFactory());
            TheaterManager.Instance.RegisterTheaterType(new Theater3DFactory(GraphicsDevice.Instance.Device));           

            MapManager.Instance.RegisterMapFormat(new Map3DFactory());
            
            if (configs.GamePath.Length > 0)
            {
                FileSystem.Instance.AddWorkingDir(configs.GamePath);
            }
            if (configs.RA2YRPath.Length > 0)
            {
                Ra2FileSystem.Instance.AddWorkingDir(configs.RA2YRPath);
            }
            FileSystem.Instance.AddWorkingDir(Application.StartupPath);
           
            ConverterBase[] converters = ConverterManager.Instance.GetAllConverters();

            for (int i = 0; i < converters.Length; i++)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(converters[i].Name, converters[i].GetIcon.ToBitmap(), new EventHandler(converters[i].ShowDialog));
                converterMenuItem.DropDownItems.Add(mi);
            }

            if (File.Exists(DockingConfigFile))
            {
                dockPanel.LoadFromXml(DockingConfigFile, LoadToolPanels);
            }
        }



        ProjectBase CurrentProject
        {
            get { return currentProject; }
            set { currentProject = value; }
        }

        IDockContent LoadToolPanels(string name)
        {
            try
            {
                ITool tool = ToolManager.Instance.CreateTool(name);                

                if (tool.GetType() == typeof(PropertyWindow))
                {
                    propertyWnd = (PropertyWindow)tool;
                }
                else if (tool.GetType() == typeof(ExplorerWindow))
                {
                    explorerWnd = (ExplorerWindow)tool;
                }

                return tool.Form;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        
        public void AddDocumentTab(DocumentBase des)
        {
            //docs.Add(des);

            des.LoadRes();

            des.Show(dockPanel, DockState.Document);

            SetLayout(des);
        }

        void SetLayout(DocumentBase des)
        {
            int x = toolStripMain.Width + toolStripMain.Left;
            int y = toolStripMain.Top;
            ToolStrip[] list = des.ToolStrips;

            for (int i = 0; i < list.Length; i++)
            {
                if (x >toolStripContainer1.TopToolStripPanel.Width)
                {
                    x = 0;
                    y += toolStripMain.Height;
                }

                list[i].Location = new Point(x, y);

                x += list[i].Width;
            }
        }

        void docSaveStateChanged(object sender)
        {
            bool state = DocumentCanSave((DocumentBase)sender);// !saved && !((IDocument)sender).IsReadOnly;
            saveMenuItem.Enabled = state;
            saveTool.Enabled = state;
        }
        void docPropertyUpdateNeeded(object sender, object[] allObjs)
        {
            if (propertyWnd != null)
            {
                propertyWnd.SetObjects(sender, allObjs);
            }
        }
        void docClosing(object sender, FormClosingEventArgs e)
        {
            CloseDocumentResult res = CloseDocuments(new IDockContent[] { (IDockContent)sender });
            if (res == CloseDocumentResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                SwichDocumentTab(null);
            }
        }

        void SwichDocumentTab(DocumentBase des)
        {
            for (int i = toolStripContainer1.TopToolStripPanel.Controls.Count - 1; i >= 0; i--)
            {
                string n = toolStripContainer1.TopToolStripPanel.Controls[i].Name;
                if (n != toolStripMain.Name && n != menuStripMain.Name)
                    toolStripContainer1.TopToolStripPanel.Controls.RemoveAt(i);
            }

            foreach (IDockContent con in dockPanel.Documents)
            {
                DocumentBase doc = (DocumentBase)con;
                if (((IDocument)doc).IsActivated)
                {
                    doc.SavedStateChanged -= docSaveStateChanged;
                    doc.PropertyUpdate -= docPropertyUpdateNeeded;
                    doc.FormClosing -= docClosing;
                    docPropertyUpdateNeeded(null, null);

                    ((IDocument)doc).DocDeactivate();
                }
            }

            if (des != null)
            {
                ToolStrip[] list = des.ToolStrips;
                for (int i = 0; i < list.Length; i++)
                    toolStripContainer1.TopToolStripPanel.Controls.Add(list[i]);

                des.SavedStateChanged += docSaveStateChanged;
                des.PropertyUpdate += docPropertyUpdateNeeded;
                des.FormClosing += docClosing;
                
                docPropertyUpdateNeeded(null, null);
                docSaveStateChanged(des);

                ((IDocument)des).DocActivate();
            }
        }
        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            SwichDocumentTab((DocumentBase)dockPanel.ActiveDocument);
        }


        /// <summary>
        /// 保存需要保存的文档
        /// </summary>
        /// <param name="doc"></param>
        void SaveDocument(DocumentBase doc)
        {
            if (!doc.Saved)
            {
                if (doc.ResourceLocation == null || doc.IsReadOnly)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        saveFileDialog1.Filter = doc.Factory.Filter;
                        doc.ResourceLocation = new FileLocation(saveFileDialog1.FileName);
                        doc.SaveRes();
                        statusLabel.Text = Program.StringTable["STATUS:SAVEDDOC"];
                    }
                }
                else
                {
                    doc.SaveRes();
                    statusLabel.Text = Program.StringTable["STATUS:SAVEDDOC"];
                }
                
            }
        }


        public IDockContent CurrentDocument
        {
            get { return dockPanel.ActiveDocument; }
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IDockContent[] docs = dockPanel.DocumentsToArray();
            CloseDocumentResult res = CloseDocuments(docs);

            if (res == CloseDocumentResult.Saved)
            {
                for (int i = 0; i < docs.Length; i++)
                    docs[i].DockHandler.Close();

                DocumentConfigBase.SaveAll();

                dockPanel.SaveAsXml(DockingConfigFile);

                Serialization.XmlSerialize<BasicConfigs>(configs, ConfigFile);
            }
            else if (res == CloseDocumentResult.Cancel)
            {
                e.Cancel = true;
            }

        }

        public CloseDocumentResult CloseDocuments(IDockContent[] docFrms)
        {
            //IDocument[] docs = new IDocument[docs.Length];
            List<DocumentBase> docs = new List<DocumentBase>(docFrms.Length);

            for (int i = 0; i < docFrms.Length; i++)
            {
                DocumentBase doc = (DocumentBase)docFrms[i];
                if (!doc.IsReadOnly && !doc.Saved)
                {
                    docs.Add(doc);
                }
            }

            if (docs.Count > 0)
            {
                Pair<DialogResult, DocumentBase[]> res = SaveConfirmationDlg.Show(this, docs.ToArray());
                if (res.a == DialogResult.Yes)
                {
                    for (int i = 0; i < res.b.Length; i++)
                    {
                        res.b[i].SaveRes();
                    }
                    return CloseDocumentResult.Saved;
                }
                else if (res.a == DialogResult.No)
                {
                    return CloseDocumentResult.NotSaved;
                }

                return CloseDocumentResult.Cancel;
            }
            return CloseDocumentResult.Saved;

        }



        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            if (dockPanel.ActiveDocument != null)
            {
                DocumentBase doc = (DocumentBase)dockPanel.ActiveDocument;
                SaveDocument(doc);
            }
        }

        private void openFileMenuItem_Click(object sender, EventArgs e)
        {
            //StringBuilder sb = new StringBuilder();

            //Pair<string, string>[] fmts = DesignerManager.Instance.GetAllFormats();
            //for (int i = 0; i < fmts.Length; i++)
            //{
            //    sb.Append(fmts[i].a);
            //    sb.Append('(' + fmts[i].b + ')');
            //    sb.Append('|');
            //    sb.Append(fmts[i].b);
            //    sb.Append('|');
            //}
            //sb.Append(Program.StringTable["DOCS:ALLFILES"]);
            //sb.Append(" (*.*)|");
            //sb.Append("*.*");

            openFileDialog1.Filter = DesignerManager.Instance.GetFilter();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] files = openFileDialog1.FileNames;
                for (int i = 0; i < files.Length; i++)
                {
                    FileLocation rl;
                    if (openFileDialog1.ReadOnlyChecked)
                    {
                        rl = new FileLocation(files[i]);
                    }
                    else
                    {
                        rl = new DevFileLocation(files[i]);
                    }

                    AddDocumentTab(DesignerManager.Instance.CreateDocument(rl, Path.GetExtension(rl.Path)));
                }
            }
        }

        private void saveAsMenuItem_Click(object sender, EventArgs e)
        {
            if (dockPanel.ActiveDocument != null)
            {
                DocumentBase doc = (DocumentBase)dockPanel.ActiveDocument;

                if (doc.IsReadOnly)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        saveFileDialog1.Filter = doc.Factory.Filter;
                        doc.ResourceLocation = new DevFileLocation(saveFileDialog1.FileName);
                        doc.SaveRes();
                    }

                    statusLabel.Text = Program.StringTable["STATUS:SAVEDDOC"];
                }
                else
                {
                    statusLabel.Text = Program.StringTable["STATUS:READONLYDOC"];
                }
            }

        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveAllMenuItem_Click(object sender, EventArgs e)
        {
            IDockContent[] docs = dockPanel.DocumentsToArray();
            for (int i = 0; i < docs.Length; i++)
            {
                SaveDocument((DocumentBase)docs[i]);
            }
        }

        private void propertyWndMenuItem_Click(object sender, EventArgs e)
        {
            if (propertyWnd != null)
            {
                propertyWnd.Show(dockPanel, propertyWnd.DockState);
            }
            else
            {
                propertyWnd = (PropertyWindow)ShowTool<PropertyWindow>(DockState.DockRight);

            }
        }

        private void managerWndMenuItem_Click(object sender, EventArgs e)
        {
            if (explorerWnd != null)
            {
                explorerWnd.Show(dockPanel, explorerWnd.DockState);
            }
            else 
            {
                explorerWnd = (ExplorerWindow)ShowTool<ExplorerWindow>(DockState.DockRight);
            }
        }

        ITool ShowTool<T>(DockState ds) where T : ITool
        {
            ITool tool = ToolManager.Instance.CreateTool(typeof(T));
            tool.Form.Show(dockPanel, ds);
            return tool;
        }

        private void newFileMenuItem_Click(object sender, EventArgs e)
        {
            DocumentBase doc;
            if (NewFileForm.ShowDlg(this, false, out doc) == DialogResult.OK)
            {
                AddDocumentTab(doc);
            }
        }



    }

    public enum CloseDocumentResult
    {
        Saved,
        NotSaved,
        Cancel
    }
}
