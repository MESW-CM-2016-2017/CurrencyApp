using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Janus.Models;
using System;

namespace Janus.Data
{
    public class CurrencyDTODatabase
    {
        readonly SQLiteAsyncConnection database;

        public CurrencyDTODatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<CurrencyDTO>().Wait();
        }

        public Task<List<CurrencyDTO>> GetItemsAsync()
        {
            return database.Table<CurrencyDTO>().ToListAsync();
        }

        public async Task<String> GetLastUpdateTimeString()
        {
            List<CurrencyDTO> list = await database.QueryAsync<CurrencyDTO>("SELECT * FROM [CurrencyDTO] ORDER BY [RequestTimestamp] DESC");
            if (list == null || list.Count == 0) return "";
            return list[0].RequestTimestampString;
        }

        public Task<List<CurrencyDTO>> GetLastCurrencyDTO()
        {
            return database.QueryAsync<CurrencyDTO>("SELECT * FROM [CurrencyDTO] ORDER BY [RequestTimestamp] DESC LIMIT 10");
        }

        public Task<List<CurrencyDTO>> GetItemsNotDoneAsync()
        {
            return database.QueryAsync<CurrencyDTO>("SELECT * FROM [CurrencyDTO] WHERE [Done] = 0");
        }

        public Task<CurrencyDTO> GetItemAsync(int id)
        {
            return database.Table<CurrencyDTO>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(CurrencyDTO item)
        {
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }


        public async Task<List<int>> SaveItemsAsync(List<CurrencyDTO> items)
        {
            List<int> list = new List<int>();
            try
            {
                foreach (CurrencyDTO item in items)
                {
                    int id = await SaveItemAsync(item);
                    list.Add(id);
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return list;
        }

        public Task<int> DeleteItemAsync(CurrencyDTO item)
        {
            return database.DeleteAsync(item);
        }

        /*
        public async Task<List<CurrencyDTO>> GetLastUpdateTimeCurrency()
        {
            List<CurrencyDTO> list = await database.QueryAsync<CurrencyDTO>("SELECT * FROM [CurrencyDTO] ORDER BY [RequestTimestamp] DESC");
            if (list == null || list.Count == 0) return null;
            return list;
        }
        */

        public async Task<CurrencyDTO> GetLastUpdateTimeString(String symbol)
        {
            List<CurrencyDTO> list = await database.QueryAsync<CurrencyDTO>("SELECT * FROM [CurrencyDTO] WHERE [Symbol] = '" + symbol + "' ORDER BY [RequestTimestamp] DESC");
            if (list == null || list.Count == 0) return null;
            return list[0];
        }


    }
}

