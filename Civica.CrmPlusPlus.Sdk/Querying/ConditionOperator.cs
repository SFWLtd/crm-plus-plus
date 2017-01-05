namespace Civica.CrmPlusPlus.Sdk.Querying
{
    public class ConditionOperator
    {
        internal string Value { get; }

        private ConditionOperator(string value)
        {
            Value = value;
        }

        public static ConditionOperator Equal { get { return new ConditionOperator("eq"); } }

        public static ConditionOperator GreaterThanOrEqual { get { return new ConditionOperator("ge"); } }

        public static ConditionOperator GreaterThan { get { return new ConditionOperator("gt"); } }

        public static ConditionOperator LessThan { get { return new ConditionOperator("lt"); } }

        public static ConditionOperator Like { get { return new ConditionOperator("like"); } }

        public static ConditionOperator Between { get { return new ConditionOperator("between"); } }

        public static ConditionOperator EqualBusinessId { get { return new ConditionOperator("eq-businessid"); } }

        public static ConditionOperator EqualUserId { get { return new ConditionOperator("eq-userid"); } }

        public static ConditionOperator EqualUserTeams { get { return new ConditionOperator("eq-userteams"); } }

        public static ConditionOperator In { get { return new ConditionOperator("in"); } }

        public static ConditionOperator InFiscalPeriod { get { return new ConditionOperator("in-fiscal-period"); } }

        public static ConditionOperator InFiscalPeriodAndYear { get { return new ConditionOperator("in-fiscal-period-and-year"); } }

        public static ConditionOperator InFiscalYear { get { return new ConditionOperator("in-fiscal-year"); } }

        public static ConditionOperator InOrAfterFiscalPeriodAndYear { get { return new ConditionOperator("in-or-after-fiscal-period-and-year"); } }

        public static ConditionOperator InOrBeforeFiscalPeriodAndYear { get { return new ConditionOperator("in-or-before-fiscal-period-and-year"); } }

        public static ConditionOperator Last7Days { get { return new ConditionOperator("last-seven-days"); } }

        public static ConditionOperator LastFiscalPeriod { get { return new ConditionOperator("last-fiscal-period"); } }

        public static ConditionOperator LastFiscalYear { get { return new ConditionOperator("last-fiscal-year"); } }

        public static ConditionOperator LastMonth { get { return new ConditionOperator("last-month"); } }

        public static ConditionOperator LastWeek { get { return new ConditionOperator("last-week"); } }

        public static ConditionOperator LastXDays { get { return new ConditionOperator("last-x-days"); } }

        public static ConditionOperator LastXFiscalPeriods { get { return new ConditionOperator("last-x-fiscal-periods"); } }

        public static ConditionOperator LastXFiscalYears { get { return new ConditionOperator("last-x-fiscal-years"); } }

        public static ConditionOperator LastXHours { get { return new ConditionOperator("last-x-hours"); } }

        public static ConditionOperator LastXMonths { get { return new ConditionOperator("last-x-months"); } }

        public static ConditionOperator LastXWeeks { get { return new ConditionOperator("last-x-weeks"); } }

        public static ConditionOperator LastXYears { get { return new ConditionOperator("last-x-years"); } }

        public static ConditionOperator LastYear { get { return new ConditionOperator("last-year"); } }

        public static ConditionOperator LessThanOrEqual { get { return new ConditionOperator("le"); } }

        public static ConditionOperator Next7Days { get { return new ConditionOperator("next-seven-days"); } }

        public static ConditionOperator NextFiscalPeriod { get { return new ConditionOperator("next-fiscal-period"); } }

        public static ConditionOperator NextFiscalYear { get { return new ConditionOperator("next-fiscal-year"); } }

        public static ConditionOperator NextMonth { get { return new ConditionOperator("next-month"); } }

        public static ConditionOperator NextWeek { get { return new ConditionOperator("next-week"); } }

        public static ConditionOperator NextXDays { get { return new ConditionOperator("next-x-days"); } }

        public static ConditionOperator NextXFiscalPeriods { get { return new ConditionOperator("next-x-fiscal-periods"); } }

        public static ConditionOperator NextXFiscalYears { get { return new ConditionOperator("next-x-fiscal-years"); } }

        public static ConditionOperator NextXHours { get { return new ConditionOperator("next-x-hours"); } }

        public static ConditionOperator NextXMonths { get { return new ConditionOperator("next-x-months"); } }

        public static ConditionOperator NextXWeeks { get { return new ConditionOperator("next-x-weeks"); } }

        public static ConditionOperator NextXYears { get { return new ConditionOperator("next-x-years"); } }

        public static ConditionOperator NextYear { get { return new ConditionOperator("next-year"); } }

        public static ConditionOperator NotBetween { get { return new ConditionOperator("not-between"); } }

        public static ConditionOperator NotEqual { get { return new ConditionOperator("ne"); } }

        public static ConditionOperator NotEqualBusinessId { get { return new ConditionOperator("ne-businessid"); } }

        public static ConditionOperator NotEqualUserId { get { return new ConditionOperator("ne-userid"); } }

        public static ConditionOperator NotIn { get { return new ConditionOperator("not-in"); } }

        public static ConditionOperator NotLike { get { return new ConditionOperator("not-like"); } }

        public static ConditionOperator NotNull { get { return new ConditionOperator("not-null"); } }

        public static ConditionOperator NotOn { get { return new ConditionOperator("ne"); } }

        public static ConditionOperator Null { get { return new ConditionOperator("null"); } }

        public static ConditionOperator OlderThanXMonths { get { return new ConditionOperator("olderthan-x-months"); } }

        public static ConditionOperator On { get { return new ConditionOperator("on"); } }

        public static ConditionOperator OnOrAfter { get { return new ConditionOperator("on-or-after"); } }

        public static ConditionOperator OnOrBefore { get { return new ConditionOperator("on-or-before"); } }

        public static ConditionOperator ThisFiscalPeriod { get { return new ConditionOperator("this-fiscal-period"); } }

        public static ConditionOperator ThisFiscalYear { get { return new ConditionOperator("this-fiscal-year"); } }

        public static ConditionOperator ThisMonth { get { return new ConditionOperator("this-month"); } }

        public static ConditionOperator ThisWeek { get { return new ConditionOperator("this-week"); } }

        public static ConditionOperator ThisYear { get { return new ConditionOperator("this-year"); } }

        public static ConditionOperator Today { get { return new ConditionOperator("today"); } }

        public static ConditionOperator Tomorrow { get { return new ConditionOperator("tomorrow"); } }

        public static ConditionOperator Yesterday { get { return new ConditionOperator("yesterday"); } }
    }
}
