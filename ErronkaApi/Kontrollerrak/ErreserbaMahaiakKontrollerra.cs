using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/ErreserbaMahaiak")]
    public class ErreserbaMahaiakKontrollerra : ControllerBase
    {
        private readonly ErreserbaRepository _repo;

        public ErreserbaMahaiakKontrollerra(ErreserbaRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public IActionResult GehituMahaiErreserbara([FromBody] ErreserbaMahaiDTO dto)
        {
            var (success, error, data) = _repo.EzarriMahaia(dto.ErreserbakId, dto.MahaiakId);

            if (!success)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error
                });
            }

            return Ok(new ErantzunaDTO<int>
            {
                Code = 200,
                Message = "Mahaia erreserbara gehitu da",
                Datuak = new List<int> { data!.Value }
            });
        }

        [HttpGet("erreserba/{erreserbaId}")]
        public IActionResult LortuMahaiakErreserbarentzat(int erreserbaId)
        {
            var (success, error, data) = _repo.LortuMahaiakErreserbarentzat(erreserbaId);

            if (!success)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = error
                });
            }

            return Ok(new ErantzunaDTO<int>
            {
                Code = 200,
                Message = "Erreserbako mahaiak lortu dira",
                Datuak = data
            });
        }

        [HttpDelete("erreserba/{erreserbaId}")]
        public IActionResult EzabatuMahaiakErreserbatik(int erreserbaId)
        {
            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Ez da aldaketarik egin"
            });
        }
    }
}
