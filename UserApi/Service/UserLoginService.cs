using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
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
                        id_Email = email.id_Email,
                        id_User = user.id_User,
                        Email = email.Email,
                        Name = user.Name,
                        Descrioption = user.Descrioption,
                        Role = email.Role
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
                Descrioption = newUser.Descrioption,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var login = new Emails()
            {
                User_id = user.id_User,
                Email = newUser.Email,
                Password = newUser.Password,
                Role = "user"
            };

            await _context.Emails.AddAsync(login);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true
            });
        }

        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var email = await _context.Emails.FindAsync(id);
            var user = await _context.Users.FindAsync(email.User_id);

            if (email == null)
            {
                return new NotFoundObjectResult(new { Message = "Такого пользователь нет!" });
            }
            _context.Users.Remove(user);
            _context.Emails.Remove(email);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
    }
}
