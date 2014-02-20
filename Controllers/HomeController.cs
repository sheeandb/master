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

        public ActionResult Default()
        {
            return View();
        }

        public ActionResult Cached()
        {
            return PartialView();
        }

        [System.Web.Mvc.HttpPost]
        public PartialViewResult GetDataTable(string c, string L, string n, string sm, string d, string sy, string t, string q, string v, string adj)
        {
            double dc = Convert.ToDouble(AlphaStrip(c)) / 1200;
            double loanAmount = Convert.ToDouble(AlphaStrip(L));
            int termOfLoan = Convert.ToInt32(n) * 12;
            int startMonth = Convert.ToByte(sm);
            DateTime currentDate = Convert.ToDateTime(d);
            int startYear = Convert.ToInt16(sy);
            DateTime startDate = Convert.ToDateTime(String.Concat(sy, "-", sm, "-1"));
            double monthsIntoLoan = (currentDate.Date - startDate.Date).TotalDays * 12.0 / 365.25;
            double taxAndIns = Convert.ToDouble(AlphaStrip(t));
            double adjustment = Convert.ToDouble(AlphaStrip(adj));
            double pAndI;
            double balance;
            double addedPymt;
            double principle;
            double interest;
            double cumulativeAddedPymts = 0.0;
            double prevBalance;
            double total;
            double oldAddedPayment;
            DataTableModel model = new DataTableModel();
            model.tableSet = new List<Dictionary<string, string>>();
            int addedPymtStartingRow = Convert.ToInt32(q);
            string sessionId = Session.SessionID;
            ArrayList pAndIArray = new ArrayList();
            ArrayList balanceArray = new ArrayList(); // Holds the balance for each month
            ArrayList addedPymtArray = new ArrayList(); // Holds the Additional Principle Payment for each month
            ArrayList cumAddedPymtArray = new ArrayList(); // Holds the Cumulative Additional Principle Payment for each month
            int i = 0; // Zero-based row number, where i = 0 to model.tableSet.count()
            int month = 1; // Full term month counter

            if (addedPymtStartingRow == -1)
            {
                // This is the initial rendering of the table

                // Build the arrays
                // Iterate by rows
                for (month = 1; month <= termOfLoan; month++)
                {
                    // Row calculation
                    double a = Math.Pow((1.0d + dc), termOfLoan);
                    double b = Math.Pow((1.0d + dc), month);
                    pAndI = loanAmount * dc * a / (a - 1);
                    balance = loanAmount * (a - b) / (a - 1.0d);

                    // Save row values to arrays
                    pAndIArray.Add(pAndI);
                    balanceArray.Add(balance);
                    addedPymtArray.Add(0.0);
                    cumAddedPymtArray.Add(0.0);
                }

                // Store arrays to session State
                Session[String.Concat("pAndI-", sessionId)] = pAndIArray;
                Session[String.Concat("balance-", sessionId)] = balanceArray;
                Session[String.Concat("addedPymt-", sessionId)] = addedPymtArray;
                Session[String.Concat("cumAddedPymt-", sessionId)] = cumAddedPymtArray;
            }
            else
            {
                // There is an additional payment
                // So, there has to be Session variables
                addedPymtStartingRow = addedPymtStartingRow + (int)monthsIntoLoan;
                double additionalPymt = Convert.ToDouble(v);

                // Retrieve arrays from session state
                addedPymtArray = (ArrayList)Session[String.Concat("addedPymt-", sessionId)];
                balanceArray = (ArrayList)Session[String.Concat("balance-", sessionId)];
                cumAddedPymtArray = (ArrayList)Session[String.Concat("cumAddedPymt-", sessionId)];

                // Modify arrays
                for (int r = addedPymtStartingRow; r < addedPymtArray.Count; r++)
                {
                    oldAddedPayment = (double)addedPymtArray[r];
                    addedPymtArray[r] = additionalPymt;
                    cumulativeAddedPymts = cumulativeAddedPymts + additionalPymt - oldAddedPayment;
                    cumAddedPymtArray[r] = cumulativeAddedPymts;
                    balanceArray[r] = (double)balanceArray[r] - cumulativeAddedPymts;
                }

                // Resave the modified arrays
                Session[String.Concat("addedPymt-", sessionId)] = addedPymtArray;
                Session[String.Concat("balance-", sessionId)] = balanceArray;
                Session[String.Concat("cumAddedPymt-", sessionId)] = cumAddedPymtArray;
            }


            // Retrieve arrays from session state
            addedPymtArray = (ArrayList)Session[String.Concat("addedPymt-", sessionId)];
            balanceArray = (ArrayList)Session[String.Concat("balance-", sessionId)];
            pAndIArray = (ArrayList)Session[String.Concat("pAndI-", sessionId)];
            cumAddedPymtArray = (ArrayList)Session[String.Concat("cumAddedPymt-", sessionId)];

            // Build data model for table from the ArrayLists
            month = (int)monthsIntoLoan;
            balance = (double)balanceArray[month] - adjustment;
            total = (double)pAndIArray[month] + (double)addedPymtArray[month] + taxAndIns;

            while ((balance > 0.0) && (month < termOfLoan) )
            {
                pAndI = (double)pAndIArray[month];
                addedPymt = (double)addedPymtArray[month];

                model.tableRow = new Dictionary<string, string>();
                model.tableRow["date"] = startDate.AddMonths(month).ToString("MMM  yyyy");
                if (month < 1)
                {
                    model.tableRow["principle"] = "";
                }
                else
                {
                    model.tableRow["principle"] = (Convert.ToDouble(balanceArray[month - 1]) - Convert.ToDouble(balanceArray[month]) - addedPymt).ToString("C2");
                }
                model.tableRow["pAndI"] = pAndI.ToString("C2");
                model.tableRow["addedPymt"] = addedPymt.ToString("F0");
                model.tableRow["total"] = total.ToString("C2");
                model.tableRow["balance"] = balance.ToString("C2");
                model.tableRow["taxAndIns"] = taxAndIns.ToString("C2");

                // Add the row to the data table
                model.tableSet.Add(model.tableRow);

                // Iterate row
                month++;
                balance = (double)balanceArray[month] - adjustment;
            }


            Session[String.Concat("pAndI-", sessionId)] = pAndIArray;
            Session[String.Concat("balance-", sessionId)] = balanceArray;
            Session[String.Concat("addedPymt-", sessionId)] = addedPymtArray;

            return PartialView("DataTable", model);
        }

        private string AlphaStrip (string x)
        {
            if (!String.IsNullOrEmpty(x))
            {
                string y = x.Replace(",", "");
                y = y.Replace("$", "");
                return y;
            }
            else
            {
                return "0.0";
            }
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
            dAddedPymt = (double)addedPymtArray[i];

            return dAddedPymt;
        }
    }
}
