using SharedDTOModel;

namespace StudentAPIBusinessLayer
{
    public interface IStudent
    {
        Task<List<StudentDTO>> GetAllStudentsAsync();
    }
}