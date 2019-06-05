using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LunchOrderingSystem.Models;

namespace LunchOrderingSystem.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class ItemCategoryController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: ItemCategory
        public ActionResult Index()
        {
            return View(db.m_item_category.Where(i => i.is_exist).ToList());
        }

        // GET: ItemCategory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            m_item_category m_item_category = db.m_item_category.Find(id);
            if (m_item_category == null)
            {
                return HttpNotFound();
            }
            return View(m_item_category);
        }

        // GET: ItemCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemCategory/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,price")] m_item_category m_item_category)
        {
            if (ModelState.IsValid)
            {
                m_item_category.is_exist = true;
                db.m_item_category.Add(m_item_category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(m_item_category);
        }

        // GET: ItemCategory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            m_item_category m_item_category = db.m_item_category.Find(id);
            if (m_item_category == null)
            {
                return HttpNotFound();
            }
            return View(m_item_category);
        }

        // POST: ItemCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            m_item_category m_item_category = db.m_item_category.Find(id);
            m_item_category.is_exist = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
