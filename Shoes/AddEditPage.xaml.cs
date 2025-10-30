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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>

    public partial class AddEditPage : Page
    {
        private Product currentProduct = new Product();

        private bool isThisEditingMode;
        public AddEditPage(Product selectedProduct)
        {
            InitializeComponent();

            if (selectedProduct != null)
            {
                currentProduct = selectedProduct;
                ArticleTB.Opacity = 0.5;
                ArticleTB.IsEnabled = false;
                isThisEditingMode = true;

                ManufacturerCB.SelectedValue = currentProduct.ProductManufacturer;
                CategoryCB.SelectedValue = currentProduct.ProductCategory;
            }
            else
                isThisEditingMode = false;

            DataContext = currentProduct;

            List<String> allManufacturers = new List<String>();
            foreach(Product prod in ShoesDE2026Entities.GetContext().Product.ToList())
            {
                allManufacturers.Add(prod.ProductManufacturer.ToString());
            }
            ManufacturerCB.ItemsSource = allManufacturers.Distinct();

            List<String> allCategories = new List<String>();
            foreach (Product prod in ShoesDE2026Entities.GetContext().Product.ToList())
            {
                allCategories.Add(prod.ProductCategory.ToString());
            }
            CategoryCB.ItemsSource = allCategories.Distinct();
        }

        /// /////////////////////////////////////////////////////////
        private string CheckArticle(string article)
        {
            string msg = "";

            article = article.Trim();

            if (article.Length != 6)
                msg = "Длина артикула должна составлять 6 символов";
            else
            {
                var prodWSameArticle = ShoesDE2026Entities.GetContext().Product.ToList().Where(p => p.Article == article);
                if (prodWSameArticle.Count() > 0)
                    msg = "Товар с таким артикулом уже существует";
            }

            return msg;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (!isThisEditingMode)
            {
                string articleErrorMsg = "";

                if (string.IsNullOrWhiteSpace(currentProduct.Article))
                    errors.AppendLine("Введите артикул товара");
                else
                {
                    articleErrorMsg = CheckArticle(currentProduct.Article.Trim());
                    if (articleErrorMsg.Length > 0)
                        errors.AppendLine(articleErrorMsg);
                    /*if (currentProduct.Article.Trim().Length != 6)
                        errors.AppendLine("Длина артикула должна составлять 6 символов");
                    else
                    {
                        Product prodWSameArticle = new Product();
                        prodWSameArticle = ShoesDE2026Entities.GetContext().Product.ToList().Find(p => p.Article == currentProduct.Article);
                        if (prodWSameArticle != null)
                            errors.AppendLine("Товар с таким артикулом уже существует");
                    }*/
                }
            }

            if (string.IsNullOrWhiteSpace(currentProduct.ProductName))
                errors.AppendLine("Введите наименование товара");

            if (string.IsNullOrWhiteSpace(currentProduct.Unit))
                errors.AppendLine("Введите ед. измерения товара");

            /*           
            if (!int.TryParse(BuildingTB.Text.Trim(), out int building))
                errors.AppendLine("Номер здания должен быть целым числом");
            */

            if (!decimal.TryParse(PriceTB.Text.Trim(), out decimal curProdPrice))
                errors.AppendLine("Цена товара должна быть числом");
            else
            {
                if (currentProduct.ProductPrice <= 0)
                    errors.AppendLine("Цена товара должна быть числом больше 0");
                else
                    currentProduct.ProductPrice = curProdPrice;
            }

            if (string.IsNullOrWhiteSpace(currentProduct.ProductSupplier))
                errors.AppendLine("Введите наименование поставщика");

            if (ManufacturerCB.SelectedValue == null)
                errors.AppendLine("Выберите наименование производителя");

            if (CategoryCB.SelectedValue == null)
                errors.AppendLine("Выберите категорию товара");

            if (!int.TryParse(DiscountTB.Text.Trim(), out int curProdDiscount))
                errors.AppendLine("Скидка должна быть целым числом");
            else
            {
                if (currentProduct.ProductDiscount < 0)
                    errors.AppendLine("Скидка не может быть меньше 0");
                else currentProduct.ProductDiscount = curProdDiscount;
            }

            if (!int.TryParse(QInStockTB.Text.Trim(), out int curProdQuantity))
                errors.AppendLine("Количество должно быть целым числом");
            else
            {
                if (currentProduct.QuantityInStock < 0)
                    errors.AppendLine("Количество не может быть меньше 0");
                else currentProduct.QuantityInStock = curProdQuantity;
            }

            if (string.IsNullOrWhiteSpace(currentProduct.ProductDescription))
                errors.AppendLine("Введите описание товара");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (!isThisEditingMode)
                ShoesDE2026Entities.GetContext().Product.Add(currentProduct);

            try
            {
                ShoesDE2026Entities.GetContext().SaveChanges();
                MessageBox.Show("Изменения сохранены");
                Manager.MainFrame.GoBack();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
