using BlogAPI.Data;
using BlogAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace BlogAPI.Repository
{
    public class AutorRepository : Repository<Autor>, IAutorRepository
    {
        private readonly BlogContext _context;

        public AutorRepository(BlogContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Autor> UpdateAsync(Autor entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

    }
}
