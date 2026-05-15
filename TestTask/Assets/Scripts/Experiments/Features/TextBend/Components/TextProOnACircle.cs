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

using UnityEngine;
using System.Collections;
using TMPro;

namespace ntw.CurvedTextMeshPro
{
    /// <summary>
    /// Class for drawing a Text Pro text following a circle arc
    /// </summary>
    [ExecuteInEditMode]
    public class TextProOnACircle : TextProOnACurve
    {
        /// <summary>
        /// The radius of the text circle arc
        /// </summary>
        [SerializeField] [Tooltip("The radius of the text circle arc")]
        protected float m_radius = 10.0f;

        /// <summary>
        /// How much degrees the text arc should span
        /// </summary>
        [SerializeField] [Tooltip("How much degrees the text arc should span")]
        protected float m_arcDegrees = 20f;

        /// <summary>
        /// The angular offset at which the arc should be centered, in degrees.
        /// </summary>
        [SerializeField] [Tooltip("The angular offset at which the arc should be centered, in degrees")]
        protected float m_angularOffset = -13f;

        /// <summary>
        /// Previous value of <see cref="m_radius"/>
        /// </summary>
        private float m_oldRadius = float.MaxValue;

        /// <summary>
        /// Previous value of <see cref="m_arcDegrees"/>
        /// </summary>
        private float m_oldArcDegrees = float.MaxValue;

        /// <summary>
        /// Previous value of <see cref="m_angularOffset"/>
        /// </summary>
        private float m_oldAngularOffset = float.MaxValue;

        /// <summary>
        /// Previous value of <see cref="m_maxDegreesPerLetter"/>
        /// </summary>
        private float m_oldMaxDegreesPerLetter = float.MaxValue;

        /// <summary>
        /// Update
        /// </summary>
        protected void Update()
        {
            //if the text and the parameters are the same of the old frame, don't waste time in re-computing everything
            if (!m_forceUpdate && !m_TextComponent.havePropertiesChanged && !ParametersHaveChanged())
            {
                return;
            }

            m_forceUpdate = false;

            //during the loop, vertices represents the 4 vertices of a single character we're analyzing, 
            //while matrix is the roto-translation matrix that will rotate and scale the characters so that they will
            //follow the curve
            Vector3[] vertices;

            //Generate the mesh and get information about the text and the characters
            m_TextComponent.ForceMeshUpdate();

            TMP_TextInfo textInfo = m_TextComponent.textInfo;
            int characterCount = textInfo.characterCount;

            //if the string is empty, no need to waste time
            if (characterCount == 0)
                return;

            //gets the bounds of the rectangle that contains the text 
            float boundsMinX = m_TextComponent.bounds.min.y;
            float boundsMaxX = m_TextComponent.bounds.max.y;

            //for each character
            for (int i = 0; i < characterCount; i++)
            {
                //skip if it is invisible
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                //Get the index of the mesh used by this character, then the one of the material... and use all this data to get
                //the 4 vertices of the rect that encloses this character. Store them in vertices
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                vertices = textInfo.meshInfo[materialIndex].vertices;

                vertices[vertexIndex + 0] = GetNewVerticesPosition(vertices[vertexIndex + 0]);
                vertices[vertexIndex + 1] = GetNewVerticesPosition(vertices[vertexIndex + 1]);
                vertices[vertexIndex + 2] = GetNewVerticesPosition(vertices[vertexIndex + 2]);
                vertices[vertexIndex + 3] = GetNewVerticesPosition(vertices[vertexIndex + 3]);
            }

            //Upload the mesh with the revised information
            m_TextComponent.UpdateVertexData();
        }

        /// <summary>
        /// Get new vertices position rely to circle center
        /// </summary>
        protected override Vector3 GetNewVerticesPosition(Vector3 position)
        {
            float ratio = position.x / m_arcDegrees + m_angularOffset * Mathf.Deg2Rad;
            float mappedRatio = ratio * 2 * Mathf.PI;
            float cos = Mathf.Cos(mappedRatio);
            float sin = Mathf.Sin(mappedRatio);

            position.x = cos * m_radius * 2;
            position.z = sin * m_radius * 2;

            return position;
        }

        /// <summary>
        /// Method executed at every frame that checks if some parameters have been changed
        /// </summary>
        /// <returns></returns>
        protected override bool ParametersHaveChanged()
        {
            //check if paramters have changed and update the old values for next frame iteration
            bool retVal = m_radius != m_oldRadius || m_arcDegrees != m_oldArcDegrees ||
                          m_angularOffset != m_oldAngularOffset;

            m_oldRadius = m_radius;
            m_oldArcDegrees = m_arcDegrees;
            m_oldAngularOffset = m_angularOffset;

            return retVal;
        }
    }
}