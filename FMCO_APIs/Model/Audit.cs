namespace ProductManagment_APIs.Model
{
    public class Audit
    {
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAT { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
