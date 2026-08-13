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

    }
}
