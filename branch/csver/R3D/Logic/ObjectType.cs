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
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.ScriptEngine;
using SlimDX;

namespace R3D.Logic
{
    public abstract class ObjectType : ScriptableObject, IConfigurable
    {
        string typeName;

        protected ObjectType(string typeName)
            : base(typeName)
        {
            this.typeName = typeName;
        }

        public string TypeName
        {
            get { return typeName; }
        }

        #region IConfigurable 成员

        public abstract void Parse(ConfigurationSection sect);


        #endregion
    }
    public abstract class NamedObjectType : ObjectType
    {
        string name;

        protected NamedObjectType(string typeName)
            : base(typeName)
        {
        }

        public string ObjectName
        {
            get { return name; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            name = sect.GetString("Name", string.Empty);
        }

        protected static string ParseUIName(ConfigurationSection sect)
        {
            return sect.GetUIString("UIName", string.Empty);
        }

        public override string ToString()
        {
            string res = TypeName;
            if (!string.IsNullOrEmpty(name))
                res += "(" + name + ")";
            return res;
        }
    }

    /// <summary>
    /// 实体有图形
    /// </summary>
    public abstract class EntityType : NamedObjectType,IDisposable 
    {
        static protected readonly string ContentExt = ".mesh";

        protected EntityType(BattleField btfld, string typeName)
            : base(typeName)
        {
            Field = btfld;
        }

        public BattleField Field
        {
            get;
            private set;
        }
        public GameModel Model
        {
            get;
            set;
        }
        protected string ImageName
        {
            get;
            set;
        }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            ImageName = sect.GetString("Image", TypeName);
        }

        public virtual void LoadGraphics(ConfigModel.Configuration config)
        {
            string fileName = ImageName + ContentExt;

            FileLocation fl = FileSystem.Instance.TryLocate(fileName, FileSystem.GameCurrentResLR);

            if (fl != null)
            {
                Model = ModelManager.Instance.CreateInstance(fl);
            }
            else
            {
                Model = ModelManager.Instance.MissingModel;
            }
        }

        #region IDisposable 成员

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (Model != ModelManager.Instance.MissingModel)
                {
                    ModelManager.Instance.DestoryInstance(Model);
                }
            }
            Model = null;
            Field = null;
        }
        #endregion
    }


    public class TerrainObjectType : EntityType
    {
        public TerrainObjectType(BattleField btfld, string typeName)
            : base(btfld, typeName)
        {
 
        }

        public TerrainObject CreateInstance()
        {
            return new TerrainObject(this);
        }

        public int RadarColor
        {
            get;
            protected set;
        }
        public bool IsFlammable
        {
            get;
            protected set;
        }
        /// <summary>
        ///  Is the terrain immune to combat damage (def=no)?
        /// </summary>
        public bool Immune
        {
            get;
            protected set;
        }

        /// <summary>
        /// Is the terrain only allowed on the water (def=no)?
        /// </summary>
        public bool WaterBound
        {
            get;
            protected set;
        }

        /// <summary>
        /// Does it spawn growth of Tiberium around it (def=no)?
        /// </summary>
        public bool SpawnsTiberium
        {
            get;
            protected set;
        }

        public float AnimationProbability
        {
            get;
            protected set;
        }

        public bool IsAnimated
        {
            get;
            protected set;
        }

        public int Strength
        {
            get;
            protected set;
        }



        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            IsAnimated = sect.GetBool("IsAnimated", false);
            AnimationProbability = sect.GetFloat("AnimationProbability", 0.5f);
            SpawnsTiberium = sect.GetBool("SpawnsTiberium", false);
            WaterBound = sect.GetBool("WaterBound", false);
            Immune = sect.GetBool("Immune", false);
            IsFlammable = sect.GetBool("IsFlammable", false);
            RadarColor = sect.GetColorRGBInt("RadarColor", 0);

            Strength = sect.GetInt("Strength", 100);

        }
    }

    public class TerrainObject : Entity
    {
        TerrainObjectType type;

        public TerrainObject(TerrainObjectType ttype)
            : base(ttype.Field, ttype)
        {
            X = -1;
            Y = -1;

            type = ttype;
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

        public override void SetPosition(int x, int y)
        {
            if (X != -1 && Y != -1 && battleField.CellInfo[X][Y].Used)
            {
                battleField.CellInfo[X][Y].TObjs.Remove(this);
            }

            X = x;
            Y = y;

            if (battleField.CellInfo[x][y].Used)
            {
                battleField.CellInfo[x][y].TObjs.Add(this);
            }

            //Position =  new Vector3(Terrain.HorizontalUnit * x + Terrain.HorizontalUnit * 0.5f, battleField.CellInfo[x][y].HeightInfo.centre, Terrain.HorizontalUnit * y + Terrain.HorizontalUnit * 0.5f);
            battleField.GetCellCenter(x, y, out Position);
            BoundingSphere.Center = Position;
            BoundingSphere.Radius = Terrain.HorizontalUnit * 0.5f;
            Transformation = Matrix.Translation(base.BoundingSphere.Center);
        }
        public void SetPosition(int x, int y, float z)
        {
            if (X != -1 && Y != -1 && battleField.CellInfo[X][Y].Used)
            {
                battleField.CellInfo[X][Y].TObjs.Remove(this);
            }

            X = x;
            Y = y;

            if (battleField.CellInfo[x][y].Used)
            {
                battleField.CellInfo[x][y].TObjs.Add(this);
            }
            //Position = new Vector3(Terrain.HorizontalUnit * x + Terrain.HorizontalUnit * 0.5f, z, Terrain.HorizontalUnit * y + Terrain.HorizontalUnit * 0.5f);
            battleField.GetCellCenter(x, y, out Position);
            BoundingSphere.Center = Position;
            BoundingSphere.Radius = Terrain.HorizontalUnit * 0.5f;
            Transformation = Matrix.Translation(base.BoundingSphere.Center);
        }


        public override void Update(float dt)
        {
            // do nothing
        }
        
    }
}
