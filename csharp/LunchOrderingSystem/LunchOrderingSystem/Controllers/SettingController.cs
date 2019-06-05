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
                //カレンダーListの作成
                DateTime today = DateTime.Today;
                var monthHead = new DateTime(today.Year, today.Month, 1);
                var nextMonthHead =monthHead.AddMonths(1);
                var offset = (int)monthHead.DayOfWeek - 1 % 7;
                var calendarList = Enumerable.Repeat<int?>(null, 42).ToList();
                for (int i = 0; i < DateTime.DaysInMonth(today.Year, today.Month); i++)
                {
                    calendarList[offset + i] = i + 1;
                }
                ViewBag.Calendar = calendarList;

                var openDayList = db.t_order_calendar
                    .Where(d => d.date >= monthHead && d.date < nextMonthHead)
                    .OrderBy(d => d.date)
                    .Select(d => d.is_open)
                    .ToList();
                ViewBag.OpenDayList = openDayList;

                var calendarIdList = db.t_order_calendar
                    .Where(d => d.date >= monthHead && d.date < nextMonthHead)
                    .OrderBy(d => d.date)
                    .Select(d => d.id)
                    .ToList();
                ViewBag.CalendarIdList = calendarIdList;
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