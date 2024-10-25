namespace Microservicio_Infonet.Models
{
    public class ClienteInfonet
    {
        public int id { get; set; }
        public string dpi { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string estado_credito { get; set; }
        public int deuda { get; set; }
        public string entidad { get; set; }
        public DateTime? fecha_vencimiento { get; set; }
    }
}
