using PhotoSorting.Entities;

namespace PhotoSorting
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        public App()
        {
            using (var dbContext = new DatabaseContext())
                dbContext.EnsureDb();
        }
    }
}
