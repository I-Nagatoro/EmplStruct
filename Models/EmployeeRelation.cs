using System;
using System.Collections.Generic;

namespace EmplStruct.Models;

public partial class EmployeeRelation
{
    public int Relationid { get; set; }

    public int Employeeid { get; set; }

    public int? Managerid { get; set; }

    public int? Assistantid { get; set; }

    public virtual Employee? Assistant { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Employee? Manager { get; set; }
}
