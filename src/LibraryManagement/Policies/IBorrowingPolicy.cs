using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Policies;

public interface IBorrowingPolicy
{
    //Maximum books a member can borrow at once
    int MaximumActiveLoans { get; }

    //Number of days a member can keep a borrowed book
    int LoanPeriodInDays { get; }

    //Number of times a loan can be renewed
    int MaximumRenewals { get; }

    //Fine charged for each overdue day
    decimal DailyOverdueFine { get; }
}