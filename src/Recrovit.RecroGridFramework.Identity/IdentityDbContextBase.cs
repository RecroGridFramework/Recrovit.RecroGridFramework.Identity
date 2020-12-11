using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
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
    public class IdentityDbContextBase : DbContext
    {
        public IdentityDbContextBase() { }
        public IdentityDbContextBase(string connectionString)
        {
            this.ConnectionString = connectionString;
            if (DefaultConnectionString == null)
            {
                DefaultConnectionString = connectionString;
            }
        }
        public IdentityDbContextBase(DbContextOptions options) : base(options) { }

        public DbSet<RGFUser> RGFUser { get; set; }
        public DbSet<RGFRole> RGFRole { get; set; }
        public DbSet<RGFUserRole> RGFUserRole { get; set; }

        public static string DefaultConnectionString { get; set; } = null;

        protected string ConnectionString { get; set; } = null;
        protected DbConnection Connection { get; set; } = null;
        protected int SQLTimeout { get; set; } = -1;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbType = RGDataContext.ConnectionNameDBType;
                switch (dbType)
                {
                    case DBTypeEnum.SQLServer:
                        if (this.Connection != null)
                        {
                            optionsBuilder.UseSqlServer(this.Connection);
                        }
                        else
                        {
                            optionsBuilder.UseSqlServer(DefaultConnectionString, opts =>
                            {
                                if (this.SQLTimeout != -1)
                                {
                                    opts.CommandTimeout(this.SQLTimeout);
                                }
                            });
                        }
                        break;

                    case DBTypeEnum.PostgreSQL:
                        if (this.Connection != null)
                        {
                            optionsBuilder.UseNpgsql(this.Connection);
                        }
                        else
                        {
                            optionsBuilder.UseNpgsql(DefaultConnectionString, opts =>
                            {
                                if (this.SQLTimeout != -1)
                                {
                                    opts.CommandTimeout(this.SQLTimeout);
                                }
                            });
                        }
                        break;

                    case DBTypeEnum.Oracle:
                        if (this.Connection != null)
                        {
                            optionsBuilder.UseOracle(this.Connection);
                        }
                        else
                        {
                            optionsBuilder.UseOracle(DefaultConnectionString, opts =>
                            {
                                if (this.SQLTimeout != -1)
                                {
                                    opts.CommandTimeout(this.SQLTimeout);
                                }
                            });
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
                base.OnConfiguring(optionsBuilder);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RGFUserRole>()
                .HasKey(e => new { e.UserId, e.RoleId });

            var dbType = IRGDataContextExtensions.GetDBType(this);
            switch (dbType)
            {
                case DBTypeEnum.SQLServer:
                    break;

                case DBTypeEnum.PostgreSQL:
                    modelBuilder.HasDefaultSchema("public");
                    break;

                case DBTypeEnum.Oracle:
                    break;

                default:
                    throw new NotImplementedException();
            }

            RGDataContext.InitDbTypeDependentNames(dbType, modelBuilder, dbType == DBTypeEnum.Oracle ? 30 : 63);

            this.Seed(modelBuilder, dbType);
        }

        protected virtual void Seed(ModelBuilder modelBuilder, DBTypeEnum dbType)
        {
            modelBuilder.Entity<RGFRole>().HasData(
                new RGFRole { RoleId = "1", RoleName = "Administrators", Source = "RGF" },
                new RGFRole { RoleId = "2", RoleName = "Users", Source = "RGF" }
            );
        }

        public static bool Migrate(ILogger logger, DBTypeEnum dbType, string connectionString, bool create)
        {
            if (DefaultConnectionString == null)
            {
                DefaultConnectionString = connectionString;
            }
            IdentityDbContextBase dbContext = null;
            try
            {
                switch (dbType)
                {
                    case DBTypeEnum.Oracle:
                        dbContext = new IdentityDbContextBaseMigrOracle();
                        break;

                    case DBTypeEnum.PostgreSQL:
                        dbContext = new IdentityDbContextBaseMigrPostgreSQL();
                        break;

                    case DBTypeEnum.SQLServer:
                        dbContext = new IdentityDbContextBaseMigrSQLServer();
                        break;
                }
                Migrate(logger, dbContext, create);
            }
            finally
            {
                dbContext?.Dispose();
            }
            return false;
        }
        private static bool Migrate(ILogger logger, IdentityDbContextBase dbContext, bool create)
        {
            if (create || dbContext.Database.CanConnect())
            {
                var migr = dbContext.Database.GetPendingMigrations();
                if (migr.Any())
                {
                    foreach (var item in migr)
                    {
                        logger.LogInformation("RGF Identity Migrate: {0}", item);
                    }
                    dbContext.Database.Migrate();
                }
                return true;
            }
            return false;
        }
    }

    #region Migration
    //Add-Migration rgf_identity_v1_0 -Context IdentityDbContextBaseMigrOracle -OutputDir Migrations\Oracle
    public class IdentityDbContextBaseMigrOracle : IdentityDbContextBase { }

    //Add-Migration rgf_identity_v1_0 -Context IdentityDbContextBaseMigrPostgreSQL -OutputDir Migrations\PostgreSQL
    public class IdentityDbContextBaseMigrPostgreSQL : IdentityDbContextBase { }

    //Add-Migration rgf_identity_v1_0 -Context IdentityDbContextBaseMigrSQLServer -OutputDir Migrations\SQLServer
    public class IdentityDbContextBaseMigrSQLServer : IdentityDbContextBase { }
    #endregion
}
