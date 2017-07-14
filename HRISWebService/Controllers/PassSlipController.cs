using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRISWebService.Models;

namespace HRISWebService.Controllers
{
    public class PassSlipController : Controller
    {
        HRISEntities db = new HRISEntities();

        public JsonResult PassSlipApproval(int id, int statusId, int isOfficial)
        {

            try
            {
                var passSlip = db.tpassSlipApps.Single(r => r.recNo == id);
                passSlip.statusID = statusId;
                passSlip.isOfficial = isOfficial;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            dynamic wrap = new { pass_slip_approval = new { statusId } };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipDetail(int id)
        {
            var list = (from r in db.vpassSlipApps
                        where r.recNo == id
                        select new
                        {
                            recNo = r.recNo,
                            controlNo = r.controlNo,
                            EIC = r.EIC,
                            fullnameFirst = r.fullnameFirst,
                            timeOut = r.timeOut.ToString(),
                            timeIn = r.timeIn.ToString(),
                            destination = r.destination,
                            purpose = r.purpose,
                            isOfficial = r.isOfficial,
                            statusID = r.statusID,
                            apprvEIC = r.apprvEIC,
                        }).ToList();
            dynamic listWrapper = new { pass_slip_detail = list };
            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipsPending(String approvingEIC)
        {
            // select all pass slip application under a certain 
            // approving officer using EIC
            var list = (from r in db.vpassSlipApps
                        orderby r.timeOut
                        where r.apprvEIC == approvingEIC
                        where r.statusID == 0   // 0 (pending), 1 (approved), 2 (disapproved), 3 (cancelled)
                        select new
                        {
                            recNo = r.recNo,
                            controlNo = r.controlNo,
                            EIC = r.EIC,
                            timeOut = r.timeOut.ToString(),
                            timeIn = r.timeIn.ToString(),
                            destination = r.destination,
                            purpose = r.purpose,
                            isOfficial = r.isOfficial,
                            statusID = r.statusID,
                            apprvEIC = r.apprvEIC,
                            fullnameFirst = r.fullnameFirst,
                            empGroupCode = r.empGroupCode
                        }).ToList();

            dynamic listWrapper = new { pass_slips = list };

            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipsApproved(String approvingEIC)
        {
            // select all pass slip application under a certain 
            // approving officer using EIC
            var list = (from r in db.vpassSlipApps
                        orderby r.timeOut descending
                        where r.apprvEIC == approvingEIC
                        where r.statusID == 1   // 0 (pending), 1 (approved), 2 (disapproved), 3 (cancelled)
                        select new
                        {
                            recNo = r.recNo,
                            controlNo = r.controlNo,
                            EIC = r.EIC,
                            timeOut = r.timeOut,
                            timeIn = r.timeIn,
                            destination = r.destination,
                            purpose = r.purpose,
                            isOfficial = r.isOfficial,
                            statusID = r.statusID,
                            apprvEIC = r.apprvEIC,
                            fullnameFirst = r.fullnameFirst,
                            empGroupCode = r.empGroupCode
                        }).ToList();
            dynamic listWrapper = new { pass_slips = list };

            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipsDisapproved(String approvingEIC)
        {
            // select all pass slip application under a certain 
            // approving officer using EIC
            var list = (from r in db.vpassSlipApps
                        orderby r.timeOut descending
                        where r.apprvEIC == approvingEIC
                        where r.statusID == 2   // 0 (pending), 1 (approved), 2 (disapproved), 3 (cancelled)
                        select new
                        {
                            recNo = r.recNo,
                            controlNo = r.controlNo,
                            EIC = r.EIC,
                            timeOut = r.timeOut,
                            timeIn = r.timeIn,
                            destination = r.destination,
                            purpose = r.purpose,
                            isOfficial = r.isOfficial,
                            statusID = r.statusID,
                            apprvEIC = r.apprvEIC,
                            fullnameFirst = r.fullnameFirst,
                            empGroupCode = r.empGroupCode
                        }).ToList();
            dynamic listWrapper = new { pass_slips = list };

            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipsCancelled(String approvingEIC)
        {
            // select all pass slip application under a certain 
            // approving officer using EIC
            var list = (from r in db.vpassSlipApps
                        orderby r.timeOut descending
                        where r.apprvEIC == approvingEIC
                        where r.statusID == 3   // 0 (pending), 1 (approved), 2 (disapproved), 3 (cancelled)
                        select new
                        {
                            recNo = r.recNo,
                            controlNo = r.controlNo,
                            EIC = r.EIC,
                            timeOut = r.timeOut,
                            timeIn = r.timeIn,
                            destination = r.destination,
                            purpose = r.purpose,
                            isOfficial = r.isOfficial,
                            statusID = r.statusID,
                            apprvEIC = r.apprvEIC,
                            fullnameFirst = r.fullnameFirst,
                            empGroupCode = r.empGroupCode
                        }).ToList();
            dynamic listWrapper = new { pass_slips = list };

            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }

    }
}