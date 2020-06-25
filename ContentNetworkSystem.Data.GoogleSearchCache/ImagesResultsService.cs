using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNetworkSystem.Models.GoogleSearchCache;
using ContentNetworkSystem.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace ContentNetworkSystem.Data.GoogleSearchCache
{ 
    public interface IImagesResultsService
    {
        Task<List<ImagesResult>> GetAsync();
        Task<ImagesResult> GetAsync(int itemId);
        Task<ImagesResult> AddAsync(ImagesResult item);
        Task AddRangeAsync(List<ImagesResult> items);
        Task<ImagesResult> UpdateAsync(ImagesResult item);
        Task DeleteAsync(ImagesResult item);
        Task DeleteRangeAsync(List<ImagesResult> items);
    }
    public class ImagesResultsService : IImagesResultsService
    {
        private readonly ContentNetworkSystemContext _context;

        public ImagesResultsService(ContentNetworkSystemContext context)
        {
            _context = context;
        }

        public async Task<ImagesResult> AddAsync(ImagesResult item)
        {
            await _context.ImagesResults.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task AddRangeAsync(List<ImagesResult> items)
        {
            await _context.ImagesResults.AddRangeAsync(items); 
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ImagesResult item)
        {
            _context.ImagesResults.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<ImagesResult> items)
        {
            _context.ImagesResults.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ImagesResult>> GetAsync()
        {
            return await _context.ImagesResults.ToListAsync();
        }

        public async Task<ImagesResult> GetAsync(int itemId)
        {
            var item = await _context.ImagesResults.FindAsync(itemId);
            if (item == null)
            {
                return null;
            }

            return item;
        }

        public async Task<ImagesResult> UpdateAsync(ImagesResult item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
