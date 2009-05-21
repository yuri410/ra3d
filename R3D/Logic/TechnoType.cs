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
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.ScriptEngine;
using R3D.UI;
using SlimDX.Direct3D9;

namespace R3D.Logic
{
    //public class TechnoArt : ScriptableObject, IConfigurable
    //{
    //    public TechnoArt(string name)
    //        : base(name + "Art")
    //    {

    //    }

    //    #region IConfigurable 成员

    //    public void Parse(ConfigurationSection sect)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}

    public enum TechnoCategory
    {
        Unknown,
        Infantry,
        Unit,
        Aircraft,
        Building
    }

    public enum FactoryType
    {
        None,
        InfantryType,
        UnitType,
        AircraftType,
        BuildingType
    }

    public class CameoInfo
    {
        public string[] FileNames
        {
            get;
            set;
        }
        public string[] AltFileNames
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
        public int PaletteTransparentIndex
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

    /// <summary>
    /// 部队的抽象工厂
    /// </summary>
    /// <remarks>
    /// 负责特定类型部队的创建，以及 部分逻辑处理
    /// </remarks>
    public abstract class TechnoType : EntityType
    {
        //VertexBuffer selectionBox;

        string uiName;

        protected TechnoType(BattleField btfld, string typeName)
            : base(btfld, typeName)
        {
            Owners = new List<HouseType>(15);
            RequiredHouses = new List<HouseType>();
            ForbiddenHouses = new List<HouseType>();

            Weapons = new List<WeaponType>();

            RelatedPrerequisites = new List<TechnoType>();
        }

        #region Properties
        public LocomotionDescription LocomotorDesc
        {
            get;
            protected set;
        }
        //public LocomotionFactory Locomotion
        //{
        //    get;
        //    protected set;
        //}
        public Texture Cameo
        {
            get;
            protected set;
        }
        public Texture AltCameo
        {
            get;
            protected set;
        }

        public int Cost
        {
            get;
            protected set;
        }
        public int Soylent
        {
            get;
            protected set;
        }
        public bool Selectable
        {
            get;
            protected set;
        }

        /// <summary>
        /// Specifies the amount of 'hitpoints' this kind of objects has. When it reaches zero, the object is destroyed.
        /// </summary>
        public int Strength
        {
            get;
            protected set;
        }

        public bool DetectDisguise
        {
            get;
            protected set;
        }

        public int Power
        {
            get;
            protected set;
        }
        public bool Powered
        {
            get;
            protected set;
        }

        public bool Naval
        {
            get;
            protected set;
        }

        public bool FactoryPlant
        {
            get;
            protected set;
        }
        /// <summary>
        ///  works then FactoryPlant = true
        /// </summary>
        public float DefensesCostBonus
        {
            get;
            protected set;
        }
        public float InfantryCostBonus
        {
            get;
            protected set;
        }
        public float UnitsCostBonus
        {
            get;
            protected set;
        }
        public float AircraftCostBonus
        {
            get;
            protected set;
        }
        public float BuildingsCostBonus
        {
            get;
            protected set;
        }

        public bool Immune
        {
            get;
            protected set;
        }
        public bool ImmuneToPsionics
        {
            get;
            protected set;
        }


        public int Points
        {
            get;
            protected set;
        }
        public float Sight
        {
            get;
            protected set;
        }
        public float Speed
        {
            get;
            protected set;
        }
        public float ROT
        {
            get;
            protected set;
        }

        public int TechLevel
        {
            get;
            protected set;
        }
        public float BuildTimeRatio
        {
            get;
            protected set;
        }
        public bool AllowedToStartInMultiplayer
        {
            get;
            protected set;
        }

        public int BuildLimit
        {
            get;
            protected set;
        }

        public string UIName
        {
            get { return uiName; }
            protected set { uiName = value; }
        }

        public string ArmorName
        {
            get;
            protected set;
        }
        public int ArmorId
        {
            get;
            protected set;
        }


        protected string[] OwnerNames
        {
            get;
            set;
        }
        protected string[] ForbiddenHouseNames
        {
            get;
            set;
        }

        protected string[] RequiredHouseNames
        {
            get;
            set;
        }

        public List<HouseType> Owners
        {
            get;
            protected set;
        }
        public List<HouseType> ForbiddenHouses
        {
            get;
            protected set;
        }
        public List<HouseType> RequiredHouses
        {
            get;
            protected set;
        }

        public List<WeaponType> Weapons
        {
            get;
            protected set;
        }

        protected string[] WeaponNames
        {
            get;
            set;
        }
        protected string PrimaryWeaponName
        {
            get;
            set;
        }
        protected string SecondaryWeaponName
        {
            get;
            set;
        }

        public int WeaponCount
        {
            get;
            protected set;
        }



        public WeaponType PrimaryWeapon
        {
            get;
            protected set;
        }
        public WeaponType SecondaryWeapon
        {
            get;
            protected set;
        }

        public bool WeaponsFactory
        {
            get;
            protected set;
        }

        public FactoryType Factory
        {
            get;
            protected set;
        }

        public PrerequisiteExpression Prerequisites
        {
            get;
            protected set;
        }
        string[] PrerequisiteName
        {
            get;
            set;
        }
        string PrerequisiteEx
        {
            get;
            set;
        }



        public bool Insignificant
        {
            get;
            protected set;
        }
        public bool TypeImmune
        {
            get;
            protected set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>in degree</remarks>
        public float PitchAngle
        {
            get;
            protected set;
        }

        public float RollAngle
        {
            get;
            protected set;
        }


        public int SizeLimit
        {
            get;
            protected set;
        }

        public int Size
        {
            get;
            protected set;
        }



        public int ProduceCashStartup
        {
            get;
            protected set;
        }
        public int ProduceCashAmount
        {
            get;
            protected set;
        }
        public float ProduceCashDelay
        {
            get;
            protected set;
        }

        public bool LegalTarget
        {
            get;
            protected set;
        }

        public bool DontScore
        {
            get;
            protected set;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <value>列表包含受到该部队影响建造的部队类型</value>
        public List<TechnoType> RelatedPrerequisites
        {
            get;
            protected set;
        }

        #endregion

        public abstract Techno CreateInstance(House owner);

        public void ParseCountries(Dictionary<string, HouseType> house)
        {
            for (int i = 0; i < OwnerNames.Length; i++)
            {
                Owners.Add(house[OwnerNames[i]]);
            }
            for (int i = 0; i < ForbiddenHouseNames.Length; i++)
            {
                ForbiddenHouses.Add(house[ForbiddenHouseNames[i]]);
            }
            for (int i = 0; i < RequiredHouseNames.Length; i++)
            {
                RequiredHouses.Add(house[RequiredHouseNames[i]]);
            }
        }

        public void ParseArmor(Dictionary<string, int> armors)
        {
            ArmorId = armors[ArmorName];
        }

        public virtual void ParseTechnoTypes(PrerequisiteExpressionCompiler compiler, PrerequisiteAliasInfo pinfo, Dictionary<string, TechnoType> tech)
        {
            if (!string.IsNullOrEmpty(PrerequisiteEx))
            {
                Prerequisites = new PrerequisiteExpression(compiler, PrerequisiteEx, pinfo, tech);
            }
            else if (PrerequisiteName.Length > 0)
            {
                Prerequisites = new PrerequisiteExpression(compiler, PrerequisiteName, pinfo, tech);
            }

            if (Prerequisites != null)
            {
                TechnoType[] related = Prerequisites.RelatedTechnoTypes;

                for (int i = 0; i < related.Length; i++)
                {
                    if (related[i] != null && !related[i].RelatedPrerequisites.Contains(this))
                    {
                        related[i].RelatedPrerequisites.Add(this);
                    }
                }
            }
        }

        public virtual void ParseBuildingTypes(Dictionary<string, BuildingType> blds) { }

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            uiName = ParseUIName(sect);// uiName = sect.GetUIString("UIName");
            Cost = sect.GetInt("Cost", 0);

            Soylent = sect.GetInt("Soylent", 0);
            Strength = sect.GetInt("Strength", 1);
            Sight = sect.GetInt("Sight", 1);

            Points = sect.GetInt("Points", 0);

            BuildLimit = sect.GetInt("BuildLimit", -1);
            BuildTimeRatio = sect.GetFloat("BuildTimeMultiplier", 1);


            DetectDisguise = sect.GetBool("DetectDisguise", false);

            TechLevel = sect.GetInt("TechLevel", 1);
            Selectable = sect.GetBool("Selectable", true);
            Immune = sect.GetBool("Immune", false);


            FactoryPlant = sect.GetBool("FactoryPlant", false);
            if (FactoryPlant)
            {
                InfantryCostBonus = sect.GetFloat("InfantryCostBonus", 1);
                UnitsCostBonus = sect.GetFloat("UnitsCostBonus", 1);
                AircraftCostBonus = sect.GetFloat("AircraftCostBonus", 1);
                BuildingsCostBonus = sect.GetFloat("BuildingsCostBonus", 1);
                DefensesCostBonus = sect.GetFloat("DefensesCostBonus", 1);
            }
            else
            {
                InfantryCostBonus = 1f;
                UnitsCostBonus = 1f;
                AircraftCostBonus = 1f;
                BuildingsCostBonus = 1f;
                DefensesCostBonus = 1f;
            }

            Speed = sect.GetFloat("Speed", 0);
            ROT = sect.GetFloat("ROT", 0);


            Power = sect.GetInt("Power", 0);
            Powered = sect.GetBool("Powered", false);

            AllowedToStartInMultiplayer = sect.GetBool("AllowedToStartInMultiplayer", true);

            OwnerNames = sect.GetStringArray("Owner", Utils.EmptyStringArray);
            OwnerNames = ConfigurationSection.CheckNone(OwnerNames);
            ForbiddenHouseNames = sect.GetStringArray("ForbiddenHouses", Utils.EmptyStringArray);
            ForbiddenHouseNames = ConfigurationSection.CheckNone(ForbiddenHouseNames);
            RequiredHouseNames = sect.GetStringArray("RequiredHouseName", Utils.EmptyStringArray);
            RequiredHouseNames = ConfigurationSection.CheckNone(RequiredHouseNames);

            ArmorName = sect.GetString("Armor", "none");


            string locomotor;
            sect.TryGetValue("Locomotor", out locomotor);

            if (locomotor != null)
            {
                if (LocomotionManager.IsGuidLocomotor(locomotor))
                    LocomotorDesc = Field.LocomotionMgr.CreateDescription(Field, new Guid(locomotor));
                else
                    LocomotorDesc = Field.LocomotionMgr.CreateDescription(Field, locomotor);
                LocomotorDesc.Parse(sect);
            }

            WeaponCount = 0;
            PrimaryWeaponName = sect.GetString("Primary", null);
            PrimaryWeaponName = ConfigurationSection.CheckNoneNull(PrimaryWeaponName);
            if (!string.IsNullOrEmpty(PrimaryWeaponName))
            {
                WeaponCount++;
            }


            SecondaryWeaponName = sect.GetString("Secondary", null);
            SecondaryWeaponName = ConfigurationSection.CheckNoneNull(SecondaryWeaponName);

            if (!string.IsNullOrEmpty(SecondaryWeaponName))
            {
                WeaponCount++;
            }

            WeaponCount = sect.GetInt("WeaponCount", WeaponCount);

            WeaponsFactory = sect.GetBool("WeaponsFactory", false);
            Factory = (FactoryType)Enum.Parse(typeof(FactoryType), sect.GetString("Factory", "None"));

            PrerequisiteName = sect.GetStringArray("Prerequisite", Utils.EmptyStringArray);
            PrerequisiteEx = sect.GetString("PrerequisiteEx", null);

            PrerequisiteName = ConfigurationSection.CheckNone(PrerequisiteName);
            PrerequisiteEx = ConfigurationSection.CheckNoneNull(PrerequisiteEx);


            Naval = sect.GetBool("Naval", false);
            //DeploysIntoName = sect.GetString("DeploysInto", null);
            //DeploysIntoName = ConfigurationSection.CheckNone(DeploysIntoName);

            Insignificant = sect.GetBool("Insignificant", false);
            TypeImmune = sect.GetBool("TypeImmune", false);

            PitchAngle = sect.GetFloat("PitchAngle", 0f);
            RollAngle = sect.GetFloat("PitchAngle", 0f);

            SizeLimit = sect.GetInt("SizeLimit", 1);

            Size = sect.GetInt("Size", 1);

            ProduceCashStartup = sect.GetInt("ProduceCashStartup", 0);
            ProduceCashAmount = sect.GetInt("ProduceCashAmount", 0);
            ProduceCashDelay = sect.GetInt("ProduceCashDelay", 0);

            LegalTarget = sect.GetBool("LegalTarget", true);

            DontScore = sect.GetBool("DontScore", false);
        }
        public void ParseWeapon(Dictionary<string, WeaponType> weaps)
        {
            if (PrimaryWeaponName != null)
            {
                PrimaryWeapon = weaps[PrimaryWeaponName];
            }
            if (SecondaryWeaponName != null)
            {
                SecondaryWeapon = weaps[SecondaryWeaponName];
            }

            if (WeaponCount > 2)
            {
                for (int i = 0; i < WeaponNames.Length; i++)
                {
                    Weapons.Add(weaps[WeaponNames[i]]);
                }
            }
            else
            {
                if (PrimaryWeapon != null)
                {
                    Weapons.Add(PrimaryWeapon);
                }
                if (SecondaryWeapon != null)
                {
                    Weapons.Add(SecondaryWeapon);
                }
            }
        }

        public override void LoadGraphics(Configuration config)
        {
            ConfigurationSection sect;
            if (config.TryGetValue(ImageName, out sect))
            {
                string fileName = sect.GetString("Image", ImageName) + ContentExt;

                FileLocation fl = FileSystem.Instance.TryLocate(fileName, FileSystem.GameCurrentResLR);

                if (fl != null)
                {
                    Model = ModelManager.Instance.CreateInstance(fl);
                }
                else
                {
                    Model = ModelManager.Instance.MissingModel;
                }

                //fileName = FileSystem.Cameo_Mix + sect.GetString("Cameo", "xxcameo.shp");

                //UIImageInformation imgInfo = new UIImageInformation();
                //imgInfo.FileNames = new string[] { fileName };
                //imgInfo.FilePalettes = new string[] { FileSystem.CacheMix + "cameo.pal" };

                //imgInfo.UsePalette = CaseInsensitiveStringComparer.Compare(".shp", "");
                CameoInfo camInfo = sect.GetCameo(FileSystem.GameResLR);
                Texture[] cameos = CameoManager.Instance.CreateInstance(camInfo);
                Cameo = cameos[0];
                AltCameo = cameos[1];

            }

        }

        public override string ToString()
        {
            string res = TypeName;
            if (!string.IsNullOrEmpty(UIName))
                res += "(" + UIName + ")";
            return res;
        }

        public virtual int GetCost(House owner)
        {
            return Cost;
        }

        public virtual TechnoCategory WhatAmI
        {
            get { return TechnoCategory.Unknown; }
        }

    }
}
