using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using LLEAV.ViewModels.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Intrinsics.Arm;

namespace LLEAV.Views.Windows
{
    public partial class PopulationWindow : Window
    {
        public PopulationWindow()
        {
            InitializeComponent();
        }

        public async void Save_Clicked(object sender, RoutedEventArgs args)
        {
            var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Image",
            });

            if (file is not null)
            {
                var target = this.Find<Panel>("Tree");

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