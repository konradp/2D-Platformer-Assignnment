using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallCheck : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    private void OnCollisionStay2D(Collision2D collision)
    {
        //this could take too much cpu
        if (collision.gameObject.CompareTag("Ground"))
            playerController.ChangeWallStatus(true);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            playerController.ChangeWallStatus(false);
    }
}
