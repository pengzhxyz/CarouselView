using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarouselView.Controls
{
    public interface ICarouselViewItemSource
    {
        string ImageSource { get; set; }
        string Title { get; set; }
    }
}
