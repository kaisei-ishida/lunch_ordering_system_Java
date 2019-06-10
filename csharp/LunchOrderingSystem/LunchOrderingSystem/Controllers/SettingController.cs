using LunchOrderingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace LunchOrderingSystem.Controllers
{
    public class SettingController : Controller
    {
        // GET: Setting
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Vendors,Administrators")]
        public ActionResult OpenCalendarSetting()
        {
            var orderCalendar = Request.QueryString.Get("orderCalendar");
            var isOpen = Request.QueryString.Get("isOpen");
            if (Request.QueryString.Get("orderCalendar") != null)
            {
                OpenCalendarSetting(int.Parse(orderCalendar), bool.Parse(isOpen));
            }

            using (var db = new DatabaseContext())
            {
                DateTime today = DateTime.Today;
                var monthHead = new DateTime(today.Year, today.Month, 1);
                var nextMonthHead = monthHead.AddMonths(1);
                var nextNextMonthHead = monthHead.AddMonths(2);

                //カレンダーの作成
                var offset = (int)monthHead.DayOfWeek - 1 % 7;
                var calendarList = Enumerable.Repeat<int?>(null, 42).ToList();
                for (int i = 0; i < DateTime.DaysInMonth(today.Year, today.Month); i++)
                {
                    calendarList[offset + i] = i + 1;
                }
                ViewBag.Calendar = calendarList;

                var offset2 = (int)nextMonthHead.DayOfWeek - 1 % 7;
                var calendarList2 = Enumerable.Repeat<int?>(null, 42).ToList();
                for (int i = 0; i < DateTime.DaysInMonth(nextMonthHead.Year, nextMonthHead.Month); i++)
                {
                    calendarList2[offset2 + i] = i + 1;
                }
                ViewBag.Calendar2 = calendarList2;

                //注文可能日の取得
                var openDayList = db.t_order_calendar
                    .Where(d => d.date >= monthHead && d.date < nextMonthHead)
                    .OrderBy(d => d.date)
                    .Select(d => d.is_open)
                    .ToList();
                ViewBag.OpenDayList = openDayList;

                var openDayList2 = db.t_order_calendar
                    .Where(d => d.date >= nextMonthHead && d.date < nextNextMonthHead)
                    .OrderBy(d => d.date)
                    .Select(d => d.is_open)
                    .ToList();
                ViewBag.OpenDayList2 = openDayList2;

                //カレンダーIDの取得
                var calendarIdList = db.t_order_calendar
                    .Where(d => d.date >= monthHead && d.date < nextMonthHead)
                    .OrderBy(d => d.date)
                    .Select(d => d.id)
                    .ToList();
                ViewBag.CalendarIdList = calendarIdList;

                var calendarIdList2 = db.t_order_calendar
                    .Where(d => d.date >= nextMonthHead && d.date < nextNextMonthHead)
                    .OrderBy(d => d.date)
                    .Select(d => d.id)
                    .ToList();
                ViewBag.CalendarIdList2 = calendarIdList2;

                ViewBag.MonthHead = monthHead;
                ViewBag.NextMonthHead = nextMonthHead;
            }
            return View();
        }

        [Authorize(Roles = "Vendors,Administrators")]
        private void OpenCalendarSetting(int orderCalendar, bool isOpen)
        {
            using (var db = new DatabaseContext())
            {
                var calendar = db.t_order_calendar
                    .Where(d => d.id == orderCalendar)
                    .SingleOrDefault();
                calendar.is_open = isOpen;
                db.SaveChanges();
            }
            return;
        }
    }
}