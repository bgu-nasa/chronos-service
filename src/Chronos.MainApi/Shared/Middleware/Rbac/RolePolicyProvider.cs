using Chronos.Domain.Management.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Chronos.MainApi.Shared.Middleware.Rbac;

public class RolePolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith("OrgRole:"))
        {
            var role = Enum.Parse<Role>(policyName["OrgRole:".Length..]);
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new RequireOrgRole(role))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        if (policyName.StartsWith("DeptRole:"))
        {
            var role = Enum.Parse<Role>(policyName["DeptRole:".Length..]);
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new RequireDeptRole(role))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        _fallback.GetFallbackPolicyAsync();
}