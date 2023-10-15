namespace MtdKey.OrderMaker.Core
{
    public class IndexItem
    {        
        public string StoreId { get; set; }
        public string FieldId { get; set; }
        public int FieldType { get; set; }
        public object Value { get; set; }
        public long SortIndex { get; set; }
    }
}
