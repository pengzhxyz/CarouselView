using CarouselView.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace CarouselView.Sample
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
             carousel.ItemImageSource = new List<ICarouselViewItemSource>()
            {
                 new CarouselItemSource() {
                     ImageSource ="https://img1.doubanio.com/view/photo/photo/public/p1204310498.jpg",
                      },
                 new CarouselItemSource() {
                     ImageSource ="https://img1.doubanio.com/view/photo/photo/public/p1547743259.jpg",
                     Title ="北燕南非，西风萧萧向何处" },
                 new CarouselItemSource() {
                     ImageSource ="https://img1.doubanio.com/view/photo/photo/public/p2183422782.jpg",
                     Title ="北燕南非，西风萧萧向何处" },
                 new CarouselItemSource() {
                     ImageSource ="https://img1.doubanio.com/view/photo/photo/public/p832662844.jpg",
                     Title ="北燕南非，西风萧萧向何处" },
                 new CarouselItemSource() {
                     ImageSource ="https://img1.doubanio.com/view/photo/photo/public/p752907403.jpg",
                     Title ="北燕南非，西风萧萧向何处" },
            };
        }
    }
}
