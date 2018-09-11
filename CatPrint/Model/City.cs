using System;
using System.Collections.Generic;
using System.Text;

namespace CatPrint.Model
{
    /// <summary>
    /// 城市名称与编码对应表
    /// </summary>
    public class City : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
