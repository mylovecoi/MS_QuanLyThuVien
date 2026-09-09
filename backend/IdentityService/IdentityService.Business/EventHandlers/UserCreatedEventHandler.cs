using System;
using System.Threading.Tasks;

namespace IdentityService.Business.EventHandlers
{
    public class UserCreatedEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public interface IUserCreatedEventHandler
    {
        Task HandleAsync(UserCreatedEvent eventMessage);
    }

    public class UserCreatedEventHandler : IUserCreatedEventHandler
    {
        public Task HandleAsync(UserCreatedEvent eventMessage)
        {
            Console.WriteLine($"[Event Handler] UserCreatedEvent received for UserId: {eventMessage.UserId}, Email: {eventMessage.Email}");
            return Task.CompletedTask;
        }
    }
}
