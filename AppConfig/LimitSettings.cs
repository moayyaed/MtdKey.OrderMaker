using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MtdKey.OrderMaker.AppConfig
{
    public class LimitSettings
    {
        public int Users { get; set; }
        public int Forms { get; set; }
        public bool ExportExcel { get; set; }
        public int ExportSize { get; set; }
    }
}
