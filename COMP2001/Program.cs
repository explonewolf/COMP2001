using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;


public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    // Configures the services used by the application.
    public void ConfigureServices(IServiceCollection services)
    {
        // Authentication setup using JWT Bearer authentication.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://web.socem.plymouth.ac.uk/COMP2001/auth";
                options.Audience = "https://virtserver.swaggerhub.com/EXPLONEWOLF/COMP2001P/1.0.0";
            });

        // Authorization services setup.
        services.AddAuthorization();

        // Registers a scoped HttpClient.
        services.AddScoped<HttpClient>();
        services.AddHttpClient();

        // Registers the ProfileRepository as a scoped service.
        services.AddScoped<ProfileRepository>();

        // Swagger configuration for API documentation.
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "swagger", Version = "v1" });
        });
    }

    // Configures the application.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Adds authentication middleware to the pipeline.
        app.UseAuthentication();

        // Adds authorization middleware to the pipeline.
        app.UseAuthorization();

        // Adds Swagger middleware to generate API documentation.
        app.UseSwagger();

        // Adds Swagger UI middleware to serve Swagger UI.
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("swagger.json", "swagger");
        });
    }

    // Controller for managing user profiles.
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Perform authentication logic here (check user credentials, generate JWT, etc.)
            if (IsValidCredentials(request.UserName, request.Email, request.Password))
            {
                // Authentication successful
                // Generate JWT token or perform other actions
                return Ok(new { Message = "Login successful" });
            }
            else
            {
                // Authentication failed
                return Unauthorized(new { Message = "Invalid credentials" });
            }
        }

        private bool IsValidCredentials(string userName, string email, string password)
        {
            // Implement your authentication logic here (e.g., check against database)
            // Return true if credentials are valid, false otherwise
            // This is a placeholder, replace it with your actual logic
            return true;
        }
    }

    // Repository for managing user profiles in the database.
    public class ProfileRepository
    {
        private readonly string _connectionString;

        public ProfileRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Retrieves all user profiles asynchronously from the database.
        public async Task<List<UserProfile>> GetAllProfilesAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SELECT * FROM CW2.UserTable", connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<UserProfile> profiles = new List<UserProfile>();

                    // Maps data from the SqlDataReader to UserProfile objects.
                    while (await reader.ReadAsync())
                    {
                        UserProfile profile = new UserProfile
                        {
                            // Maps individual fields from the database to properties of the UserProfile class.
                            UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            TwoFactorAuth = reader.GetBoolean(reader.GetOrdinal("TwoFactorAuth")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DOB = reader.GetDateTime(reader.GetOrdinal("DOB")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            ProfilePicture = reader.GetString(reader.GetOrdinal("ProfilePicture")),
                            FriendUserID = reader.GetInt32(reader.GetOrdinal("FriendUserID")),
                            WalksComplete = reader.GetInt32(reader.GetOrdinal("WalksComplete")),
                            WalksNotFinished = reader.GetInt32(reader.GetOrdinal("WalksNotFinished")),
                            LastWalk = reader.GetDateTime(reader.GetOrdinal("LastWalk")),
                            CreatedTrialID = reader.GetInt32(reader.GetOrdinal("CreatedTrialID")),
                            CreatedTrialName = reader.GetString(reader.GetOrdinal("CreatedTrialName")),
                        };

                        profiles.Add(profile);
                    }

                    return profiles;
                }
            }
        }

        // Retrieves a user profile by ID asynchronously from the database.
        public async Task<UserProfile> GetProfileByIdAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SELECT * FROM CW2.UserTable WHERE UserID = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            UserProfile profile = new UserProfile
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                TwoFactorAuth = reader.GetBoolean(reader.GetOrdinal("TwoFactorAuth")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DOB = reader.GetDateTime(reader.GetOrdinal("DOB")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                ProfilePicture = reader.GetString(reader.GetOrdinal("ProfilePicture")),
                                FriendUserID = reader.GetInt32(reader.GetOrdinal("FriendUserID")),
                                WalksComplete = reader.GetInt32(reader.GetOrdinal("WalksComplete")),
                                WalksNotFinished = reader.GetInt32(reader.GetOrdinal("WalksNotFinished")),
                                LastWalk = reader.GetDateTime(reader.GetOrdinal("LastWalk")),
                                CreatedTrialID = reader.GetInt32(reader.GetOrdinal("CreatedTrialID")),
                                CreatedTrialName = reader.GetString(reader.GetOrdinal("CreatedTrialName")),
                            };

                            return profile;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        // Creates a new user profile asynchronously in the database.
        public async Task<int> CreateProfileAsync(UserProfile profile)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // SQL query to insert a new user profile.
                    string insertQuery = @"INSERT INTO CW2.UserTable (Username, TwoFactorAuth, Phone, FirstName, LastName, DOB, Email, ProfilePicture, FriendUserID, WalksComplete, WalksNotFinished, LastWalk, CreatedTrialID, CreatedTrialName) 
                                   VALUES (@Username, @TwoFactorAuth, @Phone, @FirstName, @LastName, @DOB, @Email, @ProfilePicture, @FriendUserID, @WalksComplete, @WalksNotFinished, @LastWalk, @CreatedTrialID, @CreatedTrialName);
                                   SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Sets parameters for the SQL query.
                        command.Parameters.AddWithValue("@Username", profile.Username);
                        command.Parameters.AddWithValue("@TwoFactorAuth", profile.TwoFactorAuth);
                        command.Parameters.AddWithValue("@Phone", profile.Phone);
                        command.Parameters.AddWithValue("@FirstName", profile.FirstName);
                        command.Parameters.AddWithValue("@LastName", profile.LastName);
                        command.Parameters.AddWithValue("@DOB", profile.DOB);
                        command.Parameters.AddWithValue("@Email", profile.Email);
                        command.Parameters.AddWithValue("@ProfilePicture", profile.ProfilePicture);
                        command.Parameters.AddWithValue("@FriendUserID", profile.FriendUserID);
                        command.Parameters.AddWithValue("@WalksComplete", profile.WalksComplete);
                        command.Parameters.AddWithValue("@WalksNotFinished", profile.WalksNotFinished);
                        command.Parameters.AddWithValue("@LastWalk", profile.LastWalk);
                        command.Parameters.AddWithValue("@CreatedTrialID", profile.CreatedTrialID);
                        command.Parameters.AddWithValue("@CreatedTrialName", profile.CreatedTrialName);

                        // Executes the query and gets the ID of the newly created profile.
                        int newProfileId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        return newProfileId;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Updates an existing user profile asynchronously in the database.
        public async Task<bool> UpdateProfileAsync(UserProfile profile)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // SQL query to update an existing user profile.
                    string updateQuery = @"UPDATE CW2.UserTable
                                   SET Username = @Username, 
                                       TwoFactorAuth = @TwoFactorAuth, 
                                       Phone = @Phone, 
                                       FirstName = @FirstName, 
                                       LastName = @LastName, 
                                       DOB = @DOB, 
                                       Email = @Email, 
                                       ProfilePicture = @ProfilePicture, 
                                       FriendUserID = @FriendUserID, 
                                       WalksComplete = @WalksComplete, 
                                       WalksNotFinished = @WalksNotFinished, 
                                       LastWalk = @LastWalk, 
                                       CreatedTrialID = @CreatedTrialID, 
                                       CreatedTrialName = @CreatedTrialName 
                                   WHERE UserID = @UserID;";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        // Sets parameters for the SQL query.
                        command.Parameters.AddWithValue("@UserID", profile.UserID);
                        command.Parameters.AddWithValue("@Username", profile.Username);
                        command.Parameters.AddWithValue("@TwoFactorAuth", profile.TwoFactorAuth);
                        command.Parameters.AddWithValue("@Phone", profile.Phone);
                        command.Parameters.AddWithValue("@FirstName", profile.FirstName);
                        command.Parameters.AddWithValue("@LastName", profile.LastName);
                        command.Parameters.AddWithValue("@DOB", profile.DOB);
                        command.Parameters.AddWithValue("@Email", profile.Email);
                        command.Parameters.AddWithValue("@ProfilePicture", profile.ProfilePicture);
                        command.Parameters.AddWithValue("@FriendUserID", profile.FriendUserID);
                        command.Parameters.AddWithValue("@WalksComplete", profile.WalksComplete);
                        command.Parameters.AddWithValue("@WalksNotFinished", profile.WalksNotFinished);
                        command.Parameters.AddWithValue("@LastWalk", profile.LastWalk);
                        command.Parameters.AddWithValue("@CreatedTrialID", profile.CreatedTrialID);
                        command.Parameters.AddWithValue("@CreatedTrialName", profile.CreatedTrialName);

                        // Executes the query and checks if any rows were affected.
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Deletes a user profile asynchronously from the database by ID.
        public async Task<bool> DeleteProfileAsync(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // SQL query to delete a user profile by ID.
                    string deleteQuery = "DELETE FROM CW2.UserTable WHERE UserID = @UserID;";

                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        // Sets parameter for the SQL query.
                        command.Parameters.AddWithValue("@UserID", id);

                        // Executes the query and checks if any rows were affected.
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    // Model representing a user profile.
    public class UserProfile
    {
        // Properties representing fields in the UserProfiles table.
        public int UserID { get; set; }
        public string Username { get; set; }
        public bool TwoFactorAuth { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public String Email { get; set; }
        public string ProfilePicture { get; set; }
        public int FriendUserID { get; set; }
        public int WalksComplete { get; set; }
        public int WalksNotFinished { get; set; }
        public DateTime LastWalk { get; set; }
        public int CreatedTrialID { get; set; }
        public string CreatedTrialName { get; set; }
    }

    // Model representing JWT settings.
    public class JwtSettings
    {
        // Properties for Authority and Audience.
        public string Authority { get; set; }
        public string Audience { get; set; }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
