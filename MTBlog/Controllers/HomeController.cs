using MTBlog.Models;
using MTBlog.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MTBlog.Controllers
{
    public class HomeController : Controller
    {
        MyBlogEntities db = new MyBlogEntities();
        // GET: Home
        public ActionResult Index()
        {
            List<Makale> makales = db.Makales.OrderByDescending(h => h.MakaleId).Take(5).ToList();
            return View(makales);
        }

        public ActionResult Kategoriler()
        {
            List<Kategori> kategoris = db.Kategoris.OrderBy(h => h.KategoriId).ToList();
            return PartialView(kategoris);
        }
        public ActionResult KategoriMakale(Guid? Id)
        {
            Kategori kategoris = db.Kategoris.Where(h => h.KategoriId == Id).SingleOrDefault();
            if (kategoris == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Kategori = kategoris.KategoriAdi;
            List<Makale> makales = db.Makales.Where(h => h.KategoriId == Id).ToList();
            if (makales.Count == 0)
            {
                ViewBag.MakaleYok = "Bu kategoride henüz makale yayınlanmamış.";

            }
            return View(makales);
        }
        public ActionResult MakaleDetay(Guid? Id)
        {
            Makale makales = db.Makales.Where(h => h.MakaleId == Id).SingleOrDefault();
            if (makales == null)
            {
                return RedirectToAction("Index");
            }

            return View(makales);
        }
        public PartialViewResult Yorumlar(Guid? makaleId)
        {
            List<Yorum> yorums = db.Yorums.Where(h => h.MakaleId == makaleId).ToList();
            return PartialView(yorums);
        }
        [HttpPost]
        public ActionResult BlogAra(string deger)
        {
            List<Makale> makales = db.Makales.Where(h => h.Icerik.Contains(deger) || h.Baslik.Contains(deger)).ToList();
            if (makales.Count == 0)
            {
                ViewBag.kayityok = "Aradığınız Değere göre makale bulunamadı";
            }
            else
            {
                ViewBag.kayityok = "Aradığınız Değere göre " + makales.Count + "  makale bulundu";

            }
            ViewBag.deger = deger;
            return View(makales);
        }

        public ActionResult OturumAç(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public ActionResult OturumAç(UyeModel model, string returnUrl)
        {
            Üye üye = db.Üye.Where(h => h.KullaniciAdi == model.KullaniciAdi && h.Sifre == model.Sifre).SingleOrDefault();
            if (üye != null)
            {
                Session["UyeOturum"] = true;
                Session["UyeId"] = üye.UyeId;
                Session["UyeAdi"] = üye.KullaniciAdi;
                Session["UyeSifre"] = üye.Sifre;
                Session["UyeAdmin"] = üye.UyeAdmin;
                Session["UyeFoto"] = üye.Foto;
                if (returnUrl == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            else
            {
                ViewBag.HataMesajı = "Kullanıcı Adı veya Paralosı Geçersizdir!";
                return View();

            }
        }
        public ActionResult OturumKapat(string returnUrl)
        {
            Session.Abandon();
            return Redirect(returnUrl);
        }
        public ActionResult ÜyeOl()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ÜyeOl(UyeModel model)
        {
            var sn = db.Üye.Where(h => h.KullaniciAdi == model.KullaniciAdi).ToList();
            if (sn.Count() > 0)
            {
                ViewBag.KayıtHata = "Girilen Kullanıcı Adı Kayıtlıdır";
                return View();

            }
            Üye yeniüye = new Üye();
            if (model.Foto != null && model.Foto.ContentLength > 0)
            {
                string uzanti = Path.GetExtension(model.Foto.FileName).ToLower();
                string dosya = Guid.NewGuid().ToString();
                if (uzanti != ".jpg" && uzanti != ".jpeg" && uzanti != ".png")
                {
                    ModelState.AddModelError("Foto", "Dosya uzantısı JPG,JPEG veya PNG Olmalıdır!");
                    return View();
                }
                string dosyaAdi = dosya + uzanti;
                model.Foto.SaveAs(Server.MapPath("~/Content/ÜyeFoto/" + dosyaAdi));
                yeniüye.AdSoyad = model.AdSoyad;
                yeniüye.Email = model.Email;
                yeniüye.KullaniciAdi = model.KullaniciAdi;
                yeniüye.Sifre = model.Sifre;
                yeniüye.Foto = dosyaAdi;
                yeniüye.UyeId = new Guid();
            }
            db.Üye.Add(yeniüye);
            db.SaveChanges();

            Üye üye = db.Üye.OrderByDescending(a => a.UyeId).FirstOrDefault();
            Session["UyeOturum"] = true;
            Session["UyeId"] = üye.UyeId;
            Session["UyeAdi"] = üye.KullaniciAdi;
            Session["UyeSifre"] = üye.Sifre;
            Session["UyeAdmin"] = üye.UyeAdmin;
            Session[",0"] = üye.Foto;

            return RedirectToAction("Index");
        }
    }
}