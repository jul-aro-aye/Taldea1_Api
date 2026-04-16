namespace ErronkaApi.DTOak
{
    public class ErabiltzaileaLoginDTO
    {
        public int id { get; set; }
        public string erabiltzailea { get; set; }
        public string emaila { get; set; }
        public int rolaId { get; set; }
        public string rolaIzena { get; set; }
        public bool txat { get; set; }
    }
}
