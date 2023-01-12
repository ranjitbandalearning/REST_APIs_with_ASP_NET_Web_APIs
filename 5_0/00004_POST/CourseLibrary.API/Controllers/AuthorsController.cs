using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    //////basics we encounter when we implement the outer-facing contract for the API throughout this course, create controller, inject repository, create action methods, and return results.
    //////Think about requiring attribute routing, automatically returning a 400 Bad Request on bad input, and returning problem details on errors. 
    [ApiController]
    //[Route("api/[controller]")]   //refactoring the code will be a problem with this approach
    [Route("api/authors")]
    //////This ControllerBase class contains basic functionality controllers need like access to the model state, the current user, and common methods for returning responses.
    //////We could also inherit it from Controller, but by doing so, we'd also add support for views, which isn't needed when building an API.
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            //_courseLibraryRepository.RestoreDataStore();
        }

        //////To return data, we need to add an action on our controller.
        //////IActionResult defines a contract that represents the results of an action method.
        ////[HttpGet("api/authors")]
        [HttpGet()]
        [HttpHead()]
        //public IActionResult GetAuthors()
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
            //[FromQuery]string mainCategory
            //[FromQuery(Name = "mainCategory")] string mainCategory
            //string mainCategory,
            //string searchQuery
            //BY DEFAULT THE COMPLEX TYPE IS EXPECTED TO BE BIND USING FROMBODY RESOURCE, AS CURRENTLY WE ARE SENDING VIA QUERY PARAMETERS
            //WE HAVE TO EXPLICITY MENTION AS FROMQUERY TO POPULATE THIS COMPLEX TYPE USING QUERY PARAMETER VALUES
            [FromQuery]AuthorsResourceParameters authorsResourceParameters
            )
        {
            var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsResourceParameters);
            //var authors = new List<AuthorDto>();

            //foreach (var author in authorsFromRepo)
            //{
            //    authors.Add(new AuthorDto()
            //    {
            //        Id = author.Id,
            //        Name = $"{author.FirstName} {author.LastName}",
            //        MainCategory = author.MainCategory,
            //        Age = author.DateOfBirth.GetCurrentAge()
            //    });
            //}
            var authors = _mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
            //////JsonResult is an action result which formats the given object as JSON.
            //return new JsonResult(authorsFromRepo);
            return Ok(authors);
        }

        //[HttpGet("{authorId:guid}")]  - with route constraint, if the method have overloading
        [HttpGet("{authorId}", Name = "GetAuthor")]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
        }


        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
        {
            var authorEntity = _mapper.Map<Entities.Author>(author);
            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor",
                new { authorId = authorToReturn.Id },
                authorToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
    }
}
