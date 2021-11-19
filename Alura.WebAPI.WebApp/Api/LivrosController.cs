using System;
using System.Linq;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Mvc;

namespace Alura.WebAPI.WebApp.Api
{
    [ApiController]
    [Route("[controller]")]
    // ControllerBase does not have the view support, so it is better for an api
    public class LivrosController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        [HttpGet]
        public IActionResult ListaDeLivros()
        {
            var lista = _repo.All.Select(l => l.ToModel()).ToList();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToModel());

        }

        [HttpPost]
        public IActionResult Incluir(LivroUpload model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var livro = model.ToLivro();
            _repo.Incluir(livro);
            var uri = Url.Action("Recuperar", new { id = livro.Id });
            return Created(uri, livro);
        }

        // ---- [FromBody] annotation ensure that LivroUpload model is binded from info passed via body
        [HttpPut("{id}")]
        public IActionResult Alterar([FromBody] LivroUpload model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var livro = model.ToLivro();
            if (model.Capa == null)
            {
                livro.ImagemCapa = _repo.All
                    .Where(l => l.Id == livro.Id)
                    .Select(l => l.ImagemCapa)
                    .FirstOrDefault();
            }
            _repo.Alterar(livro);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return RedirectToAction("Index", "Home");
        }
    }
}
