using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CarouselView.Controls
{
    /// <summary>
    /// 扩展依赖属性类
    /// </summary>
    public static class DependencyObjectExtend
    {
        public static IEnumerable<DependencyObject> GetDescendant(this DependencyObject element)
        {
            var list = new List<DependencyObject>();
            var count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                list.Add(child);
                list.AddRange(child.GetDescendant());
            }

            return list;
        }

        /// <summary>
        /// 查找子孙节点中符合类型的首个节点
        /// </summary>
        public static T GetFirstDescendantOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetDescendantsOfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// 查找子孙节点中符合类型的节点
        /// </summary>
        public static IEnumerable<T> GetDescendantsOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetDescendants().OfType<T>();
        }

        /// <summary>
        /// 获取所有的子孙阶段
        /// </summary>
        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject start)
        {
            if (start == null)
                yield break;

            var queue = new Queue<DependencyObject>();
            queue.Enqueue(start);
            yield return start;

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();
                var count2 = VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < count2; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    yield return child;
                    queue.Enqueue(child);
                }
            }
        }
    }
}
