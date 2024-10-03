namespace memoryAfteken.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public string ImageName { get; set; } // Store image name
    }
}