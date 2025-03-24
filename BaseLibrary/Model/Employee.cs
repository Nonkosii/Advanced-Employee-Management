

namespace BaseLibrary.Model;

public class Employee : Base
{
    public string CivilId { get; set; } = string.Empty;
    public string FileNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TeleNumber { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public string Other { get; set; } = string.Empty;

    public int DepartmentID { get; set; }
    public Department? Department { get; set; }

    public int GeneralDepartmentID { get; set; }
    public GeneralDepartment? GeneralDepartment { get; set; }

    public int BranchID { get; set; }
    public Branch? Branch { get; set; }

    public int TownID { get; set; }
    public Town? Town { get; set; }



}
