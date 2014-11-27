using System;
using wwtlib;
//
//Module : AASTELLARMAGNITUDES.CPP
//Purpose: Implementation for the algorithms which operate on the stellar magntidue system
//Created: PJN / 29-12-2003
//History: PJN / 12-02-2004 1. Fixed a number of level 4 warnings when the code is compiled in VC.Net 2003
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


////////////////////////// Includes ///////////////////////////////////////////



////////////////////// Classes ////////////////////////////////////////////////

public class  CAAStellarMagnitudes
{
//functions

  ////////////////////////// Implementation /////////////////////////////////////
  
  public static double CombinedMagnitude(double m1, double m2)
  {
	double x = 0.4*(m2 - m1);
	return m2 - 2.5 *Util.Log10(Math.Pow(10.0, x) + 1);
  }
  public static double CombinedMagnitude2(int Magnitudes, double[] pMagnitudes)
  {
	double @value = 0;
	for (int i =0; i<Magnitudes; i++)
	  @value += Math.Pow(10.0, -0.4 *pMagnitudes[i]);

    return -2.5 * Util.Log10(@value);
  }
  public static double BrightnessRatio(double m1, double m2)
  {
	double x = 0.4*(m2 - m1);
	return Math.Pow(10.0, x);
  }
  public static double MagnitudeDifference(double brightnessRatio)
  {
      return 2.5 * Util.Log10(brightnessRatio);
  }
}
