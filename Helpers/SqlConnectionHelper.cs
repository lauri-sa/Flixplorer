using MySqlConnector;
using Flixplorer.Models;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Flixplorer.Helpers
{
    /// <summary>
    /// A helper class that handles all database interactions.
    /// </summary>
    internal class SqlConnectionHelper
    {
        /// <summary>
        /// Searches for a user in the database based on their username.
        /// If the user is found, the method returns true. If the user is not found, the method returns false.
        /// </summary>
        /// <param name="userName">The username of the user being searched for.</param>
        /// <returns>A boolean indicating whether or not the user was found in the database.</returns>
        public static bool SearchUserFromDatabase(string userName)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                const string STATEMENT = @"SELECT Username
                                           FROM Users
                                           WHERE Username = @Username;";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@Username", userName);

                    using (var reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a new user into the database with the provided username, password and salt.
        /// </summary>
        /// <param name="userName">The username of the new user.</param>
        /// <param name="password">The hashed password of the new user.</param>
        /// <param name="salt">The salt used to hash the password.</param>
        public static void AddUserToDatabase(string userName, string password, string salt)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                const string STATEMENT = @"INSERT INTO Users (Username, Password, Salt)
                                           VALUES (@Username, @Password, @Salt);";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@Username", userName);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Salt", salt);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Retrieves a user from the database with the given username and password, and populates the UserModel with the retrieved data.
        /// This method is used for auto-login.
        /// </summary>
        /// <returns>Returns true if a user with the given username and password exists in the database, false otherwise.</returns>
        public static bool GetUserFromDatabase()
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                const string STATEMENT = @"SELECT ID,Username
                                           FROM Users
                                           WHERE Username = @Username AND Password = @Password;";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@Username", Properties.Settings.Default["UserName"]);
                    command.Parameters.AddWithValue("@Password", Properties.Settings.Default["Password"]);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserModel.ID = (int)reader["ID"];
                            UserModel.UserName = (string)reader["Username"];
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a user from the database with the given username and password, and populates the UserModel with the retrieved data.
        /// </summary>
        /// <param name="userName">The username of the user to retrieve.</param>
        /// <param name="passwordBox">The password input field from which to retrieve the password.</param>
        /// <returns>Returns true if a user with the given username and password exists in the database, false otherwise.</returns>
        public static bool GetUserFromDatabase(string userName, object passwordBox)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                const string STATEMENT = @"SELECT *
                                           FROM Users
                                           WHERE Username = @Username;";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@Username", userName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read() && SecurityHelper.VerifyPassword((string)reader["Password"], SecurityHelper.HashPassword(((PasswordBox)passwordBox).Password, (string)reader["Salt"])))
                        {
                            UserModel.ID = (int)reader["ID"];
                            UserModel.UserName = (string)reader["Username"];

                            if ((bool)Properties.Settings.Default["RememberMe"])
                            {
                                Properties.Settings.Default["UserName"] = (string)reader["Username"];
                                Properties.Settings.Default["Password"] = (string)reader["Password"];
                                Properties.Settings.Default.Save();
                            }
                            
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a list of MediaModels (movies) into the database, but only if a movie with the same NetflixID does not already exist in the database.
        /// </summary>
        /// <param name="movieModels">The list of MediaModels (movies) to add to the database.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task AddMoviesToDatabase(List<MediaModel> movieModels)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"INSERT INTO Movies (NetflixID, ImagePath, HideFromList) 
                                           SELECT * FROM (SELECT @NetflixID, @ImagePath, @HideFromList)
                                           AS temp
                                           WHERE NOT EXISTS (SELECT NetflixID FROM Movies WHERE NetflixID = @NetflixID);";

                foreach (var movie in movieModels)
                {
                    using (var command = new MySqlCommand(STATEMENT, connection))
                    {
                        command.Parameters.AddWithValue("@NetflixID", movie.netflix_id);
                        command.Parameters.AddWithValue("@ImagePath", movie.img);
                        command.Parameters.AddWithValue("@HideFromList", movie.hideFromList);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a list of MediaModels (series) into the database, but only if a serie with the same NetflixID does not already exist in the database.
        /// </summary>
        /// <param name="seriesModels">The list of MediaModels (series) to add to the database.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task AddSeriesToDatabase(List<MediaModel> seriesModels)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"INSERT INTO Series (NetflixID, ImagePath, HideFromList) 
                                           SELECT * FROM (SELECT @NetflixID, @ImagePath, @HideFromList)
                                           AS temp
                                           WHERE NOT EXISTS (SELECT NetflixID FROM Series WHERE NetflixID = @NetflixID);";

                foreach (var series in seriesModels)
                {
                    using (var command = new MySqlCommand(STATEMENT, connection))
                    {
                        command.Parameters.AddWithValue("@NetflixID", series.netflix_id);
                        command.Parameters.AddWithValue("@ImagePath", series.img);
                        command.Parameters.AddWithValue("@HideFromList", series.hideFromList);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a list of all movies from the database that are not marked as hidden, ordered by ID in descending order.
        /// </summary>
        /// <returns>A list of MediaModel objects representing the retrieved movies.</returns>
        public static async Task<List<MediaModel>> GetMoviesFromDatabase()
        {
            var moviesList = new List<MediaModel>();

            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"SELECT *
                                           FROM Movies
                                           WHERE HideFromList = false
                                           ORDER BY ID DESC;";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            moviesList.Add(new MediaModel
                            {
                                ID = (int)reader["ID"],
                                netflix_id = (int)reader["NetflixID"],
                                img = (string)reader["ImagePath"]
                            });
                        }

                        return moviesList;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a list of all series from the database that are not marked as hidden, ordered by ID in descending order.
        /// </summary>
        /// <returns>A list of MediaModel objects representing the retrieved series.</returns>
        public static async Task<List<MediaModel>> GetSeriesFromDatabase()
        {
            var seriesList = new List<MediaModel>();

            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"SELECT *
                                           FROM Series
                                           WHERE HideFromList = false
                                           ORDER BY ID DESC;";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            seriesList.Add(new MediaModel
                            {
                                ID = (int)reader["ID"],
                                netflix_id = (int)reader["NetflixID"],
                                img = (string)reader["ImagePath"]
                            });
                        }

                        return seriesList;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a movie with the given NetflixID to the current user's list of favourite movies in the database.
        /// </summary>
        /// <param name="userID">The ID of the user whose favourite movie list to add the movie to.</param>
        /// <param name="netflixID">The NetflixID of the movie to add to the favourite list.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task AddFavouriteMovieToDatabase(int userID, string netflixID)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"INSERT INTO FavouriteMovies (UserID, MovieID)
                                           VALUES (@UserID, (SELECT ID FROM Movies WHERE NetflixID = @NetflixID));";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@NetflixID", netflixID);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Adds a serie with the given NetflixID to the current user's list of favourite series in the database.
        /// </summary>
        /// <param name="userID">The ID of the user whose favourite series list to add the serie to.</param>
        /// <param name="netflixID">The NetflixID of the movie to add to the favourite list.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task AddFavouriteSeriesToDatabase(int userID, string netflixID)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"INSERT INTO FavouriteSeries (UserID, SeriesID)
                                           VALUES (@UserID, (SELECT ID FROM Series WHERE NetflixID = @NetflixID));";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@NetflixID", netflixID);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Searches the database to check if a movie or series with a given Netflix ID is in the user's favorites list.
        /// </summary>
        /// <param name="UserID">The user's ID.</param>
        /// <param name="netflixID">The Netflix ID of the movie or series to check.</param>
        /// <returns>Returns a boolean value indicating whether the movie or series is in the user's favorites list.</returns>
        public static async Task<bool> SearchFavouriteFromDatabase(int UserID, string netflixID)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"SELECT *
                                           FROM FavouriteMovies fm
                                           JOIN Movies ON fm.MovieID = Movies.ID
                                           WHERE fm.UserID = @UserID AND Movies.NetflixID = @NetflixID
                                           UNION
                                           SELECT *
                                           FROM FavouriteSeries fs
                                           JOIN Series ON fs.SeriesID = Series.ID
                                           WHERE fs.UserID = @UserID AND Series.NetflixID = @NetflixID;";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@NetflixID", netflixID);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a list of favourite movies and series from the database for a given user ID.
        /// </summary>
        /// <param name="userID">The ID of the user to retrieve favourites for.</param>
        /// <returns>A list of MediaModel objects representing the favourite movies and series.</returns>
        public static async Task<List<MediaModel>> GetFavouritesFromDatabase(int userID)
        {
            var favouritesList = new List<MediaModel>();

            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"SELECT m.NetflixID, m.ImagePath
                                           FROM FavouriteMovies fm
                                           JOIN Movies m ON fm.MovieID = m.ID
                                           WHERE fm.UserID = @UserID
                                           UNION
                                           SELECT s.NetflixID, s.ImagePath
                                           FROM FavouriteSeries fs
                                           JOIN Series s ON fs.SeriesID = s.ID
                                           WHERE fs.UserID = @UserID;";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            favouritesList.Add(new MediaModel
                            {
                                netflix_id = (int)reader["NetflixID"],
                                img = (string)reader["ImagePath"]
                            });
                        }

                        return favouritesList;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a movie from the favourites list for a given user.
        /// </summary>
        /// <param name="userID">The ID of the user whose favourite movie is being removed.</param>
        /// <param name="netflixID">The Netflix ID of the movie to be removed.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task RemoveFavouriteMovieFromDatabase(int userID, string netflixID)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"DELETE fm
                                           FROM FavouriteMovies fm
                                           INNER JOIN Movies ON fm.MovieID = Movies.ID
                                           WHERE fm.UserID = @UserID AND Movies.NetflixID = @NetflixID;";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID); 
                    command.Parameters.AddWithValue("@NetflixID", netflixID);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Removes a serie from the favourites list for a given user.
        /// </summary>
        /// <param name="userID">The ID of the user whose favourite serie is being removed.</param>
        /// <param name="netflixID">The Netflix ID of the serie to be removed.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task RemoveFavouriteSeriesFromDatabase(int userID, string netflixID)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                await connection.OpenAsync();

                const string STATEMENT = @"DELETE fs
                                           FROM FavouriteSeries fs
                                           INNER JOIN Series ON fs.SeriesID = Series.ID
                                           WHERE fs.UserID = @UserID AND Series.NetflixID = @NetflixID;";

                using (var command = new MySqlCommand(STATEMENT, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@NetflixID", netflixID);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}