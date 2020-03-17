using asb.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using asb.Models;
using System.Text.RegularExpressions;

namespace asb.Controllers
{
    public class HomeController : Controller
    {
        dbManger manager = new dbManger();
        public ActionResult Index()
        {
           
            List<vmedia> medialist = manager.getMediaList().Select(i => new vmedia
            {
                ID = i.ID,
                title = i.title,
                type = i.MediaType.title,

            }).ToList();
            List<int> idlist = medialist.Select(m => m.ID).Distinct().ToList();
            List<VtypeList> typeList = manager.getMediaTypeList().Where(c => idlist.Contains(c.typeID)).Select(i => new VtypeList
            {
                 title = i.title,
                  ID = i.typeID
            }).ToList();
          
            HomeIndexVM model = new HomeIndexVM() {
                 mediaList = medialist,
                  typeList = typeList
            };
           
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public static string ExtractHtmlInnerText(string htmlText)
        {
            //Match any Html tag (opening or closing tags) 
            // followed by any successive whitespaces
            //consider the Html text as a single line

            Regex regex = new Regex("(<.*?>\\s*)+", RegexOptions.Singleline);

            // replace all html tags (and consequtive whitespaces) by spaces
            // trim the first and last space

            string resultText = regex.Replace(htmlText, " ").Trim();

            return resultText;
        }
        public ActionResult Blog()
        {
            HomeBlogVM model = new HomeBlogVM()
            {
                articleList = manager.getArticleList(0, "", "").ToList(),
                imageTitle = "blogImage.jpg"
            };
           foreach(var item in model.articleList)
            {
                item.content = ExtractHtmlInnerText(item.content);
            }
            return View(model);
        }
    }
}