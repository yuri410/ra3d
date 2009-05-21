using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using R3D.Collections;
using R3D.AI;
using R3D.ConfigModel;
using R3D.MathLib;

namespace R3D.Logic
{
    // script->command->action 部队动作的三个级别

    public abstract class Action
    {

    }

    /// <summary>
    /// 微观运动基类
    /// </summary>
    public abstract class TechnoAction
    {
        /// <returns>仍然需要更新true，否则false</returns>
        public abstract bool Update(float dt);

        /// <summary>
        /// 第一次使用前调用
        /// </summary>
        public abstract void Execute();
    }

    /// <summary>
    /// 旋转
    /// </summary>
    public class TechnoRotateAction : TechnoAction
    {
        Locomotion locomotor;

        float targetYaw;

        public TechnoRotateAction(Techno tech, int targetDesc)
        {
            locomotor = tech.Locomotor;
            targetYaw = Techno.CalculateYaw(targetDesc);
        }
        public TechnoRotateAction(Techno tech, float targetYaw)
        {
            locomotor = tech.Locomotor;
            this.targetYaw = targetYaw;
        }

        public override void Execute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>仍然需要更新true，否则false</returns>
        public override bool Update(float dt)
        {
            if (locomotor != null)
            {
                return locomotor.UpdateRotate(targetYaw, dt);
            }
            return false;
        }
    }

    /// <summary>
    /// 移动一个单元
    /// </summary>
    public class TechnoMoveAction : TechnoAction
    {

        Locomotion locomotor;

        int nextX;
        int nextY;
        int nextZ;

        public TechnoMoveAction(Techno tech, int nx, int ny, int nz)
        {
            locomotor = tech.Locomotor;
            nextX = nx;
            nextY = ny;
            nextZ = nz;
        }

        /// <returns>仍然需要更新true，否则false</returns>
        public override bool Update(float dt)
        {
            if (locomotor != null)
            {
                return locomotor.UpdateMove(nextX, nextY, nextZ, dt);
            }
            return false;
        }

        public override void Execute()
        {

        }
    }


    public class TechnoDeployAction : TechnoAction
    {
        Techno techno;

        public TechnoDeployAction(Techno tech)
        {
            techno = tech;
        }

        public override bool Update(float dt)
        {
            return true;
        }

        public override void Execute()
        {
            techno.DeployAction();
        }
    }

    /// <summary>
    ///  基本命令如移动，攻击。一条命令只做一件事
    /// </summary>
    public abstract class TechnoCommand
    {
        //protected TechnoCommandBase(int id)
        //{
        //    ScriptTypeIndex = id;
        //}

        //public int ScriptTypeIndex
        //{
        //    get;
        //    protected set;
        //}

        /// <summary>
        ///  将该基本命令部署成若干子微观动作
        /// </summary>
        /// <returns></returns>
        public abstract TechnoAction[] DeployActions();

        /// <summary>
        ///  获取该基本命令是否可以部署多组微观动作组成
        /// </summary>
        public virtual bool HasMoreActionBatches
        {
            get { return false; }
        }
    }

    public class MoveCommand : TechnoCommand
    {
        Techno techno;

        TechnoAction[] actions;
        PathFinderResult path;
        PathFinder pathFinder;

        int targetX;
        int targetY;
        int targetZ;

        int batchIndex;

        public MoveCommand(Techno tech, int tx, int ty, int tz)
        {
            this.pathFinder = tech.PathFinder;
            this.techno = tech;

            this.targetX = tx;
            this.targetY = ty;
            this.targetZ = tz;

            pathFinder.Reset();
            path = pathFinder.FindPath(tech.X, tech.Y, tech.Z, tx, ty, tz);

            GetActions();

            if (!path.RequiresPathFinding)
            {
                path = null;
            }
        }

        void GetActions()
        {
            List<TechnoAction> acts = new List<TechnoAction>(path.NodeCount * 2);

            if (path.NodeCount > 0)
            {
                int lastOri = Techno.CalculateOriDesc(path[0].X - techno.X, path[0].Y - techno.Y);

                // 走上第一个节点的旋转
                if (techno.CellYaw != Techno.CalculateYaw(lastOri))
                {
                    acts.Add(new TechnoRotateAction(techno, lastOri));
                }
                // 走上第一个节点的平移
                acts.Add(new TechnoMoveAction(techno, path[0].X, path[0].Y, path[0].Z));

                for (int i = 1; i < path.NodeCount; i++)
                {
                    int nx = path[i].X;
                    int ny = path[i].Y;
                    int nz = path[i].Z;

                    int dx = nx - path[i - 1].X;
                    int dy = ny - path[i - 1].Y;

                    int oriDesc = Techno.CalculateOriDesc(dx, dy);

                    if (oriDesc != lastOri)
                    {
                        acts.Add(new TechnoRotateAction(techno, oriDesc));
                    }

                    acts.Add(new TechnoMoveAction(techno, nx, ny, nz));

                    lastOri = oriDesc;
                }
            }
            actions = acts.ToArray();
        }

        public override TechnoAction[] DeployActions()
        {
            if (batchIndex++ == 0)
            {
                return actions;
            }

            if (HasMoreActionBatches)
            {
                pathFinder.Continue();
                path = pathFinder.FindPath(techno.X, techno.Y, techno.Z, targetX, targetY, targetZ);

                GetActions();

                if (!path.RequiresPathFinding)
                {
                    path = null;
                }
                return actions;
            }

            return new TechnoAction[0];
        }

        public override bool HasMoreActionBatches
        {
            get { return path != null && path.RequiresPathFinding; }
        }
    }

    public class AttackTargetCommand : TechnoCommand
    {
        //public override void Run(Techno tec)
        //{

        //}
        public override TechnoAction[] DeployActions()
        {
            throw new NotImplementedException();
        }
        public override bool HasMoreActionBatches
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class JumpToLineControl : TechnoCommand
    {

        public JumpToLineControl ()
        {

        }

        public override TechnoAction[] DeployActions()
        {
            throw new NotImplementedException();
        }
    }

    public class DeployCommand : TechnoCommand
    {
        Techno techno;

        public DeployCommand(Techno tech)
        {
            techno = tech;
        }

        public override TechnoAction[] DeployActions()
        {
            return new TechnoAction[2]
            {
                new TechnoRotateAction(techno, MathEx.Angle2Radian(-90)),
                new TechnoDeployAction(techno)
            };
        }
    }

    //public class AttackCommand : TechnoCommandBase
    //{
    //    public enum AttackTargetType
    //    {
    //        NotSpecified = 1,
    //        Buildings = 2,
    //        Harvesters = 3,
    //        Infantry = 4,
    //        Vehicle = 5,
    //        Factory = 6,
    //        BaseDefense = 7,
    //        PowerPlants = 9
    //    }

    //    public AttackCommand()
    //        : base(0)
    //    { }

    //    public override void Run(Techno tec)
    //    {
    //        tec.Attack();
    //    }

    //    public override void Parse(string[] str)
    //    {
    //        TargeType = (AttackTargetType)int.Parse(str[0]);
    //    }

    //    public AttackTargetType TargeType
    //    {
    //        get;
    //        set;
    //    }

    //}

    public delegate void CommandSetChangedHandler();

    /// <summary>
    ///  部队基本命令集
    /// </summary>
    public class TechnoCommandSet : CollectionBase<TechnoCommand>
    {
        public void AddCommand(TechnoCommand cmd)
        {
            base.Add(cmd);

            OnChanged();
        }

        public new void Clear()
        {
            base.Clear();
            OnChanged();
        }

        /// <summary>
        ///  触发Changed事件
        /// </summary>
        protected void OnChanged()
        {
            if (Changed != null)
            {
                Changed();
            }
        }

        /// <summary>
        ///  当基本命令集发生变动时，该事件被触发
        /// </summary>
        public event CommandSetChangedHandler Changed;
    }

    /// <summary>
    /// TechnoCommandBase解释器
    /// </summary>    
    public class TechnoCommandInterpreter
    {
        TechnoCommandSet cmdSet;

        int curCmdId;
        public int CurCmdId
        {
            get { return curCmdId; }
            set
            {
                if (value != curCmdId)
                {
                    hasGotActionList = false;
                    curCmdId = value;
                }
            }
        }

        TechnoAction[] curActionList;
        TechnoAction[] CurActionList
        {
            get { return curActionList; }
            set
            {
                if (value != curActionList)
                {
                    curActionList = value;
                    hasGotActionList = false;
                }

            }
        }

        int CurActId
        {
            get;
            set;
        }

        bool isActExecuted;
        bool hasGotActionList;

        public TechnoCommandSet CommandSet
        {
            get
            {
                if (cmdSet == null)
                    CommandSet = new TechnoCommandSet();
                return cmdSet;
            }
            set
            {
                if (value != cmdSet)
                {
                    if (value != null)
                    {
                        value.Changed += this.CmdSetChanged;
                    }
                    if (cmdSet != null)
                    {
                        value.Changed -= this.CmdSetChanged;
                    }
                }
                cmdSet = value;
                CurCmdId = 0;
            }
        }

        void CmdSetChanged()
        {
            CurCmdId = 0;
        }

        public void Update(float dt)
        {
            if (cmdSet != null)
            {
                if (CurCmdId < cmdSet.Count)
                {
                    if (!hasGotActionList)
                    {
                        CurActionList = cmdSet.Elements[CurCmdId].DeployActions();
                        hasGotActionList = true;
                    }


                    if (CurActId < CurActionList.Length)
                    {
                        if (!isActExecuted)
                        {
                            CurActionList[CurActId].Execute();
                            isActExecuted = true;
                        }

                        if (!CurActionList[CurActId].Update(dt))
                        {
                            CurActId++;
                            isActExecuted = false;
                        }

                    }
                    else if (cmdSet.Elements[CurCmdId].HasMoreActionBatches)
                    {
                        CurActId = 0;
                        CurActionList = cmdSet.Elements[CurCmdId].DeployActions();
                    }
                    else
                    {
                        CurActId = 0;
                        CurCmdId++;
                    }
                }

            }
        }

        public void Stop()
        {
            cmdSet.Clear();
        }
    }

    /// <summary>
    ///  部队基本命令集抽象工厂
    /// </summary>
    public abstract class TechnoCommandFactory
    {
        public abstract TechnoCommand CreateInstance(Techno tech, string[] data);

        public abstract Type CreationType
        {
            get;
        }
    }

    public class MoveCommandFactory : TechnoCommandFactory
    {

        public override TechnoCommand CreateInstance(Techno tech, string[] data)
        {
            return new MoveCommand(tech, int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]));
        }

        public override Type CreationType
        {
            get { return typeof(MoveCommand); }
        }
    }


    public static class PresetedTechnoCommand
    {
        public static string Move
        {
            get;
            private set;
        }

        public static string Deploy
        {
            get;
            private set;
        }

        public static string Attack
        {
            get;
            private set;
        }

        static PresetedTechnoCommand()
        {

        }
    }

    public class TechnoCommandManager
    {
        static TechnoCommandManager singleton;

        public static TechnoCommandManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new TechnoCommandManager();
                }
                return singleton;
            }
        }

        Dictionary<string, TechnoCommandFactory> factories;

        private TechnoCommandManager()
        {
            factories = new Dictionary<string, TechnoCommandFactory>();
        }

        public void RegisterCommandType(Type type, TechnoCommandFactory fac)
        {
            factories.Add(type.Name, fac);
        }

        public void UnregisterCommandType(Type type)
        {
            factories.Remove(type.Name);
        }


        public TechnoCommand GetCommand(string name, Techno tech, string[] param)
        {
            TechnoCommandFactory fac;
            if (factories.TryGetValue(name, out fac))
            {
                return fac.CreateInstance(tech, param);
            }
            else
                throw new NotSupportedException(name);
        }
    }


    public abstract class TechnoScript
    {
        public abstract void GetCommands(BattleField btfld, TechnoCommandSet cmdSet, Techno tech);


    }
    public abstract class TechnoScriptFactory
    {

        protected TechnoScriptFactory(int index)        
        {
            ScriptIndex = index;
        }

        public int ScriptIndex
        {
            get;
            private set;
        }
    }

    public class TechnoScriptManager
    {



    }

    public class TechnoScriptSet : CollectionBase<string>, IConfigurable
    {
        BattleField battleField;

        public TechnoScriptSet(BattleField btfld)
        {
            battleField = btfld;
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }

        #endregion


        //public void WriteToCommandSet(TechnoCommandSet cmdSet, Techno tech)
        //{
            
        //}

        public void Compile(TechnoCommandSet cmdSet, Techno tech)
        {

        }

        //public int ScriptCommandCount
        //{
        //    get { return base.Count; }
        //}

        public void AddScriptCommand(string expression)
        {
            Add(expression);
        }
        
        public new void Clear()
        {
            base.Clear();
        }

        public override string ToString()
        {
            return "Lines: " + Count.ToString();
        }

        public string GetCode()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Count; i++) 
            {
                sb.AppendLine(base[i]);
            }

            return sb.ToString();
        }
    }
}
