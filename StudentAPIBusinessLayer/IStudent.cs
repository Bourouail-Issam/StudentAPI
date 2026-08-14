using SharedDTOModel;

namespace StudentAPIBusinessLayer
{
    public interface IStudent
    {
        Task<List<StudentDTO>> GetAllStudentsAsync();
        Task<List<StudentDTO>> GetPassedStudentsAsync();
        Task<decimal> GetAverageGradeAsync();
    }
}