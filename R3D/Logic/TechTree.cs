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
using System.Reflection;
using System.Reflection.Emit;

namespace R3D.Logic
{
    public delegate void ConstructionOptionChangedHandler(TechnoType[] newOptions);

    public class PrerequisiteExpressionCompiler
    {
        const char NUM = (char)256;

        public const char LBracket = '(';
        public const char RBracket = ')';

        public const char OpAnd = '&';
        public const char OpOr = '|';
        public const char OpNot = '!';

        static readonly string LBracketS = "(";
        static readonly string RBracketS = ")";

        static readonly string OpAndS = "&";
        static readonly string OpOrS = "|";
        static readonly string OpNotS = "!";

        static readonly char EndSym = '$';
        static readonly string EndSymS = "$";

        struct PriorInfo
        {
            public char OperatorChar;
            public int P1;
            public int P2;
        }
        PriorInfo[] pList = new PriorInfo[6]
        {
            new PriorInfo { OperatorChar = '&', P1 = 2, P2 = 1 },
            new PriorInfo { OperatorChar = '|', P1 = 2, P2 = 1 },
            new PriorInfo { OperatorChar = '!', P1 = 4, P2 = 3 },
            new PriorInfo { OperatorChar = '(', P1 = 0, P2 = 5 },
            new PriorInfo { OperatorChar = ')', P1 = 6, P2 = 0 },
            new PriorInfo { OperatorChar = '$', P1 = 0, P2 = 0 }
        };

        enum NodeType
        {
            Operator,
            Operand
        }

        class ExpressionTreeNode
        {
            public char optr;
            public int opnd;

            public NodeType type;

            public ExpressionTreeNode left;
            public ExpressionTreeNode right;

            /// <summary>
            /// 建立二叉树运算符结点(内结点)   
            /// </summary>
            /// <param name="op">op运算符</param>
            /// <param name="left">left左子树指针</param>
            /// <param name="right">right右子树指针</param>
            /// <returns>二叉树内结点指针</returns>
            public ExpressionTreeNode(char optr, ExpressionTreeNode left, ExpressionTreeNode right)
            {
                this.optr = optr;
                this.left = left;
                this.right = right;
                type = NodeType.Operator;
            }

            /// <summary>
            /// 建立二叉树数结点(叶结点)   
            /// </summary>
            /// <param name="num">操作数</param>
            /// <returns>二叉树叶结点指针</returns>
            public ExpressionTreeNode(int operand)
            {
                opnd = operand;
                type = NodeType.Operand;
            }
        }


        Stack<char> optrStack;
        Stack<ExpressionTreeNode> exprStack;

        /// <summary>
        /// 当GetNextSymbol()为NUM时，该字段为参数值
        /// </summary>
        int tokenval;
        List<string> expStruct;

        int position;
        Dictionary<string, int> parameters;

        /// <summary>
        /// 比较栈顶运算符与下一输入运算符优先关系
        /// </summary>
        /// <param name="opf">栈顶运算符</param>
        /// <param name="opg"> 下一输入运算符</param>
        /// <returns>关系'>',   '=',   '<'</returns>
        char Precede(char opf, char opg)
        {
            int op1 = -1, op2 = -1;
            for (int i = 0; i < pList.Length; i++)
            {
                if (pList[i].OperatorChar == opf)
                    op1 = pList[i].P1;
                if (pList[i].OperatorChar == opg)
                    op2 = pList[i].P2;
            }
            if (op1 == -1 || op2 == -1)
            {
                throw new InvalidOperationException("operator error!");
                //cout << "operator   error!" << endl;
                //exit(1);
            }
            if (op1 > op2)
                return '>';
            else if (op1 == op2)
                return '=';
            else
                return '<';
        }

        /// <summary>
        /// 获得表达式中下一个标记。如果是变量则返回NUM，变量索引tokenval
        /// </summary>
        /// <returns></returns>
        char GetNextSymbol()
        {
            string str = expStruct[position++];

            if (str == LBracketS || str == RBracketS || str == OpAndS || str == OpOrS || str == OpNotS || str == EndSymS)
            {
                return str[0];
            }
            else
            {
                tokenval = parameters[str];
                return NUM;
            }
        }


        /// <summary>
        /// 建立表达式二叉树(参考严蔚敏，吴伟民的《数据结构》P_53) 
        /// </summary>
        /// <returns>二叉树跟结点指针</returns>
        ExpressionTreeNode CreateBinaryTree()
        {
            char lookahead;
            char op;
            ExpressionTreeNode opnd1, opnd2;
            optrStack.Push(EndSym);
            lookahead = GetNextSymbol();
            while (lookahead != EndSym || optrStack.Peek() != EndSym)
            {
                if (lookahead == NUM)
                {
                    exprStack.Push(new ExpressionTreeNode(tokenval)); // mkleaf(tokenval)
                    lookahead = GetNextSymbol();
                }
                else
                {
                    switch (Precede(optrStack.Peek(), lookahead))
                    {
                        case '<':
                            optrStack.Push(lookahead);
                            lookahead = GetNextSymbol();
                            break;
                        case '=':
                            optrStack.Pop();
                            lookahead = GetNextSymbol();
                            break;
                        case '>':
                            op = optrStack.Pop();

                            if (op == OpNot)
                            {
                                opnd2 = exprStack.Pop();
                                exprStack.Push(new ExpressionTreeNode(op, null, opnd2));//mknode(op, null, opnd2)
                            }
                            else
                            {
                                opnd2 = exprStack.Pop();

                                opnd1 = exprStack.Pop();
                                exprStack.Push(new ExpressionTreeNode(op, opnd1, opnd2));//mknode(op, opnd1, opnd2)
                            }

                            break;
                    }
                }
            }
            return exprStack.Peek();
        }

        /// <summary>
        /// 后序遍历
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        bool FollowOrderTraverse(ILGenerator codeGen, ExpressionTreeNode T)
        {
            if (T == null)
                return true;

            if (T.type == NodeType.Operator)
            {
                if (FollowOrderTraverse(codeGen, T.left))
                {
                    if (FollowOrderTraverse(codeGen, T.right))
                    {
                        switch (T.optr)
                        {
                            case OpAnd:
                                //Console.WriteLine("and");
                                codeGen.Emit(OpCodes.And);
                                break;
                            case OpOr:
                                //Console.WriteLine("or");
                                codeGen.Emit(OpCodes.Or);
                                break;
                            case OpNot:
                                //Console.WriteLine("not");
                                codeGen.Emit(OpCodes.Not);
                                break;
                        }
                        return true;
                    }
                }
                return false;

            }
            else
            {
                //Console.WriteLine("ldarg.0");
                //Console.WriteLine("ldc.i4   " + T.opnd.ToString());
                //Console.WriteLine("ldelem.i1");

                codeGen.Emit(OpCodes.Ldarg_0);
                codeGen.Emit(OpCodes.Ldc_I4, T.opnd);
                codeGen.Emit(OpCodes.Ldelem_I1);
                return true;
            }
        }


        void Parse(string expression)
        {
            StringBuilder sb = new StringBuilder();

            expStruct = new List<string>();
            parameters = new Dictionary<string, int>();

            bool ended = false;

            int paramIndex = 0;

            for (int i = 0; i < expression.Length; i++)
            {
                char cc = expression[i];
                switch (cc)
                {
                    case LBracket:
                        expStruct.Add(LBracket.ToString());

                        if (sb.Length > 0)
                        {
                            string typeName = sb.ToString();
                            expStruct.Add(typeName);
                            if (!parameters.ContainsKey(typeName))
                            {
                                parameters.Add(typeName, paramIndex++);
                            }
                        }
                        ended = true;

                        sb = new StringBuilder();
                        break;
                    case RBracket:
                        if (sb.Length > 0)
                        {
                            string typeName = sb.ToString();
                            expStruct.Add(typeName);
                            if (!parameters.ContainsKey(typeName))
                            {
                                parameters.Add(typeName, paramIndex++);
                            }
                        }

                        expStruct.Add(RBracket.ToString());

                        ended = true;

                        sb = new StringBuilder();
                        break;
                    case OpAnd:
                        if (sb.Length > 0)
                        {
                            string typeName = sb.ToString();
                            expStruct.Add(typeName);
                            if (!parameters.ContainsKey(typeName))
                            {
                                parameters.Add(typeName, paramIndex++);
                            }
                        }

                        expStruct.Add(OpAnd.ToString());

                        ended = true;

                        sb = new StringBuilder();
                        break;
                    case OpOr:
                        if (sb.Length > 0)
                        {
                            string typeName = sb.ToString();
                            expStruct.Add(typeName);
                            if (!parameters.ContainsKey(typeName))
                            {
                                parameters.Add(typeName, paramIndex++);
                            }
                        }
                        expStruct.Add(OpOr.ToString());

                        ended = true;

                        sb = new StringBuilder();
                        break;
                    case OpNot:
                        if (sb.Length > 0)
                        {
                            string typeName = sb.ToString();
                            expStruct.Add(typeName);
                            if (!parameters.ContainsKey(typeName))
                            {
                                parameters.Add(typeName, paramIndex++);
                            }
                        }

                        expStruct.Add(OpNot.ToString());

                        ended = true;

                        sb = new StringBuilder();
                        break;
                    case ' ':
                        break;
                    default:
                        sb.Append(cc);
                        ended = false;

                        break;

                }
            }

            if (!ended)
            {
                string typeName = sb.ToString();
                expStruct.Add(typeName);
                if (!parameters.ContainsKey(typeName))
                {
                    parameters.Add(typeName, paramIndex++);
                }
            }
        }

        public DynamicMethod Compile(string expression)
        {
            optrStack = new Stack<char>();
            exprStack = new Stack<ExpressionTreeNode>();

            tokenval = 0;
            position = 0;

            Parse(expression);

            expStruct.Add(EndSymS);

            ExpressionTreeNode tree = CreateBinaryTree();


            DynamicMethod mthd = new DynamicMethod("evaluator", typeof(bool), new Type[] { typeof(bool[]) });

            ParameterBuilder param = mthd.DefineParameter(1, ParameterAttributes.In, "state");

            ILGenerator codeGen = mthd.GetILGenerator();

            FollowOrderTraverse(codeGen, tree);
            codeGen.Emit(OpCodes.Ret);

            return mthd;
        }

        public string[] GetParameters()
        {
            string[] res = new string[parameters.Count];

            foreach (KeyValuePair<string, int> e in parameters)
            {
                res[e.Value] = e.Key;
            }
            return res;
        }
    }

    public class PrerequisiteExpression
    {
        delegate bool InternalCall(bool[] args);

        DynamicMethod evaluator;
        string expression;

        //Dictionary<string, TechnoType> parameters;
        bool[] parameters2;
        TechnoType[] parameters;

        InternalCall invoker;
        
        void Load(PrerequisiteExpressionCompiler compiler, string expression, Dictionary<string, TechnoType> techTypes)
        {
            evaluator = compiler.Compile(expression);

            string[] paraNames = compiler.GetParameters();

            //this.parameters = new Dictionary<string, TechnoType>();
            parameters = new TechnoType[paraNames.Length];

            for (int i = 0; i < paraNames.Length; i++)
            {
                TechnoType type;
                if (techTypes.TryGetValue(paraNames[i], out type))
                {
                    parameters[i] = techTypes[paraNames[i]];
                }
                else
                {
                    GameConsole.Instance.Write(paraNames[i] + " in Prerequisite or PrerequisiteEx is not found. Ignored." , ConsoleMessageType.Warning);
                }
            }

            parameters2 = new bool[paraNames.Length];

            this.expression = expression;
            invoker = (InternalCall)evaluator.CreateDelegate(typeof(InternalCall));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="elements"></param>
        /// <param name="painf"></param>
        /// <param name="techTypes">TechnoType list containing all TechnoTypes</param>
        public PrerequisiteExpression(PrerequisiteExpressionCompiler compiler, string[] elements, PrerequisiteAliasInfo painf, Dictionary<string, TechnoType> techTypes)
        {
            StringBuilder code = new StringBuilder();
            int len = elements.Length;

            for (int i = 0; i < len; i++)
            {
                elements[i] = elements[i].ToUpper();
                switch (elements[i])
                {
                    case "POWER":
                        code.Append(painf.PrerequisitePowerExp);
                        break;
                    case "FACTORY":
                        code.Append(painf.PrerequisiteFactoryExp);
                        break;
                    case "RADAR":
                        code.Append(painf.PrerequisiteRadarExp);
                        break;
                    case "BARRACKS":
                        code.Append(painf.PrerequisiteBarracksExp);  
                        break;
                    case "TECH":
                        code.Append(painf.PrerequisiteTechExp);
                        break;
                    case "PROC":
                        code.Append(painf.PrerequisiteProcExp);
                        break;
                    default:
                        code.Append(elements[i]);
                        break;
                }

                if (i < len - 1)
                {
                    code.Append(PrerequisiteExpressionCompiler.OpAnd);
                }
            }
            Load(compiler, code.ToString(), techTypes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="expression"></param>
        /// <param name="techTypes">TechnoType list containing all TechnoTypes</param>
        public PrerequisiteExpression(PrerequisiteExpressionCompiler compiler, string expression, PrerequisiteAliasInfo painf, Dictionary<string, TechnoType> techTypes)
        {
            expression = expression.ToUpper();

            expression = expression.Replace("POWER", painf.PrerequisitePowerExp);
            expression = expression.Replace("FACTORY", painf.PrerequisiteFactoryExp);
            expression = expression.Replace("RADAR", painf.PrerequisiteRadarExp);
            expression = expression.Replace("BARRACKS", painf.PrerequisiteBarracksExp);
            expression = expression.Replace("TECH", painf.PrerequisiteTechExp);
            expression = expression.Replace("PROC", painf.PrerequisiteProcExp);
            
            Load(compiler, expression, techTypes);
        }

        public TechnoType[] RelatedTechnoTypes
        {
            get { return parameters; }
        }

        public bool Evaluate(TechTree techTree)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null)
                {
                    parameters2[i] = techTree.HasTechno(parameters[i]);
                }
                else
                {
                    parameters2[i] = true;
                }
            }
            return invoker.Invoke(parameters2); // (bool)evaluator.Invoke(null, new object[] { (object)parameters2 });
        }

        public override string ToString()
        {
            return expression;
        }
    }


    public class TechTree
    {
        enum TechTreeNodeState
        {
            None,
            HasTechno,
            Available
        }

        /// <summary>
        ///  buildableTable
        /// </summary>
        //Dictionary<TechnoType, TechTreeNodeState> buildableTable;
        Dictionary<TechnoType, bool> buildableTable;

        /// <summary>
        ///  
        /// </summary>
        Dictionary<TechnoType, int> techCount;

        House house;

        BattleField battleField;

        public TechTree(BattleField btfld, House house)
        {
            this.house = house;
            this.battleField = btfld;

            buildableTable = new Dictionary<TechnoType, bool>();

            AvailableAirTypes = new List<AircraftType>();
            AvailableDefenceTypes = new List<BuildingType>();
            AvailableInfantryTypes = new List<InfantryType>();
            AvailableResTypes = new List<BuildingType>();
            AvailableUnitTypes = new List<UnitType>();

            techCount = new Dictionary<TechnoType, int>();
        }

        public List<InfantryType> AvailableInfantryTypes
        {
            get;
            protected set;
        }
        public List<UnitType> AvailableUnitTypes
        {
            get;
            protected set;
        }
        public List<AircraftType> AvailableAirTypes
        {
            get;
            protected set;
        }
        public List<BuildingType> AvailableResTypes
        {
            get;
            protected set;
        }
        public List<BuildingType> AvailableDefenceTypes
        {
            get;
            protected set;
        }

        //public event ConstructionOptionChangedHandler ConstructionOptionChanged;
        public event ConstructionOptionChangedHandler NewConstructionOptions;
        public event ConstructionOptionChangedHandler LostConstructionOptions;

        void OnNewConstructionOptions(TechnoType[] newOptions)
        {
            if (NewConstructionOptions != null)
            {
                NewConstructionOptions(newOptions);
            }
        }
        void OnLostConstructionOptions(TechnoType[] options)
        {
            if (LostConstructionOptions != null)
            {
                LostConstructionOptions(options);
            }
        }

        public bool HasTechno(TechnoType type)
        {
            int curTechnoCount;
            if (techCount.TryGetValue(type, out curTechnoCount))
            {
                return curTechnoCount > 0;
            }
            return false;
        }
        //[Obsolete()]
        //bool IsBuildable(TechnoType type)
        //{
        //    List<TechnoType> relatedTechnoTypes = type.RelatedPrerequisites;

        //    for (int i = 0; i < relatedTechnoTypes.Count; i++)
        //    {
        //        TechnoType techType = relatedTechnoTypes[i];
        //        for (int j = 0; j < techType.Prerequisites.Length; j++)
        //        {
        //            if (!HasTechno(techType.Prerequisites[j]))
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return type.TechLevel != -1 && house.TechLevel >= type.TechLevel;
        //}
        bool IsBuildable(TechnoType type)
        {
            //List<TechnoType> relatedTechnoTypes = type.RelatedPrerequisites;

            //for (int i = 0; i < relatedTechnoTypes.Count; i++)
            //{
            //    TechnoType techType = relatedTechnoTypes[i];
            //    for (int j = 0; j < techType.Prerequisites.Length; j++)
            //    {
            //        if (!HasTechno(techType.Prerequisites[j]))
            //        {
            //            return false;
            //        }
            //    }
            //}
            if (type.TechLevel != -1 && house.TechLevel >= type.TechLevel)
            {
                if (type.Prerequisites != null)
                {
                    return type.Prerequisites.Evaluate(this);
                }
                return true;
            }
            return false;
        }

        void TechnoCreated(Techno techno)
        {
            TechnoType type = techno.Type;

            if (!techCount.ContainsKey(type))
            {
                techCount.Add(techno.Type, 1);
            }
            else
            {
                techCount[techno.Type]++;
            }



            List<TechnoType> newOptions = null;
            List<TechnoType> relatedTechnoTypes = type.RelatedPrerequisites;

            for (int i = 0; i < relatedTechnoTypes.Count; i++)
            {
                bool buildable;

                if (!buildableTable.TryGetValue(relatedTechnoTypes[i], out buildable))
                {
                    buildableTable.Add(relatedTechnoTypes[i], false);
                    buildable = false;
                }


                if (!buildable &&
                     IsBuildable(relatedTechnoTypes[i]))
                {
                    buildableTable[relatedTechnoTypes[i]] = true;

                    if (newOptions == null)
                    {
                        newOptions = new List<TechnoType>();
                    }
                    newOptions.Add(relatedTechnoTypes[i]);
                }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
            }

            if (newOptions != null)
            {
                OnNewConstructionOptions(newOptions.ToArray());
                for (int i = 0; i < newOptions.Count; i++)
                {
                    switch (newOptions[i].WhatAmI)
                    {
                        case TechnoCategory.Infantry:
                            AvailableInfantryTypes.Add((InfantryType)newOptions[i]);
                            break;
                        case TechnoCategory.Unit:
                            AvailableUnitTypes.Add((UnitType)newOptions[i]);
                            break;
                        case TechnoCategory.Building:
                            BuildingType bt = (BuildingType)newOptions[i];
                            if (bt.IsBaseDefense)
                            {
                                AvailableDefenceTypes.Add(bt);
                            }
                            else
                            {
                                AvailableResTypes.Add(bt);
                            }
                            break;
                        case TechnoCategory.Aircraft:
                            AvailableAirTypes.Add((AircraftType)newOptions[i]);
                            break;
                    }

                }
            }

        }
        void TechnoDeleted(Techno techno)
        {
            TechnoType type = techno.Type;
            int curTechnoCount;
            if (techCount.TryGetValue(type, out curTechnoCount))
            {
                if (curTechnoCount == 1)
                {
                    techCount.Remove(techno.Type);
                }
                else
                {
                    techCount[techno.Type]--;
                }
            }

            List<TechnoType> relatedTechnoTypes = type.RelatedPrerequisites;
            List<TechnoType> lostOptions = null;

            for (int i = 0; i < relatedTechnoTypes.Count; i++)
            {
                bool buildable;

                if (!buildableTable.TryGetValue(relatedTechnoTypes[i], out buildable))
                {
                    buildableTable.Add(relatedTechnoTypes[i], false);
                    buildable = false;
                }

                if (buildable &&
                    !IsBuildable(relatedTechnoTypes[i]))
                {
                    buildableTable[relatedTechnoTypes[i]] = true;

                    if (lostOptions == null)
                    {
                        lostOptions = new List<TechnoType>();
                    }
                    lostOptions.Add(relatedTechnoTypes[i]);
                }

            }
            if (lostOptions != null)
            {
                OnLostConstructionOptions(lostOptions.ToArray());
                for (int i = 0; i < lostOptions.Count; i++)
                {
                    switch (lostOptions[i].WhatAmI)
                    {
                        case TechnoCategory.Infantry:
                            AvailableInfantryTypes.Remove((InfantryType)lostOptions[i]);
                            break;
                        case TechnoCategory.Unit:
                            AvailableUnitTypes.Remove((UnitType)lostOptions[i]);
                            break;
                        case TechnoCategory.Building:
                            BuildingType bt = (BuildingType)lostOptions[i];
                            if (bt.IsBaseDefense)
                            {
                                AvailableDefenceTypes.Remove(bt);
                            }
                            else
                            {
                                AvailableResTypes.Remove(bt);
                            }
                            break;
                        case TechnoCategory.Aircraft:
                            AvailableAirTypes.Remove((AircraftType)lostOptions[i]);
                            break;
                    }

                }
            }
        }

        public void InfantryCreated(Infantry obj)
        {
            TechnoCreated(obj);
        }
        public void InfantryDeleted(Infantry obj)
        {
            TechnoDeleted(obj);

        }

        public void UnitCreated(Unit unit)
        {
            TechnoCreated(unit);

        }
        public void UnitDeleted(Unit unit)
        {
            TechnoDeleted(unit);

        }

        public void AirCreated(Aircraft air)
        {
            TechnoCreated(air);

        }
        public void AircraftDeleted(Aircraft air)
        {
            TechnoDeleted(air);

        }

        public void ResBuildingCreated(Building bld)
        {
            TechnoCreated(bld);

        }
        public void ResBuildingDeleted(Building bld)
        {
            TechnoDeleted(bld);

        }


        public void DefBuildingCreated(Building bld)
        {
            TechnoCreated(bld);

        }
        public void DefBuildingDeleted(Building bld)
        {
            TechnoDeleted(bld);

        }
    }
}
