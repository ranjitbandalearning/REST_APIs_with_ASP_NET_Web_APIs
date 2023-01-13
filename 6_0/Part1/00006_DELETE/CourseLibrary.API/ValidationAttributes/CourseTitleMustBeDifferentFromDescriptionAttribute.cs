using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.ValidationAttributes
{
    public class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if(validationContext.ObjectInstance is IEnumerable<CourseForManipulationDto>)
            {
                foreach (var course in (IEnumerable<CourseForManipulationDto>)validationContext.ObjectInstance)
                {
                    if (course.Title == course.Description)
                    {
                        return new ValidationResult(ErrorMessage,
                            new[] { nameof(CourseForManipulationDto) });
                    }
                }
            }
            else if(validationContext.ObjectInstance is CourseForManipulationDto)
            {
                var course = (CourseForManipulationDto)validationContext.ObjectInstance;
                if (course.Title == course.Description)
                {
                    return new ValidationResult(ErrorMessage,
                        new[] { nameof(CourseForManipulationDto) });
                }
            }
            
            return ValidationResult.Success;
        }
    }
}
