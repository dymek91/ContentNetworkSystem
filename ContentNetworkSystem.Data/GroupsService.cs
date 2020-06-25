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
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
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

        public async Task<Group> UpdateAsync(Group group)
        {
            _context.Entry(group).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return group;
        }
    }
}
