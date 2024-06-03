namespace Mcsg.Realtime.Api.DTOs.Mention
{
    public class MentionDto
    {
        public Guid EntityId { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
        public string Text { get; set; }
    }
}
