import { Behaviour, GameObject, Rigidbody, serializable } from "@needle-tools/engine";
import { KeyCode } from "@needle-tools/engine/engine/engine_input";
import { Mathf } from "@needle-tools/engine/engine/engine_math";
import { Vector3 } from "three"

export class Car extends Behaviour {

    @serializable(GameObject)
    frontAxis!: GameObject;

    @serializable(Rigidbody)
    wheels: Rigidbody[] = [];

    @serializable(Rigidbody)
    wheelsFront: Rigidbody[] = [];

    update() {

        if (this.context.input.isKeyPressed(KeyCode.KEY_W)) {
            for (const wheel of this.wheels) {
                const vel = wheel.getAngularVelocity();
                // apply on world X axis
                const axis = new Vector3(1,0,0);
                axis.applyQuaternion(wheel.gameObject.quaternion);
                vel.addScaledVector(axis, -20 * this.context.time.deltaTime);
                // vel.x -= 20 * this.context.time.deltaTime;

                wheel.setAngularVelocity(vel);
            }
        }
        else if(this.context.input.isKeyPressed(KeyCode.KEY_S)) {
            for (const wheel of this.wheels) {
                const vel = wheel.getAngularVelocity();
                // apply on world X axis
                const axis = new Vector3(1,0,0);
                axis.applyQuaternion(wheel.gameObject.quaternion);
                vel.addScaledVector(axis, 20 * this.context.time.deltaTime);
                // vel.x += 20 * this.context.time.deltaTime;

                wheel.setAngularVelocity(vel);
            }
        }
        else {
            for (const wheel of this.wheels) {
                const vel = wheel.getAngularVelocity();
                vel.multiplyScalar(.9);
                wheel.setAngularVelocity(vel);
            }
        }


        if(this.context.input.isKeyPressed(KeyCode.KEY_A)) {
            // for(const wheel of this.wheelsFront) {
            //     const go = wheel.gameObject;
            //     go.rotateOnWorldAxis(new Vector3(0,1,0), .3 * this.context.time.deltaTime);
            //     // const rot = wheel.gameObject.rotation;
            //     // rot.y = Mathf.lerp(rot.y, Math.PI * .1, this.context.time.deltaTime / .2);
            // }
            // const rot = this.frontAxis.rotation;
            // rot.y = Mathf.lerp(rot.y, Math.PI * .1, this.context.time.deltaTime / .2);
        }
        else if(this.context.input.isKeyPressed(KeyCode.KEY_D)) {
            // for(const wheel of this.wheelsFront) {
            //     const go = wheel.gameObject;
            //     go.rotateOnWorldAxis(new Vector3(0,1,0), -.3 * this.context.time.deltaTime);
            //     // const rot = wheel.gameObject.rotation;
            //     // rot.y = Mathf.lerp(rot.y, -Math.PI * .1, this.context.time.deltaTime / .2);
            // }
            // const rot = this.frontAxis.rotation;
            // rot.y = Mathf.lerp(rot.y, -Math.PI * .1, this.context.time.deltaTime / .2);
        }
    }

}