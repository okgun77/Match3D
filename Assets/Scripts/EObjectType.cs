using UnityEngine;

public class EObjectType : MonoBehaviour
{
    public enum ESelObj { NONE = -1, SELECTABLE, SEL_01, SEL_02, SEL_03, LENGTH }

    public ESelObj objectType = ESelObj.NONE;

    public Vector3 fixedRotation;

}
