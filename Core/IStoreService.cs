using System.Collections.Generic;
using System.Threading.Tasks;

namespace MtdKey.OrderMaker.Core
{
    public interface IStoreService
    {
        public Task CreateStoreAsync(StorePostRequest storeRequest);
        public Task SaveStoreAsync(StorePostRequest storeRequest);
        public Task<RequestResult> GetDocsBySQLRequestAsync(StoreDocRequest docRequest);
        public Task<DocModel> CreateEmptyDocModelAsync(StoreDocRequest request);
    }
}
