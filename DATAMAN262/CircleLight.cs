using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATAMAN262
{
    public partial class CircleLight : UserControl
    {
        public CircleLightWpf CircleSubject;
        public CircleLight()
        {
            InitializeComponent();
            CircleSubject = new CircleLightWpf(this.Width, this.Height);
            this.Host.Child = CircleSubject;
            this.SizeChanged += CircleLight_SizeChanged;
        }

        private void CircleLight_SizeChanged(object sender, EventArgs e)
        {
            if(this.Width != this.Height)
            {
                this.Height = this.Width;
            }
            this.CircleSubject.Width = this.Width;
            this.CircleSubject.Height = this.Height;
        }
    }
}
