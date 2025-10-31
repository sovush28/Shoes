using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoes
{
    public class ProductArticleValidator
    {
        public string CheckArticle(string article)
        {
            string msg = null;

            article = article.ToUpper().Trim();

            if (article.Length != 6)
                msg = "Длина артикула должна составлять 6 символов";
            else
            {
                // проверка на допустимые символы
                foreach (char c in article)
                {
                    if (!((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
                    {
                        msg = "Артикул может содержать только латинские буквы и цифры";
                        break;
                    }
                }

                /*
                if (string.IsNullOrEmpty(msg))
                {
                    var prodWSameArticle = ShoesDE2026Entities.GetContext().Product.ToList().Where(p => p.Article == article);
                    if (prodWSameArticle.Count() > 0)
                        msg = "Товар с таким артикулом уже существует";
                }
                */
            }

            return msg;
        }
    }
}
