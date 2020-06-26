using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentNetworkSystem.Models.ViewModels
{
    public class ProjectsViewModel
    {
        public Filters Filters { get; set; } = new Filters();

    }
    public class Filters
    {
        public string WasSuccess { get; set; } = "null";
        public string Active { get; set; } = "null";
        public string GroupId { get; set; } = "null";

        public bool? GetWasSuccess()
        {
            return bool.TryParse(WasSuccess, out bool tmp) ? (bool?)tmp : null;
        }

        public bool? GetActive()
        {
            return bool.TryParse(Active, out bool tmp) ? (bool?)tmp : null;
        }

        public int? GetGroupId()
        {
            return int.TryParse(GroupId, out int tmp) ? (int?)tmp : null;
        }
    }
}
