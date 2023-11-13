using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BankSystemAPI.Tests
{
    public class TestDbContext : DataContext
    {
        public TestDbContext(DbContextOptions<DataContext> options) : base(options) { }

        public static TestDbContext GetTestDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new TestDbContext(options);
        }

        public static List<UserAccount> GetFakeAccountList()
        {
            return new List<UserAccount>
            {
                new UserAccount
                {
                    AccountId = new Guid("D1CFA332-1D16-4E0B-9D07-8FC894D57D8F"),
                    FirstName = "Bob",
                    MiddleName = "Billy",
                    LastName = "Joe",
                    Balance = 4500,
                    IsActive = true,
                    Birthdate = DateTime.UtcNow,
                    DateTimeCreated = DateTime.UtcNow,
                    UserName = "bob@username.com"
                },
                new UserAccount
                {
                    AccountId = new Guid("082DC2DF-9014-4126-A98F-EE47180FAD14"),
                    FirstName = "John",
                    MiddleName = "Wesley",
                    LastName = "Carlo",
                    Balance = 6000,
                    IsActive = true,
                    Birthdate = DateTime.UtcNow,
                    DateTimeCreated = DateTime.UtcNow,
                    UserName = "john@username.com"
                }
            };
        }
    }
}
