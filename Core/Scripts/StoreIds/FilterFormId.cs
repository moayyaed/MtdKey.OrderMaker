using System.Collections.Generic;

namespace MtdKey.OrderMaker.Core.Scripts.StoreIds
{
    public class FilterFormId : FilterHandler
    {
        public override string ReplaceFilter(string script, FilterSQLparams filter)
        {
            return script.Replace("[FormId]", filter.FormId.ToString());
        }
    }
}
