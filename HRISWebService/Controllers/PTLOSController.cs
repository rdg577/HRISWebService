using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRISWebService.Models;

namespace HRISWebService.Controllers
{
    public class PTLOSController : Controller
    {
        HRISEntities db = new HRISEntities();

        public JsonResult GetPTLOSApplications(String approvingEIC)
        {
            var list = (from r in db.tptlosApps
                        where r.approveEIC == approvingEIC
                        where r.recommendStatus == 1
                        where r.Tag == 3
                        select new
                        {
                            r.recNo,
                            name = r.nameText,
                            destination = r.proceedTo.Trim(),
                            date_applied = r.dateApplied.ToString(),
                            r.purpose
                        }).ToList();
            dynamic wrap = new { ptlos_applications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PTLOSDetail(int id)
        {
            var list = (from r in db.tptlosApps
                        where r.recNo == id
                        select new
                        {
                            r.recNo,
                            name = r.nameText,
                            destination = r.proceedTo.Trim(),
                            purpose = r.purpose.Trim(),
                            date_applied = r.dateApplied.ToString(),
                            departure = r.departure.ToString(),
                            arrival = r.arrival.ToString(),
                            official_return = r.returnOfficial.ToString()
                        }).ToList();
            dynamic wrap = new { ptlos_detail = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PTLOSApproval(int id, int tag)
        {
            try
            {
                var ptlos = db.tptlosApps.Single(r => r.recNo == id);
                ptlos.Tag = tag;      // 3 - Recommend, 5 - approve, 6 - Disapprove, 9 - Cancel, 0 - Returned
                // update to code
                if (tag == 5)   // if approve
                {
                    ptlos.approvedDate = DateTime.Now;
                    ptlos.approveStatus = 1;    // 0 - default, 1 - approve, 3 - cancel
                    ptlos.remarks = "Approved";
                }

                if (tag == 6)   // if disapprove
                {
                    ptlos.approveStatus = 2;
                    ptlos.remarks = "Disapproved";
                }

                if (tag == 0)   // if returned
                {
                    ptlos.recommendStatus = 0;
                    ptlos.approveStatus = 0;    // 0 - default, 1 - approve, 3 - cancel
                    ptlos.remarks = "Returned";
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            dynamic wrap = new { ptlos_approval = new { tag } };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }

    }
}