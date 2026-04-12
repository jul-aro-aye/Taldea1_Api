namespace ErronkaApi.DTOak
{
    public class EskaeraSortuDTO
    {
        public int ErabiltzaileId { get; set; }
        public int MahaiaId { get; set; }
        public int Komensalak { get; set; }
        public int? ErreserbaId { get; set; }
        public List<EskaeraProduktuaSortuDTO> Produktuak { get; set; }
    }

}
