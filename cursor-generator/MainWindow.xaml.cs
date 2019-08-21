using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

using cursor_generator.Util;
using cursor_generator.Constants;
using System.Drawing.Imaging;

namespace cursor_generator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Bitmap bitmap;
        
        private Bitmap CreateBitmap(string path) {
            return new Bitmap(path);
        }
        
        private void SaveIcon(Bitmap bmp, string fileName) {
            IntPtr Hicon = bmp.GetHicon();
            Icon icon = System.Drawing.Icon.FromHandle(Hicon);
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate);
            icon.Save(fs);
            fs.Close();

            icon.Dispose();
        }
        
        private void SaveSelectedCursor(string saveFileName) {
            SaveIcon(bitmap, saveFileName);

            Byte[] bytes = File.ReadAllBytes(saveFileName);
            bytes[2] = 0x02;

            File.WriteAllBytes(saveFileName, bytes);
            string cursorName = Path.Combine(Path.GetDirectoryName(saveFileName), Path.GetFileNameWithoutExtension(saveFileName) + ".cur");
            File.Move(saveFileName, cursorName);
        }

        public MainWindow() {
            InitializeComponent();

            this.sourceImageTextBox.IsReadOnly = true;
        }

        private void saveCursorButton_Click(object sender, RoutedEventArgs e) {
            if (bitmap == null) {
                throw new InvalidOperationException("No cursor was selected");
            }
            
            using (var dialog = new SaveFileDialog()) {
                dialog.Filter = "ico files (*.ico)|*.ico";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    SaveSelectedCursor(dialog.FileName);
                }
            }
        }

        private void selectSourceImageButton_Click(object sender, RoutedEventArgs e) {
            //var size = 32;

            using (var dialog = new OpenFileDialog()) {
                dialog.Filter = ImageConstants.ALL_IMAGE_FILTERS;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    if (bitmap != null) {
                        bitmap.Dispose();
                    }

                    bitmap = CreateBitmap(dialog.FileName);
                    sourceImageTextBox.Text = dialog.FileName;
                }
            }
        }
    }
}
