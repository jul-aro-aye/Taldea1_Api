using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace ErronkaApi.Kontrollerrak
{
    [ApiController]
    [Route("api/erabiltzaileak")]
    public class ErabiltzaileaKontrollerra : ControllerBase
    {
        private readonly ErabiltzaileaRepository _repo;

        public ErabiltzaileaKontrollerra(ErabiltzaileaRepository repo)
        {
            _repo = repo;
        }

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
    }
}
