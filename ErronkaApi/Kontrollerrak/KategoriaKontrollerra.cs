using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Produktuen kategoriak kudeatzeko balio du.
    /// </summary>
    [ApiController]
    [Route("api/kategoriak")]
    public class KategoriaKontrollerra : ControllerBase
    {
        private readonly KategoriaRepository _repo;

        /// <summary>
        /// Kategorien kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">Kategoriak kudeatzeko biltegia.</param>
        public KategoriaKontrollerra(KategoriaRepository repo)
        {
            _repo = repo;
        }


        /// <summary>
        /// Kategoria guztiak lortzen ditu.
        /// </summary>
        /// <remarks>
        /// Ruta honek kategoria guztiak itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Kategorien zerrenda.
        /// </returns>
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

        /// <summary>
        /// Kategoria jakin bat lortzen du bere zenbakia erabiliz.
        /// </summary>
        /// <param name="id">Kategoriaren ID-a.</param>
        /// <remarks>
        /// Ruta honek ID bidez kategoria bilatzen du.
        /// </remarks>
        /// <returns>
        /// Kategoriaren datuak edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Kategoria berri bat sortzen du.
        /// </summary>
        /// <param name="dto">Kategoria berriaren datuak.</param>
        /// <remarks>
        /// Ruta honek kategoria berria sortzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua kategoria gehitu dela esanez.
        /// </returns>
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

        /// <summary>
        /// Kategoria bat aldatzen du.
        /// </summary>
        /// <param name="id">Aldatu nahi den kategoriaren ID-a.</param>
        /// <param name="dto">Kategoriaren datu berriak.</param>
        /// <remarks>
        /// Ruta honek kategoria eguneratzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua kategoria eguneratu dela esanez.
        /// </returns>
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

        /// <summary>
        /// Kategoria bat ezabatzen du.
        /// </summary>
        /// <param name="id">Ezabatu nahi den kategoriaren ID-a.</param>
        /// <remarks>
        /// Ruta honek kategoria ezabatzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua kategoria ezabatu dela esanez.
        /// </returns>
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
