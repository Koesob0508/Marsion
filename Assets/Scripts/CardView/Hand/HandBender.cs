using System;
using UnityEngine;

namespace Marsion.CardView
{
    public class HandBender : MonoBehaviour
    {
        private Material lineMaterial;
        [SerializeField] bool IsMine;

        [SerializeField]
        [Tooltip("Controls the curve that the hand uses.")]
        private Vector3 CurveStart = new Vector3(-2f, -0.7f, 0f), CurveEnd = new Vector3(2f, -0.7f, 0f);
        [SerializeField]
        float Height;

        #region UnityCallbacks

        private void OnDrawGizmos()
        {
            // 그릴 선의 색상을 파란색으로 설정합니다.
            Gizmos.color = Color.red;


            Vector3 curveStart = Vector3.zero;
            Vector3 curveEnd = Vector3.zero;
            Vector3 height = Vector3.zero;

            if (IsMine)
            {
                curveStart = CurveStart + transform.position;
                curveEnd = CurveEnd + transform.position;
                height = new Vector3(0f, Height, 0f) + transform.position;
            }
            else
            {
                curveStart = transform.position - CurveStart;
                curveEnd = transform.position - CurveEnd;
                height = new Vector3(0f, -Height, 0f) + transform.position;
            }
                
            // 시작점과 끝점을 나타내는 스피어를 그립니다.
            Gizmos.DrawSphere(curveStart, 0.03f);
            Gizmos.DrawSphere(curveEnd, 0.03f);

            Vector3 p1 = curveStart;
            for (int i = 0; i < 19; i++)
            {
                float t = (i + 1) / 19f;
                Vector3 p2 = Vector3.zero;
                p2 = GetCurvePoint(curveStart, height, curveEnd, t);

                // 두 점 사이에 선을 그립니다.
                Gizmos.DrawLine(p1, p2);

                p1 = p2;
            }
        }

        // 접선을 그리는 함수를 Gizmos용으로 만듭니다.
        private void DrawTangentAtPointGizmos(Vector3 point, float t, float length)
        {
            Vector3 tangent = GetCurveTangent(CurveStart, Vector3.zero, CurveEnd, t);
            Gizmos.color = Color.red; // 예: 접선의 색상은 빨간색으로 설정
            Gizmos.DrawLine(point, point + tangent.normalized * length);
        }

        // 법선을 그리는 함수를 Gizmos용으로 만듭니다.
        private void DrawNormalAtPointGizmos(Vector3 point, float t, float length)
        {
            Vector3 normal = GetCurveNormal(CurveStart, Vector3.zero, CurveEnd, t, true);
            Gizmos.color = Color.green; // 예: 법선의 색상은 초록색으로 설정
            Gizmos.DrawLine(point, point + normal.normalized * length);
        }

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

            DrawSphere(CurveStart, 0.03f);
            DrawSphere(CurveEnd, 0.03f);

            Vector3 p1 = CurveStart;
            for (int i = 0; i < 19; i++)
            {
                float t = (i + 1) / 19f;
                Vector3 p2 = GetCurvePoint(CurveStart, new Vector3(0f, Height, 0f), CurveEnd, t);
                GL.Vertex(p1);
                GL.Vertex(p2);

                DrawTangentAtPoint(p2, t, 0.1f);
                DrawNormalAtPoint(p2, t, 0.1f); // 여기서 0.1f는 Normal 선의 길이

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

            float[] objLerps = new float[cards.Length];

            switch (cards.Length)
            {
                case 1: objLerps = new float[] { 0.5f }; break;
                case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
                case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
                default:
                    float interval = 1f / (cards.Length - 1);
                    for (int i = 0; i < cards.Length; i++)
                        objLerps[i] = interval * i;
                    break;
            }

            for (int i = 0; i < cards.Length; i++)
            {
                ICardView card = cards[i];

                if (!card.FSM.IsCurrent<CardViewIdle>()) continue;

                var cardPos = GetCurvePoint(CurveStart, new Vector3(0f, Height, 0f), CurveEnd, objLerps[i]);
                cardPos.z = -1;
                var cardRot = Quaternion.identity;

                if (cards.Length >= 4)
                {
                    Vector3 cardUp;

                    if (Managers.Client.ID == card.Card.PlayerID)
                        cardUp = GetCurveNormal(CurveStart, new Vector3(0f, Height, 0f), CurveEnd, objLerps[i], true);
                    else
                        cardUp = GetCurveNormal(CurveStart, new Vector3(0f, Height, 0f), CurveEnd, objLerps[i], false);

                    cardRot = Quaternion.LookRotation(Vector3.forward, cardUp);
                }

                card.MoveToWithZ(cardPos, 10f);
                card.Transform.localRotation = cardRot;
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

        private static Vector3 GetCurveNormal(Vector3 a, Vector3 b, Vector3 c, float t, bool isMine)
        {
            Vector3 tangent = GetCurveTangent(a, b, c, t);

            if (isMine)
                return Vector3.Cross(Vector3.forward, tangent);
            else
                return Vector3.Cross(Vector3.back, tangent);
        }

        private static Vector3 GetCurveTangent(Vector3 a, Vector3 b, Vector3 c, float t)
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

        private void DrawTangentAtPoint(Vector3 point, float t, float length)
        {
            // 곡선의 현재 점에서 Tangent 벡터를 계산합니다.
            Vector3 tangent = GetCurveTangent(CurveStart, new Vector3(0f, Height, 0f), CurveEnd, t).normalized;

            // Tangent 벡터 방향으로 선을 그립니다.
            GL.Color(Color.green);  // Tangent 선을 녹색으로 설정
            GL.Vertex(point);
            GL.Vertex(point + tangent * length);
        }

        private void DrawNormalAtPoint(Vector3 point, float t, float length)
        {
            // 곡선의 현재 점에서 Normal 벡터를 계산합니다.
            Vector3 normal = GetCurveNormal(CurveStart, new Vector3(0f, Height, 0f), CurveEnd, t, GetComponent<HandView>().IsMine).normalized;

            // Normal 벡터 방향으로 선을 그립니다.
            GL.Vertex(point);
            GL.Vertex(point + normal * length);
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

                Vector3 cardPos = GetCurvePoint(CurveStart, new Vector3(0f, Height, 0f), CurveEnd, t);


                DrawSphere(cardPos, 0.03f);
            }

            GL.End();
            GL.PopMatrix();
        }

        #endregion
    }
}