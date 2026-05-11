using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Mahaien erreserbak kudeatzeko balio du.
    /// </summary>
    [ApiController]
    [Route("api/Erreserbak")]
    public class ErreserbaKontrollerra : ControllerBase
    {
        private readonly ErreserbaRepository _repo;

        /// <summary>
        /// Erreserben kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">Erreserbak egiteko eta egiaztatzeko biltegia.</param>
        public ErreserbaKontrollerra(ErreserbaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Gordeta dauden erreserba guztiak lortzen ditu.
        /// </summary>
        /// <remarks>
        /// Ruta honek erreserba guztiak itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Erreserba guztien zerrenda.
        /// </returns>
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

        /// <summary>
        /// Data jakin bateko erreserbak lortzen ditu.
        /// </summary>
        /// <param name="data">Bilatu nahi den eguna.</param>
        /// <remarks>
        /// Ruta honek data jakin bateko erreserbak itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Data horretako erreserben zerrenda.
        /// </returns>
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

        /// <summary>
        /// Erreserba berri bat egiten du.
        /// </summary>
        /// <param name="dto">Erreserba egiteko datuak.</param>
        /// <remarks>
        /// Ruta honek erreserba berria sortzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Sortutako erreserba edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Gordeta dagoen erreserba bat aldatzen du.
        /// </summary>
        /// <param name="id">Aldatu nahi den erreserbaren ID-a.</param>
        /// <param name="dto">Erreserba berriaren datuak.</param>
        /// <remarks>
        /// Ruta honek erreserba eguneratzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Aldatutako erreserba edo errore mezua.
        /// </returns>
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

        /// <summary>
        /// Erreserba bat ezabatzen du.
        /// </summary>
        /// <param name="id">Ezabatu nahi den erreserbaren ID-a.</param>
        /// <remarks>
        /// Ruta honek erreserba ezabatzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Mezua erreserba ezabatu dela esanez.
        /// </returns>
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
