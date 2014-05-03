using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowingData.Mung.Core {
	public enum TimePeriod {
		Milliseconds = 0,
		Seconds = 1,
		Minutes = 2,
		Hours = 3,
		Days = 4,
		Months = 5,
		Quarters = 6,
		Halves = 7,
		Years = 8,
	}

	public static class TimePeriodExtensions {


		/// <summary>
		/// Start and End are inclusive, and will both be returned by the enumeration
		/// as the first and last items respectively.
		/// </summary>
		/// <param name="period"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static IEnumerable<DateTime> DatesBetween(this TimePeriod period, DateTime start, DateTime end) {

			var periodCount = period.PeriodsBetween(start, end);
			for (var i = 0; i <= periodCount; i++) {
				yield return period.IncrementDate(start, i);
			}
		}


		public static int PeriodsBetween(this TimePeriod period, DateTime start, DateTime end) {

			int count = -1;
			switch (period) {
				case TimePeriod.Milliseconds:
					count = (int)end.Subtract(start).TotalMilliseconds;
					break;
				case TimePeriod.Seconds:
					count = (int)end.Subtract(start).TotalSeconds;
					break;
				case TimePeriod.Minutes:
					count = (int)end.Subtract(start).TotalMinutes;
					break;
				case TimePeriod.Hours:
					count = (int)end.Subtract(start).TotalHours;
					break;
				case TimePeriod.Days:
					count = (int)end.Subtract(start).TotalDays;
					break;
				case TimePeriod.Months:
					count = (int)((end.Year - start.Year) * 12) + end.Month - start.Month;
					break;
				case TimePeriod.Quarters:
					count = ((int)((end.Year - start.Year) * 12) + end.Month - start.Month) / 3;
					break;
				case TimePeriod.Halves:
					count = ((int)((end.Year - start.Year) * 12) + end.Month - start.Month) / 6;
					break;
				case TimePeriod.Years:
					count = (int)end.Year - start.Year;
					break;

				default:
					throw new Exception("Unknown period: " + period.ToString());

			}

			return count;
		}

		public static DateTime IncrementDate(this TimePeriod period, DateTime date, int amount) {
			switch (period) {
				case TimePeriod.Milliseconds:
					return date.AddMilliseconds(amount);
				case TimePeriod.Seconds:
					return date.AddSeconds(amount);
				case TimePeriod.Minutes:
					return date.AddMinutes(amount);
				case TimePeriod.Hours:
					return date.AddHours(amount);
				case TimePeriod.Days:
					return date.AddDays(amount);
				case TimePeriod.Months:
					return date.AddMonths(amount);
				case TimePeriod.Quarters:
					return date.AddMonths(amount * 3);
				case TimePeriod.Halves:
					return date.AddMonths(amount * 6);
				case TimePeriod.Years:
					return date.AddYears(amount);

				default:
					throw new Exception("Unknown period: " + period.ToString());

			}
		}


		/// <summary>
		/// A date represents a point in time, but we want our periods to have stanrardized
		/// start and end dates (e.g. if the period is Monthly, all dates in a sequence should
		/// represent the first day of the month.
		/// </summary>
		/// <param name="period"></param>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime StandardizeDate(this TimePeriod period, DateTime date) {
			switch (period) {
				case TimePeriod.Milliseconds:
					return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);
				case TimePeriod.Seconds:
					return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0);
				case TimePeriod.Minutes:
					return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, 0);
				case TimePeriod.Hours:
					return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0, 0);
				case TimePeriod.Days:
					return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
				case TimePeriod.Months:
					return new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
				case TimePeriod.Quarters:
					var monthQ = 1 + ((int)Math.Floor((date.Month - 1) / 3d) * 3);
					return new DateTime(date.Year, monthQ, 1, 0, 0, 0, 0);
				case TimePeriod.Halves:
					var monthH = 1 + ((int)Math.Floor((date.Month - 1) / 6d) * 6);
					return new DateTime(date.Year, monthH, 1, 0, 0, 0, 0);
				case TimePeriod.Years:
					return new DateTime(date.Year, 1, 1, 0, 0, 0, 0);

				default:
					throw new Exception("Unknown period: " + period.ToString());

			}


		}

	}

}