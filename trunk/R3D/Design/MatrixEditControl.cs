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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using R3D.UI;
using SlimDX;
using R3D.Base;
using R3D.MathLib;

namespace R3D.Design
{
    public partial class MatrixEditControl : UserControl, IEditControl<Matrix>
    {
        enum TransformationType
        {
            None = 0,
            Translation,
            RotationAxis,
            RotationX,
            RotationY,
            RotationZ,
            RotationYawPitchRoll,
            Scaling
        }

        abstract class TransformationObject
        {
            [Browsable(false)]
            public abstract TransformationType Type { get; }
            [Browsable(false)]
            public abstract Matrix Transform { get; }
        }
        class NoTransformation : TransformationObject
        {
            public override TransformationType Type
            {
                get { return TransformationType.None; }
            }
            public override Matrix Transform
            {
                get { return Matrix.Identity; }
            }
        }
        class TranslationTransformation : TransformationObject
        {
            Vector3 trans;
            public float TranslationX
            {
                get { return trans.X; }
                set { trans.X = value; }
            }
            public float TranslationY
            {
                get { return trans.Y; }
                set { trans.Y = value; }
            }
            public float TranslationZ
            {
                get { return trans.Z; }
                set { trans.Z = value; }
            }

            public override TransformationType Type
            {
                get { return TransformationType.Translation; }
            }

            public override Matrix Transform
            {
                get { return Matrix.Translation(trans); }
            }
        }
        class RotationAxisTransformation : TransformationObject
        {
            float angle;
            Vector3 axis;

            public float AxisX
            {
                get { return axis.X; }
                set { axis.X = value; }
            }
            public float AxisY
            {
                get { return axis.Y; }
                set { axis.Y = value; }
            }
            public float AxisZ
            {
                get { return axis.Z; }
                set { axis.Z = value; }
            }
            public float Angle
            {
                get { return MathEx.Radian2Angle(angle); }
                set { angle = MathEx.Angle2Radian(value); }
            }
            public override TransformationType Type
            {
                get { return TransformationType.RotationAxis; }
            }

            public override Matrix Transform
            {
                get { return Matrix.RotationAxis(axis, angle); }
            }
        }
        class RotationXTransformation : TransformationObject
        {
            float angle;

            public float Angle
            {
                get { return MathEx.Radian2Angle(angle); }
                set { angle = MathEx.Angle2Radian(value); }
            }
            public override TransformationType Type
            {
                get { return TransformationType.RotationX; }
            }

            public override Matrix Transform
            {
                get { return Matrix.RotationX(angle); }
            }
        }
        class RotationYTransformation : TransformationObject
        {
            float angle;

            public float Angle
            {
                get { return MathEx.Radian2Angle(angle); }
                set { angle = MathEx.Angle2Radian(value); }
            }
            public override TransformationType Type
            {
                get { return TransformationType.RotationY; }
            }

            public override Matrix Transform
            {
                get { return Matrix.RotationY(angle); }
            }
        }
        class RotationZTransformation : TransformationObject
        {
            float angle;

            public float Angle
            {
                get { return MathEx.Radian2Angle(angle); }
                set { angle = MathEx.Angle2Radian(value); }
            }
            public override TransformationType Type
            {
                get { return TransformationType.RotationZ; }
            }

            public override Matrix Transform
            {
                get { return Matrix.RotationZ(angle); }
            }
        }
        class RotationYawPitchRollTransformation : TransformationObject
        {
            float yaw;
            float pitch;
            float roll;

            public float Yaw
            {
                get { return MathEx.Radian2Angle(yaw); }
                set { yaw = MathEx.Angle2Radian(value); }
            }
            public float Pitch
            {
                get { return MathEx.Radian2Angle(pitch); }
                set { pitch = MathEx.Angle2Radian(value); }
            }
            public float Roll
            {
                get { return MathEx.Radian2Angle(roll); }
                set { roll = MathEx.Angle2Radian(value); }
            }
            public override TransformationType Type
            {
                get { return TransformationType.RotationYawPitchRoll; }
            }

            public override Matrix Transform
            {
                get { return Matrix.RotationYawPitchRoll(yaw, pitch, roll); }
            }
        }
        class ScalingTransformation : TransformationObject
        {
            Vector3 scaling;

            public float ScalingX
            {
                get { return scaling.X; }
                set { scaling.X = value; }
            }
            public float ScalingY
            {
                get { return scaling.Y; }
                set { scaling.Y = value; }
            }
            public float ScalingZ
            {
                get { return scaling.Z; }
                set { scaling.Z = value; }
            }
            public override TransformationType Type
            {
                get { return TransformationType.Scaling; }
            }

            public override Matrix Transform
            {
                get { return Matrix.Scaling(scaling); }
            }
        }

        public MatrixEditControl(StringTable strTbl)
        {
            InitializeComponent();
            LanguageParser.ParseLanguage(strTbl, this);


            comboBox1.Items.Add("GUI:NoTransformation");
            comboBox1.Items.Add("GUI:TranslationTransformation");
            comboBox1.Items.Add("GUI:RotationAxisTransformation");
            comboBox1.Items.Add("GUI:RotationXTransformation");
            comboBox1.Items.Add("GUI:RotationYTransformation");
            comboBox1.Items.Add("GUI:RotationZTransformation");
            comboBox1.Items.Add("GUI:RotationYawPitchRollTransformation");
            comboBox1.Items.Add("GUI:ScalingTransformation");

        }

        Matrix value;
        IWindowsFormsEditorService service;

        #region IEditControl<Matrix> 成员

        public Matrix Value
        {
            get
            {
                if (propertyGrid1.SelectedObject != null)
                {
                    if (radioButton1.Checked && propertyGrid1.SelectedObject != null)
                    {
                        value = Matrix.Multiply(value, ((TransformationObject)propertyGrid1.SelectedObject).Transform);
                    }
                    else
                    {
                        value = ((TransformationObject)propertyGrid1.SelectedObject).Transform;
                    }
                }
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public IWindowsFormsEditorService Service
        {
            get { return service; }
            set { service = value; }
        }

        #endregion

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
             switch (comboBox1.SelectedIndex)
            {
                case (int)TransformationType.None:
                    propertyGrid1.SelectedObject = new NoTransformation();
                    break;
                case (int)TransformationType.RotationAxis:
                    propertyGrid1.SelectedObject = new RotationAxisTransformation();
                    break;
                case (int)TransformationType.RotationX:
                    propertyGrid1.SelectedObject = new RotationXTransformation();
                    break;
                case (int)TransformationType.RotationY:
                    propertyGrid1.SelectedObject = new RotationYTransformation();
                    break;
                case (int)TransformationType.RotationYawPitchRoll:
                    propertyGrid1.SelectedObject = new RotationYawPitchRollTransformation();
                    break;
                case (int)TransformationType.RotationZ:
                    propertyGrid1.SelectedObject = new RotationZTransformation();
                    break;
                case (int)TransformationType.Scaling:
                    propertyGrid1.SelectedObject = new ScalingTransformation();
                    break;
                case (int)TransformationType.Translation:
                    propertyGrid1.SelectedObject = new TranslationTransformation();
                    break;
            }
        }

        public void Reset()
        {
            comboBox1.SelectedIndex = -1;
            propertyGrid1.SelectedObject = null;
            service = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IWindowsFormsEditorService edSvc = service;
            Reset();
            edSvc.CloseDropDown();
        }
    }
}
