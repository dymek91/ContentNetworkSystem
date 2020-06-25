
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using ContentNetworkSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentNetworkSystem.Data
{
    public interface IContentsService
    {
        Task<List<Content>> GetAsync();
        Task<Content> GetAsync(int contentId);
        Task<Content> AddAsync(Content content);
        Task<Content> UpdateAsync(Content content);
        Task DeleteAsync(Content content);
    }
    public class ContentsService : IContentsService
    {
        private readonly ContentNetworkSystemContext _context;

        public ContentsService(ContentNetworkSystemContext context)
        {
            _context = context;
        }

        public async Task<Content> AddAsync(Content content)
        {
            await _context.Contents.AddAsync(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task DeleteAsync(Content content)
        { 
            _context.Contents.Remove(content);
            //if(await _context.Projects.Where(e => e.ContentId == content.ID).AnyAsync())
            //{
            //    var project = await _context.Projects.Where(e => e.ContentId == content.ID).FirstAsync();
            //    project.ContentId = null;
            //}
            await _context.SaveChangesAsync();

        }

        public async Task<List<Content>> GetAsync()
        {
            return await _context.Contents.ToListAsync();
        }

        public async Task<Content> GetAsync(int contentId)
        {
            var content = await _context.Contents.FindAsync(contentId);
            if (content == null)
            {
                return null;
            }

            return content;
        }

        public async Task<Content> UpdateAsync(Content content)
        {
            _context.Entry(content).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return content;
        }
    }
}
