using MortgageCalulator.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace MortgageCalulator.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Default/

        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpGet]
        public PartialViewResult GetDataTable(string c, string L, string p, string n, string sm, string m, string sy, string y, string t, string q, string v)
        {
            double dc = Convert.ToDouble(c) / 12;
            double dL = Convert.ToDouble(L); // Loan amount
            double dp0 = Convert.ToDouble(p);  //  Months Into Loan
            double dn = Convert.ToDouble(n) * 12;  //  Term Of Loan
            int startMonth = Convert.ToByte(sm);
            int currMonth = Convert.ToByte(m);
            int startYear = Convert.ToInt16(sy);
            int currYear = Convert.ToInt16(y);
            double taxAndIns = Convert.ToDouble(t);
            double pAndI;
            double pAndA;
            double balance;
            double principle;
            double interest;
            double cumulativeAddedPymts;
            double prevBalance;
            int month = currMonth;
            int year = currYear;
            MortgageCalulator.Models.DataTableModel model = new DataTableModel();
            model.tableSet = new List<Dictionary<string, string>>();
            int k = Convert.ToInt32(q);
            string sessionId = Session.SessionID;
            ArrayList addedPymtArray = new ArrayList();
            int i = 0;

            if (q != "-1")
            {
                //addToPymt has changed
                double f = Convert.ToDouble(v);

                // Get the cache
                if (Session.Count < 1)
                {
                    // Cache timed out
                }
                else
                {
                    addedPymtArray = (ArrayList)Session[sessionId];
                    for (int r = k; r < addedPymtArray.Count; r++)
                    {
                        addedPymtArray[r] = v;
                    }
                }
            }
            else
            {
                // Initialize array
                for (int s = 0; s < 500; s++)
                {
                    addedPymtArray.Add(0.0);
                }

                Session[sessionId] = addedPymtArray;
            }

            // First row calculation
            double a = Math.Pow((1.0d + dc), dn);
            double b0 = Math.Pow((1.0d + dc), dp0);
            pAndI = dL * dc * a / (a - 1);
            balance = dL * (a - b0) / (a - 1.0d);
          
            double addedPymt = GetAddedPaymentValue(0, addedPymtArray);
            cumulativeAddedPymts = addedPymt;

            // Modify balance
            balance = balance - addedPymt;

            principle = 0.0;
            interest = 0.0;

            // Build the data table and the array
            // Iterate by rows
            while (balance > 0.0d)
            {
                double dTotal = pAndI + taxAndIns + addedPymt;

                // Load the new row values
                model.tableRow = new Dictionary<string, string>();
                model.tableRow["date"] = Convert.ToDateTime(String.Concat(year.ToString(), "-", month.ToString(), "-1")).ToString("MMM  yyyy"); 
                model.tableRow["principle"] = principle.ToString("C2");
                model.tableRow["pAndI"] = pAndI.ToString("C2");
                model.tableRow["tanAndIns"] = taxAndIns.ToString("C2");
                model.tableRow["addedPymt"] = addedPymt.ToString("F0");
                model.tableRow["total"] = dTotal.ToString("C2");
                model.tableRow["balance"] = balance.ToString("C2");

                // Add the new row to the data table
                model.tableSet.Add(model.tableRow);

                // Advance to the next row
                i++; // row ordinal
                dp0++; // months into the loan
                DateTime nextMonth = Convert.ToDateTime(String.Concat(year, "-", month, "-1")).AddMonths(1);
                month = nextMonth.Month;
                year = nextMonth.Year;

                // Next row calculation
                b0 = Math.Pow((1.0d + dc), dp0);
                prevBalance = balance;
                balance = dL * (a - b0) / (a - 1.0d);

                // Next row added payment
                addedPymt = GetAddedPaymentValue(i, addedPymtArray);
                cumulativeAddedPymts = cumulativeAddedPymts + addedPymt;

                // Adjusted balance
                balance = balance - cumulativeAddedPymts;

                // Calculate principle
                principle = prevBalance - balance;
            }

            if (Session.Count < 1)
            {
                Session[sessionId] = addedPymtArray;
            }

            Session[sessionId] = addedPymtArray;

            return PartialView("DataTable", model);
        }

        //[System.Web.Mvc.HttpGet]
        //public void UpdatePayments(string i, string v)
        //{
        //    int k = Convert.ToInt32(i);
        //    double f = Convert.ToDouble(v);
        //    string sessionId = Session.SessionID;
        //    ArrayList addToPymt = new ArrayList();

        //    // Get the cache
        //    if (Session.Count < 1)
        //    {
        //        // Cache timed out
        //    }
        //    else
        //    {
        //        addToPymt = (ArrayList)Session[sessionId];
        //        for (int j = k; j < addToPymt.Count; j++ )
        //        {
        //            addToPymt[j] = v;
        //        }
        //    }
        //    Session[sessionId] = addToPymt;
        //}

        private double GetAddedPaymentValue(int i, ArrayList addedPymtArray)
        {
            double dAddedPymt = 0.0;
            if (addedPymtArray.Count == 0)
            {
                // Initialize array element
                dAddedPymt = 0.0;
                addedPymtArray.Add(0.0);

                // Apply cache element
                if (addedPymtArray[i] == null)
                {
                    // Append array with new element having the value of the previous element
                    addedPymtArray.Add(addedPymtArray[i - 1]);
                }
            }

            // Get the value of the new element
            dAddedPymt = Convert.ToDouble(addedPymtArray[i]);

            return dAddedPymt;
        }
    }
}
