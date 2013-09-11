using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Site.Util
{
	public class DateRegExpr
	{
		private static String REGEX = "@(today|tomorrow|monday|tuesday|wednesday|thursday|friday|saturday|sunday|\\d{2}(-|/)\\d{2}(-|/)\\d{4})( |$)";
		public DateTime Date{ get; private set; }
		public string Message{ get; private set;}
		public bool Success {
			get;
			private set;
		}

		public DateRegExpr(String input) {
			Match m = Regex.Match(input,REGEX);
			if( m.Success ) {
				Success = true;
				String inputPattern = m.Groups[0].Value;
				DateTime now =DateTime.Now;
				int dayOfweek = -1;

				if ("sunday".Equals(inputPattern,StringComparison.CurrentCultureIgnoreCase)) {
					dayOfweek = (int)DayOfWeek.Sunday;
				} else if ("monday".Equals(inputPattern,StringComparison.CurrentCultureIgnoreCase)) {
					dayOfweek = (int)DayOfWeek.Monday;
				} else if ("tuesday".Equals(inputPattern,StringComparison.CurrentCultureIgnoreCase)) {
					dayOfweek = (int)DayOfWeek.Tuesday;
				} else if ("wednesday".Equals(inputPattern,StringComparison.CurrentCultureIgnoreCase)) {
					dayOfweek = (int)DayOfWeek.Wednesday;
				} else if ("thursday".Equals(inputPattern,StringComparison.CurrentCultureIgnoreCase)) {
					dayOfweek = (int)DayOfWeek.Thursday;
				} else if ("friday".Equals(inputPattern,StringComparison.CurrentCultureIgnoreCase)) {
					dayOfweek = (int)DayOfWeek.Friday;
				} else if ("saturday".Equals(inputPattern,StringComparison.CurrentCultureIgnoreCase)) {
					dayOfweek = (int)DayOfWeek.Saturday;
				}
				if( dayOfweek != -1) {
					int weekday = (int)now.DayOfWeek;
					if (weekday != dayOfweek)
					{
						int days = ((int)DayOfWeek.Saturday - weekday + dayOfweek) % 7;
						now.Add (new TimeSpan (days, 0, 0, 0));
					}else{
						now.Add (new TimeSpan (7, 0, 0, 0));
					}
					Date = now;
				}else if("today".Equals(inputPattern,StringComparison.CurrentCultureIgnoreCase)) {
					Date = now;
				}else if("tomorrow".Equals(inputPattern,StringComparison.CurrentCultureIgnoreCase)) {
					now.Add (new TimeSpan (1, 0, 0, 0));
					Date = now;
				}else if(inputPattern.Length == 10) {
					Date =DateTime.ParseExact(inputPattern.Replace("-", "/"),"dd/MM/yyyy",null);
				}
				Message= Regex.Replace (input, REGEX, "");
			}
		}
	}
}

