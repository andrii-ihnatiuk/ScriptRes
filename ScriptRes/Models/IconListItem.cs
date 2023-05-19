using System.Windows.Media;

namespace ScriptRes.Models
{
    internal class IconListItem
    {
        public int Id { get; set; }
        public ImageSource ImageSource { get; set; }

        public IconListItem(int id, ImageSource source)
        {
            Id = id;
            ImageSource = source;
        }
    }
}
