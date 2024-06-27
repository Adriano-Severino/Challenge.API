using Coding.Challenge.API.Models;
using Coding.Challenge.Dependencies.Managers;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Challenge.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ContentController : Controller
    {
        private readonly IContentsManager _manager;
        private readonly ILogger<ContentController> _logger;
        public ContentController(IContentsManager manager, ILogger<ContentController> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        [HttpGet]
        [Obsolete("Este endpoint está obsoleto. Use o novo endpoint para filtrar conteúdos.")]
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetManyContents()
        {
            var contents = await _manager.GetManyContents().ConfigureAwait(false);

            if (!contents.Any())
                return NotFound();

            _logger.LogInformation($"Registros encontrados: {contents}");
            return Ok(contents);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredContents(string? title, string? genre)
        {
            var filteredContents = await _manager.GetFilteredContents(title, genre).ConfigureAwait(false);

            if (!filteredContents.Any())
                return NotFound();

            _logger.LogInformation($"Registros filtrados encontrados: {filteredContents}");
            return Ok(filteredContents);
        }

        [HttpGet("{id}")]
        [ResponseCache(Duration = 2)]
        public async Task<IActionResult> GetContent(Guid id)
        {
            var content = await _manager.GetContent(id).ConfigureAwait(false);

            if (content == null)
                return NotFound();

            _logger.LogInformation($"Registro encontrado: {content}");
            return Ok(content);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContent(
            [FromBody] ContentInput content
            )
        {
            var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

            _logger.LogInformation($"Registro encontrado: {createdContent}");

            return createdContent == null ? Problem() : Ok(createdContent);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateContent(
            Guid id,
            [FromBody] ContentInput content
            )
        {
            var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);
           
            _logger.LogInformation($"Registro atualizado: {updatedContent}");

            return updatedContent == null ? NotFound() : Ok(updatedContent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(
            Guid id
        )
        {
            var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);

            _logger.LogInformation($"Registro deletado: {deletedId}");

            return Ok(deletedId);
        }

        [HttpPost("{id}/genre")]
        public async Task<IActionResult> AddGenres(
            Guid id,
            [FromBody] IEnumerable<string> genres
        )
        {
            var addGenre = await _manager.AddGenreById(id, genres).ConfigureAwait(false);

            _logger.LogInformation($"genero adicionado: {genres}");
            return Ok(addGenre);
        }

        [HttpDelete("{id}/genre")]
        public async Task<IActionResult> RemoveGenres(
            Guid id,
            [FromBody] IEnumerable<string> genres
        )
        {
            var removeGenre = await _manager.RemoveGenreById(id, genres).ConfigureAwait(false);
            _logger.LogInformation($"genero removido: {genres}");
            return Ok(removeGenre);
        }

    }
}
