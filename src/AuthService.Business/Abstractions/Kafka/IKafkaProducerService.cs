using AuthService.Shared.DTO.Kafka;

namespace AuthService.Business.Abstractions.Kafka
{
    public interface IKafkaProducerService
    {
        public Task ProduceAvatarAsync(SendAvatarKafka sendAvatarKafka);
        public Task ProduceNotificationAsync(SendNotificationKafka sendNotificationKafka);
    }
}