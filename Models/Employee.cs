using System;
using System.Collections.Generic;

namespace EmplStruct.Models;

public partial class Employee
{
    public int Employeeid { get; set; }

    public string? Personalnumber { get; set; }

    public string Lastname { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string? Middlename { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? Mobilephone { get; set; }

    public string Workphone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Office { get; set; } = null!;

    public int Positionid { get; set; }

    public int? Subdivisionid { get; set; }

    public bool? IsManager { get; set; }

    public virtual ICollection<EmployeeRelation> EmployeeRelationAssistants { get; set; } = new List<EmployeeRelation>();

    public virtual ICollection<EmployeeRelation> EmployeeRelationEmployees { get; set; } = new List<EmployeeRelation>();

    public virtual ICollection<EmployeeRelation> EmployeeRelationManagers { get; set; } = new List<EmployeeRelation>();

    public virtual Position Position { get; set; } = null!;

    public virtual DepartmentSubdivision? Subdivision { get; set; }
}
