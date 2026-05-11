namespace ErronkaApi.DTOak
{
    /// <summary>
    /// APIak ematen duen erantzun orokorra.
    /// </summary>
    /// <remarks>
    /// Kontrolatzaile guztiek erantzun formatu bera erabiltzeko balio du.
    /// </remarks>
    public class ErantzunaDTO<T>
    {
        /// <summary>
        /// Egoera kodea.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Erantzunaren mezua.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Bidaltzen diren datuak.
        /// </summary>
        public List<T>? Datuak { get; set; }
    }
}
