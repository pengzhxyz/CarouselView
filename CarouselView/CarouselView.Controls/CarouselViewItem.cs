using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CarouselView.Controls
{
    public sealed class CarouselViewItem : Control
    {

        #region ImageSource

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(CarouselViewItem), new PropertyMetadata(null));


        #endregion

        #region Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CarouselViewItem), new PropertyMetadata(""));

        #endregion

        #region BlackMaskOpacity



        public double BlackMaskOpacity
        {
            get { return (double)GetValue(BlackMaskOpacityProperty); }
            set { SetValue(BlackMaskOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlackMaskOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlackMaskOpacityProperty =
            DependencyProperty.Register("BlackMaskOpacity", typeof(double), typeof(CarouselViewItem), new PropertyMetadata(0.00d));



        #endregion



        #region ItemSource


        public ICarouselViewItemSource ItemSource
        {
            get { return (ICarouselViewItemSource)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(ICarouselViewItemSource), typeof(CarouselViewItem), new PropertyMetadata(null,(s,e)=> 
            {
                var item = (s as CarouselViewItem);
                if (item != null)
                {
                    var source = e.NewValue as ICarouselViewItemSource;
                    if (source != null)
                    {
                        item.Title = source.Title;
                        item.ImageSource = new BitmapImage(new Uri(source.ImageSource));
                    }
                }
            }));


        #endregion
        public CarouselViewItem()
        {
            this.DefaultStyleKey = typeof(CarouselViewItem);
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            VisualStateManager.GoToState(this, "PointerOver", true);
        }
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            VisualStateManager.GoToState(this, "Pressed", true);

        }
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            VisualStateManager.GoToState(this, "Normal", true);

        }
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            VisualStateManager.GoToState(this, "PointerOver", true);
        }


    }
}
