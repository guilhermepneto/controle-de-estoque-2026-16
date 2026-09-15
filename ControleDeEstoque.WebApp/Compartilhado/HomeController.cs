using ControleDeEstoque.WebApp.ModuloProdutos;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeEstoque.WebApp.Compartilhado;

public class HomeController : Controller
{
    private readonly RepositorioProdutoEmArquivo repositorioProduto;

    public HomeController(RepositorioProdutoEmArquivo repositorioProduto)
    {
        this.repositorioProduto = repositorioProduto;
    }

    public ActionResult Index()
    {
        List<Produto> produtos = repositorioProduto.SelecionarTodos();

        ViewBag.TotalProdutos = produtos.Count;
        ViewBag.TotalEstoque = produtos.Sum(p => p.QuantidadeEmEstoque);

        return View();
    }
}