using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Model;

namespace DocumentManagementSystem.Interface
{
    public interface IDocManagementSystem
    {
       Task<DocManagementSystem?> GetDocument(QueryObject query);
    }
}
