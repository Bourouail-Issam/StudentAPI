using Microsoft.Data.SqlClient;
using SharedDTOModel;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDataAccessLayer
{
    public class UserData
    {
        private readonly string _connectionString;

        public UserData(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<UserDTO?> GetUserByEmailAsync(string Email)
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            using( SqlCommand cmd = new SqlCommand("usp_GetUserByEmail", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter inputParam = new SqlParameter("@Email", SqlDbType.NVarChar, 150)
                {
                    Direction = ParameterDirection.Input,
                    Value = Email
                };
                
                cmd.Parameters.Add(inputParam);
                await conn.OpenAsync();

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    if (!await dr.ReadAsync())
                        return null;
                    else
                        return new UserDTO
                        {
                            UserId = dr.GetInt32(dr.GetOrdinal("UserId")),
                            Email = dr.GetString(dr.GetOrdinal("Email")),
                            PasswordHash = dr.GetString(dr.GetOrdinal("PasswordHash")),
                            Role = dr.GetString(dr.GetOrdinal("Role")),
                        };


                }
            }
        }
    }
}
