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
        public async Task<StudentDTO> FindAsync(int studentID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_GetStudentByID", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter inputParam = new SqlParameter("@StudentID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = studentID
                };
                cmd.Parameters.Add(inputParam);
                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();                    
                    return new StudentDTO
                    {
                        StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        Age = reader.GetInt32(reader.GetOrdinal("Age")),
                        Grade = reader.GetInt32(reader.GetOrdinal("Grade"))
                    };                  
                }
            }
        }

        public async Task<int> UpdateStudentAsync(StudentDTO UpdatedStudent)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_UpdateStudent", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UpdateStudentID", SqlDbType.Int).Value = UpdatedStudent.StudentId;
                cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = UpdatedStudent.FullName;
                cmd.Parameters.Add("@Age", SqlDbType.Int).Value = UpdatedStudent.Age;
                cmd.Parameters.Add("@Grade", SqlDbType.Int).Value = UpdatedStudent.Grade;

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> AddStudentAsync(StudentDTO student)
        {
            int NewStudentID = -1;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_AddStudent", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = student.FullName;
                cmd.Parameters.Add("@Age",SqlDbType.Int).Value = student.Age;
                cmd.Parameters.Add("@Grade", SqlDbType.Int).Value = student.Grade;

                SqlParameter outputParam = new SqlParameter("@NewStudentID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                if (outputParam.Value != DBNull.Value)
                    NewStudentID = Convert.ToInt32(outputParam.Value);
            }
            return NewStudentID;
        }
    }
}
