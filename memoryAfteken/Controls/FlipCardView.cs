using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace memoryAfteken.Controls
{
    public class FlipCardView : Grid
    {
        private readonly BoxView _frontView;
        private readonly BoxView _backView;

        public int CardIndex { get; set; }

        public FlipCardView(Color frontColor, Color backColor)
        {
            AnchorX = 0.5;
            AnchorY = 0.5;

            // Initialize rotation
            RotationY = 0;

            // Set the back view (e.g., the back of the card)
            _backView = new BoxView
            {
                Color = backColor,
                IsVisible = true
            };

            // Set the front view (the face of the card)
            _frontView = new BoxView
            {
                Color = frontColor,
                IsVisible = false
            };

            // Add views to the grid
            // Ensure _backView is added before _frontView
            Children.Add(_backView);
            Children.Add(_frontView);
        }

        public async Task FlipToFront()
        {
            // Ensure rotation starts at 0
            this.RotationY = 0;

            await this.RotateYTo(90, 150, Easing.Linear);
            _backView.IsVisible = false;
            _frontView.IsVisible = true;
            this.RotationY = -90;
            await this.RotateYTo(0, 150, Easing.Linear);
        }

        public async Task FlipToBack()
        {
            // Ensure rotation starts at 0
            this.RotationY = 0;

            await this.RotateYTo(90, 150, Easing.Linear);
            _frontView.IsVisible = false;
            _backView.IsVisible = true;
            this.RotationY = -90;
            await this.RotateYTo(0, 150, Easing.Linear);
        }
    }
}