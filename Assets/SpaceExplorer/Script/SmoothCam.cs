//	The MIT License (MIT)
//	
//	Copyright (c) 2014 SvenFrankson
//		
//		Permission is hereby granted, free of charge, to any person obtaining a copy
//		of this software and associated documentation files (the "Software"), to deal
//		in the Software without restriction, including without limitation the rights
//		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//		copies of the Software, and to permit persons to whom the Software is
//		furnished to do so, subject to the following conditions:
//		
//		The above copyright notice and this permission notice shall be included in all
//		copies or substantial portions of the Software.
//		
//		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//		SOFTWARE.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SpaceExplorer {

	public class SmoothCam : MonoBehaviour {

		public int delay = 5;
		public Transform target;
		private List<Vector3> lastPositions;
		private List<Quaternion> lastRotations;

		void Start () {
			this.lastPositions = new List<Vector3> ();
			this.lastRotations = new List<Quaternion> ();
		}

		void Update () {
			this.lastPositions.Add (target.position);
			this.lastRotations.Add (target.rotation);

			if (this.lastPositions.Count > delay) {
				this.lastPositions.RemoveAt (0);
			}
			if (this.lastRotations.Count > delay) {
				this.lastRotations.RemoveAt (0);
			}

			Vector3 newPos = Vector3.zero;
			Quaternion newRot = this.lastRotations[0];

			foreach (Vector3 v in this.lastPositions) {
				newPos += v;
			}
			newPos = newPos * (1f / this.lastPositions.Count);

			for (int i = 1; i < this.lastRotations.Count; i++) {
				newRot = Quaternion.Lerp (newRot, this.lastRotations[i], 0.5f);
			}

			this.transform.position = newPos;
			this.transform.rotation = newRot;
		}
	}
}
