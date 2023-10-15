namespace MtdKey.OrderMaker.Core
{
    public interface IStoreField
    {
        public string FieldId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
