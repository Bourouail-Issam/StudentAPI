using SharedDTOModel;
using StudentDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAPIBusinessLayer
{
    internal class StudentEntity
    {
        private readonly StudentData _studentData; 
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _mode;

        private List<string> Roles = ["Owner", "Manager", "Admin", "Student", "Teacher"];

        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Grade { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;


        public StudentEntity(StudentData studentData, StudentDTO dto, enMode mode)
        {
            this._studentData = studentData;
            this.StudentId = dto.StudentId;
            this.FullName = dto.FullName;
            this.Age = dto.Age;
            this.Grade = dto.Grade;
            this._mode = mode;
        }
        public StudentEntity(StudentData studentData, StudentDTO dto,UserDTO user , enMode mode)
        {
            this._studentData = studentData;  
            this.StudentId = dto.StudentId;
            this.FullName = dto.FullName;
            this.Age = dto.Age;
            this.Grade = dto.Grade;
            this.UserId = user.UserId;
            this.Email = user.Email;
            this.PasswordHash = user.PasswordHash;
            this.Role = user.Role;
            this._mode = mode;
        }

        #region Validate Methods
        /// <summary>
        /// Validates student data before Add or Update operations.
        /// Mirrors the CHECK constraints defined on the Students table.
        /// </summary>
        private void _ValidateForSave()
        {
            if (string.IsNullOrWhiteSpace(FullName))
                throw new ArgumentException("FullName cannot be empty.");

            if (Age < 12 || Age > 60)
                throw new ArgumentException("Age must be between 12 and 60.");

            if (Grade < 0 || Grade > 100)
                throw new ArgumentException("Grade must be between 0 and 100.");

            if (_mode == enMode.AddNew)
            {
                if (string.IsNullOrWhiteSpace(Email))
                    throw new ArgumentException("Email cannot be empty.");

                if (string.IsNullOrEmpty(PasswordHash))
                    throw new ArgumentException("PasswordHash cannot be empty");

                if (!Roles.Contains(this.Role))
                    throw new ArgumentException("Invalid Role value.");
            }
              
        }

        #endregion

        #region CRUD Methods
        private async Task _AddNewStudent(StudentDTO currentData, UserDTO userData)
        {
            userData.PasswordHash = PasswordHasher.HashPassword(this.PasswordHash);
            this.StudentId = await _studentData.AddStudentAsync(currentData, userData);
        }
        private async Task<int> _UpdateStudent(StudentDTO currentData)
        {
            return await _studentData.UpdateStudentAsync(currentData);
        }

        
        #endregion
        public async Task SaveAsync()
        {
            _ValidateForSave();
            StudentDTO currentData = new StudentDTO(StudentId, FullName, Age, Grade);                

            switch (_mode)
            {
                case enMode.AddNew:
                    UserDTO userData = new UserDTO(UserId, Email, PasswordHash, Role, StudentId);
                    await _AddNewStudent(currentData, userData);
                    break;

                case enMode.Update:
                    await _UpdateStudent(currentData);
                    break;
            }
        }
    }
}
