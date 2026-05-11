using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Saioa hasteko eta baimenak ikusteko balio du.
    /// </summary>
    /// <remarks>
    /// Kontrolatzaile honek erabiltzailearen oinarrizko datuak bakarrik ematen ditu.
    /// </remarks>
    [ApiController]
    [Route("api/login")]
    public class LoginKontrollera : ControllerBase
    {
        private readonly ErabiltzaileaRepository _repo;

        /// <summary>
        /// Login kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">
        /// Erabiltzaileak egiaztatzeko biltegia.
        /// </param>
        /// <remarks>
        /// Pasahitza eta erabiltzailea biltegian begiratzen dira.
        /// </remarks>
        public LoginKontrollera(ErabiltzaileaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Erabiltzailearen datuak egiaztatu eta sartzeko baimena ematen du.
        /// </summary>
        /// <param name="dto">
        /// Erabiltzaile izena eta pasahitza.
        /// </param>
        /// <returns>
        /// Sartzeko baimena (OK) edo errore mezua (Unauthorized).
        /// </returns>
        /// <remarks>
        /// Erantzunak bezeroak behar dituen datuak bakarrik ditu, segurtasunagatik.
        /// </remarks>
        [HttpPost]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            var (success, error, user) = _repo.Login(dto.erabiltzailea, dto.pasahitza);

            if (!success)
            {
                return Unauthorized(new ErantzunaDTO<string>
                {
                    Code = 401,
                    Message = error,
                    Datuak = null
                });
            }

            var datuak = new
            {
                user.id,
                user.erabiltzailea,
                user.emaila,
                user.rola,
                user.txat
            };

            return Ok(new ErantzunaDTO<object>
            {
                Code = 200,
                Message = "Login egokia",
                Datuak = new List<object> { datuak }
            });
        }

        /// <summary>
        /// Erabiltzaileak txata erabili dezakeen ikusten du.
        /// </summary>
        /// <param name="erabiltzaileId">
        /// Erabiltzailearen zenbakia.
        /// </param>
        /// <returns>
        /// Txat baimena edo errore mezua.
        /// </returns>
        /// <remarks>
        /// Bai edo ez (true/false) bueltatzen du baimenaren arabera.
        /// </remarks>
        [HttpGet("{erabiltzaileId}/txat")]
        public IActionResult LortuTxatBaimena(int erabiltzaileId)
        {
            var (success, exists, error, txat) = _repo.LortuTxatBaimena(erabiltzaileId);

            if (!success)
            {
                return StatusCode(500, new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = error
                });
            }

            if (!exists)
            {
                return NotFound(new ErantzunaDTO<string>
                {
                    Code = 404,
                    Message = "Erabiltzailea ez da aurkitu"
                });
            }

            return Ok(new ErantzunaDTO<bool>
            {
                Code = 200,
                Message = "Txat baimena lortu da",
                Datuak = new List<bool> { txat }
            });
        }
    }
}
