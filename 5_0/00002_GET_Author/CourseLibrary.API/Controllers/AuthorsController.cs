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

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _courseLibraryRepository.RestoreDataStore();
        }

        //////To return data, we need to add an action on our controller.
        //////IActionResult defines a contract that represents the results of an action method.
		////[HttpGet("api/authors")]
        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var authorsFromRepo = _courseLibraryRepository.GetAuthors();
            //////JsonResult is an action result which formats the given object as JSON.
            //return new JsonResult(authorsFromRepo);
            return Ok(authorsFromRepo);
        }

        //[HttpGet("{authorId:guid}")]  - with route constraint, if the method have overloading
        [HttpGet("{authorId}")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(authorFromRepo);
        }
    }
}
