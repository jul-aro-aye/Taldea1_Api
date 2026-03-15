using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/kategoriak")]
    public class KategoriaKontrollerra : ControllerBase
    {
        private readonly KategoriaRepository _repo;

        public KategoriaKontrollerra(KategoriaRepository repo)
        {
            _repo = repo;
        }


        [HttpGet]
        public IActionResult LortuKategoriak()
        {
            var (success, error, data) = _repo.LortuKategoriak();

            if (!success)
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = error });

            return Ok(new ErantzunaDTO<KategoriaDTO>
            {
                Code = 200,
                Message = "Kategoriak lortu dira",
                Datuak = data
            });
        }

        [HttpGet("{id}")]
        public IActionResult LortuKategoria(int id)
        {
            var (success, error, data) = _repo.LortuKategoria(id);

            if (!success)
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = error });

            return Ok(new ErantzunaDTO<KategoriaDTO>
            {
                Code = 200,
                Message = "Kategoria lortu da",
                Datuak = new List<KategoriaDTO> { data! }
            });
        }

        [HttpPost]
        public IActionResult GehituKategoria([FromBody] KategoriaDTO dto)
        {
            var (success, error) = _repo.GehituKategoria(dto);

            if (!success)
                return BadRequest(new ErantzunaDTO<string> { Code = 400, Message = error });

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Kategoria gehituta"
            });
        }

        [HttpPut("{id}")]
        public IActionResult EguneratuKategoria(int id, [FromBody] KategoriaDTO dto)
        {
            var (success, error) = _repo.EguneratuKategoria(id, dto);

            if (!success)
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = error });

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Kategoria eguneratuta"
            });
        }

        [HttpDelete("{id}")]
        public IActionResult EzabatuKategoria(int id)
        {
            var (success, error) = _repo.EzabatuKategoria(id);

            if (!success)
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = error });

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Kategoria ezabatuta"
            });
        }
    }
}
