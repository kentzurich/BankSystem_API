namespace BankSystemAPI.Tests.Handler
{
    public class BalanceInquiryHandler
    {
        private Mock<IMapper> _mapper;
        private Mock<IUserAccessor> _userAccessor;
        private TestDbContext _dbContext;
        public BalanceInquiryHandler()
        {
            _mapper = new();
            _userAccessor = new();
            _dbContext = TestDbContext.GetTestDbContext();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResultNotNullExpectedResultMustEqualToUser()
        {
            // Arrange
            List<UserAccount> userList = TestDbContext.GetFakeAccountList();
            var expectedUserResult = userList.Where(f => f.UserName == "bob@username.com").FirstOrDefault();

            _dbContext.UserAccounts.AddRange(userList);
            _dbContext.SaveChanges();

            _userAccessor.Setup(ua => ua.GetUserName()).Returns("bob@username.com");

            _mapper.Setup(m => m.Map<AccountDTO>(It.IsAny<UserAccount>()))
                .Returns<UserAccount>(userAccount => new AccountDTO
                {
                    AccountId = userAccount.AccountId,
                    FirstName = userAccount.FirstName,
                    MiddleName = userAccount.MiddleName,
                    LastName = userAccount.LastName,
                    Balance = userAccount.Balance
                });

            var handler = new BalanceInquiry.Handler(_dbContext, _mapper.Object, _userAccessor.Object);

            // Act
            var result = await handler.Handle(null, default);

            // Assert
            Assert.IsType<Result<AccountDTO>>(result);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedUserResult.AccountId, result.Value.AccountId);
        }
    }
}
