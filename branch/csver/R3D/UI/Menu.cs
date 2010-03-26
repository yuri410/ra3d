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
using R3D.Base;
using R3D.IO;
using R3D.Media;
using R3D.ScriptEngine;
using R3D.UI.Controls;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.UI
{
    public enum MenuType
    {
        Standard,
        Preview
    }
    public enum MenuSlideState
    {
        Loaded,
        SlidingIn,
        SlidingOut,
        Empty
    }

    public class Menu : InterfaceManager
    {
        bool isLoaded;

        MenuSounds mSounds;

        IMenuPage[] menus;
        Dictionary<string, IMenuPage> menuLookup;

        IMenuPage current;
        IMenuPage next;

        UITexAnim mapBoardAnim;
        UITexAnim stdButtonAnim;
        UITexAnim invButtonAnim;

        UITexAnim rightTopAnim;

        Texture rightTop;
        Texture rightTopPrev;

        Texture rightBottom;
        Texture btnBack;
        Texture hintBar;
        Texture backImage;

        Texture mapBoard;
        Size mapBoardSize;

        Label hintText;
        bool isHintShown;

        Label version;
        Label pageTitle;
        Point tlPos1, tlPos2;
        Size tlSize1, tlSize2;

        #region 菜单按钮
        /// <summary>
        /// 菜单按钮-标准
        /// </summary>
        int buttonStd;
        /// <summary>
        /// 菜单按钮-无功能
        /// </summary>
        int buttonInv;
        /// <summary>
        /// 菜单按钮-按下
        /// </summary>
        int buttonDown;
        /// <summary>
        /// 菜单按钮-悬浮
        /// </summary>
        int buttonOver;
        /// <summary>
        /// 菜单按钮-失效
        /// </summary>
        int buttonDisa;
        /// <summary>
        /// 菜单按钮-动画滑出
        /// </summary>
        int buttonOut;

        /// <summary>
        /// 菜单按钮文字x偏移量
        /// </summary>
        int textOffset;
        #endregion

        #region 按钮动画
        MenuSlideState slideState;
        int[] slideSteps;
        int slideCount;
        bool slidePassed;
        #endregion

        IMenuPageFactory menuFactory;

        public IMenuPageFactory MenuPageCreator
        {
            get { return menuFactory; }
            set { menuFactory = value; }
        }


        public Menu(GameUI gui)
            : base(gui)
        {
            SlideState = MenuSlideState.Loaded;
        }

        public MenuSlideState SlideState
        {
            get { return slideState; }
            set
            {
                slideState = value;
                slideSteps = new int[9];
                slideCount = 1;

                if (slideState == MenuSlideState.SlidingIn)
                {
                    slidePassed = false;
                    for (int i = 0; i < 9; i++)
                        if (current[i].IsValid)
                            slideSteps[i] = stdButtonAnim.StartFrame;
                        else
                            slideSteps[i] = invButtonAnim.StartFrame;

                    if (current.Type == MenuType.Preview)
                        slideSteps[0] = mapBoardAnim.StartFrame;
                }
                else if (slideState == MenuSlideState.SlidingOut)
                {
                    slidePassed = false;
                    for (int i = 0; i < 9; i++)
                        if (current[i].IsValid)
                            slideSteps[i] = stdButtonAnim.EndFrame + stdButtonAnim.Step;
                        else
                            slideSteps[i] = invButtonAnim.EndFrame + invButtonAnim.Step;

                    if (current.Type == MenuType.Preview)
                        slideSteps[0] = mapBoardAnim.EndFrame + mapBoardAnim.Step;
                }
            }
        }

        public MenuSounds MenuSounds
        {
            get { return mSounds; }
        }

        public string HintText
        {
            get { return hintText.Text; }
            set
            {
                hintText.Text = value;
                isHintShown = !string.IsNullOrEmpty(value);
            }
        }

        public int ButtonHeight
        {
            get { return stdButtonAnim.Height; }
        }
        public int ButtonWidth
        {
            get { return stdButtonAnim.Width; }
        }

        public Texture ButtonStdImg
        {
            get { return stdButtonAnim[buttonStd]; }
        }
        public Texture ButtonMouseDownImg
        {
            get { return stdButtonAnim[buttonDown]; }
        }
        public Texture ButtonMouseOverImg
        {
            get { return stdButtonAnim[buttonOver]; }
        }
        public Texture ButtonInvImg
        {
            get { return stdButtonAnim[buttonInv]; }
        }
        public Texture ButtonDisImg
        {
            get { return stdButtonAnim[buttonDisa]; }
        }

        public Texture MapBoard
        {
            get { return mapBoard; }
        }
        public Size MapBoardSize
        {
            get { return mapBoardSize; }
        }

        public void UserChangeMenu(IMenuPage m)
        {
            next = m;
            isHintShown = false;
            SlideState = MenuSlideState.SlidingOut;
        }

        public void UserChangeMenu(string menuTypeName)
        {
            IMenuPage mp;
            menuLookup.TryGetValue(menuTypeName, out mp);
            UserChangeMenu(mp);
        }

        void SetCurrentMenu(IMenuPage m)
        {
            if (m == null)
            {
                if (current != null)
                    current.CloseMenuPage();//.OnClose(this);
                current = null;
                return;
            }
            isHintShown = false;

            if (current != null)
                current.CloseMenuPage();

            m.Update(0);
            current = m;
            SlideState = MenuSlideState.SlidingIn;

            pageTitle.Text = m.UIName;
            if (m.Type == MenuType.Preview)
            {
                pageTitle.X = tlPos1.X;
                pageTitle.Y = tlPos1.Y;
                pageTitle.Size = tlSize1;
            }
            else
            {
                pageTitle.X = tlPos2.X;
                pageTitle.Y = tlPos2.Y;
                pageTitle.Size = tlSize2;
            }

        }

        public IMenuPage this[int idx]
        {
            get { return menus[idx]; }
        }
        public IMenuPage this[string name]
        {
            get
            {
                for (int i = 0; i < menus.Length; i++)
                    if (CaseInsensitiveStringComparer.Compare(menus[i].Name, name))
                        return menus[i];
                throw new KeyNotFoundException();
            }
        }


        public override void Load()
        {
            Game game = Game.Instance;
            if (mSounds == null)
                mSounds = new MenuSounds(game.SoundManager);

            ConfigModel.Configuration uiIni = GameUI.UIConfig;

            ConfigurationSection sect = uiIni["ButtonAnim"];
            stdButtonAnim = new UITexAnim(sect, FileSystem.NeutralMix, Game.Suffix);
            stdButtonAnim.StartFrame = int.Parse(sect["StdStart"]) - 1;
            stdButtonAnim.EndFrame = int.Parse(sect["StdEnd"]) - 1;

            invButtonAnim = new UITexAnim(stdButtonAnim.Data);
            invButtonAnim.StartFrame = int.Parse(sect["InvStart"]) - 1;
            invButtonAnim.EndFrame = int.Parse(sect["InvEnd"]) - 1;

            buttonInv = int.Parse(sect["Invalid"]) - 1;
            buttonStd = int.Parse(sect["Standard"]) - 1;
            buttonOver = int.Parse(sect["StdMouseOver"]) - 1;
            buttonDown = int.Parse(sect["StdMouseDown"]) - 1;
            buttonDisa = int.Parse(sect["Disabled"]) - 1;
            buttonOut = int.Parse(sect["Out"]) - 1;

            textOffset = int.Parse(sect["TextOffset"]);

            sect = uiIni["BoardAnim"];
            mapBoardAnim = new UITexAnim(sect, FileSystem.NeutralMix, Game.Suffix);
            mapBoardAnim.StartFrame = mapBoardAnim.Count - 1;
            mapBoardAnim.EndFrame = 0;
            mapBoardAnim.CurrentFrame = mapBoardAnim.StartFrame;

            sect = uiIni["RightTopAnim"];
            rightTopAnim = new UITexAnim(sect, FileSystem.NeutralMix, Game.Suffix);
            rightTopAnim.CurrentFrame = rightTopAnim.StartFrame;



            sect = uiIni["Menu.RightBottomImage"];
            UIImageInformation imgInfo = sect.GetImage(FileSystem.GameResLR);
            //ImageBase image = sect.GetImage(FileSystem.GameResLR);
            rightBottom = UITextureManager.Instance.CreateInstance(imgInfo);// image.GetTexture(dev, Usage.None, Pool.Managed); // new UIImage(sect, FileSystem.NeutralMix, Game.Suffix, FileSystem.GameResLR).ToTexture(dev);
            //image.Dispose();



            sect = uiIni["Menu.RightTopImage"];
            imgInfo = sect.GetImage(FileSystem.GameResLR);
            //image = sect.GetImage(FileSystem.GameResLR);
            rightTop = UITextureManager.Instance.CreateInstance(imgInfo); //image.GetTexture(dev, Usage.None, Pool.Managed); //  new UIImage(sect, FileSystem.NeutralMix, Game.Suffix, FileSystem.GameResLR).ToTexture(dev);
            //image.Dispose();


            sect = uiIni["Menu.RightTopImagePrev"];
            imgInfo = sect.GetImage(FileSystem.GameResLR);
            //image = sect.GetImage(FileSystem.GameResLR);
            //rightTopPrev = image.GetTexture(dev, Usage.None, Pool.Managed); // new UIImage(sect, FileSystem.NeutralMix, Game.Suffix, FileSystem.GameResLR).ToTexture(dev);
            rightTopPrev = UITextureManager.Instance.CreateInstance(imgInfo);
            //image.Dispose();


            sect = uiIni["Menu.BtnBack"];
            imgInfo = sect.GetImage(FileSystem.GameResLR);
            btnBack = UITextureManager.Instance.CreateInstance(imgInfo);
            //image = sect.GetImage(FileSystem.GameResLR);
            //btnBack = image.GetTexture(dev, Usage.None, Pool.Managed); // new UIImage(sect, FileSystem.NeutralMix, Game.Suffix, FileSystem.GameResLR).ToTexture(dev);
            //image.Dispose();


            sect = uiIni["Menu.Version"];
            version = Label.FromConfigSection(GameUI, sect);
            version.Text += " " + Game.GameVersion;
            if (version.Location == Point.Empty)
            {
                version.X = 645;
                version.Y = 570;
            }            

            sect = uiIni["Menu.Title"];
            pageTitle = Label.FromConfigSection(GameUI, sect);
            tlPos1 = sect.GetPoint("Position1");
            tlPos2 = sect.GetPoint("Position2");
            tlSize1 = sect.GetSize("Size1");
            tlSize2 = sect.GetSize("Size2");

            sect = uiIni["Menu.HintBar"];
            imgInfo = sect.GetImage(FileSystem.GameResLR);
            //image = sect.GetImage(FileSystem.GameResLR);
            //UIImage bmp = new UIImage(sect, FileSystem.NeutralMix, Game.Suffix, FileSystem.GameResLR);
            sect = uiIni["Menu.HintText"];
            hintText = Label.FromConfigSection(GameUI, sect);
            hintText.X = 5;
            hintText.Y = 570;

            hintBar = UITextureManager.Instance.CreateInstance(imgInfo);// image.GetTexture(dev, Usage.None, Pool.Managed); // bmp.ToTexture(dev, Usage.None, Pool.Managed);// Texture.FromBitmap(game.Device, bmp.Data, Usage.None, Pool.Managed);
            //image.Dispose();
            SurfaceDescription surfDesc = hintBar.GetLevelDescription(0);
            hintText.Size = new Size(surfDesc.Width, surfDesc.Height);


            sect = uiIni["Menu.MainBackImage"];
            imgInfo = sect.GetImage(FileSystem.GameResLR);
            //image = sect.GetImage(FileSystem.GameResLR);
            backImage = UITextureManager.Instance.CreateInstance(imgInfo);// image.GetTexture(dev, Usage.None, Pool.Managed); //  new UIImage(sect, FileSystem.NeutralMix, Game.Suffix, FileSystem.GameResLR).ToTexture(dev);
            //image.Dispose();


            sect = uiIni["Menu.Board"];
            imgInfo = sect.GetImage(FileSystem.GameResLR);
            //image = sect.GetImage(FileSystem.GameResLR);
            //bmp = new UIImage(sect, FileSystem.NeutralMix, Game.Suffix, FileSystem.GameResLR);
            //mapBoardSize = new Size(image.Width, image.Height);
            //mapBoard = image.GetTexture(dev, Usage.None, Pool.Managed); //bmp.ToTexture(dev, Usage.None, Pool.Managed);// // Texture.FromBitmap(dev, bmp.Data, Usage.None, Pool.Managed);
            mapBoard = UITextureManager.Instance.CreateInstance(imgInfo);
            surfDesc = mapBoard.GetLevelDescription(0);
            mapBoardSize = new Size(surfDesc.Width, surfDesc.Height);
            //image.Dispose();

            sect = uiIni["MenuList"];

            List<IMenuPage> meData = new List<IMenuPage>(sect.Count);
            menuLookup = new Dictionary<string, IMenuPage>(sect.Count);

            for (int i = 0; i < sect.Count; i++)
            {
                if (sect[i.ToString()] != string.Empty)
                {
                    //meData.Add(new MenuPage(gameUI, sect[i.ToString()]));
                    string menuPageTypeName = sect[i.ToString()];
                    IMenuPage menuPage = menuFactory.CreateInstance(GameUI, menuPageTypeName);
                    meData.Add(menuPage);
                    menuLookup.Add(menuPageTypeName, menuPage);
                }
            }

            meData.TrimExcess();
            menus = meData.ToArray();
            meData.Clear();

            //for (int i = 0; i < menus.Length; i++)
            //    menus[i].Update(0);

            isLoaded = true;
            SetCurrentMenu(menus[0]);
        }
        public override void Unload()
        {
            if (isLoaded)
            {
                menuLookup.Clear();

                hintText.Dispose();
                version.Dispose();
                pageTitle.Dispose();

                mapBoardAnim.Dispose();
                stdButtonAnim.Dispose();
                invButtonAnim.Dispose();
                rightTopAnim.Dispose();
                //for (int i = 0; i < menus.Length; i++)
                //{
                //    menus[i].Dispose();
                //    menus[i] = null;
                //}
                if (current != null)
                    current.Dispose();
                menus = null;
                current = null;
                next = null;

                //rightTop.Dispose();//.DisposeHandler(null, EventArgs.Empty);
                //rightTopPrev.Dispose();//.DisposeHandler(null, EventArgs.Empty);

                //rightBottom.Dispose();
                //btnBack.Dispose();
                //hintBar.Dispose();
                //backImage.Dispose();
                UITextureManager.Instance.DestoryInstance(rightTop); 
                UITextureManager.Instance.DestoryInstance(rightTopPrev); 
                UITextureManager.Instance.DestoryInstance(rightBottom); 
                UITextureManager.Instance.DestoryInstance(btnBack);
                UITextureManager.Instance.DestoryInstance(hintBar); 
                
                UITextureManager.Instance.DestoryInstance(backImage);
                UITextureManager.Instance.DestoryInstance(mapBoard);
                //mapBoard.Dispose();

                isLoaded = false;
            }

        }

        bool PaintItemIn(Sprite spr, int i, UITexAnim anim, int outFrame, bool freese)
        {
            bool passed = true;
            int idx = anim.StartFrame;
            int step = anim.Step;

            if (i < slideCount)
            {
                if (!freese)
                    slideSteps[i] += step;

                if (slideSteps[i] * step > anim.EndFrame * step)
                    slideSteps[i] = outFrame; // anim.EndFrame;
                else
                    passed = false;

                idx = slideSteps[i];
            }
            anim.Draw(spr, idx);
            return passed;
        }
        bool PaintItemOut(Sprite spr, int i, UITexAnim anim, int prevFrame, int outFrame, bool freese)
        {
            bool passed = true;
            int idx = prevFrame;
            int step = anim.Step;

            if (i < slideCount)
                if (slideSteps[i] * step > anim.EndFrame * step)
                {
                    idx = prevFrame;
                    passed = false;

                    if (!freese)
                        slideSteps[i] -= step;
                }
                else
                    if (slideSteps[i] * step < anim.StartFrame * step)
                        idx = outFrame;
                    else
                    {
                        passed = false;
                        idx = slideSteps[i];
                        if (!freese)
                            slideSteps[i] -= step;
                    }

            anim.Draw(spr, idx);
            return passed;
        }
        protected override void paintUI(Sprite spr)
        {
            bool freezeSlide = false;

            spr.Transform = Matrix.Identity;
            spr.Draw(backImage, -1);

            spr.Transform = Matrix.Translation(632, 0, 0);

            if (current.Type == MenuType.Standard)
                if (!rightTopAnim.EndReached)
                    if (!slidePassed)
                        spr.Draw(rightTopPrev, -1);
                    else
                    {
                        spr.Draw(rightTopPrev, -1);
                        rightTopAnim.OnPaint(spr);
                        freezeSlide = true;
                    }
                else
                    spr.Draw(rightTop, -1);
            else
                if (!rightTopAnim.StartReached)
                    if (!slidePassed)
                        spr.Draw(rightTop, -1);
                    else
                    {
                        spr.Draw(rightTop, -1);
                        rightTopAnim.OnPaint(spr);
                        freezeSlide = true;
                    }
                else
                    spr.Draw(rightTopPrev, -1);

            // 画按钮背景
            for (int i = 0; i < 9; i++)
            {
                spr.Transform = Matrix.Translation(632, 199 + i * stdButtonAnim.Height, 0);
                spr.Draw(btnBack, -1);
            }


            bool passed = true;
            switch (slideState)
            {
                case MenuSlideState.Loaded:
                    // 正常（用户可操作）状态（直接用UIButton.OnPaint）
                    current.PaintControl();
                    break;
                case MenuSlideState.Empty:
                    //无菜单状态
                    for (int i = 0; i < 9; i++)
                    {
                        spr.Transform = Matrix.Translation(644, 199 + i * stdButtonAnim.Height, 0);
                        spr.Draw(stdButtonAnim[buttonOut], -1);
                    }

                    break;
                case MenuSlideState.SlidingIn:

                    if (slideCount == 1)
                        mSounds.GuiMoveInSound.Play();

                    // 另外单独绘制动画
                    for (int i = 0; i < 9; i++)
                        if (i == 0 && current.Type == MenuType.Preview)
                        {
                            spr.Transform = Matrix.Translation(644, 199 - 42, 0);
                            passed &= PaintItemIn(spr, 0, mapBoardAnim, mapBoardAnim.EndFrame, freezeSlide);
                        }
                        else
                        {
                            spr.Transform = Matrix.Translation(644, 199 + i * stdButtonAnim.Height, 0);
                            if (current[i].IsValid)
                                passed &= PaintItemIn(spr, i, stdButtonAnim, buttonDisa, freezeSlide);
                            else
                                passed &= PaintItemIn(spr, i, invButtonAnim, buttonInv, freezeSlide);
                        }

                    slideCount++;
                    if (slideCount > 9)
                        slideCount = 9;

                    if (passed)
                    {
                        slidePassed = true;
                        SlideState = MenuSlideState.Loaded;
                    }

                    break;
                case MenuSlideState.SlidingOut:

                    if (slideCount == 1)
                        mSounds.GuiMoveOutSound.Play();

                    // 另外单独绘制动画
                    for (int i = 0; i < 9; i++)
                        if (i == 0 && current.Type == MenuType.Preview)
                        {
                            spr.Transform = Matrix.Translation(644, 199 - 42, 0);
                            passed &= PaintItemOut(spr, 0, mapBoardAnim, mapBoardAnim.EndFrame, mapBoardAnim.StartFrame, freezeSlide);
                        }
                        else
                        {
                            spr.Transform = Matrix.Translation(644, 199 + i * stdButtonAnim.Height, 0);
                            if (current[i].IsValid)
                                passed &= PaintItemOut(spr, i, stdButtonAnim, buttonDisa, buttonOut, freezeSlide);
                            else
                                passed &= PaintItemOut(spr, i, invButtonAnim, buttonInv, buttonOut, freezeSlide);
                        }

                    slideCount++;
                    if (slideCount > 9)
                        slideCount = 9;

                    if (passed)
                    {
                        slidePassed = true;
                        SlideState = MenuSlideState.Loaded;

                        // Out动画完成后切换至菜单next
                        if (next != null)
                        {
                            
                            // 根据next类型设置右上动画状态
                            if (next.Type == MenuType.Preview)
                            {
                                if (current.Type == next.Type)
                                    rightTopAnim.CurrentFrame = rightTopAnim.StartFrame;
                                else
                                {
                                    rightTopAnim.Direction = UITexAnim.PlayDirection.Backward;
                                    rightTopAnim.CurrentFrame = rightTopAnim.EndFrame;
                                }
                            }
                            else
                            {
                                if (current.Type == next.Type)
                                    rightTopAnim.CurrentFrame = rightTopAnim.EndFrame;
                                else
                                {
                                    rightTopAnim.Direction = UITexAnim.PlayDirection.Forward;
                                    rightTopAnim.CurrentFrame = rightTopAnim.StartFrame;
                                }
                            }
                            SetCurrentMenu(next);
                        }
                        else
                            SlideState = MenuSlideState.Empty;
                    }

                    break;
            }

            spr.Transform = Matrix.Translation(632, 577, 0);
            spr.Draw(rightBottom, -1);

            spr.Transform = Matrix.Translation(0, 568, 0);
            spr.Draw(hintBar, -1);

            if (isHintShown)
                hintText.PaintControl();//.OnPaint(spr);

            version.PaintControl();

            if (current != null && slidePassed && rightTopAnim.SEReached)
                pageTitle.PaintControl();//.OnPaint(spr);
        }

        public override void InvokeKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (current != null && slidePassed && rightTopAnim.SEReached)
                current.InvokeKeyPressed(e);
        }
        public override void InvokeMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (current != null && slidePassed && rightTopAnim.SEReached)
                current.InvokeMouseDown(e);
        }
        public override void InvokeMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (current != null && slidePassed && rightTopAnim.SEReached)
                current.InvokeMouseUp(e);
        }
        public override void InvokeMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            isHintShown = false;
            //hintText.Text = string.Empty;
            if (current != null && slidePassed && rightTopAnim.SEReached)
                current.InvokeMouseMove(e);
        }
        public override void InvokeMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            if (current != null && slidePassed && rightTopAnim.SEReached)
                current.InvokeMouseWheel(e);
        }

        public override void InvokeKeyStateChanged(SlimDX.DirectInput.KeyCollection pressed)
        {
            if (current != null && slidePassed && rightTopAnim.SEReached)
                current.InvokeKeyStateChanged(pressed);
        }
        public override void Update(float dt)
        {
            if (current != null && slidePassed && rightTopAnim.SEReached)
            {
                current.Update(dt);
            }
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Unload();
            }
        }




    }

    
    public class UIImageInformation
    {
        public string[] FileNames
        {
            get;
            set;
        }

        public string[] FilePalettes
        {
            get;
            set;
        }

        public bool UsePalette
        {
            get;
            set;
        }

        public int Frame
        {
            get;
            set;
        }

        public bool HasTransparentColor
        {
            get;
            set;
        }

        public Color TransparentColor
        {
            get;
            set;
        }

        public FileLocateRule LocateRule
        {
            get;
            set;
        }
    }
}
