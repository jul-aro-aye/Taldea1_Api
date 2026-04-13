using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/eskaerak")]
    public class EskaeraKontrollerra : ControllerBase
    {
        private readonly EskaeraRepository _repo;

        public EskaeraKontrollerra(EskaeraRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public IActionResult SortuEskaera([FromBody] EskaeraSortuDTO dto)
        {
            var (success, error, data, details) = _repo.SortuEskaera(dto);

            if (!success)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error,
                    Datuak = details
                });
            }

            return Ok(new ErantzunaDTO<int>
            {
                Code = 200,
                Message = "Eskaera sortuta",
                Datuak = new List<int> { data.Value }
            });
        }

        [HttpGet]
        public IActionResult LortuEskaerak()
        {
            var (success, error, data) = _repo.LortuEskaerak();

            if (!success)
            {
                return StatusCode(500, new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = error,
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<EskaeraDTO>
            {
                Code = 200,
                Message = "Eskaerak lortu dira",
                Datuak = data
            });
        }

        [HttpGet("mahaia/{mahaiaId}/aktiboa")]
        public IActionResult LortuEskaeraAktiboaMahaika(int mahaiaId)
        {
            var (success, error, data) = _repo.LortuEskaeraAktiboaMahaika(mahaiaId);

            if (!success)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = error,
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<EskaeraDTO>
            {
                Code = 200,
                Message = "Eskaera lortu da",
                Datuak = new List<EskaeraDTO> { data! }
            });
        }

        [HttpGet("{eskaeraId}/produktuak")]
        public IActionResult LortuEskaeraProduktuak(int eskaeraId)
        {
            var (success, error, data) = _repo.LortuEskaeraProduktuak(eskaeraId);

            if (!success)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = error,
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<EskaeraLortuDTO>
            {
                Code = 200,
                Message = "Produktuak lortu dira",
                Datuak = data
            });
        }

        [HttpDelete("{eskaeraId}")]
        public IActionResult EzabatuEskaera(int eskaeraId)
        {
            var (success, error) = _repo.EzabatuEskaera(eskaeraId);

            if (!success)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = error,
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Eskaera ezabatuta",
                Datuak = null
            });
        }

        [HttpGet("mahaiak/{mahaiaId}/kapazitatea")]
        public IActionResult LortuMahaiKapazitatea(int mahaiaId)
        {
            var (success, error, data) = _repo.LortuMahaiKapazitatea(mahaiaId);

            if (!success)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = error,
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<int>
            {
                Code = 200,
                Message = "Kapazitatea lortuta",
                Datuak = new List<int> { data.Value }
            });
        }

        [HttpPut("{eskaeraId}")]
        public IActionResult EguneratuEskaera(int eskaeraId, [FromBody] EskaeraEguneratuDTO dto)
        {
            var (success, error, details) = _repo.EguneratuEskaera(eskaeraId, dto);

            if (!success)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error,
                    Datuak = details
                });
            }

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Eskaera eguneratuta",
                Datuak = null
            });
        }

        [HttpPut("{eskaeraId}/sukaldea-egoera")]
        public IActionResult EguneratuSukaldeaEgoera(int eskaeraId, [FromBody] EskaeraSukaldeaEgoeraDTO dto)
        {
            var (success, error) = _repo.EguneratuSukaldeaEgoera(eskaeraId, dto.SukaldeaEgoera);

            if (!success)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error,
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Sukaldea egoera eguneratuta",
                Datuak = null
            });
        }

        [HttpPost("{eskaeraId}/ordainduEskaera")]
        public IActionResult OrdainduEskaera(int eskaeraId)
        {
            var (success, error) = _repo.OrdaintzeraBidali(eskaeraId);

            if (!success)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = error,
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Eskaera ordainketara bidalita",
                Datuak = null
            });
        }

        [HttpPost("{eskaeraId}/sortuFaktura")]
        public IActionResult SortuFaktura(int eskaeraId)
        {
            var (success, error, path) = _repo.SortuFaktura(eskaeraId);

            if (!success)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error,
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Faktura sortuta",
                Datuak = new List<string> { path }
            });
        }

        [HttpGet("ordainketa-pendiente")]
        public IActionResult LortuEskaerakOrdaintzeko()
        {
            var (success, error, data) = _repo.LortuEskaerakOrdaintzeko();

            if (!success)
            {
                return StatusCode(500, new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = error,
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<EskaeraDTO>
            {
                Code = 200,
                Message = "Eskaerak lortu dira",
                Datuak = data
            });
        }
    }
}
