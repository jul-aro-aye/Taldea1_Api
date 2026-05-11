using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Mahaiak eta libre dauden ikusteko balio du.
    /// </summary>
    /// <remarks>
    /// Mahaiak egunaren eta txandaren arabera iragazi daitezke.
    /// </remarks>
    [ApiController]
    [Route("api/mahaiak")]
    public class MahaiakController : ControllerBase
    {
        private readonly MahaiaRepository _repo;

        /// <summary>
        /// Mahaien kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">
        /// Mahaien egoera kalkulatzeko biltegia.
        /// </param>
        /// <remarks>
        /// Biltegiak badaki zein mahai dauden hartuta edo ordaintzeko zain.
        /// </remarks>
        public MahaiakController(MahaiaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Mahai guztiak eta beraien egoera lortzen ditu.
        /// </summary>
        /// <param name="data">
        /// Zein egunetan ikusi nahi den (aukerakoa).
        /// </param>
        /// <param name="txanda">
        /// Bazkaria edo afaria (aukerakoa).
        /// </param>
        /// <returns>
        /// Mahaien zerrenda.
        /// </returns>
        /// <remarks>
        /// Iragazkirik jarri ezean, une honetako egoera erakusten du.
        /// </remarks>
        [HttpGet]
        public IActionResult LortuMahaiak([FromQuery] DateTime? data = null, [FromQuery] string? txanda = null)
        {
            var (success, error, dataResult) = _repo.LortuMahaiak(data, txanda);

            if (!success)
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = error });

            return Ok(new ErantzunaDTO<MahaiaDTO>
            {
                Code = 200,
                Message = "Mahaiak lortu dira",
                Datuak = dataResult
            });
        }

        /// <summary>
        /// Libre dauden mahaiak bakarrik lortzen ditu.
        /// </summary>
        /// <param name="data">
        /// Zein egunetan (aukerakoa).
        /// </param>
        /// <param name="txanda">
        /// Zein txandatan (aukerakoa).
        /// </param>
        /// <returns>
        /// Mahai libreen zerrenda edo errore mezua.
        /// </returns>
        /// <remarks>
        /// Erreserba bat edo eskari bat egin aurretik erabiltzeko egokia da.
        /// </remarks>
        [HttpGet("libre")]
        public IActionResult LortuMahaiLibre([FromQuery] DateTime? data = null, [FromQuery] string? txanda = null)
        {
            var (success, error, dataResult) = _repo.LortuMahaiLibre(data, txanda);

            if (!success)
                return StatusCode(500, new ErantzunaDTO<string> { Code = 500, Message = error });

            if (dataResult == null || !dataResult.Any())
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = "Ez dago mahai librerik" });

            return Ok(new ErantzunaDTO<MahaiaDTO>
            {
                Code = 200,
                Message = "Mahai libreak lortu dira",
                Datuak = dataResult
            });
        }

        /// <summary>
        /// Mahai jakin baten egoera lortzen du.
        /// </summary>
        /// <param name="id">
        /// Mahaiaren zenbakia.
        /// </param>
        /// <param name="data">
        /// Eguna (aukerakoa).
        /// </param>
        /// <param name="txanda">
        /// Txanda (aukerakoa).
        /// </param>
        /// <returns>
        /// Mahaiaren datuak edo errore mezua.
        /// </returns>
        /// <remarks>
        /// Mahaiaren informazioa bakarrik erakusten du.
        /// </remarks>
        [HttpGet("{id}")]
        public IActionResult LortuMahaiBat(int id, [FromQuery] DateTime? data = null, [FromQuery] string? txanda = null)
        {
            var (success, error, dataResult) = _repo.LortuMahaiBat(id, data, txanda);

            if (!success)
                return NotFound(new ErantzunaDTO<string> { Code = 404, Message = error });

            return Ok(new ErantzunaDTO<MahaiaDTO>
            {
                Code = 200,
                Message = "Mahaia lortu da",
                Datuak = new List<MahaiaDTO> { dataResult! }
            });
        }
    }
}
