using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Recrovit.RecroGridFramework;
using Recrovit.RecroGridFramework.Data;
using Recrovit.RecroGridFramework.Identity.Models;
using Recrovit.RecroGridFramework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recrovit.RecroGridFramework.Identity.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        public AuthController(ILogger<AuthController> logger, IdentityDbContextBase dbContext)
        {
            this.Logger = logger;
            this.RGDbContext = new RecroGridDbContext(dbContext);
        }

        protected readonly ILogger Logger;
        protected IRGDataContext RGDbContext { get; }

        protected string ViewPath { get; set; } = "~/Areas/RGF/Views/Shared/RGFDefaultView.cshtml";

        public async Task<ActionResult> Users()
        {
            var rg = await RecroGrid.CreateAsync<RGFUser>(new RecroGridContext(this.HttpContext, RGDbContext));
            return View(ViewPath, rg);
        }

        public async Task<ActionResult> Roles()
        {
            var rg = await RecroGrid.CreateAsync<RGFRole>(new RecroGridContext(this.HttpContext, RGDbContext));
            return View(ViewPath, rg);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChangeLanguage(LanguageViewModel model)
        {
            string langId = model?.Language.ToLower();
            if (RecroDict.ValidLanguages.ContainsKey(langId))
            {
                var prev = RecroDict.SetSessionLanguage(this.HttpContext, langId);
                if (prev != langId && RecroSec.IsAuthenticated(this.HttpContext))
                {
                    var userId = RecroSec.GetCurrentUserId(this.HttpContext);
                    if (userId != RecroSec.AnonymousId)
                    {
                        var ctx = this.RGDbContext.GetContext<IdentityDbContextBase>();
                        var user = ctx.RGFUser.SingleOrDefault(e => e.UserId == userId);
                        if (user != null)
                        {
                            user.Language = langId;
                            await ctx.SaveChangesAsync();
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(model?.ReturnUrl) || !Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect("/");
            }
            return LocalRedirect(model.ReturnUrl);
        }
    }
}