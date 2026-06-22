using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Repositories;

namespace NguyenCongNhatMVC.Services;

public class TagService : ITagService
{
    private readonly IRepository<Tag> _tags;
    private readonly IRepository<NewsTag> _newsTags;

    public TagService(IRepository<Tag> tags, IRepository<NewsTag> newsTags)
    {
        _tags = tags;
        _newsTags = newsTags;
    }

    public Task<List<Tag>> AllAsync() => _tags.Query().OrderBy(t => t.TagName).ToListAsync();

    public Task<List<Tag>> SearchAsync(string? keyword)
    {
        var query = _tags.Query().OrderBy(t => t.TagName).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(t => t.TagName.Contains(keyword) || (t.Note != null && t.Note.Contains(keyword)));
        }
        return query.ToListAsync();
    }

    public Task<Tag?> GetAsync(int id) => _tags.Query().FirstOrDefaultAsync(t => t.TagID == id);

    public async Task SaveAsync(Tag tag)
    {
        if (tag.TagID == 0)
        {
            await _tags.AddAsync(tag);
        }
        else
        {
            _tags.Update(tag);
        }
        await _tags.SaveAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await _newsTags.AnyAsync(nt => nt.TagID == id))
        {
            return false;
        }

        var tag = await _tags.GetByIdAsync(id);
        if (tag == null)
        {
            return false;
        }

        _tags.Delete(tag);
        await _tags.SaveAsync();
        return true;
    }
}
