using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IUserOtpRepository _userOtpRepository;
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IRepository<Session> sessionRepository
            , IUserOtpRepository userOtpRepository
            , ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
            _userOtpRepository = userOtpRepository;
        }

        [HttpPost("AddSession")]
        public async Task<string> AddSession(string test)
        {
            var ses = new Session()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "Test user name " + test
            };
            var tttt = await _sessionRepository.GetAllAsync();
            var abc = await _sessionRepository.InsertAsync(ses);

            var ffSessions = await _sessionRepository.GetByPredicateAsync(x => x.UserName == "Test user name afs");
            var firstSession = ffSessions.FirstOrDefault();
            if (firstSession != null)
            {
                firstSession.UserName = "Test user name " + test;
                firstSession.CreatedBy = "yup" + test;
                firstSession.LastModifiedDate = DateTime.Now;

                await _sessionRepository.UpdateAsync(firstSession);
            }
            var deS = await _sessionRepository.GetByPredicateAsync(x => x.UserName == "Test user name 12456");
            var firstDelete = deS.FirstOrDefault();
            if (firstDelete != null) {
                await _sessionRepository.DeleteAsync(firstDelete.Id);
            }
            
            var lst = await _sessionRepository.GetAllAsync();

            return "ssss";
        }

        [HttpPost("AddUserOtp")]
        public async Task<string> AddUserOtp(string test)
        {
            var userId = Guid.NewGuid();

            var uOtp = new UserOtp()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OtpType = Mcsg.Lib.Data.Enums.UserOtpType.VerifyEmail,
                Code = "Otp_Code_" + test,
                Token = "Token_" + test,
                ExpiryTime = DateTime.Now.AddMinutes(15),
            };
            var tttt = await _userOtpRepository.GetAllAsync();
            await _userOtpRepository.InsertAsync(uOtp);

            var tokensss = await _userOtpRepository.GetByTokenAsync("Token_" + test, UserOtpType.VerifyEmail);
            var otpTypes = await _userOtpRepository.GetByOtpTypeAsync(Mcsg.Lib.Data.Enums.UserOtpType.VerifyEmail);

            var ffSessions = await _userOtpRepository.GetByPredicateAsync(x => x.UserId == userId);

            var firstSession = ffSessions.FirstOrDefault();
            if (firstSession != null)
            {
                firstSession.CreatedBy = "TestUser_" + test;
                firstSession.LastModifiedDate = DateTime.Now;

                await _userOtpRepository.UpdateAsync(firstSession);
            }
            var deS = await _userOtpRepository.GetByPredicateAsync(x => x.UserId == userId);
            //var firstDelete = deS.FirstOrDefault();
            //if (firstDelete != null)
            //{
            //    await _userOtpRepository.DeleteAsync(firstDelete.Id);
            //}

            var lst = await _userOtpRepository.GetAllAsync();

            return "ssss";
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}