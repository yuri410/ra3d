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
using System.Text;
using R3D.ConfigModel;
using R3D.IO;
using R3D.Media;
using R3D.UI.Controls;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;

namespace R3D.UI
{
    public class SideBar : Control
    {
        const int RadarWidth = 107;
        const int RadarHeight = 108;

        const int SideBarWidth = 168;
        const int ACBHeight = 32;

        string name;

        Texture radar;

        Button repairBtn;
        Button sellBtn;

        Button tlBtn;
        Button trBtn;

        Button[] tabs;
        Button[] acbBtns;

        Button cameoPu;
        Button cameoPd;

        #region Textures
        Texture ACBLeftCap
        {
            get;
            set;
        }
        Texture ACBRightCap
        {
            get;
            set;
        }
        Texture ACBBackground
        {
            get;
            set;
        }
        Texture CreditsBackground
        {
            get;
            set;
        }
        Texture TopBackground
        {
            get;
            set;
        }
        Texture TopLeftButton
        {
            get;
            set;
        }
        Texture TopLeftButtonDown
        {
            get;
            set;
        }
        Texture TopRightButton
        {
            get;
            set;
        }
        Texture TopRightButtonDown
        {
            get;
            set;
        }
        Texture Tab01Image
        {
            get;
            set;
        }
        Texture Tab01ImageInv
        {
            get;
            set;
        }
        Texture Tab01ImageAct
        {
            get;
            set;
        }
        Texture Tab02Image
        {
            get;
            set;
        }
        Texture Tab02ImageInv
        {
            get;
            set;
        }
        Texture Tab02ImageAct
        {
            get;
            set;
        }
        Texture Tab03Image
        {
            get;
            set;
        }
        Texture Tab03ImageInv
        {
            get;
            set;
        }
        Texture Tab03ImageAct
        {
            get;
            set;
        }
        Texture Tab04Image
        {
            get;
            set;
        }
        Texture Tab04ImageInv
        {
            get;
            set;
        }
        Texture Tab04ImageAct
        {
            get;
            set;
        }
        Texture RepairButton
        {
            get;
            set;
        }
        Texture RepairButtonDown
        {
            get;
            set;
        }
        Texture SellButton
        {
            get;
            set;
        }
        Texture SellButtonDown
        {
            get;
            set;
        }

        Texture MiddleBackground
        {
            get;
            set;
        }
        Texture CameoBackground
        {
            get;
            set;
        }
        Texture BottomImage
        {
            get;
            set;
        }
        Texture CameoPageUdBg
        {
            get;
            set;
        }

        Texture CameoPageUp
        {
            get;
            set;
        }
        Texture CameoPageUpDn
        {
            get;
            set;
        }
        Texture CameoPageUpInv
        {
            get;
            set;
        }
        Texture CameoPageDown
        {
            get;
            set;
        }
        Texture CameoPageDownDn
        {
            get;
            set;
        }
        Texture CameoPageDownInv
        {
            get;
            set;
        }
        #endregion

        public SideBar(BattleUI bui, string name)
            : base(bui.GameUI, name)
        {
            this.name = name;

            repairBtn = new Button(GameUI);
            sellBtn = new Button(GameUI);
            tlBtn = new Button(GameUI);
            trBtn = new Button(GameUI);

            repairBtn.Enabled = true;
            repairBtn.IsValid = true;
            repairBtn.Text = string.Empty;

            sellBtn.Enabled = true;
            sellBtn.IsValid = true;
            sellBtn.Text = string.Empty;

            tlBtn.Enabled = true;
            tlBtn.IsValid = true;
            tlBtn.Text = string.Empty;

            trBtn.Enabled = true;
            trBtn.IsValid = true;
            trBtn.Text = string.Empty;

            tabs = new Button[4];

            for (int i = 0; i < 4; i++)
            {
                tabs[i] = new Button(GameUI, name + Type.Delimiter + "Tab" + i.ToString());
                tabs[i].IsValid = true;
                tabs[i].Enabled = false;
                tabs[i].Text = string.Empty;
            }

            cameoPu = new Button(GameUI);
            cameoPu.Enabled = false;
            cameoPu.IsValid = true;
            cameoPu.Text = string.Empty;

            cameoPd = new Button(GameUI);
            cameoPd.Enabled = false;
            cameoPd.IsValid = true;
            cameoPd.Text = string.Empty;

        }

        public override void Parse(ConfigurationSection sect)
        {
        }

        public void Load()
        {
            ConfigModel.Configuration uiConfig = GameUI.UIConfig;

            ConfigurationSection sect = uiConfig[name];

            PaletteCache pCache = new PaletteCache();
            UITextureManager.Instance.PaletteCache = pCache;


            UIImageInformation imgInfo = sect.GetImage("Bottom", FileSystem.GameResLR);
            imgInfo = sect.GetImage("ACBLeftCap", FileSystem.GameResLR);            
            ACBLeftCap = UITextureManager.Instance.CreateInstance(imgInfo);



            imgInfo = sect.GetImage("ACBRightCap", FileSystem.GameResLR);
            ACBRightCap = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("ACBBackground", FileSystem.GameResLR);
            ACBBackground = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("CreditsBackground", FileSystem.GameResLR);
            CreditsBackground = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("TopBackground", FileSystem.GameResLR);
            TopBackground = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("TopLeftButton", FileSystem.GameResLR);
            TopLeftButton = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("TopLeftButtonDn", FileSystem.GameResLR);
            TopLeftButtonDown = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("TopRightButton", FileSystem.GameResLR);
            TopRightButton = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("TopRightButtonDn", FileSystem.GameResLR);
            TopRightButtonDown = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab01", FileSystem.GameResLR);
            Tab01Image = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab01Inv", FileSystem.GameResLR);
            Tab01ImageInv = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab01Act", FileSystem.GameResLR);
            Tab01ImageAct = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab02", FileSystem.GameResLR);
            Tab02Image = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab02Inv", FileSystem.GameResLR);
            Tab02ImageInv = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab02Act", FileSystem.GameResLR);
            Tab02ImageAct = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab03", FileSystem.GameResLR);
            Tab03Image = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab03Inv", FileSystem.GameResLR);
            Tab03ImageInv = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab03Act", FileSystem.GameResLR);
            Tab03ImageAct = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab04", FileSystem.GameResLR);
            Tab04Image = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab04Inv", FileSystem.GameResLR);
            Tab04ImageInv = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Tab04Act", FileSystem.GameResLR);
            Tab04ImageAct = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("RepairButton", FileSystem.GameResLR);
            RepairButton = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("RepairButtonDn", FileSystem.GameResLR);
            RepairButtonDown = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("SellButton", FileSystem.GameResLR);
            SellButton = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("SellButtonDn", FileSystem.GameResLR);
            SellButtonDown = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("MiddleBackground", FileSystem.GameResLR);
            MiddleBackground = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("CameoBackground", FileSystem.GameResLR);
            CameoBackground = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("Bottom", FileSystem.GameResLR);
            BottomImage = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("CameoPageUdBg", FileSystem.GameResLR);
            CameoPageUdBg = UITextureManager.Instance.CreateInstance(imgInfo);



            imgInfo = sect.GetImage("CameoPageUp", FileSystem.GameResLR);
            CameoPageUp = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("CameoPageUpDn", FileSystem.GameResLR);
            CameoPageUpDn = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("CameoPageUpInv", FileSystem.GameResLR);
            CameoPageUpInv = UITextureManager.Instance.CreateInstance(imgInfo);



            imgInfo = sect.GetImage("CameoPageDown", FileSystem.GameResLR);
            CameoPageDown = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("CameoPageDownDn", FileSystem.GameResLR);
            CameoPageDownDn = UITextureManager.Instance.CreateInstance(imgInfo);

            imgInfo = sect.GetImage("CameoPageDownInv", FileSystem.GameResLR);
            CameoPageDownInv = UITextureManager.Instance.CreateInstance(imgInfo);




            sect = uiConfig[name + Type.Delimiter + "CameoBuildClock"];
            sect = uiConfig[name + Type.Delimiter + "Radar"];





            sect = uiConfig["AdvancedCommandBar"];


            string[] btnList = sect.GetStringArray("ButtonList");
            acbBtns = new Button[btnList.Length];


            int acbBtnLeft = 28;
            int acbBtnTop = Height - 32;

            for (int i = 0; i < btnList.Length; i++)
            {
                Button btn = new Button(GameUI, name + Type.Delimiter + btnList[i]);
                btn.Enabled = true;
                btn.IsValid = true;
                btn.Text = string.Empty;
                btn.Size = new Size(52, 32);
                btn.Location = new Point(acbBtnLeft, acbBtnTop);
                acbBtnLeft += 52;

                sect = uiConfig[name + Type.Delimiter + btnList[i]];

                imgInfo = sect.GetImage("Normal", FileSystem.GameResLR);
                Texture normal = UITextureManager.Instance.CreateInstance(imgInfo);
                btn.Image = normal;
                btn.ImageMouseOver = normal;
                btn.ImageInvalid = normal;
                btn.ImageDisabled = normal;


                imgInfo = sect.GetImage("Pressed", FileSystem.GameResLR);
                Texture pressed = UITextureManager.Instance.CreateInstance(imgInfo);
                btn.ImageMouseDown = pressed;

                acbBtns[i] = btn;
                
            }
            tlBtn.Image = TopLeftButton;
            tlBtn.ImageMouseOver = TopLeftButton;
            tlBtn.ImageMouseDown = TopLeftButtonDown;
            tlBtn.Location = new Point(643, 20);
            tlBtn.Size = new Size(72, 18);

            trBtn.Image = TopRightButton;
            trBtn.ImageMouseOver = TopRightButton;
            trBtn.ImageMouseDown = TopRightButtonDown;
            trBtn.Location = new Point(715, 20);
            trBtn.Size = new Size(72, 18);

            repairBtn.Image = RepairButton;
            repairBtn.ImageMouseOver = RepairButton;
            repairBtn.ImageMouseDown = RepairButtonDown;
            repairBtn.Location = new Point(652, 166);
            repairBtn.Size = new Size(64, 31);

            sellBtn.Image = SellButton;
            sellBtn.ImageMouseOver = SellButton;
            sellBtn.ImageMouseDown = SellButtonDown;
            sellBtn.Location = new Point(716, 166);
            sellBtn.Size = new Size(64, 31);

            tabs[0].Image = Tab01Image;
            tabs[0].ImageMouseOver = Tab01Image;
            tabs[0].ImageMouseDown = Tab01ImageAct;
            tabs[0].ImageDisabled = Tab01ImageInv;
            tabs[0].Location = new Point(658, 197);
            tabs[0].Size = new Size(28, 27);

            tabs[1].Image = Tab02Image;
            tabs[1].ImageMouseOver = Tab02Image;
            tabs[1].ImageMouseDown = Tab02ImageAct;
            tabs[1].ImageDisabled = Tab02ImageInv;
            tabs[1].Location = new Point(687, 197);
            tabs[1].Size = new Size(28, 27);

            tabs[2].Image = Tab03Image;
            tabs[2].ImageMouseOver = Tab03Image;
            tabs[2].ImageMouseDown = Tab03ImageAct;
            tabs[2].ImageDisabled = Tab03ImageInv;
            tabs[2].Location = new Point(717, 197);
            tabs[2].Size = new Size(28, 27);

            tabs[3].Image = Tab04Image;
            tabs[3].ImageMouseOver = Tab04Image;
            tabs[3].ImageMouseDown = Tab04ImageAct;
            tabs[3].ImageDisabled = Tab04ImageInv;
            tabs[3].Location = new Point(746, 197);
            tabs[3].Size = new Size(28, 27);



            cameoPu.Image = CameoPageUp; 
            cameoPu.ImageMouseOver = CameoPageUp;
            cameoPu.ImageMouseDown = CameoPageUpDn;
            cameoPu.ImageDisabled = CameoPageUpInv;
            cameoPu.Location = new Point(717, 534);
            cameoPu.Size = new Size(46, 25);

            cameoPd.Image = CameoPageDown;
            cameoPd.ImageMouseOver = CameoPageDown;
            cameoPd.ImageMouseDown = CameoPageDownDn;
            cameoPd.ImageDisabled = CameoPageDownInv;
            cameoPd.Location = new Point(671, 534);
            cameoPd.Size = new Size(46, 25);

            UITextureManager.Instance.PaletteCache = null;

            radar = new Texture(Device, RadarWidth, RadarHeight, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

        }


        public new void InvokeMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            for (int i = 0; i < acbBtns.Length; i++)
            {
                if (acbBtns[i] != null)
                {
                    acbBtns[i].InvokeMouseMove(e);
                }
            }

            tlBtn.InvokeMouseMove(e);
            trBtn.InvokeMouseMove(e);
            for (int i = 0; i < 4; i++)
            {
                tabs[i].InvokeMouseMove(e);
            }
            repairBtn.InvokeMouseMove(e);
            sellBtn.InvokeMouseMove(e);
            cameoPu.InvokeMouseMove(e);
            cameoPd.InvokeMouseMove(e);
        }
        public new void InvokeMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            for (int i = 0; i < acbBtns.Length; i++)
            {
                if (acbBtns[i] != null)
                {
                    acbBtns[i].InvokeMouseUp(e);
                }
            }

            tlBtn.InvokeMouseUp(e);
            trBtn.InvokeMouseUp(e);
            for (int i = 0; i < 4; i++)
            {
                tabs[i].InvokeMouseUp(e);
            }
            repairBtn.InvokeMouseUp(e);
            sellBtn.InvokeMouseUp(e);
            cameoPu.InvokeMouseUp(e);
            cameoPd.InvokeMouseUp(e);
        }
        public new void InvokeMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            for (int i = 0; i < acbBtns.Length; i++)
            {
                if (acbBtns[i] != null)
                {
                    acbBtns[i].InvokeMouseDown(e);
                }
            }

            tlBtn.InvokeMouseDown(e);
            trBtn.InvokeMouseDown(e);
            for (int i = 0; i < 4; i++)
            {
                tabs[i].InvokeMouseDown(e);
            }
            repairBtn.InvokeMouseDown(e);
            sellBtn.InvokeMouseDown(e);
            cameoPu.InvokeMouseDown(e);
            cameoPd.InvokeMouseDown(e);
        }
        public new void InvokeMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            for (int i = 0; i < acbBtns.Length; i++)
            {
                if (acbBtns[i] != null)
                {
                    acbBtns[i].InvokeMouseWheel(e);
                }
            }

            tlBtn.InvokeMouseWheel(e);
            trBtn.InvokeMouseWheel(e);
            for (int i = 0; i < 4; i++)
            {
                tabs[i].InvokeMouseWheel(e);
            }
            repairBtn.InvokeMouseWheel(e);
            sellBtn.InvokeMouseWheel(e);
            cameoPu.InvokeMouseWheel(e);
            cameoPd.InvokeMouseWheel(e);
        }
        public new void InvokeKeyStateChanged(KeyCollection pressed)
        {
            //if (HasHotKey)
            //{
            //    bool passed = true;
            //    for (int i = 0; i < HotKey.Count; i++)
            //    {
            //        if (!pressed.Contains(HotKey[i]))
            //        {
            //            passed = false;
            //            break;
            //        }
            //    }

            //    if (passed)
            //    {
            //        OnHotkeyInvoked(EventArgs.Empty);
            //    }
            //}
            //for (int i = 0; i < controls.Count; i++)
            //{
            //    controls[i].InvokeKeyStateChanged(pressed);
            //}
            for (int i = 0; i < acbBtns.Length; i++)
            {
                if (acbBtns[i] != null)
                {
                    acbBtns[i].InvokeKeyStateChanged(pressed);
                }
            }

            tlBtn.InvokeKeyStateChanged(pressed);
            trBtn.InvokeKeyStateChanged(pressed);
            for (int i = 0; i < 4; i++)
            {
                tabs[i].InvokeKeyStateChanged(pressed);
            }
            repairBtn.InvokeKeyStateChanged(pressed);
            sellBtn.InvokeKeyStateChanged(pressed);
            cameoPu.InvokeKeyStateChanged(pressed);
            cameoPd.InvokeKeyStateChanged(pressed);
        }

        

        public override void PaintControl()
        {
            base.PaintControl();

            int left = Width - 168;
            int modClrArgb = modColor.ToArgb();

            PaintACB(modClrArgb);

            if (CreditsBackground != null)
            {
                Sprite.Transform = Matrix.Translation(left, 0, 0);
                Sprite.Draw(CreditsBackground, modClrArgb);
            }
            if (TopBackground != null)
            {
                Sprite.Transform = Matrix.Translation(left, 16, 0);
                Sprite.Draw(TopBackground, modClrArgb);
            }
            // radar +16+32

            if (MiddleBackground != null)
            {
                Sprite.Transform = Matrix.Translation(left, 16 + 32 + 110, 0);
                Sprite.Draw(MiddleBackground, modClrArgb);
            }
            int top = 16 + 32 + 110 + 69;

            while (top <= Height - 63 - 26)            
            {
                Sprite.Transform = Matrix.Translation(left, top, 0);
                Sprite.Draw(CameoBackground, modClrArgb);
                top += 50;
            }
            if (CameoPageUdBg != null)
            {
                //Sprite.Transform = Matrix.Translation(left, Height - 63 - 26, 0);
                Sprite.Transform = Matrix.Translation(left, top, 0);
                Sprite.Draw(CameoPageUdBg, modClrArgb);

                top += 26;
            }

            if (BottomImage != null)            
            {
                //Sprite.Transform = Matrix.Translation(left, Height - 63, 0);
                Sprite.Transform = Matrix.Translation(left, top, 0);
                Sprite.Draw(BottomImage, modClrArgb);
            }

            tlBtn.PaintControl();
            trBtn.PaintControl();

            repairBtn.PaintControl();
            sellBtn.PaintControl();

            for (int i = 0; i < 4; i++)
            {
                tabs[i].PaintControl();
            }
            cameoPu.PaintControl();
            cameoPd.PaintControl();
        }

        void PaintACB(int modClrArgb)
        {
            int acbBtnLeft = 0;
            int acbBtnTop = Height - 32;

            if (ACBLeftCap != null)
            {
                Sprite.Transform = Matrix.Translation(acbBtnLeft, acbBtnTop, 0);
                Sprite.Draw(ACBLeftCap, modClrArgb);
            }
            acbBtnLeft += 28;

            for (int i = 0; i < acbBtns.Length; i++)
            {
                if (acbBtns[i] != null)
                {
                    acbBtns[i].PaintControl();
                    acbBtnLeft += acbBtns[i].Width;
                }
            }

            while (acbBtnLeft < Width - 168)
            {
                Sprite.Transform = Matrix.Translation(acbBtnLeft, acbBtnTop, 0);
                Sprite.Draw(ACBBackground, modClrArgb);
                acbBtnLeft += 52;
            }

            if (ACBRightCap != null)
            {
                Sprite.Transform = Matrix.Translation(Width - 168 - 28, acbBtnTop, 0);
                Sprite.Draw(ACBRightCap, modClrArgb);
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);


            for (int i = 0; i < acbBtns.Length; i++)
            {
                if (acbBtns[i] != null)
                {
                    acbBtns[i].Update(dt);
                }
            }

            tlBtn.Update(dt);
            trBtn.Update(dt);

            repairBtn.Update(dt);
            sellBtn.Update(dt);

            for (int i = 0; i < 4; i++)
            {
                tabs[i].Update(dt);
            }
            cameoPu.Update(dt);
            cameoPd.Update(dt);

        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)            
            {
                repairBtn.Dispose();
                sellBtn.Dispose();
                tlBtn.Dispose();
                trBtn.Dispose();
                cameoPu.Dispose();
                cameoPd.Dispose();

                radar.Dispose();

                for (int i = 0; i < 4; i++)
                {
                    tabs[i].Dispose();
                    tabs[i] = null;
                }
                for (int i = 0; i < acbBtns.Length; i++)
                {
                    if (!acbBtns[i].Image.Disposed)
                    {
                        acbBtns[i].Image.Dispose();
                        acbBtns[i].Image = null;
                    }
                    if (!acbBtns[i].ImageMouseDown.Disposed)
                    {
                        acbBtns[i].ImageMouseDown.Dispose();
                        acbBtns[i].ImageMouseDown = null;
                    }
                    if (!acbBtns[i].ImageDisabled.Disposed)
                    {
                        acbBtns[i].ImageDisabled.Dispose();
                        acbBtns[i].ImageDisabled = null;
                    }
                    if (!acbBtns[i].ImageInvalid.Disposed)
                    {
                        acbBtns[i].ImageInvalid.Dispose();
                        acbBtns[i].ImageInvalid = null;
                    } 
                    if (!acbBtns[i].ImageMouseOver.Disposed)
                    {
                        acbBtns[i].ImageMouseOver.Dispose();
                        acbBtns[i].ImageMouseOver = null;
                    }

                    acbBtns[i].Dispose();
                    acbBtns[i] = null;
                }

                radar = null;

                ACBBackground.Dispose();
                ACBLeftCap.Dispose();
                ACBRightCap.Dispose();
                CreditsBackground.Dispose();
                TopBackground.Dispose();
                TopLeftButton.Dispose();
                TopLeftButtonDown.Dispose();
                TopRightButton.Dispose();
                TopRightButtonDown.Dispose();
                Tab01Image.Dispose();
                Tab01ImageInv.Dispose();
                Tab01ImageAct.Dispose();
                Tab02Image.Dispose();
                Tab02ImageInv.Dispose();
                Tab02ImageAct.Dispose();
                Tab03Image.Dispose();
                Tab03ImageInv.Dispose();
                Tab03ImageAct.Dispose();
                Tab04Image.Dispose();
                Tab04ImageInv.Dispose();
                Tab04ImageAct.Dispose();
                RepairButton.Dispose();
                RepairButtonDown.Dispose();
                SellButton.Dispose();
                SellButtonDown.Dispose();

                MiddleBackground.Dispose();
                CameoBackground.Dispose();
                BottomImage.Dispose();
                CameoPageUdBg.Dispose();

                CameoPageUp.Dispose();
                CameoPageUpDn.Dispose();
                CameoPageUpInv.Dispose();

                CameoPageDown.Dispose();
                CameoPageDownDn.Dispose();
                CameoPageDownInv.Dispose();
            }

            repairBtn = null;
            sellBtn = null;
            tlBtn = null;
            trBtn = null;
            radar = null;
            tabs[0] = null;
            tabs[1] = null;
            tabs[2] = null;
            tabs[3] = null;
            tabs = null;

            cameoPu = null;
            cameoPd = null;


            ACBBackground = null;
            ACBLeftCap = null;
            ACBRightCap = null;
            CreditsBackground = null;
            TopBackground = null;
            TopLeftButton = null;
            TopLeftButtonDown = null;
            TopRightButton = null;
            TopRightButtonDown = null;
            Tab01Image = null;
            Tab01ImageInv = null;
            Tab01ImageAct = null;
            Tab02Image = null;
            Tab02ImageInv = null;
            Tab02ImageAct = null;
            Tab03Image = null;
            Tab03ImageInv = null;
            Tab03ImageAct = null;
            Tab04Image = null;
            Tab04ImageInv = null;
            Tab04ImageAct = null;
            RepairButton = null;
            RepairButtonDown = null;
            SellButton = null;
            SellButtonDown = null;

            MiddleBackground = null;
            CameoBackground = null;
            BottomImage = null;
            CameoPageUdBg = null;

            CameoPageUp = null;
            CameoPageUpDn = null;
            CameoPageUpInv = null;

            CameoPageDown = null;
            CameoPageDownDn = null;
            CameoPageDownInv = null;
        }
    }
}
