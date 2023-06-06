using System.Windows;

namespace NetworkService.Model
{
    public class MessageChange
    {
        public Visibility Visibility_Success { get; set; }
        public Visibility Visibility_Error { get; set; }
        public string Mess { get; set; }

        public MessageChange()
        {

        }
    }
}
