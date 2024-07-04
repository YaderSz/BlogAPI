using SharedModels;

namespace BlogAPI.Repository.IRepository
{
    public interface IAutorRepository : IRepository<Autor>
    {
        Task<Autor> UpdateAsync(Autor entity);
    }
}
