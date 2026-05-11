using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Fakturak ikusteko eta kudeatzeko balio du.
    /// </summary>
    [ApiController]
    [Route("api/Fakturak")]
    public class FakturakKontrollerra : ControllerBase
    {
        private readonly FakturaRepository _repo;

        /// <summary>
        /// Fakturen kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">Fakturak kudeatzeko biltegia.</param>
        public FakturakKontrollerra(FakturaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Gordeta dauden faktura guztiak lortzen ditu.
        /// </summary>
        /// <remarks>
        /// Ruta honek faktura guztiak itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Faktura guztien zerrenda.
        /// </returns>
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

        /// <summary>
        /// Faktura jakin bat lortzen du bere zenbakia erabiliz.
        /// </summary>
        /// <param name="id">Fakturaren ID-a.</param>
        /// <remarks>
        /// Ruta honek ID bidez faktura bilatzen du.
        /// </remarks>
        /// <returns>
        /// Fakturaren datuak edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Erreserba bati lotutako azken faktura lortzen du.
        /// </summary>
        /// <param name="erreserbaId">Erreserbaren ID-a.</param>
        /// <remarks>
        /// Ruta honek erreserba bati lotutako azken faktura itzultzen du.
        /// </remarks>
        /// <returns>
        /// Fakturaren datuak edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Erreserba batetik faktura bat sortzen du edo dagoena lortu.
        /// </summary>
        /// <param name="dto">Erreserbaren ID-a.</param>
        /// <remarks>
        /// Ruta honek erreserba batetik faktura bat sortzen du.
        /// </remarks>
        /// <returns>
        /// Faktura prest dagoela dioen mezua.
        /// </returns>
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

        /// <summary>
        /// Faktura bat ezabatzen du.
        /// </summary>
        /// <param name="id">Ezabatu nahi den fakturaren ID-a.</param>
        /// <remarks>
        /// Ruta honek faktura ezabatzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua faktura ezabatu dela esanez.
        /// </returns>
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

        /// <summary>
        /// Faktura baten guztizko zenbatekoa aldatzen du.
        /// </summary>
        /// <param name="dto">Fakturaren ID-a eta gehikuntza.</param>
        /// <remarks>
        /// Ruta honek fakturaren guztizko zenbatekoa eguneratzen du.
        /// </remarks>
        /// <returns>
        /// Faktura eguneratua edo errore mezua.
        /// </returns>
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
