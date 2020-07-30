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
        Task<List<Project>> GetLiteAsync();
        Task<List<Project>> GetAsync(bool? wasSuccess = null, bool? active = null, int? groupId = null, int pageIndex = 1, int? pageSize = null);
        Task<Project> GetAsync(int projectId);
        Task<Project> GetAsync(int projectId, bool getContent, bool getGroup, bool getNiche, bool getNicheDeep);
        Task<Project> AddAsync(Project project);
        Task<Project> UpdateAsync(Project project);
        Task DeleteAsync(Project project);
        Task<int> CountAsync(bool? wasSuccess = null, bool? active = null, int? groupId = null);
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
        public async Task<List<Project>> GetLiteAsync()
        {
            return await _context.Projects.ToListAsync();
        }
        public async Task<List<Project>> GetAsync(bool? wasSuccess=null,bool? active=null, int? groupId = null, int pageIndex = 1, int? pageSize = null)
        {
            //var projects = await _context.Projects.ToListAsync();
            //var projects =  _context.Projects;
            //var projects = from m in _context.Projects
            //               select m;

            var projectsQuery = from m in _context.Projects select m;

            if (wasSuccess.HasValue) projectsQuery = projectsQuery.Where(e => e.WasSuccess == wasSuccess.Value);
            if (active.HasValue) projectsQuery = projectsQuery.Where(e => e.Active == active.Value);
            if (groupId.HasValue) projectsQuery = projectsQuery.Where(e => e.GroupId == groupId.Value);
            if (pageSize.HasValue)
            {
                projectsQuery = projectsQuery.Skip((pageIndex - 1) * pageSize.Value);
                projectsQuery = projectsQuery.Take(pageSize.Value);
            }

            var projects = await projectsQuery.ToListAsync();

            foreach (var project in projects)
            {
                await _context.Entry(project).Reference(e => e.Content).LoadAsync();
                await _context.Entry(project).Reference(e => e.Group).LoadAsync();
                await _context.Entry(project).Reference(e => e.Niche).LoadAsync();
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
            await _context.Entry(project).Reference(e => e.Niche).LoadAsync();
            if (project.Niche != null)
            {
                await _context.Entry(project.Niche).Collection(e => e.Keywords).LoadAsync();
                await _context.Entry(project.Niche).Collection(e => e.YoutubeResults).LoadAsync();
                await _context.Entry(project.Niche).Collection(e => e.ImagesResults).LoadAsync();
            }

            return project;
        }

        public async Task<Project> GetAsync(int projectId, bool getContent, bool getGroup, bool getNiche, bool getNicheDeep)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return null;
            }
            if (getContent) await _context.Entry(project).Reference(e => e.Content).LoadAsync();
            if (getGroup) await _context.Entry(project).Reference(e => e.Group).LoadAsync();
            if (getNiche)
            {
                await _context.Entry(project).Reference(e => e.Niche).LoadAsync();
                if (project.Niche != null && getNicheDeep)
                {
                    await _context.Entry(project.Niche).Collection(e => e.Keywords).LoadAsync();
                    await _context.Entry(project.Niche).Collection(e => e.YoutubeResults).LoadAsync();
                    await _context.Entry(project.Niche).Collection(e => e.ImagesResults).LoadAsync();
                }
            }

            return project;
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return project;
        }
        public async Task<int> CountAsync(bool? wasSuccess = null, bool? active = null, int? groupId = null)
        {
            var projectsQuery = from m in _context.Projects select m;

            if (wasSuccess.HasValue) projectsQuery = projectsQuery.Where(e => e.WasSuccess == wasSuccess.Value);
            if (active.HasValue) projectsQuery = projectsQuery.Where(e => e.Active == active.Value);
            if (groupId.HasValue) projectsQuery = projectsQuery.Where(e => e.GroupId == groupId.Value);

            return await projectsQuery.CountAsync();
        }
    }
}
