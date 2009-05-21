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
using System.Windows.Forms;
using R3D.ConfigModel;
using R3D.Core;
using R3D.IO;
using R3D.Logic;
using R3D.MathLib;
using R3D.Media;
using R3D.Sound;
using R3D.UI.Controls;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;
using R3D.Collections;

namespace R3D.UI
{
    public class MouseAction : IConfigurable
    {
        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            Name = sect.Name;

            StartFrame = sect.GetInt("StartFrame");
            EndFrame = sect.GetInt("EndFrame", StartFrame);

            Point pt = sect.GetPoint("HotSpot", Point.Empty);

            X = pt.X;
            Y = pt.Y;
        }

        #endregion

        public string Name
        {
            get;
            protected set;
        }

        public int StartFrame
        {
            get;
            protected set;
        }

        public int EndFrame
        {
            get;
            protected set;
        }

        public int X
        {
            get;
            protected set;
        }
        public int Y
        {
            get;
            protected set;
        }
    }

    public enum BasicMouseAction
    {
        None,
        Move,
        Attack,
        Deploy,
        InvalidOp,
        //SingleSelectable
    }

    public class BattleUI : InterfaceManager
    {
        public enum ScrollingDirection
        {
            None,
            Left,
            Up,
            Right,
            Down,
            LeftUp,
            LeftDown,
            RightUp,
            RightDown
        }
        //struct SelectRectVertex
        //{
        //    public Vector4 pos;

        //    public int diffuse;

        //    public static VertexFormat Format
        //    {
        //        get { return VertexFormat.PositionRhw | VertexFormat.Diffuse; }
        //    }
        //}

        static readonly string SelectUnitMA = "SelectUnitMouseAct";

        Battle battle;

        RtsCamera camera;

        SideBar sideBar;

        HumanPlayer localPlayer;
        BattleField btfld;


        Point currentMousePosition;


        UITexAnim mouseCursors;

        Dictionary<string, MouseAction> mouseActions;
        //Dictionary<string, MouseAction> mouseStateAct;

        SoundManager evaVocMgr;
        FMOD.System sndSys;


        //VertexBuffer selRectVb;

        Rectangle battleViewport;

        public bool MousePressed
        {
            get;
            protected set;
        }

        Point mouseDownPoint;



        public bool HasSelectRect
        {
            get;
            protected set;
        }

        Rectangle selectRect;

        SelectedUnitCollection selectedUnits;
        bool selectOwnedUnit;

        bool combinedSelection;




        BasicMouseAction currentBMA;
        MouseAction currentMouseAction;
        MouseAction CurrentMouseAction
        {
            get { return currentMouseAction; }
            set
            {
                if (value != currentMouseAction)
                {
                    currentMouseAction = value;

                    if (value != null)
                    {
                        curMouseActionFrame = value.StartFrame;
                    }
                }
            }
        }


        int curMouseActionFrame;
        Texture currentMouseImage;
        Point curMouseImgOffs;


        Size uiSize;

        TerrainPicker picker;
        int mouseCellX;
        int mouseCellY;


        Line uiLine;

        R3D.UI.Controls.Label debugInfoLabel;


        SideType currentSide;

        House mainHouse;

        /// <summary>
        ///  鼠标指针下的物体
        /// </summary>
        SceneObject currentObject;


        Dictionary<House, int> ctrlHousesTable;

        #region Scrolling

        ScrollingDirection scDir;

        public ScrollingDirection ScrollDirection
        {
            get { return scDir; }
            protected set
            {
                if (value != scDir)
                {
                    scDir = value;
                    switch (value)
                    {
                        case ScrollingDirection.Up:
                            currentMouseImage = mouseCursors[ScrollUp];
                            curMouseImgOffs.X = -scrollUpHS.X;
                            curMouseImgOffs.Y = -scrollUpHS.Y;
                            break;
                        case ScrollingDirection.Down:
                            currentMouseImage = mouseCursors[ScrollDown];
                            curMouseImgOffs.X = -scrollDownHS.X;
                            curMouseImgOffs.Y = -scrollDownHS.Y;
                            break;
                        case ScrollingDirection.Left:
                            currentMouseImage = mouseCursors[ScrollLeft];
                            curMouseImgOffs.X = -scrollLeftHS.X;
                            curMouseImgOffs.Y = -scrollLeftHS.Y;
                            break;
                        case ScrollingDirection.Right:
                            currentMouseImage = mouseCursors[ScrollRight];
                            curMouseImgOffs.X = -scrollRightHS.X;
                            curMouseImgOffs.Y = -scrollRightHS.Y;
                            break;
                        case ScrollingDirection.LeftDown:
                            currentMouseImage = mouseCursors[ScrollLeftDown];
                            curMouseImgOffs.X = -scrollLeftDownHS.X;
                            curMouseImgOffs.Y = -scrollLeftDownHS.Y;
                            break;
                        case ScrollingDirection.LeftUp:
                            currentMouseImage = mouseCursors[ScrollLeftUp];
                            curMouseImgOffs.X = -scrollLeftUpHS.X;
                            curMouseImgOffs.Y = -scrollLeftUpHS.Y;
                            break;
                        case ScrollingDirection.RightUp:
                            currentMouseImage = mouseCursors[ScrollRightUp];
                            curMouseImgOffs.X = -scrollRightUpHS.X;
                            curMouseImgOffs.Y = -scrollRightUpHS.Y;
                            break;
                        case ScrollingDirection.RightDown:
                            currentMouseImage = mouseCursors[ScrollRightDown];
                            curMouseImgOffs.X = -scrollRightDownHS.X;
                            curMouseImgOffs.Y = -scrollRightDownHS.Y;
                            break;
                        case ScrollingDirection.None:
                            // do nothing
                            break;
                    }
                }
            }
        }
        public bool IsScrolling
        {
            get;
            protected set;
        }

        int ScrollUp { get; set; }
        int ScrollLeft { get; set; }
        int ScrollRight { get; set; }
        int ScrollDown { get; set; }
        int ScrollLeftUp { get; set; }
        int ScrollLeftDown { get; set; }
        int ScrollRightUp { get; set; }
        int ScrollRightDown { get; set; }

        int NoScrollUp { get; set; }
        int NoScrollLeft { get; set; }
        int NoScrollRight { get; set; }
        int NoScrollDown { get; set; }
        int NoScrollLeftUp { get; set; }
        int NoScrollLeftDown { get; set; }
        int NoScrollRightUp { get; set; }
        int NoScrollRightDown { get; set; }

        Point scrollUpHS;
        Point scrollLeftHS;
        Point scrollRightHS;
        Point scrollDownHS;
        Point scrollLeftUpHS;
        Point scrollLeftDownHS;
        Point scrollRightUpHS;
        Point scrollRightDownHS;

        Point noScrollUpHS;
        Point noScrollLeftHS;
        Point noScrollRightHS;
        Point noScrollDownHS;
        Point noScrollLeftUpHS;
        Point noScrollLeftDownHS;
        Point noScrollRightUpHS;
        Point noScrollRightDownHS;


        #endregion

        public void RegisterMouseAction(string state, MouseAction act)
        {
            mouseActions.Add(state, act);

        }
        public void UnregisterMouseAction(string state)
        {
            mouseActions.Remove(state);
        }

        public BattleUI(Battle battle, GameUI gameUI, FMOD.System snds)
            : base(gameUI)
        {
            this.battle = battle;

            sndSys = snds;
            camera = (RtsCamera)battle.Camera;

            localPlayer = battle.LocalPlayer;

            // the ui only shows the states of the the first house controlled by the local player. (pwr, cameos, etc)
            // BattleUI仅仅指示当前玩家 (localPlayer)第一个控制的作战方的状态            
            if (localPlayer.ControlledHouseCount > 0)
            {
                currentSide = localPlayer.GetControlledHouse(0).Country.Side;
                mainHouse = localPlayer.GetControlledHouse(0);
            }
            else
            {
                currentSide = battle.Settings.Sides[0];
            }

            sideBar = new SideBar(this, currentSide.SideUIName);

            sideBar.Size = Game.Instance.ClientSize;
            uiSize = Game.Instance.ClientSize;


            battleViewport = new Rectangle(0, 0, 632, 568);
            selectedUnits = new SelectedUnitCollection();// new FastList<Techno>(100);
            mouseActions = new Dictionary<string, MouseAction>(CaseInsensitiveStringComparer.Instance);
            //mouseStateAct = new Dictionary<string, MouseAction>(CaseInsensitiveStringComparer.Instance);



            if (Game.IsInDebugMode)
            {
                debugInfoLabel = new R3D.UI.Controls.Label(GameUI);

                debugInfoLabel.Font = System.Windows.Forms.Control.DefaultFont;
                debugInfoLabel.AutoSize = false;
                debugInfoLabel.Size = new Size(300, 400);
                debugInfoLabel.X = 5;
                debugInfoLabel.Y = 100;
            }

            uiLine = new Line(Device);
        }

        /// <summary>
        ///  this is calls when the battlefield loaded
        /// </summary>
        /// <param name="btfld"></param>
        public unsafe void InitializeBattleControl(BattleField btfld)
        {
            this.btfld = btfld;

            //selRectVb = new VertexBuffer(Device, sizeof(SelectRectVertex) * 5, Usage.WriteOnly | Usage.Dynamic, SelectRectVertex.Format, Pool.Default);


            ConfigurationSection sect = GameUI.UIConfig["Cursor"];

            mouseCursors = new UITexAnim(sect, string.Empty, string.Empty);

            ScrollUp = sect.GetInt("ScrollUp");
            ScrollLeft = sect.GetInt("ScrollLeft");
            ScrollRight = sect.GetInt("ScrollRight");
            ScrollDown = sect.GetInt("ScrollDown");
            ScrollLeftUp = sect.GetInt("ScrollLeftUp");
            ScrollLeftDown = sect.GetInt("ScrollLeftDown");
            ScrollRightUp = sect.GetInt("ScrollRightUp");
            ScrollRightDown = sect.GetInt("ScrollRightDown");

            NoScrollUp = sect.GetInt("NoScrollUp");
            NoScrollLeft = sect.GetInt("NoScrollLeft");
            NoScrollRight = sect.GetInt("NoScrollRight");
            NoScrollDown = sect.GetInt("NoScrollDown");
            NoScrollLeftUp = sect.GetInt("NoScrollLeftUp");
            NoScrollLeftDown = sect.GetInt("NoScrollLeftDown");
            NoScrollRightUp = sect.GetInt("NoScrollRightUp");
            NoScrollRightDown = sect.GetInt("NoScrollRightDown");


            scrollUpHS = sect.GetPoint("ScrollUpHotSpot", Point.Empty);
            scrollLeftHS = sect.GetPoint("ScrollLeftHotSpot", Point.Empty);
            scrollRightHS = sect.GetPoint("ScrollRightHotSpot", Point.Empty);
            scrollDownHS = sect.GetPoint("ScrollDownHotSpot", Point.Empty);
            scrollLeftUpHS = sect.GetPoint("ScrollLeftUpHotSpot", Point.Empty);
            scrollLeftDownHS = sect.GetPoint("ScrollLeftDownHotSpot", Point.Empty);
            scrollRightUpHS = sect.GetPoint("ScrollRightUpHotSpot", Point.Empty);
            scrollRightDownHS = sect.GetPoint("ScrollRightDownHotSpot", Point.Empty);

            noScrollUpHS = sect.GetPoint("NoScrollUpHotSpot", Point.Empty);
            noScrollLeftHS = sect.GetPoint("NoScrollLeftHotSpot", Point.Empty);
            noScrollRightHS = sect.GetPoint("NoScrollRightHotSpot", Point.Empty);
            noScrollDownHS = sect.GetPoint("NoScrollDownHotSpot", Point.Empty);
            noScrollLeftUpHS = sect.GetPoint("NoScrollLeftUpHotSpot", Point.Empty);
            noScrollLeftDownHS = sect.GetPoint("NoScrollLeftDownHotSpot", Point.Empty);
            noScrollRightUpHS = sect.GetPoint("NoScrollRightUpHotSpot", Point.Empty);
            noScrollRightDownHS = sect.GetPoint("NoScrollRightDownHotSpot", Point.Empty);

            currentMouseImage = mouseCursors[0];
            //Device.SetCursorProperties(0, 0, mouseCursors[0].GetSurfaceLevel(0));
            //FileLocation fl = FileSystem.Instance.Locate(FileSystem.ConquerMix + "mouse.sha", FileSystem.GameResLR);

            //ShpAnim shp = (ShpAnim)AnimManager.Instance.CreateInstance(fl);

            //FileLocation pfl = FileSystem.Instance.Locate(FileSystem.CacheMix + "", FileSystem.GameResLR);

            //shp.Palette;
            picker = new TerrainPicker(btfld.BattleTerrain, btfld.CellInfo);
            btfld.BattleTerrain.Picker = picker;


            //evaVocMgr["EVA_EstablishBattlefieldControl"].Play();


            localPlayer.ControlledHousesChanged += BuildCtrlHousesTable;
            BuildCtrlHousesTable();
        }

        void BuildCtrlHousesTable()
        {
            if (ctrlHousesTable == null)
            {
                ctrlHousesTable = new Dictionary<House, int>(localPlayer.ControlledHouseCount);
            }
            else
            {
                ctrlHousesTable.Clear();
            }
            for (int i = 0; i < localPlayer.ControlledHouseCount; i++)
            {
                ctrlHousesTable.Add(localPlayer.GetControlledHouse(i), 0);
            }
        }

        public override void InvokeKeyPressed(KeyPressEventArgs e)
        {

        }
        public override void InvokeKeyStateChanged(KeyCollection pressed)
        {
            combinedSelection = false;


            bool lctrl = false;
            bool lshift = false;
            bool lalt = false;
            bool rctrl = false;
            bool rshift = false;
            bool ralt = false;
            foreach (Key e in pressed)
            {
                switch (e)
                {
                    case Key.RightAlt:
                        ralt = true;
                        break;
                    case Key.LeftAlt:
                        lalt = true;
                        break;
                    case Key.LeftControl:
                        lctrl = true;
                        break;
                    case Key.RightControl:
                        rctrl = true;
                        break;
                    case Key.LeftShift:
                        lshift = true;
                        break;
                    case Key.RightShift:
                        rshift = true;
                        break;
                }
            }

            bool ctrl = lctrl || rctrl;
            bool shift = lshift || rshift;
            bool alt = lalt || ralt;

            combinedSelection = ctrl;

            if (!ctrl)
            {
                if (pressed.Contains(Key.W))
                {
                    camera.MoveFront();
                }
                if (pressed.Contains(Key.S))
                {
                    camera.MoveBack();
                }
                if (pressed.Contains(Key.A))
                {
                    camera.MoveLeft();
                }
                if (pressed.Contains(Key.D))
                {
                    camera.MoveRight();
                }
                if (pressed.Contains(Key.LeftArrow))
                {
                    camera.TurnLeft();
                }
                if (pressed.Contains(Key.RightArrow))
                {
                    camera.TurnRight();
                }
                //if (TestOnly.GetAsyncKeyState(VKeys.VK_SPACE))
                //{
                //    camera.MoveUp();
                //}
                //if (TestOnly.GetAsyncKeyState(VKeys.VK_CONTROL))
                //{
                //    camera.MoveDown();
                //}
                if (pressed.Contains(Key.UpArrow))
                {
                    camera.Height++;
                }
                if (pressed.Contains(Key.DownArrow))
                {
                    camera.Height--;
                }
            }
            sideBar.InvokeKeyStateChanged(pressed);
        }

        public override void InvokeMouseDown(MouseEventArgs e)
        {
            if (!Controls.Control.IsInBounds(e.X, e.Y, ref battleViewport))
            {
                sideBar.InvokeMouseDown(e);
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    MousePressed = true;
                    mouseDownPoint = new Point(e.X, e.Y);
                    selectRect.X = e.X;
                    selectRect.Y = e.Y;
                }
            }
        }

        public override void InvokeMouseMove(MouseEventArgs e)
        {
            if (MousePressed)
            {
                if (Controls.Control.IsInBounds(e.X, e.Y, ref battleViewport))
                {
                    HasSelectRect = e.X != mouseDownPoint.X && e.Y != mouseDownPoint.Y;
                    if (HasSelectRect)
                    {
                        if (e.X < mouseDownPoint.X || e.Y < mouseDownPoint.Y)
                        {
                            selectRect.X = e.X;
                            selectRect.Y = e.Y;
                            selectRect.Width = mouseDownPoint.X - e.X;
                            selectRect.Height = mouseDownPoint.Y - e.Y;
                        }
                        else
                        {
                            selectRect.X = mouseDownPoint.X;
                            selectRect.Y = mouseDownPoint.Y;
                            selectRect.Width = e.X - mouseDownPoint.X;
                            selectRect.Height = e.Y - mouseDownPoint.Y;
                        }
                    }
                }
                else
                {
                    HasSelectRect = false;
                }
            }
            else
            {
                HasSelectRect = false;

                // query battle field info
            }

            currentMousePosition = new Point(e.X, e.Y);
            sideBar.InvokeMouseMove(e);
        }


        void SelectUnit(Techno unit)
        {
            if (!combinedSelection)
            {
                selectedUnits.DeselectAll();
            }

            if (unit != null && !unit.IsSelected)
            {
                if (ctrlHousesTable.ContainsKey(unit.OwnerHouse))
                {
                    selectOwnedUnit = true;
                    selectedUnits.SelectUnit(unit);
                }
                else
                {
                    selectOwnedUnit = false;
                    selectedUnits.DeselectAll();
                    selectedUnits.SelectUnit(unit);
                }
            }


        }
        void SelectUnits()
        {
            if (!combinedSelection)
            {
                selectedUnits.DeselectAll();
            }

            Frustum frus = new Frustum();
            frus.view = battle.Camera.Frustum.view;


            float invWid = 1f / (float)uiSize.Width;
            float invHgt = 1f / (float)uiSize.Height;
            PointF center = new PointF(uiSize.Width * 0.5f, uiSize.Height * 0.5f);
            RectangleF selOffCenter = new RectangleF(selectRect.Left - center.X, selectRect.Top - center.Y, selectRect.Width, selectRect.Height);

            RectangleF ratioRect = new RectangleF(selOffCenter.Left * invWid, selOffCenter.Top * invHgt, selOffCenter.Width * invWid, selOffCenter.Height * invHgt);
            battle.Camera.GetSubareaProjection(ref ratioRect, out frus.proj);


            //camera.SetProjection(frus.proj);

            frus.Update();

            FastList<SceneObject> selObjs = new FastList<SceneObject>(100);
            btfld.SceneManager.FindObjects(selObjs, frus);

            selectOwnedUnit = true;
            for (int i = 0; i < selObjs.Count; i++)
            {
                Techno unit = selObjs.Elements[i] as Techno;
                if (unit != null && !unit.IsSelected)
                {
                    if (ctrlHousesTable.ContainsKey(unit.OwnerHouse))
                    {
                        selectedUnits.SelectUnit(unit);
                    }
                    else
                    {
                        selectOwnedUnit = false;
                        selectedUnits.DeselectAll();
                        break;
                    }
                }
            }
        }


        void MoveCommand()
        {
            //int mouseCellX;
            //int mouseCellY;
            //picker.GetResultCell(out mouseCellX, out mouseCellY);

            if (mouseCellX != -1 && mouseCellY != -1)
            {
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    selectedUnits.Elements[i].Move(mouseCellY, mouseCellX, 0);
                }
            }
        }

        public override void InvokeMouseUp(MouseEventArgs e)
        {
            MousePressed = false;
            if (Controls.Control.IsInBounds(e.X, e.Y, ref battleViewport))
            {

                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    // select units
                    if (HasSelectRect)
                    {
                        SelectUnits();
                        HasSelectRect = false;

                        currentBMA = BasicMouseAction.None;
                        CurrentMouseAction = null;
                    }
                    else
                    {

                        switch (currentBMA)
                        {
                            case BasicMouseAction.None:
                                // select unit 
                                Techno curTech = null;
                                if (currentObject != null)
                                {
                                    curTech = currentObject as Techno;
                                }

                                if (curTech != null)
                                {
                                    SelectUnit(curTech);
                                    currentBMA = BasicMouseAction.None;
                                    CurrentMouseAction = null;
                                }

                                break;
                            case BasicMouseAction.Move:
                                if (selectedUnits.Count > 0 && selectOwnedUnit)
                                {
                                    MoveCommand();
                                }
                                break;
                            case BasicMouseAction.InvalidOp:
                                break;
                            case BasicMouseAction.Deploy:
                                if (selectedUnits[0].CanDeploy)
                                    selectedUnits[0].Deploy();

                                break;
                            case BasicMouseAction.Attack:
                                break;
                            //case BasicMouseAction.SingleSelectable:

                            //    break;
                        }
                    }
                }
                else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                {
                    selectedUnits.DeselectAll();
                    currentBMA = BasicMouseAction.None;
                    CurrentMouseAction = null;
                }
            }

            sideBar.InvokeMouseUp(e);
        }

        public override void InvokeMouseWheel(MouseEventArgs e)
        {
            sideBar.InvokeMouseWheel(e);
        }


        void paintDebugInfo(Sprite spr)
        {
            // debug info should be those updated often. or the console will be a better place for them. 
            StringBuilder dbinf = new StringBuilder();

            dbinf.AppendLine("Graphics Engine State:");
            dbinf.AppendLine("  Frame Vertex Count: " + btfld.SceneManager.VertexCount.ToString());
            dbinf.AppendLine("  Frame Primitive Count: " + btfld.SceneManager.PrimitiveCount.ToString());
            dbinf.AppendLine("  Frame Batch Count: " + btfld.SceneManager.BatchCount.ToString());
            dbinf.AppendLine("  Rendered Object Count: " + btfld.SceneManager.RenderedObjectCount.ToString());

            dbinf.AppendLine("  Camera Pos: " + camera.Position.ToString());
            dbinf.AppendLine("  Atmosphere Sun Angle: " + MathEx.Radian2Angle(btfld.SceneManager.Atmosphere.SunAngle).ToString());

            dbinf.AppendLine("BattleField Info:");
            dbinf.AppendLine("  Techno Count: " + btfld.UnitsOnMap.Count.ToString());
            dbinf.AppendLine("  Terrain Object Count: " + btfld.TerrainObjects.Count.ToString());

            dbinf.AppendLine("Player Info:");
            dbinf.AppendLine("  Name: " + localPlayer.Name);

            int count = localPlayer.ControlledHouseCount;
            dbinf.AppendLine("  Controlled Houses Count: " + count.ToString());
            dbinf.Append("  Controlled Houses: ");
            for (int i = 0; i < count; i++)
            {
                dbinf.Append(localPlayer.GetControlledHouse(i).ToString());
                if (i < count - 1)
                {
                    dbinf.Append(", ");
                }
                else
                {
                    dbinf.AppendLine();
                }
            }

            if (count > 0)
            {
                dbinf.AppendLine("  Techno Count: " + localPlayer.GetControlledHouse(0).TechnoList.Count.ToString());
                dbinf.AppendLine("  Power: " + localPlayer.GetControlledHouse(0).Power.ToString());
                dbinf.AppendLine("  Power Drain: " + localPlayer.GetControlledHouse(0).PowerDrain.ToString());

            }
            debugInfoLabel.Text = dbinf.ToString();
            debugInfoLabel.PaintControl();
        }

        void DrawDefaultCursor(Sprite spr)
        {
            if (currentMouseImage != null)
            {
                int x = currentMousePosition.X - curMouseImgOffs.X;
                int y = currentMousePosition.Y - curMouseImgOffs.Y;// +curMouseImgOffs.Y;

                if (x < 0)
                    x = 0;

                if (y < 0)
                    y = 0;

                if (x > uiSize.Width - mouseCursors.Width)
                    x = uiSize.Width - mouseCursors.Width;
                if (y > uiSize.Height - mouseCursors.Height)
                    y = uiSize.Height - mouseCursors.Height;

                //x += curMouseImgOffs.X;
                //y += curMouseImgOffs.Y;

                spr.Transform = Matrix.Translation(x, y, 0);
                spr.Draw(currentMouseImage, -1);

            }
        }

        protected override void paintUI(Sprite spr)
        {
            sideBar.PaintControl();

            if (IsScrolling || sideBar.IsMouseOver)
            {
                if (currentMouseImage != null)
                {
                    DrawDefaultCursor(spr);
                }
            }
            else if (CurrentMouseAction != null)
            {
                int x = currentMousePosition.X - CurrentMouseAction.X;
                int y = currentMousePosition.Y - CurrentMouseAction.Y;// +curMouseImgOffs.Y;

                if (x < 0)
                    x = 0;

                if (y < 0)
                    y = 0;

                if (x > uiSize.Width - mouseCursors.Width)
                    x = uiSize.Width - mouseCursors.Width;
                if (y > uiSize.Height - mouseCursors.Height)
                    y = uiSize.Height - mouseCursors.Height;

                //x += curMouseImgOffs.X;
                //y += curMouseImgOffs.Y;

                spr.Transform = Matrix.Translation(x, y, 0);

                if (curMouseActionFrame > CurrentMouseAction.EndFrame)
                {
                    curMouseActionFrame = CurrentMouseAction.StartFrame;
                }
                spr.Draw(mouseCursors[curMouseActionFrame++], -1);
            }
            else
            {
                DrawDefaultCursor(spr);
            }

            if (Game.IsInDebugMode)
            {
                paintDebugInfo(spr);
            }
            //throw new NotImplementedException();
        }


        public unsafe void Paint()
        {

            if (HasSelectRect)
            {
                Vector2[] selBox = new Vector2[5]
                {
                    new Vector2 { X = selectRect.X, Y = selectRect.Y },
                    new Vector2 { X = selectRect.X + selectRect.Width, Y = selectRect.Y },
                    new Vector2 { X = selectRect.X + selectRect.Width, Y = selectRect.Y + selectRect.Height },
                    new Vector2 { X = selectRect.X, Y = selectRect.Y + selectRect.Height },
                    new Vector2 { X = selectRect.X, Y = selectRect.Y },
                };
                uiLine.Draw(selBox, Color.White);
                //SelectRectVertex* dst = (SelectRectVertex*)selRectVb.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                //dst[0].pos = new Vector4(selectRect.X, selectRect.Y, 0, 0);
                //dst[0].diffuse = -1;
                //dst[1].pos = new Vector4(selectRect.X + selectRect.Width, selectRect.Y, 0, 0);
                //dst[1].diffuse = -1;
                //dst[2].pos = new Vector4(selectRect.X + selectRect.Width, selectRect.Y + selectRect.Height, 0, 0);
                //dst[2].diffuse = -1;
                //dst[3].pos = new Vector4(selectRect.X, selectRect.Y + selectRect.Height, 0, 0);
                //dst[3].diffuse = -1;
                //dst[4].pos = new Vector4(selectRect.X, selectRect.Y, 0, 0);
                //dst[4].diffuse = -1;

                //selRectVb.Unlock();

                //Device.VertexShader = null;
                //Device.PixelShader = null;
                //Device.VertexDeclaration = null;//new VertexDeclaration(Device, D3DX.DeclaratorFromFVF(SelectRectVertex.Format));

                //Device.SetTexture(0, null);
                //Device.SetRenderState(RenderState.Lighting, false);
                ////Device.SetRenderState(RenderState.ColorVertex, true);
                //Device.VertexFormat = SelectRectVertex.Format;
                //Device.SetStreamSource(0, selRectVb, 0, sizeof(SelectRectVertex));
                //Device.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);
            }


            if (selectedUnits.Count > 0)
            {
                //Vector2[] selectionBracketPos = new Vector2[selectedUnits.Count];

                Viewport vp = Device.Viewport;
                Matrix i4 = Matrix.Identity;

                uiLine.Begin();

                Vector2[] line = new Vector2[3];
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    Vector3 res;

                    Vector3 tmp = selectedUnits[i].Position + new Vector3(0, selectedUnits[i].BoundingSphere.Radius, 0);

                    Vector3.Project(ref selectedUnits[i].Position, ref vp, ref camera.Frustum.proj, ref camera.Frustum.view, ref i4, out res);

                    Vector3 refPosPrj;
                    tmp = tmp + camera.Top * selectedUnits[i].BoundingSphere.Radius;
                    //Vector3 maxPrj;
                    //Vector3 max = selectedUnits[i].Position + new Vector3(selectedUnits[i].BoundingSphere.Radius);
                    Vector3.Project(ref tmp, ref vp, ref camera.Frustum.proj, ref camera.Frustum.view, ref i4, out refPosPrj);

                    float dist = MathEx.Distance(ref res, ref refPosPrj);

                    selectedUnits[i].RenderSelectionBox(uiLine, ref res, dist);

                    //int maxWidth;
                    //int maxHeight;

                    //selectionBracketPos[i].X = res.X;
                    //selectionBracketPos[i].Y = res.Y;

                    //line[0].X = res.X - 10;
                    //line[0].Y = res.Y - 7;

                    //line[1].X = res.X - 10;
                    //line[1].Y = res.Y - 10;

                    //line[2].X = res.X - 7;
                    //line[2].Y = res.Y - 10;

                    //uiLine.Draw(line, Color.White);

                    //line[0].X = res.X + 10;
                    //line[0].Y = res.Y - 7;

                    //line[1].X = res.X + 10;
                    //line[1].Y = res.Y - 10;

                    //line[2].X = res.X + 7;
                    //line[2].Y = res.Y - 10;

                    //uiLine.Draw(line, Color.White);



                    //line[0].X = res.X - 10;
                    //line[0].Y = res.Y + 7;

                    //line[1].X = res.X - 10;
                    //line[1].Y = res.Y + 10;

                    //line[2].X = res.X - 7;
                    //line[2].Y = res.Y + 10;

                    //uiLine.Draw(line, Color.White);

                    //line[0].X = res.X + 10;
                    //line[0].Y = res.Y + 7;

                    //line[1].X = res.X + 10;
                    //line[1].Y = res.Y + 10;

                    //line[2].X = res.X + 7;
                    //line[2].Y = res.Y + 10;

                    //uiLine.Draw(line, Color.White);
                }


                uiLine.End();
            }
            //if (picker.ResuleCell 
        }

        public override void Load()
        {
            sideBar.Load();

            evaVocMgr = new SoundManager((AudioConfigs)BasicConfigs.Instance[AudioConfigFacotry.AudioConfigName]);
            evaVocMgr.Creator = new EVAVoicesFactory(evaVocMgr, sndSys, currentSide.Index);
            evaVocMgr.Load();
        }
        public override void Unload()
        {

        }

        void CheckScroll()
        {
            if (currentMousePosition.X <= 3)
            {
                IsScrolling = true;
                if (currentMousePosition.Y <= 3)
                {
                    ScrollDirection = ScrollingDirection.LeftUp;
                    camera.MoveLeft();
                    camera.MoveFront();
                }
                else if (currentMousePosition.Y >= uiSize.Height - 4)
                {
                    ScrollDirection = ScrollingDirection.LeftDown;
                    camera.MoveLeft();
                    camera.MoveBack();
                }
                else
                {
                    ScrollDirection = ScrollingDirection.Left;
                    camera.MoveLeft();
                }
            }
            else if (currentMousePosition.X >= uiSize.Width - 4)
            {
                IsScrolling = true;
                if (currentMousePosition.Y <= 3)
                {
                    ScrollDirection = ScrollingDirection.RightUp;
                    camera.MoveRight();
                    camera.MoveFront();
                }
                else if (currentMousePosition.Y >= uiSize.Height - 4)
                {
                    ScrollDirection = ScrollingDirection.RightDown;
                    camera.MoveRight();
                    camera.MoveBack();
                }
                else
                {
                    ScrollDirection = ScrollingDirection.Right;
                    camera.MoveRight();
                }
            }
            else
            {
                if (currentMousePosition.Y <= 3)
                {
                    IsScrolling = true;
                    ScrollDirection = ScrollingDirection.Up;
                    camera.MoveFront();
                }
                else if (currentMousePosition.Y >= uiSize.Height - 4)
                {
                    IsScrolling = true;
                    ScrollDirection = ScrollingDirection.Down;
                    camera.MoveBack();
                }
            }
        }
        public override void Update(float dt)
        {
            sideBar.Update(dt);

            IsScrolling = false;
            ScrollDirection = ScrollingDirection.None;

            currentMouseImage = mouseCursors[0];
            curMouseImgOffs = Point.Empty;



            if (btfld != null)
            {
                picker.GetResultCell(out mouseCellY, out mouseCellX);

                currentObject = btfld.SceneManager.FindObject(picker.PickRay);
                Techno curTech = null;
                if (currentObject != null)
                {
                    curTech = currentObject as Techno;
                }


                if (mouseCellX != -1 && mouseCellY != -1)
                {
                    //btfld.CellInfo[mouseCellX][mouseCellY];
                    if (selectedUnits.Count > 0)
                    {
                        if (selectOwnedUnit)
                        {
                            if (curTech != null)
                            {
                                if (selectedUnits.IsSelected(curTech))
                                {
                                    if (curTech.CanDeploy)
                                    {
                                        CurrentMouseAction = mouseActions[Techno.DefDeployMA];
                                        currentBMA = BasicMouseAction.Deploy;
                                    }
                                    else if (curTech.IsDeployable)
                                    {
                                        CurrentMouseAction = mouseActions[Techno.DefNoDeployMA];
                                        currentBMA = BasicMouseAction.InvalidOp;
                                    }
                                    else
                                    {
                                        Locomotion loco = selectedUnits[0].Locomotor;
                                        if (loco != null)
                                        {
                                            CurrentMouseAction = mouseActions[selectedUnits[0].Locomotor.LocomotionMAInvName];
                                            currentBMA = BasicMouseAction.InvalidOp;
                                        }
                                        else
                                        {
                                            CurrentMouseAction = null;
                                            currentBMA = BasicMouseAction.None;
                                        }
                                    }
                                }
                                else
                                {
                                    if (selectedUnits[0].IsLegalTarget(curTech))
                                    {
                                        if (selectedUnits.Count > 1)
                                        {
                                            CurrentMouseAction = mouseActions[WeaponType.DefaultAttackOutMA];
                                            currentBMA = BasicMouseAction.Attack;
                                        }
                                        else
                                        {
                                            CurrentMouseAction = mouseActions[WeaponType.DefaultAttackInMA];
                                            currentBMA = BasicMouseAction.Attack;
                                        }
                                    }
                                    else
                                    {
                                        CurrentMouseAction = mouseActions[WeaponType.DefaultAttackInvMA];
                                        currentBMA = BasicMouseAction.InvalidOp;
                                    }
                                }
                            }
                            else // mouse is over other things
                            {
                                Locomotion loco = selectedUnits[0].Locomotor;
                                if (loco != null)
                                {
                                    if (currentObject != null)
                                    {
                                        if (currentObject is TerrainObject)
                                        {
                                            CurrentMouseAction = mouseActions[selectedUnits[0].Locomotor.LocomotionMAInvName];
                                            currentBMA = BasicMouseAction.InvalidOp;
                                        }
                                        else
                                        {
                                            CurrentMouseAction = mouseActions[selectedUnits[0].Locomotor.LocomotionMAName];
                                            currentBMA = BasicMouseAction.Move;
                                        }
                                    }
                                    else
                                    {
                                        CurrentMouseAction = mouseActions[selectedUnits[0].Locomotor.LocomotionMAName];
                                        currentBMA = BasicMouseAction.Move;
                                    }
                                }
                                else
                                {
                                    CurrentMouseAction = null;
                                    currentBMA = BasicMouseAction.None;
                                }
                            }
                        }
                        else  // selecting other's units
                        {
                            CurrentMouseAction = null;
                            currentBMA = BasicMouseAction.None;
                        }
                    }
                    else // not selecting anything
                    {

                        if (curTech != null)
                        {
                            if (curTech.Type.Selectable)
                            {
                                CurrentMouseAction = mouseActions[SelectUnitMA];
                                currentBMA = BasicMouseAction.None;
                                //currentBMA = BasicMouseAction.SingleSelectable;
                                //CurrentMouseAction = null;
                            }
                            else
                            {
                                currentBMA = BasicMouseAction.None;
                                CurrentMouseAction = null;
                            }
                        }
                        else
                        {
                            currentBMA = BasicMouseAction.None;
                            CurrentMouseAction = null;
                        }
                    }
                } 
                //else
                //{
                //    if (selectedUnits.Count > 0)
                //    {
                //        if (selectOwnedUnit)
                //        {
 
                //        }
                //    }
                //}

                picker.Reset();
                Vector3 pNear = new Vector3(currentMousePosition.X, currentMousePosition.Y, 0);
                Vector3 pFar = new Vector3(currentMousePosition.X, currentMousePosition.Y, 1);

                Viewport vp = Device.Viewport;
                Frustum frus = camera.Frustum;

                Vector3 begin;//= Vector3.Unproject(new Vector3(e.X, e.Y, 0), GraphicsDevice.Instance.Device.Viewport, projection, view, Matrix.Identity);
                Vector3 end;//= Vector3.Unproject(new Vector3(e.X, e.Y, 1), GraphicsDevice.Instance.Device.Viewport, projection, view, Matrix.Identity);

                Matrix i4 = Matrix.Identity;

                Vector3.Unproject(ref pNear, ref vp, ref frus.proj, ref frus.view, ref i4, out begin);
                Vector3.Unproject(ref pFar, ref vp, ref frus.proj, ref frus.view, ref i4, out end);

                Ray ray;
                ray.Position = begin;

                end.X -= begin.X;
                end.Y -= begin.Y;
                end.Z -= begin.Z;
                end.Normalize();

                ray.Direction = end;
                picker.SetPickRay(ref ray);
            }

            if (!Game.IsInDebugMode)
            {
                CheckScroll();
            }

        }


        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            sideBar.Dispose(disposing);

            evaVocMgr.Dispose();
            sideBar = null;
        }
    }
}
