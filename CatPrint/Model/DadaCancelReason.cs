
using CatPrint.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatPrint.Model
{
    /// <summary>
    /// 达达取消订单原因
    /// </summary>
    public class DadaCancelReason : BaseEntity
    {
        /// <summary>
        /// 取消原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public int FlagId { get; set; }

    }
}
