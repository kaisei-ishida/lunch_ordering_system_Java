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
    public class UserController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: User
        public ActionResult Index()
        {
            return View(db.m_user.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            m_user m_user = db.m_user.Find(id);
            if (m_user == null)
            {
                return HttpNotFound();
            }
            return View(m_user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,login_id,login_password,role,name")] m_user m_user)
        {
            if (ModelState.IsValid)
            {
                m_user.login_password = BCrypt.Net.BCrypt.HashPassword(m_user.login_password);
                db.m_user.Add(m_user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(m_user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            m_user m_user = db.m_user.Find(id);
            if (m_user == null)
            {
                return HttpNotFound();
            }
            return View(m_user);
        }

        // POST: User/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,login_id,name")] m_user m_user)
        {
            if (ModelState.IsValid)
            {
                m_user u = db.m_user.Find(m_user.id);
                if (u != null)
                {
                    u.login_id = m_user.login_id;
                    u.name = m_user.name;
                    db.SaveChanges();
                }
                
                return RedirectToAction("Index");
            }
            return View(m_user);
        }

        // GET: User/EditPassword/5
        public ActionResult EditPassword(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            m_user m_user = db.m_user.Find(id);
            if (m_user == null)
            {
                return HttpNotFound();
            }
            return View(m_user);
        }

        // POST: User/EditPassword/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPassword([Bind(Include = "id,login_password")] m_user m_user)
        {
            if (ModelState.IsValid)
            {
                m_user u = db.m_user.Find(m_user.id);
                if (u != null)
                {
                    u.login_password = BCrypt.Net.BCrypt.HashPassword(m_user.login_password);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View(m_user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            m_user m_user = db.m_user.Find(id);
            if (m_user == null)
            {
                return HttpNotFound();
            }
            return View(m_user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            m_user m_user = db.m_user.Find(id);
            db.m_user.Remove(m_user);
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
