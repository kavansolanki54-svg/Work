using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Callyzer.App.Controls
{
    public partial class StatCard : ContentView
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(StatCard), string.Empty);

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(string), typeof(StatCard), string.Empty);

        public static readonly BindableProperty TargetNumericValueProperty =
            BindableProperty.Create(nameof(TargetNumericValue), typeof(double?), typeof(StatCard), null, propertyChanged: OnTargetNumericValueChanged);

        public static readonly BindableProperty ValueColorProperty =
            BindableProperty.Create(nameof(ValueColor), typeof(Color), typeof(StatCard), Application.Current?.Resources["PrimaryColor"] as Color ?? Colors.Blue);

        public static readonly BindableProperty StringFormatProperty =
            BindableProperty.Create(nameof(StringFormat), typeof(string), typeof(StatCard), "{0:F0}");

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double? TargetNumericValue
        {
            get => (double?)GetValue(TargetNumericValueProperty);
            set => SetValue(TargetNumericValueProperty, value);
        }

        public Color ValueColor
        {
            get => (Color)GetValue(ValueColorProperty);
            set => SetValue(ValueColorProperty, value);
        }

        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        public StatCard()
        {
            InitializeComponent();
        }

        private static void OnTargetNumericValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StatCard)bindable;
            if (newValue is double target)
            {
                control.AnimateValue(target);
            }
        }

        private void AnimateValue(double target)
        {
            // Abort any running animation
            this.AbortAnimation("CountUp");

            var animation = new Animation(v =>
            {
                // Note: The StringFormat property is used for simple numeric values.
                // If it requires custom formatting like the DurationConverter, the calling page should just bind to Value directly,
                // but for animations to work, they can use TargetNumericValue.
                if (StringFormat.Contains("s")) // Hacky check for duration, better handled via Value directly if complex
                {
                     Value = string.Format(StringFormat, v);
                }
                else
                {
                     Value = string.Format(StringFormat, v);
                }
                
            }, 0, target, Easing.CubicOut);

            // Animate over 800ms
            animation.Commit(this, "CountUp", length: 800);
        }
    }
}
