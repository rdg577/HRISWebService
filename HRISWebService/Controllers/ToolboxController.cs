using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRISWebService.Models;
using Microsoft.Ajax.Utilities;

namespace HRISWebService.Controllers
{
    public class ToolboxController : Controller
    {
        HRISEntities db = new HRISEntities();

        public JsonResult GetAllApplications(String approvingEIC)
        {
            var passSlipApp = (from r in db.vpassSlipApps
                               where r.statusID == 0
                               where r.apprvEIC == approvingEIC
                               select r).Count();

            var ptlosApp = (from g in db.vPtlosApps
                            where g.recommendStatus == 1
                            where g.Tag == 3
                            where g.approveEIC == approvingEIC
                            select g).Count();

            var justificationApp = (from g in db.tjustifyApps
                                    where g.statusID == null
                                    where g.approveEIC == approvingEIC
                                    select g).DistinctBy(t => t.EIC).Count();

            var revertJustificationApp = (from g in db.tjustifyApps
                                          where g.statusID == 1               // 1 - is approved
                                          where g.approveEIC == approvingEIC
                                          where g.returnTag == 1
                                          select g).DistinctBy(t => t.EIC).Count();

            var returnDtrApp = (from g in db.tAttDTRs
                                where g.approveEIC == approvingEIC
                                where g.returnTag == 1
                                select g).DistinctBy(t => t.EIC).Count();

            var applicationMenus = new List<ApplicationMenu>();
            var menu1 = new ApplicationMenu(1, "PASS SLIP", "", passSlipApp);
            var menu2 = new ApplicationMenu(2, "PTLOS", "", ptlosApp);
            var menu3 = new ApplicationMenu(3, "JUSTIFICATION", "", justificationApp);
            var menu4 = new ApplicationMenu(4, "REVERT - JUSTIFICATION", "", revertJustificationApp);
            var menu5 = new ApplicationMenu(5, "RETURN - DTR", "", returnDtrApp);
            applicationMenus.Add(menu1);
            applicationMenus.Add(menu2);
            applicationMenus.Add(menu3);
            applicationMenus.Add(menu4);
            applicationMenus.Add(menu5);
//            applicationMenus.Reverse();

            return Json(applicationMenus, JsonRequestBehavior.AllowGet);
        }

        // ====================================================================================================
        public JsonResult CheckBundy(String EIC)
        {
            if (IsFlexi(EIC))
            {

                var logNow = DateTime.Now;
                var logDate = logNow.Date;
                var logTime = logNow.TimeOfDay;

                var attDailyLog = db.tAttDailyLogs.Where(r => r.EIC == EIC).SingleOrDefault(r => r.LogDate == logDate);

                String in1 = null, out1 = null, in2 = null, out2 = null;

                if (attDailyLog != null)
                {
                    if (attDailyLog.In1 != null) in1 = attDailyLog.In1.Value.ToShortTimeString();
                    if (attDailyLog.Out1 != null) out1 = attDailyLog.Out1.Value.ToShortTimeString();
                    if (attDailyLog.In2 != null) in2 = attDailyLog.In2.Value.ToShortTimeString();
                    if (attDailyLog.Out2 != null) out2 = attDailyLog.Out2.Value.ToShortTimeString();
                }

                // determine next log
                String nextLog = null;
                if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 6, 0, 0).TimeOfDay &&
                    logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 12, 0, 0).TimeOfDay &&
                    in1 == null)
                {
                    nextLog = "IN1";
                }
                else if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 8, 0, 0).TimeOfDay &&
                         logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 13, 0, 0).TimeOfDay &&
                         out1 == null)
                {
                    nextLog = "OUT1";
                }
                else if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 12, 30, 0).TimeOfDay &&
                         logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 17, 0, 0).TimeOfDay &&
                         in2 == null)
                {
                    nextLog = "IN2";
                }
                else if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 13, 0, 0).TimeOfDay &&
                         logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 23, 59, 59).TimeOfDay &&
                         out2 == null)
                {
                    nextLog = "OUT2";
                }

                return
                    Json(
                        new
                        {
                            status =
                                new { logDate = logDate.ToShortDateString(), nextLog, in1, out1, in2, out2, attDailyLog }
                        },
                        JsonRequestBehavior.AllowGet);
            }
            else
            {
                return
                    Json(
                        new
                        {
                            status =
                                new { logDate = "Invalid Flexi User" }
                        },
                        JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult LogBundy(String time_period, String EIC, String schemeCode)
        {
            var logNow = DateTime.Now;
            var logDate = logNow.Date;
            var logTime = logNow.TimeOfDay;

            // check for existing record entry
            var rec = db.tAttDailyLogs.Where(r => r.EIC == EIC).SingleOrDefault(r => r.LogDate == logDate);
            if (rec == null)
            {
                // create an entry if no existing record
                var n = new tAttDailyLog
                {
                    EIC = EIC,
                    LogDate = logDate,
                    SchemeCode = schemeCode
                };

                db.tAttDailyLogs.Add(n);
                db.SaveChanges();
            }

            // update the record
            var l = db.tAttDailyLogs.Where(r => r.EIC == EIC).Single(r => r.LogDate == logDate);
            if (time_period.Equals("IN1"))
            {
                l.In1 = logNow;
                l.LastLog = time_period;
            }
            else if (time_period.Equals("OUT1"))
            {
                l.Out1 = logNow;
                l.LastLog = time_period;
            }
            else if (time_period.Equals("IN2"))
            {
                l.In2 = logNow;
                l.LastLog = time_period;
            }
            else if (time_period.Equals("OUT2"))
            {
                l.Out2 = logNow;
                l.LastLog = time_period;
            }
            // save all changes
            db.SaveChanges();
            
            var log = db.tAttDailyLogs.Where(r => r.EIC == EIC).Where(r => r.LogDate == logDate);

            return Json(new { log }, JsonRequestBehavior.AllowGet);
        }
        public bool IsFlexi(String EIC)
        {
            var b = db.tappDFlexibles.SingleOrDefault(r => r.EIC == EIC);
            return b != null;
        }
        // ====================================================================================================
        
        /*
         * User Profile Image
         */
        public ActionResult UserImage(string id)
        {
            var cover = GetImageFromDataBase(id);
            return cover != null ? File(cover, "image/jpeg") : null;
        }

        public byte[] GetImageFromDataBase(string Id)
        {
            var q = from temp in db.tapp212Image
                    where temp.EIC == Id
                    select temp.imageData;
            byte[] cover = q.SingleOrDefault();
            return cover;
        }

        // ====================================================================================================
        
        /*
         * Different Listings
         */
        public JsonResult EmployeeList()
        {
            //var list = db.tappEmployee.OrderBy(g => g.fullnameLast).ToList();
            var list = (from r in db.tappEmployees
                        orderby r.fullnameLast
                        select new
                        {
                            EIC = r.EIC, 
                            FullnameWithEIC = (r.EIC + " - " + r.fullnameLast), 
                            Fullname = r.fullnameLast
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeDetail(String EIC)
        {
            //var list = db.tappEmployee.OrderBy(g => g.fullnameLast).ToList();
            var list = (from r in db.tappEmployees
                        orderby r.fullnameLast
                        where r.EIC == EIC
                        select new
                        {
                            EIC = r.EIC,
                            FullnameWithEIC = (r.EIC + " - " + r.fullnameLast),
                            Fullname = r.fullnameLast
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PositionList()
        {
            var list = (from r in db.tappPositions
                orderby r.fullDescription
                select r).ToArray();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Position(string id)
        {
            var list = (from r in db.tappPositions
                        orderby r.fullDescription
                        where r.positionCode==id
                        select r).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SubPositionList()
        {
            var list = (from r in db.tappPositionSubs
                        orderby r.subPositionName
                        select r).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult WorkStatusList()
        {
            var list = (from r in db.tappEmpStatus
                        orderby r.statusName
                        where !r.statusName.Equals("ADMIN")
                        select r).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OfficeList()
        {
            var list = (from r in db.tappOffices
                        orderby r.officeServiceRec
                        select new
                        {
                            recNo = r.recNo,
                            officeCode = r.officeCode,
                            officeName = r.officeName,
                            accronym = r.accronym,
                            officeServiceRec = r.officeServiceRec,
                            officeServiceRecWithCode = r.officeServiceRec + " - " + r.officeCode,
                            branch = r.branch,
                            sortTag = r.sortTag,
                            tagRemarks = r.tagRemarks
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalarySchemeList()
        {
            var list = (from r in db.tappSalarySchems
                        orderby r.paySchemeName
                        select new
                        {
                            paySchemeCode = r.paySchemeCode,
                            paySchemeNameWithCode = r.paySchemeCode + " - " + r.paySchemeName,
                            paySchemeName = r.paySchemeName,
                            paySchemeAcronym = r.paySchemeAcronym
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalarySchedule(int sg, int step)
        {
            var salSched = (from r in db.tappSalaryScheds
                            where r.sg == sg
                            where r.step == step
                            where r.isActive == 1
                            select r).ToList();
            return Json(salSched, JsonRequestBehavior.AllowGet);
        }
    }

    // ====================================================================================================

    class ApplicationMenu
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public int TotalApplications { get; set; }

        public ApplicationMenu(int _Id, string title, string iconUrl, int totalApplications)
        {
            Id = _Id;
            Title = title;
            IconUrl = iconUrl;
            TotalApplications = totalApplications;
        }
    }

    // ====================================================================================================



}