namespace Chronos.MainApi.Auth.Services;

// TODO One day
public interface IVerificationService
{
    Task VerifyUserAsync(Guid organizationId, Guid userId, string verificationCode);
}