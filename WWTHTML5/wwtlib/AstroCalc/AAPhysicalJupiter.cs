using System;
//
//Module : AAPHYSICALJUPITER.CPP
//Purpose: Implementation for the algorithms which obtain the physical parameters of the Jupiter
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


///////////////////////////////// Includes ////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAPhysicalJupiterDetails
{
//Constructors / Destructors
  public CAAPhysicalJupiterDetails()
  {
	  DE = 0;
	  DS = 0;
	  Geometricw1 = 0;
	  Geometricw2 = 0;
	  Apparentw1 = 0;
	  Apparentw2 = 0;
	  P = 0;
  }

//Member variables
  public double DE;
  public double DS;
  public double Geometricw1;
  public double Geometricw2;
  public double Apparentw1;
  public double Apparentw2;
  public double P;
}

public class  CAAPhysicalJupiter
{
//Static methods

  //////////////////////////////// Implementation ///////////////////////////////
  
  public static CAAPhysicalJupiterDetails Calculate(double JD)
  {
	//What will be the return value
	CAAPhysicalJupiterDetails details = new CAAPhysicalJupiterDetails();
  
	//Step 1
	double d = JD - 2433282.5;
	double T1 = d/36525;
	double alpha0 = 268.00 + 0.1061 *T1;
	double alpha0rad = CT.D2R(alpha0);
	double delta0 = 64.50 - 0.0164 *T1;
	double delta0rad = CT.D2R(delta0);
  
	//Step 2
	double W1 = CT.M360(17.710 + 877.90003539 *d);
	double W2 = CT.M360(16.838 + 870.27003539 *d);
  
	//Step 3
	double l0 = CAAEarth.EclipticLongitude(JD);
	double l0rad = CT.D2R(l0);
	double b0 = CAAEarth.EclipticLatitude(JD);
	double b0rad = CT.D2R(b0);
	double R = CAAEarth.RadiusVector(JD);
  
	//Step 4
	double l = CAAJupiter.EclipticLongitude(JD);
	double lrad = CT.D2R(l);
	double b = CAAJupiter.EclipticLatitude(JD);
	double brad = CT.D2R(b);
	double r = CAAJupiter.RadiusVector(JD);
  
	//Step 5
	double x = r *Math.Cos(brad)*Math.Cos(lrad) - R *Math.Cos(l0rad);
	double y = r *Math.Cos(brad)*Math.Sin(lrad) - R *Math.Sin(l0rad);
	double z = r *Math.Sin(brad) - R *Math.Sin(b0rad);
	double DELTA = Math.Sqrt(x *x + y *y + z *z);
  
	//Step 6
	l -= 0.012990 *DELTA/(r *r);
	lrad = CT.D2R(l);
  
	//Step 7
	x = r *Math.Cos(brad)*Math.Cos(lrad) - R *Math.Cos(l0rad);
	y = r *Math.Cos(brad)*Math.Sin(lrad) - R *Math.Sin(l0rad);
	z = r *Math.Sin(brad) - R *Math.Sin(b0rad);
	DELTA = Math.Sqrt(x *x + y *y + z *z);
  
	//Step 8
	double e0 = CAANutation.MeanObliquityOfEcliptic(JD);
	double e0rad = CT.D2R(e0);
  
	//Step 9
	double alphas = Math.Atan2(Math.Cos(e0rad)*Math.Sin(lrad) - Math.Sin(e0rad)*Math.Tan(brad), Math.Cos(lrad));
	double deltas = Math.Asin(Math.Cos(e0rad)*Math.Sin(brad) + Math.Sin(e0rad)*Math.Cos(brad)*Math.Sin(lrad));
  
	//Step 10
	details.DS = CT.R2D(Math.Asin(-Math.Sin(delta0rad)*Math.Sin(deltas) - Math.Cos(delta0rad)*Math.Cos(deltas)*Math.Cos(alpha0rad - alphas)));
  
	//Step 11
	double u = y *Math.Cos(e0rad) - z *Math.Sin(e0rad);
	double v = y *Math.Sin(e0rad) + z *Math.Cos(e0rad);
	double alpharad = Math.Atan2(u, x);
	double alpha = CT.R2D(alpharad);
	double deltarad = Math.Atan2(v, Math.Sqrt(x *x + u *u));
	double delta = CT.R2D(deltarad);
	double xi = Math.Atan2(Math.Sin(delta0rad)*Math.Cos(deltarad)*Math.Cos(alpha0rad - alpharad) - Math.Sin(deltarad)*Math.Cos(delta0rad), Math.Cos(deltarad)*Math.Sin(alpha0rad - alpharad));
  
	//Step 12
	details.DE = CT.R2D(Math.Asin(-Math.Sin(delta0rad)*Math.Sin(deltarad) - Math.Cos(delta0rad)*Math.Cos(deltarad)*Math.Cos(alpha0rad - alpharad)));
  
	//Step 13
	details.Geometricw1 = CT.M360(W1 - CT.R2D(xi) - 5.07033 *DELTA);
	details.Geometricw2 = CT.M360(W2 - CT.R2D(xi) - 5.02626 *DELTA);
  
	//Step 14
	double C = 57.2958 * (2 *r *DELTA + R *R - r *r - DELTA *DELTA)/(4 *r *DELTA);
	if (Math.Sin(lrad - l0rad) > 0)
	{
	  details.Apparentw1 = CT.M360(details.Geometricw1 + C);
	  details.Apparentw2 = CT.M360(details.Geometricw2 + C);
	}
	else
	{
	  details.Apparentw1 = CT.M360(details.Geometricw1 - C);
	  details.Apparentw2 = CT.M360(details.Geometricw2 - C);
	}
  
	//Step 15
	double NutationInLongitude = CAANutation.NutationInLongitude(JD);
	double NutationInObliquity = CAANutation.NutationInObliquity(JD);
	e0 += NutationInObliquity/3600;
	e0rad = CT.D2R(e0);
  
	//Step 16
	alpha += 0.005693*(Math.Cos(alpharad)*Math.Cos(l0rad)*Math.Cos(e0rad) + Math.Sin(alpharad)*Math.Sin(l0rad))/Math.Cos(deltarad);
	alpha = CT.M360(alpha);
	alpharad = CT.D2R(alpha);
	delta += 0.005693*(Math.Cos(l0rad)*Math.Cos(e0rad)*(Math.Tan(e0rad)*Math.Cos(deltarad) - Math.Sin(alpharad)*Math.Sin(deltarad)) + Math.Cos(alpharad)*Math.Sin(deltarad)*Math.Sin(l0rad));
	deltarad = CT.D2R(delta);
  
	//Step 17
	double NutationRA = CAANutation.NutationInRightAscension(alpha/15, delta, e0, NutationInLongitude, NutationInObliquity);
	double alphadash = alpha + NutationRA/3600;
	double alphadashrad = CT.D2R(alphadash);
	double NutationDec = CAANutation.NutationInDeclination(alpha/15, delta, e0, NutationInLongitude, NutationInObliquity);
	double deltadash = delta + NutationDec/3600;
	double deltadashrad = CT.D2R(deltadash);
	NutationRA = CAANutation.NutationInRightAscension(alpha0/15, delta0, e0, NutationInLongitude, NutationInObliquity);
	double alpha0dash = alpha0 + NutationRA/3600;
	double alpha0dashrad = CT.D2R(alpha0dash);
	NutationDec = CAANutation.NutationInDeclination(alpha0/15, delta0, e0, NutationInLongitude, NutationInObliquity);
	double delta0dash = delta0 + NutationDec/3600;
	double delta0dashrad = CT.D2R(delta0dash);
  
	//Step 18
	details.P = CT.M360(CT.R2D(Math.Atan2(Math.Cos(delta0dashrad)*Math.Sin(alpha0dashrad - alphadashrad), Math.Sin(delta0dashrad)*Math.Cos(deltadashrad) - Math.Cos(delta0dashrad)*Math.Sin(deltadashrad)*Math.Cos(alpha0dashrad - alphadashrad))));
  
	return details;
  }
}
