using CourseLibrary.API.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.DataStore
{
    public class AuthorData : IAuthorData
    {
        public AuthorData()
        {
        }
        public IEnumerable<Author> GetAuthors()
        {
            var authors = JsonConvert.DeserializeObject<IEnumerable<Author>>(File.ReadAllText(@"DataStore\Authors.json"));
            return authors;
        }

        public void RestoreDataStore()
        {
            File.Copy(@"DataStore\Authors-master.json", @"DataStore\Authors.json", true);
        }
    }
}
