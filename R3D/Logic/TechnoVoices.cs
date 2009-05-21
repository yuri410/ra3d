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
using R3D.ScriptEngine;
using R3D.Sound;

namespace R3D.Logic
{
    public class TechnoVoices : ScriptableObject, IConfigurable
    {
        EVAVocManager vocMgr;

        #region Properties
        public string VoiceAttack
        {
            get;
            protected set;
        }
        public string VoiceCapture
        {
            get;
            protected set;
        }
        public string VoiceComment
        {
            get;
            protected set;
        }
        public string VoiceCrashing
        {
            get;
            protected set;
        }
        public string VoiceDeploy
        {
            get;
            protected set;
        }
        public string VoiceDie
        {
            get;
            protected set;
        }
        public string VoiceEnter
        {
            get;
            protected set;
        }
        public string VoiceFalling
        {
            get;
            protected set;
        }
        public string VoiceFeedback
        {
            get;
            protected set;
        }
        public string VoiceHarvest
        {
            get;
            protected set;
        }
        public string VoiceMove
        {
            get;
            protected set;
        }
        public string VoicePrimaryEliteWeaponAttack
        {
            get;
            protected set;
        }
        public string VoicePrimaryWeaponAttack
        {
            get;
            protected set;
        }
        public string VoiceSecondaryWeaponAttack
        {
            get;
            protected set;
        }
        public string VoiceSecondaryEliteWeaponAttack
        {
            get;
            protected set;
        }
        public string VoiceSelect
        {
            get;
            protected set;
        }
        public string VoiceSelectDeactivated
        {
            get;
            protected set;
        }

        public string VoiceSinking
        {
            get;
            protected set;
        }
        public string VoiceSpecialAttack
        {
            get;
            protected set;
        }
        public string VoiceUndeploy
        {
            get;
            protected set;
        }
        #endregion

        #region Methods
        public void PlayVoiceAttack()
        {
            if (!string.IsNullOrEmpty(VoiceAttack))
            {
                vocMgr.PlayEvaVoice(VoiceAttack);
            }
        }
        public void PlayVoiceCapture()
        {
            if (!string.IsNullOrEmpty(VoiceCapture))
            {
                vocMgr.PlayEvaVoice(VoiceCapture);
            }
        }
        public void PlayVoiceComment()
        {
            if (!string.IsNullOrEmpty(VoiceComment))
            {
                vocMgr.PlayEvaVoice(VoiceComment);
            }
        }
        public void PlayVoiceCrashing()
        {
            if (!string.IsNullOrEmpty(VoiceCrashing))
            {
                vocMgr.PlayEvaVoice(VoiceCrashing);
            }
        }
        public void PlayVoiceDeploy()
        {
            if (!string.IsNullOrEmpty(VoiceDeploy))
            {
                vocMgr.PlayEvaVoice(VoiceDeploy);
            }
        }
        public void PlayVoiceDie()
        {
            if (!string.IsNullOrEmpty(VoiceDie))
            {
                vocMgr.PlayEvaVoice(VoiceDie);
            }
        }
        public void PlayVoiceEnter()
        {
            if (!string.IsNullOrEmpty(VoiceEnter))
            {
                vocMgr.PlayEvaVoice(VoiceEnter);
            }
        }
        public void PlayVoiceFalling()
        {
            if (!string.IsNullOrEmpty(VoiceFalling))
            {
                vocMgr.PlayEvaVoice(VoiceFalling);
            }
        }
        public void PlayVoiceFeedback()
        {
            if (!string.IsNullOrEmpty(VoiceFeedback))
            {
                vocMgr.PlayEvaVoice(VoiceFeedback);
            }
        }
        public void PlayVoiceHarvest()
        {
            if (!string.IsNullOrEmpty(VoiceHarvest))
            {
                vocMgr.PlayEvaVoice(VoiceHarvest);
            }
        }
        public void PlayVoiceMove()
        {
            if (!string.IsNullOrEmpty(VoiceMove))
            {
                vocMgr.PlayEvaVoice(VoiceMove);
            }
        }
        public void PlayVoicePrimaryEliteWeaponAttack()
        {
            if (!string.IsNullOrEmpty(VoicePrimaryEliteWeaponAttack))
            {
                vocMgr.PlayEvaVoice(VoicePrimaryEliteWeaponAttack);
            }
        }
        public void PlayVoicePrimaryWeaponAttack()
        {
            if (!string.IsNullOrEmpty(VoicePrimaryWeaponAttack))
            {
                vocMgr.PlayEvaVoice(VoicePrimaryWeaponAttack);
            }
        }
        public void PlayVoiceSecondaryWeaponAttack()
        {
            if (!string.IsNullOrEmpty(VoiceSecondaryWeaponAttack))
            {
                vocMgr.PlayEvaVoice(VoiceSecondaryWeaponAttack);
            }
        }
        public void PlayVoiceSecondaryEliteWeaponAttack()
        {
            if (!string.IsNullOrEmpty(VoiceSecondaryEliteWeaponAttack))
            {
                vocMgr.PlayEvaVoice(VoiceSecondaryEliteWeaponAttack);
            }
        }
        public void PlayVoiceSelect()
        {
            if (!string.IsNullOrEmpty(VoiceSelect))
            {
                vocMgr.PlayEvaVoice(VoiceSelect);
            }
        }
        public void PlayVoiceSelectDeactivated()
        {
            if (!string.IsNullOrEmpty(VoiceSelectDeactivated))
            {
                vocMgr.PlayEvaVoice(VoiceSelectDeactivated);
            }
        }
        public void PlayVoiceSinking()
        {
            if (!string.IsNullOrEmpty(VoiceSinking))
            {
                vocMgr.PlayEvaVoice(VoiceSinking);
            }
        }
        public void PlayVoiceSpecialAttack()
        {
            if (!string.IsNullOrEmpty(VoiceSpecialAttack))
            {
                vocMgr.PlayEvaVoice(VoiceSpecialAttack);
            }
        }
        public void PlayVoiceUndeploy()
        {
            if (!string.IsNullOrEmpty(VoiceUndeploy))
            {
                vocMgr.PlayEvaVoice(VoiceUndeploy);
            }
        }
        #endregion

        public TechnoVoices(EVAVocManager voc, TechnoType techType)
            : base(techType.TypeName +"")
        {
            vocMgr = voc;
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            VoiceAttack = sect.GetString("VoiceAttack", null);
            VoiceCapture = sect.GetString("VoiceCapture", null);
            VoiceComment = sect.GetString("VoiceComment", null);
            VoiceCrashing = sect.GetString("VoiceCrashing", null);
            VoiceDeploy = sect.GetString("VoiceDeploy", null);
            VoiceDie = sect.GetString("VoiceDie", null);
            VoiceEnter = sect.GetString("VoiceEnter", null);
            VoiceFalling = sect.GetString("VoiceFalling", null);
            VoiceFeedback = sect.GetString("VoiceFeedback", null);
            VoiceHarvest = sect.GetString("VoiceHarvest", null);
            VoiceMove = sect.GetString("VoiceMove", null);
            VoicePrimaryEliteWeaponAttack = sect.GetString("VoicePrimaryEliteWeaponAttack", null);
            VoicePrimaryWeaponAttack = sect.GetString("VoicePrimaryWeaponAttack", null);
            VoiceSecondaryWeaponAttack = sect.GetString("VoiceSecondaryWeaponAttack", null);
            VoiceSecondaryEliteWeaponAttack = sect.GetString("VoiceSecondaryEliteWeaponAttack", null);
            VoiceSelect = sect.GetString("VoiceSelect", null);
            VoiceSelectDeactivated = sect.GetString("VoiceSelectDeactivated", null);

            VoiceSinking = sect.GetString("VoiceSinking", null);
            VoiceSpecialAttack = sect.GetString("VoiceSpecialAttack", null);
            VoiceUndeploy = sect.GetString("VoiceUndeploy", null);
        }

        #endregion
    }
}
