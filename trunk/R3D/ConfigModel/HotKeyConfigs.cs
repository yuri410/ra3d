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
using System.Text;
using R3D.ConfigModel;
using R3D.IO;
using SlimDX.DirectInput;

namespace R3D.ConfigModel
{
    [Flags()]
    public enum HotKey
    {
        None = 0,
        Backspace = 8,
        /// <summary>
        ///   The Return/Enter key.
        /// </summary>
        Return = 13,
        //VK_0 thru VK_9 are the same as ASCII '0' thru '9' ($30 - $39)  
        //VK_A thru VK_Z are the same as ASCII 'A' thru 'Z' ($41 - $5A)  
        Escape=27,
        Space = 0x20,
        PageUp = 33,
        PageDown = 34,

        End = 35,
        Home = 36,
        LeftArrow = 37,
        UpArrow = 38,
        RightArrow = 39,
        DownArrow = 40,


        Insert = 45,
        Delete = 46,

        /// <summary>
        ///   The number 0.
        /// </summary>
        D0 = 0x30,
        /// <summary>
        ///   The number 1.
        /// </summary>
        D1 = 0x31,
        /// <summary>
        ///   The number 2.
        /// </summary>
        D2 = 0x32,
        /// <summary>
        ///   The number 3.
        /// </summary>
        D3 = 0x33,
        /// <summary>
        ///   The number 4.
        /// </summary>
        D4 = 0x34,
        /// <summary>
        ///   The number 5.
        /// </summary>
        D5 = 0x35,
        /// <summary>
        ///   The number 6.
        /// </summary>
        D6 = 0x36,
        /// <summary>
        ///   The number 7.
        /// </summary>
        D7 = 0x37,
        /// <summary>
        ///   The number 8.
        /// </summary>
        D8 = 0x38,
        /// <summary>
        ///   The number 9.
        /// </summary>
        D9 = 0x39,

        A = 0x41,
        B = 0x42,
        C = 0x43,
        D = 0x44,
        E = 0x45,
        F = 0x46,
        G = 0x47,
        H = 0x48,
        I = 0x49,
        J = 0x4A,
        K = 0x4B,
        L = 0x4C,
        M = 0x4D,
        N = 0x4E,
        O = 0x4F,
        P = 0x50,
        Q = 0x51,
        R = 0x52,
        S = 0x53,
        T = 0x54,
        U = 0x55,
        V = 0x56,
        W = 0x57,
        X = 0x58,
        Y = 0x59,
        Z = 0x5A,

        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Devide = 111,

        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 127,
        F17 = 128,
        F18 = 129,
        F19 = 130,
        F20 = 131,
        F21 = 132,
        F22 = 133,
        F23 = 134,
        F24 = 135,

        /// <summary>
        /// ;
        /// </summary>
        Semicolon = 186,
        /// <summary>
        /// =
        /// </summary>
        Equal = 187,
        /// <summary>
        /// ,
        /// </summary>
        Comma = 188,
        /// <summary>
        /// -
        /// </summary>
        Minus = 189,
        /// <summary>
        /// .
        /// </summary>
        Period = 190,
        /// <summary>
        /// /
        /// </summary>
        Slash = 191,
        /// <summary>
        /// `
        /// </summary>
        OEM3 = 192,
        /// <summary>
        /// [
        /// </summary>
        LeftBracket = 219,
        /// <summary>
        /// \
        /// </summary>
        Backslash = 220,
        /// <summary>
        /// ]
        /// </summary>
        RightBracket = 221,
        OEM7 = 222,
        OEM8 = 223,

        Shift = 256,
        Control = 512,
        Alt = 1024,
        Unknown = 1 << 31,
    }

    public class HotKeyConfigs
    {
        static readonly string HotKeySection = "HotKey";
        static HotKey[] key2HotKey;

        static HotKeyConfigs()
        {      
            key2HotKey = new HotKey[145];

            for (int i = 0; i < 145; i++)
            {
                key2HotKey[i] = HotKey.Unknown;
            }

            key2HotKey[(int)Key.D0] = HotKey.D0;
            key2HotKey[(int)Key.D1] = HotKey.D1;
            key2HotKey[(int)Key.D2] = HotKey.D2;
            key2HotKey[(int)Key.D3] = HotKey.D3;
            key2HotKey[(int)Key.D4] = HotKey.D4;
            key2HotKey[(int)Key.D5] = HotKey.D5;
            key2HotKey[(int)Key.D6] = HotKey.D6;
            key2HotKey[(int)Key.D7] = HotKey.D7;
            key2HotKey[(int)Key.D8] = HotKey.D8;
            key2HotKey[(int)Key.D9] = HotKey.D9;
            key2HotKey[(int)Key.A] = HotKey.A;
            key2HotKey[(int)Key.B] = HotKey.B;
            key2HotKey[(int)Key.C] = HotKey.C;
            key2HotKey[(int)Key.D] = HotKey.D;
            key2HotKey[(int)Key.E] = HotKey.R;
            key2HotKey[(int)Key.F] = HotKey.F;
            key2HotKey[(int)Key.G] = HotKey.G;
            key2HotKey[(int)Key.H] = HotKey.H;
            key2HotKey[(int)Key.I] = HotKey.I;
            key2HotKey[(int)Key.J] = HotKey.J;
            key2HotKey[(int)Key.K] = HotKey.K;
            key2HotKey[(int)Key.L] = HotKey.L;
            key2HotKey[(int)Key.M] = HotKey.M;
            key2HotKey[(int)Key.N] = HotKey.N;
            key2HotKey[(int)Key.O] = HotKey.O;
            key2HotKey[(int)Key.P] = HotKey.P;
            key2HotKey[(int)Key.Q] = HotKey.Q;
            key2HotKey[(int)Key.R] = HotKey.R;
            key2HotKey[(int)Key.S] = HotKey.S;
            key2HotKey[(int)Key.T] = HotKey.T;
            key2HotKey[(int)Key.U] = HotKey.U;
            key2HotKey[(int)Key.V] = HotKey.V;
            key2HotKey[(int)Key.W] = HotKey.W;
            key2HotKey[(int)Key.X] = HotKey.X;
            key2HotKey[(int)Key.Y] = HotKey.Y;
            key2HotKey[(int)Key.Z] = HotKey.Z;
            key2HotKey[(int)Key.Apostrophe] = HotKey.OEM7;


            key2HotKey[(int)Key.Backspace] = HotKey.Backspace;
            key2HotKey[(int)Key.Backslash] = HotKey.Backslash;
            key2HotKey[(int)Key.Comma] = HotKey.Comma;
            key2HotKey[(int)Key.Delete] = HotKey.Delete;

            key2HotKey[(int)Key.DownArrow] = HotKey.DownArrow;
            key2HotKey[(int)Key.End] = HotKey.End;
            key2HotKey[(int)Key.F1] = HotKey.F1;
            key2HotKey[(int)Key.F2] = HotKey.F2;
            key2HotKey[(int)Key.F3] = HotKey.F3;
            key2HotKey[(int)Key.F4] = HotKey.F4;
            key2HotKey[(int)Key.F5] = HotKey.F5;
            key2HotKey[(int)Key.F6] = HotKey.F6;
            key2HotKey[(int)Key.F7] = HotKey.F7;
            key2HotKey[(int)Key.F8] = HotKey.F8;
            key2HotKey[(int)Key.F9] = HotKey.F9;
            key2HotKey[(int)Key.F10] = HotKey.F10;
            key2HotKey[(int)Key.F11] = HotKey.F11;
            key2HotKey[(int)Key.F12] = HotKey.F12;
            key2HotKey[(int)Key.F13] = HotKey.F13;
            key2HotKey[(int)Key.F14] = HotKey.F14;
            key2HotKey[(int)Key.F15] = HotKey.F15;
            key2HotKey[(int)Key.Grave] = HotKey.OEM3;
            key2HotKey[(int)Key.Home] = HotKey.Home;
            key2HotKey[(int)Key.Insert] = HotKey.Insert;
            key2HotKey[(int)Key.LeftBracket] = HotKey.LeftBracket;
            key2HotKey[(int)Key.LeftControl] = HotKey.Control;
            key2HotKey[(int)Key.LeftArrow] = HotKey.LeftArrow;
            key2HotKey[(int)Key.LeftAlt] = HotKey.Alt;
            key2HotKey[(int)Key.LeftShift] = HotKey.Shift;
            key2HotKey[(int)Key.Minus] = HotKey.Minus;
            key2HotKey[(int)Key.NumberPad0] = HotKey.NumPad0;
            key2HotKey[(int)Key.NumberPad1] = HotKey.NumPad1;
            key2HotKey[(int)Key.NumberPad2] = HotKey.NumPad2;
            key2HotKey[(int)Key.NumberPad3] = HotKey.NumPad3;
            key2HotKey[(int)Key.NumberPad4] = HotKey.NumPad4;
            key2HotKey[(int)Key.NumberPad5] = HotKey.NumPad5;
            key2HotKey[(int)Key.NumberPad6] = HotKey.NumPad6;
            key2HotKey[(int)Key.NumberPad7] = HotKey.NumPad7;
            key2HotKey[(int)Key.NumberPad8] = HotKey.NumPad8;
            key2HotKey[(int)Key.NumberPad9] = HotKey.NumPad9;
            key2HotKey[(int)Key.NumberPadStar] = HotKey.Multiply;
            key2HotKey[(int)Key.NumberPadPlus] = HotKey.Add;
            key2HotKey[(int)Key.NumberPadMinus] = HotKey.Subtract;
            key2HotKey[(int)Key.NumberPadPeriod] = HotKey.Decimal;
            
            key2HotKey[(int)Key.PageDown] = HotKey.PageDown;
            key2HotKey[(int)Key.PageUp] = HotKey.PageUp;
            key2HotKey[(int)Key.Period] = HotKey.Period;
            key2HotKey[(int)Key.RightBracket] = HotKey.RightBracket;
            key2HotKey[(int)Key.RightControl] = HotKey.Control;
            key2HotKey[(int)Key.Return] = HotKey.Return;
            key2HotKey[(int)Key.RightArrow] = HotKey.RightArrow;
            key2HotKey[(int)Key.RightAlt] = HotKey.Alt;
            key2HotKey[(int)Key.RightShift] = HotKey.Shift;

            key2HotKey[(int)Key.Semicolon] = HotKey.Semicolon;
            key2HotKey[(int)Key.Slash] = HotKey.Slash;

            key2HotKey[(int)Key.Space] = HotKey.Space;
            key2HotKey[(int)Key.UpArrow] = HotKey.UpArrow;

        }

        public static HotKey Key2HotKey(Key key)
        {
            return key2HotKey[(int)key];
        }


        static HotKeyConfigs singleton;

        public static HotKeyConfigs Instance
        {
            get
            {
                return singleton;
            }
        }

        public static void Initialize()
        {
            singleton = new HotKeyConfigs();
        }




        #region keys

        public HotKey HealthNav
        {
            get;
            set;
        }

        public HotKey CursorCheat
        {
            get;
            set;
        }

        public HotKey CenterView
        {
            get;
            set;
        }

        public HotKey Options
        {
            get;
            set;
        }

        public HotKey CenterOnRadarEvent
        {
            get;
            set;
        }

        public HotKey TeamSelect_10
        {
            get;
            set;
        }
        
        public HotKey TeamSelect_1
        {
            get;
            set;
        }

        public HotKey TeamSelect_2
        {
            get;
            set;
        }
       
        public HotKey TeamSelect_3
        {
            get;
            set;
        }
       
        public HotKey TeamSelect_4
        {
            get;
            set;
        }

        public HotKey TeamSelect_5
        {
            get;
            set;
        }

        public HotKey TeamSelect_6
        {
            get;
            set;
        }

        public HotKey TeamSelect_7
        {
            get;
            set;
        }

        public HotKey TeamSelect_8
        {
            get;
            set;
        }

        public HotKey TeamSelect_9
        {
            get;
            set;
        }

        public HotKey ToggleAlliance
        {
            get;
            set;
        }

        public HotKey PlaceBeacon
        {
            get;
            set;
        }

        public HotKey AllToCheer
        {
            get;
            set;
        }

        public HotKey DeployObject
        {
            get;
            set;
        }

        public HotKey InfantryTab
        {
            get;
            set;
        }

        public HotKey Follow
        {
            get;
            set;
        }

        public HotKey GuardObject
        {
            get;
            set;
        }

        public HotKey CenterBase
        {
            get;
            set;
        }

        public HotKey ToggleRepair
        {
            get;
            set;
        }

        public HotKey ToggleSell
        {
            get;
            set;
        }

        public HotKey PreviousObject
        {
            get;
            set;
        }
       
        public HotKey NextObject
        {
            get;
            set;
        }
        
        public HotKey CombatantSelect
        {
            get;
            set;
        }
        
        public HotKey StructureTab
        {
            get;
            set;
        }
        
        public HotKey UnitTab
        {
            get;
            set;
        }

        public HotKey StopObject
        {
            get;
            set;
        }
        
        public HotKey TypeSelect
        {
            get;
            set;
        }
       
        public HotKey PageUser
        {
            get;
            set;
        }
        
        public HotKey DefenseTab
        {
            get;
            set;
        }
       
        public HotKey ScatterObject
        {
            get;
            set;
        }
     
        public HotKey VeterancyNav
        {
            get;
            set;
        }

        public HotKey PlanningMode
        {
            get;
            set;
        }

        public HotKey SidebarDown
        {
            get;
            set;
        }

        public HotKey SidebarUp
        {
            get;
            set;
        }

        public HotKey Delete
        {
            get;
            set;
        }

        public HotKey View1
        {
            get;
            set;
        }

        public HotKey View2
        {
            get;
            set;
        }

        public HotKey View3
        {
            get;
            set;
        }

        public HotKey View4
        {
            get;
            set;
        }

        public HotKey Taunt_1
        {
            get;
            set;
        }

        public HotKey Taunt_2
        {
            get;
            set;
        }

        public HotKey Taunt_3
        {
            get;
            set;
        }

        public HotKey Taunt_4
        {
            get;
            set;
        }

        public HotKey Taunt_5
        {
            get;
            set;
        }

        public HotKey Taunt_6
        {
            get;
            set;
        }

        public HotKey Taunt_7
        {
            get;
            set;
        }

        public HotKey Taunt_8
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_10
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_1
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_2
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_3
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_4
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_5
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_6
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_7
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_8
        {
            get;
            set;
        }

        public HotKey TeamAddSelect_9
        {
            get;
            set;
        }

        public HotKey ScreenCapture
        {
            get;
            set;
        }

        public HotKey TeamCreate_10
        {
            get;
            set;
        }

        public HotKey TeamCreate_1
        {
            get;
            set;
        }

        public HotKey TeamCreate_2
        {
            get;
            set;
        }

        public HotKey TeamCreate_3
        {
            get;
            set;
        }

        public HotKey TeamCreate_4
        {
            get;
            set;
        }

        public HotKey TeamCreate_5
        {
            get;
            set;
        }

        public HotKey TeamCreate_6
        {
            get;
            set;
        }

        public HotKey TeamCreate_7
        {
            get;
            set;
        }

        public HotKey TeamCreate_8
        {
            get;
            set;
        }

        public HotKey TeamCreate_9
        {
            get;
            set;
        }

        public HotKey SetView1
        {
            get;
            set;
        }

        public HotKey SetView2
        {
            get;
            set;
        }

        public HotKey SetView3
        {
            get;
            set;
        }

        public HotKey SetView4
        {
            get;
            set;
        }

        public HotKey TeamCenter_10
        {
            get;
            set;
        }

        public HotKey TeamCenter_1
        {
            get;
            set;
        }

        public HotKey TeamCenter_2
        {
            get;
            set;
        }

        public HotKey TeamCenter_3
        {
            get;
            set;
        }

        public HotKey TeamCenter_4
        {
            get;
            set;
        }

        public HotKey TeamCenter_5
        {
            get;
            set;
        }

        public HotKey TeamCenter_6
        {
            get;
            set;
        }

        public HotKey TeamCenter_7
        {
            get;
            set;
        }

        public HotKey TeamCenter_8
        {
            get;
            set;
        }

        public HotKey TeamCenter_9
        {
            get;
            set;
        }

        #endregion
        private HotKeyConfigs()
        {

            HealthNav = HotKey.None;
            CursorCheat = HotKey.None;


            CenterView = (HotKey)12;
            Options = (HotKey)27;
            CenterOnRadarEvent = (HotKey)32;
            TeamSelect_10 = (HotKey)48;
            TeamSelect_1 = (HotKey)49;
            TeamSelect_2 = (HotKey)50;
            TeamSelect_3 = (HotKey)51;
            TeamSelect_4 = (HotKey)52;
            TeamSelect_5 = (HotKey)53;
            TeamSelect_6 = (HotKey)54;
            TeamSelect_7 = (HotKey)55;
            TeamSelect_8 = (HotKey)56;
            TeamSelect_9 = (HotKey)57;
            ToggleAlliance = (HotKey)65;
            PlaceBeacon = (HotKey)66;
            AllToCheer = (HotKey)67;
            DeployObject = (HotKey)68;
            InfantryTab = (HotKey)69;
            Follow = (HotKey)70;
            GuardObject = (HotKey)71;
            CenterBase = (HotKey)72;
            ToggleRepair = (HotKey)75;
            ToggleSell = (HotKey)76;
            PreviousObject = (HotKey)77;
            NextObject = (HotKey)78;
            CombatantSelect = (HotKey)80;
            StructureTab = (HotKey)81;
            UnitTab = (HotKey)82;
            StopObject = (HotKey)83;
            TypeSelect = (HotKey)84;
            PageUser = (HotKey)85;
            DefenseTab = (HotKey)87;
            ScatterObject = (HotKey)88;
            VeterancyNav = (HotKey)89;
            PlanningMode = (HotKey)90;
            SidebarDown = (HotKey)98;
            SidebarUp = (HotKey)104;
            Delete = (HotKey)110;
            View1 = (HotKey)112;
            View2 = (HotKey)113;
            View3 = (HotKey)114;
            View4 = (HotKey)115;
            Taunt_1 = (HotKey)116;
            Taunt_2 = (HotKey)117;
            Taunt_3 = (HotKey)118;
            Taunt_4 = (HotKey)119;
            Taunt_5 = (HotKey)120;
            Taunt_6 = (HotKey)121;
            Taunt_7 = (HotKey)122;
            Taunt_8 = (HotKey)123;
            TeamAddSelect_10 = (HotKey)304;
            TeamAddSelect_1 = (HotKey)305;
            TeamAddSelect_2 = (HotKey)306;
            TeamAddSelect_3 = (HotKey)307;
            TeamAddSelect_4 = (HotKey)308;
            TeamAddSelect_5 = (HotKey)309;
            TeamAddSelect_6 = (HotKey)310;
            TeamAddSelect_7 = (HotKey)311;
            TeamAddSelect_8 = (HotKey)312;
            TeamAddSelect_9 = (HotKey)313;
            ScreenCapture = (HotKey)339;
            TeamCreate_10 = (HotKey)560;
            TeamCreate_1 = (HotKey)561;
            TeamCreate_2 = (HotKey)562;
            TeamCreate_3 = (HotKey)563;
            TeamCreate_4 = (HotKey)564;
            TeamCreate_5 = (HotKey)565;
            TeamCreate_6 = (HotKey)566;
            TeamCreate_7 = (HotKey)567;
            TeamCreate_8 = (HotKey)568;
            TeamCreate_9 = (HotKey)569;
            SetView1 = (HotKey)624;
            SetView2 = (HotKey)625;
            SetView3 = (HotKey)626;
            SetView4 = (HotKey)627;
            TeamCenter_10 = (HotKey)1072;
            TeamCenter_1 = (HotKey)1073;
            TeamCenter_2 = (HotKey)1074;
            TeamCenter_3 = (HotKey)1075;
            TeamCenter_4 = (HotKey)1076;
            TeamCenter_5 = (HotKey)1077;
            TeamCenter_6 = (HotKey)1078;
            TeamCenter_7 = (HotKey)1079;
            TeamCenter_8 = (HotKey)1080;
            TeamCenter_9 = (HotKey)1081;
        }

        string configPath;

        public void Load()
        {
            FileLocation fl = FileSystem.Instance.TryLocate(FileSystem.Keyboard_Ini, FileLocateRule.Root);
            if (fl != null)
            {
                configPath = fl.Path;
                Configuration config = ConfigurationManager.Instance.CreateInstance(fl);
                ConfigurationSection sect = config[HotKeySection];

                HealthNav = (HotKey)sect.GetInt("HealthNav", (int)HealthNav);
                CursorCheat = (HotKey)sect.GetInt("CursorCheat", (int)CursorCheat);

                CenterView = (HotKey)sect.GetInt("CenterView", (int)CenterView);
                Options = (HotKey)sect.GetInt("Options", (int)Options);
                CenterOnRadarEvent = (HotKey)sect.GetInt("CenterOnRadarEvent", (int)CenterOnRadarEvent);
                TeamSelect_10 = (HotKey)sect.GetInt("TeamSelect_10", (int)TeamSelect_10);
                TeamSelect_1 = (HotKey)sect.GetInt("TeamSelect_1", (int)TeamSelect_1);
                TeamSelect_2 = (HotKey)sect.GetInt("TeamSelect_2", (int)TeamSelect_2);
                TeamSelect_3 = (HotKey)sect.GetInt("TeamSelect_3", (int)TeamSelect_3);
                TeamSelect_4 = (HotKey)sect.GetInt("TeamSelect_4", (int)TeamSelect_4);
                TeamSelect_5 = (HotKey)sect.GetInt("TeamSelect_5", (int)TeamSelect_5);
                TeamSelect_6 = (HotKey)sect.GetInt("TeamSelect_6", (int)TeamSelect_6);
                TeamSelect_7 = (HotKey)sect.GetInt("TeamSelect_7", (int)TeamSelect_7);
                TeamSelect_8 = (HotKey)sect.GetInt("TeamSelect_8", (int)TeamSelect_8);
                TeamSelect_9 = (HotKey)sect.GetInt("TeamSelect_9", (int)TeamSelect_9);
                ToggleAlliance = (HotKey)sect.GetInt("ToggleAlliance", (int)ToggleAlliance);
                PlaceBeacon = (HotKey)sect.GetInt("PlaceBeacon", (int)PlaceBeacon);
                AllToCheer = (HotKey)sect.GetInt("AllToCheer", (int)AllToCheer);
                DeployObject = (HotKey)sect.GetInt("DeployObject", (int)DeployObject);
                InfantryTab = (HotKey)sect.GetInt("InfantryTab", (int)InfantryTab);
                Follow = (HotKey)sect.GetInt("Follow", (int)Follow);
                GuardObject = (HotKey)sect.GetInt("GuardObject", (int)GuardObject);
                CenterBase = (HotKey)sect.GetInt("CenterBase", (int)CenterBase);
                ToggleRepair = (HotKey)sect.GetInt("ToggleRepair", (int)ToggleRepair);
                ToggleSell = (HotKey)sect.GetInt("ToggleSell", (int)ToggleSell);
                PreviousObject = (HotKey)sect.GetInt("PreviousObject", (int)PreviousObject);
                NextObject = (HotKey)sect.GetInt("NextObject", (int)NextObject);
                CombatantSelect = (HotKey)sect.GetInt("CombatantSelect", (int)CombatantSelect);
                StructureTab = (HotKey)sect.GetInt("StructureTab", (int)StructureTab);
                UnitTab = (HotKey)sect.GetInt("UnitTab", (int)UnitTab);
                StopObject = (HotKey)sect.GetInt("StopObject", (int)StopObject);
                TypeSelect = (HotKey)sect.GetInt("TypeSelect", (int)TypeSelect);
                PageUser = (HotKey)sect.GetInt("PageUser", (int)PageUser);
                DefenseTab = (HotKey)sect.GetInt("DefenseTab", (int)DefenseTab);
                ScatterObject = (HotKey)sect.GetInt("ScatterObject", (int)ScatterObject);
                VeterancyNav = (HotKey)sect.GetInt("VeterancyNav", (int)VeterancyNav);
                PlanningMode = (HotKey)sect.GetInt("PlanningMode", (int)PlanningMode);
                SidebarDown = (HotKey)sect.GetInt("SidebarDown", (int)SidebarDown);
                SidebarUp = (HotKey)sect.GetInt("SidebarUp", (int)SidebarUp);
                Delete = (HotKey)sect.GetInt("Delete", (int)Delete);
                View1 = (HotKey)sect.GetInt("View1", (int)View1);
                View2 = (HotKey)sect.GetInt("View2", (int)View2);
                View3 = (HotKey)sect.GetInt("View3", (int)View3);
                View4 = (HotKey)sect.GetInt("View4", (int)View4);
                Taunt_1 = (HotKey)sect.GetInt("Taunt_1", (int)Taunt_1);
                Taunt_2 = (HotKey)sect.GetInt("Taunt_2", (int)Taunt_2);
                Taunt_3 = (HotKey)sect.GetInt("Taunt_3", (int)Taunt_3);
                Taunt_4 = (HotKey)sect.GetInt("Taunt_4", (int)Taunt_4);
                Taunt_5 = (HotKey)sect.GetInt("Taunt_5", (int)Taunt_5);
                Taunt_6 = (HotKey)sect.GetInt("Taunt_6", (int)Taunt_6);
                Taunt_7 = (HotKey)sect.GetInt("Taunt_7", (int)Taunt_7);
                Taunt_8 = (HotKey)sect.GetInt("Taunt_8", (int)Taunt_8);
                TeamAddSelect_10 = (HotKey)sect.GetInt("TeamAddSelect_10", (int)TeamAddSelect_10);
                TeamAddSelect_1 = (HotKey)sect.GetInt("TeamAddSelect_1", (int)TeamAddSelect_1);
                TeamAddSelect_2 = (HotKey)sect.GetInt("TeamAddSelect_2", (int)TeamAddSelect_2);
                TeamAddSelect_3 = (HotKey)sect.GetInt("TeamAddSelect_3", (int)TeamAddSelect_3);
                TeamAddSelect_4 = (HotKey)sect.GetInt("TeamAddSelect_4", (int)TeamAddSelect_4);
                TeamAddSelect_5 = (HotKey)sect.GetInt("TeamAddSelect_5", (int)TeamAddSelect_5);
                TeamAddSelect_6 = (HotKey)sect.GetInt("TeamAddSelect_6", (int)TeamAddSelect_6);
                TeamAddSelect_7 = (HotKey)sect.GetInt("TeamAddSelect_7", (int)TeamAddSelect_7);
                TeamAddSelect_8 = (HotKey)sect.GetInt("TeamAddSelect_8", (int)TeamAddSelect_8);
                TeamAddSelect_9 = (HotKey)sect.GetInt("TeamAddSelect_9", (int)TeamAddSelect_9);
                ScreenCapture = (HotKey)sect.GetInt("ScreenCapture", (int)ScreenCapture);
                TeamCreate_10 = (HotKey)sect.GetInt("TeamCreate_10", (int)TeamCreate_10);
                TeamCreate_1 = (HotKey)sect.GetInt("TeamCreate_1", (int)TeamCreate_1);
                TeamCreate_2 = (HotKey)sect.GetInt("TeamCreate_2", (int)TeamCreate_2);
                TeamCreate_3 = (HotKey)sect.GetInt("TeamCreate_3", (int)TeamCreate_3);
                TeamCreate_4 = (HotKey)sect.GetInt("TeamCreate_4", (int)TeamCreate_4);
                TeamCreate_5 = (HotKey)sect.GetInt("TeamCreate_5", (int)TeamCreate_5);
                TeamCreate_6 = (HotKey)sect.GetInt("TeamCreate_6", (int)TeamCreate_6);
                TeamCreate_7 = (HotKey)sect.GetInt("TeamCreate_7", (int)TeamCreate_7);
                TeamCreate_8 = (HotKey)sect.GetInt("TeamCreate_8", (int)TeamCreate_8);
                TeamCreate_9 = (HotKey)sect.GetInt("TeamCreate_9", (int)TeamCreate_9);
                SetView1 = (HotKey)sect.GetInt("SetView1", (int)SetView1);
                SetView2 = (HotKey)sect.GetInt("SetView2", (int)SetView2);
                SetView3 = (HotKey)sect.GetInt("SetView3", (int)SetView3);
                SetView4 = (HotKey)sect.GetInt("SetView4", (int)SetView4);
                TeamCenter_10 = (HotKey)sect.GetInt("TeamCenter_10", (int)TeamCenter_10);
                TeamCenter_1 = (HotKey)sect.GetInt("TeamCenter_1", (int)TeamCenter_1);
                TeamCenter_2 = (HotKey)sect.GetInt("TeamCenter_2", (int)TeamCenter_2);
                TeamCenter_3 = (HotKey)sect.GetInt("TeamCenter_3", (int)TeamCenter_3);
                TeamCenter_4 = (HotKey)sect.GetInt("TeamCenter_4", (int)TeamCenter_4);
                TeamCenter_5 = (HotKey)sect.GetInt("TeamCenter_5", (int)TeamCenter_5);
                TeamCenter_6 = (HotKey)sect.GetInt("TeamCenter_6", (int)TeamCenter_6);
                TeamCenter_7 = (HotKey)sect.GetInt("TeamCenter_7", (int)TeamCenter_7);
                TeamCenter_8 = (HotKey)sect.GetInt("TeamCenter_8", (int)TeamCenter_8);
                TeamCenter_9 = (HotKey)sect.GetInt("TeamCenter_9", (int)TeamCenter_9);
            }
            else
            {
                configPath = FileSystem.Keyboard_Ini;
                GameConsole.Instance.Write("Keyboard config file " + FileSystem.Keyboard_Ini + " not found. Use default settings.", ConsoleMessageType.Warning);
            }
        }

        public void Save()
        {
            IniConfiguration configIni = new IniConfiguration(string.Empty);

            IniSection sect = new IniSection(HotKeySection);
            sect.Add("HealthNav", ((int)HealthNav).ToString());
            sect.Add("CursorCheat", ((int)CursorCheat).ToString());

            sect.Add("CenterView", ((int)CenterView).ToString());
            sect.Add("Options", ((int)Options).ToString());
            sect.Add("CenterOnRadarEvent", ((int)CenterOnRadarEvent).ToString());
            sect.Add("TeamSelect_10", ((int)TeamSelect_10).ToString());
            sect.Add("TeamSelect_1", ((int)TeamSelect_1).ToString());
            sect.Add("TeamSelect_2", ((int)TeamSelect_2).ToString());
            sect.Add("TeamSelect_3", ((int)TeamSelect_3).ToString());
            sect.Add("TeamSelect_4", ((int)TeamSelect_4).ToString());
            sect.Add("TeamSelect_5", ((int)TeamSelect_5).ToString());
            sect.Add("TeamSelect_6", ((int)TeamSelect_6).ToString());
            sect.Add("TeamSelect_7", ((int)TeamSelect_7).ToString());
            sect.Add("TeamSelect_8", ((int)TeamSelect_8).ToString());
            sect.Add("TeamSelect_9", ((int)TeamSelect_9).ToString());
            sect.Add("ToggleAlliance", ((int)ToggleAlliance).ToString());
            sect.Add("PlaceBeacon", ((int)PlaceBeacon).ToString());
            sect.Add("AllToCheer", ((int)AllToCheer).ToString());
            sect.Add("HealthNav", ((int)HealthNav).ToString());
            sect.Add("DeployObject", ((int)DeployObject).ToString());
            sect.Add("InfantryTab", ((int)InfantryTab).ToString());
            sect.Add("Follow", ((int)Follow).ToString());
            sect.Add("GuardObject", ((int)GuardObject).ToString());
            sect.Add("CenterBase", ((int)CenterBase).ToString());
            sect.Add("ToggleRepair", ((int)ToggleRepair).ToString());
            sect.Add("ToggleSell", ((int)ToggleSell).ToString());
            sect.Add("PreviousObject", ((int)PreviousObject).ToString());
            sect.Add("NextObject", ((int)NextObject).ToString());
            sect.Add("CombatantSelect", ((int)CombatantSelect).ToString());
            sect.Add("StructureTab", ((int)StructureTab).ToString());
            sect.Add("UnitTab", ((int)UnitTab).ToString());
            sect.Add("StopObject", ((int)StopObject).ToString());
            sect.Add("TypeSelect", ((int)TypeSelect).ToString());
            sect.Add("PageUser", ((int)PageUser).ToString());
            sect.Add("DefenseTab", ((int)DefenseTab).ToString());
            sect.Add("ScatterObject", ((int)ScatterObject).ToString());
            sect.Add("VeterancyNav", ((int)VeterancyNav).ToString());
            sect.Add("PlanningMode", ((int)PlanningMode).ToString());
            sect.Add("SidebarDown", ((int)SidebarDown).ToString());
            sect.Add("SidebarUp", ((int)SidebarUp).ToString());
            sect.Add("Delete", ((int)Delete).ToString());
            sect.Add("View1", ((int)View1).ToString());
            sect.Add("View2", ((int)View2).ToString());
            sect.Add("View3", ((int)View3).ToString());
            sect.Add("View4", ((int)View4).ToString());
            sect.Add("Taunt_1", ((int)Taunt_1).ToString());
            sect.Add("Taunt_2", ((int)Taunt_2).ToString());
            sect.Add("Taunt_3", ((int)Taunt_3).ToString());
            sect.Add("Taunt_4", ((int)Taunt_4).ToString());
            sect.Add("Taunt_5", ((int)Taunt_5).ToString());
            sect.Add("Taunt_6", ((int)Taunt_6).ToString());
            sect.Add("Taunt_7", ((int)Taunt_7).ToString());
            sect.Add("Taunt_8", ((int)Taunt_8).ToString());
            sect.Add("TeamAddSelect_10", ((int)TeamAddSelect_10).ToString());
            sect.Add("TeamAddSelect_1", ((int)TeamAddSelect_1).ToString());
            sect.Add("TeamAddSelect_2", ((int)TeamAddSelect_2).ToString());
            sect.Add("TeamAddSelect_3", ((int)TeamAddSelect_3).ToString());
            sect.Add("TeamAddSelect_4", ((int)TeamAddSelect_4).ToString());
            sect.Add("TeamAddSelect_5", ((int)TeamAddSelect_5).ToString());
            sect.Add("TeamAddSelect_6", ((int)TeamAddSelect_6).ToString());
            sect.Add("TeamAddSelect_7", ((int)TeamAddSelect_7).ToString());
            sect.Add("TeamAddSelect_8", ((int)TeamAddSelect_8).ToString());
            sect.Add("TeamAddSelect_9", ((int)TeamAddSelect_9).ToString());
            sect.Add("ScreenCapture", ((int)ScreenCapture).ToString());
            sect.Add("TeamCreate_10", ((int)TeamCreate_10).ToString());
            sect.Add("TeamCreate_1", ((int)TeamCreate_1).ToString());
            sect.Add("TeamCreate_2", ((int)TeamCreate_2).ToString());
            sect.Add("TeamCreate_3", ((int)TeamCreate_3).ToString());
            sect.Add("TeamCreate_4", ((int)TeamCreate_4).ToString());
            sect.Add("TeamCreate_5", ((int)TeamCreate_5).ToString());
            sect.Add("TeamCreate_6", ((int)TeamCreate_6).ToString());
            sect.Add("TeamCreate_7", ((int)TeamCreate_7).ToString());
            sect.Add("TeamCreate_8", ((int)TeamCreate_8).ToString());
            sect.Add("TeamCreate_9", ((int)TeamCreate_9).ToString());
            sect.Add("SetView1", ((int)SetView1).ToString());
            sect.Add("SetView2", ((int)SetView2).ToString());
            sect.Add("SetView3", ((int)SetView3).ToString());
            sect.Add("SetView4", ((int)SetView4).ToString());
            sect.Add("TeamCenter_10", ((int)TeamCenter_10).ToString());
            sect.Add("TeamCenter_1", ((int)TeamCenter_1).ToString());
            sect.Add("TeamCenter_2", ((int)TeamCenter_2).ToString());
            sect.Add("TeamCenter_3", ((int)TeamCenter_3).ToString());
            sect.Add("TeamCenter_4", ((int)TeamCenter_4).ToString());
            sect.Add("TeamCenter_5", ((int)TeamCenter_5).ToString());
            sect.Add("TeamCenter_6", ((int)TeamCenter_6).ToString());
            sect.Add("TeamCenter_7", ((int)TeamCenter_7).ToString());
            sect.Add("TeamCenter_8", ((int)TeamCenter_8).ToString());
            sect.Add("TeamCenter_9", ((int)TeamCenter_9).ToString());



            configIni.Add(sect.Name, sect);
            configIni.Save(configPath);
        }
    }
}
