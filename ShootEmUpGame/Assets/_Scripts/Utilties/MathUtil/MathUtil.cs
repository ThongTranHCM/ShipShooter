using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MathUtil{
    public class ColorMath{

        public static Vector4 RGB2YIQ(Color src){
            Matrix4x4 _RGB2YIQMatrix = new Matrix4x4();
            _RGB2YIQMatrix.SetRow(0, new Vector4(0.299f, 0.587f, 0.114f, 0.0f));
            _RGB2YIQMatrix.SetRow(1, new Vector4(0.596f, -0.275f, -0.321f, 0.0f));
            _RGB2YIQMatrix.SetRow(2, new Vector4(0.212f, -0.523f, 0.311f, 0.0f));
            _RGB2YIQMatrix.SetRow(3, new Vector4(0,0,0,1));
            Vector4 vec = new Vector4(src.r,src.g,src.b,src.a);
            return _RGB2YIQMatrix * vec;
        }

        public static Color YIQ2RGB(Vector4 src){
            Matrix4x4 _YIQ2RGBMatrix = new Matrix4x4();
            _YIQ2RGBMatrix.SetRow(0, new Vector4(1.0f, 0.956f, 0.621f, 0.0f));
            _YIQ2RGBMatrix.SetRow(1, new Vector4(1.0f, -0.272f, -0.647f, 0.0f));
            _YIQ2RGBMatrix.SetRow(2, new Vector4(1.0f, -1.107f, 1.704f, 0.0f));
            _YIQ2RGBMatrix.SetRow(3, new Vector4(0,0,0,1));
            Vector4 res = _YIQ2RGBMatrix * src;
            return new Color(res.x,res.y,res.z,res.w);
        }

        public static Color HueShift(Color main, Color sub, float shift_amount, float lumi_scale){
            Vector4 YIQ_main = RGB2YIQ(main);
			Vector4 YIQ_sub = RGB2YIQ(sub);
            float cos_main = YIQ_main.y;
			float sin_main = YIQ_main.z;
			float cos_sub = YIQ_sub.y;
			float sin_sub = YIQ_sub.z;
			float sin_shift = sin_main * cos_sub - sin_sub * cos_main;
			float cos_shift = cos_main * cos_sub - sin_main * sin_sub;
			float shift = Mathf.Sign(Mathf.Atan2(sin_shift, cos_shift)) * 3.1416f;
			shift *= shift_amount;
			float hue = Mathf.Atan2(sin_main, cos_main);
			hue -= shift;
            float chroma = Mathf.Sqrt(YIQ_main.z * YIQ_main.z + YIQ_main.y * YIQ_main.y);
					
			float lumi = YIQ_main.x;
			lumi += lumi_scale;
			lumi = Mathf.Max(0,Mathf.Min(lumi, 1));

			//chroma correction
					
			float Q = Mathf.Sin(hue);
			float I = Mathf.Cos(hue);
			float R = 0.956f * I + 0.621f * Q;
			float G = -0.272f * I - 0.647f * Q;
			float B = -1.107f * I + 1.704f * Q;
					
			chroma = Mathf.Min(chroma, Mathf.Max((1 - lumi)/R,-lumi/R));
			chroma = Mathf.Min(chroma, Mathf.Max((1 - lumi)/G,-lumi/G));
			chroma = Mathf.Min(chroma, Mathf.Max((1 - lumi)/B,-lumi/B));

			Q *= chroma;
			I *= chroma;

			return YIQ2RGB(new Vector4(lumi,I,Q,0));
        }

        public static float Value(Color src){
            return src.r * 0.299f + src.g * 0.587f + src.b * 0.114f;
        }

        public static float Chroma(Color src){
            Vector4 yiq = RGB2YIQ(src);
            return Mathf.Sqrt(yiq.y * yiq.y + yiq.z * yiq.z);
        }
    }

    public class LowPassFilter : MonoBehaviour
    {
        private float _damping;
        private float _value;
        private float _ref;
        public LowPassFilter(float damping, float value = 0){
            _damping = damping;
            _value = value;
        }

        public void Input(float input){
            float alpha = Time.unscaledDeltaTime / (Time.fixedUnscaledDeltaTime + _damping);
            _value = _value + alpha * (input - _value);
        }

        public float Output (){
            return _value;
        }

        public float GetAlpha (){
            return Time.unscaledDeltaTime / (Time.unscaledDeltaTime + _damping);
        }
    }

    public class LinearPassFilter  : MonoBehaviour
    {
        private float _duration;
        private float _time;
        private float _trueValue;
        private float _refValue;

        public LinearPassFilter(float duration, float value = 0){
            _duration = duration;
            _trueValue = value;
            _refValue = value;
        }

        public void Input(float input){
            if(input != _refValue){
                _trueValue = Output();
                _time = Time.unscaledTime;
                _refValue = input;
            }
        }

        public float Output (){
            float offset = Time.unscaledTime - _time;
            float result = _trueValue;
            if (offset > _duration){
                _trueValue = _refValue;
            }
            else {
                result = _trueValue + (_refValue - _trueValue) * Mathf.Clamp(offset, 0, _duration) / _duration;
            }
            return result;
        }
    }
}
