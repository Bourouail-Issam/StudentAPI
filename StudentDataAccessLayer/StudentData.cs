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

        public async Task<List<StudentDTO>> GetPassedStudentsAsync()
        {
            List<StudentDTO> Passedstudents = new List<StudentDTO>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_GetPassedStudents", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Passedstudents.Add(new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("StudentId")),
                                reader.GetString(reader.GetOrdinal("FullName")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))
                            ));
                    }
                }
            }

            return Passedstudents;
        }

        public async Task<decimal> GetAverageGradeAsync()
        {
            decimal averageGrade = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_GetAverageGrade", conn))
            {
                cmd.CommandType= CommandType.StoredProcedure;
                SqlParameter outputParam = new SqlParameter("@AverageGrade", SqlDbType.Decimal)
                {
                    Precision = 10,
                    Scale = 2,
                    Direction = ParameterDirection.Output,
                };
                cmd.Parameters.Add(outputParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                if (outputParam.Value != DBNull.Value)
                    averageGrade = Convert.ToDecimal(outputParam.Value);
            }
            return  averageGrade;
        }
    }
}
