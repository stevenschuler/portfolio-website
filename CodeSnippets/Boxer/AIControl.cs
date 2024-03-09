using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class AIControl : MonoBehaviour
{

    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject midSpine;

    float midx = 1.21f;
    float midz = 7.8f;
    public float factor = 0.4f;

    float basePower = 100;

    // Randomly change position of boxer
    public void change_pos()
    {
        float newx = Random.Range(midx - 1.5f, midx + 1.5f);
        float newy = 0f;
        float newz = midz - Mathf.Abs(factor * (midx - newx));
        gameObject.GetComponent<Transform>().transform.localPosition = new Vector3(newx, newy, newz);
    }


    /* After much experimentation, I decided to discritise my boxer's movement.
     * This means that at every decision step, the agent will choose which body part it wants to move and by how much.
     */

    // Set new target spine rotation
    public void set_spine(float t, float l)
    {
        midSpine.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Euler(l * 45f, t * 45f, 0f);
    }

    public void left_hand_out(float handPower)
    {
       handPower = basePower + (300 * handPower);
       leftHand.GetComponent<Rigidbody>().AddRelativeForce(handPower * Vector3.forward);
    }
    public void left_hand_down(float handPower)
    {
        handPower = basePower + (300 * handPower);
        leftHand.GetComponent<Rigidbody>().AddRelativeForce(handPower * Vector3.down);
    }
    public void left_hand_up(float handPower)
    {
        handPower = basePower + (300 * handPower);
        leftHand.GetComponent<Rigidbody>().AddRelativeForce(handPower * Vector3.up);
    }

    public void right_hand_out(float handPower)
    {
        handPower = basePower + (300 * handPower);
        rightHand.GetComponent<Rigidbody>().AddRelativeForce(-handPower * Vector3.forward);
    }
    public void right_hand_down(float handPower)
    {
        handPower = basePower + (300 * handPower);
        rightHand.GetComponent<Rigidbody>().AddRelativeForce(-handPower * Vector3.down);
    }
    public void right_hand_up(float handPower)
    {
        handPower = basePower + (300 * handPower);
        rightHand.GetComponent<Rigidbody>().AddRelativeForce(-handPower * Vector3.up);
    }
}
