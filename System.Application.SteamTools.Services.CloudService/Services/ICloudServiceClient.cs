using System.Application.Services.CloudService.Clients.Abstractions;

namespace System.Application.Services
{
    public interface ICloudServiceClient
    {
        IAccountClient Account { get; }

        IAuthMessageClient AuthMessage { get; }
    }
}