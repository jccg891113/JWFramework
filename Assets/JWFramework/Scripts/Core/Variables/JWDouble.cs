using UnityEngine;
using System;
using System.Collections;

public class JWDouble : IComparable<double>, IEquatable<double>
{
	/// <summary>
	/// 实部
	/// </summary>
	protected long multiple;
	/// <summary>
	/// 分子
	/// </summary>
	protected long numerator;
	/// <summary>
	/// 分母
	/// </summary>
	protected long denominator;

	protected double tmpValue{ get { return multiple + (double)numerator / (float)denominator; } }

	private JWDouble ()
	{
	}

	public JWDouble (double Value)
	{
		TranslateDouble (Value, out multiple, out numerator, out denominator);
	}

	public JWDouble (long numerator, long denominator)
	{
		TranslateDouble (numerator, denominator, out multiple, out this.numerator, out this.denominator);
	}

	public JWDouble (long multiple, long numerator, long denominator)
	{
		TranslateDouble (numerator + multiple * denominator, denominator, out this.multiple, out this.numerator, out this.denominator);
	}

	public int CompareTo (double other)
	{
		return tmpValue.CompareTo (other);
	}

	public bool Equals (double other)
	{
		return tmpValue.Equals (other);
	}

	public override bool Equals (object obj)
	{
		return tmpValue.Equals (obj);
	}

	public override string ToString ()
	{
		return string.Format ("Value is {0}, multiple is {1}, numerator is {2}, denominator is {3}", tmpValue, multiple, numerator, denominator);
	}

	/// 
	/// Operator Method
	/// 
	
	public static implicit operator JWDouble (double value)
	{
		return new JWDouble (value);
	}

	public static JWDouble operator + (JWDouble cValue, int nValue)
	{
		return new JWDouble (nValue + cValue.multiple, cValue.numerator, cValue.denominator);
	}

	public static JWDouble operator + (int nValue, JWDouble cValue)
	{
		return cValue + nValue;
	}

	public static JWDouble operator + (JWDouble cValue, long nValue)
	{
		return new JWDouble (nValue + cValue.multiple, cValue.numerator, cValue.denominator);
	}

	public static JWDouble operator + (long nValue, JWDouble cValue)
	{
		return cValue + nValue;
	}

	public static JWDouble operator + (JWDouble left, JWDouble right)
	{
		long baseMultiple = left.multiple + right.multiple;
		
		long baseNumerator = left.numerator * right.denominator + right.numerator * left.denominator;
		
		long baseDenominator = left.denominator * right.denominator;
		
		return new JWDouble (baseMultiple, baseNumerator, baseDenominator);
	}

	public static JWDouble operator - (JWDouble cValue, int nValue)
	{
		return new JWDouble (cValue.multiple - nValue, cValue.numerator, cValue.denominator);
	}

	public static JWDouble operator - (int nValue, JWDouble cValue)
	{
		return new JWDouble (nValue - cValue.multiple, -cValue.numerator, cValue.denominator);
	}

	public static JWDouble operator - (JWDouble cValue, long nValue)
	{
		return new JWDouble (cValue.multiple - nValue, cValue.numerator, cValue.denominator);
	}

	public static JWDouble operator - (long nValue, JWDouble cValue)
	{
		return new JWDouble (nValue - cValue.multiple, -cValue.numerator, cValue.denominator);
	}

	public static JWDouble operator - (JWDouble left, JWDouble right)
	{
		long baseMultiple = left.multiple - right.multiple;

		long baseNumerator = left.numerator * right.denominator - right.numerator * left.denominator;

		long baseDenominator = left.denominator * right.denominator;

		return new JWDouble (baseMultiple, baseNumerator, baseDenominator);
	}

	public static JWDouble operator * (JWDouble cValue, int nValue)
	{
		return new JWDouble (cValue.multiple * nValue, cValue.numerator * nValue, cValue.denominator);
	}

	public static JWDouble operator * (int nValue, JWDouble cValue)
	{
		return cValue * nValue;
	}

	public static JWDouble operator * (JWDouble cValue, long nValue)
	{
		return new JWDouble (cValue.multiple * nValue, cValue.numerator * nValue, cValue.denominator);
	}

	public static JWDouble operator * (long nValue, JWDouble cValue)
	{
		return cValue * nValue;
	}

	public static JWDouble operator * (JWDouble left, JWDouble right)
	{
		long aa = left.multiple * right.multiple;
		JWDouble acb1 = new JWDouble (left.multiple * right.numerator, right.denominator);
		JWDouble acb2 = new JWDouble (right.multiple * left.numerator, left.denominator);
		JWDouble ccbb = new JWDouble (left.numerator * right.numerator, left.denominator * right.denominator);
		return (aa + acb1) + (acb2 + ccbb);
	}

	public static JWDouble operator / (JWDouble cValue, int nValue)
	{
		return new JWDouble (cValue.multiple * cValue.denominator + cValue.numerator, cValue.denominator * nValue);
	}

	public static JWDouble operator / (int nValue, JWDouble cValue)
	{
		return new JWDouble (cValue.denominator * nValue, cValue.multiple * cValue.denominator + cValue.numerator);
	}

	public static JWDouble operator / (JWDouble cValue, long nValue)
	{
		return new JWDouble (cValue.multiple * cValue.denominator + cValue.numerator, cValue.denominator * nValue);
	}

	public static JWDouble operator / (long nValue, JWDouble cValue)
	{
		return new JWDouble (cValue.denominator * nValue, cValue.multiple * cValue.denominator + cValue.numerator);
	}

	public static JWDouble operator / (JWDouble left, JWDouble right)
	{
		long baseNumerator = (left.multiple * left.denominator + left.numerator) * right.denominator;

		long baseDenominator = (right.multiple * right.denominator + right.numerator) * left.denominator;
		
		return new JWDouble (baseNumerator, baseDenominator);
	}

	public static void TranslateDouble (double num, out long _multiple, out long _numerator, out long _denominator)
	{
		bool positive = true;
		if (num < 0) {
			num = -num;
			positive = false;
		}
		_numerator = 0;
		_denominator = 1;
		//强行拆分小数分子与分母
		string numStr = num.ToString ();
		string[] numPart = numStr.Split (new char[]{ '.' });
		_multiple = long.Parse (numPart [0]);
		if (numPart.Length > 1) {
			if (numPart [1].Length > 10) {
				numPart [1] = numPart [1].Substring (0, 10);
			}
			_numerator = long.Parse (numPart [1]);
			_denominator *= (long)Math.Pow (10, numPart [1].Length);
		}
		StreamlineDouble (_numerator, _denominator, out _numerator, out _denominator);
		//转变符号
		if (!positive) {
			_multiple = -_multiple;
			_numerator = -_numerator;
		}
	}

	public static void TranslateDouble (long i_numerator, long i_denominator, out long _multiple, out long _numerator, out long _denominator)
	{
		_multiple = i_numerator / i_denominator;
		bool positive = true;
		_numerator = i_numerator - _multiple * i_denominator;
		if (_numerator < 0) {
			_numerator = -_numerator;
			positive = false;
		}
		_denominator = i_denominator;
		StreamlineDouble (_numerator, _denominator, out _numerator, out _denominator);
		//转变符号
		if (!positive) {
			_numerator = -_numerator;
		}
	}

	public static void StreamlineDouble (long i_numerator, long i_denominator, out long o_numerator, out long o_denominator)
	{
		//求取分子分母最大公约数
		long gr = GreatestCommonDivisor (i_numerator, i_denominator);
		//分子分母根据最大公约数缩放
		o_numerator = i_numerator / gr;
		o_denominator = i_denominator / gr;
	}

	public static long GreatestCommonDivisor (long num1, long num2)
	{
		long p = num1;
		long q = num2;
		long gr;
		while (true) {
			if (p % q == 0) {
				gr = q;
				break;
			} else {
				long r = p % q;
				p = q;
				q = r;
			}
		}
		return gr;
	}
}
