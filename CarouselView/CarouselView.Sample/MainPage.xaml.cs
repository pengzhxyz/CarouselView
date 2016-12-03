using CarouselView.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
            carousel.ItemClick += Carousel_ItemClick;
        }

        private async void Carousel_ItemClick(object arg1, CarouselViewItemClickEventArgs arg2)
        {
            MessageDialog md = new MessageDialog($"You have clicked {(arg2.ClickItem as ICarouselViewItemSource).Title} ;-)","Wow");
            await md.ShowAsync();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            carousel.ItemImageSource = new List<ICarouselViewItemSource>()
            {
                 new CarouselItemSource() {
                     ImageSource ="https://img1.doubanio.com/view/photo/photo/public/p1547743259.jpg",
                     Title ="北方有佳人" },
                 new CarouselItemSource() {
                     ImageSource ="https://img1.doubanio.com/view/photo/photo/public/p2183422782.jpg",
                     Title ="绝世而独立" },
                 new CarouselItemSource() {
                     ImageSource ="https://img1.doubanio.com/view/photo/photo/public/p832662844.jpg",
                     Title ="一顾倾人城" },
                 new CarouselItemSource() {
                     ImageSource ="https://img1.doubanio.com/view/photo/photo/public/p752907403.jpg",
                     Title ="再顾倾人国" },
            };
        }
    }
}
