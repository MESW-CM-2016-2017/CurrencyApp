using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Janus.Models;

namespace Janus
{
    public class WalletDatabase
    {
        readonly SQLiteAsyncConnection database;

        public WalletDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Wallet>().Wait();
        }

        public Task<List<Wallet>> GetItemsAsync()
        {
            return database.Table<Wallet>().ToListAsync();
        }

        public Task<List<Wallet>> GetItemsNotDoneAsync()
        {
            return database.QueryAsync<Wallet>("SELECT * FROM [Wallet] WHERE [Done] = 0");
        }

        public Task<Wallet> GetItemAsync(int id)
        {
            return database.Table<Wallet>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(Wallet item)
        {
            // if same symbol sum 
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public async Task<int> SaveOrUpdateItemAsync(Wallet item)
        {
            List<Wallet> wallets = await App.Database.GetItemsAsync();
            if (wallets.Count > 0)
            {
                foreach (Wallet w in wallets)
                {
                    if (w.Symbol.Equals(item.Symbol))
                    {
                        w.Quantity = w.Quantity + item.Quantity;
                        return await App.Database.SaveItemAsync(w);
                    }
                }
            }
            return await App.Database.SaveItemAsync(item);
        }

        public Task<int> DeleteItemAsync(Wallet item)
        {
            return database.DeleteAsync(item);
        }
    }
}

