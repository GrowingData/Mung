using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowingData.Mung.Core {
	public enum TimePeriod {
		Millisecond = 0,
		Second = 1,
		Minute = 2,
		Hour = 3,
		Day = 4,
		Month = 5,
		Quarter = 6,
		Half = 7,
		Year = 8,
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
				case TimePeriod.Millisecond:
					count = (int)end.Subtract(start).TotalMilliseconds;
					break;
				case TimePeriod.Second:
					count = (int)end.Subtract(start).TotalSeconds;
					break;
				case TimePeriod.Minute:
					count = (int)end.Subtract(start).TotalMinutes;
					break;
				case TimePeriod.Hour:
					count = (int)end.Subtract(start).TotalHours;
					break;
				case TimePeriod.Day:
					count = (int)end.Subtract(start).TotalDays;
					break;
				case TimePeriod.Month:
					count = (int)((end.Year - start.Year) * 12) + end.Month - start.Month;
					break;
				case TimePeriod.Quarter:
					count = ((int)((end.Year - start.Year) * 12) + end.Month - start.Month) / 3;
					break;
				case TimePeriod.Half:
					count = ((int)((end.Year - start.Year) * 12) + end.Month - start.Month) / 6;
					break;
				case TimePeriod.Year:
					count = (int)end.Year - start.Year;
					break;

				default:
					throw new Exception("Unknown period: " + period.ToString());

			}

			return count;
		}

		public static DateTime IncrementDate(this TimePeriod period, DateTime date, int amount) {
			switch (period) {
				case TimePeriod.Millisecond:
					return date.AddMilliseconds(amount);
				case TimePeriod.Second:
					return date.AddSeconds(amount);
				case TimePeriod.Minute:
					return date.AddMinutes(amount);
				case TimePeriod.Hour:
					return date.AddHours(amount);
				case TimePeriod.Day:
					return date.AddDays(amount);
				case TimePeriod.Month:
					return date.AddMonths(amount);
				case TimePeriod.Quarter:
					return date.AddMonths(amount * 3);
				case TimePeriod.Half:
					return date.AddMonths(amount * 6);
				case TimePeriod.Year:
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
				case TimePeriod.Millisecond:
					return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);
				case TimePeriod.Second:
					return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0);
				case TimePeriod.Minute:
					return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, 0);
				case TimePeriod.Hour:
					return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0, 0);
				case TimePeriod.Day:
					return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
				case TimePeriod.Month:
					return new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
				case TimePeriod.Quarter:
					var monthQ = 1 + ((int)Math.Floor((date.Month - 1) / 3d) * 3);
					return new DateTime(date.Year, monthQ, 1, 0, 0, 0, 0);
				case TimePeriod.Half:
					var monthH = 1 + ((int)Math.Floor((date.Month - 1) / 6d) * 6);
					return new DateTime(date.Year, monthH, 1, 0, 0, 0, 0);
				case TimePeriod.Year:
					return new DateTime(date.Year, 1, 1, 0, 0, 0, 0);

				default:
					throw new Exception("Unknown period: " + period.ToString());

			}


		}

	}

}