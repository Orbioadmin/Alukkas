using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Orbio.Web.UI.Infrastructure.Cache;
using Orbio.Services.Catalog;
using Orbio.Web.UI.Models.Catalog;

namespace Orbio.Web.UI.Controllers
{
    /// <summary>
    /// catalog controller
    /// </summary>
    public class CatalogController : Controller
    {
        private readonly ICategoryService categoryService;
       // private readonly IWorkContext workContext;
       // private readonly IStoreContext storeContext;

        private readonly ICacheManager cacheManager;
       // private readonly CatalogSettings catalogSettings;

        /// <summary>
        /// instantiates catalog controller
        /// </summary>
        public CatalogController(ICategoryService categoryService,
            //IWorkContext workContext,
            //IStoreContext storeContext,       
            //CatalogSettings catalogSettings,          
            ICacheManager cacheManager
            )
        {
            this.categoryService = categoryService;

            //this.workContext = workContext;
            //this.storeContext = storeContext;

            //this.catalogSettings = catalogSettings;

            this.cacheManager = cacheManager;
        }

        /// <summary>
        /// action to return topmenu data
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult TopMenu()
        {
            // var customerRolesIds = workContext.CurrentCustomer.CustomerRoles
            //    .Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            //string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_MENU_MODEL_KEY, workContext.WorkingLanguage.Id,
            //    string.Join(",", customerRolesIds), storeContext.CurrentStore.Id);


            var cachedModel = cacheManager.Get(string.Format(ModelCacheEventConsumer.CATEGORY_MENU_MODEL_KEY, 1, 4, 1),
                () => PrepareCategorySimpleModels());
            
           var model = new TopMenuModel()
            {
                Categories = cachedModel,
               // RecentlyAddedProductsEnabled = catalogSettings.RecentlyAddedProductsEnabled,
                BlogEnabled =  false, //blogSettings.Enabled,
                ForumEnabled = false //forumSettings.ForumsEnabled
            };
            return PartialView(model);
        }

        public ActionResult Category(string SeName)
        {
            return View();
        }

        private IList<CategorySimpleModel> PrepareCategorySimpleModels()
        {
           return (from c in categoryService.GetTopMenuCategories()
                                select new CategorySimpleModel(c)).ToList();
        }

    }
}
