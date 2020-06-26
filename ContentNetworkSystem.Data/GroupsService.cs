using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNetworkSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentNetworkSystem.Data
{
    public interface IGroupsService
    {
        Task<List<Group>> GetAsync();
        Task<Group> GetAsync(int groupId);
        Task<Group> GetAsync(string name);
        Task<Group> AddAsync(Group group);
        Task<Group> UpdateAsync(Group group);
        Task DeleteAsync(Group group);
    }
    public class GroupsService : IGroupsService
    {  
        private readonly ContentNetworkSystemContext _context;

        public GroupsService(ContentNetworkSystemContext context)
        {
            _context = context;
        }

        public async Task<Group> AddAsync(Group group)
        {
            if (! await _context.Groups.Where(e => e.Name == group.Name).AnyAsync())
            {
                await _context.Groups.AddAsync(group); 
                await _context.SaveChangesAsync();
            }
            else
            {
                group = await GetAsync(group.Name);
            }
            return group;
        }

        public async Task DeleteAsync(Group group)
        {
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Group>> GetAsync()
        {
            return await _context.Groups.ToListAsync();
        }

        public async Task<Group> GetAsync(int groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return null;
            }

            return group;
        }

        public async Task<Group> GetAsync(string name)
        {
            Group group = null;
            if (await _context.Groups.Where(e => e.Name == name).AnyAsync())
            {
                group = await _context.Groups.Where(e => e.Name == name).FirstAsync();
            }
            return group;
        }

        public async Task<Group> UpdateAsync(Group group)
        {
            _context.Entry(group).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return group;
        }
    }
}
