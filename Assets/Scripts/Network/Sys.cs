using System;

public class Sys{
    static int maxindex = 30000;

    public static int GetIndex(ref int index)
    {
        if (index < maxindex)
        {
            index++;
        }
        else
        {
            index = 0;
        }
        return index;
    }

    public static bool IsOrderRight(int lastindex, int nowindex)
    {
        if (nowindex - lastindex > 0) return true;
        if (nowindex - lastindex < -25000) return true;

        return false;
    }

    //获取时间戳
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
	
}
