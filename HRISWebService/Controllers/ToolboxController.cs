using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRISWebService.Models;

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
                                    select g).Count();

            var revertJustificationApp = (from g in db.tjustifyApps
                                          where g.statusID == 1               // 1 - is approved
                                          where g.approveEIC == approvingEIC
                                          where g.returnTag == 1
                                          select g).Count();

            var returnDtrApp = (from g in db.tAttDTRs
                                where g.approveEIC == approvingEIC
                                where g.returnTag == 1
                                select g).Count();

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
            applicationMenus.Reverse();

            return Json(applicationMenus, JsonRequestBehavior.AllowGet);
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