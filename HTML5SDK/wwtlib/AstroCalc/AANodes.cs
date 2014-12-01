using System;
//
//Module : AANODES.CPP
//Purpose: Implementation for the algorithms which calculate passage thro the nodes
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


///////////////////////////// Includes ////////////////////////////////////////




//////////////////////// Includes /////////////////////////////////////////////



//////////////////////// Classes //////////////////////////////////////////////

public class  CAANodeObjectDetails
{
//Constructors / Destructors
  public CAANodeObjectDetails()
  {
	  t = 0;
	  radius = 0;
  }

//Member variables
  public double t;
  public double radius;
}

public class  CAANodes
{
//Static methods

  ///////////////////////////// Implementation //////////////////////////////////
  
  public static CAANodeObjectDetails PassageThroAscendingNode(CAAEllipticalObjectElements elements)
  {
	double v = CAACoordinateTransformation.MapTo0To360Range(-elements.w);
	v = CAACoordinateTransformation.DegreesToRadians(v);
	double E = Math.Atan(Math.Sqrt((1 - elements.e) / (1 + elements.e)) * Math.Tan(v/2))*2;
	double M = E - elements.e *Math.Sin(E);
	M = CAACoordinateTransformation.RadiansToDegrees(M);
	double n = CAAElliptical.MeanMotionFromSemiMajorAxis(elements.a);
  
	CAANodeObjectDetails details = new CAANodeObjectDetails();
	details.t = elements.T + M/n;
	details.radius = elements.a*(1 - elements.e *Math.Cos(E));
  
	return details;
  }
  public static CAANodeObjectDetails PassageThroDescendingNode(CAAEllipticalObjectElements elements)
  {
	double v = CAACoordinateTransformation.MapTo0To360Range(180 - elements.w);
	v = CAACoordinateTransformation.DegreesToRadians(v);
	double E = Math.Atan(Math.Sqrt((1 - elements.e) / (1 + elements.e)) * Math.Tan(v/2))*2;
	double M = E - elements.e *Math.Sin(E);
	M = CAACoordinateTransformation.RadiansToDegrees(M);
	double n = CAAElliptical.MeanMotionFromSemiMajorAxis(elements.a);
  
	CAANodeObjectDetails details = new CAANodeObjectDetails();
	details.t = elements.T + M/n;
	details.radius = elements.a*(1 - elements.e *Math.Cos(E));
  
	return details;
  }
  public static CAANodeObjectDetails PassageThroAscendingNode(CAAParabolicObjectElements elements)
  {
	double v = CAACoordinateTransformation.MapTo0To360Range(-elements.w);
	v = CAACoordinateTransformation.DegreesToRadians(v);
	double s = Math.Tan(v / 2);
	double s2 = s *s;
  
	CAANodeObjectDetails details = new CAANodeObjectDetails();
	details.t = elements.T + 27.403895*(s2 *s + 3 *s)*elements.q *Math.Sqrt(elements.q);
	details.radius = elements.q*(1 + s2);
  
	return details;
  }
  public static CAANodeObjectDetails PassageThroDescendingNode(CAAParabolicObjectElements elements)
  {
	double v = CAACoordinateTransformation.MapTo0To360Range(180 - elements.w);
	v = CAACoordinateTransformation.DegreesToRadians(v);
  
	double s = Math.Tan(v / 2);
	double s2 = s *s;
  
	CAANodeObjectDetails details = new CAANodeObjectDetails();
	details.t = elements.T + 27.403895*(s2 *s + 3 *s)*elements.q *Math.Sqrt(elements.q);
	details.radius = elements.q*(1 + s2);
  
	return details;
  }
}
