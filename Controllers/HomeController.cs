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

        public ActionResult Cached()
        {
            return PartialView();
        }

        [System.Web.Mvc.HttpGet]
        public PartialViewResult GetDataTable(string c, string L, string n, string sm, string d, string sy, string t, string q, string v)
        {
            double dc = Convert.ToDouble(AlphaStrip(c)) / 1200;
            double dL = Convert.ToDouble(AlphaStrip(L)); // Loan amount
            double dn = Convert.ToDouble(n) * 12;  //  Term Of Loan
            int startMonth = Convert.ToByte(sm);
            DateTime date = Convert.ToDateTime(d); // Current date
            int startYear = Convert.ToInt16(sy);
            DateTime startDate = Convert.ToDateTime(String.Concat(sy, "-", sm, "-1"));
            double dp0 = (date.Date - startDate.Date).TotalDays * 12.0 / 365.25;  //  Months Into Loan
            double taxAndIns = Convert.ToDouble(AlphaStrip(t));
            double pAndI;
            double balance;
            double principle;
            double interest;
            double cumulativeAddedPymts;
            double prevBalance;
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
                model.tableRow["date"] = date.ToString("MMM  yyyy"); 
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
                date = date.AddMonths(1);

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

        private string AlphaStrip (string x)
        {
            string y = x.Replace(",", "");
            y = y.Replace("$", "");
            return y;
        }

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
