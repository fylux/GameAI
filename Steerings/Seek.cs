using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : Steering {

    public Transform target;

    override
    public Vector3 Steer(Vector3 velocity)
    {

        var desiredVelocity = (target.transform.position - transform.position).normalized * MaxVelocity;

        if (visibleRays) drawRays(desiredVelocity);

        return (desiredVelocity - velocity);

    }

    private void drawRays(Vector3 dv) {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
    }
    /*
        private void seek3()
        {
            var desired_velocity = (target.transform.position - transform.position).normalized * MaxVelocity;
            var steering = desired_velocity - rigid.velocity;
            steering.y = 0;
            steering = Vector3.ClampMagnitude(steering, MaxForce);
            velocity += steering;
            rigid.velocity = transform.forward;//steering;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 5);

            Debug.DrawRay(transform.position, rigid.velocity.normalized * 2, Color.green);
            Debug.DrawRay(transform.position, desired_velocity.normalized * 2, Color.red);
        }

        private void seek2()
        {
            /* var desired_velocity = (target.transform.position - transform.position).normalized * MaxVelocity;
             var steering = desired_velocity - rigid.velocity;

             rigid.velocity = Vector3.ClampMagnitude(rigid.velocity + steering, MaxVelocity);

             transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rigid.velocity), Time.deltaTime * 5);

             Debug.DrawRay(transform.position, rigid.velocity.normalized * 2, Color.green);
            var desired_velocity = (target.transform.position - transform.position).normalized * MaxVelocity;
            var steering = desired_velocity - rigid.velocity;

            rigid.velocity = Vector3.ClampMagnitude(rigid.velocity + steering, MaxVelocity);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rigid.velocity), Time.deltaTime * 60);

            Debug.DrawRay(transform.position, rigid.velocity.normalized * 2, Color.green);
            Debug.DrawRay(transform.position, desired_velocity.normalized * 2, Color.red);
        }


        private void flee()
        {
            var desiredVelocity = transform.position - target.transform.position;
            desiredVelocity = desiredVelocity.normalized * MaxVelocity;

            var steering = desiredVelocity - velocity;
            steering = Vector3.ClampMagnitude(steering, MaxForce);

            velocity = Vector3.ClampMagnitude(velocity + steering, MaxVelocity);


            transform.position += transform.forward * Time.deltaTime;
            //  transform.position +=  velocity * Time.deltaTime;  //Para el modo de giro brusco


            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity.normalized), Time.deltaTime);

            var z = desiredVelocity.normalized * 2 - transform.forward * 2;
            Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
            Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.blue);
            Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
        }

        private void wander()
        {
            var velocity = transform.forward;
            velocity = velocity.normalized * MaxVelocity;


            Quaternion current = transform.rotation;
            Quaternion target = transform.rotation;
            if (Time.frameCount % wanderCooldown == 0)
            {
                var temp = transform;
                temp.Rotate(Vector3.up * Random.Range(180f, 180f));
                target = temp.rotation;
            }

            transform.rotation = Quaternion.Slerp(current, target, Time.deltaTime * 500);

            transform.position += velocity * Time.deltaTime;
            //transform.forward = velocity.normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 60);

            Debug.DrawRay(transform.position, velocity.normalized * 2, Color.green);
        }*/
}
