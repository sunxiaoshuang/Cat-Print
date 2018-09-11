using System;
using System.Collections.Generic;
using System.Text;

namespace CatPrint.Model
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
    }
}
