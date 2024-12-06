using Bogus;
using GettingStarted.Model;

namespace GettingStarted.Data
{
    internal sealed class UserRepository
    {
        private static UserRepository? _instance;
        private List<User>? _users;

        private UserRepository()
        {
            SeedData();
        }

        private void SeedData()
        {
            _users = new Faker<User>()
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .Generate(50);
        }

        public static UserRepository Instance => _instance ??= new UserRepository();

        public IReadOnlyCollection<User> GetAllUsers()
        {
            return _users ?? [];
        }

        public void AddUser(
            string firstName,
            string lastName,
            string email)
        {
            _users?.Add(
                new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                });
        }
    }
}
