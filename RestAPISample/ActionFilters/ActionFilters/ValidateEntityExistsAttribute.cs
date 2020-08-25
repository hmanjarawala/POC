using ActionFilters.Contracts;
using ActionFilters.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActionFilters.ActionFilters
{
    public class ValidateEntityExistsAttribute<TEntity> : IActionFilter where TEntity : class, IEntity
    {
        private readonly MovieContext _context;

        public ValidateEntityExistsAttribute(MovieContext context)
        {
            _context = context;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var id = Guid.Empty;

            if (context.ActionArguments.ContainsKey("id"))
            {
                id = (Guid)context.ActionArguments["id"];
            }
            else
            {
                context.Result = new BadRequestObjectResult("Bad id parameter");
                return;
            }

            var entity = _context.Set<TEntity>().SingleOrDefault(x => x.Id.Equals(id));
            if(entity == null)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("entity", entity);
            }
        }
    }
}
