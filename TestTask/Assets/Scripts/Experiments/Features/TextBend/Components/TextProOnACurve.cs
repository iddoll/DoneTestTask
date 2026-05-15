//MIT License

//Copyright(c) 2019 Antony Vitillo(a.k.a. "Skarredghost")

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

//Code inspired by the one of the WarpTextExample of TextMeshPro package

using UnityEngine;
using System.Collections;
using TMPro;
using System;

namespace ntw.CurvedTextMeshPro
{
    /// <summary>
    /// Base class for drawing a Text Pro text following a particular curve
    /// </summary>
    [ExecuteInEditMode]
    public abstract class TextProOnACurve : MonoBehaviour
    {
        /// <summary>
        /// The text component of interest
        /// </summary>
        protected TMP_Text m_TextComponent;

        /// <summary>
        /// True if the text must be updated at this frame 
        /// </summary>
        protected bool m_forceUpdate;

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();
        }

        /// <summary>
        /// OnEnable
        /// </summary>
        private void OnEnable()
        {
            //every time the object gets enabled, we have to force a re-creation of the text mesh
            m_forceUpdate = true;
        }

        void OnDrawGizmosSelected()
        {
            // Draw a red sphere at the transform's position
            Gizmos.color = Color.red;
            Vector3 position = transform.position;
            position.z = position.z;
            Gizmos.DrawSphere(position, 0.1f);
        }

        /// <summary>
        /// Method change of the vertices position to match the curve
        /// </summary>
        /// <returns></returns>
        protected abstract Vector3 GetNewVerticesPosition(Vector3 position);

        /// <summary>
        /// Method executed at every frame that checks if some parameters have been changed
        /// </summary>
        /// <returns></returns>
        protected abstract bool ParametersHaveChanged();
    }
}