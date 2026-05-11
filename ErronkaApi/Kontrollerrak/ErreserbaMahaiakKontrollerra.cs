using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Erreserbak eta mahaiak lotzeko balio du.
    /// </summary>
    [ApiController]
    [Route("api/ErreserbaMahaiak")]
    public class ErreserbaMahaiakKontrollerra : ControllerBase
    {
        private readonly ErreserbaRepository _repo;

        /// <summary>
        /// Erreserba eta mahaien lotura kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">Loturak kudeatzeko eta egiaztatzeko biltegia.</param>
        public ErreserbaMahaiakKontrollerra(ErreserbaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Mahai bat jartzen dio erreserba bati.
        /// </summary>
        /// <param name="dto">Erreserbaren eta mahaiaren zenbakiak.</param>
        /// <remarks>
        /// Ruta honek erreserba eta mahaia lotzen ditu.
        /// </remarks>
        /// <returns>
        /// Loturaren zenbakia edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Erreserba batek zein mahai dituen ikusteko.
        /// </summary>
        /// <param name="erreserbaId">Bilatu nahi den erreserbaren ID-a.</param>
        /// <remarks>
        /// Ruta honek erreserba bati lotutako mahaiak itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Mahaiaren zenbakia edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Erreserba eta mahaien arteko lotura kentzen du.
        /// </summary>
        /// <param name="erreserbaId">Lotura kendu nahi zaion erreserbaren ID-a.</param>
        /// <remarks>
        /// Ruta honek erreserba eta mahaiaren arteko lotura ezabatzen du.
        /// </remarks>
        /// <returns>
        /// Mezua esanez ez dela ezer aldatu.
        /// </returns>
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
