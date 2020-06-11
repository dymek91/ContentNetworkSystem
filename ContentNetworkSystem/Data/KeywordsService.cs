using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNetworkSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentNetworkSystem.Data
{
    public interface IKeywordsService
    {
        Task<List<Keyword>> GetAsync();
        Task<Keyword> GetAsync(int keywordId);
        Task<Keyword> AddAsync(Keyword keyword);
        Task AddRangeAsync(HashSet<Keyword> keywords);
        Task AddRangeAsync(HashSet<Keyword> keywords, int nicheId);
        Task DeleteAsync(Keyword keyword);
        Task<Keyword> UpdateAsync(Keyword keyword);
    }
    public class KeywordsService : IKeywordsService
    {
        private readonly ContentNetworkSystemContext _context;

        public KeywordsService(ContentNetworkSystemContext context)
        {
            _context = context;
        }

        public async Task<Keyword> AddAsync(Keyword keyword)
        {
            if (!await _context.Keywords.Where(e => e.NicheId == keyword.NicheId && e.Name == keyword.Name).AnyAsync())
            {
                await _context.Keywords.AddAsync(keyword);
                await _context.SaveChangesAsync();
            }
            return keyword;
        }

        public async Task AddRangeAsync(HashSet<Keyword> keywords)
        {
            foreach (var keyword in keywords)
            {
                if (!await _context.Keywords.Where(e => e.NicheId == keyword.NicheId && e.Name == keyword.Name).AnyAsync())
                {
                    await _context.Keywords.AddAsync(keyword);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(HashSet<Keyword> keywords, int nicheId)
        {
            List<Keyword> keywordsToCompare = await _context.Keywords.Where(e => e.NicheId == nicheId).ToListAsync();
            var keywordsToAdd = keywords.Where(e => !keywordsToCompare.Where(x => x.Name == e.Name).Any());
            if (keywordsToAdd.Count() == 0)
            {
                return;
            }
            await _context.Keywords.AddRangeAsync(keywordsToAdd);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(Keyword keyword)
        {
            _context.Keywords.Remove(keyword);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Keyword>> GetAsync()
        {
            return await _context.Keywords.ToListAsync();
        }

        public async Task<Keyword> GetAsync(int keywordId)
        {
            return await _context.Keywords.FindAsync(keywordId);
        }

        public async Task<Keyword> UpdateAsync(Keyword keyword)
        {
            _context.Entry(keyword).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return keyword;
        }
    }
}
