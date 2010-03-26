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
using R3D.InputEngine;
using R3D.IO;
using R3D.UI.Controls;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;

namespace R3D.UI
{
    public class GameUI : IDisposable
    {
        public const int UIWidth = 800;
        public const int UIHeight = 600;

        Sprite sprite;
        LoadScreen loadScreen;
        DialogSettings dlgSets;

        Device dev;

        Game game;

        bool disposed;


        FpsCounter fpsCounter;
        Label debugInfoLabel;

        public ConfigModel.Configuration UIConfig
        {
            get;
            private set;
        }
        //BattleUI battleUI;

        /// <summary>
        ///  Gets the InputEngine instance created by this GameUI
        /// </summary>
        public Input InputEngine
        {
            get;
            private set;
        }

        public DialogSettings DialogSettings
        {
            get { return dlgSets; }
        }
        public Menu Menu
        {
            get;
            private set;
        }
        public BattleUI BattleUI
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the current loading screen
        /// </summary>
        public IBattleLoadScreen BattleLoadScrn
        {
            get;
            set;
        }

        public Device Device
        {
            get { return dev; }
        }

        /// <summary>
        ///  Get or set the current dialog
        /// </summary>
        /// <remarks>Dialog can be shown in any game state</remarks>
        public Dialog CurrentDialog
        {
            get;
            set;
        }


        public GameUI(Game game)
        {
            //Game g = Game.Instance;
            this.game = game;
            this.dev = game.Device;
            sprite = new Sprite(dev);

            game.KeyPress += InvokeKeyPressed;

            InputEngine = new Input(game);

            InputEngine.MouseDown += this.InvokeMouseDown;
            InputEngine.MouseMove += this.InvokeMouseMove;
            InputEngine.MouseUp += this.InvokeMouseUp;
            InputEngine.MouseWheel += this.InvokeMouseWheel;
            InputEngine.KeyStateChanged += this.InvokeKeyStateChanged;

            UIConfig = ConfigurationManager.Instance.CreateInstance(FileSystem.Instance.Locate(FileSystem.UI_Ini, FileSystem.GameResLR));

            loadScreen = new LoadScreen(this, UIConfig);

            Menu = new Menu(this);
        }

        public void Load()
        {
            ControlFactory fac = new ButtonFactory();
            ControlManager.Instance.RegisterControlType(fac.TypeName, fac);
            fac = new LabelFactory();
            ControlManager.Instance.RegisterControlType(fac.TypeName, fac);
            fac = new TrackbarFactory();
            ControlManager.Instance.RegisterControlType(fac.TypeName, fac);
            fac = new ImageControlFactory();
            ControlManager.Instance.RegisterControlType(fac.TypeName, fac);


            dlgSets = new DialogSettings(this);

            Menu.Load();


            if (Game.IsInDebugMode)
            {
                // debug only, so no script name
                debugInfoLabel = new Label(this);

                debugInfoLabel.Font = System.Windows.Forms.Control.DefaultFont;
                debugInfoLabel.AutoSize = false;
                debugInfoLabel.Size = new Size(250, 80);
                debugInfoLabel.X = 5;
                debugInfoLabel.Y = 5;

            }

            fpsCounter = new FpsCounter();
            fpsCounter.SetBegin();
        }


        public void OnPaint2D()
        {
            sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.DoNotSaveState);

            switch (game.GameState)
            {
                case GameState.Loading:
                    if (loadScreen != null) // anti-shit
                        loadScreen.PaintUI(sprite);
                    break;
                case GameState.MainMenu:
                    Menu.PaintUI(sprite);

                    if (loadScreen != null)
                    {
                        loadScreen.Dispose();
                        loadScreen = null;
                    }


                    break;
                case GameState.LoadingBattle:
                    if (BattleLoadScrn != null)
                    {
                        BattleLoadScrn.PaintUI(sprite);
                    }
                    break;
                case GameState.InGame:
                    if (BattleUI != null)
                    {
                        this.BattleUI.PaintUI(sprite);
                    }
                    break;
            }

            if (CurrentDialog != null)
                CurrentDialog.PaintControl();

            if (fpsCounter != null)
            {
                fpsCounter.SetEnd();
                // debug info label is drawn last
                if (Game.IsInDebugMode)
                {
                    float fpsLimit = game.FPSLimit;

                    if (float.IsInfinity(fpsLimit) || fpsLimit < 0)
                    {
                        debugInfoLabel.Text = "Debug Info:\n" + "FPS: " + fpsCounter.FPS.ToString();
                    }
                    else
                    {
                        debugInfoLabel.Text = "Debug Info:\n" + "FPS: " + fpsCounter.FPS.ToString() + " (Limited to " + fpsLimit.ToString() + ")";
                    }

                    debugInfoLabel.PaintControl();
                }
            }
            sprite.End();
        }

        public void OnPaint()
        {
            switch (game.GameState)
            {
                case GameState.InGame:
                    this.BattleUI.Paint();
                    break;
            }
        }


        public Sprite GetSprite
        {
            get { return sprite; }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (loadScreen != null)
                    loadScreen.Dispose();
                sprite.Dispose();//.DisposeHandler(null, EventArgs.Empty);
                sprite = null;
                Menu.Dispose();
                Menu = null;

                InputEngine.MouseDown -= this.InvokeMouseDown;
                InputEngine.MouseMove -= this.InvokeMouseMove;
                InputEngine.MouseUp -= this.InvokeMouseUp;
                InputEngine.MouseWheel -= this.InvokeMouseWheel;
                InputEngine.KeyStateChanged -= this.InvokeKeyStateChanged;

                InputEngine.Dispose();
                InputEngine = null;


                disposed = true;
                //GC.SuppressFinalize(this);
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }

        public void Update(float dt)
        {
            if (CurrentDialog == null)
            {
                switch (game.GameState)
                {
                    case GameState.Loading:
                        // no thing to do
                        break;
                    case GameState.MainMenu:
                        Menu.Update(dt);//.OnMouseUp(this, e);
                        break;
                    case GameState.LoadingBattle:
                        // nothing to do BattleLoadScrn
                        break;
                    case GameState.InGame:
                        this.BattleUI.Update(dt);
                        break;
                }
            }
            else
                CurrentDialog.Update(dt);//.OnMouseUp(this, e);

            if (InputEngine != null)
            {
                InputEngine.Update(dt);
            }
        }

        ~GameUI()
        {
            if (!disposed)
                Dispose();
        }

        public void InvokeMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CurrentDialog == null)
            {
                switch (game.GameState)
                {
                    case GameState.Loading:
                        // no thing to do
                        break;
                    case GameState.MainMenu:
                        Menu.InvokeMouseDown(e);
                        break;
                    case GameState.LoadingBattle:
                        // nothing to do BattleLoadScrn
                        break;
                    case GameState.InGame:
                        this.BattleUI.InvokeMouseDown(e);
                        break;
                }
            }
            else
            {
                CurrentDialog.InvokeMouseDown(e);
            }
        }
        public void InvokeMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CurrentDialog == null)
            {
                switch (game.GameState)
                {
                    case GameState.Loading:
                        // no thing to do
                        break;
                    case GameState.MainMenu:
                        Menu.InvokeMouseUp(e);
                        break;
                    case GameState.LoadingBattle:
                        // nothing to do BattleLoadScrn
                        break;
                    case GameState.InGame:
                        this.BattleUI.InvokeMouseUp(e);
                        break;
                }
            }
            else
            {
                CurrentDialog.InvokeMouseUp(e);
            }
        }
        public void InvokeMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CurrentDialog == null)
            {
                switch (game.GameState)
                {
                    case GameState.Loading:
                        // no thing to do
                        break;
                    case GameState.MainMenu:
                        Menu.InvokeMouseMove(e);
                        break;
                    case GameState.LoadingBattle:
                        // nothing to do BattleLoadScrn
                        break;
                    case GameState.InGame:
                        this.BattleUI.InvokeMouseMove(e);
                        break;
                }
            }
            else
            {
                CurrentDialog.InvokeMouseMove(e);
            }
        }
        public void InvokeMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CurrentDialog == null)
            {
                switch (game.GameState)
                {
                    case GameState.Loading:
                        // no thing to do
                        break;
                    case GameState.MainMenu:
                        Menu.InvokeMouseWheel(e);
                        break;
                    case GameState.LoadingBattle:
                        // nothing to do BattleLoadScrn
                        break;
                    case GameState.InGame:
                        this.BattleUI.InvokeMouseWheel(e);
                        break;
                }
            }
            else
            {
                CurrentDialog.InvokeMouseWheel(e);
            }
        }
        public void InvokeKeyStateChanged(object sender, KeyCollection pressed)
        {
            if (CurrentDialog == null)
            {
                switch (game.GameState)
                {
                    case GameState.Loading:
                        // no thing to do
                        break;
                    case GameState.MainMenu:
                        Menu.InvokeKeyStateChanged(pressed);
                        break;
                    case GameState.LoadingBattle:
                        // nothing to do BattleLoadScrn
                        break;
                    case GameState.InGame:
                        this.BattleUI.InvokeKeyStateChanged(pressed);
                        break;
                }
            }
            else
            {
                CurrentDialog.InvokeKeyStateChanged(pressed);
            }
        }
        public void InvokeKeyPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (CurrentDialog == null)
            {
                switch (game.GameState)
                {
                    case GameState.Loading:
                        // no thing to do
                        break;
                    case GameState.MainMenu:
                        Menu.InvokeKeyPressed(e);
                        break;
                    case GameState.LoadingBattle:
                        // nothing to do BattleLoadScrn
                        break;
                    case GameState.InGame:
                        this.BattleUI.InvokeKeyPressed(e);
                        break;
                }
            }
            else
            {
                CurrentDialog.InvokeKeyPressed(e);
            }
        }

    }
}
