﻿/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using MtdKey.OrderMaker.Core;
using System;
using System.Collections.Generic;

namespace MtdKey.OrderMaker.Entity
{
    public partial class MtdStoreOwner
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public virtual MtdStore IdNavigation { get; set; }
    }
}
