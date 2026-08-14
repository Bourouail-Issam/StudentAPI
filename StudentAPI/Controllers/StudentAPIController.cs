using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudentsAsync()
        {
            List<StudentDTO> students = await _student.GetAllStudentsAsync();

            if (students.Count == 0)
            {
                return NotFound("No Students Found!");
            }
            return Ok(students); // Returns the list of students.
        }

        /// <summary>
        /// Retrieves all Passed students from the database.
        /// </summary>
        /// <returns>A list of all Passed students, or 404 if none exist.</returns>
        [HttpGet("Passed", Name = "GetPassedStudents")]
        [ProducesResponseType(typeof(IEnumerable<StudentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetPassedStudentsAsync()
        {
            List<StudentDTO> passesStudentsList = await _student.GetPassedStudentsAsync();

            if (passesStudentsList.Count == 0)
            {
                return NotFound("No Students Found!");
            }
            return Ok(passesStudentsList); // Returns the list of students.
        }

        /// <summary>
        /// Retrive Average Grade of All Student from database.
        /// </summary>
        /// <returns>The average grade of all students, or 404 if no students exist.</returns>
        [HttpGet("AverageGrade", Name = "GetAverageGrade")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<decimal>> GetAverageGradeAsync()
        {
            try
            {
                decimal averageGrade = await _student.GetAverageGradeAsync();
                return Ok(averageGrade);
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return NotFound("No students found.");
            }
        }

    }
}
