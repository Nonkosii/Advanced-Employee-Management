
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTO;

public class BaseDepartmentDTO
{
    public int Id { get; set; }
    [Required, MaxLength(60), MinLength(3)]
    public string? Name { get; set; }
}
