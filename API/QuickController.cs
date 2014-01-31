using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;



namespace MortgageCalulator.API
{
    public class QuickController : ApiController
    {
        //[System.Web.Http.HttpPost]
        //public PartialViewResult GetDataTable(Dictionary<string, string> postData)
        //{

        //    double dc = Convert.ToDouble(postData["c"]) / 12;
        //    double dL = Convert.ToDouble(postData["L"]);
        //    double dp = Convert.ToDouble(postData["p"]);  //  Months Into Loan
        //    double dn = Convert.ToDouble(postData["n"]);  //  Term Of Loan
        //    byte startMonth = Convert.ToByte(postData["sm"]);
        //    byte currMonth = Convert.ToByte(postData["m"]);
        //    int startYear = Convert.ToInt16(postData["sy"]);
        //    int currYear = Convert.ToInt16(postData["y"]);
        //    double dTaxAndIns = Convert.ToDouble(postData["t"]);

        //    double a = Math.Pow((1.0d + dc), dn);
        //    double b = Math.Pow((1.0d + dc), dp);
        //    double dPrinciple = dL * dc * a / (a - 1);
        //    double dBalance = dL * (a - b) / (a - 1.0d);

        //    byte month = currMonth;
        //    int year = currYear;

        //    double dAddedPymt = 0.0;
        //    double dTotal = dPrinciple + dTaxAndIns + dAddedPymt;

        //    List<Dictionary<string, string>> dt = new List<Dictionary<string, string>>();

        //    Dictionary<string, string> dr = new Dictionary<string, string>();
        //    dr["month"] = month.ToString();
        //    dr["year"] = year.ToString();
        //    dr["principle"] = dPrinciple.ToString("C2");
        //    dr["tanAndIns"] = dTaxAndIns.ToString("C2");
        //    dr["addedPymt"] = dAddedPymt.ToString("C2");
        //    dr["total"] = dTotal.ToString("C2");
        //    dr["balance"] = dBalance.ToString("C2");

        //    dt.Add(dr);

        //    return PartialView("DataTable");
        //}
    }
}
