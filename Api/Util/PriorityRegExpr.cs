using System;
using System.Text.RegularExpressions;

namespace Api.Util
{
	public class PriorityRegExpr
	{
		private static String REGEX = "( |^)(!|!!|!!!)( |$)";
		public int Priority { get; private set; }
		public String Message {get;private set;}

		public bool Success 
		{
			get
			{
				return (Priority !=-1);
			}
		}

		public PriorityRegExpr(String input) {
			Priority = -1;
			Match m= Regex.Match(input,REGEX);

			if( m.Success ) {
				String inputPattern = m.Groups[2].Value;
				if("!".Equals(inputPattern)) {
					Priority = 0;
				}else if("!!".Equals(inputPattern)) {
					Priority = 1;
				}else if("!!!".Equals(inputPattern)) {
					Priority = 2;
				}
				Message= Regex.Replace (input, REGEX, "");
			}
		}
	}
}

