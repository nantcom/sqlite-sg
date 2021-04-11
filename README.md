# NC.SQLite + NC.SQLite.SourceGen

This repository is a SQLite lightweight ORM based on [SQLite-net](https://github.com/praeclarum/sqlite-net). The goal is to provide more convinient API to work with queries, improve performance and support generating [TableMapping](https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L2427) by using [C# Source Generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) 

As I am using SQLite-net for at least 8 years - the plan was to make this project as a drop-in replacement for SQLite-net. However, after looking at SQLite-net code more closely, I have decided to get rid many of the overloads and redundant features.

# Getting Started
1. Install Nuget Package **NC.SQLite.SourceGen** using your preferred method.
2. Attach Attributes to your class.
   - **Table** : Specify that the class should be mapped
   - **PrimaryKey** : Specify that the property should be Primary Key. Guid is supported as PrimaryKey. _**Unlike SQLite-net, There is no implicit Primary Key in NC.SQLite**_
   - **AutoIncrement** : Specfily alongside PrimaryKey to make it auto-increment (running number)
   - **Indexed** : Create Index by specifying name, order and whether to make unique. Multiple properties in same Index Name will be put in the same index.
````cs
  [Table("MyDataTable")]
  public class MyData
  {
      [PrimaryKey, AutoIncrement]
      public int Id { get; set; }

      public string MyProperty { get; set; }
  }
````
3. Use SQLiteConnection to Insert/Update/Delete/Select
````cs
MyData p = new MyData();
connection.Insert(p);

// p.Id get assigned with new Id
Assert.IsTrue(p.Id != 0);

p.MyProperty = $"New Value {DateTime.Now.Ticks}";
// Update or Insert. Same as .Insert(p, true)
connection.Upsert(p);

// Use LinqTo to build SELECT query
var lastInserted = connection.LinqTo<MyData>().OrderByDescending(d => d.Id).Result().FirstOrDefault();

Assert.IsTrue(lastInserted.MyProperty == p.MyProperty);

// Or Write your own query - Parameters are supported using ? in Query
var countResult = connection.QueryDeferred("SELECT Count(Id) as IdCount FROM MyDataTable", (stmt) =>
{
    return new
    {
        IdCount = SQLite3.ColumnInt(stmt, 0)
    };

}).FirstOrDefault();

Assert.IsTrue(countResult.IdCount > 0);
````

# Main Differences to SQLite-net

- The project (currently) does not use [SQLitePCLRaw.bundle_green](https://www.nuget.org/packages/sqlite-net-pcl/1.7.335) for its SQLite library. I was having too much fun messing with the code before realizing that there are 2 ways to access SQLite library in SQLite-net and it's too late to go back and change (it's a minor change, but I am not going to do it yet :P).    
Since I am using this project within another product that target Windows, NC.SQLite will deploy its own SQLite.dll at first run (for either x86/x64) for now. If you want to use in Linux Docker, you would need to include your own Sqlite build.

- All Queries in SQLiteConnection / SQLiteCommand are "Deferred" - which means it would not execute until you start iterating over the result. There are both Deferred and Not-Deferred variants in SQLite-net which is too confusing for me.

- There is a flag in Constructor which stops "Prepared Statement" (Parsed Statement) from getting disposed at the end of each command excution. In SQLite-net it is called "PreparedSqliteInsertCommand" and not available for normal SQLiteCommand.

- Since NC.SQLite was designed to used with its SourceGen, there is no run-time generation of TableMapping class. If you are not using NC.SQLite.SourceGen, there will be no Mapping Created and you would not be able to do anything unless implementing your own TableMapping Class.

- There is no Implcit Index/Implicit Primary Key in NC.SQLite-net in order to prevent confusion.

# Improvements over SQLite-net

- **You can run any complex query againts database.**    
In SQLite-net, the query must be returning your Mapped Types. For example, you could not do "SELECT SUM(Cost) as TotalCost FROM Product" unless you create a table mapping.    
In NC.SQLite, you can directly read the result per-row and project (return new...) to any type:  
````cs
var now = DateTime.Now;
FrameTimeLog log = new FrameTimeLog()
{
    FrameTime = 1,
    ProcessName = "Sample",
    SessionName = "Session",
    TimeStamp = now
};

SQLiteConnection connection = new SQLiteConnection($"{TestFolder}\\test.sqlite");
connection.Insert(log);

// Query any arbitary result without mapping
var result = connection.QueryDeferred("SELECT TimeStamp FROM FrameTime ORDER BY TimeStamp DESC", (IReader r) =>
{
    return new
    {
        TimeStamp = r.ReadCurrentRow<long>("TimeStamp")
    };
});

Assert.IsTrue(result.First().TimeStamp == now.Ticks, "Not Inserted");
````
- To prevent confusion, [TableQuery](https://github.com/praeclarum/sqlite-net/blob/ff6507e2accd79ab60aa84a1039215884e4118fa/src/SQLite.cs#L3613) no longer implements IEnumerable. In order to finalize the query and get the result, use **.Result()** to enumerate through result of your mapped type or **.Select()** to project the result. **ResultCount()** is also provided for counting the results.
````cs
var result = connection.LinqTo<FrameTimeLog>()
            .Where(l => l.FrameTime == 123)
            .Result().FirstOrDefault();
````
````cs
var result = connection.LinqTo<FrameTimeLog>()
            .Where(l => l.FrameTime == 123)
            .Select((reader) =>
            {
                return new
                {
                    DoubleFT = reader.ReadCurrentRow<int>("FrameTime") * 2
                };

            }).FirstOrDefault();
````
````cs
var result = connection.LinqTo<FrameTimeLog>()
            .Where(l => l.FrameTime == 123)
            .ResultCount();
````
