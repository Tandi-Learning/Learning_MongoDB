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
            Console.WriteLine("Q to Quit");
            while (true)
            {
                var cmd = Console.ReadLine();
                if (string.Compare(cmd, "Q", true) == 0)
                    break;
                AddPerson();
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

        static async void AddPerson()
        {
            var person = new Person
            {
                Name = "Tandi",
                Age = 40,
                Colors = new List<string> { "Red", "Yellow"},
                Pets = new List<Pet> { new Pet { Name = "Colby", Type = "Poodle" } },
                ExtraElements = new BsonDocument("Some Random Name", "Some Random Food")
            };

            var people = Open<Person>("test", "people");
            if (!people.AsQueryable().Where(p => p.Name == "Tandi").Any())
                await people.InsertOneAsync(person);

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
