using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedDTOModel;
using StudentAPIBusinessLayer;

namespace StudentAPI.Controllers
{
    [Route("api/StudentAPI")]
    [ApiController] // Marks the class as a Web API controller with enhanced features.
    [Produces("application/json")]
    public class StudentAPIController : ControllerBase
    {
        private readonly IStudent _student;

        public StudentAPIController(IStudent student)
        {
            _student = student;
        }

        /// <summary>
        /// Retrieves all students from the database.
        /// </summary>
        /// <returns>A list of all students, or 404 if none exist.</returns>
        [HttpGet("All", Name ="GetAllStudents")]
        [ProducesResponseType(typeof(IEnumerable<StudentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
        {
            List<StudentDTO> students = await _student.GetAllStudentsAsync();

            if (students.Count == 0)
            {
                return NotFound("No Students Found!");
            }
            return Ok(students); // Returns the list of students.
        }
    }
}
