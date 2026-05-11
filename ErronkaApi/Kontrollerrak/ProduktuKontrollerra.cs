using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Produktuen katalogoa kudeatzeko balio du.
    /// </summary>
    [ApiController]
    [Route("api/produktuak")]
    public class ProduktuakKontrollera : ControllerBase
    {
        private readonly ProduktuaRepository _repo;

        /// <summary>
        /// Produktuen kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">Produktuak kudeatzeko biltegia.</param>
        public ProduktuakKontrollera(ProduktuaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Produktu guztien zerrenda lortzen ditu.
        /// </summary>
        /// <remarks>
        /// Ruta honek produktu guztiak itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Produktuen zerrenda.
        /// </returns>
        [HttpGet]
        public IActionResult LortuProduktuak()
        {
            var (success, error, data) = _repo.LortuProduktuak();

            if (!success)
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = error });

            return Ok(new ErantzunaDTO<ProduktuaDTO>
            {
                Code = 200,
                Message = "Produktuak lortu dira",
                Datuak = data
            });
        }

        /// <summary>
        /// Produktu jakin bat lortzen du bere zenbakia erabiliz.
        /// </summary>
        /// <param name="id">Produktuaren ID-a.</param>
        /// <remarks>
        /// Ruta honek ID bidez produktu bat bilatzen du.
        /// </remarks>
        /// <returns>
        /// Produktuaren datuak edo errore mezua.
        /// </returns>
        [HttpGet("{id}")]
        public IActionResult LortuProduktua(int id)
        {
            var (success, error, data) = _repo.LortuProduktua(id);

            if (!success)
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = error });

            return Ok(new ErantzunaDTO<ProduktuaDTO>
            {
                Code = 200,
                Message = "Produktua lortu da",
                Datuak = new List<ProduktuaDTO> { data! }
            });
        }

        /// <summary>
        /// Produktu berri bat sortzen du katalogoan.
        /// </summary>
        /// <param name="dto">Produktu berriaren datuak.</param>
        /// <remarks>
        /// Ruta honek produktu berria sortzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua produktua gehitu dela esanez.
        /// </returns>
        [HttpPost]
        public IActionResult GehituProduktua([FromBody] ProduktuaDTO dto)
        {
            var (success, error) = _repo.GehituProduktua(dto);

            if (!success)
                return BadRequest(new ErantzunaDTO<string> { Code = 400, Message = error });

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Produktua gehituta"
            });
        }

        /// <summary>
        /// Produktu baten datuak aldatzen ditu.
        /// </summary>
        /// <param name="id">Aldatu nahi den produktuaren ID-a.</param>
        /// <param name="dto">Produktuaren datu berriak.</param>
        /// <remarks>
        /// Ruta honek produktu baten datuak eguneratzen ditu datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua produktua eguneratu dela esanez.
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult EguneratuProduktua(int id, [FromBody] ProduktuaDTO dto)
        {
            var (success, error) = _repo.EguneratuProduktua(id, dto);

            if (!success)
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = error });

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Produktua eguneratuta"
            });
        }

        /// <summary>
        /// Produktu bat ezabatzen du.
        /// </summary>
        /// <param name="id">Ezabatu nahi den produktuaren ID-a.</param>
        /// <remarks>
        /// Ruta honek produktu bat ezabatzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua produktua ezabatu dela esanez.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult EzabatuProduktua(int id)
        {
            var (success, error) = _repo.EzabatuProduktua(id);

            if (!success)
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = error });

            return Ok(new ErantzunaDTO<string>
            {
                Code = 200,
                Message = "Produktua ezabatuta"
            });
        }
    }
}
