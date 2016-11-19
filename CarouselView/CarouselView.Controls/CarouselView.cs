using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CarouselView.Controls
{
    public sealed class CarouselView : Control
    {

        #region Properties

        #region SelectedIndex


        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(CarouselView), new PropertyMetadata(-1,(s,e)=> 
            {
                //var carousel = (s as CarouselView);
                //if (carousel!=null)
                //{
                //    if (carousel._listbox!=null)
                //    {
                //        if (carousel._listbox.Items.Count>0)
                //        {
                //            carousel._listbox.SelectedIndex = (int)e.NewValue;
                //        }
                //    }
                //}
            }));


        #endregion

        #region ItemWidth

        /// <summary>
        /// Get or Set the ItemWidth in CarouselView and it's suggested to to larger than 300
        /// </summary>
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(CarouselView), new PropertyMetadata(300.00d));


        #endregion

        #region ItemImageSource


        public List<ICarouselViewItemSource> ItemImageSource
        {
            get { return (List<ICarouselViewItemSource>)GetValue(ItemImageSourceProperty); }
            set
            {
                SetValue(ItemImageSourceProperty, value);
                this.SetItemsImageSource();
            }
        }

        // Using a DependencyProperty as the backing store for ItemImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemImageSourceProperty =
            DependencyProperty.Register("ItemImageSource", typeof(List<ICarouselViewItemSource>), typeof(CarouselView), new PropertyMetadata(null,(s,e)=> 
            {
                //if (e.NewValue!=e.OldValue)
                //{
                //    (s as CarouselView).SetItemsImageSource(true);
                //}
            }));


        #endregion

        #region IsAutoSwitchEnabled
        public bool IsAutoSwitchEnabled
        {
            get { return (bool)GetValue(IsAutoSwitchEnableProperty); }
            set { SetValue(IsAutoSwitchEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAutoSwitchEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoSwitchEnableProperty =
            DependencyProperty.Register("IsAutoSwitchEnable", typeof(bool), typeof(CarouselView), new PropertyMetadata(true,(s,e)=> 
            {
                if (e.NewValue!=e.OldValue)
                {
                    if ((bool)e.NewValue)
                    {
                        (s as CarouselView)._dispatcherTimer.Start();
                    }
                    else
                    {
                        (s as CarouselView)._dispatcherTimer.Stop();
                    }
                }
            }));

        #endregion

        #region AutoSwitchInterval

        /// <summary>
        /// default is 7 seconds
        /// </summary>
        public TimeSpan AutoSwitchInterval
        {
            get { return (TimeSpan)GetValue(AutoSwitchIntervalProperty); }
            set { SetValue(AutoSwitchIntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoSwitchInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoSwitchIntervalProperty =
            DependencyProperty.Register("AutoSwitchInterval", typeof(TimeSpan), typeof(CarouselView), new PropertyMetadata(new TimeSpan(0,0,7),(s,e)=> 
            {
                if (e.NewValue!=e.OldValue)
                {
                    (s as CarouselView)._dispatcherTimer.Interval = (TimeSpan)e.NewValue;
                    (s as CarouselView)._dispatcherTimer.Start();
                }
            }));


        #endregion
        #endregion

        #region Event
        public delegate void CarouselViewItemClickEventHandler(object sender, CarouselViewItemClickEventArgs e);
        public event CarouselViewItemClickEventHandler ItemClick;
        private void OnItemClick(ICarouselViewItemSource e)
        {
            if (e == null) return;
            ItemClick?.Invoke(this, new CarouselViewItemClickEventArgs(e));
        }
        #endregion

        Compositor _compositor;
        //ContainerVisual _touchAreaVisual;
        Visual _touchAreaVisual, _indicatorVisual;
        List<Visual> _itemVisualList;
        List<CarouselViewItem> _itemUIElementList;
        ExpressionAnimation _animation, _animation_0, _animation_1, _animation_2, _animation_3, _animation_4;
        ScalarKeyFrameAnimation _indicatorAnimation;
        float _x;
        int _selectedIndex;
        Panel _canvas, _rootGrid;
        ListBox _listbox;
        DispatcherTimer _dispatcherTimer; // for auto switch
        bool _isAnimationRunning = false; // flag of animation running, for precessing mouse WheelChanged Event

        public CarouselView()
        {
            this.DefaultStyleKey = typeof(CarouselView);
            _selectedIndex = 2;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the original elements from the template
            _rootGrid = this.GetTemplateChild("RootGrid") as Panel;
            _canvas = this.GetTemplateChild("canvas") as Panel;
            var indiRect = this.GetTemplateChild("indicatorRect") as Rectangle;
            _indicatorVisual = ElementCompositionPreview.GetElementVisual(indiRect);
            _touchAreaVisual = ElementCompositionPreview.GetElementVisual(_canvas);
            _compositor = _touchAreaVisual.Compositor;
            _itemVisualList = new List<Visual>();
            _itemUIElementList = _canvas.GetDescendantsOfType<CarouselViewItem>().ToList();
            foreach (var item in _itemUIElementList)
            {
                _itemVisualList.Add(ElementCompositionPreview.GetElementVisual(item));
                // For ItemClick Event
                item.Tapped += (s,e) => { OnItemClick((s as CarouselViewItem).ItemSource); };
            }
            _listbox = this.GetTemplateChild("indexlistbox") as ListBox;
            
            // Event handlers
            this._canvas.ManipulationMode = ManipulationModes.TranslateX;

            _canvas.ManipulationStarted += Canvas_ManipulationStarted;
            _canvas.ManipulationDelta += Canvas_ManipulationDelta;
            _canvas.ManipulationCompleted += Canvas_ManipulationCompleted;
            //_canvas.ManipulationInertiaStarting += Canvas_ManipulationInertiaStarting;

            _canvas.PointerWheelChanged += Canvas_PointerWheelChanged;
            // Response to the SizeChanged
            _rootGrid.SizeChanged += (ss, ee) => 
            {
                MeasureItemsPosition(_selectedIndex);
                // Change the Clip to fit new size
                _canvas.Clip = new RectangleGeometry() { Rect = RectHelper.FromCoordinatesAndDimensions(0, 0, (float)ee.NewSize.Width, (float)ee.NewSize.Height) };
            };
            //_canvas.SizeChanged += (ss, ee) => { MeasureItemsPosition(_selectedIndex, _selectedIndex); };
            this.Loaded += (ss,ee) => 
            {
                // Initial items' image data
                SetItemsImageSource(true);
                SetSelectedAppearance();
                //MeasureItemsPosition(_selectedIndex);
            };

            // setup the timer in order to autoswitch
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Interval = this.AutoSwitchInterval;
            if (this.IsAutoSwitchEnabled)
            {
                _dispatcherTimer.Start();
            }

        }

        private void PrepareAnimations()
        {
            _animation = _compositor.CreateExpressionAnimation("touch.Offset.X");
            _animation.SetReferenceParameter("touch", _indicatorVisual);

            _animation_0 = _compositor.CreateExpressionAnimation("touch.Offset.X+self");
            float offsestX_0 = _itemVisualList[0].Offset.X;
            _animation_0.SetScalarParameter("self", offsestX_0);
            _animation_0.SetReferenceParameter("touch", _indicatorVisual);

            _animation_1 = _compositor.CreateExpressionAnimation("touch.Offset.X+self");
            float offsestX_1 = _itemVisualList[1].Offset.X;
            _animation_1.SetScalarParameter("self", offsestX_1);
            _animation_1.SetReferenceParameter("touch", _indicatorVisual);

            _animation_2 = _compositor.CreateExpressionAnimation("touch.Offset.X+self");
            float offsestX_2 = _itemVisualList[2].Offset.X;
            _animation_2.SetScalarParameter("self", offsestX_2);
            _animation_2.SetReferenceParameter("touch", _indicatorVisual);

            _animation_3 = _compositor.CreateExpressionAnimation("touch.Offset.X+self");
            float offsestX_3 = _itemVisualList[3].Offset.X;
            _animation_3.SetScalarParameter("self", offsestX_3);
            _animation_3.SetReferenceParameter("touch", _indicatorVisual);

            _animation_4 = _compositor.CreateExpressionAnimation("touch.Offset.X+self");
            float offsestX_4 = _itemVisualList[4].Offset.X;
            _animation_4.SetScalarParameter("self", offsestX_4);
            _animation_4.SetReferenceParameter("touch", _indicatorVisual);
        }

        private void SetItemsImageSource(bool isinitial=false)
        {
            System.Diagnostics.Debug.WriteLine("SetItemsImageSource");
            // Set the imagesource of each item. 
            if (ItemImageSource==null)
            {
                return;
            }
            if (ItemImageSource.Count == 0) return;

            // get the source indexes
            int count = ItemImageSource.Count;
            if (SelectedIndex<0)
            {
                SelectedIndex = 0;
            }
            int sindex = SelectedIndex;
            int sindex_0, sindex_1, sindex_2, sindex_3, sindex_4;
            sindex_0 = sindex - 2 < 0 ? (sindex - 2) + count : sindex - 2;
            sindex_1 = (sindex - 1) < 0 ? (sindex - 1) + count : sindex - 1;
            sindex_2 = sindex;
            sindex_3 = (sindex + 1) > count - 1 ? (sindex + 1) - count : sindex + 1;
            sindex_4 = (sindex + 2) > count - 1 ? (sindex + 2) - count : sindex + 2;

            // if count=1 all the sindexes = 0
            if (count==1)
            {
                sindex_0 = sindex_1 = sindex_2 = sindex_3 = sindex_4 = 0;
            }
            // get the UIelement indexes
            int index_0, index_1, index_2, index_3, index_4;
            index_0 = _selectedIndex - 2;
            if (index_0 < 0) index_0 = index_0 + 5;
            index_1 = _selectedIndex - 1;
            if (index_1 < 0) index_1 = index_1 + 5;
            index_2 = _selectedIndex;
            index_3 = _selectedIndex + 1;
            if (index_3 > 4) index_3 = index_3 - 5;
            index_4 = _selectedIndex + 2;
            if (index_4 > 4) index_4 = index_4 - 5;

            //_itemUIElementList[index_0].ImageSource = new BitmapImage(new Uri(ItemImageSource[sindex_0].ImageSource));
            //_itemUIElementList[index_0].Title = ItemImageSource[sindex_0].Title;
            //// To avoid to flash (reason is unclear).
            //if (isinitial)
            //{
            //    _itemUIElementList[index_1].ImageSource = new BitmapImage(new Uri(ItemImageSource[sindex_1].ImageSource));
            //    _itemUIElementList[index_1].Title = ItemImageSource[sindex_1].Title;
            //    _itemUIElementList[index_2].ImageSource = new BitmapImage(new Uri(ItemImageSource[sindex_2].ImageSource));
            //    _itemUIElementList[index_2].Title = ItemImageSource[sindex_2].Title;
            //    _itemUIElementList[index_3].ImageSource = new BitmapImage(new Uri(ItemImageSource[sindex_3].ImageSource));
            //    _itemUIElementList[index_3].Title = ItemImageSource[sindex_3].Title;
            //}
            //_itemUIElementList[index_4].ImageSource = new BitmapImage(new Uri(ItemImageSource[sindex_4].ImageSource));
            //_itemUIElementList[index_4].Title = ItemImageSource[sindex_4].Title;
            _itemUIElementList[index_0].ItemSource = ItemImageSource[sindex_0];
            _itemUIElementList[index_1].ItemSource = ItemImageSource[sindex_1];
            _itemUIElementList[index_2].ItemSource = ItemImageSource[sindex_2];
            _itemUIElementList[index_3].ItemSource = ItemImageSource[sindex_3];
            _itemUIElementList[index_4].ItemSource = ItemImageSource[sindex_4];
        }

        private void SetSelectedAppearance()
        {
            // Set the Selected or UnSelected Items' stype
            // Like BlackMaskOpacity of CarouselViewItem
            for (int i = 0; i < 5; i++)
            {
                if (i==_selectedIndex)
                {
                    _itemUIElementList[i].BlackMaskOpacity = 0.0;
                }
                else
                {
                    _itemUIElementList[i].BlackMaskOpacity = 0.3;
                }
            }
        }

        private void Canvas_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Canvas_ManipulationInertiaStarted");
            // Stop the Animation and reset the postion of indicator
            _itemVisualList[0].StopAnimation("Offset.X");
            _itemVisualList[1].StopAnimation("Offset.X");
            _itemVisualList[2].StopAnimation("Offset.X");
            _itemVisualList[3].StopAnimation("Offset.X");
            _itemVisualList[4].StopAnimation("Offset.X");
            _x = 0.0f;
            _indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

            // Prepare animiations
            PrepareAnimations();

            // Strat the Animations based on the indicator
            _itemVisualList[0].StartAnimation("Offset.X", _animation_0);
            _itemVisualList[1].StartAnimation("Offset.X", _animation_1);
            _itemVisualList[2].StartAnimation("Offset.X", _animation_2);
            _itemVisualList[3].StartAnimation("Offset.X", _animation_3);
            _itemVisualList[4].StartAnimation("Offset.X", _animation_4);
        }
        private void Canvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _x += (float)e.Delta.Translation.X;

            // set the pan rectangle's visual's offset
            _indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

        }

        //private void Canvas_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("Canvas_ManipulationInertiaStarting");
        //}
        private void Canvas_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            double containerWidth = _canvas.ActualWidth;
            double itemWidth = _itemUIElementList[0].ActualWidth;
            double cleft = (containerWidth - itemWidth) / 2;
            // evaluate the new selectedIndex
            int oldSelectedIndex = _selectedIndex;
            var cha = _indicatorVisual.Offset.X - cleft;
            if (cha <= -cleft)
            {
                // new selcted item is the right item of current item
                _selectedIndex = _selectedIndex + 1;
                if (_selectedIndex > 4)
                {
                    _selectedIndex = _selectedIndex - 5;
                }
                // Change the SelectedIndex
                var k = SelectedIndex + 1;
                SelectedIndex = k > ItemImageSource.Count-1 ? k - ItemImageSource.Count : k;
            }
            if (cha >= cleft)
            {
                // new selcted item is the left item of current item
                _selectedIndex = _selectedIndex - 1;
                if (_selectedIndex < 0) _selectedIndex = _selectedIndex + 5;

                // Change the SelectedIndex
                var k = SelectedIndex - 1;
                SelectedIndex = k < 0 ? k + ItemImageSource.Count : k;
            }
            else
            {
                //_animation.InsertKeyFrame(1.0f, 0.0f);
            }

            MeasureItemsPosition(_selectedIndex, oldSelectedIndex);
            //foreach (var item in _itemVisualList)
            //{
            //    item.StartAnimation("Offset.X", _animation);
            //}
            //var backScopedBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            //_itemVisualList[4].StartAnimation("Offset.X", _animation);
            //backScopedBatch.End();
            //backScopedBatch.Completed += BackScopedBatch_Completed;
            //_visual.StartAnimation("Offset.X", _animation);
            //_touchAreaVisual.StartAnimation("Offset.X", _animation);
        }



        static ContainerVisual GetVisual(UIElement element)
        {
            var hostVisual = ElementCompositionPreview.GetElementVisual(element);
            ContainerVisual root = hostVisual.Compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(element, root);
            return root;
        }

        /// <summary>
        /// key function of the carousel logic, before calling this, should comfirm:
        /// 1. indicator's Offset.X is correct
        /// 2. set the _selectedIndex and SelectedIndex to new index
        /// note: _selectedIndex and SelectedIndex have diffrent meaning
        /// </summary>
        /// <param name="index"></param>
        /// <param name="oldindex"></param>
        private void MeasureItemsPosition(int index, int oldindex = -1)
        {
            System.Diagnostics.Debug.WriteLine($"MeasureItemsPosition _selectedIndex is {index} _oldselected is {oldindex}");

            // Set the itemwidth
            // if the container width is larger than ItemWidtn, itemwidth = ItemWidth
            // else itemwidth = container
            double containerWidth = _canvas.ActualWidth;
            double itemWidth = _itemUIElementList[0].ActualWidth;
            if (containerWidth < this.ItemWidth)
            {
                foreach (var item in _itemUIElementList)
                {
                    item.Width = containerWidth;
                }
            }
            else if (itemWidth < ItemWidth)
            {
                foreach (var item in _itemUIElementList)
                {
                    item.Width = ItemWidth;
                }
            }
            itemWidth = _itemUIElementList[0].Width;
            double LLeft, Cleft, Rleft;
            Cleft = (containerWidth - itemWidth) / 2;
            LLeft = -(itemWidth - Cleft);
            Rleft = containerWidth - Cleft;

            int index_0, index_1, index_2, index_3, index_4;
            index_0 = index - 2;
            if (index_0 < 0) index_0 = index_0 + 5;
            index_1 = index - 1;
            if (index_1 < 0) index_1 = index_1 + 5;
            index_2 = index;
            index_3 = index + 1;
            if (index_3 > 4) index_3 = index_3 - 5;
            index_4 = index + 2;
            if (index_4 > 4) index_4 = index_4 - 5;

            // for initialing only, or no need to enable animiations
            if (oldindex == -1)
            {
                _itemVisualList[index_0].Offset = new Vector3((float)(LLeft - itemWidth), 0, 0);
                _itemVisualList[index_1].Offset = new Vector3((float)LLeft, 0, 0);
                _itemVisualList[index_2].Offset = new Vector3((float)Cleft, 0, 0);
                _itemVisualList[index_3].Offset = new Vector3((float)(Rleft), 0, 0);
                _itemVisualList[index_4].Offset = new Vector3((float)(Rleft + itemWidth), 0, 0);
                return;
            }

            int diff = index - oldindex;
            // new selected item equals to current item
            if (diff == 0)
            {
                _indicatorAnimation = _compositor.CreateScalarKeyFrameAnimation();
                _indicatorAnimation.InsertKeyFrame(1.0f, 0.0f);
                _indicatorAnimation.Duration = TimeSpan.FromMilliseconds(300);
            }
            // new selected item is the right item of current item
            if (diff == 1 || diff < -1)
            {
                _indicatorAnimation = _compositor.CreateScalarKeyFrameAnimation();
                _indicatorAnimation.InsertKeyFrame(1.0f, (float)-itemWidth);
                _indicatorAnimation.Duration = TimeSpan.FromMilliseconds(300);
            }
            // new selected item is the left one of current item
            if (diff == -1 || diff > 1)
            {
                _indicatorAnimation = _compositor.CreateScalarKeyFrameAnimation();
                _indicatorAnimation.InsertKeyFrame(1.0f, (float)itemWidth);
                _indicatorAnimation.Duration = TimeSpan.FromMilliseconds(300);
            }

            _isAnimationRunning = true;

            // Start the indicator animiation
            var backScopedBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _indicatorVisual.StartAnimation("Offset.X", _indicatorAnimation);
            backScopedBatch.End();
            backScopedBatch.Completed += (ss, ee) =>
            {
                // reset the firt and last item's postion
                _itemVisualList[index_0].Offset = new Vector3((float)(LLeft - itemWidth), 0, 0);
                _itemVisualList[index_1].Offset = new Vector3((float)LLeft, 0, 0);
                _itemVisualList[index_2].Offset = new Vector3((float)Cleft, 0, 0);
                _itemVisualList[index_3].Offset = new Vector3((float)(Rleft), 0, 0);
                _itemVisualList[index_4].Offset = new Vector3((float)(Rleft + itemWidth), 0, 0);

                // Change item's imagesources
                SetItemsImageSource();
                // Set Selected Item's appearance
                SetSelectedAppearance();
                // reset animation running flas
                _isAnimationRunning = false;
            };

            // Set the ZIndex of the items
            //Canvas.SetZIndex(_itemUIElementList[index_0], 0);
            //Canvas.SetZIndex(_itemUIElementList[index_1], 1);
            //Canvas.SetZIndex(_itemUIElementList[index_2], 2);
            //Canvas.SetZIndex(_itemUIElementList[index_3], 1);
            //Canvas.SetZIndex(_itemUIElementList[index_4], 0);

            // set the dispatcherTimer
            if (IsAutoSwitchEnabled)
            {
                _dispatcherTimer.Start();
            }
            else if (_dispatcherTimer.IsEnabled)
            {
                _dispatcherTimer.Stop();
            }
           


        }

        private void Canvas_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            // Handle the event
            e.Handled = true;

            if (_isAnimationRunning)
            {
                return;
            }

            // Stop the Animation and reset the postion of indicator
            _itemVisualList[0].StopAnimation("Offset.X");
            _itemVisualList[1].StopAnimation("Offset.X");
            _itemVisualList[2].StopAnimation("Offset.X");
            _itemVisualList[3].StopAnimation("Offset.X");
            _itemVisualList[4].StopAnimation("Offset.X");
            _x = 0.0f;
            _indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

            // Processing the PointerWheelChanged event by mouse
            var pointerpoint = e.GetCurrentPoint(_canvas);
            var mousewheeldelta = pointerpoint.Properties.MouseWheelDelta;

            int newindex = _selectedIndex;
            // MouseWheelDelta:
            // A positive value indicates that the wheel was rotated forward (away from the user) or tilted to the right;
            // A negative value indicates that the wheel was rotated backward (toward the user) or tilted to the left.
            if (mousewheeldelta > 0)
            {
                // get the index of last one
                newindex = _selectedIndex - 1;
                if (newindex < 0)
                {
                    newindex = newindex + 5;
                }
                // Change the SelectedIndex
                var k = SelectedIndex - 1;
                SelectedIndex = k <0  ? k + ItemImageSource.Count : k;
            }
            else if (mousewheeldelta < 0)
            {
                // get the index of next one
                newindex = _selectedIndex + 1;
                if (newindex > 4)
                {
                    newindex = newindex - 5;
                }
                // Change the SelectedIndex
                var k = SelectedIndex + 1;
                SelectedIndex = k > ItemImageSource.Count-1 ? k - ItemImageSource.Count : k;
            }
            PrepareAnimations();
            _itemVisualList[0].StartAnimation("Offset.X", _animation_0);
            _itemVisualList[1].StartAnimation("Offset.X", _animation_1);
            _itemVisualList[2].StartAnimation("Offset.X", _animation_2);
            _itemVisualList[3].StartAnimation("Offset.X", _animation_3);
            _itemVisualList[4].StartAnimation("Offset.X", _animation_4);
            // Changed the _selectedIndex
            int oldindex = _selectedIndex;
            _selectedIndex = newindex;
            MeasureItemsPosition(newindex, oldindex);

        }

        private void _dispatcherTimer_Tick(object sender, object e)
        {
            // Implement autoswitch
            if (this.IsAutoSwitchEnabled)
            {
                GotoNext();
            }
            else
            {
                _dispatcherTimer.Stop();
            }
        }

        private void GotoNext()
        {
            // Stop the Animation and reset the postion of indicator
            _itemVisualList[0].StopAnimation("Offset.X");
            _itemVisualList[1].StopAnimation("Offset.X");
            _itemVisualList[2].StopAnimation("Offset.X");
            _itemVisualList[3].StopAnimation("Offset.X");
            _itemVisualList[4].StopAnimation("Offset.X");
            _x = 0.0f;
            _indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

            int newindex = _selectedIndex;
            // get the index of next one
            newindex = _selectedIndex + 1;
            if (newindex > 4)
            {
                newindex = newindex - 5;
            }
            // Change the SelectedIndex
            var k = SelectedIndex + 1;
            SelectedIndex = k > ItemImageSource.Count - 1 ? k - ItemImageSource.Count : k;
            
            PrepareAnimations();
            _itemVisualList[0].StartAnimation("Offset.X", _animation_0);
            _itemVisualList[1].StartAnimation("Offset.X", _animation_1);
            _itemVisualList[2].StartAnimation("Offset.X", _animation_2);
            _itemVisualList[3].StartAnimation("Offset.X", _animation_3);
            _itemVisualList[4].StartAnimation("Offset.X", _animation_4);
            // Changed the _selectedIndex
            int oldindex = _selectedIndex;
            _selectedIndex = newindex;
            MeasureItemsPosition(newindex, oldindex);
        }

        private void GotoPrevious()
        {
            // Stop the Animation and reset the postion of indicator
            _itemVisualList[0].StopAnimation("Offset.X");
            _itemVisualList[1].StopAnimation("Offset.X");
            _itemVisualList[2].StopAnimation("Offset.X");
            _itemVisualList[3].StopAnimation("Offset.X");
            _itemVisualList[4].StopAnimation("Offset.X");
            _x = 0.0f;
            _indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

            int newindex = _selectedIndex;
            // get the index of last one
            newindex = _selectedIndex - 1;
            if (newindex < 0)
            {
                newindex = newindex + 5;
            }
            // Change the SelectedIndex
            var k = SelectedIndex - 1;
            SelectedIndex = k < 0 ? k + ItemImageSource.Count : k;
            
            PrepareAnimations();
            _itemVisualList[0].StartAnimation("Offset.X", _animation_0);
            _itemVisualList[1].StartAnimation("Offset.X", _animation_1);
            _itemVisualList[2].StartAnimation("Offset.X", _animation_2);
            _itemVisualList[3].StartAnimation("Offset.X", _animation_3);
            _itemVisualList[4].StartAnimation("Offset.X", _animation_4);
            // Changed the _selectedIndex
            int oldindex = _selectedIndex;
            _selectedIndex = newindex;
            MeasureItemsPosition(newindex, oldindex);
        }
    }
}
