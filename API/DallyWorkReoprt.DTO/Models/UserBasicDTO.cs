namespace DallyWorkReoprt.DTO.Models
{
    public class UserBasicDTO
    {
        public int EmployeeID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string RoleType { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int CompanyId { get; set; }
        public bool IsTenant { get; set; }
    }
}

