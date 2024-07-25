using System;
using UnityEngine;

namespace Marsion
{
    public class HandBender : MonoBehaviour
    {
        private Material lineMaterial;

        [SerializeField]
        [Tooltip("Controls the curve that the hand uses.")]
        private Vector3 curveStart = new Vector3(-2f, -0.7f, 0f), curveEnd = new Vector3(2f, -0.7f, 0f);

        #region UnityCallbacks

        // OnRenderObject는 매 프레임마다 호출되며 GL 명령어를 사용해 직접적으로 객체를 그립니다.
        private void OnRenderObject()
        {
            // 선을 그리기 위한 재질(Material)을 생성합니다.
            CreateLineMaterial();

            // Material의 첫 번째 패스(Pass)를 활성화합니다.
            lineMaterial.SetPass(0);

            // 현재 행렬을 스택에 저장합니다.
            GL.PushMatrix();
            // 현재 객체의 변환 행렬을 적용합니다.
            GL.MultMatrix(transform.localToWorldMatrix);

            // GL_LINES 모드를 시작합니다. 각 쌍의 정점들이 하나의 선분을 이룹니다.
            GL.Begin(GL.LINES);
            // 그릴 선의 색상을 파란색으로 설정합니다.
            GL.Color(Color.blue);

            DrawSphere(curveStart, 0.03f);
            DrawSphere(curveEnd, 0.03f);

            Vector3 p1 = curveStart;
            for (int i = 0; i < 19; i++)
            {
                float t = (i + 1) / 19f;
                Vector3 p2 = GetCurvePoint(curveStart, Vector3.zero, curveEnd, t);
                GL.Vertex(p1);
                GL.Vertex(p2);
                p1 = p2;
            }

            // GL_LINES 모드를 종료합니다.
            GL.End();
            // 저장된 행렬을 스택에서 꺼내서 복원합니다.
            GL.PopMatrix();

            VisualizeBend();
        }

        #endregion

        #region Operations

        public void Bend(ICardView[] cards)
        {
            if (cards == null)
                throw new ArgumentNullException("Can't bend a null card list");

            for (int i = 0; i < cards.Length; i++)
            {
                ICardView card = cards[i];

                if (!card.FSM.IsCurrent<CardViewIdle>()) continue;

                float t = (cards.Length == 1) ? 0.5f : (float)i / (cards.Length - 1);

                Vector3 cardPos = GetCurvePoint(curveStart, Vector3.zero, curveEnd, t);
                cardPos.z = i * -0.1f;
                Vector3 cardUp = GetCurveNormal(curveStart, Vector3.zero, curveEnd, t);
                Quaternion cardRot = Quaternion.LookRotation(Vector3.forward, cardUp);

                card.MoveToWithZ(cardPos, 10f);
                card.Transform.rotation = cardRot;
            }
        }

        #endregion

        #region Utils

        /// <summary>
        ///     세 개의 점을 기반으로 곡선 상의 점을 얻습니다.
        ///     Lerp(Lerp(a, b, t), Lerp(b, c, t), t)와 동일합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static Vector3 GetCurvePoint(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;

            // a, b, c로 정의된 2차 베지어 곡선의 구현과 동일
            return (oneMinusT * oneMinusT * a) + (2f * oneMinusT * t * b) + (t * t * c);
        }

        private static Vector3 GetCurveNormal(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            Vector3 tangent = GetCurveTangent(a, b, c, t);

            return Vector3.Cross(Vector3.forward, tangent);
        }

        public static Vector3 GetCurveTangent(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return 2f * (1f - t) * (b - a) + 2f * t * (c - b);
        }

        private void CreateLineMaterial()
        {
            // lineMaterial이 아직 생성되지 않은 경우에만 생성 진행
            if (!lineMaterial)
            {
                // "Hidden/Internal-Colored" 셰이더를 찾아서 사용합니다.
                Shader shader = Shader.Find("Hidden/Internal-Colored");

                lineMaterial = new Material(shader);

                // 이 Material을 숨기고 저장하지 않도록 설정합니다.
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;

                // 소스 블렌드 모드를 SrcAlpha로 설정합니다.
                lineMaterial.SetInt("_ScrBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                // 대상 블렌드 모드를 OneMinusSrcAlpha로 설정합니다.
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

                // 컬링 모드를 Off로 설정하여, 뒷면이 제거되지 않도록 합니다.
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

                // 깊이 쓰기를 비활성화합니다.
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        private void DrawSphere(Vector3 center, float radius)
        {
            float step = Mathf.PI * 0.1f;
            for (float theta = 0; theta < 2 * Mathf.PI; theta += step)
            {
                GL.Vertex(center + new Vector3(Mathf.Cos(theta) * radius, Mathf.Sin(theta) * radius, 0));
                GL.Vertex(center + new Vector3(Mathf.Cos(theta + step) * radius, Mathf.Sin(theta + step) * radius, 0));
            }
        }

        private void VisualizeBend()
        {
            //if (cards == null)
            //    throw new ArgumentNullException("Can't bend a card list null");

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.LINES);
            GL.Color(Color.red);

            for (int i = 0; i < 10; i++)
            {
                //CardView card = cards[i];

                float t = (float)i / 9;

                Vector3 cardPos = GetCurvePoint(curveStart, Vector3.zero, curveEnd, t);


                DrawSphere(cardPos, 0.03f);
            }

            GL.End();
            GL.PopMatrix();
        }

        #endregion
    }
}