using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LLEAV.ViewModels.Windows;

namespace LLEAV.Views.Windows
{
    public partial class MainWindow : Window
    {
        public static FilePickerFileType LLEAVFile { get; } = new("LLEAV File")
        {
            Patterns = new[] { "*.lleav" }
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        public async void SaveFileAsButton_Clicked(object sender, RoutedEventArgs args)
        {
            var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save File",
                FileTypeChoices = new FilePickerFileType[] { LLEAVFile }
            });

            if (file is not null)
            {
                ((MainWindowViewModel)this.DataContext).SaveAs(file.Path.AbsolutePath);
            }
        }

        public async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
        {
            // Start async operation to open the dialog.
            var file = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open LLEAV File",
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[] { LLEAVFile }
            });

            if (file.Count >= 1)
            {
                ((MainWindowViewModel)this.DataContext).Load(file[0].Path.AbsolutePath);
            }
        }
    }
}