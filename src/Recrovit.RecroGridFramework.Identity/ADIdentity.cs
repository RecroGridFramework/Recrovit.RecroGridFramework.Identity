using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Recrovit.RecroGridFramework;
using Recrovit.RecroGridFramework.Data;
using Recrovit.RecroGridFramework.Identity.Models;
using Recrovit.RecroGridFramework.Security;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Recrovit.RecroGridFramework.Identity
{
    public class ADIdentity
    {
        public ADIdentity(ILogger logger) : this(logger, ClaimTypes.Name) { } //ClaimTypes.PrimarySid
        public ADIdentity(ILogger logger, string userIdMode, int cacheSeconds = 600)
        {
            this.UserIdMode = userIdMode;
            this._cache = new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromSeconds(cacheSeconds) });
            this.Logger = logger;
        }

        public readonly string UserIdMode;
        private readonly MemoryCache _cache;
        private readonly ILogger Logger;

        public virtual string GetCurrentUserId(HttpContext context)
        {
            string userId = context.User.FindFirstValue(UserIdMode);
            var rgctx = new RecroGridContext(context);
            var key = $"{rgctx.RGSessionId}-{userId}";
            bool ok = _cache.GetOrCreate(key, (k) =>
            {
                bool ret = InitUser(context, userId);
                if (ret)
                {
                    ret = InitUserSession(rgctx, userId);
                }
                return ret;
            });
            if (!ok)
            {
                _cache.Remove(key);
            }
            return userId;
        }

        public virtual bool InitUser(HttpContext context, string userId)
        {

            try
            {
                if (RGDataContext.IsDatabaseInitialized)
                {
#if DEBUG
                    var name1 = context.User.Identity.Name;
                    var nameIdentifier = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var admin = context.User.IsInRole($"{Environment.UserDomainName}\\domain admins");
#endif
                    using (var ctx = context.RequestServices.GetService(typeof(IdentityDbContextBase)) as IdentityDbContextBase)
                    {
                        var sid = context.User.FindFirstValue(ClaimTypes.PrimarySid);
                        var name = context.User.FindFirstValue(ClaimTypes.Name);
                        var wi = context.User.Identity as WindowsIdentity;

                        var user = ctx.RGFUser.Include(e => e.UserRole).SingleOrDefault(e => e.UserId.ToLower() == userId.ToLower());
                        string username;
                        PrincipalContext pc = new PrincipalContext(ContextType.Machine);
                        UserPrincipal usr = UserPrincipal.FindByIdentity(pc, IdentityType.Sid, sid);
                        if (usr != null)
                        {
                            username = string.IsNullOrEmpty(usr.DisplayName) ? usr.Name : usr.DisplayName;
                        }
                        else
                        {
                            pc = new PrincipalContext(ContextType.Domain, name.Split('\\').First());
                            usr = UserPrincipal.FindByIdentity(pc, IdentityType.Sid, sid);
                            username = string.IsNullOrEmpty(usr.DisplayName) ? usr.Name : usr.DisplayName;
                        }
                        /*if (string.IsNullOrEmpty(username))
                        {
                            username = name.Split('\\').Last();
                        }*/
                        if (user == null)
                        {
                            user = new Models.RGFUser()
                            {
                                UserId = userId,//a kapott userId-t kell használni mert paraméterezés szerint SID is lehet a userId-ben
                                UserName = username
                            };
                            ctx.Add(user);
                            var r = ctx.RGFRole.SingleOrDefault(e => e.RoleName == "Users");
                            if (r != null)
                            {
                                user.UserRole.Add(new RGFUserRole() { Role = r });
                            }
                            Logger.LogInformation("InitUser: {0}, {1}", userId, username);
                            ctx.SaveChanges();
                        }
                        else
                        {
                            user.UserName = username;
                            if (user.Language != null)
                            {
                                Recrovit.RecroGridFramework.RecroDict.SetSessionLanguage(context, user.Language);
                            }
                        }

                        if (wi?.Groups != null)
                        {
                            var groups = new List<string>();
                            foreach (var group in wi.Groups)
                            {
                                try
                                {
                                    string g = group.Translate(typeof(NTAccount)).ToString();
                                    groups.Add(g);
                                }
                                catch { }
                            }

                            var roles = ctx.RGFRole.ToList();
                            foreach (var item in groups.Where(e => roles.Select(e => e.RoleId).Contains(e) == false))
                            {
                                ctx.Add(new RGFRole()
                                {
                                    RoleId = item,
                                    RoleName = item,
                                    Source = "AD"
                                });
                            }
                            ctx.SaveChanges();

                            int count = 0;
                            foreach (var item in user.UserRole.Where(e => e.Role.Source == "AD" && groups.Contains(e.RoleId) == false).ToList())
                            {
                                //kitörölni az elavultakat
                                ctx.Remove(item);
                                count++;
                            }
                            foreach (var item in groups.Where(e => user.UserRole.Select(r => r.RoleId).Contains(e) == false).ToList())
                            {
                                //hozzáadni az újakat
                                ctx.Add(new RGFUserRole()
                                {
                                    UserId = user.UserId,
                                    RoleId = item
                                });
                                count++;
                            }
                            if (count > 0)
                            {
                                Logger.LogInformation("InitRoles: {0} => {1} Roles changed", userId, count);
                            }
                            ctx.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "InitUser");
            }
            return false;
        }

        public virtual bool InitUserSession(RecroGridContext context, string userId) { return true; }
    }
}
