using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRISWebService.Models;

namespace HRISWebService.Controllers
{
    public class OrasNaController : Controller
    {
        HRISEntities db = new HRISEntities();

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

            // log bundy transaction
            BundyTransaction(EIC, time_period);

            var log = db.tAttDailyLogs.Where(r => r.EIC == EIC).Where(r => r.LogDate == logDate);

            return Json(new { log }, JsonRequestBehavior.AllowGet);
        }
        public bool IsFlexi(String EIC)
        {
            var b = db.tappDFlexibles.SingleOrDefault(r => r.EIC == EIC);
            return b != null;
        }
        public void BundyTransaction(String EIC, String logStatus)
        {
            // log every bundy transaction
            var bt = new tappDFlexiblesLog()
            {
                EIC = EIC,
                loginStatus = logStatus,
                timeStamp = DateTime.Now
            };
            db.tappDFlexiblesLogs.Add(bt);
            db.SaveChanges();
        }
    }
}