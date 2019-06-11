using LunchOrderingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LunchOrderingSystem.Controllers
{
    public class BillingController : Controller
    {
        // GET: Billing
        [Authorize(Roles = "Collectors")]
        public ActionResult Index()
        {
            var today = DateTime.Today;
            var lastMonth = today.AddMonths(-1);
    
            return Index(lastMonth.Year, lastMonth.Month);
        }

        [Authorize(Roles = "Collectors")]
        [HttpPost]
        public ActionResult Index(int year, int month)
        {
            var today = DateTime.Today;
            var currentMonthHead = new DateTime(today.Year, today.Month, 1);
            var selectedMonthHead = new DateTime(year, month, 1);
            var selectedNextMonthHead = selectedMonthHead.AddMonths(1);
            if (selectedMonthHead < currentMonthHead)
            {
                using (var db = new DatabaseContext())
                {
                    var billings = db.t_billing
                        .Where(b => b.month == selectedMonthHead)
                        .ToList();

                    //存在しなければ作る
                    if(!billings.Any())
                    {
                        //ユーザリスト
                        var users = db.m_user
                            .Where(u => u.role == "Users")
                            .ToList();
                        foreach(var user in users)
                        {
                            //ユーザ別のmonth月の請求額を計算
                            int charge = 0;
                            var orders = db.t_order
                                .Where(o => o.t_order_calendar.date >= selectedMonthHead && o.t_order_calendar.date < selectedNextMonthHead && o.user_id == user.id);

                            //挿入するテーブルを作成・追加
                            if (orders.Any()) {
                                charge = orders.Sum(o => o.m_item_category.price);
                                
                                var table = new t_billing
                                {
                                    month = selectedMonthHead,
                                    user_id = user.id,
                                    charge = charge
                                };
                                db.t_billing.Add(table);
                            }
                        }
                        //クエリ発行
                        db.SaveChanges();
                    }

                    var billing = db.t_billing
                        .Include(b => b.t_billing_close)
                        .Where(b => b.month == selectedMonthHead && b.charge > 0)
                        .ToList();
                    var userList = db.m_user
                        .ToDictionary(u => u.id);
                    ViewBag.BillingList = billing;
                    ViewBag.UserList = userList;
                    ViewBag.SelectedMonthHead = selectedMonthHead;
                }
                return View("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Collectors")]
        public ActionResult Pay(int id, bool isPay, int year, int month)
        {
            using (var db = new DatabaseContext())
            {
                var billingTable = db.t_billing
                    .Find(id);
                if (billingTable != null)
                {
                    //ユーザ情報取得
                    var userId = int.Parse(User.Identity.Name);

                    if (isPay)
                    {
                        //支払い承認
                        var table = new t_billing_close
                        {
                            billing_id = id,
                            user_id = userId
                        };
                        db.t_billing_close.Add(table);
                        db.SaveChanges();
                    }
                    else
                    {
                        //承認取り消し
                        var table = db.t_billing_close
                            .Find(id);
                        db.t_billing_close.Remove(table);
                        db.SaveChanges();
                    }
                }
                
            }
            return Index(year, month);
        }

        [Authorize(Roles = "Users")]
        public ActionResult ShowBilling()
        {
            var today = DateTime.Today;
            var lastMonth = today.AddMonths(-1);

            return ShowBilling(lastMonth.Year, lastMonth.Month);
        }

        [Authorize(Roles = "Users")]
        [HttpPost]
        public ActionResult ShowBilling(int year, int month)
        {
            using (var db = new DatabaseContext())
            {
                var selectedMonthHead = new DateTime(year, month, 1);
                var selectedMonthHeadNext = selectedMonthHead.AddMonths(1);

                //ユーザ情報取得
                var userId = int.Parse(User.Identity.Name);

                //請求が発生していたら
                if(db.t_billing.Where(b => b.user_id == userId
                && b.month == selectedMonthHead).Any())
                {
                    //支払い情報を取得
                    var billing = db.t_billing
                        .Include(b => b.m_user)
                        .Where(b => b.month == selectedMonthHead && b.user_id == userId)
                        .SingleOrDefault();
                    ViewBag.Billing = billing;

                    //支払い記録を取得
                    var billingClose = db.t_billing_close
                        .Where(b => b.billing_id == billing.id)
                        .SingleOrDefault();
                    ViewBag.BillingClose = billingClose;

                    //注文リストを取得
                    var monthlyOrders = db.t_order
                    .Include(o => o.m_item_category)
                    .Include(o => o.t_order_calendar)
                    .Where(u => u.user_id.Equals(userId))
                    .Where(t => t.t_order_calendar.date >= selectedMonthHead && t.t_order_calendar.date < selectedMonthHeadNext)
                    .ToList();
                    return View(monthlyOrders);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            
        }
    }
}