﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MtdKey.OrderMaker.Entity
{
    public partial class MtdFilterOwner
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }

        public virtual MtdFilter IdNavigation { get; set; }
    }
}
