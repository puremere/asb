using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asb.Models
{
    public class Context : DbContext
    {
        public Context() : base("asb1")
        {

        }
        public DbSet<articleCat> articleCats { get; set; }
        public DbSet<article> articles { get; set; }
        public DbSet<media> medias { get; set; }
        public DbSet<mediaType> mediaTypes { get; set; }
        public DbSet<userMedia> userMedias { get; set; }
        public DbSet<user> users { get; set; }
    }
    public class articleCat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int articleCatID { get; set; }
        public string title { get; set; }
        public string titleEn { get; set; }
        public string image { get; set; }

        public virtual ICollection<article> Articles { get; set; }

    }
    public class article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        public int articleCatID { get; set; }
        public virtual articleCat articleCat { get; set; }
    }

    public class media
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string title { get; set; }
        public int typeID { get; set; }

        public virtual mediaType MediaType { get; set; }


    }
    public class mediaType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int typeID { get; set; }
        public string  title { get; set; }
        public ICollection<media> medias { get; set; }
    }

    public class userMedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string title { get; set; }
        public int userID { get; set; }
        public virtual user user { get; set; }


    }
    public class user
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userID { get; set; }
        public string fullname { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public virtual ICollection<userMedia> usermedias {get; set;}

    }


}