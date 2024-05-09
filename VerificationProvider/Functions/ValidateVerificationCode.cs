using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VerificationProvider.Services;

namespace VerificationProvider.Functions
{
    public class ValidateVerificationCode
    {
        private readonly ILogger<ValidateVerificationCode> _logger;
        private readonly IValidateVerificationCodeService _validateCodeService;

        public ValidateVerificationCode(ILogger<ValidateVerificationCode> logger, IValidateVerificationCodeService validateCodeService)
        {
            _logger = logger;
            _validateCodeService = validateCodeService;
        }

        [Function("ValidateVerificationCode")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "validate")] HttpRequest req)
        {
            try
            {
                var validateReq = await _validateCodeService.UnPackValidationRequest(req);
                if(validateReq is not null)
                {
                    var validateResult = await _validateCodeService.ValidateCodeAsync(validateReq);
                    if (validateResult)
                    {
                        return new OkResult();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR :: ValidateVerificationCode.Run() : {ex.Message}");
            }
            return new UnauthorizedResult();
        }



    }
}
