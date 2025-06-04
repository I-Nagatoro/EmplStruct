using System;
using System.Collections.Generic;
using System.Linq;

namespace EmplStruct.Models;

public partial class Department
{
    public int Departmentid { get; set; }

    public string Departmentname { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<DepartmentSubdivision> DepartmentSubdivisions { get; set; } = new List<DepartmentSubdivision>();
    
    public IEnumerable<object> Items => DepartmentSubdivisions.Cast<object>();
}
