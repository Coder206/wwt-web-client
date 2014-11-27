using System.Diagnostics;
using System;
//
//Module : AADATE.CPP
//Purpose: Implementation for the algorithms which convert between the Gregorian and Julian calendars and the Julian Day
//Created: PJN / 29-12-2003
//History: PJN / 10-11-2004 1. Fix for CAADate::Get so that it works correctly for propalactive calendar dates
//         PJN / 15-05-2005 1. Fix for CAADate::Set(double JD, bool bGregorianCalendarCalendar) not setting the m_bGregorianCalendarCalendar
//                          member variable correctly. 
//         PJN / 26-01-2006 1. After a bug report from Ing. Taras Kapuszczak that a round trip of the date 25 January 100 as 
//                          specified in the Gregorian calendar to the Julian day number and then back again produces the
//                          incorrect date 26 January 100, I've spent some time looking into the 2 key Meeus Julian Day
//                          algorithms. It seems that the algorithms which converts from a Calendar date to JD works ok for
//                          propalactive dates, but the reverse algorithm which converts from a JD to a Calendar date does not.
//                          Since I made the change in behaviour to support propalactive Gregorian dates to address issues
//                          with the Moslem calendar (and since then I have discovered further unresolved bugs in the Moslem
//                          calendar algorithms and advised people to check out my AA+ library instead), I am now reverting
//                          these changes so that the date algorithms are now as presented in Meeus's book. This means that 
//                          dates after 15 October 1582 are assumed to be in the Gregorian calendar and dates before are 
//                          assumed to be in the Julian calendar. This change also means that some of the CAADate class 
//                          methods no longer require the now defunct "bool" parameter to specify which calendar the date 
//                          represents. As part of the testing for this release verification code has been added to AATest.cpp
//                          to test all the dates from JD 0 (i.e. 1 January -4712) to a date long in the future. Hopefully
//                          with this verification code, we should have no more reported issues with the class CAADate. Again 
//                          if you would prefer a much more robust and comprehensive Date time class framework, don't forget 
//                          to check out the authors DTime+ library.
//                          2. Optimized CAADate constructor code
//                          3. Provided a static version of CAADate::DaysInMonth() method
//                          4. Discovered an issue in CAADate::JulianToGregorian. It seems the algorithm presented in the
//                          book to do conversion from the Julian to Gregorian calendar fails for Julian dates before the 
//                          Gregorian calendar reform in 1582. I have sent an email to Jean Meeus to find out if this is a
//                          bug in my code or a deficiency in the algorithm presented. Currently the code will assert in this
//                          function if it is called for a date before the Gregorian reform.
//         PJN / 27-01-2007 1. The static version of the Set method has been renamed to DateToJD to avoid any confusion with
//                          the other Set methods. Thanks to Ing. Taras Kapuszczak for reporting this issue.
//                          2. The method InGregorianCalendar has now also been renamed to the more appropriate 
//                          AfterPapalReform.
//                          3. Reinstated the bGregorianCalendar parameter for the CAADate constructors and Set methods.
//                          4. Changed the parameter layout for the static version of DaysInMonth
//                          5. Addition of a InGregorianCalendar method.
//                          6. Addition of a SetInGregorianCalendar method.
//                          7. Reworked implementation of GregorianToJulian method.
//                          8. Reworked implementation of JulianToGregorian method.
//
//Copyright (c) 2003 - 2007 by PJ Naughter (Web: www.naughter.com, Email: pjna@naughter.com)
//
//All rights reserved.
//
//Copyright / Usage Details:
//
//You are allowed to include the source code in any product (commercial, shareware, freeware or otherwise) 
//when your product is released in binary form. You are allowed to modify the source code in any way you want 
//except you cannot modify the copyright details at the top of each module. If you want to distribute source 
//code with your application, then you are only allowed to distribute versions released by the author. This is 
//to maintain a single distribution point for the source code. 
//
//


//////////////////////////// Includes /////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class CalD
{
    //Constructors / Destructors
    public CalD()
    {
        Year = 0;
        Month = 0;
        Day = 0;
    }
    public static CalD Create(int year, int month, int day)
    {
        CalD item = new CalD();
        item.Year = year;
        item.Month = month;
        item.Day = day;

        return item;
    }
    //Member variables
    public int Year;
    public int Month;
    public int Day;
}
public enum DAY_OF_WEEK : int
{
    SUNDAY = 0,
    MONDAY = 1,
    TUESDAY = 2,
    WEDNESDAY = 3,
    THURSDAY = 4,
    FRIDAY = 5,
    SATURDAY = 6
}

public class  DT
{
//Enums
  

//Constructors / Destructors

	//////////////////////////// Implementation ///////////////////////////////////
	
	public DT()
	{
		m_dblJulian = 0;
		m_bGregorianCalendar = false;
	}
  public static DT Create(int Year, int Month, double Day, bool bGregorianCalendar)
  {
      DT item = new DT();
      item.Set(Year, Month, Day, 0, 0, 0, bGregorianCalendar);
      return item;
  }
  public static DT CreateHMS(int Year, int Month, double Day, double Hour, double Minute, double Second, bool bGregorianCalendar)
  {
      DT item = new DT();
      item.Set(Year, Month, Day, Hour, Minute, Second, bGregorianCalendar);
      return item;
  }
  public static DT CreateJD(double JD, bool bGregorianCalendar)
  {
      DT item = new DT();
      item.SetJD(JD, bGregorianCalendar);
      return item;
  }

//Static Methods
  public static double DateToJD(int Year, int Month, double Day, bool bGregorianCalendar)
  {
	int Y = Year;
	int M = Month;
	if (M < 3)
	{
	  Y = Y - 1;
	  M = M + 12;
	}
  
	int A = 0;
	int B = 0;
	if (bGregorianCalendar)
	{
	  A = (int)(Y / 100.0);
	  B = 2 - A + (int)(A / 4.0);
	}
  
	return (int)(365.25 * (Y + 4716)) + (int)(30.6001 * (M + 1)) + Day + B - 1524.5;
  }
  public static bool IsLeap(int Year, bool bGregorianCalendar)
  {
	if (bGregorianCalendar)
	{
	  if ((Year % 100) == 0)
		return ((Year % 400) == 0) ? true : false;
	  else
		return ((Year % 4) == 0) ? true : false;
	}
	else
	  return ((Year % 4) == 0) ? true : false;
  }
  //public static void DayOfYearToDayAndMonth(int DayOfYear, bool bLeap, ref int DayOfMonth, ref int Month)
  //{
  //  int K = bLeap ? 1 : 2;
  
  //  Month = (int)(9*(K + DayOfYear)/275.0 + 0.98);
  //  if (DayOfYear < 32)
  //    Month = 1;
  
  //  DayOfMonth = DayOfYear - (int)((275 *Month)/9.0) + (K *(int)((Month + 9)/12.0)) + 30;
  //}
  //public static CAACalendarDate JulianToGregorian(int Year, int Month, int Day)
  //{
  //  CAADate date = CAADate.Create(Year, Month, Day, false);
  //  date.SetInGregorianCalendar(true);
  
  //  CAACalendarDate GregorianDate = new CAACalendarDate();
  //  int Hour = 0;
  //  int Minute = 0;
  //  double Second = 0;
  //  date.Get(ref GregorianDate.Year, ref GregorianDate.Month, ref GregorianDate.Day, ref Hour, ref Minute, ref Second);
  
  //  return GregorianDate;
  //}
  //public static CAACalendarDate GregorianToJulian(int Year, int Month, int Day)
  //{
  //  CAADate date = CAADate.Create(Year, Month, Day, true);
  //  date.SetInGregorianCalendar(false);
  
  //  CAACalendarDate JulianDate = new CAACalendarDate();
  //  double[] D = date.Get();

  //  JulianDate.Year = (int)D[0];
  //  JulianDate.Month = (int)D[1];
  //  JulianDate.Day = (int)D[2];

  //  return JulianDate;
  //}
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  static int @int(double value);
  public static bool AfterPapalReform(int Year, int Month, double Day)
  {
	return ((Year > 1582) || ((Year == 1582) && (Month > 10)) || ((Year == 1582) && (Month == 10) && (Day >= 15)));
  }
  public static bool AfterPapalReformJD(double JD)
  {
	return (JD >= 2299160.5);
  }
  public static double DayOfYearJD(double JD, int Year, bool bGregorianCalendar)
  {
	return JD - DateToJD(Year, 1, 1, bGregorianCalendar) + 1;
  }
  public static int DaysInMonthForMonth(int Month, bool bLeap)
  {
	//Validate our parameters
	Debug.Assert(Month >= 1 && Month <= 12);
  
	int[] MonthLength = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 0 };
	if (bLeap)
	  MonthLength[1]++;
  
	return MonthLength[Month-1];
  }

//Non Static methods
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double Julian() const
	public double Julian()
	{
		return m_dblJulian;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: operator double() const
  //public static implicit operator double(CAADate ImpliedObject)
  //{
  //    return ImpliedObject.m_dblJulian;
  //}



//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int Day() const
	public int Day()
	{
        double[] D = Get();
        return (int)D[2];
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int Month() const
    public int Month()
    {
        double[] D = Get();
        return (int)D[1];

    }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int Year() const
	public int Year()
	{
        double[] D = Get();
        return (int)D[0];

	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int Hour() const
	public int Hour()
	{
        double[] D = Get();
        return (int)D[3];

	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int Minute() const
	public int Minute()
	{
        double[] D = Get();
        return (int)D[4];
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double Second() const
	public double Second()
	{
        double[] D = Get();
        return (int)D[5];
	}
  public void Set(int Year, int Month, double Day, double Hour, double Minute, double Second, bool bGregorianCalendar)
  {
	double dblDay = Day + (Hour/24) + (Minute/1440) + (Second / 86400);
	SetJD(DateToJD(Year, Month, dblDay, bGregorianCalendar), bGregorianCalendar);
  }
	public void SetJD(double JD, bool bGregorianCalendar)
	{
	  m_dblJulian = JD;
	  SetInGregorianCalendar(bGregorianCalendar);
	}
  public void SetInGregorianCalendar(bool bGregorianCalendar)
  {
	bool bAfterPapalReform = (m_dblJulian >= 2299160.5);
  

	m_bGregorianCalendar = bGregorianCalendar && bAfterPapalReform;
  }

 
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: void Get(int& Year, int& Month, int& Day, int& Hour, int& Minute, double& Second) const
  public double[] Get()
  {
    int Year;
    int Month;
    int Day;
    int Hour;
    int Minute;
    double Second;

	double JD = m_dblJulian + 0.5;
    double tempZ = Math.Floor(JD);
    double F = JD - tempZ;
	int Z = (int)(tempZ);
	int A;
  
	if (m_bGregorianCalendar) //There is a difference here between the Meeus implementation and this one
	//if (Z >= 2299161)       //The Meeus implementation automatically assumes the Gregorian Calendar
							  //came into effect on 15 October 1582 (JD: 2299161), while the CAADate
							  //implementation has a "m_bGregorianCalendar" value to decide if the date
							  //was specified in the Gregorian or Julian Calendars. This difference
							  //means in effect that CAADate fully supports a propalactive version of the
							  //Julian calendar. This allows you to construct Julian dates after the Papal
							  //reform in 1582. This is useful if you want to construct dates in countries
							  //which did not immediately adapt the Gregorian calendar
	{
	  int alpha = (int)((Z - 1867216.25) / 36524.25);
	  A = Z + 1 + alpha - (int)((int)alpha/4.0);
	}
	else
	  A = Z;
  
	int B = A + 1524;
	int C = (int)((B - 122.1) / 365.25);
	int D = (int)(365.25 * C);
	int E = (int)((B - D) / 30.6001);
  
	double dblDay = B - D - (int)(30.6001 * E) + F;
	Day = (int)(dblDay);
  
	if (E < 14)
	  Month = E - 1;
	else
	  Month = E - 13;
  
	if (Month > 2)
	  Year = C - 4716;
	else
	  Year = C - 4715;
    tempZ = Math.Floor(dblDay);

    F = dblDay - tempZ;
	Hour = (int)(F *24);
	Minute = (int)((F - (Hour)/24.0)*1440.0);
	Second = (F - (Hour / 24.0) - (Minute / 1440.0)) * 86400.0;

    return new double[] { Year, Month, Day, Hour, Minute, Second };
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: CAADate::DAY_OF_WEEK DayOfWeek() const
	public DAY_OF_WEEK DayOfWeek()
	{
	  return (DAY_OF_WEEK)(((int)(m_dblJulian + 1.5) % 7));
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double DayOfYear() const
  public double DayOfYear()
  {
      int year = (int)Get()[0];
  
	  return DayOfYearJD(m_dblJulian, year, AfterPapalReform(year, 1, 1));
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int DaysInMonth() const
  public int DaysInMonth()
  {
      double[] D = Get();

      int Year = (int)D[0];
      int Month = (int)D[1];


      return DaysInMonthForMonth(Month, IsLeap(Year, m_bGregorianCalendar));
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int DaysInYear() const
  public int DaysInYear()
  {
      double[] D = Get();

      int Year = (int)D[0];


      if (IsLeap(Year, m_bGregorianCalendar))
          return 366;
      else
          return 365;
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool Leap() const
  public bool Leap()
  {
	return IsLeap(Year(), m_bGregorianCalendar);
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool InGregorianCalendar() const
  public bool InGregorianCalendar()
  {
	  return m_bGregorianCalendar;
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double FractionalYear() const
  public double FractionalYear()
  {
      double[] D = Get();

      int Year = (int)D[0];
      int Month = (int)D[1];
      int Day = (int)D[2];
      int Hour = (int)D[3];
      int Minute = (int)D[4];
      double Second = D[5];
  
	int DaysInYear;
	if (IsLeap(Year, m_bGregorianCalendar))
	  DaysInYear = 366;
	else
	  DaysInYear = 365;
  
	return Year + ((m_dblJulian - DateToJD(Year, 1, 1, AfterPapalReform(Year, 1, 1))) / DaysInYear);
  }

//Member variables
  protected double m_dblJulian; //Julian Day number for this date
  protected bool m_bGregorianCalendar; //Is this date in the Gregorian calendar

  public static int INT(double @value)
  {
      if (@value >= 0)
          return (int)(@value);
      else
          return (int)(@value - 1);
  }
}

////C++ TO C# CONVERTER WARNING: The declaration of the following method implementation was not found:
////ORIGINAL LINE: int CAADate::INT(double value)



//public class CAADate
//{
//    public int INT(double @value)
//    {
//      if (@value >= 0)
//        return (int)(@value);
//      else
//        return (int)(@value - 1);
//    }
//}