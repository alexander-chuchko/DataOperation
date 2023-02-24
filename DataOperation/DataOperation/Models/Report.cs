using System.Collections.Generic;

namespace DataOperation.Models
{
    public class Report
    {
        public int ParsedFiles { get; set; }
        public int ParsedLines { get; set; }
        public int FoundErrors { get; set; }
        public List<string> InvalidFiles { get; set; } = new List<string>();
    }
}
