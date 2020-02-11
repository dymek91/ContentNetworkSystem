using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ContentNetworkSystem.Data;

namespace ContentNetworkSystem.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ContentNetworkSystemContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ContentNetworkSystemContext>>()))
            {
                context.Schedulers.RemoveRange(context.Schedulers);
                context.SaveChanges();

                if(!context.Schedulers.Any())
                {
                    var scheduler = new Scheduler { RequestId = null};
                    context.Schedulers.Add(scheduler);
                    context.SaveChanges();
                }
            }
        }
    }
}
