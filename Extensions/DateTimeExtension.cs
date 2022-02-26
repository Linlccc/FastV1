namespace System
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 获取指定时间的的时间戳 毫秒
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string DateToTimeStamp(this DateTime dateTime) => (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds.OToString().GetNumberString();

        /// <summary>
        /// 获取两个时间相差的时间段
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static string TimeSubTract(this DateTime t1, DateTime t2)
        {
            TimeSpan span = t1.Subtract(t2);
            return $"{span.Days} 天 {span.Hours} 时 {span.Minutes} 分";
        }

        /// <summary>
        /// 两个时间是否是同一天
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool IsSameDay(this DateTime t1, DateTime t2 = default) => t1.Date == (t2 == default ? DateTime.Now.Date : t2.Date);

        /// <summary>
        /// 判断时间是否在一个时间段内
        /// </summary>
        /// <param name="t"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool IsInTimeSlot(this DateTime t, DateTime startTime, DateTime endTime) => t > startTime && t < endTime;

        /// <summary>
        /// 获取指定时间的周
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetWeek(this DateTime dateTime) => dateTime.DayOfWeek switch
        {
            DayOfWeek.Monday => "周一",
            DayOfWeek.Tuesday => "周二",
            DayOfWeek.Wednesday => "周三",
            DayOfWeek.Thursday => "周四",
            DayOfWeek.Friday => "周五",
            DayOfWeek.Saturday => "周六",
            DayOfWeek.Sunday => "周日",
            _ => "N/A",
        };
    }
}