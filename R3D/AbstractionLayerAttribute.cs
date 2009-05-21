using System;
using System.Collections.Generic;
using System.Text;

namespace Ra2Reload
{
    /// <summary>
    /// 表示类是可以由高级脚本实现的
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class AbstractionLayerAttribute : Attribute
    {

    }



}
