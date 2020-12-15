using System;
using Audio;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

namespace Audio
{
	/// <summary>
	///  Attribute to work in junction with the <see cref="MinMaxFloat"/>
	/// </summary>
	public class MinMaxFloatAttribute : PropertyAttribute
	{
		public MinMaxFloatAttribute(float min, float max)
		{
			this.Min = min;
			this.Max = max;
		}

		public float Min;
		public float Max;
	}
}
