namespace SharedDTOModel
{
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Grade { get; set; }

        public StudentDTO() {}

        public StudentDTO(int studentId ,int userId ,string fullname, int age , int grade)
        {
            this.StudentId = studentId;
            this.UserId = userId;
            this.FullName = fullname;
            this.Age = age;
            this.Grade = grade;
        }
    }
}
