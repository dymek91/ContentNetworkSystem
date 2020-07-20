using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentNetworkSystem.Models.ViewModels
{
    public class ProjectsViewModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; private set; }
        public Filters Filters { get; set; } = new Filters();
         
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public ProjectsViewModel()
        {

        }
        public ProjectsViewModel(int count, int pageSize)
        {
            PageIndex = 1;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        } 

        public void ChangeCount(int count)
        {
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
        }
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
