namespace DataOperation.Models
{
    public class Report
    {
        public int ParsedFiles { get; set; }
        public int ParsedLines { get; set; }
        public int FoundErrors { get; set; }
        public string InvalidFiles { get; set; }
    }
}
