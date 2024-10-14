using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace memoryAfteken.Controls
{
    public class FlipCardView : Grid
    {
        private readonly Image _frontImage;
        private readonly Image _backImage;

        public int CardIndex { get; set; }

        public FlipCardView(string frontImageSource, string backImageSource = "back_of_card.png")
        {
            AnchorX = 0.5;
            AnchorY = 0.5;

            // Initialize rotation
            RotationY = 0;

            // Set the back image (e.g., the back of the card)
            _backImage = new Image
            {
                Source = backImageSource,
                Aspect = Aspect.AspectFill,
                IsVisible = true
            };

            // Set the front image (the face of the card)
            _frontImage = new Image
            {
                Source = frontImageSource,
                Aspect = Aspect.AspectFill,
                IsVisible = false
            };
            Children.Add(_backImage);
            Children.Add(_frontImage);
        }

        public async Task FlipToFront()
        {
            // Ensure rotation starts at 0
            this.RotationY = 0;

            await this.RotateYTo(90, 150, Easing.Linear);
            _backImage.IsVisible = false;
            _frontImage.IsVisible = true;
            this.RotationY = -90;
            await this.RotateYTo(0, 150, Easing.Linear);
        }

        public async Task FlipToBack()
        {
            // Ensure rotation starts at 0
            this.RotationY = 0;

            await this.RotateYTo(90, 150, Easing.Linear);
            _frontImage.IsVisible = false;
            _backImage.IsVisible = true;
            this.RotationY = -90;
            await this.RotateYTo(0, 150, Easing.Linear);
        }
    }
}