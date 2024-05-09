using Microsoft.AspNetCore.Http;
using VerificationProvider.Models;

namespace VerificationProvider.Services
{
    public interface IValidateVerificationCodeService
    {
        Task<ValidateRequest> UnPackValidationRequest(HttpRequest req);
        Task<bool> ValidateCodeAsync(ValidateRequest validateReq);
    }
}