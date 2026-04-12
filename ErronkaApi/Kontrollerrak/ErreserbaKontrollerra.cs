using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/Erreserbak")]
    public class ErreserbaKontrollerra : ControllerBase
    {
        private readonly ErreserbaRepository _repo;

        public ErreserbaKontrollerra(ErreserbaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult LortuErreserbak()
        {
            var (success, error, data) = _repo.LortuErreserbak();

            if (!success)
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = error });

            return Ok(new ErantzunaDTO<ErreserbaDTO>
            {
                Code = 200,
                Message = "Erreserbak lortu dira",
                Datuak = data
            });
        }

        [HttpGet("data/{data}")]
        public IActionResult LortuErreserbakData(string data)
        {
            if (!DateTime.TryParse(data, out var aukeratutakoData))
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = "Data ez da zuzena"
                });
            }

            var (success, error, datuak) = _repo.LortuErreserbakDatarenArabera(aukeratutakoData);

            if (!success)
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = error });

            return Ok(new ErantzunaDTO<ErreserbaDTO>
            {
                Code = 200,
                Message = "Erreserbak lortu dira",
                Datuak = datuak
            });
        }

        [HttpPost]
        public IActionResult SortuErreserba([FromBody] ErreserbaDTO dto)
        {
            var (success, error, data) = _repo.SortuErreserba(dto);

            if (!success)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error
                });
            }

            return Ok(new ErantzunaDTO<ErreserbaDTO>
            {
                Code = 200,
                Message = "Erreserba sortu da",
                Datuak = new List<ErreserbaDTO> { data! }
            });
        }

        [HttpPut("{id}")]
        public IActionResult EguneratuErreserba(int id, [FromBody] ErreserbaDTO dto)
        {
            var (success, error, data) = _repo.EguneratuErreserba(id, dto);

            if (!success)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error
                });
            }

            return Ok(new ErantzunaDTO<ErreserbaDTO>
            {
                Code = 200,
                Message = "Erreserba eguneratu da",
                Datuak = new List<ErreserbaDTO> { data! }
            });
        }

        [HttpDelete("{id}")]
        public IActionResult EzabatuErreserba(int id)
        {
            var (success, error) = _repo.EzabatuErreserba(id);

            if (!success)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = error
                });
            }

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Erreserba ezabatu da"
            });
        }
    }
}
