using SharedDTOModel;
using StudentDataAccessLayer;

namespace StudentAPIBusinessLayer
{
    public class Student : IStudent
    {
        private readonly StudentData _studentData;
        private readonly IPasswordHasher _passwordHasher;
        public Student(string connectionString, IPasswordHasher passwordHasher)
        {
            _studentData = new StudentData(connectionString);
            _passwordHasher = passwordHasher;
        }
        public async Task<List<StudentDTO>> GetAllStudentsAsync()
        {
            return await _studentData.GetAllStudentsAsync();
        }
        public async Task<List<StudentDTO>> GetPassedStudentsAsync()
        {
            return await _studentData.GetPassedStudentsAsync();
        }
        public async Task<decimal> GetAverageGradeAsync()
        {
            return await _studentData.GetAverageGradeAsync();
        }
        public async Task<StudentDTO> GetStudentByIDAsync(int studentID)
        {
            return await _studentData.FindAsync(studentID);
        }
        public async Task<int> AddStudentAsync(StudentDTO dto, UserDTO user)
        {
            StudentEntity entity = new StudentEntity(_studentData,dto, user ,
                StudentEntity.enMode.AddNew, _passwordHasher);

            await entity.SaveAsync();
            return entity.StudentId;
        }
        public async Task UpdateStudentAsync(StudentDTO dto)
        {
            StudentEntity entity = new StudentEntity(_studentData, dto, 
                StudentEntity.enMode.Update, _passwordHasher);

            await entity.SaveAsync();
        }
        public async Task<bool> DeleteStudentAsync(int studentID)
        {
            if (studentID <= 0)
                throw new ArgumentException("Invalid StudentID.");

            int rowsAffected = await _studentData.DeleteStudentAsync(studentID);
            return rowsAffected > 0;
        }
    }
}
