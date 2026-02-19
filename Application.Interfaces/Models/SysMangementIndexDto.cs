using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Models
{
    public class SysMangementIndexDto
    {
        public IReadOnlyList<EgxEmployeeDto>? EgxEmployeeDto { get; set; }
        public IReadOnlyList<InvUserDto>? invUserDto { get; set; }
        public InvUserDto? UserForm { get; set; }
    }
}
