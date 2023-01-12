using CourseLibrary.API.DataStore;
using CourseLibrary.API.Entities;
using CourseLibrary.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.API.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository
    {
        private readonly IAuthorData _authorData;

        public CourseLibraryRepository(IAuthorData authorData)
        {
            _authorData = authorData ??
                throw new ArgumentNullException(nameof(authorData));
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _authorData.GetAuthors();
        }
        public IEnumerable<Author> GetAuthors(
            //string mainCategory, string SearchQuery
            AuthorsResourceParameters authorsResourceParameters
            )
        {
            IEnumerable<Author> returnedAuthors = null;

            returnedAuthors = _authorData.GetAuthors();

            //if (string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory)
            //    && string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            //{
            //}
            //else
            //{
                if (!string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory))
                {
                    authorsResourceParameters.MainCategory = authorsResourceParameters.MainCategory.Trim();
                    returnedAuthors = returnedAuthors.Where(x => x.MainCategory == authorsResourceParameters.MainCategory).ToList();
                }

            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                authorsResourceParameters.MainCategory = authorsResourceParameters.SearchQuery.Trim();
                returnedAuthors = returnedAuthors.Where(x => x.MainCategory.Contains(authorsResourceParameters.SearchQuery)
                || x.FirstName.Contains(authorsResourceParameters.SearchQuery)
                || x.LastName.Contains(authorsResourceParameters.SearchQuery)
                ).ToList();
            }
            //}


            return returnedAuthors;
        }

        //public void RestoreDataStore()
        //{
        //    //_authorData.RestoreDataStore();
        //}


        public Author GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _authorData.GetAuthors().FirstOrDefault(a => a.Id == authorId);
        }


        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _authorData.GetAuthors().Any(a => a.Id == authorId);
        }

        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return _authorData.GetAuthors().Where(a => a.Id == authorId).First().Courses.Where(b => b.Id == courseId).FirstOrDefault();
            //return _context.Courses
            //  .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefault();
        }

        public IEnumerable<Course> GetCourses(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _authorData.GetAuthors().Where(a => a.Id == authorId).First().Courses;
            //return _context.Courses
            //            .Where(c => c.AuthorId == authorId)
            //            .OrderBy(c => c.Title).ToList();
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
                course.AuthorId = author.Id;
            }

            _authorData.Authors.Add(author);
        }


        public bool Save()
        {
            return (_authorData.SaveChanges() >= 0);
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            course.Id = Guid.NewGuid();
            _authorData.Authors.First(x => x.Id == authorId).Courses.Add(course);
        }


        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _authorData.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }


        //public void DeleteCourse(Course course)
        //{
        //    _context.Courses.Remove(course);
        //}



        //public void UpdateCourse(Course course)
        //{
        //    // no code in this implementation
        //}


        //public void DeleteAuthor(Author author)
        //{
        //    if (author == null)
        //    {
        //        throw new ArgumentNullException(nameof(author));
        //    }

        //    _context.Authors.Remove(author);
        //}



        //public void UpdateAuthor(Author author)
        //{
        //    // no code in this implementation
        //}



        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //       // dispose resources when needed
        //    }
        //}
    }
}
