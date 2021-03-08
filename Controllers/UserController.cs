using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDShop.Data;
using DDShop.Models;
using DDShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDShop.Controllers
{
    [Route("v1/users")]
    public class UserController : Controller
    {
        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        // [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post([FromServices] DataContext context, [FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                model.Role = "employee";
                context.Users.Add(model);
                await context.SaveChangesAsync();
                model.Password = "";
                return model;
            }
            catch (Exception)
            {

                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Login([FromServices] DataContext context, [FromBody] User model)
        {
            var user = await context.Users.AsNoTracking().Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefaultAsync();
            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";

            return new
            {
                user = user,
                token = token
            };

        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return users;

        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(int id, [FromServices] DataContext context, [FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
                return NotFound("Não foi possível localizar o usuário");

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {

                return BadRequest("Não foi possível alterar o usuário");
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Delete(int id, [FromServices] DataContext context)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok("Usuário removido com sucesso");
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover o usuário" });
            }
        }
    }
}