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
        List<Author> _authors = null;


        public AuthorData()
        {
        }

        public List<Author> Authors { 
            get
            {
                if(_authors == null || !_authors.Any())
                {
                    _authors = GetAuthors().ToList();
                }
                return _authors;
            }
            //set
            //{
            //    if(value != null)
            //    {
            //        _authors.AddRange(value);
            //    }
            //}
        }


        public string path = @"DataStore\Authors.json";
        public IEnumerable<Author> GetAuthors()
        {
            var authors = JsonConvert.DeserializeObject<IEnumerable<Author>>(File.ReadAllText(path));
            return authors;
        }

        public void RestoreDataStore()
        {
            File.Copy(@"DataStore\Authors-master.json", @"DataStore\Authors.json", true);
        }

        public int SaveChanges()
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this.Authors));
            return 0;
        }
    }
}
