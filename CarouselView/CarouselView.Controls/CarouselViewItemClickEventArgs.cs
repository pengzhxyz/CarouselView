using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarouselView.Controls
{
    public class CarouselViewItemClickEventArgs:EventArgs
    {
        public object ClickItem { get; set; }
        public CarouselViewItemClickEventArgs(object clickItem)
        {
            this.ClickItem = clickItem;
        }
    }
}
