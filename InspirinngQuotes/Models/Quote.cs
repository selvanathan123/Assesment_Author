using System.Collections.Generic;

namespace InspirinngQuotes.Models
{
    public class Quote
    {
        public int Id { get; set; } 
        public string ?Author { get; set; }
        public List<string> ?Tags { get; set; }
        public List<string>  ?QuoteText { get; set; }
    }
}
