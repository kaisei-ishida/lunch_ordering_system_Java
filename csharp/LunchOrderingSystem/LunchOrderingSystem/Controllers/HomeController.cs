using LunchOrderingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LunchOrderingSystem.Controllers
{
    /// <summary>
    /// ホーム画面のコントローラです。
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 各ユーザ向けのホーム画面に遷移します。ログインしていなければログインを促します。
        /// </summary>
        /// <returns>ビュー</returns>
        [Authorize]
        public ActionResult Index()
        {
            var defineRoles = new List<string>
            {
                "Administrators",
                "Users",
                "Vendors",
                "PublicTerminals",
                "Collectors",
            };

            if (Request.IsAuthenticated)
            {
                var havingRoles = new List<string>();
                foreach(var r in defineRoles)
                {
                    if (User.IsInRole(r))
                    {
                        havingRoles.Add(r);
                    }
                }
                if (havingRoles.Count() == 1)
                {
                    return RedirectToAction(havingRoles.Last().Substring(0, havingRoles.Last().Count() - 1) + "Home");
                }
                ViewBag.HavingRoles = havingRoles;
            }
            
            return View();
        }

        /// <summary>
        /// 管理者用のホーム画面です。
        /// </summary>
        /// <returns>ビュー</returns>
        [Authorize(Roles = "Administrators")]
        public ActionResult AdministratorHome()
        {
            using (var db = new DatabaseContext())
            {
                var dateList = new List<string>();
                var orders = new List<int>();
                DateTime today = DateTime.Today;
                DateTime monthHead = new DateTime(today.Year, today.Month, 1);

                //カレンダーの作成
                for (int i = 0; i < 7; i++)
                {
                    var iTime = today.AddDays(-i);
                    var diTime = today.AddDays(-i + 1);

                    var iOrders = db.t_order
                    .Where(u => u.ordered_at > iTime && u.ordered_at < diTime)
                    .Count();

                    orders.Add(iOrders);
                    dateList.Add(iTime.ToString("MM月dd日"));
                }

                orders.Reverse();
                dateList.Reverse();
                ViewBag.Orders = orders;
                ViewBag.DateList = dateList;

                //今月の支払額計算
                int totalPrice = 0;
                var allTransactions = db.t_order
                    .Include(o => o.m_user)
                    .Where(o => o.t_order_calendar.date <= today && o.t_order_calendar.date >= monthHead)
                    .ToList();
                foreach (var item in allTransactions)
                {
                    totalPrice += item.m_item_category.price;
                }
                ViewBag.TotalPrice = totalPrice;

                return View(allTransactions.Where(t => t.t_order_calendar.date == today).ToList());
            }
        }

        /// <summary>
        /// 一般ユーザ向けのホーム画面です。
        /// </summary>
        /// <returns>ビュー</returns>
        [Authorize(Roles = "Users")]
        public ActionResult UserHome()
        {
            using (var db = new DatabaseContext())
            {
                var today = DateTime.Today;
                var monthHead = new DateTime(today.Year, today.Month, 1);

                //ユーザIDの取得
                var user = db.m_user
                    .Find(int.Parse(User.Identity.Name));
                ViewBag.UserName = user.name;
                ViewBag.UserId = user.id;

                //注文履歴の取得
                var monthlyOrders = db.t_order
                    .Include(o => o.m_item_category)
                    .Include(o => o.t_order_calendar)
                    .Where(u => u.user_id.Equals(user.id))
                    .Where(t => t.t_order_calendar.date <= today && t.t_order_calendar.date >= monthHead)
                    .ToList();

                //今月の支払額計算
                int totalPrice = 0;
                foreach(var item in monthlyOrders)
                {
                    totalPrice += item.m_item_category.price;
                }
                ViewBag.TotalPrice = totalPrice;

                var itemCount = monthlyOrders
                    .GroupBy(m => m.m_item_category)
                    .Select(m => new
                    {
                        Name = m.Key.name,
                        Price = m.Key.price,
                        Count = m.Count()
                    })
                    .ToList();
                ViewBag.ItemCount = itemCount;

                //本日注文済みか
                var todayTransaction = db.t_order
                    .Where(u => u.user_id.Equals(user.id) && u.ordered_at > today)
                    .FirstOrDefault();

                if (todayTransaction != null)
                {
                    ViewBag.Order = todayTransaction.is_receipted;
                }

                //受付が終了しているか取得
                var closeUser = db.t_order_close
                    .Where(u => u.closed_at > today)
                    .SingleOrDefault();

                if (closeUser != null) ViewBag.IsOpen = false;
                else ViewBag.IsOpen = true;

                //本日のカレンダーIDを取得
                var todayCalendar = db.t_order_calendar
                    .Where(c => c.date == today)
                    .SingleOrDefault();
                ViewBag.IsOpenDay = false;
                if (todayCalendar != null)
                {
                    ViewBag.IsOpenDay = todayCalendar.is_open;
                    ViewBag.TodayCalendarId = todayCalendar.id;
                }

                //商品カテゴリを取得
                var itemCategory = db.m_item_category
                    .Where(c => c.is_exist)
                    .ToList();
                ViewBag.ItemCategory = itemCategory;

                //カレンダーListの作成
                var offset = (int)monthHead.DayOfWeek - 1 % 7;
                var calendarList = Enumerable.Repeat<int?>(null, 42).ToList();
                for (int i = 0; i < DateTime.DaysInMonth(today.Year, today.Month); i++)
                {
                    calendarList[offset + i] = i + 1;
                }
                ViewBag.Calendar = calendarList;

                return View(monthlyOrders);
            }
        }

        /// <summary>
        /// 注文者向けのホーム画面です。
        /// </summary>
        /// <returns>ビュー</returns>
        [Authorize(Roles = "Vendors")]
        public ActionResult VendorHome()
        {
            using (var db = new DatabaseContext())
            {
                DateTime today = DateTime.Today;

                var user = db.t_order_close
                    .Where(u => u.closed_at > today)
                    .SingleOrDefault();

                if(user != null)
                {
                    ViewBag.CloseUserName = user.m_user.name;
                    ViewBag.ClosedAt = user.closed_at;
                }

                //カレンダーテーブルの作成
                for(var i = 0; i < 3; i++)
                {
                    var iDate = today.AddMonths(i);
                    CreateCalendarTable(iDate.Year, iDate.Month);
                }
                

                //本日のカレンダーIDを取得
                var todayCalendar = db.t_order_calendar
                    .Where(c => c.date == today)
                    .SingleOrDefault();
                ViewBag.TodayCalendarId = todayCalendar.id;

                //本日のオーダー数を取得
                var todayOrders = db.t_order.Where(o => o.order_calendar_id == todayCalendar.id).Count();
                ViewBag.TodayOrders = todayOrders;

                return View();
            }
        }

        /// <summary>
        /// 共有端末向けのホーム画面です。
        /// </summary>
        /// <returns>ビュー</returns>
        [Authorize(Roles = "PublicTerminals")]
        public ActionResult PublicTerminalHome()
        {
            using (var db = new DatabaseContext())
            {
                //本日のカレンダーIDを取得
                DateTime today = DateTime.Today;
                var todayCalendar = db.t_order_calendar
                    .Where(c => c.date == today)
                    .SingleOrDefault();
                ViewBag.IsOpenDay = false;
                if (todayCalendar != null)
                {
                    ViewBag.IsOpenDay = todayCalendar.is_open;
                    ViewBag.TodayCalendarId = todayCalendar.id;
                }

                //受付しているか取得
                //注文履歴を取得
                var closeUser = db.t_order_close
                    .Where(u => u.order_calendar_id == todayCalendar.id)
                    .SingleOrDefault();
                if (closeUser != null) ViewBag.IsOpen = false;
                else ViewBag.IsOpen = true;
                var orders = db.t_order
                        .Where(o => o.order_calendar_id == todayCalendar.id)
                        .Include(o => o.m_user).Include(o => o.m_item_category)
                        .ToList();
                ViewBag.OrderList = orders;
            }
            return View();
        }

        /// <summary>
        /// 代金徴収者向けのホーム画面です。
        /// </summary>
        /// <returns>ビュー</returns>
        [Authorize(Roles = "Collectors")]
        public ActionResult CollectorHome()
        {
            return RedirectToAction("../Billing");
        }

        /// <summary>
        /// カレンダーテーブルが存在しなければ作成します
        /// </summary>
        /// <param name="year">西暦</param>
        /// <param name="month">1~12</param>
        private void CreateCalendarTable(int year, int month)
        {
            using (var db = new DatabaseContext())
            {
                var monthHead = new DateTime(year, month, 1);
                var isExist = db.t_order_calendar
                    .Where(c => c.date == monthHead)
                    .Any();
                if (!isExist)
                {
                    for(var iDate = monthHead; iDate.Month == month; iDate = iDate.AddDays(1))
                    {
                        var dayTable = new t_order_calendar { date = iDate, is_open = true };
                        db.t_order_calendar.Add(dayTable);
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}