

namespace MtdKey.OrderMaker.Core.Scripts
{
    public enum TypeRequest
    {
        GetRowCount,
        GetIds
    }

    internal static class SqlScript
    {

        public static string GetStoreIds(FilterSQLparams filter)
        {
            var storeIdsScript = new StoreIdsScript();
            var factory = new FilterScriptFactory(storeIdsScript, filter);

            if (filter.FormId == string.Empty)
                return factory.Script;

            return factory.BuildScript();
        }


    }
}
