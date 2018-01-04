using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRISWebService.Models;
using Microsoft.Ajax.Utilities;

namespace HRISWebService.Controllers
{
    public class JustificationController : Controller
    {
        HRISEntities db = new HRISEntities();

        public JsonResult JustificationRevertDetail(String EIC, String approvingEIC, int month, int year, int period)
        {
            var list = (from r in db.vJustifyApps
                        where r.EIC == EIC
                        where r.approveEIC == approvingEIC
                        where r.month == month
                        where r.year == year
                        where r.period == period
                        where r.returnTag == 1
                        select new
                        {
                            r.fullnameFirst,
                            r.logType,
                            r.logTitle,
                            r.time,
                            date = r.date.ToString(),
                            r.reason
                        }).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationRevertDetail2(String EIC, String approvingEIC, int month, int year, int period)
        {
            var list = (from r in db.vJustifyApps
                        where r.EIC == EIC
                        where r.approveEIC == approvingEIC
                        where r.month == month
                        where r.year == year
                        where r.period == period
                        where r.returnTag == 1
                        select new
                        {
                            r.recNo,
                            r.EIC,
                            r.month,
                            r.year,
                            r.monthyear,
                            r.approveEIC,
                            r.period,
                            r.fullnameFirst,
                            r.logType,
                            r.logTitle,
                            r.time,
                            date = r.date.ToString(),
                            r.reason
                        }).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationRevertPerMonth(String EIC, String approvingEIC)
        {
            /**
             * Returns a list of Justifications per Month for revert
             */
            var list = (from r in db.vJustifyApps
                        group r by new
                        {
                            month_year = r.monthyear,
                            r.EIC,
                            r.approveEIC,
                            r.month,
                            r.year,
                            r.period,
                            r.statusID,
                            r.returnTag,
                            total = (from y in db.vJustifyApps
                                     where y.EIC == r.EIC
                                     where y.monthyear == r.monthyear
                                     where y.period == r.period
                                     select y).Count()
                        } into g
                        orderby g.Key.year, g.Key.month
                        where g.Key.EIC == EIC
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.statusID == 1
                        where g.Key.returnTag == 1
                        select g.Key).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationRevert(String approvingEIC)
        {
            /**
             * Returns a list of Justifications for revert
             */
            var list = (from r in db.tjustifyApps
                        group r by new
                        {
                            r.EIC,
                            r.approveEIC,
                            r.statusID,
                            fullnameFirst = (from s in db.tappEmployees where s.EIC == r.EIC select s.fullnameFirst).FirstOrDefault().ToString(),
                            r.returnTag,
                            total = (from y in db.tjustifyApps
                                     where y.EIC == r.EIC
                                     where y.approveEIC == r.approveEIC
                                     where y.returnTag == 1
                                     select y).Count()
                        } into g
                        orderby g.Key.fullnameFirst
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.returnTag == 1
                        select new
                        {
                            g.Key.EIC,
                            g.Key.approveEIC,
                            g.Key.statusID,
                            g.Key.fullnameFirst,
                            g.Key.returnTag,
                            g.Key.total
                        });

            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationRevertPending2(String approvingEIC)
        {
            /**
             * Returns a list of Justification Totals for Approval
             */
            var list = (from r in db.vJustifyApps
                        group r by new
                        {
                            r.EIC,
                            r.approveEIC,
                            r.statusID,
                            r.fullnameFirst,
                            r.month,
                            r.year,
                            r.period,
                            r.returnTag,
                            total = (from y in db.vJustifyApps
                                     where y.EIC == r.EIC
                                     where y.approveEIC == r.approveEIC
                                     where y.returnTag == 1
                                     select y).Count()
                        } into g
                        orderby g.Key.fullnameFirst
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.returnTag == 1
                        select new
                        {
                            g.Key.EIC,
                            g.Key.approveEIC,
                            g.Key.statusID,
                            g.Key.fullnameFirst,
                            g.Key.total,
                            g.Key.returnTag,
                            details = (from x in db.vJustifyApps
                                       group x by new
                                       {
                                           x.EIC,
                                           x.approveEIC,
                                           x.statusID,
                                           x.month,
                                           x.year,
                                           x.period,
                                           x.monthyear,
                                           x.returnTag
                                       } into gx
                                       where gx.Key.EIC == g.Key.EIC
                                       where gx.Key.approveEIC == g.Key.approveEIC
                                       where gx.Key.returnTag == 1
                                       where gx.Key.month == g.Key.month
                                       where gx.Key.year == g.Key.year
                                       where gx.Key.period == g.Key.period
                                       select gx.Key).ToList()
                        });

            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JustificationPending(String approvingEIC)
        {
            /**
             * Returns a list of Justification Totals for Approval
             */
            var list = (from r in db.vJustifyApps
                        group r by new
                        {
                            r.EIC,
                            r.approveEIC,
                            r.statusID,
                            r.fullnameFirst,
                            total = (from y in db.vJustifyApps
                                     where y.EIC == r.EIC
                                     where y.approveEIC == r.approveEIC
                                     where y.statusID != 1
                                     select y).Count()
                        } into g
                        orderby g.Key.fullnameFirst
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.statusID != 1
                        select new
                        {
                            g.Key
                        });

            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationPending2(String approvingEIC)
        {
            /**
             * Returns a list of Justification Totals for Approval
             */
            var list = (from r in db.vJustifyApps
                        group r by new
                        {
                            r.EIC,
                            r.approveEIC,
                            r.statusID,
                            r.fullnameFirst,
                            r.month,
                            r.year,
                            r.period,
                            total = (from y in db.vJustifyApps
                                         where y.EIC == r.EIC
                                         where y.approveEIC == r.approveEIC
                                         where y.statusID != 1
                                         select y).Count()
                        } into g
                        orderby g.Key.fullnameFirst
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.statusID == null
                        select new
                        {
                            g.Key.EIC,
                            g.Key.approveEIC,
                            g.Key.statusID,
                            g.Key.fullnameFirst,
                            g.Key.total,
                            details = (from x in db.vJustifyApps
                                       group x by new
                                       {
                                           x.EIC,
                                           x.approveEIC,
                                           x.statusID,
                                           x.month,
                                           x.year,
                                           x.period,
                                           x.monthyear
                                       } into gx
                                       orderby gx.Key.monthyear 
                                       where gx.Key.EIC == g.Key.EIC
                                       where gx.Key.approveEIC == g.Key.approveEIC
                                       where gx.Key.statusID == null
//                                       where gx.Key.statusID != 1
//                                       where gx.Key.month == g.Key.month
//                                       where gx.Key.year == g.Key.year
//                                       where gx.Key.period == g.Key.period
                                       select gx.Key).ToList()
                        }).DistinctBy(t => t.EIC);

            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationPerMonth(String EIC, String approvingEIC)
        {
            /**
             * Returns a list of Justifications per Month for Approval
             */
            var list = (from r in db.vJustifyApps
                        group r by new
                        {
                            month_year = r.monthyear,
                            r.EIC,
                            r.approveEIC,
                            r.month,
                            r.year,
                            r.period,
                            r.statusID,
                            total = (from y in db.vJustifyApps
                                     where y.EIC == r.EIC
                                     where y.monthyear == r.monthyear
                                     where y.period == r.period
                                     select y).Count()
                        } into g
                        orderby g.Key.year, g.Key.month
                        where g.Key.EIC == EIC
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.statusID != 1
                        select g.Key).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationDetail(String EIC, String approvingEIC, int month, int year, int period)
        {
            var list = (from r in db.vJustifyApps
                        where r.EIC == EIC
                        where r.approveEIC == approvingEIC
                        where r.month == month
                        where r.year == year
                        where r.period == period
                        where r.statusID != 1
                        select new
                        {
                            r.fullnameFirst,
                            r.logType,
                            r.logTitle,
                            r.time,
                            date = r.date.ToString(),
                            r.reason
                        }).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationDetail2(String EIC, String approvingEIC, int month, int year, int period)
        {
            var list = (from r in db.vJustifyApps
                        where r.EIC == EIC
                        where r.approveEIC == approvingEIC
                        where r.month == month
                        where r.year == year
                        where r.period == period
                        where r.statusID != 1
                        select r).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationApproval(String EIC, int month, int year, String month_year, String approvingEIC, int statusID, int period, String remarks)
        {
            Boolean has_error;
            try
            {
                DateTime startDate = new DateTime(), endDate = new DateTime();
                String CtrlNo = EIC.Substring(0, 8) + year.ToString() + month_year.Substring(0, 3);

                // determine startDate and endDate
                if (period == 0)
                {
                    startDate = new DateTime(year, month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }
                else if (period == 1)
                {
                    startDate = new DateTime(year, month, 1);
                    endDate = new DateTime(year, month, 15);
                }
                else if (period == 2)
                {
                    startDate = new DateTime(year, month, 16);
                    endDate = startDate.AddDays(DateTime.DaysInMonth(year, month) - 16);
                }


                db.JustifyAction(EIC, DateTime.Parse(startDate.ToShortDateString()), DateTime.Parse(endDate.ToShortDateString()), statusID, approvingEIC, CtrlNo, remarks, period);
                db.SaveChanges();

                has_error = false;
            }
            catch (Exception ex)
            {
                has_error = true;
            }

            dynamic wrap = new { justification_approval = new { has_error } };
            return Json(wrap, JsonRequestBehavior.AllowGet);

        }

    }
}