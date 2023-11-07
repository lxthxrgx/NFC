using SQLite;
using System;
using System.Diagnostics;

namespace NFC
{
    public class SQLiteDataCanvas
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string UID { get; set; }
        public string Technologies { get; set; }
    }

    public class SQLiteActions
    {
        public void SaveToDB(string name, string uid, string technologies)
        {
            try
            {
                var dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "mydatabase.db");
                var db = new SQLiteConnection(dbPath);
                db.CreateTable<SQLiteDataCanvas>();
                var dataToDb = new SQLiteDataCanvas
                {
                    Name = name,
                    UID = uid,
                    Technologies = technologies
                };
                db.Insert(dataToDb);
                Debug.WriteLine("File path: " + dbPath);
                db.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка при сохранении в базу данных: " + ex.Message);
            }
        }

        public void ReadFromDB()
        {
            var dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "mydatabase.db");
            var db = new SQLiteConnection(dbPath);
            var items = db.Table<SQLiteDataCanvas>().ToList();
            foreach (var item in items)
            {
                Debug.WriteLine($"ID: {item.Id}, Name: {item.Name}, UID: {item.UID}, Technologies: {item.Technologies}");
            }
            db.Close();
        }
    }

}