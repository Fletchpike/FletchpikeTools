using UnityEngine;

namespace Fletchpike.Debug
{
    public class EditorTestScript : MonoBehaviour
    {
        public SingleRange floatRange = new(0, 1, 0, 1);
        public IntegerRange intRange = new(0, 10, 0, 10);
        public SingleSlider floatSlider = new(0, 1);
        public IntegerSlider intSlider = new(0, 10);
    }
}
