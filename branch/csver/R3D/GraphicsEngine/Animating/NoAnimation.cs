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
using System.Drawing.Design;
using System.Text;
using R3D.Design;
using R3D.GraphicsEngine;
using R3D.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine.Animating
{
    /// <summary>
    /// 仅有实体的world变换矩阵，无层次关系
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NoAnimation : Animation
    {
        static readonly string TransformsTag = "Transforms";

        public const string TypeName = "NoAnimation";

        Matrix[] transforms;

        public NoAnimation(Device dev, int entityCount)
            : base(dev, false)
        {
            transforms = new Matrix[entityCount];
            Matrix i4 = Matrix.Identity;
            for (int i = 0; i < entityCount; i++)
            {
                transforms[i] = i4;
            }
            CurrentFrame = 0;
        }
        public NoAnimation(Device dev, Matrix[] trans)
            : base(dev, false)
        {
            transforms = trans;
            CurrentFrame = 0;
        }

        [TypeConverter(typeof(ArrayConverter<MatrixEditor, Matrix>))]
        public Matrix[] Transforms
        {
            get { return transforms; }
            set { transforms = value; }
        }

        public override bool IsAvailable(int index)
        {
            return index >= 0 && index < transforms.Length;
        }
        //public override void SetAnimation(int index)
        //{
        //    device.SetTransform(TransformState.World, baseTranform);
        //    device.MultiplyTransform(TransformState.World, transforms[index]);
        //    CurrentFrame = 0;
        //}
        public override void GetTransform(int entityId,out Matrix mat)
        {
            mat = transforms[entityId];
            CurrentFrame = 0;
        }

        public override string AnimationType
        {
            get { return TypeName; }
        }

        public override void ReadData(BinaryDataReader data)
        {
            ArchiveBinaryReader br = data.GetData(TransformsTag);
            for (int i = 0; i < transforms.Length; i++)
            {
                br.ReadMatrix(out transforms[i]);
            }

            br.Close();
        }

        public override void WriteData(BinaryDataWriter data)
        {
            ArchiveBinaryWriter bw = data.AddEntry(TransformsTag);

            for (int i = 0; i < transforms.Length; i++)
            {
                bw.Write(ref transforms[i]);
            }

            bw.Close();
        }
    }
}
