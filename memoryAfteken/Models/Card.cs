using Microsoft.Maui.Graphics;

namespace memoryAfteken.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public Color Color { get; set; } // Add this property
    }
}