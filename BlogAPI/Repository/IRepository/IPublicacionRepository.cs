using SharedModels;

namespace BlogAPI.Repository.IRepository
{
    public interface IPublicacionRepository : IRepository<Publicacion>
    {
        Task<Publicacion> UpdateAsync(Publicacion entity);
    }
}
