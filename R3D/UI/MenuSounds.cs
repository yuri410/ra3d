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

using R3D.Sound;
using R3D.IO;
using R3D.ConfigModel;

namespace R3D.UI
{
    public class MenuSounds
    {
        SoundBase guiMainButtonSound;//=MenuClick
        //SoundEffectBase guiBuildSound;//=MenuClick
        SoundBase guiTabSound;//=MenuTab
        SoundBase guiOpenSound;//=MenuACBOpen
        SoundBase guiCloseSound;//=MenuACBClose
        SoundBase guiMoveOutSound;//=MenuSlideOut
        SoundBase guiMoveInSound;//=MenuSlideIn
        SoundBase guiComboOpenSound;//=MenuACBOpen
        SoundBase guiComboCloseSound;//=MenuACBClose
        SoundBase guiCheckboxSound;//=MenuClick

        public SoundBase GuiCheckboxSound
        {
            get { return guiCheckboxSound; }
            set { guiCheckboxSound = value; }
        }
        public SoundBase GuiComboCloseSound
        {
            get { return guiComboCloseSound; }
            set { guiComboCloseSound = value; }
        }
        public SoundBase GuiComboOpenSound
        {
            get { return guiComboOpenSound; }
            set { guiComboOpenSound = value; }
        }
        public SoundBase GuiMoveInSound
        {
            get { return guiMoveInSound; }
            set { guiMoveInSound = value; }
        }
        public SoundBase GuiMoveOutSound
        {
            get { return guiMoveOutSound; }
            set { guiMoveOutSound = value; }
        }
        public SoundBase GuiCloseSound
        {
            get { return guiCloseSound; }
            set { guiCloseSound = value; }
        }
        public SoundBase GuiMainButtonSound
        {
            get { return guiMainButtonSound; }
            set { guiMainButtonSound = value; }
        }
        public SoundBase GuiTabSound
        {
            get { return guiTabSound; }
            set { guiTabSound = value; }
        }
        public SoundBase GuiOpenSound
        {
            get { return guiOpenSound; }
            set { guiOpenSound = value; }
        }

        SoundBase GetSound(SoundManager sndm, ConfigurationSection sect, string key)
        {
            string name;
            if (sect.TryGetValue(key, out name))
            {
                return sndm[name];
            }
            GameConsole.Instance.Write(ResourceAssembly.Instance.CM_SoundNotFound("GUIMainButtonSound"), ConsoleMessageType.Exclamation);
            return DummySound.Instance;
        }
        public MenuSounds(SoundManager sndm)
        {
            //ResourceLocation fl = FileSystem.Instance.Locate(FileSystem.Rule_Ini, FileSystem.GameResLR);
            //IniConfiguration ini = new IniConfiguration(fl);

            ConfigurationSection sect = GameConfigs.Instance.Rules["AudioVisual"];

            guiMainButtonSound = GetSound(sndm, sect, "GUIMainButtonSound");
            guiTabSound = GetSound(sndm, sect, "GUITabSound");
            guiOpenSound = GetSound(sndm, sect, "GUIOpenSound");//[name];
            guiCloseSound = GetSound(sndm, sect, "GUICloseSound");//[name];
            guiMoveOutSound =GetSound( sndm,sect ,"GUIMoveOutSound");//[name];
            guiMoveInSound = GetSound(sndm, sect, "GUIMoveInSound");// sndm[name];
            guiComboOpenSound = GetSound(sndm, sect, "GUIComboOpenSound");//[name];
            guiComboCloseSound = GetSound(sndm, sect, "GUIComboCloseSound");//[name];
            guiCheckboxSound = GetSound(sndm, sect, "GUICheckboxSound");//[name];


        }
    }
}
