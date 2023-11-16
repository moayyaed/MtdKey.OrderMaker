using Microsoft.AspNetCore.Mvc.Formatters;
using MtdKey.OrderMaker.Core.Scripts.StoreIds;
using System.Collections.Generic;

namespace MtdKey.OrderMaker.Core.Scripts
{
    public class StoreIdsScript : IScriptFile
    {
        public string ResourceName => typeof(StoreIdsScript).Namespace + ".StoreIds.StoreIdsScript.sql";

        public IEnumerable<FilterHandler> FilterHandlers => new List<FilterHandler>()
        {
               new FilterFormId(),
               new FilterStoreId(),
               new FilterSorting(),
               new FilterDateRange(),
               new FilterSearchText(),
               new FilterSearchNumber(),
               new FilterOwner(),
               new FilterFileds(),
               new FilterTypeRequest(),
        };
    }
}
