using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Functions;
using VerificationProvider.Models;

namespace VerificationProvider.Services;

public class ValidateVerificationCodeService(ILogger<ValidateVerificationCodeService> logger, DataContext context) : IValidateVerificationCodeService
{
    private readonly ILogger<ValidateVerificationCodeService> _logger = logger;
    private readonly DataContext _context = context;

    public async Task<ValidateRequest> UnPackValidationRequest(HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                var validateReq = JsonConvert.DeserializeObject<ValidateRequest>(body);
                if (validateReq is not null)
                {
                    return validateReq;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR :: ValidateVerificationCode.UnPackValidationRequest() : {ex.Message}");
        }
        return null!;
    }

    public async Task<bool> ValidateCodeAsync(ValidateRequest validateReq)
    {
        try
        {
            var entity = await _context.VerificationRequests.FirstOrDefaultAsync(x => x.Email == validateReq.Email && x.Code == validateReq.Code);
            if (entity is not null)
            {
                _context.VerificationRequests.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR :: ValidateVerificationCode.ValidateCodeAsync() : {ex.Message}");
        }
        return false;
    }

}
