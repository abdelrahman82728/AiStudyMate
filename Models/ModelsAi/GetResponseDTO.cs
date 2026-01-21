namespace StudyMate.API.Models.ModelsAi
{
    public class GetResponseDTO <T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public T Data { get; set; }
    }
}
