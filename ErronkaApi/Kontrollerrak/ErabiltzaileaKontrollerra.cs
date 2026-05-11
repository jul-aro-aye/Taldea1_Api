using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// Aretoko erabiltzaileak ikusteko eta sortzeko balio du.
    /// </summary>
    [ApiController]
    [Route("api/erabiltzaileak")]
    public class ErabiltzaileaKontrollerra : ControllerBase
    {
        private readonly ErabiltzaileaRepository _repo;

        /// <summary>
        /// Erabiltzaileen kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">Erabiltzaileak kudeatzeko biltegia.</param>
        public ErabiltzaileaKontrollerra(ErabiltzaileaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Saioa hasteko prest dauden erabiltzaileak lortzen ditu.
        /// </summary>
        /// <remarks>
        /// Ruta honek IDa, izena, posta, rola eta txat baimena itzultzen ditu.
        /// </remarks>
        /// <returns>
        /// Erabiltzaileen zerrenda edo errore mezua.
        /// </returns>
        [HttpGet("login")]
        public IActionResult LortuLoginErabiltzaileak()
        {
            var (success, error, users) = _repo.LortuAktiboak();

            if (!success || users == null)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error ?? "Errorea erabiltzaileak lortzean",
                    Datuak = null
                });
            }

            var datuak = users.Select(user => new ErabiltzaileaLoginDTO
            {
                id = user.id,
                erabiltzailea = user.erabiltzailea,
                emaila = user.emaila,
                rolaId = user.rola?.id ?? 0,
                rolaIzena = user.rola?.izena ?? string.Empty,
                txat = user.txat
            }).ToList();

            return Ok(new ErantzunaDTO<ErabiltzaileaLoginDTO>
            {
                Code = 200,
                Message = "Erabiltzaileak zuzen lortu dira",
                Datuak = datuak
            });
        }

        /// <summary>
        /// Aretoko erabiltzaile berri bat sortzen du.
        /// </summary>
        /// <param name="eskaera">Erabiltzaile berriaren datuak.</param>
        /// <remarks>
        /// Ruta honek zerbitzari berri bat sortzen du datu-basean.
        /// </remarks>
        /// <returns>
        /// Sortutako erabiltzailea edo errore mezua.
        /// </returns>
        [HttpPost]
        public IActionResult SortuZerbitzaria([FromBody] ErabiltzaileaSortuDTO eskaera)
        {
            var (success, error, user) = _repo.SortuZerbitzaria(
                eskaera.erabiltzailea,
                eskaera.emaila,
                eskaera.pasahitza,
                eskaera.txat);

            if (!success || user == null)
            {
                return BadRequest(new ErantzunaDTO<string>
                {
                    Code = 400,
                    Message = error ?? "Errorea zerbitzaria sortzean",
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<ErabiltzaileaLoginDTO>
            {
                Code = 200,
                Message = "Zerbitzaria zuzen sortu da",
                Datuak = new List<ErabiltzaileaLoginDTO>
                {
                    new ErabiltzaileaLoginDTO
                    {
                        id = user.id,
                        erabiltzailea = user.erabiltzailea,
                        emaila = user.emaila,
                        rolaId = user.rola?.id ?? 0,
                        rolaIzena = user.rola?.izena ?? string.Empty,
                        txat = user.txat
                    }
                }
            });
        }
    }
}
