using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApi.DataBaseContext;
using UserApi.Interfaces;
using UserApi.Model;
using UserApi.Requests;

namespace UserApi.Service
{
    public class UserLoginService : IUsersLoginsService
    {
        private readonly ContextDb _context;

        public UserLoginService(ContextDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetAllUsersAsync()
        {
            // Объединяем данные из таблиц Users и Emails
            var usersWithEmails = await _context.Users
                .Join(
                    _context.Emails,
                    user => user.id_User,
                    email => email.User_id,
                    (user, email) => new
                    {
                        id_User = user.id_User,
                        Name = user.Name,
                        Description = user.Descrioption,
                        Email = email.Email
                    }
                )
                .ToListAsync();

            return new OkObjectResult(new
            {
                data = new { users = usersWithEmails },
                status = true
            });
        }

      

        public async Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUserAndLogin newUser)
        {
            var user = new Users()
            {
                Name = newUser.Name,
                Descrioption = newUser.Description,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var login = new Emails()
            {
                User_id = user.id_User,
                Email = newUser.Email,
                Password = newUser.Password,
            };

            await _context.Emails.AddAsync(login);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true
            });
        }
    }
}
