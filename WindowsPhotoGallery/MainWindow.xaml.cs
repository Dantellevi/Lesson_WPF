using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WindowsPhotoGallery
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ScaleTransform st = new ScaleTransform(3, 3);
        object dummyNode = null;
        Photos photos = new Photos();
        public MainWindow()
        {
            InitializeComponent();
            photos.ItemsUpdated += delegate { this.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(Refresh)); };
        }

        #region Closing
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //проверка на закрытие окна
            if (MessageBox.Show("Are you sure you want to close Photo Gallery?", "Annoying Prompt", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.No)
                e.Cancel = true;


        }

        #endregion

        #region action before closing
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //сохранить список ибранного
        }
        #endregion

        #region Initilize List Folder
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            //восстановить список избранного
            IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForAssembly();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("myFile", System.IO.FileMode.OpenOrCreate, f))
            using (StreamReader reader = new StreamReader(stream))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    AddFavorite(line);
                    line = reader.ReadLine();
                }
            }
            // At least have the user's Pictures folder as a favorite if nothing else
            if (!favoritesItem.HasItems)
            {
                AddFavorite(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            }

            (treeView.Items[0] as TreeViewItem).IsSelected = true;

        }


        private void AddFavorite(string folder)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = folder;
            item.Tag = folder;
            favoritesItem.Items.Add(item);
        }
        #endregion

        #region TreeView Management
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                foldersItem.Items.Add(item);
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Refresh();
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }
                }
                catch (UnauthorizedAccessException) { }
            }
        }

        private void ShowPhoto(bool? showFixBar)
        {
            string filename = (pictureBox.SelectedItem as ListBoxItem).Tag as string;
            imageView.Visibility = Visibility.Visible;
            backButton.Visibility = Visibility.Visible;
            image.Source = new BitmapImage(new Uri(filename));
            if (showFixBar == true)
                fixBar.Visibility = Visibility.Visible;
            else if (showFixBar == false)
                fixBar.Visibility = Visibility.Collapsed;
        }

        void AddPhotosInFolder(string folder)
        {
            try
            {
                foreach (string s in Directory.GetFiles(folder, "*.jpg"))
                {
                    Photo photo = new Photo(s);
                    photos.Add(photo);

                    // Construct the ListBoxItem with an Image as content:
                    ListBoxItem item = new ListBoxItem();
                    item.Padding = new Thickness(3, 8, 3, 8);
                    item.MouseDoubleClick += delegate { ShowPhoto(false); };
                    TransformGroup tg = new TransformGroup();
                    tg.Children.Add(st);
                    tg.Children.Add(new RotateTransform());
                    item.LayoutTransform = tg;
                    item.Tag = s;

                    Image image = new Image();
                    image.Height = 35;

                    // If the image contains a thumbnail, use that instead of the entire image:
                    Uri uri = new Uri(s);
                    BitmapDecoder bd = BitmapDecoder.Create(uri, BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
                    if (bd.Frames[0].Thumbnail != null)
                        image.Source = bd.Frames[0].Thumbnail;
                    else
                        image.Source = new BitmapImage(uri);

                    // Construct a ToolTip for the item:
                    Image toolImage = new Image();
                    toolImage.Source = bd.Frames[0].Thumbnail;

                    TextBlock textBlock1 = new TextBlock();
                    textBlock1.Text = System.IO.Path.GetFileName(s);
                    TextBlock textBlock2 = new TextBlock();
                    textBlock2.Text = photo.DateTime.ToString();

                    StackPanel sp = new StackPanel();
                    sp.Children.Add(toolImage);
                    sp.Children.Add(textBlock1);
                    sp.Children.Add(textBlock2);

                    item.ToolTip = sp;
                    item.Content = image;

                    pictureBox.Items.Add(item);
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
        }

        private void Refresh()
        {
            try
            {
                this.Cursor = Cursors.Wait;

                // Go back to the gallery if we're viewing an individual photo:
                imageView.Visibility = Visibility.Hidden;
                backButton.Visibility = Visibility.Hidden;

                pictureBox.Items.Clear();
                photos.Clear();

                if (treeView.SelectedItem == favoritesItem)
                {
                    foreach (TreeViewItem item in favoritesItem.Items)
                    {
                        AddPhotosInFolder(item.Tag as string);
                    }
                    favoritesMenu.IsEnabled = false;
                }
                else if (treeView.SelectedItem != foldersItem)
                {
                    string folder = (treeView.SelectedItem as TreeViewItem).Tag as string;
                    AddPhotosInFolder(folder);

                    // Update the favorites menu text depending on whether the folder is already a favorite
                    favoritesMenu.IsEnabled = true;
                    foreach (TreeViewItem item in favoritesItem.Items)
                    {
                        if (item.Header as string == folder)
                        {
                            favoritesMenu.Header = "Remove Current Folder from Fa_vorites";
                            return;
                        }
                    }
                    favoritesMenu.Header = "Add Current Folder to Fa_vorites";
                }
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }
        
        private void RemoveFavorite(string folder)
        {
            for (int i = 0; i < favoritesItem.Items.Count; i++)
            {
                if ((favoritesItem.Items[i] as TreeViewItem).Header as string == folder)
                {
                    favoritesItem.Items.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion

        #region Menu Handlers

        private void favoritesMenu_Click(object sender, RoutedEventArgs e)
        {
            string folder = (treeView.SelectedItem as TreeViewItem).Tag as string;
            if (favoritesMenu.Header as string == "Add Current Folder to Fa_vorites")
            {
                AddFavorite(folder);
                favoritesMenu.Header = "Remove Current Folder from Fa_vorites";
            }
            else
            {
                RemoveFavorite(folder);
                favoritesMenu.Header = "Add Current Folder to Fa_vorites";
            }
        }

        //------------------------------------------------------------
        private void DeleteCurrentPhoto()
        {
            string filename = (pictureBox.SelectedItem as ListBoxItem).Tag as string;
            if (MessageBox.Show("Are you sure you want to delete '" + filename + "'?", "Delete Picture", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                try
                {
                    File.Delete(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Cannot Rename File", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        //--------------------------------------------------------

        private void deleteMenu_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrentPhoto();
        }

        private void renameMenu_Click(object sender, RoutedEventArgs e)
        {
            string filename = (pictureBox.SelectedItem as ListBoxItem).Tag as string;
            RenameDialog dialog = new RenameDialog(System.IO.Path.GetFileNameWithoutExtension(filename));
            if (dialog.ShowDialog() == true) // Result could be true, false, or null
            {
                // Attempt to rename the file
                try
                {
                    File.Move(filename, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), dialog.NewFilename) + System.IO.Path.GetExtension(filename));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Cannot Rename File", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void _Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void fixMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowPhoto(true);
        }

        private void printMenu_Click(object sender, RoutedEventArgs e)
        {
            string filename = (pictureBox.SelectedItem as ListBoxItem).Tag as string;
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute));

            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true) // Result could be true, false, or null
                pd.PrintVisual(image, System.IO.Path.GetFileName(filename) + " from Photo Gallery");
        }

        private void editMenu_Click(object sender, RoutedEventArgs e)
        {
            string filename = (pictureBox.SelectedItem as ListBoxItem).Tag as string;
            System.Diagnostics.Process.Start("mspaint.exe", filename);
            
        }


        #endregion

        #region Close Window
        private void E_xit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion


        #region Bottom Button Handlers

        private void defaultSizeButton_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = 3;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            imageView.Visibility = Visibility.Hidden;
            backButton.Visibility = Visibility.Hidden;
        }

        private void zoomButton_Click(object sender, RoutedEventArgs e)
        {
            zoomPopup.IsOpen = true;
        }

        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            st.ScaleX = zoomSlider.Value;
            st.ScaleY = zoomSlider.Value;
        }

        private void zoomPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            zoomPopup.IsOpen = false;
        }

        private void slideshowButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("NYI");
        }

        private void clockwiseButton_Click(object sender, RoutedEventArgs e)
        {
            if (pictureBox.SelectedItem != null)
            {
                RotateTransform rt = ((pictureBox.SelectedItem as ListBoxItem).LayoutTransform as TransformGroup).Children[1] as RotateTransform;
                rt.Angle += 90;
            }
        }
        private void counterclockwiseButton_Click(object sender, RoutedEventArgs e)
        {
            if (pictureBox.SelectedItem != null)
            {
                RotateTransform rt = ((pictureBox.SelectedItem as ListBoxItem).LayoutTransform as TransformGroup).Children[1] as RotateTransform;
                rt.Angle -= 90;
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            int index = pictureBox.SelectedIndex - 1;
            if (index < 0) index = pictureBox.Items.Count - 1;
            (pictureBox.Items[index] as ListBoxItem).IsSelected = true;
            pictureBox.ScrollIntoView(pictureBox.SelectedItem);

            if (imageView.Visibility == Visibility.Visible)
            {
                ShowPhoto(null);
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            int index = pictureBox.SelectedIndex + 1;
            if (index == pictureBox.Items.Count) index = 0;
            (pictureBox.Items[index] as ListBoxItem).IsSelected = true;
            pictureBox.ScrollIntoView(pictureBox.SelectedItem);

            if (imageView.Visibility == Visibility.Visible)
            {
                ShowPhoto(null);
            }
        }
        
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrentPhoto();
        }


        #endregion

        private void pictureBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                deleteMenu.IsEnabled = false;
                renameMenu.IsEnabled = false;
                fixMenu.IsEnabled = false;
                printMenu.IsEnabled = false;
                editMenu.IsEnabled = false;
                previousButton.IsEnabled = false; previousButton.Opacity = 0.5;
                nextButton.IsEnabled = false; nextButton.Opacity = 0.5;
                counterclockwiseButton.IsEnabled = false; counterclockwiseButton.Opacity = 0.5;
                clockwiseButton.IsEnabled = false; clockwiseButton.Opacity = 0.5;
                deleteButton.IsEnabled = false; deleteButton.Opacity = 0.5;

            }
            else
            {
                deleteMenu.IsEnabled = true;
                renameMenu.IsEnabled = true;
                fixMenu.IsEnabled = true;
                printMenu.IsEnabled = true;
                editMenu.IsEnabled = true;
                previousButton.IsEnabled = true; previousButton.Opacity = 1;
                nextButton.IsEnabled = true; nextButton.Opacity = 1;
                counterclockwiseButton.IsEnabled = true; counterclockwiseButton.Opacity = 1;
                clockwiseButton.IsEnabled = true; clockwiseButton.Opacity = 1;
                deleteButton.IsEnabled = true; deleteButton.Opacity = 1;
            }
        }

        #region Fix Bar Handlers
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (image.LayoutTransform as RotateTransform).Angle += 90;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (image.LayoutTransform as RotateTransform).Angle -= 90;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("NYI");
        }


        #endregion





    }
}
