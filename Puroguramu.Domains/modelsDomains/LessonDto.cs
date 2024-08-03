namespace Puroguramu.Domains.modelsDomains
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public int Order { get; set; }
        public int CompletedExercises { get; set; }
        public int TotalExercises { get; set; }

        public int TotalStudents { get; set; }

        public int StudentsWhoCompleted { get; set; }
    }

}
