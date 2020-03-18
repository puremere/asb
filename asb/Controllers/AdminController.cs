using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using asb.ViewModel;
using asb.Models;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace asb.Controllers
{
    public class AdminController : Controller
    {
        dbManger manager = new dbManger();
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }


        #region blogRegion
       
        public ActionResult blog()
        {

            Session["imageListAdd"] = "";
            Session["imageListEdit"] = "";
            string device = RandomString(10);
            string code = MD5Hash(device + "ncase8934f49909");
            string result = "";
            //using (WebClient client = new WebClient())
            //{

            //    var collection = new NameValueCollection();
            //    collection.Add("device", device);
            //    collection.Add("code", code);
            //    collection.Add("servername", servername);
            //    byte[] response = client.UploadValues(ConfigurationManager.AppSettings["server"] + "/Admin/getDataCatArticle.php", collection);

            //    result = System.Text.Encoding.UTF8.GetString(response);
            //}

            List<articleCatVM> log2 = manager.getCatArticleList().Select(i => new articleCatVM
            {
                articleCatID = i.articleCatID,
                image = i.image,
                title = i.title,
                titleEn = i.titleEn
            }).ToList();
            List<articleVM> log = manager.getArticleList(0,"","").Select(i => new articleVM
            {
                arcticleID = i.arcticleID,
                articleCatID = i.articleCatID,
                content = i.content,
                title = i.title,
                date = i.date,
                hashtags = i.hashtags,
                image = i.image,
                link = i.link,
                view = i.view,
                writer = i.writer,
                catname = i.articleCat.title


            }).ToList();

            ViewModel.AdminBlogVM model = new ViewModel.AdminBlogVM()
            {
                Articlelist = log,
                Catlist = log2
            };

            return View(model);
        }
        public List<article> getArticleList(int cat, string search,string tag)
        {
            List<article> model = manager.getArticleList(cat, search,tag).ToList();
            return model;
        }
       
        public ActionResult setNewArticle(ViewModel.newArcticelVM model)
        {
            if (model.description.Contains("script"))
            {
                return RedirectToAction("blog", "Admin");
            }
            model.tag = model.tag.Replace(",", "-");

            string ss = Session["imageListAdd"] as string;
            ss = ss.Substring(0, ss.Length - 1);
            List<string> imageList = ss.Split(',').ToList();
            string imagename = "";
            if (imageList != null)
            {
                imagename = imageList[0];
            }
            string device = RandomString(10);
            string code = MD5Hash(device + "ncase8934f49909");

            article newArticle = new article()
            {
                articleCatID = model.catList,
                content = model.description,
                hashtags = model.tag,
                date = DateTime.Now,
                image = imagename,
                title = model.title,
                writer = "مدیر",
                

            };
            dbRespose res = manager.addArticle(newArticle);
            if (res.status == 200)
            {
                return RedirectToAction("blog");
            }
            else
            {
                return RedirectToAction("blog");
            }

        }
        [HttpPost]
        public ActionResult setNewCatArticle(string image, string title)
        {
            string pathString = "~/images/panelimages";
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }
            string imagename = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {

                HttpPostedFileBase hpf = Request.Files[i];

                if (hpf.ContentLength == 0)
                    continue;
                imagename = RandomString(7) + Path.GetExtension(hpf.FileName);
                string savedFileName = Path.Combine(Server.MapPath(pathString), imagename);
                hpf.SaveAs(savedFileName);
            }
            string device = RandomString(10);
            //string code = MD5Hash(device + "ncase8934f49909");
            articleCat NewCat = new articleCat()
            {
                image = imagename,
                title = title,
                titleEn = RandomString(5),

            };

            dbRespose res = manager.addArticleCat(NewCat);
            if (res.status == 200)
            {
                return RedirectToAction("blog");

            }
            else
            {
                return RedirectToAction("blog");

            }

        }
        public ActionResult updateArticle(ViewModel.updateArticleVM model)
        {
            model.tagupdate = model.tagupdate.Replace(",", "-");

            //string device = RandomString(10);
            //string code = MD5Hash(device + "ncase8934f49909");
            string image = "";
            if (Session["imageListEdit"] != null)
            {
                string ss = Session["imageListEdit"] as string;
                // ss = ss + filename;
                ss = ss.Length>0 ?  ss.Substring(0, ss.Length - 1) : "";
                List<string> imaglist = ss.Split(',').ToList();
                image = imaglist[0];
            }
            

            article updatedArticle = new article()
            {
                articleCat = manager.getCatArtice(model.catListupdate),
                content = model.descriptionupdate,
                hashtags = model.tagupdate,
                date = DateTime.Now,
                image = image,
                arcticleID = Int32.Parse(model.IDupdate),
                 title = model.titleupdate
            };
            dbRespose res = manager.updataArtice(updatedArticle);
            if (res.status == 200)
            {
                return RedirectToAction("blog");

            }
            else
            {
                return RedirectToAction("blog");

            }

        }
        [HttpPost]
        public ActionResult updateCArticle(int CIDupdate, string Cimageupdate, string Ctitleupdate)
        {
            string imagename = "";
            string pathString = "~/images/panelimages";
            if (Cimageupdate != "")
            {


                string oldFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(Cimageupdate));
                System.IO.File.Delete(oldFileName);
            }
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }



            for (int i = 0; i < Request.Files.Count; i++)
            {

                HttpPostedFileBase hpf = Request.Files[i];

                if (hpf.ContentLength == 0)
                    continue;
                imagename = RandomString(7) + Path.GetExtension(hpf.FileName);
                string savedFileName = Path.Combine(Server.MapPath(pathString), imagename);
                hpf.SaveAs(savedFileName);



            }
            articleCat model = new articleCat()
            {
                image = imagename,
                title = Ctitleupdate,
                articleCatID = CIDupdate,

            };
            dbRespose response = manager.updataCatArtice(model);
            if (response.status == 200)
            {
                return RedirectToAction("blog");
            }
            else
            {
                return RedirectToAction("blog");
            }

            //string device = RandomString(10);
            //string code = MD5Hash(device + "ncase8934f49909");
            //string result = "";
            //using (WebClient client = new WebClient())
            //{

            //    var collection = new NameValueCollection();
            //    collection.Add("servername", servername);
            //    collection.Add("device", device);
            //    collection.Add("code", code);
            //    collection.Add("image", imagename);
            //    collection.Add("title", Ctitleupdate);
            //    collection.Add("ID", CIDupdate);

            //    byte[] response = client.UploadValues(ConfigurationManager.AppSettings["server"] + "/Admin/UpdateCArticles.php", collection);

            //    result = System.Text.Encoding.UTF8.GetString(response);
            //}


        }

        public void DeleteArticle(string id)
        {
            string device = RandomString(10);
            string code = MD5Hash(device + "ncase8934f49909");
            string result = "";
            //using (WebClient client = new WebClient())
            //{

            //    var collection = new NameValueCollection();
            //    collection.Add("device", device);
            //    collection.Add("code", code);
            //    collection.Add("ID", id);
            //    collection.Add("servername", servername);
            //    byte[] response = client.UploadValues(ConfigurationManager.AppSettings["server"] + "/Admin/DeleteArticles.php", collection);

            //    result = System.Text.Encoding.UTF8.GetString(response);
            //}
            if (true)
            {
                string pathString = "~/images/panelimages";
                string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(result));
                System.IO.File.Delete(savedFileName);
            }


            //string imagename = result;
            //string savedFileName = Path.Combine(Server.MapPath(pathString), imagename);
            //System.IO.File.Delete(savedFileName);
        }
        public void DeleteCArticle(string id)
        {
            string device = RandomString(10);
            string code = MD5Hash(device + "ncase8934f49909");
            string result = "";
            //using (WebClient client = new WebClient())
            //{

            //    var collection = new NameValueCollection();
            //    collection.Add("device", device);
            //    collection.Add("code", code);
            //    collection.Add("ID", id);
            //    collection.Add("servername", servername);
            //    byte[] response = client.UploadValues(ConfigurationManager.AppSettings["server"] + "/Admin/DeleteCArticles.php", collection);

            //    result = System.Text.Encoding.UTF8.GetString(response);
            //}
            string pathString = "~/images/panelimages";
            string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(result));
            System.IO.File.Delete(savedFileName);

            //string imagename = result;
            //string savedFileName = Path.Combine(Server.MapPath(pathString), imagename);
            //System.IO.File.Delete(savedFileName);
        }
        public PartialViewResult getNewListArticle(string search, int cat,string tag)
        {
            List<article> log = getArticleList(cat, search,tag);
            return PartialView("/Views/Shared/admin/_ListOfArticles.cshtml", log);
        }
        public ActionResult GetImageForMCEEditContext(string srt, string image)
        {
            srt = srt.Replace("../images/panelimages/", "").Replace(image + ",", "");
            Session["imageListEdit"] = (Session["imageListEdit"] as string) + srt;
            string session = Session["imageListEdit"] as string;

            string finalsrt = image + ",";
            finalsrt = session.Length > 1 ? (finalsrt + session).Substring(0, (finalsrt + session).Length - 1) : image;
            return PartialView("/Views/Shared/admin/_imageForMCEEdit.cshtml", finalsrt);
        }
        [HttpPost]
        public ActionResult GetImageForMCEEditUpload(string filename, HttpPostedFileBase blob)
        {
            string pathString = "~/images/panelimages";
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }
            string tobeaddedtoimagename = RandomString(7);
            string savedFileName = Path.Combine(Server.MapPath(pathString), tobeaddedtoimagename + Path.GetExtension(blob.FileName));
            blob.SaveAs(savedFileName);
            Session["imageListEdit"] = Session["imageListEdit"] as string + tobeaddedtoimagename + Path.GetExtension(blob.FileName);
            string ss = Session["imageListEdit"] as string;
            // ss = ss + filename;
            ss = ss.Substring(0, ss.Length - 1);
            ViewModel.imageForEMCVM model = new ViewModel.imageForEMCVM();
            return PartialView("/Views/Shared/admin/_imageForMCEEdit.cshtml", ss);
            // return Content(tobeaddedtoimagename);
        }

        [HttpPost]
        public ActionResult GetImageForMCE(string filename, HttpPostedFileBase blob)
        {
            string pathString = "~/images/panelimages";
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }
            string tobeaddedtoimagename = RandomString(7);
            string savedFileName = Path.Combine(Server.MapPath(pathString), tobeaddedtoimagename + ".jpg");
            blob.SaveAs(savedFileName);
            Session["imageListAdd"] = Session["imageListAdd"] as string + tobeaddedtoimagename + ".jpg,";
            string ss = Session["imageListAdd"] as string;
            // ss = ss + filename;
            ss = ss.Substring(0, ss.Length - 1);
            ViewModel.imageForEMCVM model = new ViewModel.imageForEMCVM();
            model.data = ss;
            model.type = filename;

            return PartialView("/Views/Shared/admin/_imageForMCE.cshtml", model);
            // return Content(tobeaddedtoimagename);
        }

        public ActionResult DelImageForMCE(string filename, string type, string image)
        {
            string filestring = filename.Replace("../images/panelimages/", "");
            if (type == "edit")
            {

                if (image == "") // اصلی حذف شده 
                {
                    string ss = Session["imageListEdit"] as string;
                    ss = ss.Substring(0, ss.Length - 1);
                    List<string> list = ss.Split(',').ToList();
                    if (list.Count > 1)
                    {
                        list.Remove(filename);
                        string final = "";
                        foreach (var item in list)
                        {
                            final = final + item + ",";
                        }
                        Session["imageListEdit"] = final;

                    }
                    else
                    {
                        return Content("Error");
                    }
                }
                else
                {

                    if (filestring == image)
                    {
                        string ss = Session["imageListEdit"] as string;
                        if (ss == "")
                        {
                            return Content("Error");
                        }
                    }
                }

            }
            string pathString = "~/images/panelimages";
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }

            if (filename.Contains("images"))
            {
                string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(filename));
                filename = filename.Replace("..", "~");
                System.IO.File.Delete(Server.MapPath(filename));
                return Content("success");
            }
            else
            {
                string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(filename));
                System.IO.File.Delete(savedFileName);
                if (type == "edit")
                {
                    string ss = Session["imageListEdit"] as string;
                    ss = ss.Substring(0, ss.Length - 1);
                    List<string> list = ss.Split(',').ToList();
                    list.Remove(filename);
                    string final = "";
                    foreach (var item in list)
                    {
                        final = final + item + ",";
                    }
                    Session["imageListEdit"] = final;
                    if (filestring == image)
                    {
                        return Content("main");
                    }
                    else
                    {
                        return Content("notmain");
                    }

                }
                else
                {
                    string ss = Session["imageListAdd"] as string;
                    ss = ss.Replace(filename + ",", "").Replace(",,", ",");
                    string final = ss;
                    Session["imageListAdd"] = final;

                    //asb.ViewModel.imageForEMCVM model = new ViewModel.imageForEMCVM();
                    //model.data = final;
                    //model.type = "add";
                    //return PartialView("/Views/Shared/AdminShared/_imageForMCE.cshtml", model);
                    return Content("");
                }



            }


        }
        public void DelImageForMCEImage(string filename, string type)
        {
            if (type == "aboutus")
            {
                string srt = Request.Cookies["imageAboutUs"].Value;
                srt = srt.Replace(filename + ",", "").Replace(",,", ",");
                Response.Cookies["imageAboutUs"].Value = srt;

            }
            else if (type == "contactus")
            {
                string srt = Request.Cookies["imageContactUs"].Value;
                srt = srt.Replace(filename + ",", "").Replace(",,", ",");
                Response.Cookies["imageContactUs"].Value = srt;
            }
            string pathString = "~/images/panelimages";

            string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(filename));
            System.IO.File.Delete(savedFileName);


        }
        //public ActionResult comment()
        //{

        //    string device = RandomString(10);
        //    string code = MD5Hash(device + "ncase8934f49909");
        //    string result = "";
        //    using (WebClient client = new WebClient())
        //    {

        //        var collection = new NameValueCollection();
        //        collection.Add("device", device);
        //        collection.Add("code", code);
        //        collection.Add("servername", servername);
        //        //collection.Add("DayOfWeek", DayOfWeek);
        //        //collection.Add("TimeFrom", TimeFrom);
        //        //collection.Add("TimeTo", TimeTo);
        //        byte[] response =
        //        client.UploadValues(ConfigurationManager.AppSettings["server"] + "/Admin/GetComments.php?", collection);

        //        result = System.Text.Encoding.UTF8.GetString(response);
        //    }

        //    ViewModel.Comments log = JsonConvert.DeserializeObject<asb.ViewModel.Comments>(result);
        //    return View(log);
        //}
        //public void setAdminComment(string id, string comment)
        //{
        //    string result = "";
        //    string device = RandomString(10);
        //    string code = MD5Hash(device + "ncase8934f49909");
        //    using (WebClient client = new WebClient())
        //    {

        //        var collection = new NameValueCollection();
        //        collection.Add("servername", servername);
        //        collection.Add("device", device);
        //        collection.Add("code", code);
        //        collection.Add("id", id);
        //        collection.Add("comment", comment);

        //        byte[] response =
        //        client.UploadValues(ConfigurationManager.AppSettings["server"] + "/Admin/setAdminComment.php?", collection);

        //        result = System.Text.Encoding.UTF8.GetString(response);
        //    }
        //}
        //public void delCommnet(string id)
        //{
        //    string result = "";
        //    string device = RandomString(10);
        //    string code = MD5Hash(device + "ncase8934f49909");
        //    using (WebClient client = new WebClient())
        //    {

        //        var collection = new NameValueCollection();
        //        collection.Add("servername", servername);
        //        collection.Add("device", device);
        //        collection.Add("code", code);
        //        collection.Add("id", id);


        //        byte[] response =
        //        client.UploadValues(ConfigurationManager.AppSettings["server"] + "/Admin/delComment.php?", collection);

        //        result = System.Text.Encoding.UTF8.GetString(response);
        //    }
        //}
        //public void changeCommnetActive(string id, string value)
        //{
        //    string result = "";
        //    string device = RandomString(10);
        //    string code = MD5Hash(device + "ncase8934f49909");
        //    using (WebClient client = new WebClient())
        //    {

        //        var collection = new NameValueCollection();
        //        collection.Add("servername", servername);
        //        collection.Add("device", device);
        //        collection.Add("code", code);
        //        collection.Add("id", id);
        //        collection.Add("value", value);

        //        byte[] response =
        //        client.UploadValues(ConfigurationManager.AppSettings["server"] + "/Admin/ChangeCommentStatus.php?", collection);

        //        result = System.Text.Encoding.UTF8.GetString(response);
        //    }
        //}

        #endregion



        #region MediaDownload
        public ActionResult Media(string message)
        {
            //if (Session["LogedInUser2"] == null)
            //{

            //    return RedirectToAction("Index", "Admin");

            //}

            if (message == "1")
            {
                ViewBag.mess = "1";
            }
            
            List<vmedia> medialist = manager.getMediaList().Select(i => new vmedia
            {
                ID = i.ID,
                 title = i.title,
                  type = i.MediaType.title,

            }).ToList();
            List<string> typeList = new List<string>();
            foreach (var item in manager.getMediaTypeList().ToList())
            {
                typeList.Add(item.title);
            }

            manager.addUser(new user
            {
                fullname = "مهرداد منصوری",
                phone = "09194594505",
                email = "a@gmail.com"
            });

            List<string> userList = new List<string>();
            foreach (var item in manager.getUsers().ToList())
            {
                userList.Add(item.fullname);
            }


            mediaVM model = new mediaVM()
            {
                  mediaList = medialist,
                  typeList = typeList,
                  userList = userList

            };
            
            return View(model);
        }
        [HttpPost]
        public ActionResult Media(sliderforedit detail)
        {

            //if (Session["LogedInUser2"] == null)
            //{

            //    return RedirectToAction("Index", "Admin");

            //}

            
           

            string pathString = "~/images/panelimages";
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }
            try
            {
                List<string> medialist = new List<string>();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    string mediaName = RandomString(5);
                    HttpPostedFileBase hpf = Request.Files[i];
                    if (hpf.ContentLength == 0)
                        continue;
                    medialist.Add(mediaName + Path.GetExtension(hpf.FileName));
                    string savedFileName = Path.Combine(Server.MapPath(pathString), mediaName + Path.GetExtension(hpf.FileName));
                    hpf.SaveAs(savedFileName); // Save the file

                };
                int TypeID = 0;
                if (detail.dataType == "user")
                {
                    TypeID = manager.getUserID(detail.typeTitle);
                }
                else
                {
                    TypeID = manager.returnTypeID(detail.typeTitle);
                }

                foreach (var item in medialist)
                {
                    if (detail.dataType == "user")
                    {
                        manager.addUserMedia(new userMedia
                        {
                            title = item,
                            userID = TypeID
                        });
                    }
                    else
                    {
                        manager.addMedia(new media
                        {
                            title = item,
                            typeID = TypeID
                        });
                    }
                   

                }


                ViewBag.message = "محصول مورد نظر اضافه شد";

                return RedirectToAction("Media", "Admin", new { message = "1" });
            }
            catch 
            {
                ViewBag.message = "خطا! لطفا مجددا تلاش کنید";
                return RedirectToAction("Media", "Admin", new { message = "1" });
            }


        
}

        public ActionResult deleteimage(string id, string title,string type)
        {
            //if (Session["LogedInUser2"] == null)
            //{

            //    return RedirectToAction("Index", "Admin");

            //}
            string str = id;
            str = str.Substring(9, str.Length - 9);

            if (type == "user")
            {
                manager.delUserMedia(str);
            }
            else
            {
                manager.delMedia(str);
            }
          
            string pathString2 = "~/images/panelimages";

            string savedFileName = Path.Combine(Server.MapPath(pathString2), Path.GetFileName(title));
            if (System.IO.File.Exists(savedFileName))
            {
                System.IO.File.Delete(savedFileName);
            };

            return Content("1");
        }
        public ActionResult GetUserImageList(string username)
        {
            List<vmedia> usermedia = manager.getUserMediaList(manager.getUserID(username)).Select(i => new vmedia() {
                title = i.title,
                ID = i.ID,
            }).ToList();
            mediaVM model = new mediaVM()
            {
                mediaList = usermedia,
            };
            return PartialView("/Views/Shared/admin/_UpdateUserImage.cshtml", model);
        }
        #endregion
    }
}