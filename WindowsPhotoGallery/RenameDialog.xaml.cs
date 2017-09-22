using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowsPhotoGallery
{
    /// <summary>
    /// Логика взаимодействия для RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window
    {
        string newFilename;
        string oldFilename;
        public RenameDialog()
        {
            InitializeComponent();

            textBox.Text = oldFilename;
            textBox.Focus();
        }

        public RenameDialog(string oldFilename)
        {
            InitializeComponent();
            this.oldFilename = oldFilename;
            textBox.Text = oldFilename;
            textBox.Focus();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            newFilename = textBox.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string OldFilename
        {
            get { return oldFilename; }
            set { oldFilename = value; }
        }

        public string NewFilename
        {
            get { return newFilename; }
        }
    }
}
