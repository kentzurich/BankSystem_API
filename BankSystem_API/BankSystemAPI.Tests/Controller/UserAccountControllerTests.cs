using API.Controllers.v1;
using API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankSystemAPI.Tests.Controller
{
    public class UserAccountControllerTests
    {
        private Mock<IMapper> _mapper;
        private Mock<UserManager<UserAccount>> _userManager;
        private Mock<SignInManager<UserAccount>> _signInManager;
        private TestServer _server;
        private UserAccountController _controller;

        public UserAccountControllerTests()
        {
            var userStore = new Mock<IUserStore<UserAccount>>();
            var builder = new WebHostBuilder().UseStartup<TestStartup>();

            _server = new TestServer(builder);
            _controller = _server.Host.Services.GetRequiredService<UserAccountController>();

            _mapper = new();
            _userManager = new(
                userStore.Object,
                null, null, null, null, null, null, null, null);
            _signInManager = new(
                _userManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<UserAccount>>(),
                null, null, null
            );
        }

        [Fact]
        public async void Controller_Should_ReturnSuccessResultOfRegister()
        {
            //Arrange
            var registerAccountDTO = new RegisterAccountDTO()
            {
                FirstName = "Kent Zurich Anthony",
                MiddleName = "Sumbrana",
                LastName = "Calderon",
                Birthdate = Convert.ToDateTime("07/01/1095"),
                Username = "kent@user.com",
                Password = "S@mple123"
            };

            _userManager.Setup(um => um.CreateAsync(It.IsAny<UserAccount>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success);

            _signInManager.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _mapper.Setup(m => m.Map<UserDTO>(It.IsAny<UserAccount>()))
                .Returns<UserDTO>(userAccount => new UserDTO
                {
                    AccountId = userAccount.AccountId,
                    FirstName = userAccount.FirstName,
                    MiddleName = userAccount.MiddleName,
                    LastName = userAccount.LastName,
                    Username = userAccount.Username
                });

            //Act
            var result = await _controller.Register(registerAccountDTO);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void Controller_Should_ReturnSuccessResultOfLogin()
        {
            //Arrange
            var registerAccountDTO = new RegisterAccountDTO()
            {
                FirstName = "Kent Zurich Anthony",
                MiddleName = "Sumbrana",
                LastName = "Calderon",
                Birthdate = Convert.ToDateTime("07/01/1095"),
                Username = "kent@user.com",
                Password = "S@mple123"
            };

            var login = new LoginAccountDTO()
            {
                Username = "kent@user.com",
                Password = "S@mple123"
            };

            var httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            httpContextMock.Setup(c => c.Response).Returns(responseMock.Object);
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextAccessorMock.Object.HttpContext,
            };

            responseMock.Setup(r => r.Cookies.Append("refreshToken", "SampleValue"));

            _userManager.Setup(um => um.CreateAsync(It.IsAny<UserAccount>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success);

            _signInManager.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _mapper.Setup(m => m.Map<UserDTO>(It.IsAny<UserAccount>()))
                .Returns<UserDTO>(userAccount => new UserDTO
                {
                    AccountId = userAccount.AccountId,
                    FirstName = userAccount.FirstName,
                    MiddleName = userAccount.MiddleName,
                    LastName = userAccount.LastName,
                    Username = userAccount.Username
                });

            //Act
            var registerResult =  await _controller.Register(registerAccountDTO);
            var loginResult = await _controller.Login(login);

            //Assert
            Assert.NotNull(registerResult);
            Assert.NotNull(loginResult);
        }
    }
}
