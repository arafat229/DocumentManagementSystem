using DocumentManagementSystem.Data;
using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Interface;
using DocumentManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagementSystem.Repository
{
    public class DocManagementSystemRepository : IDocManagementSystem
    {
        private readonly ApplicationDBContext _context;
        public DocManagementSystemRepository(ApplicationDBContext context)
        {
            _context = context;   
        }

        public  async Task<DocManagementSystem?> GetDocument(QueryObject query)
        {
            var document = _context.DocManagementSystems.AsQueryable();
            if (query.Id != null)
            {
                document = _context.DocManagementSystems.Where(x => x.Id == query.Id);

            }

            if (!string.IsNullOrWhiteSpace(query.Content))
            {

                document = _context.DocManagementSystems.Where(x => x.Content.Contains(query.Content));
            }

            if (!string.IsNullOrWhiteSpace(query.Author))
            {

                document = _context.DocManagementSystems.Where(x => x.Author.Contains(query.Author));
            }
            if (query.Price!=null)
            {

                document = _context.DocManagementSystems.Where(x=>x.Price == query.Price);
            }

            if (!string.IsNullOrWhiteSpace(query.Tilte))
            {

                document = _context.DocManagementSystems.Where(x => x.Tilte.Contains(query.Tilte));
            }

          
            if (query.CreationDate != null)
            {

                document = _context.DocManagementSystems.Where(x => x.CreationDate == query.CreationDate);
            }


            if (document == null)
            {
                return null;

            }
            return await document.FirstOrDefaultAsync();

        }

        
    }
}
