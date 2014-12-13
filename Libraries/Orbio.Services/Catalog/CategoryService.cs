using System.Collections.Generic;
using Nop.Core.Domain;
using Nop.Data;
using Orbio.Core.Domain.Catalog;
using System.Linq;
using Orbio.Services.Utility;

namespace Orbio.Services.Catalog
{
    /// <summary>
    /// category service implementations
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IDbContext context;

        /// <summary>
        /// instantiates Category service type
        /// </summary>
        /// <param name="context">db context</param>
        public CategoryService(IDbContext context)
        {
            this.context = context;
        }
         /// <summary>
        /// gets topmenu categories
        /// </summary>
        /// <returns>list of category</returns>
        public List<Category> GetTopMenuCategories()
        {
            var result = context.ExecuteFunction<XmlResultSet>("usp_Catalog_GetTopMenu", null).FirstOrDefault();
            var categories = Serializer.GenericDataContractDeSerializer<List<Category>>(result.XmlResult);
            return categories;
        }
    }
}
