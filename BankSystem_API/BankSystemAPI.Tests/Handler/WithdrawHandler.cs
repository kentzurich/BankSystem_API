using FluentValidation;

namespace BankSystemAPI.Tests.Handler
{
    public class WithdrawHandler
    {
        private TestDbContext _dbContext;
        private Mock<IMapper> _mapper;
        private Mock<IUserAccessor> _userAccessor;
        private Mock<IValidator<Withdraw.Command>> _validator;
        public WithdrawHandler()
        {
            _mapper = new();
            _userAccessor = new();
            _dbContext = TestDbContext.GetTestDbContext();
            _validator = new();
        }

        [Fact]
        public async void Handle_Should_ReturnSuccessResultNotNullExpectedResultMustEqualToBalance()
        {
            // Arrange
            UserAccount user = TestDbContext.GetFakeAccountList()
                .Where(f => f.UserName == "john@username.com").FirstOrDefault()!;

            var command = new Withdraw.Command { Amount = 1000 };
            var expectedResult = 5000;

            _dbContext.UserAccounts.Add(user);
            _dbContext.SaveChanges();

            _userAccessor.Setup(ua => ua.GetUserName()).Returns("john@username.com");

            _validator.Setup(v => v.Validate(It.IsAny<Withdraw.Command>()))
                   .Returns(new ValidationResult());

            _mapper.Setup(m => m.Map<AccountDTO>(It.IsAny<UserAccount>()))
                .Returns<UserAccount>(userAccount => new AccountDTO
                {
                    AccountId = userAccount.AccountId,
                    FirstName = userAccount.FirstName,
                    MiddleName = userAccount.MiddleName,
                    LastName = userAccount.LastName,
                    Balance = userAccount.Balance
                });

            var handler = new Withdraw.Handler(
                _dbContext,
                _mapper.Object,
                _userAccessor.Object,
                _validator.Object);

            //Act
            var result = await handler.Handle(command, default);


            //Assert
            Assert.IsType<Result<AccountDTO>>(result);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedResult, result.Value.Balance);
        }
    }
}
