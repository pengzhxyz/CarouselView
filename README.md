# CarouselView
An UWP carousel control supporting looping and based on composition api
- It supports infinite looping scrolling
- AutoSwitch supporting
- It could show 3 pictures at the same time, rather than filpview which only shows one picture
- Responsable to the container size
- Touch or mouse wheel
- Indicator 
- Thanks to the Composition API!

You can follow me at:

Twitter:[@PengZh](https://twitter.com/vergil_1988)

Weibo:[@羡世_摔倒的小鱼](http://weibo.com/2360273757)
## Some gif inlustrates
![CarouselView demo](https://img3.doubanio.com/view/status/raw/public/7f8372f71c091e6.jpg)

## Usage

### Qiuck integration
It's easy to use just like the below

	<carousel:CarouselView x:Name="carousel" ItemWidth="500"  xmlns:carousel="using:CarouselView.Controls">
 	</carousel:CarouselView>`
and than set the `ItemImageSource`, which is a `List<ICarouselViewItemSource>` property, in code-behind or **using binding**.
```
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
```

**Note**

You need implement the ICarouselViewItemSource as the source of CarouselViewItem

### Advanced 

#### `ItemWidth` property
ItemWidth defines the width of each item in the CarouselView.

When the width of CarouselView is larger than `ItemWidth` the items' width is equal to `ItemWidth`, while items' width is equal to the width of CarouselView.

**I recommend to set the ItemWidth to 500**, although the dafault value is 300.

#### `ItemImageSource` property
it's a `List<ICarouselViewItemSource>` property providing the image url source.

The count of `ItemImageSource` should be 3 at least, as you know.

#### IsAutoSwitchEnabled property
`bool` property, indicates if enable AutoSwitch, default is true

#### AutoSwitchInterval property
`TimeSpan` property, indicates how long to switch automaticlly, default is 7 seconds

## Further works
- [x] Apparently, an ItemClickedEvent or something like that are needed.
- [x] An indicator, which shows which one is selected