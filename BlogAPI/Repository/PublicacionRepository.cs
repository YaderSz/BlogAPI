using BlogAPI.Data;
using BlogAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace BlogAPI.Repository
{
    public class PublicacionRepository : Repository<Publicacion>, IPublicacionRepository
    {
        private readonly BlogContext _context;

    public PublicacionRepository(BlogContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Publicacion> UpdateAsync(Publicacion entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }
}
}
