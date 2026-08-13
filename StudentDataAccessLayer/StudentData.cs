using Microsoft.Data.SqlClient;
using SharedDTOModel;
using System.Data;

namespace StudentDataAccessLayer
{
    public class StudentData
    {
        private readonly string _connectionString;

        public StudentData(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<List<StudentDTO>> GetAllStudentsAsync()
        {
            List<StudentDTO> students = new List<StudentDTO>();

            using(SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_GetAllStudents",conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        students.Add(new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("StudentId")),
                                reader.GetString(reader.GetOrdinal("FullName")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))
                            ));
                    }
                }
            }
            
            return students;
        }
    }
}
