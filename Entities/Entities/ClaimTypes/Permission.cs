using Entities.Entities.BaseEntities;

namespace Entities.Entities.ClaimTypes
{
    public class Permission : BaseEntity
    {
        public string ApiName { get; set; }
        public string Url { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool? NeedAccess { get; set; } = false;
    }
}
