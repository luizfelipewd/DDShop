using System.Threading.Tasks;
using DDShop.Data;
using DDShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace DDShop.Controllers
{
    [Route("v1")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context)
        {
            var manager = new User { Id = 1, Username = "gerente", Password = "gerente", Role = "manager" };
            var employee = new User { Id = 2, Username = "funcionario", Password = "funcionario", Role = "employee" };
            var category = new Category { Id = 1, Title = "Categoria 1" };
            var product = new Product { Id = 1, Title = "Produto 1", Category = category, Price = 299, Description = "Descrição do produto" };

            context.Users.Add(manager);
            context.Users.Add(employee);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Dados configurados com sucesso"
            });


        }
    }
}