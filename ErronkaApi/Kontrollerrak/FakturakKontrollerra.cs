using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/Fakturak")]
    public class FakturakKontrollerra : ControllerBase
    {
        private readonly FakturaRepository _repo;

        public FakturakKontrollerra(FakturaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult LortuFakturak()
        {
            var (success, error, data) = _repo.LortuFakturak();

            if (!success)
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = error });

            return Ok(new ErantzunaDTO<FakturaDTO>
            {
                Code = 200,
                Message = "Fakturak lortu dira",
                Datuak = data
            });
        }

        [HttpGet("{id}")]
        public IActionResult LortuFaktura(int id)
        {
            var (success, error, data) = _repo.LortuFaktura(id);

            if (!success)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = error
                });
            }

            return Ok(new ErantzunaDTO<FakturaDTO>
            {
                Code = 200,
                Message = "Faktura lortu da",
                Datuak = new List<FakturaDTO> { data! }
            });
        }

        [HttpGet("erreserba/{erreserbaId}")]
        public IActionResult LortuFakturaErreserbarenArabera(int erreserbaId)
        {
            var (success, error, data) = _repo.LortuFakturaErreserbarenArabera(erreserbaId);

            if (!success)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = error
                });
            }

            return Ok(new ErantzunaDTO<FakturaDTO>
            {
                Code = 200,
                Message = "Faktura lortu da",
                Datuak = new List<FakturaDTO> { data! }
            });
        }

        [HttpPost("sortu-erreserbatik")]
        public IActionResult SortuEdoLortuFakturaErreserbatik([FromBody] FakturaErreserbaSortuDTO dto)
        {
            var (success, error, data) = _repo.SortuEdoLortuFakturaErreserbatik(dto.ErreserbaId);

            if (!success)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error
                });
            }

            return Ok(new ErantzunaDTO<FakturaDTO>
            {
                Code = 200,
                Message = "Faktura prest dago",
                Datuak = new List<FakturaDTO> { data! }
            });
        }

        [HttpDelete("{id}")]
        public IActionResult EzabatuFaktura(int id)
        {
            var (success, error) = _repo.EzabatuFaktura(id);

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
                Message = "Faktura ezabatu da"
            });
        }

        [HttpPost("eguneratu-totala")]
        public IActionResult EguneratuTotala([FromBody] FakturaTotalaEguneratuDTO dto)
        {
            var (success, error, data) = _repo.EguneratuTotala(dto.FakturaId, dto.Gehikuntza);

            if (!success)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error
                });
            }

            return Ok(new ErantzunaDTO<FakturaDTO>
            {
                Code = 200,
                Message = "Fakturaren totala eguneratu da",
                Datuak = new List<FakturaDTO> { data! }
            });
        }
    }
}
