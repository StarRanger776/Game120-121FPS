using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class MatchPosRot : MonoBehaviour
{
    public GameObject objectToRotateWith;
    public GameObject objectToMoveWith;

    [Header("Match Rotation")]
    public bool rotX;
    public bool rotY;
    public bool rotZ;
    public Quaternion rotOffset;

    [Header("Match Position")]
    public bool posX;
    public bool posY;
    public bool posZ;
    public Vector3 posOffset;

    void LateUpdate()
    {
        if (objectToRotateWith != null)
        {
            if (rotX && rotY && rotZ) // x, y, z
                this.gameObject.transform.rotation = new Quaternion(objectToRotateWith.transform.rotation.x + rotOffset.x, objectToRotateWith.transform.rotation.y + rotOffset.y, objectToRotateWith.transform.rotation.z + rotOffset.z, objectToRotateWith.transform.rotation.w + rotOffset.w);
            else if (rotX && rotY && !rotZ) // x, y
                this.gameObject.transform.rotation = new Quaternion(objectToRotateWith.transform.rotation.x + rotOffset.x, objectToRotateWith.transform.rotation.y + rotOffset.y, this.gameObject.transform.rotation.z, objectToRotateWith.transform.rotation.w + rotOffset.w);
            else if (rotX && !rotY && rotZ) // x, z
                this.gameObject.transform.rotation = new Quaternion(objectToRotateWith.transform.rotation.x + rotOffset.x, this.gameObject.transform.rotation.y, objectToRotateWith.transform.rotation.z + rotOffset.z, objectToRotateWith.transform.rotation.w + rotOffset.w);
            else if (rotX && !rotY && !rotZ) // x
                this.gameObject.transform.rotation = new Quaternion(objectToRotateWith.transform.rotation.x + rotOffset.x, this.gameObject.transform.rotation.y, this.gameObject.transform.rotation.z, objectToRotateWith.transform.rotation.w + rotOffset.w);
            else if (!rotX && rotY && rotZ) // y, z
                this.gameObject.transform.rotation = new Quaternion(this.gameObject.transform.rotation.x, objectToRotateWith.transform.rotation.y + rotOffset.y, objectToRotateWith.transform.rotation.z + rotOffset.z, objectToRotateWith.transform.rotation.w + rotOffset.w);
            else if (!rotX && rotY && !rotZ) // y
                this.gameObject.transform.rotation = new Quaternion(this.gameObject.transform.rotation.x, objectToRotateWith.transform.rotation.y + rotOffset.y, this.gameObject.transform.rotation.z, objectToRotateWith.transform.rotation.w + rotOffset.w);
            else if (!rotX && !rotY && rotZ) // z
                this.gameObject.transform.rotation = new Quaternion(this.gameObject.transform.rotation.x, this.gameObject.transform.rotation.y, objectToRotateWith.transform.rotation.z + rotOffset.z, objectToRotateWith.transform.rotation.w + rotOffset.w);
        }

        if (objectToMoveWith != null)
        {
            if (posX && posY && posZ) // x, y, z
                this.gameObject.transform.position = new Vector3(objectToMoveWith.transform.position.x + posOffset.x, objectToMoveWith.transform.position.y + posOffset.y, objectToMoveWith.transform.position.z + posOffset.z);
            else if (posX && posY && !posZ) // x, y
                this.gameObject.transform.position = new Vector3(objectToMoveWith.transform.position.x + posOffset.x, objectToMoveWith.transform.position.y + posOffset.y, this.gameObject.transform.position.z);
            else if (posX && !posY && posZ) // x, z
                this.gameObject.transform.position = new Vector3(objectToMoveWith.transform.position.x + posOffset.x, this.gameObject.transform.position.y, objectToMoveWith.transform.position.z + posOffset.z);
            else if (posX && !posY && !posZ) // x
                this.gameObject.transform.position = new Vector3(objectToMoveWith.transform.position.x + posOffset.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
            else if (!posX && posY && posZ) // y, z
                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, objectToMoveWith.transform.position.y + posOffset.y, objectToMoveWith.transform.position.z + posOffset.z);
            else if (!posX && posY && !posZ) // y
                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, gameObject.transform.position.y + posOffset.y, this.gameObject.transform.position.z);
            else if (!posX && !posY && posZ) // z
                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, objectToMoveWith.transform.position.z + posOffset.z);
        }
    }
}
