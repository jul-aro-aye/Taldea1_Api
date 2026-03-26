using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/mahaiak")]
    public class MahaiakController : ControllerBase
    {
        private readonly MahaiaRepository _repo;

        public MahaiakController(MahaiaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult LortuMahaiak()
        {
            var (success, error, data) = _repo.LortuMahaiak();

            if (!success)
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = error });

            return Ok(new ErantzunaDTO<MahaiaDTO>
            {
                Code = 200,
                Message = "Mahaiak lortu dira",
                Datuak = data
            });
        }

        [HttpGet("libre")]
        public IActionResult LortuMahaiLibre()
        {
            var (success, error, data) = _repo.LortuMahaiLibre();

            if (!success)
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = error });

            if (data == null || !data.Any())
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = "Ez dago mahai librerik" });

            return Ok(new ErantzunaDTO<MahaiaDTO>
            {
                Code = 200,
                Message = "Mahai libreak lortu dira",
                Datuak = data
            });
        }

        [HttpGet("{id}")]
        public IActionResult LortuMahaiBat(int id)
        {
            var (success, error, data) = _repo.LortuMahaiBat(id);

            if (!success)
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = error });

            return Ok(new ErantzunaDTO<MahaiaDTO>
            {
                Code = 200,
                Message = "Mahaia lortu da",
                Datuak = new List<MahaiaDTO> { data! }
            });
        }
    }
}
