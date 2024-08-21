using System;
using CorePlay.SDK.Models;

namespace CorePlay.SDK.Services;

public interface IPlatformService
{
    Platform GetPlatformByIdAsync(Guid id);
    Platform GetPlatformByNameAsync(string name);
    ICollection<Platform> GetPlatformsAsync();
}
