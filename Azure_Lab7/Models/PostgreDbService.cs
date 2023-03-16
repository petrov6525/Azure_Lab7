using Npgsql;
using Azure_Lab7.Models;
using System.Net;
using System.IO;

namespace Azure_Lab7.Models
{
    public class PostgreDbService
    {
        private static readonly NpgsqlConnectionStringBuilder connString = new NpgsqlConnectionStringBuilder("Server=c.daryncevpostgre.postgres.database.azure.com;Database=citus;Port=5432;User Id=citus;Password=80675273807Olo;Ssl Mode=Require;Pooling = true;Minimum Pool Size=0;Maximum Pool Size =50;");




        public static async Task<List<Address>> GetAllAddresses()
        {
            var addresses = new List<Address>();

            connString.TrustServerCertificate = true;

            try
            {
                using (var connection = new NpgsqlConnection(connString.ToString()))
                {
                    await connection.OpenAsync();

                    var cmd = new NpgsqlCommand("select * from address", connection);

                    var result = await cmd.ExecuteReaderAsync();

                    while (result.Read())
                    {
                        addresses.Add(new Address()
                        {
                            Id=result.GetInt32(0),
                            Country = result.GetString(1),
                            City = result.GetString(2),
                            Street = result.GetString(3),
                            Building = result.GetString(4)
                        });
                    }
                }

                return addresses;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);

                return null;
            }
        }



        public static async Task<Address> GetAddressById(int id)
        {
            var address = new Address();

            connString.TrustServerCertificate = true;

            try
            {
                using (var connection = new NpgsqlConnection(connString.ToString()))
                {
                    await connection.OpenAsync();

                    var cmd = new NpgsqlCommand("select * from address where id=@id", connection);
                    cmd.Parameters.AddWithValue("id", id);

                    var result = await cmd.ExecuteReaderAsync();

                    while (result.Read())
                    {
                        address.Country = result.GetString(1);
                        address.City = result.GetString(2);
                        address.Street = result.GetString(3);
                        address.Building = result.GetString(4);
                    }
                }

                return address;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);

                return null;
            }
        }


        public static async Task<List<Book>> GetAllBooks()
        {
            connString.TrustServerCertificate = true;

            var books = new List<Book>();

            try
            {
                using (var connection = new NpgsqlConnection(connString.ToString()))
                {
                    await connection.OpenAsync();

                    NpgsqlCommand cmd = new NpgsqlCommand("select * from books", connection);
                    var result =await cmd.ExecuteReaderAsync();

                    while (result.Read())
                    {
                        books.Add(new Book()
                        {
                            Id=result.GetInt32(0),
                            BookName = result.GetString(1),
                            AuthorFirstName = result.GetString(2),
                            AuthorLastName = result.GetString(3),
                            DateOfPublish = result.GetString(4),
                            PublishingName = result.GetString(5),
                            PublishingAddressId = result.GetInt32(6)
                        });
                        //Console.WriteLine(string.Format("id:{0} username:{1} age:{2}", result.GetInt32(0).ToString(), result.GetString(1), result.GetInt32(2).ToString()));
                    }

                    result.Close();
                }

                return books;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);

                return null;
            }



        }


        public static async Task<bool> TryAddAddress(Address address)
        {
            try
            {
                connString.TrustServerCertificate = true;

                using (var connection = new NpgsqlConnection(connString.ToString()))
                {
                    await connection.OpenAsync();

                    var cmd = new NpgsqlCommand(
                        "insert into address" +
                        "(country,city,street,building)" +
                        "values" +
                        "(@country,@city,@street,@building)",
                        connection);

                    cmd.Parameters.AddWithValue("country", address.Country);
                    cmd.Parameters.AddWithValue("city", address.City);
                    cmd.Parameters.AddWithValue("street", address.Street);
                    cmd.Parameters.AddWithValue("building", address.Building);

                    await cmd.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }
        }


        public static async Task<bool> TryAddBook(Book book)
        {
            try
            {
                connString.TrustServerCertificate = true;

                using (var connection = new NpgsqlConnection(connString.ToString()))
                {
                    await connection.OpenAsync();

                    var cmd = new NpgsqlCommand(
                        "insert into books" +
                        "(book_name,author_firstname,author_lastname,date_of_publish,publishing_name,address_id)" +
                        "values" +
                        "(@book_name,@author_firstname,@author_lastname,@date_of_publish,@publishing_name,@address_id)",
                        connection);

                    cmd.Parameters.AddWithValue("book_name", book.BookName);
                    cmd.Parameters.AddWithValue("author_firstname", book.AuthorFirstName);
                    cmd.Parameters.AddWithValue("author_lastname", book.AuthorLastName);
                    cmd.Parameters.AddWithValue("date_of_publish", book.DateOfPublish);
                    cmd.Parameters.AddWithValue("publishing_name", book.PublishingName);
                    cmd.Parameters.AddWithValue("address_id", book.PublishingAddressId);

                    await cmd.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }
        }


        public static async Task<bool> TryCreateTables()
        {
            connString.TrustServerCertificate = true;

            try
            {
                using (var connection = new NpgsqlConnection(connString.ToString()))
                {
                    await connection.OpenAsync();

                    NpgsqlCommand cmdCreateBook = new NpgsqlCommand(
                 "CREATE TABLE books " +
                 "(id serial PRIMARY KEY," +
                 "book_name VARCHAR (20)," +
                 "author_firstname VARCHAR (30) NOT NULL," +
                 "author_lastname VARCHAR (30) NOT NULL," +
                 "date_of_publish varchar(30) not null," +
                 "publishing_name VARCHAR (30) NOT NULL," +
                 "address_id int not null,foreign key (address_id) references address(id) )",
                 connection);

                    NpgsqlCommand cmdCreateAddress = new NpgsqlCommand(
                        "create table address " +
                        "(id serial primary key," +
                        "country varchar(50) not null," +
                        "city varchar(50) not null," +
                        "street varchar(50) not null," +
                        "building varchar(50) not null)", connection);

                    await cmdCreateAddress.ExecuteNonQueryAsync();

                    await cmdCreateBook.ExecuteNonQueryAsync();

                }


                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }

        }


        public static async Task<bool> TryDropTable(string table)
        {
            connString.TrustServerCertificate = true;

            try
            {
                using (var connection = new NpgsqlConnection(connString.ToString()))
                {
                    await connection.OpenAsync();

                    var cmd = new NpgsqlCommand($"drop table if exists {table}", connection);
                    await cmd.ExecuteNonQueryAsync();


                    await Console.Out.WriteLineAsync($"Table {table} was deleted!");

                    return true;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }
        }


        public static async Task<bool> TryCreateAddress(Address address)
        {
            connString.TrustServerCertificate = true;

            try
            {
                using (var connection = new NpgsqlConnection(connString.ToString()))
                {
                    await connection.OpenAsync();

                    var cmd = new NpgsqlCommand(
                        "insert into address (country, city, street, building)" +
                        "values (@country,@city,@street, @building)",
                        connection);

                    cmd.Parameters.AddWithValue("country", address.Country);
                    cmd.Parameters.AddWithValue("city", address.City);
                    cmd.Parameters.AddWithValue("street", address.Street);
                    cmd.Parameters.AddWithValue("building", address.Building);

                    await cmd.ExecuteNonQueryAsync();

                    await Console.Out.WriteLineAsync("Address inserted!");

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
