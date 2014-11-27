using System;
//
//Module : AABINARYSTAR.CPP
//Purpose: Implementation for the algorithms for an binary stars system
//Created: PJN / 29-12-2003
//History: None
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


/////////////////////////////////// Includes //////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAABinaryStarDetails
{
//Constructors / Destructors
  public CAABinaryStarDetails()
  {
	  r = 0;
	  Theta = 0;
	  Rho = 0;
  }

//Member variables
  public double r;
  public double Theta;
  public double Rho;
}

public class  CAABinaryStar
{
//Static methods
  //Tangible Process Only End
  
  
  ////////////////////////////////// Implementation /////////////////////////////
  
  public static CAABinaryStarDetails Calculate(double t, double P, double T, double e, double a, double i, double omega, double w)
  {
	double n = 360 / P;
	double M = CT.M360(n*(t - T));
	double E = CAAKepler.Calculate(M, e);
	E = CT.D2R(E);
	i = CT.D2R(i);
	w = CT.D2R(w);
	omega = CT.D2R(omega);
  
	CAABinaryStarDetails details = new CAABinaryStarDetails();
  
	details.r = a*(1 - e *Math.Cos(E));
  
	double v = Math.Atan(Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E/2)) * 2;
	details.Theta = Math.Atan2(Math.Sin(v + w) * Math.Cos(i), Math.Cos(v + w)) + omega;
	details.Theta = CT.M360(CT.R2D(details.Theta));
  
	double sinvw = Math.Sin(v + w);
	double cosvw = Math.Cos(v + w);
	double cosi = Math.Cos(i);
	details.Rho = details.r * Math.Sqrt((sinvw *sinvw *cosi *cosi) + (cosvw *cosvw));
  
	return details;
  }
  public static double ApparentEccentricity(double e, double i, double w)
  {
	i = CT.D2R(i);
	w = CT.D2R(w);
  
	double cosi = Math.Cos(i);
	double cosw = Math.Cos(w);
	double sinw = Math.Sin(w);
	double esquared = e *e;
	double A = (1 - esquared *cosw *cosw)*cosi *cosi;
	double B = esquared *sinw *cosw *cosi;
	double C = 1 - esquared *sinw *sinw;
	double D = (A - C)*(A - C) + 4 *B *B;
  
	double sqrtD = Math.Sqrt(D);
	return Math.Sqrt(2 *sqrtD / (A + C + sqrtD));
  }
}

