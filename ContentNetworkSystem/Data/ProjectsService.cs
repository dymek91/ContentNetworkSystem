using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNetworkSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentNetworkSystem.Data
{
    public interface IProjectsService
    {
        Task<List<Project>> GetAsync();
        Task<Project> GetAsync(int projectId);
        Task<Project> AddAsync(Project project);
        Task<Project> UpdateAsync(Project project);
        Task DeleteAsync(Project project);
    }
    public class ProjectsService : IProjectsService
    {
        private readonly ContentNetworkSystemContext _context;

        public ProjectsService(ContentNetworkSystemContext context)
        {
            _context = context;
        }

        public async Task<Project> AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task DeleteAsync(Project project)
        {
            //if (project.ContentId != null)
            //{
            //    var content = await _context.Contents.FindAsync(project.ContentId);
            //    _context.Contents.Remove(content);
            //}
            
             _context.Projects.Remove(project);
             await _context.SaveChangesAsync();
            
        }

        public async Task<List<Project>> GetAsync()
        {
            var projects = await _context.Projects.ToListAsync();
            foreach(var project in projects)
            {
                await _context.Entry(project).Reference(e => e.Content).LoadAsync();
                await _context.Entry(project).Reference(e => e.Group).LoadAsync();
            }
            return projects;
        }

        public async Task<Project> GetAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return null;
            }
            await _context.Entry(project).Reference(e => e.Content).LoadAsync();
            await _context.Entry(project).Reference(e => e.Group).LoadAsync();

            return project;
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return project;
        }
    }
}
