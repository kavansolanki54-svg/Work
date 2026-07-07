using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Callyzer.App.Controls
{
    public partial class ContactAvatar : ContentView
    {
        public static readonly BindableProperty ContactNameProperty =
            BindableProperty.Create(nameof(ContactName), typeof(string), typeof(ContactAvatar), string.Empty, propertyChanged: OnContactNameChanged);

        public static readonly BindableProperty SizeProperty =
            BindableProperty.Create(nameof(Size), typeof(double), typeof(ContactAvatar), 48.0, propertyChanged: OnSizeChanged);

        // Read-only bindables for internal calculation
        public string Initials { get; private set; } = string.Empty;
        public double CornerRadius { get; private set; } = 24.0;
        public double FontSize { get; private set; } = 18.0;
        public Color AvatarColor { get; private set; } = Colors.Gray;

        public string ContactName
        {
            get => (string)GetValue(ContactNameProperty);
            set => SetValue(ContactNameProperty, value);
        }

        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public ContactAvatar()
        {
            InitializeComponent();
        }

        private static void OnSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ContactAvatar control && newValue is double size)
            {
                control.CornerRadius = size / 2.0;
                control.FontSize = size * 0.4; // 40% of size
                
                control.OnPropertyChanged(nameof(CornerRadius));
                control.OnPropertyChanged(nameof(FontSize));
            }
        }

        private static void OnContactNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ContactAvatar control && newValue is string name)
            {
                control.Initials = GenerateInitials(name);
                control.AvatarColor = GenerateColor(name);
                
                control.OnPropertyChanged(nameof(Initials));
                control.OnPropertyChanged(nameof(AvatarColor));
            }
        }

        private static string GenerateInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            
            // If it's a phone number (starts with + or a digit), just return '#'
            if (name.StartsWith("+") || char.IsDigit(name.FirstOrDefault(c => c != ' '))) return "#";

            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
            
            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpper();
        }

        private static Color GenerateColor(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Colors.Gray;
            
            var colors = new[]
            {
                Color.FromArgb("#F44336"), // Red
                Color.FromArgb("#E91E63"), // Pink
                Color.FromArgb("#9C27B0"), // Purple
                Color.FromArgb("#673AB7"), // Deep Purple
                Color.FromArgb("#3F51B5"), // Indigo
                Color.FromArgb("#2196F3"), // Blue
                Color.FromArgb("#03A9F4"), // Light Blue
                Color.FromArgb("#00BCD4"), // Cyan
                Color.FromArgb("#009688"), // Teal
                Color.FromArgb("#4CAF50"), // Green
                Color.FromArgb("#8BC34A"), // Light Green
                Color.FromArgb("#FF9800"), // Orange
                Color.FromArgb("#FF5722")  // Deep Orange
            };

            var hash = 0;
            foreach (var c in name) hash = c + ((hash << 5) - hash);
            var index = Math.Abs(hash) % colors.Length;
            
            return colors[index];
        }
    }
}
