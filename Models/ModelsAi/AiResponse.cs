namespace StudyMate.API.Models.ModelsAi
{
    public class AiResponse <T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public T Data { get; set; }
    }
}
