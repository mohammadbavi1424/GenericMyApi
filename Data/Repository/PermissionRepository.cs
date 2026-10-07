using Common.LifeTimeInterface;
using Data.Context;
using Data.Contract;
using Data.DTOs.Permission;
using Data.Repository.GenericRepository;
using Entities.Entities.ClaimTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Repository
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        private readonly AppDbContext _dbContext;

        public PermissionRepository(AppDbContext dbContext, IConfiguration configuration) : base(dbContext, configuration)
        {
            _dbContext = dbContext;
        }

        public async Task AddRangeAsync(List<PermissionDto> permissionDtos, bool saveNow = true)
        {

            // حذف تکراری‌ها
            var listItems = permissionDtos
                .GroupBy(x => $"{x.AreaName}-{x.ControllerName}-{x.ActionName}")
                .Select(g => g.First())
                .ToList();

            // 1) داده‌های دیتابیس
            var dbClaims = await _dbContext.Permission.ToListAsync();

            // 2) پیدا کردن موارد جدید
            var newClaims = listItems
                .Where(item => !dbClaims.Any(c =>
                    c.AreaName == item.AreaName &&
                    c.ControllerName == item.ControllerName &&
                    c.ActionName == item.ActionName))
                .Select(item => new Permission
                {
                    AreaName = item.AreaName,
                    ControllerName = item.ControllerName,
                    ActionName = item.ActionName,
                    Url = string.IsNullOrWhiteSpace(item.AreaName) || item.AreaName.Contains("NoArea") ?
                    ("api/" + item.ControllerName + "/" + item.ActionName) :
                    (item.AreaName + "/" + item.ControllerName + "/" + item.ActionName),
                    NeedAccess = item.NeedAccess,
                    ApiName = item.ActionName + " " + item.ControllerName
                })
                .ToList();

            // 3) پیدا کردن موارد حذف‌شده
            var removedClaims = dbClaims
                .Where(c => !listItems.Any(item =>
                    item.AreaName == c.AreaName &&
                    item.ControllerName == c.ControllerName &&
                    item.ActionName == c.ActionName))
                .ToList();

            // 4) حذف Role های مرتبط با ClaimType
            if (removedClaims.Any())
            {
                var removedIds = removedClaims.Select(c => c.Id).ToList();

                var relatedRoles = await _dbContext.PermissionRole
                    .Where(r => removedIds.Contains(r.PermissionId)).ToListAsync();

                //foreach (var relatedRole in relatedRoles)
                //{
                //}

                _dbContext.RemoveRange(relatedRoles);
                base.UpdateRange(removedClaims);
            }


            if (newClaims.Any())
                base.AddRange(newClaims.Distinct().ToList(), false);

            await _dbContext.SaveChangesAsync();

        }
    }
}
