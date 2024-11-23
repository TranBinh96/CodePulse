using CodePulse.API.Models.Domain;

namespace CodePulse.API.Repositories.Interface
{
    public interface ICategortRepository
    {
        Task<Category> CreateAsync(Category category);
    }
}
