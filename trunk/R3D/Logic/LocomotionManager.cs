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
using System.Runtime.InteropServices;
using R3D.ConfigModel;

namespace R3D.Logic
{
    public abstract class LocomotionFactory : IConfigurable
    {
        public abstract Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc);

        public abstract LocomotionDescription CreateDescription(BattleField fld);

        public abstract string TypeName { get; }

        #region IConfigurable 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sect">A ConfigurationSection sets this kind of Locomotor up</param>
        public abstract void Parse(ConfigurationSection sect);

        #endregion
    }


    [Guid("4A582741-9839-11d1-B709-00A024DDAFD1")]
    public class DriveLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "DriveLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }
    [Guid("4A582746-9839-11d1-B709-00A024DDAFD1")]
    public class FlyLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "FlyLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }
    [Guid("4A582742-9839-11d1-B709-00A024DDAFD1")]
    public class HoverLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "HoverLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }
    [Guid("92612C46-F71F-11d1-AC9F-006008055BB5")]
    public class JumpjetLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "JumpjetLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }
    [Guid("55D141B8-DB94-11d1-AC98-006008055BB5")]
    public class MechLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "MechLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }
    [Guid("B7B49766-E576-11d3-9BD9-00104B972FE8")]
    public class RocketLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "RocketLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }
    [Guid("2BEA74E1-7CCA-11d3-BE14-00104B62A16C")]
    public class ShipLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "ShipLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }
    [Guid("4A582747-9839-11d1-B709-00A024DDAFD1")]
    public class TeleportLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "TeleportLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }
    [Guid("4A582743-9839-11D1-B709-00A024DDAFD1")]
    public class TunnelLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "TunnelLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }
    [Guid("4A582744-9839-11d1-B709-00A024DDAFD1")]
    public class WalkLocomotionFactory : LocomotionFactory
    {
        public override Locomotion CreateInstance(BattleField fld, Techno unit, LocomotionDescription desc)
        {
            return new DriveLocomotion(fld, unit, (DriverLocomotionDescription)desc);
        }

        public override LocomotionDescription CreateDescription(BattleField fld)
        {
            return new DriverLocomotionDescription(fld, this);
        }
        public override string TypeName
        {
            get { return "WalkLocomotor"; }
        }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }
    }

    public class LocomotionManager
    {
        //static LocomotionManager singleton;

        //public static LocomotionManager Instance
        //{
        //    get
        //    {
        //        if (singleton == null)
        //            singleton = new LocomotionManager();
        //        return singleton;
        //    }
        //}

        Dictionary<string, LocomotionFactory> factories;
        Dictionary<Guid, LocomotionFactory> guidTable;

        public LocomotionManager()
        {
            factories = new Dictionary<string, LocomotionFactory>(CaseInsensitiveStringComparer.Instance);
            guidTable = new Dictionary<Guid, LocomotionFactory>();
 
        }

        public static bool IsGuidLocomotor(string typeName)
        {
            return (typeName.StartsWith("{") && typeName.EndsWith("}"));            
        }

        public void Clear()
        {
            factories.Clear();
        }
        public void UnregisterLocomotionType(string typeName)
        {
            factories.Remove(typeName);
        }
        public void UnregisterLocomotionType(LocomotionFactory fac)
        {
            string typeName = fac.TypeName;
            factories.Remove(typeName);

            Type facType = fac.GetType();
            object[] attrs = facType.GetCustomAttributes(typeof(GuidAttribute), true);
            if (attrs.Length > 0)
            {
                GuidAttribute guidAtt = (GuidAttribute)attrs[0];
                
                guidTable.Remove(new Guid(guidAtt.Value));
            }
        }
        public void RegisterLocomotionType(LocomotionFactory fac)
        {
            string typeName = fac.TypeName;
            if (IsGuidLocomotor(typeName))
                throw new ArgumentException();

            factories.Add(typeName, fac);

            Type facType = fac.GetType();
            object[] attrs = facType.GetCustomAttributes(typeof(GuidAttribute), true);
            if (attrs.Length > 0)
            {
                GuidAttribute guidAtt = (GuidAttribute)attrs[0];
                
                guidTable.Add(new Guid(guidAtt.Value), fac);
            }
        }


        public LocomotionDescription CreateDescription(BattleField fld, Guid guid)
        {
            LocomotionFactory fac;
            if (guidTable.TryGetValue(guid, out fac))
            {
                return fac.CreateDescription(fld);
            }
            throw new NotSupportedException(guid.ToString());
        }
        public LocomotionDescription CreateDescription(BattleField fld, string typeName)
        {
            LocomotionFactory fac;
            if (factories.TryGetValue(typeName, out fac))
            {
                return fac.CreateDescription(fld);
            }
            throw new NotSupportedException(typeName);
        }
    }
}
