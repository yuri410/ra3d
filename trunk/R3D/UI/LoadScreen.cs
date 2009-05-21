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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using R3D.ConfigModel;
using R3D.Base;
using R3D.IO;
using R3D.Media;
using R3D.UI.Controls;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.UI
{
    public class LoadScreen : IDisposable
    {
        public const string LoadingSection = "Loading";

        public static readonly string LoadScreenPic;
        public static readonly string LoadScreenPal;

        Texture tTexLdr;

        //Texture tLoadingText;
        Label loadingLbl;
        Label cpyrightLeft;
        Label cpyrightRight;

        bool disposed;
        //Sprite spr;


        static LoadScreen()
        {
            LoadScreenPic = FileSystem.LangMix + "glsl" + Game.Suffix + FileSystem.dotShp;
            LoadScreenPal = FileSystem.LangMix + "gls" + Game.Suffix + FileSystem.dotPal;

        }

        public LoadScreen(GameUI gameUI, ConfigModel.Configuration ui)
        {
            float oneLineHeight;

            //ImageBase bgImg = ui["Loading"].GetImage(FileSystem.GameLangLR);

            UIImageInformation bgImgInfo = ui["Loading"].GetImage(FileSystem.GameLangLR);
            tTexLdr = UITextureManager.Instance.CreateInstance(bgImgInfo);
            ////UIImage bgImg;

            ////bgImg = new UIImage(ui["LOADING"], FileSystem.LangMix, Game.Suffix, FileSystem.GameLangLR);
            //tTexLdr = bgImg.GetTexture(gameUI.Device, Usage.None, Pool.Managed);// bgImg.ToTexture(dev, Usage.None, Pool.Managed);// Texture.FromBitmap(dev, bgImg.Data, Usage.None, Pool.Managed);
            ////bgImg.Dispose();
            //bgImg.Dispose();

            ConfigurationSection sect = ui["Loading.LoadLbl"];

            loadingLbl = Label.FromConfigSection(gameUI, sect);//, "GUI:LOADINGEX", //new Point(5, 5));

            if (loadingLbl.Location == Point.Empty)
            {
                loadingLbl.X = 5;
                loadingLbl.Y = 5;
            }

            sect = ui["LOADING.COPYRIGHTL"];
            cpyrightLeft = Label.FromConfigSection(gameUI, sect);//, "GUI:TRADEMARKTOP, GUI:TRADEMARKBOTTOM",
            //  new Point());
            oneLineHeight = cpyrightLeft.MeasureString().Height;
            cpyrightLeft.AutoSize = false;
            cpyrightLeft.X = 5;
            cpyrightLeft.Y = (int)(GameUI.UIHeight - 5 - 2 * oneLineHeight);
            cpyrightLeft.Size = new Size(480 - 5, 2 * (int)oneLineHeight);

            sect = ui["LOADING.COPYRIGHTR"];
            cpyrightRight = Label.FromConfigSection(gameUI, sect);//, "TXT_COPYRIGHT",                new Point());
            oneLineHeight = cpyrightRight.MeasureString().Height;
            cpyrightRight.X = 500;
            cpyrightRight.Y = (int)(GameUI.UIHeight - 5 - 2 * oneLineHeight);
            cpyrightRight.AutoSize = false;
            cpyrightRight.Size = new Size(300 - 5, (int)oneLineHeight * 2);
        }

        public void PaintUI(Sprite spr)
        {
            spr.Transform = Matrix.Identity;
            spr.Draw(tTexLdr, -1);

            loadingLbl.PaintControl();
            cpyrightLeft.PaintControl();
            cpyrightRight.PaintControl();
        }

        public void Update(float dt)
        {
            loadingLbl.Update(dt);
            cpyrightLeft.Update(dt);
            cpyrightRight.Update(dt);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (loadingLbl != null)
                    loadingLbl.Dispose();
                if (cpyrightLeft != null)
                    cpyrightLeft.Dispose();
                if (cpyrightRight != null)
                    cpyrightRight.Dispose();

                UITextureManager.Instance.DestoryInstance(tTexLdr);//.Dispose();//.DisposeHandler(null, EventArgs.Empty);
                disposed = true;
                //GC.SuppressFinalize(this);
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }

        ~LoadScreen()
        {
            if (!disposed)
                Dispose();
        }
    }

    /// <summary>   
    /// 默认 遭遇战和多人游戏的载入图
    /// </summary>
    public class SkirmishLoadScreen : IBattleLoadScreen
    {
        Label label;

        public bool Disposed
        {
            get;
            protected set;
        }
        public Texture BackgroundImage
        {
            get;
            protected set;
        }

        volatile float progress;
        volatile bool requiresUpdate;

        object syncHelper;

        public SkirmishLoadScreen(GameUI gameUI, string name)
        {
            label = new Label(gameUI);
            //label.Size = new Size(150, 50);
            //label.Text = "0";
            //label.Font = new System.Drawing.Font("宋体", 10.5f);
            //label.ModulateColor = Color.White;

            //label.ForeColor = Color.White;


            ConfigModel.Configuration uiConfig = gameUI.UIConfig;
            ConfigurationSection sect = uiConfig[name];

            Parse(sect);

            syncHelper = new object();
        }

        void Parse(ConfigurationSection sect)
        {
            UIImageInformation imgInfo = sect.GetImage(FileSystem.GameResLR);
            BackgroundImage = UITextureManager.Instance.CreateInstance(imgInfo);

            label.Text = sect.GetUIString("LoadingText", string.Empty);
            label.Location = sect.GetPoint("LoadingTextPosition", Point.Empty);

        }


        #region IBattleLoadScreen 成员

        public float Progress
        {
            get { return progress; }
            set
            {
                lock (syncHelper)
                {
                    requiresUpdate = progress != value;

                    progress = value;
                }
            }
        }

        public void PaintUI(Sprite spr)
        {
            //if (requiresUpdate)
            //    label.Text = "载入中：" + progress.ToString();

            //spr.Draw(Game.Instance.GameUI.Menu.MapBoard, -1);
            //label.OnPaint(spr);

            spr.Transform = Matrix.Identity;
            if (BackgroundImage != null)
            {
                spr.Draw(BackgroundImage, -1);
            }

            label.PaintControl();
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (!Disposed)
            {
                if (label != null)
                {
                    label.Dispose();
                    label = null;
                }

                if (BackgroundImage != null)
                {
                    BackgroundImage.Dispose();
                    BackgroundImage = null;
                }
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        ~SkirmishLoadScreen()
        {
            if (!Disposed)
            {
                Dispose();
            }
        }
    }
}
