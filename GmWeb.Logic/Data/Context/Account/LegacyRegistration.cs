using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Context.Identity
{
    public class LegacyRegistration
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static LegacyRegistration _LegacyRegistration;
        public static LegacyRegistration Instance => _LegacyRegistration ?? (_LegacyRegistration = new LegacyRegistration());
        public enum ResponseCode
        {
            Unknown = -1,
            Success = 0,
            DuplicateUser = 1
        }

        private LegacyRegistration() { }

        public static string ConnectionString => ConfigurationManager.ConnectionStrings["CURRENT_INSTANCE_DB"].ConnectionString;
        /// <summary>
        /// This routine inserts agencyID, userFName, userLName, userPhone,
        /// and UserEmailAddress into tblUser. All other data in the row is defaulted.
        /// </summary>
        /// <param name="agencyID">The agency's AgencyID key.</param>
        /// <param name="userFName">The user's first name.</param>
        /// <param name="userLName">The user's last name.</param>
        /// <param name="userPhone">The user's phone number.</param>
        /// <param name="userEmailAddress">The user's email address.</param>
        /// <param name="cubeRole">The user's Cube Role.</param>
        /// <returns>0 if OK, else an error code.</returns>
        public Task<ResponseCode> CreateUser(string userEmailAddress, string passwordHash)
            => this.CreateUser(userEmailAddress, passwordHash, null, null, null, null, null);
        public async Task<ResponseCode> CreateUser(string userEmailAddress, string passwordHash, int? agencyID, string userFName, string userLName, string userPhone, string cubeRole)
        {
            // Create a new SqlConnection.
            var conn = new SqlConnection(ConnectionString);
            try
            {
                // Create a SqlCommand and load it with the stored procedure.
                var cmd = new SqlCommand("spAICreateUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Set the stored procedure's parameters.
                SqlParameter param;
                param = cmd.Parameters.Add("@AgencyID", SqlDbType.Int);
                param.Value = agencyID;
                param = cmd.Parameters.Add("@UserFName", SqlDbType.VarChar, 32);
                param.Value = userFName;
                param = cmd.Parameters.Add("@UserLName", SqlDbType.VarChar, 32);
                param.Value = userLName;
                param = cmd.Parameters.Add("@UserPhone", SqlDbType.VarChar, 12);
                param.Value = userPhone;
                param = cmd.Parameters.Add("@UserEmailAddress", SqlDbType.VarChar, 100);
                param.Value = userEmailAddress;
                param = cmd.Parameters.Add("@UserPasswordHash", SqlDbType.VarChar, 64);
                param.Value = passwordHash;
                param = cmd.Parameters.Add("@CubeRole", SqlDbType.Char, 3);
                param.Value = cubeRole;
                foreach (SqlParameter p in cmd.Parameters)
                    if (p.Value == null)
                        p.Value = DBNull.Value;
                conn.Open();
                await cmd.ExecuteNonQueryAsync();
                conn.Close();
                return ResponseCode.Success;
            }
            catch (Exception exc)
            {
                _logger.Error("CreateUser", exc);
                conn.Close();
                if (exc.Message.Contains("Duplicate email"))
                {
                    return ResponseCode.DuplicateUser;
                }
                else
                {
                    return ResponseCode.Unknown;
                }
            }
        }

        /// <summary>
        /// Inserts the volunteer data into tblClient and tblStaff.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <param name="agencyID">Agency ID.</param>
        /// <param name="lastName">Last Name.</param>
        /// <param name="firstName">First Name.</param>
        /// <param name="addressNo">Address No.</param>
        /// <param name="addressStreet">Address Street.</param>
        /// <param name="addressStreetType">Address Street Type.</param>
        /// <param name="addressLine2">Address Line 2.</param>
        /// <param name="city">City.</param>
        /// <param name="zip">Zip.</param>
        /// <param name="phone">Phone.</param>
        /// <returns>ClientID if OK, else 0.</returns>
        public Task<ResponseCode> CreateClient(string email, string passwordHash)
            => this.CreateClient(email, passwordHash, null, null, null, null, null, null, null, null, null, null);
        public async Task<ResponseCode> CreateClient(string email, string passwordHash, int? agencyID, string lastName, string firstName, string addressNo, string addressStreet, string addressStreetType, string addressLine2, string city, string zip, string phone)
        {
            // Create a new SqlConnection.
            var conn = new SqlConnection(ConnectionString);
            try
            {
                // Create a SqlCommand and load it with the stored procedure.
                var cmd = new SqlCommand("spVOLSaveVolunteerData", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Set the stored procedure's parameters.
                SqlParameter param;
                param = cmd.Parameters.Add("@Email", SqlDbType.VarChar, 50);
                param.Value = email;
                param = cmd.Parameters.Add("@PasswordHash", SqlDbType.VarChar, 30);
                param.Value = passwordHash;
                param = cmd.Parameters.Add("@AgencyID", SqlDbType.Int);
                param.Value = agencyID;
                param = cmd.Parameters.Add("@LastName", SqlDbType.VarChar, 30);
                param.Value = lastName;
                param = cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 30);
                param.Value = firstName;
                param = cmd.Parameters.Add("@AddressNo", SqlDbType.VarChar, 5);
                param.Value = addressNo;
                param = cmd.Parameters.Add("@AddressStreet", SqlDbType.VarChar, 30);
                param.Value = addressStreet;
                param = cmd.Parameters.Add("@AddressStreetType", SqlDbType.Char, 2);
                param.Value = addressStreetType;
                param = cmd.Parameters.Add("@AddressLine2", SqlDbType.VarChar, 30);
                param.Value = addressLine2;
                param = cmd.Parameters.Add("@City", SqlDbType.VarChar, 25);
                param.Value = city;
                param = cmd.Parameters.Add("@Zip", SqlDbType.VarChar, 10);
                param.Value = zip;
                param = cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 14);
                param.Value = phone;
                param = cmd.Parameters.Add("@ClientID", SqlDbType.Int);
                param.Direction = ParameterDirection.Output;
                foreach (SqlParameter p in cmd.Parameters)
                    if (p.Value == null)
                        p.Value = DBNull.Value;
                conn.Open();
                await cmd.ExecuteNonQueryAsync();
                return ResponseCode.Success;
            }
            catch (Exception exc)
            {
                _logger.Error("SaveVolunteerData", exc);
                if (exc.Message.Contains("Duplicate email"))
                {
                    return ResponseCode.DuplicateUser;
                }
                else
                {
                    return ResponseCode.Unknown;
                }
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
