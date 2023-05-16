﻿using despesas_backend_api_net_core.Business.Generic;
using despesas_backend_api_net_core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace despesas_backend_api_net_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : Controller
    {
        private IBusiness<Categoria> _categoriaBusiness;

        public CategoriaController(IBusiness<Categoria> categoriaBusiness)
        {
            _categoriaBusiness = categoriaBusiness;
        }

        [HttpGet]
        [Authorize("Bearer")]
        public IActionResult Get()
        {
            return Ok(_categoriaBusiness.FindAll());
        }

        [HttpGet("{id}")]
        [Authorize("Bearer")]
        public IActionResult Get(int id)
        {
            Categoria _categoria = _categoriaBusiness.FindById(id);

            if (_categoria == null)
                return NotFound();

            return Ok(_categoria);
        }

        [HttpGet("byTipoCategoria/{idUsuario}/{idTipoCategoria}")]
        [Authorize("Bearer")]
        public IActionResult GetByTipoCategoria([FromRoute] int idUsuario, [FromRoute] TipoCategoria tipoCategoria)
        {
            var _categoria = _categoriaBusiness.FindAll()
                .FindAll(prop => prop.IdTipoCategoria.Equals(tipoCategoria) &&
                                (prop.IdUsuario.Equals(idUsuario) ||
                                 prop.IdUsuario == null ||
                                 prop.IdUsuario.Equals(0)));

            if (tipoCategoria.Equals(1))
            {
                _categoria.Add(new Categoria
                {
                    Id = 1,
                    Descricao = "Alimentação",
                    IdTipoCategoria = 1                    
                });
                _categoria.Add(new Categoria
                {
                    Id = 2,
                    Descricao = "Casa",
                    IdTipoCategoria = 1
                });
                _categoria.Add(new Categoria
                {
                    Id = 3,
                    Descricao = "Serviços",
                    IdTipoCategoria = 1
                });
                _categoria.Add(new Categoria
                {
                    Id = 4,
                    Descricao = "Saúde",
                    IdTipoCategoria = 1
                });
                _categoria.Add(new Categoria
                {
                    Id = 5,
                    Descricao = "Imposto",
                    IdTipoCategoria = 1
                });
                _categoria.Add(new Categoria
                {
                    Id = 6,
                    Descricao = "Transporte",
                    IdTipoCategoria = 1
                });
                _categoria.Add(new Categoria
                {
                    Id = 7,
                    Descricao = "Lazer",
                    IdTipoCategoria = 1
                });
                _categoria.Add(new Categoria
                {
                    Id = 8,
                    Descricao = "Outros",
                    IdTipoCategoria = 1
                }); 
            }
            else
            {
                _categoria.Add(new Categoria
                {
                    Id = 1,
                    Descricao = "Salário",
                    IdTipoCategoria = 2
                });
                _categoria.Add(new Categoria
                {
                    Id = 2,
                    Descricao = "Prêmio",
                    IdTipoCategoria = 2
                });
                _categoria.Add(new Categoria
                {
                    Id = 3,
                    Descricao = "Investimento",
                    IdTipoCategoria = 2
                });
                _categoria.Add(new Categoria
                {
                    Id = 4,
                    Descricao = "Benefício",
                    IdTipoCategoria = 2
                });
                _categoria.Add(new Categoria
                {
                    Id = 5,
                    Descricao = "Outros",
                    IdTipoCategoria = 2
                });
            }
            if (_categoria == null)
                return NotFound();

            return Ok(_categoria);
        }

        [HttpPost]
        [Authorize("Bearer")]
        public IActionResult Post([FromBody] Categoria categoria)
        {
            if (categoria == null)
                return BadRequest();

            try
            {
                return new ObjectResult(new { message = true, categoria = _categoriaBusiness.Create(categoria) });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível realizar o cadastro de uma nova categoria, tente mais tarde ou entre em contato com o suporte." });
            }
        }

        [HttpPut]
        [Authorize("Bearer")]
        public IActionResult Put([FromBody] Categoria categoria)
        {
            if (categoria == null)
                return BadRequest();

            Categoria updateCategoria = _categoriaBusiness.Update(categoria);
            if (updateCategoria == null)
                return NoContent();

            return new ObjectResult(updateCategoria);
        }

        [HttpDelete("{id}")]
        [Authorize("Bearer")]
        public IActionResult Delete(int id)
        {
            _categoriaBusiness.Delete(id);
            return NoContent();
        }
    }
}