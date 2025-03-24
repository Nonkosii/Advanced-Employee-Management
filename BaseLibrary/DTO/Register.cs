using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.DTO
{
    public class Register : AccountBase
    {
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
