using System;
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
                var projectsLite = await _projectsService.GetLiteAsync();
                foreach (var projectLite in projectsLite)
                {
                    if(!projectLite.Active)
                    {
                        continue;
                    }
                    var currDate = DateTime.UtcNow;
                    var lastPushed = projectLite.LastPushed;
                    if(lastPushed == null)
                    {
                        lastPushed = DateTime.MinValue;
                    }
                    var frequency = projectLite.Frequency;

                    if ((lastPushed + frequency) < currDate)
                    {
                        _logger.LogInformation("Processing Project - ({0}) {1}.", projectLite.ID, projectLite.Name);
                        var project = await _projectsService.GetAsync(projectLite.ID);
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

                //RUN WORDPRESSES CRONS
                //run every 10min
                if ((DateTime.Now.Hour % 3 == 0) && (DateTime.Now.Minute == 4))
                {
                    _logger.LogInformation("Processing Projects - Running Wordpresses Crons.");
                    foreach (var projectLite in projectsLite)
                    {
                        if(projectLite.Active)
                        {
                            var project = await _projectsService.GetAsync(projectLite.ID,getContent:true,getGroup:false,getNiche:false,getNicheDeep:false);
                            if (project.Content != null)
                            {
                                var content = project.Content;
                                if (content.TypeName == "Wordpress")
                                {
                                    await RunWordpressCron(content.Url + "wp-cron.php");
                                }
                            }
                        }
                    }
                    _logger.LogInformation("Processing Projects - Running Wordpresses Crons - Finished.");
                }
            }
            catch(Exception e)
            {
                _logger.LogError("{0}", e);
            }

            await UnlockAsync();
            _logger.LogInformation("Processing Projects - Finished.");
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

        async Task RunWordpressCron(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                try
                {
                    await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                }
                catch(Exception e)
                {
                    _logger.LogError("{0} - {1}",url, e);
                }

            }
        }
    }
}
