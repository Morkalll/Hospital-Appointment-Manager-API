using TPI_2026.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace TPI_2026.Infrastructure.ExternalServices;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}
