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

        //------------------------------------------------------------------------
                //Показывает панель 1 когда указатель мыши находится над ее кнопкой
        private void panel1Button_MouseEnter(object sender, MouseEventArgs e)
        {
            layer1.Visibility = Visibility.Visible;
            //коррекцируем Z-порядок , чтобы панель всегда была вверху:
            Grid.SetZIndex(layer1, 1);
            Grid.SetZIndex(layer2, 0);
            //скрываем вторую панел если она не пристыкована
            if(panel2Button.Visibility==Visibility.Visible)
            {
                layer2.Visibility = Visibility.Collapsed;
            }

        }

        //-----------------------------------------------------------------------
        //Показываем панель 2 когда указатель мыши находится над ее кнопкой
        private void panel2Button_MouseEnter(object sender, MouseEventArgs e)
        {
            layer2.Visibility = Visibility.Visible;
            //Коррекцируем Z-порядок, чтобы панел всегда была вверху:
            Grid.SetZIndex(layer2, 1);
            Grid.SetZIndex(layer1, 0);
            //скрываем вторую панель если она не пристыкована
            if(panel1Button.Visibility==Visibility.Visible)
            {
                layer1.Visibility = Visibility.Collapsed;
            }
        }

        //------------------------------------------------------------------------

        private void layer0_MouseEnter(object sender, MouseEventArgs e)
        {
            if(panel1Button.Visibility==Visibility.Visible)
            {
                layer1.Visibility = Visibility.Collapsed;
            }
            if(panel2Button.Visibility==Visibility.Visible)
            {
                layer2.Visibility = Visibility.Collapsed;
            }
        }
        //-------------------------------------------------------------------------
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseEnter_1(object sender, MouseEventArgs e)
        {

        }
        //-------------------------------------------------------------
        //пристыковать панель, при этом скрывается соответствующая ей кнопка
        public void DockPane(int paneNumber)
        {
            if(paneNumber==1)
            {
                panel1Button.Visibility = Visibility.Collapsed;
                panel1PinImage.Source = new BitmapImage(new Uri("bin/Debug/pin.gif", UriKind.Relative));
                //добавляем клонированный столбец в сой 0:
                layer0.ColumnDefinitions.Add(column1CloneForLayer0);
                //добавляем клонированный столбец в слой 1, но только если панель 2 пристыкована
                if(panel2Button.Visibility==Visibility.Collapsed)
                {
                    layer1.ColumnDefinitions.Add(column2CloneForLayer1);
                }
                else if(paneNumber==2)
                {
                    pane2PinImage.Source= new BitmapImage(new Uri("bin/Debug/pin.gif", UriKind.Relative));
                    //добавляем клонированный столбец в слой 0
                    layer0.ColumnDefinitions.Add(column2CloneForLayer0);
                    //добавляем клонированный столбец с лой 1, но только если панель 1 пристыкована
                    if(panel1Button.Visibility==Visibility.Collapsed)
                    {
                        layer1.ColumnDefinitions.Add(column2CloneForLayer1);
                    }
                }
            }
        }
        //--------------------------------------------------------------
        public void UndockPane(int paneNumber)
        {
            if(paneNumber==1)
            {
                layer1.Visibility = Visibility.Visible;
                panel1Button.Visibility = Visibility.Visible;
                panel1PinImage.Source = new BitmapImage(
                    new Uri("bin/Debug/pinHorizontal.gif", UriKind.Relative));
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
                pane2PinImage.Source = new BitmapImage(new Uri("bin/Debug/pinHorizontal.gif", UriKind.Relative));
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
            else
            {
                DockPane(1);
            }

        }

        private void Grid_MouseEnter_2(object sender, MouseEventArgs e)
        {

        }

        private void panel2Pin_Click(object sender, RoutedEventArgs e)
        {
            if(panel2Button.Visibility==Visibility.Collapsed)
            {
                UndockPane(2);
            }
            else
            {
                DockPane(2);
            }
        }


        //-------------------------------------------------------------------
        //скрываем вторую панель если оона не пристыкована, когда указатель мыши перемещается
        //на панель 1
        private void Grid_MouseEnter_3(object sender, MouseEventArgs e)
        {
            //скрываем вторую панель если она не пристыкована
            if(panel2Button.Visibility==Visibility.Visible)
            {
                layer2.Visibility = Visibility.Collapsed;
            }
        }

        //--------------------------------------------------------------------
        //Скрываем вторую панель если она не пристыкована, когда указатель мыши перемещается на панель2

        private void Grid_MouseEnter_4(object sender, MouseEventArgs e)
        {
            //скрываем вторую панель если она не пристыкована
            if(panel1Button.Visibility==Visibility.Visible)
            {
                layer1.Visibility = Visibility.Collapsed;
            }
        }
    }
}
