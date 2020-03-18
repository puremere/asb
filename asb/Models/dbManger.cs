using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using asb.Models;
using asb.ViewModel;


namespace asb.Models
{
   
    public class dbManger
    {

        #region article
        Context context = new Context();
        public dbRespose addArticle (article model)
        {
            dbRespose response = new dbRespose();
            context.articles.Add(model);
            context.SaveChanges();
            response.status = 200;
            return response;
        }
        public dbRespose updataArtice(article model)
        {
            dbRespose response = new dbRespose();
            try
            {
                
                article selectedArticle  = getArticle(model.arcticleID);
                selectedArticle.hashtags = model.hashtags;
                if (model.image != "")
                {
                    selectedArticle.image = model.image;
                }
                selectedArticle.title = model.title;
                selectedArticle.content = model.content;
                context.SaveChanges();
                response.status = 200;

            }
            catch (Exception)
            {
                response.status = 400;
            }
            
            return response;
        }
        public dbRespose addArticleCat(articleCat model) {
            dbRespose MR = new dbRespose();
            try
            {
                context.articleCats.Add(model);
                context.SaveChanges();
                MR.status = 200;


            }
            catch (Exception err)
            {

                MR.status = 400;
            }
            return MR;
            

        }
        public dbRespose updataCatArtice(articleCat model)
        {
            dbRespose response = new dbRespose();
       
            try
            {

                articleCat selectedArticle = getCatArtice(model.articleCatID);
                if (model.image != "") 
                     selectedArticle.image = model.image;
                selectedArticle.title = model.title;
                context.SaveChanges();
                response.status = 200;

            }
            catch (Exception)
            {
                response.status = 400;
            }

            return response;
        }
        public articleCat getCatArtice(int id)
        {
            return context.articleCats.Where(x => x.articleCatID == id).FirstOrDefault();
        }
        public IQueryable<articleCat> getCatArticleList()
        {
            return context.articleCats.AsQueryable();
        }
        public article getArticle(int id)
        {
            return context.articles.Where(x => x.arcticleID == id).FirstOrDefault();
        }
        public IQueryable<article> getArticleList(int id, string search, string tag)
        {
            IQueryable<article> q = context.articles.AsQueryable();
            if( id != 0)
            {
                q = q.Where(x => x.articleCatID == id).AsQueryable();
               
            }
             if(search != "")
            {
                q = q.Where(x => x.title.Contains(search)).AsQueryable();
            }
            return q;
        }
        #endregion


        #region Mainmedia
        public IQueryable<media> getMediaList()
        {
            var list = context.medias.AsQueryable();
            return list;
        }
        public IQueryable<mediaType> getMediaTypeList()
        {
            var list = context.mediaTypes.AsQueryable();
            List<mediaType> lst = list.ToList();
            return list;
        }
        public int returnTypeID(string title)
        {
           
            mediaType tp = context.mediaTypes.SingleOrDefault(x => x.title == title);
            
            if (tp != null)
            {
                return tp.typeID;
            }
            else
            {
                addMediaType(new mediaType {
                      title = title
                });
                return context.mediaTypes.SingleOrDefault(x => x.title == title).typeID;
            }
           
       
        }
        public void addMedia (media model)
        {
            try
            {
                context.medias.Add(model);
                context.SaveChanges();
            }
            catch (Exception error)
            {
                
            }
           
        }
        public void addMediaType(mediaType model)
        {
            context.mediaTypes.Add(model);
            context.SaveChanges();
        }
        public void delMedia(string id)
        {
            int mediaID = Int32.Parse(id);
            context.medias.Remove(context.medias.SingleOrDefault(x => x.ID == mediaID));
            context.SaveChanges();
        }
        #endregion

        #region Usermedia
        public IQueryable<userMedia> getUserMediaList(int userID)
        {
            List<userMedia> lst = context.userMedias.ToList();
            return context.userMedias.Where(x => x.userID == userID).AsQueryable();
        }
        public int getUserID (string name)
        {
            try
            {
                user myuser = context.users.Where(x => x.fullname == name).First();
                if (myuser != null)
                {
                    return myuser.userID;
                }
                return 0;
            }
            catch (Exception error)
            {
                return 0;
            }
        }
        public user getUserByID (int ID)
        {
            return context.users.SingleOrDefault(x => x.userID == ID);
        }
        public IQueryable<user> getUsers() {
            return context.users.AsQueryable();
        }
        public void addUser(user model)
        {
            try
            {
                context.users.Add(model);
                context.SaveChanges();
            }
            catch (Exception)
            {

              
            }
          
        }

        public void addUserMedia(userMedia model)
        {
            try
            {
                context.userMedias.Add(model);
                context.SaveChanges();
            }
            catch (Exception error)
            {

            }

        }
        public void delUserMedia(string id)
        {
            int mediaID = Int32.Parse(id);
            context.userMedias.Remove(context.userMedias.SingleOrDefault(x => x.ID == mediaID));
            context.SaveChanges();
        }
        
        #endregion
    }
}