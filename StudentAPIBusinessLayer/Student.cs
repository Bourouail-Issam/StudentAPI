using SharedDTOModel;
using StudentDataAccessLayer;

namespace StudentAPIBusinessLayer
{
    public class Student : IStudent
    {
        private readonly StudentData _studentData;
        public Student(string connectionString)
        {
            _studentData = new StudentData(connectionString);
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
        public async Task<int> AddStudentAsync(StudentDTO dto)
        {
            StudentEntity entity = new StudentEntity(_studentData,dto ,StudentEntity.enMode.AddNew);
            await entity.SaveAsync();
            return entity.StudentId;
        }
    }
}
