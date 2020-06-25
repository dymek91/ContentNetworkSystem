﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ContentNetworkSystem.Models; 
using ContentNetworkSystem.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using ContentNetworkSystem.ModelsExtensions;

namespace ContentNetworkSystem.Data
{
    public class SchedulerService
    {
        private readonly ContentNetworkSystemContext _context;
        private IProjectsService  _projectsService;
        private IServiceProvider _serviceProvider;
        private IHttpClientFactory _httpClientFactory;
        private ILogger<SchedulerService> _logger;

        public SchedulerService(ContentNetworkSystemContext context, IProjectsService projectsService, IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory, ILogger<SchedulerService> logger)
        {
            _context = context;
            _projectsService = projectsService;
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task ProcessProjectsAsync()
        {
            _logger.LogInformation("Processing Projects.");
            if (!await LockAsync())
            {
                _logger.LogWarning("Can't lock.");
                return;
            }

            try
            {
                var projects = await _projectsService.GetAsync();
                foreach (var project in projects)
                {
                    if(!project.Active)
                    {
                        continue;
                    }
                    var currDate = DateTime.UtcNow;
                    var lastPushed = project.LastPushed;
                    if(lastPushed == null)
                    {
                        lastPushed = DateTime.MinValue;
                    }
                    var frequency = project.Frequency;

                    if ((lastPushed + frequency) < currDate)
                    {
                        var content = project.Content;
                        project.WasSuccess = true;
                        try
                        {
                            await  content.PushContent(_serviceProvider, _httpClientFactory) ;
                        }
                        catch(Exception e)
                        {
                            _logger.LogError("{0}", e);
                            project.WasSuccess = false;
                        }
                        project.LastPushed = currDate;
                        await _projectsService.UpdateAsync(project);
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogError("{0}", e);
            }

            await UnlockAsync();
        }
        
        /// <summary>
        /// return true on successful lock
        /// </summary>
        /// <returns></returns>
        async Task<bool> LockAsync()
        {
            var requestId = Guid.NewGuid().GetHashCode();
            await _context.Schedulers.Where(t => t.RequestId == null).Take(1).UpdateAsync(t => new Scheduler() { RequestId = requestId });
            var scheduler = await _context.Schedulers.FirstAsync(); 
            if (scheduler.RequestId!= requestId)
            {
                return false;
            }

            return true;
        }

        async Task<bool> UnlockAsync()
        {
            await _context.Schedulers.UpdateAsync(t => new Scheduler() { RequestId = null });

            return true;
        }
    }
}