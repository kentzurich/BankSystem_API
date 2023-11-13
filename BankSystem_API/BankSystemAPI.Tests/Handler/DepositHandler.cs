namespace BankSystemAPI.Tests.Handler
{
    public class DepositHandler
    {
        private TestDbContext _dbContext;
        private Mock<IMapper> _mapper;  
        private Mock<IUserAccessor> _userAccessor;
        private Mock<IValidator<Deposit.Command>> _validator;
        public DepositHandler()
        {
            _mapper = new();
            _userAccessor = new();
            _dbContext = TestDbContext.GetTestDbContext();
            _validator = new();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResultNotNullExpectedResultMustEqualToBalance()
        {
            // Arrange
            List<UserAccount> userList = TestDbContext.GetFakeAccountList();

            var command = new Deposit.Command { Amount = 500 };
            var expectedResult = 6500;

            _dbContext.UserAccounts.AddRange(userList);
            _dbContext.SaveChanges();

            _userAccessor.Setup(ua => ua.GetUserName()).Returns("john@username.com");

            _validator.Setup(v => v.Validate(It.IsAny<Deposit.Command>()))
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

            var handler = new Deposit.Handler(_dbContext, 
                _mapper.Object, 
                _userAccessor.Object, 
                _validator.Object);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.IsType<Result<AccountDTO>>(result);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedResult, result.Value.Balance);
        }
    }
}
