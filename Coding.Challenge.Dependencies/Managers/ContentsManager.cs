using Coding.Challenge.Dependencies.Database;
using Coding.Challenge.Dependencies.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Coding.Challenge.Dependencies.Managers
{
    public class ContentsManager : IContentsManager
    {
        private readonly IDatabase<Content?, ContentDto?> _database;
        private readonly IBancoSqlServer _bancoSqlServer;
        private readonly ILogger<ContentsManager> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper<Content?, ContentDto> _mapper;
        public ContentsManager(IDatabase<Content?, ContentDto> database, ILogger<ContentsManager> logger, IBancoSqlServer bancoSqlServer, IMemoryCache memoryCache, IMapper<Content?, ContentDto> mapper)
        {
            _database = database;
            _logger = logger;
            _bancoSqlServer = bancoSqlServer;
            _memoryCache = memoryCache;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Content?>> GetManyContents()
        {
            try
            {
                var content = _memoryCache.Get("content");
                if (content is null)
                {
                    var contents = await _database.ReadAll();
                    //return _bancoSqlServer.GetManyContents(); //Novo banco de dados retornar para a tela

                    _memoryCache.Set("content", contents, TimeSpan.FromMinutes(10)); // Armazena no cache por 10 minutos
                    return contents;
                }
                else
                {
                    return (IEnumerable<Content?>)content;
                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Aconteceu um erro: {ex.Message} dia: {DateTime.Now}");
                return null;
            }

        }
        public async Task<IEnumerable<Content?>> GetFilteredContents(string? titleFilter, string? genreFilter)
        {
            try
            {
                var content = _memoryCache.Get("content");
                if (content is null)
                {
                    var contents = await _database.ReadAll();
                    //return _bancoSqlServer.GetManyContents(); //Novo banco de dados retornar para a tela

                    _memoryCache.Set("content", contents, TimeSpan.FromMinutes(10));


                    var filteredContents = contents
                  .Where(c =>
                      (string.IsNullOrEmpty(titleFilter) || c.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase)) &&
                      (string.IsNullOrEmpty(genreFilter) || c.GenreList.Contains(genreFilter, StringComparer.OrdinalIgnoreCase)))
                  .ToList();

                    return filteredContents;
                }
                else
                {
                    var filteredContents = ((IEnumerable<Content?>)content)
                 .Where(c =>
                     (string.IsNullOrEmpty(titleFilter) || c.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase)) &&
                     (string.IsNullOrEmpty(genreFilter) || c.GenreList.Contains(genreFilter, StringComparer.OrdinalIgnoreCase)))
                 .ToList();

                    return filteredContents;
                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Aconteceu um erro: {ex.Message} dia: {DateTime.Now}");
                return null;
            }
        }

        public async Task<Content?> CreateContent(ContentDto content)
        {
            try
            {
                var cachedContent = await _database.Create(content);
                //return _bancoSqlServer.CreateContent(content); //Novo Banco de dados jogar para tela

                _memoryCache.Set(cachedContent.Id, cachedContent, TimeSpan.FromMinutes(10)); // Armazena no cache por 10 minutos
                return cachedContent;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Aconteceu um erro: {ex.Message} dia: {DateTime.Now}");
                return null;
            }

        }

        public async Task<Content?> GetContent(Guid id)
        {
            try
            {
                if (!_memoryCache.TryGetValue(id, out Content? cachedContent))
                {
                    var content = _memoryCache.Get("content");
                    if (content != null)
                    {
                        var contentData = (IEnumerable<Content?>)content;

                        foreach (var item in contentData)
                        {
                            if (item.Id == id)
                            {
                                return item;
                            }
                        }

                        //return _bancoSqlServer.GetContent(id);//Novo Banco de dados jogar para tela
                        cachedContent = await _database.Read(id);

                    }
                    
                    if (cachedContent != null)
                    {
                        _memoryCache.Set(cachedContent.Id, cachedContent, TimeSpan.FromMinutes(10)); // Armazena no cache por 10 minutos
                    }
                }

                return cachedContent;
                

            }
            catch (Exception ex)
            {
                _logger.LogError($"Aconteceu um erro: {ex.Message} dia: {DateTime.Now}");
                return null;
            }

        }

        public Task<Content?> UpdateContent(Guid id, ContentDto content)
        {
            try
            {
                if (_memoryCache.TryGetValue(id, out Content? cachedContent))
                {
                    var updatedItem = _mapper.Patch(cachedContent!, content);
                    _memoryCache.Set(id, updatedItem, TimeSpan.FromMinutes(10)); // Armazena no cache por 10 minutos
                }
                //return _bancoSqlServer.UpdateContent(id, content); //Novo Banco de dados jogar para tela
                return _database.Update(id, content);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Aconteceu um erro: {ex.Message} dia: {DateTime.Now}");
                return null;
            }

        }

        public Task<Guid> DeleteContent(Guid id)
        {
            try
            {
                if (_memoryCache.TryGetValue(id, out _))
                {
                    _memoryCache.Remove(id);
                }
                var content = _memoryCache.Get("content");
               
                return _database.Delete(id);
                //return _bancoSqlServer.DeleteContent(id); //Novo Banco de dados jogar para tela

            }
            catch (Exception ex)
            {
                _logger.LogError($"Aconteceu um erro: {ex.Message} dia: {DateTime.Now}");
                return null;
            }

        }

        public async Task<Content?> AddGenreById(Guid id, IEnumerable<string> genres)
        {
            var content = await GetContent(id);
            if (content == null)
            {
                return null;
            }

            try
            {
                var updatedGenres = content.GenreList.Concat(genres).Distinct().ToList();
                var updatedContentDto = new ContentDto(
                    title: content.Title,
                    subTitle: content.SubTitle,
                    description: content.Description,
                    imageUrl: content.ImageUrl,
                    duration: content.Duration,
                    startTime: content.StartTime,
                    endTime: content.EndTime,
                    genreList: updatedGenres
                );

                var updatedItem = _mapper.Patch(content!, updatedContentDto);
                _memoryCache.Set(id, updatedItem, TimeSpan.FromMinutes(10));
                return await _database.Update(id, updatedContentDto);
                //return _bancoSqlServer.UpdateContent(id, updatedContentDto).Result; //Novo Banco de dados jogar para tela
            }
            catch (Exception ex)
            {

                _logger.LogError($"Aconteceu um erro: {ex.Message} dia: {DateTime.Now}");
                return null;
            }
        }

        public async Task<Content?> RemoveGenreById(Guid id, IEnumerable<string> genres)
        {
            var content = await GetContent(id);
            if (content == null)
            {
                return null;
            }

            try
            {
                var updatedGenres = content.GenreList.Except(genres).ToList();
                var updatedContentDto = new ContentDto(
                    title: content.Title,
                    subTitle: content.SubTitle,
                    description: content.Description,
                    imageUrl: content.ImageUrl,
                    duration: content.Duration,
                    startTime: content.StartTime,
                    endTime: content.EndTime,
                    genreList: updatedGenres
                );

                var updatedItem = _mapper.Patch(content!, updatedContentDto);
                _memoryCache.Set(id, updatedItem, TimeSpan.FromMinutes(10));
                return await _database.Update(id, updatedContentDto);
                //var newUpdateMongoData = _bancoSqlServer.UpdateContent(id, updatedContentDto).Result; //Novo Banco de dados jogar para tela

            }
            catch (Exception ex)
            {
                _logger.LogError($"Aconteceu um erro: {ex.Message} dia: {DateTime.Now}");
                return null;
            }
        }

    }
}
