using Microsoft.AspNetCore.Mvc;

namespace ControleDeEstoque.WebApp.Compartilhado;

public class HomeController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}
