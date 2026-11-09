using Microsoft.Data.SqlClient;
using SharedDTOModel;
using System.Data;

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

        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_GetUserById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter inputParam = new SqlParameter("@userID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = userId
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

        public async Task SaveRefreshTokenAsync(
            int userId, string refreshTokenHash, DateTime expiresAtUtc)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_Users_UpdateRefreshToken", conn))
            {
                cmd.CommandType= CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@RefreshTokenHash", SqlDbType.VarChar, 44).Value = refreshTokenHash;
                cmd.Parameters.Add("@RefreshTokenExpiresAt", SqlDbType.DateTime2).Value = expiresAtUtc;

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<UserDTO?> RotateRefreshTokenAsync(
            string oldHash, string newHash, DateTime newExpiresAtUtc)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_Users_RotateRefreshToken", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@OldRefreshTokenHash", SqlDbType.VarChar, 44).Value =oldHash;
                cmd.Parameters.Add("@NewRefreshTokenHash", SqlDbType.VarChar, 44).Value = newHash;
                cmd.Parameters.Add("@NewExpiresAt", SqlDbType.DateTime2).Value = newExpiresAtUtc;

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    return new UserDTO
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Role = reader.GetString(reader.GetOrdinal("Role"))
                    };
                }
            }
        }

        public async Task RevokeRefreshTokenAsync(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_Users_RevokeRefreshToken",conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }

        }
    }
}
