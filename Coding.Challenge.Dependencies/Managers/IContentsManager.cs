using Coding.Challenge.Dependencies.Models;

namespace Coding.Challenge.Dependencies.Managers
{
    public interface IContentsManager
    {
        Task<IEnumerable<Content?>> GetManyContents();
        Task<IEnumerable<Content?>> GetFilteredContents(string? titleFilter, string? genreFilter);
        Task<Content?> CreateContent(ContentDto content);
        Task<Content?> GetContent(Guid id);
        Task<Content?> UpdateContent(Guid id, ContentDto content);
        Task<Guid> DeleteContent(Guid id);
        Task<Content?> AddGenreById(Guid id, IEnumerable<string> genres);
        Task<Content?> RemoveGenreById(Guid id, IEnumerable<string> genres);
    }
}
