using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRISWebService.Models;

namespace HRISWebService.Controllers
{
    public class DTRController : Controller
    {
        HRISEntities db = new HRISEntities();

        public JsonResult DTRAction(String DtrId, String strPeriod, int intPeriod, int action, String approvingEIC, String remarks)
        {
            Boolean has_error;
            try
            {
                // invoke the store procedure for DTR
                db.DtrAction(DtrId, strPeriod, intPeriod, action, approvingEIC, remarks);
                db.SaveChanges();

                has_error = false;
            }
            catch (Exception ex)
            {
                has_error = true;
            }

            dynamic wrap = new { dtr_action = new { has_error } };
            return Json(wrap, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DTRReturnRequest(String approvingEIC)
        {
            /**
             * Returns a list of Employee that requests return of their DTRs
             */
            var list = (from r in db.tAttDTRs
                        group r by new
                        {
                            r.EIC,
                            r.approveEIC,
                            fullnameFirst = (from s in db.tappEmployees where s.EIC == r.EIC select s.fullnameFirst).FirstOrDefault().ToString().Trim(),
                            r.returnTag,
                            total = (from y in db.tAttDTRs
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
                            g.Key.fullnameFirst,
                            g.Key.total,
                            g.Key.returnTag
                        });

            dynamic wrap = new { dtrs = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DTRReturnRequestPerEmployee(String EIC, String approvingEIC)
        {
            /**
             * Returns a list of DTRs requested by employee
             */
            var list = (from r in db.tAttDTRs
                        group r by new
                        {
                            r.DtrID,
                            r.Year,
                            r.Month,
                            r.Period,
                            r.EIC,
                            r.approveEIC,
                            r.Remarks,
                            r.returnTag,
                            fullnameFirst = (from s in db.tappEmployees where s.EIC == r.EIC select s.fullnameFirst).FirstOrDefault().ToString().Trim()
                        } into g
                        orderby new { g.Key.Year, g.Key.Month, g.Key.Period } descending
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.EIC == EIC
                        where g.Key.returnTag == 1
                        select new
                        {
                            g.Key.DtrID,
                            g.Key.Year,
                            g.Key.Month,
                            g.Key.Period,
                            g.Key.EIC,
                            g.Key.approveEIC,
                            g.Key.fullnameFirst,
                            g.Key.Remarks,
                            g.Key.returnTag
                        });

            dynamic wrap = new { dtrs = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DTRDetail(String DtrId)
        {
            /**
             * Returns DTR detail requested by employee for return
             */
            var list = (from r in db.tAttDTRs
                        where r.DtrID == DtrId
                        where r.returnTag == 1
                        select new
                        {
                            r.DtrID,
                            r.EIC,
                            r.approveEIC,
                            r.Remarks,
                            r.returnTag,
                            fullnameFirst = (from s in db.tappEmployees where s.EIC == r.EIC select s.fullnameFirst).FirstOrDefault().ToString().Trim()
                        });

            dynamic wrap = new { dtr = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
    }
}