﻿using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Tasks;
using Nop.Core.Domain.Tasks;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Tasks;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public partial class ScheduleTaskController : BaseNopController
	{
		#region Fields

        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

		#endregion

		#region Constructors

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService, 
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService)
		{
            this._scheduleTaskService = scheduleTaskService;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
		}

		#endregion 

        #region Utility

        [NonAction]
        protected ScheduleTaskModel PrepareScheduleTaskModel(ScheduleTask task)
        {
            var model = new ScheduleTaskModel()
                            {
                                Id = task.Id,
                                Name = task.Name,
                                Seconds = task.Seconds,
                                Enabled = task.Enabled,
                                StopOnError = task.StopOnError,
                                LastStartUtc = task.LastStartUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(task.LastStartUtc.Value, DateTimeKind.Utc).ToString("G") : "",
                                LastEndUtc = task.LastEndUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(task.LastEndUtc.Value, DateTimeKind.Utc).ToString("G") : "",
                                LastSuccessUtc = task.LastSuccessUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(task.LastSuccessUtc.Value, DateTimeKind.Utc).ToString("G") : "",
                            };
            return model;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            return View();
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            var models = _scheduleTaskService.GetAllTasks(true)
                .Select(PrepareScheduleTaskModel)
                .ToList();
            var model = new GridModel<ScheduleTaskModel>
            {
                Data = models,
                Total = models.Count
            };

		    return new JsonResult
			{
				Data = model
			};
		}

        [GridAction(EnableCustomBinding=true)]
        public ActionResult TaskUpdate(ScheduleTaskModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();
            
            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var scheduleTask = _scheduleTaskService.GetTaskById(model.Id);
            if (scheduleTask == null)
                return Content("Schedule task cannot be loaded");

            scheduleTask.Name = model.Name;
            scheduleTask.Seconds = model.Seconds;
            scheduleTask.Enabled = model.Enabled;
            scheduleTask.StopOnError = model.StopOnError;
            _scheduleTaskService.UpdateTask(scheduleTask);
            
            return List(command);
        }

        public ActionResult RunNow(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            try
            {
                var scheduleTask = _scheduleTaskService.GetTaskById(id);
                if (scheduleTask == null)
                    throw new Exception("Schedule task cannot be loaded");

                var task = new Task(scheduleTask);
                //ensure that the task is enabled
                task.Enabled = true;
                task.Execute(true);
                SuccessNotification(_localizationService.GetResource("Admin.System.ScheduleTasks.RunNow.Done"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }
        #endregion
    }
}
