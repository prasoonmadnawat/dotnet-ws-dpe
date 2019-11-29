using System.Threading.Tasks;

namespace CoffeeApi.Connectors
{
    public interface IMessagePublisher
    {
        Task PublishMessageAsync (string message);
    }
}