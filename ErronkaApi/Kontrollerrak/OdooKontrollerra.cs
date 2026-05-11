using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    /// <summary>
    /// APIa eta Odoo sistema lotzeko balio du.
    /// </summary>
    /// <remarks>
    /// Erabiltzaileak, produktuak, mahaiak, eskariak eta fakturak pakete bakarrean bidaltzen ditu.
    /// </remarks>
    [ApiController]
    [Route("api/odoo")]
    public class OdooKontrollerra : ControllerBase
    {
        private readonly OdooRepository _repo;

        /// <summary>
        /// Odoo kontrolatzailea hasieratzen du.
        /// </summary>
        /// <param name="repo">
        /// Datuak Odoorako prestatzen dituen biltegia.
        /// </param>
        /// <remarks>
        /// Datu guztiak Odook ulertzen duen formatura pasatzen dira biltegian.
        /// </remarks>
        public OdooKontrollerra(OdooRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Odoorako beharrezko datu guztiak lortzen ditu.
        /// </summary>
        /// <returns>
        /// Sinkronizazio datuak edo errore mezua.
        /// </returns>
        /// <remarks>
        /// Endpoint honek datu guztiak batera bidaltzen ditu kanpoko sistemara.
        /// </remarks>
        [HttpGet("sinkronizazioa")]
        public IActionResult LortuSinkronizazioDatuak()
        {
            var (success, error, data) = _repo.LortuSinkronizazioDatuak();

            if (!success || data == null)
            {
                return StatusCode(500, new ErantzunaDTO<string>
                {
                    Code = 500,
                    Message = error ?? "Errorea sinkronizazio datuak eskuratzean",
                    Datuak = null
                });
            }

            return Ok(new ErantzunaDTO<OdooSyncDTO>
            {
                Code = 200,
                Message = "Odoorako sinkronizazio datuak eskuratu dira",
                Datuak = new List<OdooSyncDTO> { data }
            });
        }
    }
}
