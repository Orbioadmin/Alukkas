using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbio.Core.Domain.Catalog;
 

namespace Orbio.Services.Catalog
{
    /// <summary>
    /// interface for category service
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// gets topmenu categories
        /// </summary>
        /// <returns>list of category</returns>
        List<Category> GetTopMenuCategories();
        
    }
}
