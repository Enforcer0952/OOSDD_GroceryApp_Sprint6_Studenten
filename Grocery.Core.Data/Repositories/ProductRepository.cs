using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        private readonly List<Product> Products = [];
        public ProductRepository()
        {

            //ISO 8601 format: date.ToString("o", CultureInfo.InvariantCulture)
            CreateTable(@"CREATE TABLE IF NOT EXISTS ProductRepository(
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] STRING NOT NULL,
                            [Stock] INTERGER NOT NULL,
                            [ShelfLife] DATE NOT NULL,
                            [Price] REAL NOT NULL,
                            UNIQUE(Name, ShelfLife))");

            List<string> insertQueries = [@"INSERT OR IGNORE INTO ProductRepository(Name, Stock, ShelfLife, Price) VALUES( Melk, 300, '2025-09-25', 0.95)",
                                            @"INSERT OR IGNORE INTO ProductRepository(Name, Stock, ShelfLife, Price) VALUES( Kaas, 100, '2025-09-30', 7.95)",
                                            @"INSERT OR IGNORE INTO ProductRepository(Name, Stock, ShelfLife, Price) VALUES( Brood, 400, '2025-09-12', 2.19)",
                                            @"INSERT OR IGNORE INTO ProductRepository(Name, Stock, ShelfLife, Price) VALUES( Cornflakes, 0, '2025-12-31', 1.48)"];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }

        public List<Product> GetAll()
        {
            Products.Clear();
            string selectQuery = "SELECT Id, Name, Stock, ShelfLife, Price FROM ProductRepository";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelflife = DateOnly.FromDateTime(reader.GetDateTime(3));
                    Decimal Price = reader.GetDecimal(4);
                    Products.Add(new(id, name, stock, shelflife, Price));
                }
            }
            CloseConnection();
            return Products;
        }

        public Product? Get(int id)
        {
            string selectQuery = $"SELECT Id, Name, date(Date), Color, ClientId FROM GroceryList WHERE Id = {id}";
            ProductRepository? PR = null;
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int Id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelflife = DateOnly.FromDateTime(reader.GetDateTime(3));
                    Decimal Price = reader.GetDecimal(4);
                    PR = (new(Id, name, stock, shelflife, Price));
                }
            }
            CloseConnection();
            return gl;
        }

        public Product Add(Product item)
        {
            int recordsAffected;
            string insertQuery = $"INSERT INTO ProductRepository(Name, Stock, ShelfLife, Price) VALUES(@Name, @Stock, @ShelfLife, @Price) Returning RowId;";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("Name", item.Name);
                command.Parameters.AddWithValue("Stock", item.Stock);
                command.Parameters.AddWithValue("Shelflife", item.ShelfLife);
                command.Parameters.AddWithValue("Price", item.Price);

                //recordsAffected = command.ExecuteNonQuery();
                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public Product? Delete(Product item)
        {
            string deleteQuery = $"DELETE FROM ProductRepository WHERE Id = {item.Id};";
            OpenConnection();
            Connection.ExecuteNonQuery(deleteQuery);
            CloseConnection();
            return item;
        }
        public Product? Update(Product item)
        {
            int recordsAffected;
            string updateQuery = $"UPDATE GroceryList SET Name = @Name, Stock = @Stock, ShelfLife = @ShelfLife, Price = @Price  WHERE Id = {item.Id};";
            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("Name", item.Name);
                command.Parameters.AddWithValue("Stock", item.Stock);
                command.Parameters.AddWithValue("ShelfLife", item.ShelfLife);
                command.Parameters.AddWithValue("Price", item.Price);

                recordsAffected = command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }
    }
}
