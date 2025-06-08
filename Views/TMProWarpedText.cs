namespace UniGame.UI
{
    using System;
    using R3;
    using TMPro;
    using Core.Runtime;
     
    using UnityEngine;

    public class TMProWarpedText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private bool _rebuildOnTextUpdate = true;
        
        public AnimationCurve VertexCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 2.0f), new Keyframe(0.5f, 0), new Keyframe(0.75f, 2.0f), new Keyframe(1, 0f));
        public float          CurveScale  = 1.0f;

        private void Awake()
        {
            if (!_rebuildOnTextUpdate) return;
            
            var lifeTime = this.GetLifeTime();

            Observable.EveryValueChanged(_text, x => x.font)
                .Debounce(TimeSpan.FromMilliseconds(50))
                .Subscribe(x => WarpText())
                .AddTo(lifeTime);

            Observable.EveryValueChanged(_text, x => x.text)
                .Debounce(TimeSpan.FromMilliseconds(50))
                .Subscribe(x => WarpText())
                .AddTo(lifeTime);
            
            Observable.EveryValueChanged(_text, x => x.color)
                .Subscribe(x => WarpText())
                .AddTo(lifeTime);
        }

        private void OnEnable()
        {
            WarpText();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
        #endif
        private void WarpText()
        {
            VertexCurve.preWrapMode = WrapMode.Clamp;
            VertexCurve.postWrapMode = WrapMode.Clamp;

            _text.havePropertiesChanged = true;

            _text.ForceMeshUpdate();

            var textInfo       = _text.textInfo;
            var characterCount = textInfo.characterCount;

            if (characterCount == 0) return;

            var boundsMinX = _text.bounds.min.x;
            var boundsMaxX = _text.bounds.max.x;

            for (var i = 0; i < characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                var vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the index of the mesh used by this character.
                var materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                var vertices = textInfo.meshInfo[materialIndex].vertices;

                // Compute the baseline mid point for each character
                Vector3 offsetToMidBaseline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);

                // Apply offset to adjust our pivot point.
                vertices[vertexIndex + 0] -= offsetToMidBaseline;
                vertices[vertexIndex + 1] -= offsetToMidBaseline;
                vertices[vertexIndex + 2] -= offsetToMidBaseline;
                vertices[vertexIndex + 3] -= offsetToMidBaseline;

                // Compute the angle of rotation for each character based on the animation curve
                var x0 = (offsetToMidBaseline.x - boundsMinX) / (boundsMaxX - boundsMinX); // Character's position relative to the bounds of the mesh.
                var x1 = x0 + 0.0001f;
                var y0 = VertexCurve.Evaluate(x0) * CurveScale * 10;
                var y1 = VertexCurve.Evaluate(x1) * CurveScale * 10;

                var horizontal = new Vector3(1, 0, 0);
                var tangent = new Vector3(x1 * (boundsMaxX - boundsMinX) + boundsMinX, y1) - new Vector3(offsetToMidBaseline.x, y0);

                var dot = Mathf.Acos(Vector3.Dot(horizontal, tangent.normalized)) * 57.2957795f;
                var cross = Vector3.Cross(horizontal, tangent);
                var angle = cross.z > 0 ? dot : 360 - dot;

                var matrix = Matrix4x4.TRS(new Vector3(0, y0, 0), Quaternion.Euler(0, 0, angle), Vector3.one);

                vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
                vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
                vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
                vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

                vertices[vertexIndex + 0] += offsetToMidBaseline;
                vertices[vertexIndex + 1] += offsetToMidBaseline;
                vertices[vertexIndex + 2] += offsetToMidBaseline;
                vertices[vertexIndex + 3] += offsetToMidBaseline;
            }

            _text.UpdateVertexData();
        }
    }
}