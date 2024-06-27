using Coding.Challenge.Dependencies.Models;

namespace Coding.Challenge.Dependencies.Database
{
    public interface IBancoSqlServer
    {
        Task<IEnumerable<Content?>> GetManyContents();
        Task<Content?> CreateContent(ContentDto content);
        Task<Content?> GetContent(Guid id);
        Task<Content?> UpdateContent(Guid id, ContentDto content);
        Task<Guid> DeleteContent(Guid id);
    }
}
