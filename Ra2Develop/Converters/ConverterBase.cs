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
using R3D.IO;
using System.Windows.Forms;

namespace Ra2Develop.Converters
{
    /// <summary>
    /// 用于转换数据的转换器
    /// </summary>
    public abstract class ConverterBase
    {
        protected Icon icon;        

        public abstract void ShowDialog(object sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <remarks>将参数设计成属性</remarks>
        public abstract void Convert(ResourceLocation source, ResourceLocation dest);

        public abstract string Name
        {
            get;
        }

        public Icon GetIcon
        {
            get
            {
                if (icon == null)
                {
                    return Program.DefaultIcon;
                }
                return icon;                                                 
            }
        }

        public abstract string[] SourceExt { get; }
        public abstract string[] DestExt { get; }

        public abstract string SourceDesc { get; }
        public abstract string DestDesc { get; }

        public string  GetOpenFilter()
        {
            return DevUtils.GetFilter(SourceDesc, SourceExt);
        }
        public string GetSaveFilter()
        {
            return DevUtils.GetFilter(DestDesc, DestExt);
        }
    }


}
