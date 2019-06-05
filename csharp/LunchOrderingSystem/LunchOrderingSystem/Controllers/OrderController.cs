using LunchOrderingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LunchOrderingSystem.Controllers
{
    public class OrderController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Users")]
        public ActionResult Order(int orderCalendar, int category)
        {
            using (var db = new DatabaseContext())
            {
                //ユーザ情報取得
                var user = db.m_user
                    .Where(u => u.login_id.Equals(User.Identity.Name))
                    .FirstOrDefault();

                //商品カテゴリが存在しているかどうか取得
                var isExistCategory = db.m_item_category
                    .Where(c => c.id == category)
                    .Any();

                //営業日であるかどうか取得
                var isBusinessDay = db.t_order_calendar
                    .Where(c => c.is_open == true)
                    .Any();

                //受付終了しているかどうか取得
                var isClosed = db.t_order_close
                    .Where(o => o.order_calendar_id == orderCalendar)
                    .Any();

                //営業日 & 受付終了していない & カテゴリが存在している とき注文を反映する
                if (isBusinessDay && !isClosed && isExistCategory)
                {
                    var item = new t_order
                    {
                        user_id = user.id,
                        order_calendar_id = orderCalendar,
                        item_category_id = category
                    };
                    db.t_order.Add(item);
                    db.SaveChanges();
                }
            }
            
            Response.Redirect("/", true);
            return View();
        }

        [Authorize(Roles = "Users")]
        public ActionResult Cancel(int orderCalendar)
        {
            using (var db = new DatabaseContext())
            {
                //ユーザ情報取得
                var user = db.m_user
                    .Where(u => u.login_id.Equals(User.Identity.Name))
                    .FirstOrDefault();

                //受付終了しているかどうか取得
                var isClosed = db.t_order_close
                    .Where(o => o.order_calendar_id == orderCalendar)
                    .Any();

                //受付が終了していなかったら注文を取り消す
                if (!isClosed)
                {
                    var items = db.t_order
                        .Where(u => u.user_id == user.id && u.order_calendar_id == orderCalendar);
                    db.t_order.RemoveRange(items);
                    db.SaveChanges();
                } 
            }

            Response.Redirect("/", true);
            return View();
        }

        [Authorize(Roles = "Users")]
        public ActionResult Receipt(int orderCalendar)
        {
            using (var db = new DatabaseContext())
            {
                //ユーザ情報取得
                var user = db.m_user
                    .Where(u => u.login_id.Equals(User.Identity.Name))
                    .FirstOrDefault();

                //注文情報取得
                var order = db.t_order
                    .Where(o => o.user_id == user.id && o.order_calendar_id == orderCalendar)
                    .FirstOrDefault();

                //注文していたら受け取りフラグを立てる
                if (order != null)
                {
                    order.is_receipted = true;
                    db.SaveChanges();
                }
            }

            Response.Redirect("/", true);
            return View();
        }

        [Authorize(Roles = "PublicTerminals")]
        public ActionResult SelectedReceipt(int orderId)
        {
            using (var db = new DatabaseContext())
            {
                //注文情報取得
                var order = db.t_order
                    .Find(orderId);

                //注文していたら受け取りフラグを立てる
                if (order != null)
                {
                    order.is_receipted = !order.is_receipted;
                    db.SaveChanges();
                }
            }

            Response.Redirect("/", true);
            return View();
        }

        [Authorize(Roles = "Vendors")]
        public ActionResult Close(int orderCalendar)
        {
            using (var db = new DatabaseContext())
            {
                //ユーザ情報取得
                var user = db.m_user
                        .Where(u => u.login_id.Equals(User.Identity.Name))
                        .FirstOrDefault();

                //既に受付終了しているかどうか取得
                var isClosed = db.t_order_close
                    .Where(o => o.order_calendar_id == orderCalendar)
                    .Any();

                //受付終了していなかったら終了させる
                if(!isClosed)
                {
                    var item = new t_order_close { order_calendar_id = orderCalendar, user_id = user.id };
                    db.t_order_close.Add(item);
                    db.SaveChanges();
                } 
            }

            Response.Redirect("/", true);
            return View();
        }

        [Authorize(Roles = "Vendors")]
        public ActionResult Open(int orderCalendar)
        {
            using (var db = new DatabaseContext())
            {
                //受付終了していたらそれを取り消す
                var items = db.t_order_close
                    .Where(o => o.order_calendar_id == orderCalendar);
                db.t_order_close.RemoveRange(items);
                db.SaveChanges();
            }

            Response.Redirect("/", true);
            return View();
        }
    }
}