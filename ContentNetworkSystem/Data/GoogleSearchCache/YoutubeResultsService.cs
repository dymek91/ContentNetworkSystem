using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNetworkSystem.Models.GoogleSearchCache;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace ContentNetworkSystem.Data.GoogleSearchCache
{ 
    public interface IYoutubeResultsService
    {
        Task<List<YoutubeResult>> GetAsync();
        Task<YoutubeResult> GetAsync(int itemId); 
        Task<YoutubeResult> AddAsync(YoutubeResult item);
        Task AddRangeAsync(List<YoutubeResult> items);
        Task<YoutubeResult> UpdateAsync(YoutubeResult item);
        Task DeleteAsync(YoutubeResult item);
        Task DeleteRangeAsync(List<YoutubeResult> items);
    }
    public class YoutubeResultsService : IYoutubeResultsService
    {
        private readonly ContentNetworkSystemContext _context;

        public YoutubeResultsService(ContentNetworkSystemContext context)
        {
            _context = context;
        }

        public async Task<YoutubeResult> AddAsync(YoutubeResult item)
        {
            await _context.YoutubeResults.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task AddRangeAsync(List<YoutubeResult> items)
        {
            await _context.YoutubeResults.AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(YoutubeResult item)
        {
            _context.YoutubeResults.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<YoutubeResult> items)
        {
            _context.YoutubeResults.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<List<YoutubeResult>> GetAsync()
        {
            return await _context.YoutubeResults.ToListAsync();
        }

        public async Task<YoutubeResult> GetAsync(int itemId)
        {
            var item = await _context.YoutubeResults.FindAsync(itemId);
            if (item == null)
            {
                return null;
            }

            return item;
        }

        public async Task<YoutubeResult> UpdateAsync(YoutubeResult item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
