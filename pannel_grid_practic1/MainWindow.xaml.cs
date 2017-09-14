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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pannel_grid_practic1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //фиктивные стоблцы для слоев 1 и 0
        ColumnDefinition column1CloneForLayer0;
        ColumnDefinition column2CloneForLayer0;

        ColumnDefinition column1CloneForLayer1;
        ColumnDefinition column2CloneForLayer1;
        public MainWindow()
        {
            InitializeComponent();
            //Инициализировать фиктивные стоблцы использованные когда панели пристыковваны:

            column1CloneForLayer0 = new ColumnDefinition();
            column1CloneForLayer0.SharedSizeGroup = "column1";
            column2CloneForLayer0 = new ColumnDefinition();
            column2CloneForLayer0.SharedSizeGroup = "column2";
            column2CloneForLayer1 = new ColumnDefinition();
            column2CloneForLayer1.SharedSizeGroup = "column2";

        }

        private void panel1Button_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void panel2Button_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void layer0_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseEnter_1(object sender, MouseEventArgs e)
        {

        }
        //--------------------------------------------------------------
        public void UndockPane(int paneNumber)
        {
            if(paneNumber==1)
            {
                layer1.Visibility = Visibility.Visible;
                panel1Button.Visibility = Visibility.Visible;
                panel1PinImage.Source = new BitmapImage(
                    new Uri("pinHorizontal.gif", UriKind.Relative));
                //Удаляем клонированные столюцы из слоев 0 и 1:
                layer0.ColumnDefinitions.Remove(column1CloneForLayer0);
                //Этот столбец присутствует  не всегда , но метод remove
                //игнарирует попытку удалить несущиствующий столбец
                layer1.ColumnDefinitions.Remove(column2CloneForLayer1);
            }
            else if(paneNumber==2)
            {
                layer2.Visibility = Visibility.Visible;
                panel2Button.Visibility = Visibility.Visible;
                pane2PinImage.Source = new BitmapImage(new Uri("pinHorizontal.gif", UriKind.Relative));
                //удаляем клонированные столбцы из слоев 0 и 1:
                layer0.ColumnDefinitions.Remove(column2CloneForLayer0);
                layer1.ColumnDefinitions.Remove(column2CloneForLayer1);
            }
        }
        
        


            //-----------------------------------------------------

        //переключаем состояние: пристыковано/не пристыковано(панель 1)
        private void panel1Pin_Click(object sender, RoutedEventArgs e)
        {
            if(panel1Button.Visibility==Visibility.Collapsed)
            {
                UndockPane(1);
            }
        }

        private void Grid_MouseEnter_2(object sender, MouseEventArgs e)
        {

        }

        private void panel2Pin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_MouseEnter_3(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseEnter_4(object sender, MouseEventArgs e)
        {

        }
    }
}
