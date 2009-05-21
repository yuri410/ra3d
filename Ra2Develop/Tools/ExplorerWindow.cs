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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using WeifenLuo.WinFormsUI.Docking;
using Ra2Develop.Projects;
using R3D.UI;

namespace Ra2Develop.Tools
{
    public partial class ExplorerWindow : DockContent, ITool
    {
        enum FSOType : int
        {
            Archive = 0,
            Folder,
            File,
            ArchiveFile,
            ErrorMsg
        }
        class FileSystemObject
        {
            FSOType type;

            string path;

            public FSOType Type
            {
                get { return type; }
                set { type = value; }
            }
            public string Path
            {
                get { return path; }
                set { path = value; }
            }
        }
        
        ProjectBase currentProject;
        
        public ExplorerWindow()
        {
            InitializeComponent();
            
            LanguageParser.ParseLanguage(Program.StringTable, this);

        }

        public ProjectBase CurrentProject
        {
            get { return currentProject; }
            set { currentProject = value; }
        }

        public ExplorerState State
        {
            get { return currentProject == null ? ExplorerState.FreeBrowse : ExplorerState.Project; }
        }

        #region ITool 成员

        public DockContent Form
        {
            get { return this; }
        }

        public bool IsVisibleInMenu
        {
            get { return true; }
        }

        #endregion

        private void refreshTool_Click(object sender, EventArgs e)
        {
            if (State == ExplorerState.FreeBrowse)
            {
                treeView1.Nodes.Clear();                                

                DriveInfo[] drives = DriveInfo.GetDrives();
                for (int i = 0; i < drives.Length; i++)
                {
                    AddNode(treeView1.Nodes, FSOType.Folder, drives[i].RootDirectory.FullName, drives[i].Name);
                }
            }
            else
            {

            }
        }

        void AddNode(TreeNodeCollection nodes, FSOType type, string path)
        {
            AddNode(nodes, type, path, Path.GetFileName(path));
        }
        void AddNode(TreeNodeCollection nodes, FSOType type, string path, string name)
        {
            TreeNode node = new TreeNode();
            node.Text = name;
            FileSystemObject data = new FileSystemObject();
            data.Path = path;
            data.Type = type;
            node.Tag = data;

            nodes.Add(node);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                if (e.Node.Nodes.Count == 0)
                {
                    FileSystemObject data = (FileSystemObject)e.Node.Tag;

                    if (data.Type == FSOType.Folder)
                    {
                        if (Directory.Exists(data.Path))
                        {
                            try
                            {
                                string[] subd = Directory.GetDirectories(data.Path, "*", SearchOption.TopDirectoryOnly);
                                for (int i = 0; i < subd.Length; i++)
                                {
                                    AddNode(e.Node.Nodes, FSOType.Folder, subd[i]);
                                }
                                subd = Directory.GetFiles(data.Path, "*.*", SearchOption.TopDirectoryOnly);
                                for (int i = 0; i < subd.Length; i++)
                                {
                                    AddNode(e.Node.Nodes, FSOType.File, subd[i]);
                                }
                            }
                            catch (Exception ex)
                            {
                                AddNode(e.Node.Nodes, FSOType.ErrorMsg, string.Empty, ex.Message);
                            }

                        }
                        else
                        {
                            AddNode(e.Node.Nodes, FSOType.ErrorMsg, string.Empty, Program.StringTable["ERROR:IVAILD"]);
                        }
                    }
                }
                e.Node.Expand();
            }
        }

        private void ExplorerWindow_Load(object sender, EventArgs e)
        {
            refreshTool_Click(null, null);
        }

        private void ExplorerWindow_TextChanged(object sender, EventArgs e)
        {
            TabText = Text;
        }
    }

    public enum ExplorerState
    {
        FreeBrowse,
        Project
    }
}
