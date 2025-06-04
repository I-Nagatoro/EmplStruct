using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EmplStruct.Models;
using Microsoft.EntityFrameworkCore;

namespace EmplStruct
{
    public partial class EmployeeAddOrEditWindow : Window
    {
        private readonly Employee _employee;
        private bool _isEditing;
        private readonly User15Context _db;
        private List<DepartmentSubdivision> _subdivisions = new();
        private List<Position> _positions = new();
        private List<Employee> _assistants = new();

        public Employee Employee => _employee;

        public EmployeeAddOrEditWindow()
        {
            InitializeComponent();
            _employee = new Employee();
            _db = new User15Context();
            LoadData();
            SetEditMode(false);
        }

        public EmployeeAddOrEditWindow(Employee? employee = null)
        {
            InitializeComponent();
            _db = new User15Context();
            _employee = employee ?? new Employee();
            LoadData();
            
            LoadEmployee(_employee);
            SetEditMode(true);
        }

        private void LoadData()
        {
            _subdivisions = _db.DepartmentSubdivisions.ToList();
            _positions = _db.Positions.ToList();
            _assistants = _db.Employees
                .Where(e => e.Employeeid != _employee.Employeeid)
                .ToList();
            this.FindControl<ComboBox>("DepartmentsComboBox").ItemsSource = _subdivisions;
            this.FindControl<ComboBox>("PositionsComboBox").ItemsSource = _positions;
            this.FindControl<ComboBox>("AssistantsComboBox").ItemsSource = _assistants;
        }

        private void SetEditMode(bool isEditing)
        {
            _isEditing = isEditing;
            this.FindControl<TextBox>("LastNameTextBox").IsReadOnly = !isEditing;
            this.FindControl<TextBox>("FirstNameTextBox").IsReadOnly = !isEditing;
            this.FindControl<TextBox>("MiddleNameTextBox").IsReadOnly = !isEditing;
            this.FindControl<TextBox>("MobilePhoneTextBox").IsReadOnly = !isEditing;
            this.FindControl<TextBox>("WorkPhoneTextBox").IsReadOnly = !isEditing;
            this.FindControl<TextBox>("EmailTextBox").IsReadOnly = !isEditing;
            this.FindControl<TextBox>("OfficeTextBox").IsReadOnly = !isEditing;

            this.FindControl<DatePicker>("BirthDatePicker").IsEnabled = isEditing;
            this.FindControl<ComboBox>("DepartmentsComboBox").IsEnabled = isEditing;
            this.FindControl<ComboBox>("PositionsComboBox").IsEnabled = isEditing;
            this.FindControl<ComboBox>("AssistantsComboBox").IsEnabled = isEditing;

            this.FindControl<Button>("EditButton").IsVisible = !isEditing;
            this.FindControl<Button>("SaveButton").IsVisible = isEditing;
            this.FindControl<Button>("CancelButton").IsVisible = isEditing;
            this.FindControl<Button>("CloseButton").IsVisible = !isEditing;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode(true);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) 
                return;

            _employee.Lastname = this.FindControl<TextBox>("LastNameTextBox").Text;
            _employee.Firstname = this.FindControl<TextBox>("FirstNameTextBox").Text;
            _employee.Middlename = this.FindControl<TextBox>("MiddleNameTextBox").Text;
            _employee.Mobilephone = this.FindControl<TextBox>("MobilePhoneTextBox").Text;
            _employee.Workphone = this.FindControl<TextBox>("WorkPhoneTextBox").Text;
            _employee.Email = this.FindControl<TextBox>("EmailTextBox").Text;
            _employee.Office = this.FindControl<TextBox>("OfficeTextBox").Text;

            var selected = this.FindControl<DatePicker>("BirthDatePicker").SelectedDate;
            if (selected.HasValue)
            {
                _employee.Birthdate = DateOnly.FromDateTime(selected.Value.DateTime);
            }
            else
            {
                _employee.Birthdate = null;
            }

            _employee.Subdivisionid = (this.FindControl<ComboBox>("DepartmentsComboBox")
                                          .SelectedItem as DepartmentSubdivision)
                                      ?.Subdivisionid ?? 0;

            _employee.Positionid = (this.FindControl<ComboBox>("PositionsComboBox")
                                        .SelectedItem as Position)
                                  ?.Positionid ?? 0;

            if (_employee.Employeeid == 0)
            {
                _db.Employees.Add(_employee);
                _db.SaveChanges();

                _employee.Personalnumber = $"EMP{_employee.Employeeid}";
                _db.SaveChanges();
            }
            else
            {
                var existing = _db.Employees
                                  .Include(e => e.EmployeeRelationAssistants)
                                  .First(e => e.Employeeid == _employee.Employeeid);

                _db.Entry(existing).CurrentValues.SetValues(_employee);
                _db.SaveChanges();
            }

            SetEditMode(false);
            Close(true);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployee(_employee);
            SetEditMode(false);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }

        private bool ValidateInputs()
        {
            var errors = new List<string>();

            var lastName = this.FindControl<TextBox>("LastNameTextBox").Text;
            var firstName = this.FindControl<TextBox>("FirstNameTextBox").Text;
            var workPhone = this.FindControl<TextBox>("WorkPhoneTextBox").Text;
            var email = this.FindControl<TextBox>("EmailTextBox").Text;
            var office = this.FindControl<TextBox>("OfficeTextBox").Text;

            if (string.IsNullOrWhiteSpace(lastName))
                errors.Add("Фамилия обязательна");
            if (string.IsNullOrWhiteSpace(firstName))
                errors.Add("Имя обязательно");
            if (string.IsNullOrWhiteSpace(workPhone))
                errors.Add("Рабочий телефон обязателен");
            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email обязателен");
            if (string.IsNullOrWhiteSpace(office))
                errors.Add("Офис обязателен");

            var phoneRegex = new Regex(@"^[0-9+()\-\s#]{1,20}$");
            var mobileText = this.FindControl<TextBox>("MobilePhoneTextBox").Text;
            if (!string.IsNullOrEmpty(mobileText) && !phoneRegex.IsMatch(mobileText))
                errors.Add("Неверный формат мобильного телефона");
            if (!phoneRegex.IsMatch(workPhone))
                errors.Add("Неверный формат рабочего телефона");

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
                errors.Add("Некорректный email");

            if (errors.Any())
            {
                new MessageBox(string.Join("\n", errors)).ShowDialog(this);
                return false;
            }

            return true;
        }

        private void LoadEmployee(Employee employee)
        {
            this.FindControl<TextBox>("LastNameTextBox").Text = employee.Lastname;
            this.FindControl<TextBox>("FirstNameTextBox").Text = employee.Firstname;
            this.FindControl<TextBox>("MiddleNameTextBox").Text = employee.Middlename;
            this.FindControl<TextBox>("MobilePhoneTextBox").Text = employee.Mobilephone;
            this.FindControl<TextBox>("WorkPhoneTextBox").Text = employee.Workphone;
            this.FindControl<TextBox>("EmailTextBox").Text = employee.Email;
            this.FindControl<TextBox>("OfficeTextBox").Text = employee.Office;

            if (employee.Birthdate.HasValue)
            {
                var dt = employee.Birthdate.Value.ToDateTime(new TimeOnly(0, 0));
                this.FindControl<DatePicker>("BirthDatePicker").SelectedDate = new DateTimeOffset(dt);
            }
            else
            {
                this.FindControl<DatePicker>("BirthDatePicker").SelectedDate = null;
            }

            this.FindControl<ComboBox>("DepartmentsComboBox").SelectedItem =
                _subdivisions.FirstOrDefault(s => s.Subdivisionid == employee.Subdivisionid);

            this.FindControl<ComboBox>("PositionsComboBox").SelectedItem =
                _positions.FirstOrDefault(p => p.Positionid == employee.Positionid);

            var assistantRelation = _db.EmployeeRelations.FirstOrDefault(r => r.Employeeid == employee.Employeeid);
            if (assistantRelation != null)
            {
                this.FindControl<ComboBox>("AssistantsComboBox").SelectedItem =
                    _assistants.FirstOrDefault(a => a.Employeeid == assistantRelation.Assistantid);
            }
            else
            {
                this.FindControl<ComboBox>("AssistantsComboBox").SelectedItem = null;
            }
        }
    }
}