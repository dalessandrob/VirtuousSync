using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Sync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Sync().GetAwaiter().GetResult();
        }

        private static async Task Sync()
        {
            var apiKey = "v_eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiN2VhYTBhNTQtYTBiZC00OTNlLWFjNDMtZjNjZGEwZmVlNWQ5IiwiZXhwIjoyMTQ3NDgzNjQ3LCJpc3MiOiJodHRwczovL2FwcC52aXJ0dW91c3NvZnR3YXJlLmNvbSIsImF1ZCI6Imh0dHBzOi8vYXBpLnZpcnR1b3Vzc29mdHdhcmUuY29tIn0.oN0bfmYMS7lPxGtVH3ouEVhD0Kuzoqa2nAnuvPTyPpk";
            var configuration = new Configuration(apiKey);
            var virtuousService = new VirtuousService(configuration);

            var skip = 0;
            var take = 100;
            var maxContacts = 1000;

            //await ToCsv(virtuousService, skip, take, maxContacts);
            await ToDb(virtuousService, skip, take, maxContacts);
        }

        private static async Task ToCsv(VirtuousService virtuousService, int skip, int take, int maxContacts)
        {
            var hasMore = true;

            using (var writer = new StreamWriter($"Contacts_{DateTime.Now:MM_dd_yyyy}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                do
                {
                    var contacts = await virtuousService.GetContactsAsync(skip, take);
                    skip += take;
                    csv.WriteRecords(contacts.List);
                    hasMore = skip > maxContacts - take;
                }
                while (!hasMore);
            }
        }

        private static async Task ToDb(VirtuousService virtuousService, int skip, int take, int maxContacts)
        {
            var hasMore = true;

            string cs = "Data Source=contactSync.db"; // file-based DB
            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                string stm = "CREATE TABLE IF NOT EXISTS contacts (id INTEGER PRIMARY KEY, name TEXT, contactType TEXT, contactName TEXT, address TEXT, email TEXT, phone TEXT)";
                var cmd = new SQLiteCommand(stm, con);
                cmd.ExecuteNonQuery();

                do
                {
                    var contacts = await virtuousService.GetContactsAsync(skip, take);
                    skip += take;

                    foreach (var contact in contacts.List)
                    {
                        string insertQuery = @"
                INSERT INTO contacts (id, name, contactType, contactName, address, email, phone)
                VALUES (@Id, @Name, @ContactType, @ContactName, @Address, @Email, @Phone)";
                        using (var insertCmd = new SQLiteCommand(insertQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@Id", contact.Id);
                            insertCmd.Parameters.AddWithValue("@Name", contact.Name);
                            insertCmd.Parameters.AddWithValue("@ContactType", contact.ContactType);
                            insertCmd.Parameters.AddWithValue("@ContactName", contact.ContactName);
                            insertCmd.Parameters.AddWithValue("@Address", contact.Address);
                            insertCmd.Parameters.AddWithValue("@Email", contact.Email);
                            insertCmd.Parameters.AddWithValue("@Phone", contact.Phone);
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    hasMore = skip > maxContacts - take;
                }
                while (!hasMore);
            };
        }
    }
}
