namespace BankSystemAPI.Tests.Handler
{
    public class TransferCashHandler
    {
        private TestDbContext _dbContext;
        private Mock<IMapper> _mapper;
        private Mock<IUserAccessor> _userAccessor;
        private Mock<IValidator<TransferCash.Command>> _validator;
        public TransferCashHandler()
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
            List<UserAccount> userList = TestDbContext.GetFakeAccountList();
            var command = new TransferCash.Command 
            {
                ReceiverAccountId = new Guid("D1CFA332-1D16-4E0B-9D07-8FC894D57D8F"), 
                Amount = 1000 
            };
            var expectedResult = 1000;
            
            _dbContext.UserAccounts.AddRange(userList);
            _dbContext.SaveChanges();

            _userAccessor.Setup(ua => ua.GetUserName()).Returns("john@username.com");

            _validator.Setup(v => v.Validate(It.IsAny<TransferCash.Command>()))
                   .Returns(new ValidationResult());

            _mapper.Setup(m => m.Map<TransactionDTO>(It.IsAny<TransactionDTO>()))
                .Returns<UserAccount>(userAccount => new TransactionDTO
                {
                    Amount = userAccount.Balance
                });

            var handler = new TransferCash.Handler(
                _dbContext,
                _mapper.Object,
                _userAccessor.Object,
                _validator.Object);

            //Act
            var result = await handler.Handle(command, default);


            //Assert
            Assert.IsType<Result<TransactionDTO>>(result);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedResult, result.Value.Amount);
        }
    }
}
