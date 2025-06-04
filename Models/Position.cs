using System;
using System.Collections.Generic;

namespace EmplStruct.Models;

public partial class Position
{
    public int Positionid { get; set; }

    public string Positionname { get; set; } = null!;

    public bool? Ismanager { get; set; }

    public virtual ICollection<DepartmentSubdivision> DepartmentSubdivisions { get; set; } = new List<DepartmentSubdivision>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
