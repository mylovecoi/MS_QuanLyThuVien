using System;

namespace Shared.Contracts.Interfaces
{
    public interface ICurrentUserContext
    {
        Guid? UserId { get; }
        bool IsAuthenticated { get; }
    }
}
