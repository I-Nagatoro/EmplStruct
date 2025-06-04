using System;
using System.Collections.Generic;

namespace EmplStruct.Models;

public partial class DepartmentSubdivision
{
    public int Subdivisionid { get; set; }

    public int Departmentid { get; set; }

    public string Subdivisionname { get; set; } = null!;

    public string? Description { get; set; }

    public int? Headpositionid { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Position? Headposition { get; set; }
}
