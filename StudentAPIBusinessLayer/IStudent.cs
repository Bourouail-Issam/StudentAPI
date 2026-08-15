using SharedDTOModel;

namespace StudentAPIBusinessLayer
{
    public interface IStudent
    {
        Task<List<StudentDTO>> GetAllStudentsAsync();
        Task<List<StudentDTO>> GetPassedStudentsAsync();
        Task<decimal> GetAverageGradeAsync();
        Task<StudentDTO> GetStudentByIDAsync(int studentID);
        Task<int> AddStudentAsync(StudentDTO dto);
        Task UpdateStudentAsync(StudentDTO dto);
    }
}