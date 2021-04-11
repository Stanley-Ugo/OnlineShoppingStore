using Newtonsoft.Json;
using OnlineShoppingStore.DAL;
using OnlineShoppingStore.Models;
using OnlineShoppingStore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShoppingStore.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            var categories = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecords();

            foreach (var category in categories)
            {
                list.Add(new SelectListItem { Value = category.CategoryId.ToString(), Text = category.CategoryName});
            }

            return list;
        }
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Categories()
        {
            List<Tbl_Category> allCategories = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecordsIQueryable().Where(i => i.IsDeleted == false).ToList();

            return View(allCategories);
        }

        public ActionResult AddCategory()
        {

            return UpdateCategory(0);
        }

        public ActionResult UpdateCategory(int categoryId)
        {
            CategoryDetail cd;

                if (categoryId != null)
                {
                    cd = JsonConvert.DeserializeObject<CategoryDetail>(JsonConvert.SerializeObject(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstOrDefault(categoryId)));
                }
                else
                {
                    cd = new CategoryDetail();
                }

            return View("UpdateCategory", cd);
        }

        public ActionResult Product()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetProducts());
        }

        public ActionResult CategoryEdit(int catId)
        {
            
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstOrDefault(catId));
        }

        [HttpPost]
        public ActionResult CategoryEdit(Tbl_Category tbl_Category)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Update(tbl_Category);
            return RedirectToAction("Categories");
        }

        public ActionResult ProductEdit(int productId)
        {
            ViewBag.CategoryList = GetCategory();
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstOrDefault(productId));
        }

        [HttpPost]
        public ActionResult ProductEdit(Tbl_Product tbl_Product, HttpPostedFileBase file)
        {
            string pic = null;

            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                file.SaveAs(path);
            }

            tbl_Product.ProductImage = file != null ? pic : tbl_Product.ProductImage;

            tbl_Product.ModiifiedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Update(tbl_Product);
            return RedirectToAction("Product");
        }

        public ActionResult ProductAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProductAdd(Tbl_Product tbl_Product, HttpPostedFileBase file)
        {
            string pic = null;

            if(file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                file.SaveAs(path);
            }

            tbl_Product.ProductImage = pic;

            tbl_Product.CreatedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Add(tbl_Product);
            return RedirectToAction("Product");
        }
    }
}