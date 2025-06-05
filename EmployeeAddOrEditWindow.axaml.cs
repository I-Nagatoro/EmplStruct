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
            SetEditMode(true);
        }

        public EmployeeAddOrEditWindow(Employee? employee = null)
        {
            InitializeComponent();
            _db = new User15Context();
            _employee = employee ?? new Employee();
            LoadData();
            
            LoadEmployee(_employee);
            SetEditMode(false);
        }

        private void LoadData()
        {
            _subdivisions = _db.DepartmentSubdivisions.ToList();
            _positions = _db.Positions.ToList();
            _assistants = _db.Employees
                .Where(e => e.Employeeid != _employee.Employeeid)
                .ToList();
            DepartmentsComboBox.ItemsSource = _subdivisions;
            PositionsComboBox.ItemsSource = _positions;
            AssistantsComboBox.ItemsSource = _assistants;
        }

        private void SetEditMode(bool isEditing)
        {
            _isEditing = isEditing;
            LastNameTextBox.IsReadOnly = !isEditing;
            FirstNameTextBox.IsReadOnly = !isEditing;
            MiddleNameTextBox.IsReadOnly = !isEditing;
            MobilePhoneTextBox.IsReadOnly = !isEditing;
            WorkPhoneTextBox.IsReadOnly = !isEditing;
            EmailTextBox.IsReadOnly = !isEditing;
            OfficeTextBox.IsReadOnly = !isEditing;

            BirthDatePicker.IsEnabled = isEditing;
            DepartmentsComboBox.IsEnabled = isEditing;
            PositionsComboBox.IsEnabled = isEditing;
            AssistantsComboBox.IsEnabled = isEditing;

            EditButton.IsVisible = !isEditing;
            SaveButton.IsVisible = isEditing;
            CancelButton.IsVisible = isEditing;
            CloseButton.IsVisible = !isEditing;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode(true);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) 
                return;

            _employee.Lastname = LastNameTextBox.Text;
            _employee.Firstname = FirstNameTextBox.Text;
            _employee.Middlename = MiddleNameTextBox.Text;
            _employee.Mobilephone = MobilePhoneTextBox.Text;
            _employee.Workphone = WorkPhoneTextBox.Text;
            _employee.Email = EmailTextBox.Text;
            _employee.Office = OfficeTextBox.Text;

            var selected = BirthDatePicker.SelectedDate;
            if (selected.HasValue)
            {
                _employee.Birthdate = DateOnly.FromDateTime(selected.Value.DateTime);
            }
            else
            {
                _employee.Birthdate = null;
            }

            _employee.Subdivisionid = (DepartmentsComboBox
                    .SelectedItem as DepartmentSubdivision)
                ?.Subdivisionid ?? 0;

            _employee.Positionid = (PositionsComboBox
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
            Close(false);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }

        private bool ValidateInputs()
        {
            var errors = new List<string>();

            var lastName = LastNameTextBox.Text;
            var firstName = FirstNameTextBox.Text;
            var workPhone = WorkPhoneTextBox.Text;
            var email = EmailTextBox.Text;
            var office = OfficeTextBox.Text;

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
            var mobileText = MobilePhoneTextBox.Text;
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
            LastNameTextBox.Text = employee.Lastname;
            FirstNameTextBox.Text = employee.Firstname;
            MiddleNameTextBox.Text = employee.Middlename;
            MobilePhoneTextBox.Text = employee.Mobilephone;
            WorkPhoneTextBox.Text = employee.Workphone;
            EmailTextBox.Text = employee.Email;
            OfficeTextBox.Text = employee.Office;

            if (employee.Birthdate.HasValue)
            {
                var dt = employee.Birthdate.Value.ToDateTime(new TimeOnly(0, 0));
                BirthDatePicker.SelectedDate = new DateTimeOffset(dt);
            }
            else
            {
                BirthDatePicker.SelectedDate = null;
            }

            DepartmentsComboBox.SelectedItem =
                _subdivisions.FirstOrDefault(s => s.Subdivisionid == employee.Subdivisionid);

            PositionsComboBox.SelectedItem =
                _positions.FirstOrDefault(p => p.Positionid == employee.Positionid);

            var assistantRelation = _db.EmployeeRelations.FirstOrDefault(r => r.Employeeid == employee.Employeeid);
            if (assistantRelation != null)
            {
                AssistantsComboBox.SelectedItem =
                    _assistants.FirstOrDefault(a => a.Employeeid == assistantRelation.Assistantid);
            }
            else
            {
                AssistantsComboBox.SelectedItem = null;
            }
        }
    }
}