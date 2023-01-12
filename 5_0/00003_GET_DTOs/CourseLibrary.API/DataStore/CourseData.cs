using CourseLibrary.API.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.DataStore
{
    public class CourseData : ICourseData
    {
        public CourseData()
        {
        }
        //public IEnumerable<Course> GetAuthors()
        //{
        //    return JsonConvert.DeserializeObject<IEnumerable<Course>>(File.ReadAllText(@"DataStore\Courses.json"));
        //}

        //public void RestoreDataStore()
        //{
        //    File.Copy(@"DataStore\Courses-master.json", @"DataStore\Authors.json", true);
        //}
    }
}
