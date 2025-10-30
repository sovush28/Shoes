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

namespace Shoes
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        // public currentorder= new orderproduct?

        public ProductPage(User currentUser)
        {
            InitializeComponent();

            if (currentUser == null)
            {
                UserNameTB.Text = "Гость";
                UserRoleTB.Text = "Гость";

                SearchEtcStackPanel.Visibility = Visibility.Collapsed;
                AddBtn.Visibility = Visibility.Collapsed;
                ViewOrdersBtn.Visibility = Visibility.Collapsed;
                OrderBtn.Visibility = Visibility.Collapsed;
                //EditDeleteBtnsStackPanel.Visibility = Visibility.Collapsed;
                ChangeEditDeleteBtnVisibility(false);
            }
            else
            {
                UserNameTB.Text = currentUser.UserSurname + " " + currentUser.UserFirstName;
                if (currentUser.UserPatronymic != null)
                    UserNameTB.Text += (" " + currentUser.UserPatronymic);

                UserRoleTB.Text = currentUser.UserRoleString;

                switch (currentUser.UserRoleID)
                {
                    case 1: //администратор
                        { 
                            SearchEtcStackPanel.Visibility = Visibility.Visible;
                            AddBtn.Visibility = Visibility.Visible;
                            ViewOrdersBtn.Visibility = Visibility.Visible;
                            OrderBtn.Visibility = Visibility.Visible;
                            //EditDeleteBtnsStackPanel.Visibility = Visibility.Visible;
                            ChangeEditDeleteBtnVisibility(true);
                        }
                        ; break;
                    case 2: //менеджер
                        {
                            SearchEtcStackPanel.Visibility = Visibility.Visible;
                            ViewOrdersBtn.Visibility = Visibility.Visible;
                            AddBtn.Visibility = Visibility.Collapsed;
                            OrderBtn.Visibility = Visibility.Collapsed;
                            //EditDeleteBtnsStackPanel.Visibility = Visibility.Collapsed;
                            ChangeEditDeleteBtnVisibility(false);
                        }
                        ; break;
                    case 3: //авт. клиент
                        {
                            SearchEtcStackPanel.Visibility = Visibility.Collapsed;
                            AddBtn.Visibility = Visibility.Collapsed;
                            ViewOrdersBtn.Visibility = Visibility.Collapsed;
                            OrderBtn.Visibility = Visibility.Collapsed;
                            //EditDeleteBtnsStackPanel.Visibility = Visibility.Collapsed;
                            ChangeEditDeleteBtnVisibility(false);
                        }
                        ; break;
                }
            }

            var currentProducts = ShoesDE2026Entities.GetContext().Product.ToList();

            ProductListView.ItemsSource = currentProducts;

            //выбор поставщиков и удаление дубликатов
            List<String> SupplierCBItems = new List<string>(); //динамический массив строк
            SupplierCBItems.Add("Все поставщики");
            var allSuppliers = currentProducts.Select(p => p.ProductSupplier).Distinct().ToList(); //выбор поставщиков из бд без дубликатов
            foreach (string supplier in allSuppliers)
            {
                SupplierCBItems.Add(supplier);
            }
            //SupplierCB.ItemsSource = currentProducts.Select(p => p.ProductSupplier).Distinct().ToList();
            SupplierCB.ItemsSource = SupplierCBItems;

            UpdateProducts();
        }

        private void ChangeEditDeleteBtnVisibility(bool showBtns)
        {
            ProductListView.Loaded += (s, e) =>
            {
                UpdateEditDelBtnsVisibility(showBtns);
            };
            ProductListView.LayoutUpdated += (s, e) =>
            {
                UpdateEditDelBtnsVisibility(showBtns);
            };
        }

        private void UpdateEditDelBtnsVisibility(bool showBtns)
        {
            foreach (var productEntry in ProductListView.Items)
            {
                // Получаем контейнер элемента ListView
                ListViewItem container = ProductListView.ItemContainerGenerator.ContainerFromItem(productEntry) as ListViewItem;

                if (container != null)
                {
                    // Ищем StackPanel с кнопками в визуальном дереве
                    StackPanel editPanel = FindVisualChild<StackPanel>(container);

                    if (!showBtns) //спрятать
                    {
                        if (editPanel != null && editPanel.Name == "EditDeleteBtnsStackPanel")
                        {
                            editPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    else //показать
                    {
                        if (editPanel != null && editPanel.Name == "EditDeleteBtnsStackPanel")
                        {
                            editPanel.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        // Вспомогательный метод для поиска в визуальном дереве
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;

                var descendant = FindVisualChild<T>(child);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }

        private void UpdateProducts()
        {
            var currentProducts = ShoesDE2026Entities.GetContext().Product.ToList();

            /// фильтрация
            /// поиск
            /// сортировка
            /// загрузка списка в листвью


            //фильтрация
            if (SupplierCB.SelectedValue != null)
            {
                switch (SupplierCB.SelectedValue.ToString())
                {
                    case "Все поставщики":
                        break;

                    default:
                        currentProducts = currentProducts.Where(p => p.ProductSupplier == SupplierCB.SelectedValue.ToString()).ToList();
                        break;
                }
            }

            //поиск
            currentProducts = currentProducts.Where(p => p.StringInfoCombined.ToLower().Contains(SearchTB.Text.ToLower())).ToList();

            //сортировка
            if (SortAscRB.IsChecked == true)
                currentProducts = currentProducts.OrderBy(p => p.QuantityInStock).ToList();
            if (SortDescRB.IsChecked == true)
                currentProducts = currentProducts.OrderByDescending(p => p.QuantityInStock).ToList();


            ProductListView.ItemsSource = currentProducts;
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void SupplierCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void SortDescRB_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void SortAscRB_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentProduct = (sender as Button).DataContext as Product;

            var currentOrderProducts = ShoesDE2026Entities.GetContext().OrderProduct.Where(op => op.ProductArticle == currentProduct.Article).ToList();

            if (currentOrderProducts.Count > 0)
            {
                MessageBox.Show("Невозможно удалить запись о товаре, т.к. существуют записи о заказах с ним");
                return;
            }

            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    ShoesDE2026Entities.GetContext().Product.Remove(currentProduct);
                    ShoesDE2026Entities.GetContext().SaveChanges();

                    UpdateProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Product));
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void ViewOrdersBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
