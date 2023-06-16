using NetworkService.ViewModel;

namespace System.lO
{
    public class File
    {
        public static string[] ReadAllLines(string path)
        {
            return HomeViewModel.DELIMITER_CONST.ToArray();
        }

        public static bool Exists(string path)
        {
            return IO.File.Exists(path);
        }
    }
}
