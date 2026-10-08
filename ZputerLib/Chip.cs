using System.Drawing;
using System.Numerics;

namespace ZputerLib
{
	public class Chip
	{
		public static Color wireColor = Color.FromArgb(128, 128, 128);
		public static Color connectionColor = Color.Black;
		public static Pen wire = new Pen(wireColor);
		public static Pen connection = new Pen(connectionColor);

		public static bool FastMode = false;
		int inputCounter = 0;
		public bool[] inputs;
		public bool[] outputs;

		public bool this[int index]
		{
			get
			{
				return outputs[index];
			}
			set
			{
				inputs[index] = value;
			}
		}

		public Chip(int inputCount, int outputCount)
		{
			this.inputs = new bool[inputCount];
			this.outputs = new bool[outputCount];
		}
		public virtual Chip Calc()
		{
			return this;
		}
		public virtual Chip SetInputs(params bool[] inputs)
		{
			for (int i = 0; i < inputs.Length; i++)
			{
				this.inputs[inputCounter++] = inputs[i];
				if (inputCounter >= this.inputs.Length) inputCounter = 0;
			}
			return this;
		}

		public static void DrawLine(Graphics g, Pen pen, int xOffset, int yOffset, params (int X, int Y)[] points)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				g.DrawLine(pen, xOffset + points[i].X, yOffset + points[i].Y, xOffset + points[i + 1].X, yOffset + points[i + 1].Y);
			}
		}
	}

	/// <summary>
	/// And chip. <br/>
	/// Inputs:<br/>
	/// 0-1 - in<br/>
	/// Outputs:<br/>
	/// 0
	/// </summary>
	public class And : Chip
	{
		public static Pen compPen = new Pen(Color.Red);
		public And() : base(2, 1) { }
		public override And Calc()
		{
			outputs[0] = inputs[0] && inputs[1];
			return this;
		}

		public static void Draw(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			bmp.SetPixel(xOffset, yOffset, wireColor);
			bmp.SetPixel(xOffset, yOffset + 2, wireColor);
			DrawLine(g, compPen, xOffset, yOffset, (1, 0), (1, 2));
			bmp.SetPixel(xOffset + 2, yOffset + 1, wireColor);
		}
		public static void DrawUp(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			bmp.SetPixel(xOffset + 1, yOffset, wireColor);
			DrawLine(g, compPen, xOffset, yOffset, (0, 1), (2, 1));
			bmp.SetPixel(xOffset + 2, yOffset, wireColor);
			bmp.SetPixel(xOffset + 2, yOffset + 2, wireColor);
		}
	}

	/// <summary>
	/// Or chip. <br/>
	/// Inputs:<br/>
	/// 0-1 - in <br/>
	/// Outputs:<br/>
	/// 0 - out
	/// </summary>
	public class Or : Chip
	{
		public static Pen compPen = new Pen(Color.Green);
		public Or() : base(2, 1) { }
		public override Or Calc()
		{
			outputs[0] = inputs[0] || inputs[1];
			return this;
		}
		public static void Draw(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			bmp.SetPixel(xOffset, yOffset, wireColor);
			bmp.SetPixel(xOffset, yOffset + 2, wireColor);
			DrawLine(g, compPen, xOffset, yOffset, (1, 0), (1, 2));
			bmp.SetPixel(xOffset + 2, yOffset + 1, wireColor);
		}
		public static void DrawUp(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			bmp.SetPixel(xOffset + 1, yOffset, wireColor);
			DrawLine(g, compPen, xOffset, yOffset, (0, 1), (2, 1));
			bmp.SetPixel(xOffset + 2, yOffset, wireColor);
			bmp.SetPixel(xOffset + 2, yOffset + 2, wireColor);
		}
	}

	/// <summary>
	/// Nand chip. <br/>
	/// Inputs:<br/>
	/// 0 - in <br/>
	/// Outputs:<br/>
	/// 0 - out
	/// </summary>
	public class Not : Chip
	{
		public static Pen compPen = new Pen(Color.Blue);
		public Not() : base(1, 1) { }
		public override Not Calc()
		{
			outputs[0] = !inputs[0];
			return this;
		}
		public static void Draw(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			bmp.SetPixel(xOffset, yOffset + 1, wireColor);
			DrawLine(g, compPen, xOffset, yOffset, (1, 0), (1, 2));
			bmp.SetPixel(xOffset + 2, yOffset + 1, wireColor);
		}
		public static void DrawUp(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			bmp.SetPixel(xOffset + 1, yOffset, wireColor);
			DrawLine(g, compPen, xOffset, yOffset, (0, 1), (2, 1));
			bmp.SetPixel(xOffset + 1, yOffset + 2, wireColor);
		}
	}

	/// <summary>
	/// Nand chip. <br/>
	/// Inputs:<br/>
	/// 0-1 - in <br/>
	/// Outputs:<br/>
	/// 0 - out
	/// </summary>
	public class Nand : Chip
	{
		And and;
		Not not;

		public Nand() : base(2, 1) { }
		public override Nand Calc()
		{
			if (FastMode)
			{
				and = null;
				not = null;
				outputs[0] = !(inputs[0] && inputs[1]);
				return this;
			}

			if (and == null)
			{
				and = new();
				not = new();
			}
			and.SetInputs(inputs).Calc();
			not.SetInputs(and[0]).Calc();
			outputs[0] = not[0];
			return this;
		}

		public static void Draw(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			And.Draw(bmp, g, xOffset, yOffset);
			Not.Draw(bmp, g, xOffset + 2, yOffset);
		}
		public static void DrawUp(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			Not.DrawUp(bmp, g, xOffset, yOffset);
			And.DrawUp(bmp, g, xOffset, yOffset + 3);

		}
	}
	
	/// <summary>
	/// Xor chip. <br/>
	/// Inputs:<br/>
	/// 0-1 - in <br/>
	/// Outputs:<br/>
	/// 0 - out
	/// </summary>
	public class Xor : Chip
	{
		Or or;
		Nand nand;
		And and;

		public Xor() : base(2, 1) { }
		public override Xor Calc()
		{
			if (FastMode)
			{
				or = null;
				nand = null;
				and = null;
				outputs[0] = inputs[0] ^ inputs[1];
				return this;
			}

			if (or == null)
			{
				or = new();
				nand = new();
				and = new();
			}
			or.SetInputs(inputs).Calc();
			nand.SetInputs(inputs).Calc();
			and.SetInputs(or[0], nand[0]).Calc();
			outputs[0] = and[0];
			return this;
		}
		public static void Draw(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			And.Draw(bmp, g, xOffset + 4, yOffset);
			Nand.Draw(bmp, g, xOffset + 4, yOffset + 4);
			Or.Draw(bmp, g, xOffset + 10, yOffset + 2);
			DrawLine(g, wire, xOffset, yOffset, (7, 1), (9, 1), (9, 2));
			DrawLine(g, wire, xOffset, yOffset, (9, 4), (9, 5));
			DrawLine(g, wire, xOffset, yOffset, (0, 6), (3, 6), (3, 2));
			DrawLine(g, wire, xOffset, yOffset, (0, 0), (3, 0));
			DrawLine(g, wire, xOffset, yOffset, (1, 0), (1, 4), (3, 4));
			bmp.SetPixel(xOffset + 1, yOffset, connectionColor);
			bmp.SetPixel(xOffset + 3, yOffset + 6, connectionColor);
		}
	}

	/// <summary>
	/// Mux chip. <br/>
	/// Inputs:<br/>
	/// 0-1 - in <br/>
	///	2 - sel <br/>
	/// Outputs:<br/>
	/// 0 - out
	/// </summary>
	public class Mux : Chip
	{
		And and0;
		And and1;
		Not not;
		Or or;

		public Mux() : base(3, 1) { }
		public override Chip Calc()
		{
			if (FastMode)
			{
				and0 = null;
				and1 = null;
				not = null;
				or = null;

				outputs[0] = inputs[inputs[2] ? 1 : 0];
				return this;
			}

			if (and0 == null)
			{
				and0 = new();
				and1 = new();
				not = new();
				or = new();
			}

			not.SetInputs(inputs[2]).Calc();
			and0.SetInputs(inputs[0], not[0]).Calc();
			and1.SetInputs(inputs[1], inputs[2]).Calc();
			or.SetInputs(and0[0], and1[0]).Calc();
			outputs[0] = or[0];
			return this;
		}

		public static void Draw(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			And.Draw(bmp, g, xOffset + 4, yOffset);
			And.Draw(bmp, g, xOffset + 4, yOffset + 7);
			Not.DrawUp(bmp, g, xOffset + 1, yOffset + 3);
			Or.Draw(bmp, g, xOffset + 7, yOffset + 3);

			DrawLine(g, wire, xOffset, yOffset, (0, 0), (3, 0));
			DrawLine(g, wire, xOffset, yOffset, (2, 2), (3, 2));
			DrawLine(g, wire, xOffset, yOffset, (0, 7), (3, 7));
			DrawLine(g, wire, xOffset, yOffset, (0, 9), (3, 9));
			DrawLine(g, wire, xOffset, yOffset, (2, 9), (2, 6));
			DrawLine(g, wire, xOffset, yOffset, (7, 1), (7, 2));
			DrawLine(g, wire, xOffset, yOffset, (7, 6), (7, 8));
			bmp.SetPixel(xOffset + 2, yOffset + 9, connectionColor);
		}
	}

	/// <summary>
	/// Mux chip. <br/>
	/// Inputs:<br/>
	/// 0 - in <br/>
	///	1 - sel <br/>
	/// Outputs:<br/>
	/// 0-1 - out
	/// </summary>
	public class DMux : Chip
	{
		Not not;
		And and0;
		And and1;

		public DMux() : base(2, 2) { }
		public override DMux Calc()
		{
			if (FastMode)
			{
				not = null;
				and0 = null;
				and1 = null;
				outputs[0] = inputs[0] && !inputs[1];
				outputs[1] = inputs[0] && inputs[1];
				return this;
			}

			if (not == null)
			{
				not = new();
				and0 = new();
				and1 = new();
			}

			not.SetInputs(inputs[1]).Calc();
			and0.SetInputs(inputs[0], not[0]).Calc();
			and1.SetInputs(inputs[0], inputs[1]).Calc();
			outputs[0] = and0[0];
			outputs[1] = and1[0];
			return this;
		}

		public static void Draw(Bitmap bmp, Graphics g, int xOffset, int yOffset)
		{
			Not.Draw(bmp, g, xOffset + 2, yOffset);
			And.Draw(bmp, g, xOffset + 5, yOffset + 2);
			And.Draw(bmp, g, xOffset + 5, yOffset + 6);

			DrawLine(g, wire, xOffset, yOffset, (0, 1), (1, 1), (1, 6), (4, 6));
			DrawLine(g, wire, xOffset, yOffset, (0, 4), (4, 4));
			DrawLine(g, wire, xOffset, yOffset, (3, 4), (3, 8), (4,8));
			bmp.SetPixel(xOffset + 5, yOffset + 1, wireColor);
			bmp.SetPixel(xOffset + 1, yOffset + 1, connectionColor);
			bmp.SetPixel(xOffset + 3, yOffset + 4, connectionColor);
		}
	}

	/// <summary>
	/// Mux chip. <br/>
	/// Inputs:<br/>
	/// 0-2 - sum <br/>
	/// Outputs:<br/>
	/// 0 - sum <br/>
	///	1 - carry
	/// </summary>
	public class Add : Chip
	{
		private Xor xor;
		private And and;
		private Or or;
		private Not not;
		private Mux mux0;
		private Mux mux1;
		
		public Add() : base(3,2) { }

		public override Add Calc()
		{
			if (FastMode)
			{
				xor = null;
				and = null;
				or = null;
				not = null;
				mux0 = null;
				mux1 = null;
				
				int sum = 0;
				for (int i = 0; i < 3; i++) sum += inputs[i] ? 1 : 0;
				outputs[1] = sum >= 2;
				outputs[0] = sum % 2 == 1;
				return this;
			}

			if (xor == null)
			{
				xor = new();
				and = new ();
				or = new();
				not = new();
				mux0 = new();
				mux1 = new();
			}

			xor.SetInputs(inputs[0], inputs[1]).Calc();
			not.SetInputs(xor[0]).Calc();
			and.SetInputs(inputs[1], inputs[2]).Calc();
			or.SetInputs(inputs[1], inputs[2]).Calc();
			mux0.SetInputs(xor[0], not[0]).Calc();
			mux1.SetInputs(and[0], or[0]).Calc();
			outputs[0] = mux0[0];
			outputs[1] = mux1[0];
			return this;
		}
	}
}
