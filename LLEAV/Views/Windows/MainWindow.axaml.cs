using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using LLEAV.ViewModels.Windows;
using LLEAV.Views.Controls.PopulationDepictions;
using System.Diagnostics;
using System.Linq;

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

        public async void Save_Clicked(object sender, RoutedEventArgs args)
        {
            var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Image",
            });

            if (file is not null)
            {
                var depictionView = this.GetLogicalDescendants().OfType<PopulationDepictionViewBase>().First(s => s.IsVisible);


                if (depictionView == null) return;

                var target = depictionView.Find<ItemsControl>("Containers");

                if (target == null) return;

                var pixelSize = new PixelSize((int)target.Bounds.Width, (int)target.Bounds.Height);
                var size = new Size(target.Bounds.Width, target.Bounds.Height);


                using (RenderTargetBitmap bitmap = new RenderTargetBitmap(pixelSize))
                {
                    target.Measure(size);
                    target.Arrange(new Rect(size));
                    bitmap.Render(target);
                    bitmap.Save(file.Path.AbsolutePath.Replace('/', '\\'));
                }
            }
        }
    }
}