namespace AuthService.Shared.DTO.Kafka
{
    public class SendAvatarKafka
    {
        public long IDUser { get; set; }

        public string File { get; set; }

        public string FileExtension { get; set; }

        public string FileName { get; set; }
    }
}
