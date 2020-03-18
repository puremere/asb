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
           
            List<vmedia> medialist = manager.getMediaList().Where((x=> !x.title.Contains(".mp4"))).Select(i => new vmedia
            {
                ID = i.ID,
                title = i.title,
                type = i.MediaType.title,

            }).ToList();
            List<string> idlist = medialist.Select(m => m.type).Distinct().ToList();
            List<VtypeList> typeList = manager.getMediaTypeList().Where(c => idlist.Contains(c.title)).Select(i => new VtypeList
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

        public ActionResult blogDetail(int id)
        {
            article article = manager.getArticle(id);
            List<article> whole = manager.getArticleList(0, "", "").ToList();
            int index = whole.IndexOf(article);
            article nextArticle =  (index + 1) <= whole.Count() - 1 ?  whole[index + 1] : null;
            article preArticle = (index - 1) >= 0 && (index - 1) < whole.Count() - 1 ? whole[index - 1] : null; 
               
                blogDetailVM model = new blogDetailVM() {
                     article = article,
                      nextArticle = nextArticle,
                       previousArticle = preArticle

                };
            return View(model);
        }

        public ActionResult Live()
        {
            return View();
        }
        public ActionResult Bio() {
            return View();
        }
        public ActionResult Video() {
            List<vmedia> medialist = manager.getMediaList().Where((x => x.title.Contains(".mp4"))).Select(i => new vmedia
            {
                ID = i.ID,
                title = i.title,
                type = i.MediaType.title,

            }).ToList();


            return View(medialist);
          
        }
        public ActionResult loginRegister(string message)
        {
            ViewBag.LoginMessage = "";

            if (message != null)
            {
                ViewBag.LoginMessage = "نام کاربری یا رمز عبور اشتباه است";
            }
            return View();
        }
        public ActionResult confirmCode(string message)
        {
            ViewBag.message = "";
            if (message != null)
            {
                
                ViewBag.message = "کد تایید صحیح نیست";
            }
            return View();
        }
        [HttpPost]
        public ActionResult Register(string post_phone)
        {
            if (true) // check if user not used and then send code
            {
                
                Session["code"] = "1234";
                return RedirectToAction("confirmCode");
            }
            else
            {
                // error comes from manager
                ViewBag.RegisterMessage = "error";
                return RedirectToAction("confirmCode", new { message = "register"}); 
            }
            
        }
        [HttpPost]
        public ActionResult CheckCode(string post_code)
        {
            if (Session["code"] as string == post_code)
            {
                // create user save to session user
                Session["user"] = 1;
                return RedirectToAction("UserProfile");
            }
            else
            {
                return RedirectToAction("confirmCode", new { message = "error"});
            }
            
        }
        [HttpPost]
        public ActionResult login(string post_username,string post_password)
        {
            // check if user is valid
            if (true)
            {
                Session["user"] = 1;
                return RedirectToAction("UserProfile");
            }
            else
            {
                return RedirectToAction("loginRegister" , new { message = "error"});
            }
            
        }
        public ActionResult UserProfile()
        {
            int id = (int)Session["user"];
            profileVM model = new profileVM() {
                user = manager.getUserByID(id),
                mediaList = manager.getUserMediaList(id).ToList(),
            };
            return View(model);
        }
    }
}