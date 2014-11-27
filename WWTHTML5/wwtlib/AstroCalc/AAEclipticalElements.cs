using System;
//
//Module : AAECLIPTICALELEMENTS.CPP
//Purpose: Implementation for the algorithms which map the ecliptical elements from one equinox to another
//Created: PJN / 29-12-2003
//History: PJN / 29-11-2006 1. Fixed a bug where CAAEclipticalElements::Calculate and CAAEclipticalElements::FK4B1950ToFK5J2000
//                          would return the incorrect value for the reduced inclination when the initial inclination value
//                          was > 90 degrees. 
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


/////////////////////////// Includes //////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAEclipticalElementDetails
{
//Constructors / Destructors
  public CAAEclipticalElementDetails()
  {
	  i = 0;
	  w = 0;
	  omega = 0;
  }

//Member variables
  public double i;
  public double w;
  public double omega;
}

public class  CAAEclipticalElements
{
//Static methods

  /////////////////////////// Implementation ////////////////////////////////////
  
  public static CAAEclipticalElementDetails Calculate(double i0, double w0, double omega0, double JD0, double JD)
  {
	double T = (JD0 - 2451545.0) / 36525;
	double Tsquared = T *T;
	double t = (JD - JD0) / 36525;
	double tsquared = t *t;
	double tcubed = tsquared * t;
  
	//Now convert to radians
	double i0rad = CT.D2R(i0);
	double omega0rad = CT.D2R(omega0);
  
	double eta = (47.0029 - 0.06603 *T + 0.000598 *Tsquared)*t + (-0.03302 + 0.000598 *T)*tsquared + 0.00006 *tcubed;
	eta = CT.D2R(CT.DMS2D(0, 0, eta));
  
	double pi = 174.876384 *3600 + 3289.4789 *T + 0.60622 *Tsquared - (869.8089 + 0.50491 *T)*t + 0.03536 *tsquared;
	pi = CT.D2R(CT.DMS2D(0, 0, pi));
  
	double p = (5029.0966 + 2.22226 *T - 0.000042 *Tsquared)*t + (1.11113 - 0.000042 *T)*tsquared - 0.000006 *tcubed;
	p = CT.D2R(CT.DMS2D(0, 0, p));
  
	double sini0rad = Math.Sin(i0rad);
	double cosi0rad = Math.Cos(i0rad);
	double sinomega0rad_pi = Math.Sin(omega0rad - pi);
	double cosomega0rad_pi = Math.Cos(omega0rad - pi);
	double sineta = Math.Sin(eta);
	double coseta = Math.Cos(eta);
	double A = sini0rad *sinomega0rad_pi;
	double B = -sineta *cosi0rad + coseta *sini0rad *cosomega0rad_pi;
	double irad = Math.Asin(Math.Sqrt(A *A + B *B));
  
	CAAEclipticalElementDetails details = new CAAEclipticalElementDetails();
  
	details.i = CT.R2D(irad);
	double cosi = cosi0rad *coseta + sini0rad *sineta *cosomega0rad_pi;
	if (cosi < 0)
	  details.i = 180 - details.i;
  
	double phi = pi + p;
	details.omega = CT.M360(CT.R2D(Math.Atan2(A, B) + phi));
  
	A = -sineta *sinomega0rad_pi;
	B = sini0rad *coseta - cosi0rad *sineta *cosomega0rad_pi;
	double deltaw = CT.R2D(Math.Atan2(A, B));
	details.w = CT.M360(w0 + deltaw);
  
	return details;
  }
  public static CAAEclipticalElementDetails FK4B1950ToFK5J2000(double i0, double w0, double omega0)
  {
	//convert to radians
	double L = CT.D2R(5.19856209);
	double J = CT.D2R(0.00651966);
	double i0rad = CT.D2R(i0);
	double omega0rad = CT.D2R(omega0);
	double sini0rad = Math.Sin(i0rad);
	double cosi0rad = Math.Cos(i0rad);
  
	//Calculate some values used later
	double cosJ = Math.Cos(J);
	double sinJ = Math.Sin(J);
	double W = L + omega0rad;
	double cosW = Math.Cos(W);
	double sinW = Math.Sin(W);
	double A = sinJ *sinW;
	double B = sini0rad *cosJ + cosi0rad *sinJ *cosW;
  
	//Calculate the values
	CAAEclipticalElementDetails details = new CAAEclipticalElementDetails();
	details.i = CT.R2D(Math.Asin(Math.Sqrt(A *A + B *B)));
	double cosi = cosi0rad *cosJ - sini0rad *sinJ *cosW;
	if (cosi < 0)
	  details.i = 180 - details.i;
  
	details.w = CT.M360(w0 + CT.R2D(Math.Atan2(A, B)));
	details.omega = CT.M360(CT.R2D(Math.Atan2(sini0rad *sinW, cosi0rad *sinJ + sini0rad *cosJ *cosW)) - 4.50001688);
  
	return details;
  }
}
