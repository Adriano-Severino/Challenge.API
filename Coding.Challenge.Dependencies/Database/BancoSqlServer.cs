using Coding.Challenge.Dependencies.Database.Data;
using Coding.Challenge.Dependencies.Models;
using Microsoft.EntityFrameworkCore;

namespace Coding.Challenge.Dependencies.Database
{
    public class BancoSqlServer(AppDbContext context, IMapper<Content, ContentDto> _mapper) : IBancoSqlServer
    {
        public async Task<Content?> CreateContent(ContentDto contentDto)
        {
            var newGuid = Guid.NewGuid();
            var content =  _mapper.Map(newGuid, contentDto);
            await context.Contents.AddAsync(content);
            await context.SaveChangesAsync();
            return content;
            
        }
        public async Task<Guid> DeleteContent(Guid id)
        {
            var content = await GetContent(id);

            if (content != null)
            {
                context.Contents.Remove(content);
                await context.SaveChangesAsync();
                return id;
            }

            return id;
        }

        public async Task<Content?> GetContent(Guid id)
        {
           return await context.Contents.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Content?>> GetManyContents()
        {
            return await  context.Contents.AsTracking().ToListAsync();
        }

        public async Task<Content?> UpdateContent(Guid id, ContentDto contentDto)
        {
            var content = _mapper.Map(id, contentDto);

            if (content != null)
            {
                context.Contents.Update(content);
                await context.SaveChangesAsync();
                return content;
            }

            return null;
            
        }
    }
}
