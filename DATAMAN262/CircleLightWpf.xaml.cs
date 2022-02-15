using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DATAMAN262
{
    /// <summary>
    /// Interaction logic for CircleLightWpf.xaml
    /// </summary>
    public partial class CircleLightWpf : UserControl
    {
        public enum ColorOption
        {
            Red,
            Gray,
            Green
        }
        public CircleLightWpf(double w, double h)
        {
            this.SizeChanged += CircleLightWpf_SizeChanged;
            InitializeComponent();
            this.Width = w;
            this.Height = h;
        }

        private void CircleLightWpf_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(this.Width != this.Height)
            {
                this.Height = this.Width;
            }
            this.Circle.Width = this.Width-5;
            this.Circle.Height = this.Height-5;
        }

        public void ChangeColor(ColorOption color)
        {
            SolidColorBrush fillcolor = new SolidColorBrush();
            switch (color)
            {
                case ColorOption.Gray:
                    fillcolor.Color = Color.FromRgb(230, 230, 230);
                    break;
                case ColorOption.Red:
                    fillcolor.Color = Color.FromRgb(255, 64, 0);
                    break;
                case ColorOption.Green:
                    fillcolor.Color = Color.FromRgb(0, 153, 51);
                    break;
            }
            this.Circle.Fill = fillcolor;
        }
    }
}
