using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNetworkSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentNetworkSystem.Data
{
    public interface INichesService
    {
        Task<List<Niche>> GetAsync(int? textGenerationCategoryId = null, int? textGenerationLowQCategoryId = null);
        Task<Niche> GetAsync(int nicheId);
        Task<Niche> AddAsync(Niche niche);
        Task<Niche> UpdateAsync(Niche niche);
        Task DeleteAsync(Niche group);
        Task<List<Keyword>> GetKeywordsAsync(Niche niche);
        Task<List<Keyword>> GetKeywordsAsync(int nicheId);
    }
    public class NichesService : INichesService
    {
        private readonly ContentNetworkSystemContext _context;

        public NichesService(ContentNetworkSystemContext context)
        {
            _context = context;
        }

        public async Task<Niche> AddAsync(Niche niche)
        {
            await _context.Niches.AddAsync(niche);
            await _context.SaveChangesAsync();
            return niche;
        }

        public async Task DeleteAsync(Niche group)
        {
            _context.Niches.Remove(group);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Niche>> GetAsync(int? textGenerationCategoryId = null, int? textGenerationLowQCategoryId = null)
        {
            var nichesQuery = from m in _context.Niches select m;

            if (textGenerationCategoryId.HasValue) nichesQuery = nichesQuery.Where(e => e.TextGenerationCategoryId == textGenerationCategoryId);
            if (textGenerationLowQCategoryId.HasValue) nichesQuery = nichesQuery.Where(e=>e.TextGenerationLowQCategoryId == textGenerationLowQCategoryId);

            var niches = await nichesQuery.ToListAsync();

            foreach (var niche in niches)
            {
                await _context.Entry(niche).Collection(e => e.Keywords).LoadAsync();
                await _context.Entry(niche).Collection(e => e.Projects).LoadAsync();
            }
            return niches;
        }

        public async Task<Niche> GetAsync(int nicheId)
        {
            var niche = await _context.Niches.FindAsync(nicheId);
            if (niche == null)
            {
                return null;
            }

            await _context.Entry(niche).Collection(e => e.Keywords).LoadAsync();
            await _context.Entry(niche).Collection(e => e.Projects).LoadAsync();
            await _context.Entry(niche).Collection(e => e.YoutubeResults).LoadAsync();
            await _context.Entry(niche).Collection(e => e.ImagesResults).LoadAsync();

            return niche;
        }

        public async Task<Niche> UpdateAsync(Niche niche)
        {
            _context.Entry(niche).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return niche;
        }

        public async Task<List<Keyword>> GetKeywordsAsync(Niche niche)
        {
            return await _context.Keywords.Where(e => e.NicheId == niche.ID).ToListAsync();
        }

        public async Task<List<Keyword>> GetKeywordsAsync(int nicheId)
        {
            return await _context.Keywords.Where(e => e.NicheId == nicheId).ToListAsync();
        }
    }
}
