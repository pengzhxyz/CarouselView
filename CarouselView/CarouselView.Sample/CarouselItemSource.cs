using CarouselView.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarouselView.Sample
{
    public class CarouselItemSource : ICarouselViewItemSource
    {
        public string ImageSource { get; set; }
        public string Title { get; set; }
    }
}
