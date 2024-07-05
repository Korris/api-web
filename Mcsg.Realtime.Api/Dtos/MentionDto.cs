namespace Mcsg.Realtime.Api.Dtos
{
    public class MentionDto
    {
        public Guid EntityId { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
        public string Text { get; set; }
    }
}
