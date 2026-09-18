namespace Pocket_Updater.UI
{

    internal static class LibraryChangeNotifier
    {
        public static event EventHandler? Changed;

        public static void Notify()
        {
            Changed?.Invoke(null, EventArgs.Empty);
        }
    }
}
