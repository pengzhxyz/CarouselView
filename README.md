# CarouselView
An UWP carousel control supporting looping and based on composition api
- It supports infinite looping scrolling
- It could show 3 pictures at the same time, rather than filpview which only shows one picture
- Responsable to the container size
- Touch or mouse wheel
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
and than set the `ItemImageSource`, which is a `List<string>` property, in code-behind or **using binding**.

	carousel.ItemImageSource = new List<string>()
	{
		"https://img1.doubanio.com/view/photo/photo/public/p1204310498.jpg",
		"https://img1.doubanio.com/view/photo/photo/public/p1547743259.jpg",
		"https://img1.doubanio.com/view/photo/photo/public/p2183422782.jpg",
		"https://img1.doubanio.com/view/photo/photo/public/p832662844.jpg",
		"https://img1.doubanio.com/view/photo/photo/public/p752907403.jpg"
	};

### About `ItemWidth` property
ItemWidth defines the width of each item in the CarouselView.

When the width of CarouselView is larger than `ItemWidth` the items' width is equal to `ItemWidth`, while items' width is equal to the width of CarouselView.

**I recommend to set the ItemWidth to 500**, although the dafault value is 300.

### About `ItemImageSource` property
it's a `List<string>` property providing the image url source.

The count of `ItemImageSource` should be 3 at least, as you know.

## Further works
- Apparently, an ItemClickedEvent or something like that are needed.
- An indicator, which shows which one is selected