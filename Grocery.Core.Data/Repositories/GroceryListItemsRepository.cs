using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        private readonly List<GroceryListItem> groceryListItems = [];

        public GroceryListItemsRepository()
        {

            //ISO 8601 format: date.ToString("o", CultureInfo.InvariantCulture)
            CreateTable(@"CREATE TABLE IF NOT EXISTS groceryListItems (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [GroceryListID] INTERGER NOT NULL,
                            [ProductID] DATE NOT NULL,
                            [Amount] NVARCHAR(12) NOT NULL");
            List<string> insertQueries =   [@"INSERT OR IGNORE INTO groceryListItems(GroceryListID, ProductID, Amount) VALUES( 1, 1, 3)",
                                            @"INSERT OR IGNORE INTO groceryListItems(GroceryListID, ProductID, Amount) VALUES( 1, 2, 1)",
                                            @"INSERT OR IGNORE INTO groceryListItems(GroceryListID, ProductID, Amount) VALUES( 1, 3, 4)",
                                            @"INSERT OR IGNORE INTO groceryListItems(GroceryListID, ProductID, Amount) VALUES( 2, 1, 2)",
                                            @"INSERT OR IGNORE INTO groceryListItems(GroceryListID, ProductID, Amount) VALUES( 2, 2, 5)"];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();

        }

        public List<GroceryListItem> GetAll()
        {
            groceryListItems.Clear();
            string selectQuery = "SELECT Id, GroceryListID, ProductID, Amount FROM groceryListItems";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int ID = reader.GetInt32(0);
                    int GroceryListID = reader.GetInt32(1);
                    int ProductID = reader.GetInt32(2);
                    int Amount = reader.GetInt32(3);
                    int clientId = reader.GetInt32(4);
                    groceryListItems.Add(new(ID, GroceryListID, ProductID, Amount));
                }
            }
            CloseConnection();
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            return groceryListItems.Where(g => g.GroceryListId == id).ToList();
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            int newId = groceryListItems.Max(g => g.Id) + 1;
            item.Id = newId;
            groceryListItems.Add(item);
            return Get(item.Id);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return groceryListItems.FirstOrDefault(g => g.Id == id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            GroceryListItem? listItem = groceryListItems.FirstOrDefault(i => i.Id == item.Id);
            listItem = item;
            return listItem;
        }
    }
}
