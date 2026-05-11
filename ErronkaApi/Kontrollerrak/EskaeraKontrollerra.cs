using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Jatetxeko eskarien bizitza osoa kudeatzen du.
    /// </summary>
    [ApiController]
    [Route("api/eskaerak")]
    public class EskaeraKontrollerra : ControllerBase
    {
        private readonly EskaeraRepository _repo;

        /// <summary>
        /// Eskarien kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">Eskariak, stock-a eta fakturak kudeatzeko biltegia.</param>
        public EskaeraKontrollerra(EskaeraRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Eskari berri bat sortzen du mahai bati lotuta.
        /// </summary>
        /// <param name="dto">Eskaria irekitzeko datuak.</param>
        /// <remarks>
        /// Ruta honek eskaera berria sortzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Sortutako eskariaren zenbakia edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Une honetan irekita dauden eskari guztiak lortzen ditu.
        /// </summary>
        /// <remarks>
        /// Ruta honek eskaera guztiak itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Eskari guztien zerrenda.
        /// </returns>
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

        /// <summary>
        /// Mahai batek une honetan duen eskari aktiboa lortzen du.
        /// </summary>
        /// <param name="mahaiaId">Mahaiaren ID-a.</param>
        /// <param name="data">Bilatu nahi den eguna.</param>
        /// <param name="txanda">Bilatu nahi den txanda.</param>
        /// <remarks>
        /// Ruta honek mahai jakin bateko eskaera aktiboa itzultzen du.
        /// </remarks>
        /// <returns>
        /// Eskari aktiboa edo errore mezua.
        /// </returns>
        [HttpGet("mahaia/{mahaiaId}/aktiboa")]
        public IActionResult LortuEskaeraAktiboaMahaika(
            int mahaiaId,
            [FromQuery] DateTime? data = null,
            [FromQuery] string? txanda = null)
        {
            var (success, error, dataResult) = _repo.LortuEskaeraAktiboaMahaika(mahaiaId, data, txanda);

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
                Datuak = new List<EskaeraDTO> { dataResult! }
            });
        }

        /// <summary>
        /// Eskari baten barruan dauden produktu guztiak lortzen ditu.
        /// </summary>
        /// <param name="eskaeraId">Eskariaren ID-a.</param>
        /// <remarks>
        /// Ruta honek eskaera bati lotutako produktuak itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Produktuen zerrenda edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Eskari bat ezabatzen du.
        /// </summary>
        /// <param name="eskaeraId">Ezabatu nahi den eskariaren ID-a.</param>
        /// <remarks>
        /// Ruta honek eskaera ezabatzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua eskaria ezabatu dela esanez.
        /// </returns>
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

        /// <summary>
        /// Mahai baten lekua (kapazitatea) ikusteko.
        /// </summary>
        /// <param name="mahaiaId">Mahaiaren ID-a.</param>
        /// <remarks>
        /// Ruta honek mahai jakin baten kapazitatea itzultzen du.
        /// </remarks>
        /// <returns>
        /// Mahaiaren lekua edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Eskari bateko pertsona kopurua edo produktuak aldatzen ditu.
        /// </summary>
        /// <param name="eskaeraId">Aldatu nahi den eskariaren ID-a.</param>
        /// <param name="dto">Eskariaren datu berriak.</param>
        /// <remarks>
        /// Ruta honek eskaera eguneratzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua eskaria eguneratu dela esanez.
        /// </returns>
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

        /// <summary>
        /// Eskari baten sukaldeko egoera aldatzen du.
        /// </summary>
        /// <param name="eskaeraId">Eskariaren ID-a.</param>
        /// <param name="dto">Sukaldeko egoera berria.</param>
        /// <remarks>
        /// Ruta honek sukaldeko egoera eguneratzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua egoera aldatu dela esanez.
        /// </returns>
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

        /// <summary>
        /// Eskaria ordaintzera bidaltzen du (deskonturik gabe).
        /// </summary>
        /// <param name="eskaeraId">Eskariaren ID-a.</param>
        /// <remarks>
        /// Ruta honek eskaera ordaintzera bidaltzen du.
        /// </remarks>
        /// <returns>
        /// Mezua ordainketara bidali dela esanez.
        /// </returns>
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

        /// <summary>
        /// Eskaria ordaintzera bidaltzen du deskontu batekin.
        /// </summary>
        /// <param name="eskaeraId">Eskariaren ID-a.</param>
        /// <param name="dto">Deskontuaren datuak.</param>
        /// <remarks>
        /// Ruta honek eskaera ordaintzera bidaltzen du deskontu batekin.
        /// </remarks>
        /// <returns>
        /// Mezua ordainketara bidali dela esanez.
        /// </returns>
        [HttpPost("{eskaeraId}/ordainduEskaeraDeskontuarekin")]
        public IActionResult OrdainduEskaeraDeskontuarekin(int eskaeraId, [FromBody] EskaeraOrdainduDTO dto)
        {
            var (success, error) = _repo.OrdaintzeraBidali(eskaeraId, dto);

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

        /// <summary>
        /// Eskari baten faktura (PDF) sortzen du.
        /// </summary>
        /// <param name="eskaeraId">Eskariaren ID-a.</param>
        /// <remarks>
        /// Ruta honek faktura berria sortzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// PDF fitxategiaren bidea edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Ordaintzeko zain dauden eskariak lortzen ditu.
        /// </summary>
        /// <remarks>
        /// Ruta honek ordaintzeko zain dauden eskaerak itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Eskarien zerrenda.
        /// </returns>
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
