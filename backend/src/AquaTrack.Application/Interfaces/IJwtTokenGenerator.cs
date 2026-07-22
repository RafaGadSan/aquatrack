using AquaTrack.Application.Common;
using AquaTrack.Domain.Entities;

namespace AquaTrack.Application.Interfaces;

public interface IJwtTokenGenerator
{
    GeneratedToken GenerateToken(User user);
}
