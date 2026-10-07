using Data.DTOs.Base;
using Entities.Entities.ClaimTypes;

namespace Data.DTOs.Permission
{
    public class PermissionDto:BaseDto<PermissionDto,Entities.Entities.ClaimTypes.Permission>
    {
        public string ApiName { get; set; }
        public string Url { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool? NeedAccess { get; set; }
    }
}
