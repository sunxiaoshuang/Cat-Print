
using CatPrint.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatPrint.Model
{
    /// <summary>
    /// 达达取消订单时，产生的违约金
    /// </summary>
    public class DadaLiquidatedDamages : BaseEntity
    {
        /// <summary>
        /// 违约金
        /// </summary>
        public double Deduct_fee { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
