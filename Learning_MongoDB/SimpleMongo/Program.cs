using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;

namespace SimpleMongo
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();
            bool keepLoop = true;
            while (keepLoop)
            {
                Console.WriteLine("(A) Add - (F) Find - (D) Delete - (Q) Quit");
                var cmd = Console.ReadLine();
                //if (string.Compare(cmd, "Q", true) == 0)
                //    break;
                switch (cmd.ToUpper())
                {
                    case "A":
                        AddPerson();
                        break;
                    case "F":
                        Console.Write("Filter = ");
                        cmd = Console.ReadLine();
                        FindPerson(cmd);
                        break;
                    case "D":
                        break;
                    case "Q":
                        keepLoop = false;
                        break;
                }
            }
        }

        static void Init()
        {
            //BsonClassMap.RegisterClassMap<Person>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.MapMember(x => x.Name).SetElementName("name");
            //});

            var conventionPack = new ConventionPack();
            conventionPack.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
        }

        private static async void FindPerson(string cmd)
        {
            var people = Open<Person>("test", "people");
            //var filter = Builders<Person>.Filter.Eq("Name", "Tandi");
            //var list = await people.Find(filter).ToListAsync();

            cmd = cmd.Replace("\"","\\\"");
            var list = await people.Find("{name:\"Tandi\"}").ToListAsync();
            foreach (var doc in list)
            {
                Console.WriteLine(doc.ToBsonDocument());
            }

            //await people.Find(new BsonDocument())
            //    .ForEachAsync(d => Console.WriteLine(d.ToBsonDocument()));
        }

        static async void AddPerson()
        {
            var person = new Person
            {
                Name = "Tandi",
                Age = 40,
                Colors = new List<string> { "Red", "Yellow" },
                Pets = new List<Pet> { new Pet { Name = "Colby", Type = "Poodle" } },
                ExtraElements = new BsonDocument("Some Random Name", "Some Random Food")
            };

            var people = Open<Person>("test", "people");
            if (!people.AsQueryable().Where(p => p.Name == "Tandi").Any())
                await people.InsertOneAsync(person);

            var persons = new List<Person>
            {
                new Person
                {
                    Name = "John",
                    Age = 35,
                    Colors = new List<string> { "Yellow", "Black", "Blue" },
                    Pets = new List<Pet> { new Pet { Name = "Shelby", Type = "Cat" } },
                    ExtraElements = new BsonDocument("What is my name", "My name is John")
                },
                new Person
                {
                    Name = "Paul",
                    Age = 38,
                    Colors = new List<string> { "Green", "Silver", "Cyan", "Red" },
                    Pets = new List<Pet> { new Pet { Name = "Kat", Type = "Lizard" } },
                }
            };

            List<string> names = new List<string> { "John", "Paul" };
            if (!people.AsQueryable().Where(p => names.Contains(p.Name)).Any())
                await people.InsertManyAsync(new[] { persons[0], persons[1] });

            //using (var writer = new JsonWriter(Console.Out))
            //{
            //    BsonSerializer.Serialize(writer, person);
            //};
        }

        private static IMongoCollection<T> Open<T>(string dbname, string collection)
        {
            var connString = "mongodb://localhost:27017";
            var client = new MongoClient(connString);

            var db = client.GetDatabase(dbname);

            var col = db.GetCollection<T>(collection);
            return col;
        }

        static void ProcessCommand(string command)
        {

            var doc = new BsonDocument
            {
                { "Name", "Jones" },
            };

            var hobbies = new BsonArray
            {
                "Fishing", "Hunting"
            };
            doc.Add("Hobbies", hobbies);

            Console.WriteLine(doc);
        }
    }

    public class Person
    {
        public ObjectId Id { get; set; }
        //[BsonElement("name")]
        public string Name { get; set; }
        //[BsonRepresentation(BsonType.String)]
        public int Age { get; set; }
        public List<string> Colors { get; set; }
        public List<Pet> Pets { get; set; }
        public BsonDocument ExtraElements { get; set; }
    }

    public class Pet
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
