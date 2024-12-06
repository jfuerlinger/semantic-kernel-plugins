using GettingStarted.Data;
using GettingStarted.Model;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace GettingStarted.Plugins
{
    public sealed class D365Plugin
    {

        [KernelFunction, Description("Gets a list of users in elVIS")]
        public static IReadOnlyCollection<User> GetAllUsers()
        {
            return UserRepository.Instance.GetAllUsers();
        }

        [KernelFunction, Description("Gets users in elVIS with a search pattern")]
        public static IReadOnlyCollection<User> GetUsersBySearchPattern(string pattern)
        {
            return UserRepository.Instance
                .GetAllUsers()
                .Where(user =>
                           (user?.LastName?.Contains(pattern, StringComparison.OrdinalIgnoreCase) ?? false)
                        || (user?.Email?.Contains(pattern, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();

        }

        [KernelFunction, Description("Adds a new user to elVIS")]
        public static void AddUser(
            [Description("The firstname of the user to add")] string firstName,
            [Description("The lastname of the user to add")] string lastName,
            [Description("The email of the user to add")] string email)
        {
            UserRepository.Instance.AddUser(
               firstName,
               lastName,
               email);
        }
    }
}
