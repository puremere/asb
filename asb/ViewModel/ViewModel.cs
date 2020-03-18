using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using asb.Models;

namespace asb.ViewModel
{
    public class ViewModel
    {
    }
    public class AdminBlogVM
    {

        public List<articleVM> Articlelist { get; set; }
        public List<articleCatVM> Catlist { get; set; }
    }
    public class articleVM
    {
        public int arcticleID { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string shortDesc { get; set; }
        public string image { get; set; }
        public string writer { get; set; }
        public string hashtags { get; set; }
        public string link { get; set; }
        public int view { get; set; }
        public DateTime? date { get; set; }
        public string catname { get; set; }

        public int articleCatID { get; set; }
    }
    public class articleCatVM
    {
        public int articleCatID { get; set; }
        public string title { get; set; }
        public string titleEn { get; set; }
        public string image { get; set; }
    }

    public class imageForEMCVM
    {
        public string data { get; set; }
        public string type { get; set; }
    }
    public class newArcticelVM
    {
        public string image { get; set; }
        public int catList { get; set; }
        public string title { get; set; }
        [AllowHtml]
        public string description { get; set; }
        public string tag { get; set; }
    }
    public class updateArticleVM
    {
        public string IDupdate { get; set; }
        public int catListupdate { get; set; }
        public string titleupdate { get; set; }
        [AllowHtml]
        public string descriptionupdate { get; set; }
        public string tagupdate { get; set; }
    }
    public class dbRespose
    {
        public int status { get; set; }
        public string data { get; set; }
    }

    public class vmedia
    {
       public int ID { get; set; }
        public string title { get; set; }
        public string type { get; set; }


    }
    public class VtypeList
    {
        public int ID { get; set; }
        public string title { get; set; }
    }


    public class mediaVM
    {
        public List<vmedia> mediaList { get; set; }
        public  List<string> typeList { get; set; }
        public List<string> userList { get; set; }

    }
    public class sliderforedit
    {

        public List<HttpPostedFileBase> file { get; set; }
        public string typeTitle { get; set; }
        public string dataType { get; set; }

    }

    public class HomeIndexVM
    {
        public List<vmedia> mediaList { get; set; }
        public List<VtypeList> typeList { get; set; }
    }
    public class HomeBlogVM
    {
        public string imageTitle { get; set; }
        public List<article> articleList { get; set; }
    }
    public class blogDetailVM
    {
        public article article  { get; set; }
        public article nextArticle { get; set; }
        public article previousArticle { get; set; }
    }

    public class profileVM
    {
        public List<userMedia> mediaList { get; set; }
        public user user { get; set; }
    }
}