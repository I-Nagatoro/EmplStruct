using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia;
using EmplStruct.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EmplStruct
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly User15Context db = new();

        private IEnumerable<Department> _departments = new List<Department>();
        private IEnumerable<DepartmentSubdivision> _subdivisions = new List<DepartmentSubdivision>();
        private IEnumerable<Employee> _employees = new List<Employee>();

        public IEnumerable<Department> Departments
        {
            get => _departments;
            set
            {
                _departments = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<DepartmentSubdivision> Subdivisions
        {
            get => _subdivisions;
            set
            {
                _subdivisions = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, System.EventArgs e)
        {
            LoadDepartments();
            LoadSubdivisions();
            LoadAllEmployees();
        }

        private void LoadDepartments()
        {
            Departments = db.Departments.OrderBy(d => d.Departmentname).ToList();
        }

        private void LoadSubdivisions()
        {
            Subdivisions = db.DepartmentSubdivisions.OrderBy(s => s.Subdivisionname).ToList();
        }

        private void LoadAllEmployees()
        {
            Employees = db.Employees
                .Include(e => e.Position)
                .Include(e => e.Subdivision)
                .OrderBy(e => e.Lastname)
                .ToList();
        }

        private void FilterByDepartment(Department department)
        {
            var subIds = db.DepartmentSubdivisions
                .Where(s => s.Departmentid == department.Departmentid)
                .Select(s => s.Subdivisionid)
                .ToList();

            Employees = db.Employees
                .Include(e => e.Position)
                .Include(e => e.Subdivision)
                .Where(e => e.Subdivisionid.HasValue && subIds.Contains(e.Subdivisionid.Value))
                .OrderBy(e => e.Lastname)
                .ToList();
        }

        private void FilterBySubdivision(DepartmentSubdivision subdivision)
        {
            Employees = db.Employees
                .Include(e => e.Position)
                .Include(e => e.Subdivision)
                .Where(e => e.Subdivisionid == subdivision.Subdivisionid)
                .OrderBy(e => e.Lastname)
                .ToList();
        }

        private async void AddEmployee_Click(object? sender, RoutedEventArgs e)
        {
            var dialog = new EmployeeAddOrEditWindow();
            if (await dialog.ShowDialog<bool>(this))
            {
                LoadAllEmployees();
            }
        }

        private async void EmployeeListBox_DoubleTapped(object? sender, RoutedEventArgs e)
        {
            if (EmployeeListBox.SelectedItem is Employee selected)
            {
                var employeeFromDb = db.Employees
                    .Include(e => e.Position)
                    .Include(e => e.Subdivision)
                    .FirstOrDefault(e => e.Employeeid == selected.Employeeid);

                if (employeeFromDb is null) return;

                var dialog = new EmployeeAddOrEditWindow(employeeFromDb);
                var result = await dialog.ShowDialog<bool>(this);
                if (result)
                {
                    LoadAllEmployees();
                }
            }
        }

        private void ReloadData_Click(object? sender, RoutedEventArgs e)
        {
            LoadDepartments();
            LoadSubdivisions();
            LoadAllEmployees();
        }

        private void SearchInput_TextChanged(object? sender, TextChangedEventArgs e)
        {
            var text = SearchInput.Text?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(text))
            {
                LoadAllEmployees();
                return;
            }

            Employees = db.Employees
                .Include(e => e.Position)
                .Include(e => e.Subdivision)
                .Where(e =>
                    e.Firstname.ToLower().Contains(text) ||
                    e.Lastname.ToLower().Contains(text) ||
                    e.Position.Positionname.ToLower().Contains(text) ||
                    e.Subdivision.Subdivisionname.ToLower().Contains(text))
                .OrderBy(e => e.Lastname)
                .ToList();
        }

        private void DepartmentList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DepartmentListBox.SelectedItem is Department dep)
                FilterByDepartment(dep);
        }

        private void SubdivisionList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (SubdivisionListBox.SelectedItem is DepartmentSubdivision sub)
                FilterBySubdivision(sub);
        }
    }
}
